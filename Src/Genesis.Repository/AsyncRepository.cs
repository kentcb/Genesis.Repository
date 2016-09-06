namespace Genesis.Repository
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Reactive.Concurrency;
    using System.Reactive.Linq;
    using System.Reactive.Subjects;
    using Genesis.Ensure;

    /// <summary>
    /// Provides asynchronous semantics around an <see cref="IRepository{TId, TEntity}"/> instance.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Instances of this class can be used to wrap an <see cref="IRepository{TId, TEntity}"/> implementation with asynchronous and thread-safe behavior.
    /// That is, the caller is not blocked waiting on database operations, and those operations are performed on a dedicated scheduler.
    /// </para>
    /// </remarks>
    /// <typeparam name="TId">
    /// The entity's ID type.
    /// </typeparam>
    /// <typeparam name="TEntity">
    /// The entity type.
    /// </typeparam>
    public class AsyncRepository<TId, TEntity>
        where TId : struct
        where TEntity : IEntity<TId>
    {
        private readonly IRepository<TId, TEntity> repository;
        private readonly IScheduler dataStoreScheduler;
        private readonly ISubject<TEntity, TEntity> addedItemsSynchronized;
        private readonly ISubject<TEntity, TEntity> updatedItemsSynchronized;
        private readonly ISubject<TEntity, TEntity> removedItemsSynchronized;

        /// <summary>
        /// Creates a new instance of this class.
        /// </summary>
        /// <param name="repository">
        /// The <see cref="IRepository{TId, TEntity}"/> to wrap.
        /// </param>
        /// <param name="dataStoreScheduler">
        /// The scheduler on which all operations will be performed.
        /// </param>
        public AsyncRepository(
            IRepository<TId, TEntity> repository,
            IScheduler dataStoreScheduler)
        {
            Ensure.ArgumentNotNull(repository, nameof(repository));
            Ensure.ArgumentNotNull(dataStoreScheduler, nameof(dataStoreScheduler));

            this.repository = repository;
            this.dataStoreScheduler = dataStoreScheduler;
            this.addedItemsSynchronized = Subject.Synchronize(new Subject<TEntity>());
            this.updatedItemsSynchronized = Subject.Synchronize(new Subject<TEntity>());
            this.removedItemsSynchronized = Subject.Synchronize(new Subject<TEntity>());
        }

        /// <summary>
        /// Gets the wrapped repository.
        /// </summary>
        public IRepository<TId, TEntity> Repository => this.repository;

        /// <summary>
        /// Gets the scheduler being used for all operations.
        /// </summary>
        public IScheduler DataStoreScheduler => this.dataStoreScheduler;

        /// <summary>
        /// Gets a stream of all items added to the repository from this point forwards.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Note that calling <see cref="DeleteAll"/> will not tick anything through this observable.
        /// </para>
        /// </remarks>
        public IObservable<ItemChange<TEntity>> FutureItems =>
            Observable
                .Merge(
                    this.addedItemsSynchronized.Select(addedEntity => ItemChange.CreateAdd(addedEntity)),
                    this.updatedItemsSynchronized.Select(updatedEntity => ItemChange.CreateUpdate(updatedEntity)),
                    this.removedItemsSynchronized.Select(removedEntity => ItemChange.CreateRemove(removedEntity)));

        /// <summary>
        /// Gets a stream of all items added to the repository, including those that already exist.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Note that calling <see cref="DeleteAll"/> will not tick anything through this observable.
        /// </para>
        /// </remarks>
        public IObservable<ItemChange<TEntity>> Items =>
            Observable
                .Defer(
                    () =>
                        Observable
                            .Start(
                                () =>
                                {
                                    var existingEntities = this.repository.GetAll();

                                    return existingEntities
                                        .ToObservable()
                                        .Select(existingEntity => ItemChange.CreateAdd(existingEntity))
                                        .Concat(this.FutureItems);
                                },
                                this.dataStoreScheduler))
                .Switch();

        /// <summary>
        /// Gets the entity with the given ID.
        /// </summary>
        /// <param name="id">
        /// The ID of the entity to retrieve.
        /// </param>
        /// <returns>
        /// An observable that ticks the entity once retrieved, or <see langword="null"/> if the entity was not found.
        /// </returns>
        public IObservable<TEntity> Get(TId id) =>
            Observable
                .Defer(
                    () =>
                        Observable
                            .Start(
                                () => this.repository.Get(id),
                                this.dataStoreScheduler));

        /// <summary>
        /// Gets all entities.
        /// </summary>
        /// <returns>
        /// An observable that ticks all entities in the repository.
        /// </returns>
        public IObservable<IImmutableList<TEntity>> GetAll() =>
            Observable
                .Defer(
                    () =>
                        Observable
                            .Start(
                                () => this.repository.GetAll(),
                                this.dataStoreScheduler));

        /// <summary>
        /// Saves the given entity.
        /// </summary>
        /// <param name="entity">
        /// The entity to save.
        /// </param>
        /// <returns>
        /// An observable that ticks the saved entity, which may differ from <paramref name="entity"/> (such as an updated ID).
        /// </returns>
        public IObservable<TEntity> Save(TEntity entity)
        {
            Ensure.GenericArgumentNotNull(entity, nameof(entity));

            return Observable
                .Defer(
                    () =>
                        Observable
                            .Start(
                                () => this
                                    .repository
                                    .Save(entity),
                                this.dataStoreScheduler))
                .Do(
                    savedEntity =>
                    {
                        if (!entity.Id.HasValue)
                        {
                            this.addedItemsSynchronized.OnNext(savedEntity);
                        }
                        else
                        {
                            this.updatedItemsSynchronized.OnNext(savedEntity);
                        }
                    });
        }

        /// <summary>
        /// Saves all the given entities.
        /// </summary>
        /// <param name="entities">
        /// The entities to save.
        /// </param>
        /// <returns>
        /// An observable that ticks the saved entities, which may differ from <paramref name="entities"/> (such as updated IDs).
        /// </returns>
        public IObservable<IImmutableList<TEntity>> SaveAll(IEnumerable<TEntity> entities)
        {
            Ensure.ArgumentNotNull(entities, nameof(entities), assertContentsNotNull: true);
            var entitiesList = entities.ToImmutableList();

            return Observable
                .Defer(
                    () =>
                        Observable
                            .Start(
                                () => this
                                    .repository
                                    .SaveAll(entitiesList),
                            this.dataStoreScheduler))
                .Do(
                    savedEntities =>
                    {
                        for (var i = 0; i < entitiesList.Count; ++i)
                        {
                            if (!entitiesList[i].Id.HasValue)
                            {
                                this.addedItemsSynchronized.OnNext(savedEntities[i]);
                            }
                            else
                            {
                                this.updatedItemsSynchronized.OnNext(savedEntities[i]);
                            }
                        }
                    });
        }

        /// <summary>
        /// Saves all the given entities.
        /// </summary>
        /// <param name="entities">
        /// The entities to save.
        /// </param>
        /// <returns>
        /// An observable that ticks the saved entities, which may differ from <paramref name="entities"/> (such as updated IDs).
        /// </returns>
        public IObservable<IImmutableList<TEntity>> SaveAll(params TEntity[] entities) =>
            this.SaveAll((IEnumerable<TEntity>)entities);

        /// <summary>
        /// Deletes the entity with the specified ID.
        /// </summary>
        /// <param name="id">
        /// The ID of the entity to delete.
        /// </param>
        /// <returns>
        /// An observable that ticks <see langword="true"/> if the entity was deleted, otherwise <see langword="false"/>.
        /// </returns>
        public IObservable<bool> Delete(TId id) =>
            Observable
                .Defer(
                    () =>
                        Observable
                            .Start(
                                () =>
                                {
                                    var existingEntity = this.repository.Get(id);

                                    if (existingEntity != null)
                                    {
                                        return new { Entity = existingEntity, Deleted = this.repository.Delete(id) };
                                    }

                                    return new { Entity = existingEntity, Deleted = false };
                                },
                                this.dataStoreScheduler))
                .Do(
                    info =>
                    {
                        if (info.Entity != null)
                        {
                            this.removedItemsSynchronized.OnNext(info.Entity);
                        }
                    })
                .Select(info => info.Deleted);

        /// <summary>
        /// Deletes all entities.
        /// </summary>
        /// <returns>
        /// An observable that ticks the number of entities deleted.
        /// </returns>
        public IObservable<int> DeleteAll() =>
            Observable
                .Defer(
                    () =>
                        Observable
                            .Start(
                                () => this.repository.DeleteAll(),
                                this.dataStoreScheduler));
    }
}