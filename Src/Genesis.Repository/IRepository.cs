namespace Genesis.Repository
{
    using System.Collections.Generic;
    using System.Collections.Immutable;

    /// <summary>
    /// Defines the behavior of a repository, which is capable of storing and retrieving entities of a specific type.
    /// </summary>
    /// <typeparam name="TId">
    /// The entity's ID type.
    /// </typeparam>
    /// <typeparam name="TEntity">
    /// The entity type.
    /// </typeparam>
    public interface IRepository<TId, TEntity>
    {
        /// <summary>
        /// Gets the entity with the given ID from this repository.
        /// </summary>
        /// <param name="id">
        /// The ID of the entity to retrieve.
        /// </param>
        /// <returns>
        /// The entity, or <see langword="null"/> if no such entity exists.
        /// </returns>
        TEntity Get(TId id);

        /// <summary>
        /// Gets all entities in this repository.
        /// </summary>
        /// <returns>
        /// All entities.
        /// </returns>
        IImmutableList<TEntity> GetAll();

        /// <summary>
        /// Saves the given entity to this repository.
        /// </summary>
        /// <param name="entity">
        /// The entity to save.
        /// </param>
        /// <returns>
        /// The saved entity, which may differ from <paramref name="entity"/> (such as an updated ID).
        /// </returns>
        TEntity Save(TEntity entity);

        /// <summary>
        /// Saves all the given entities to this repository.
        /// </summary>
        /// <param name="entities">
        /// The entities to save.
        /// </param>
        /// <returns>
        /// The saved entities, which may differ from <paramref name="entities"/> (such as updated IDs).
        /// </returns>
        IImmutableList<TEntity> SaveAll(IEnumerable<TEntity> entities);

        /// <summary>
        /// Saves all the given entities to this repository.
        /// </summary>
        /// <param name="entities">
        /// The entities to save.
        /// </param>
        /// <returns>
        /// The saved entities, which may differ from <paramref name="entities"/> (such as updating IDs).
        /// </returns>
        IImmutableList<TEntity> SaveAll(params TEntity[] entities);

        /// <summary>
        /// Deletes the entity with the specified ID from this repository.
        /// </summary>
        /// <param name="id">
        /// The ID of the entity to delete.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the entity was deleted, otherwise <see langword="false"/>.
        /// </returns>
        bool Delete(TId id);

        /// <summary>
        /// Deletes all entities in this repository.
        /// </summary>
        /// <returns>
        /// The number of entities deleted.
        /// </returns>
        int DeleteAll();
    }
}