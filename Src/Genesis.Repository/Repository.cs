namespace Genesis.Repository
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using Genesis.Ensure;
    using SQLitePCL.pretty;

    /// <summary>
    /// Serves as a useful, though optional, base class for <see cref="IRepository{TId, TEntity}"/> implementations.
    /// </summary>
    /// <remarks>
    /// <para>
    /// All database operations instigated by this class are performed on the caller's thread. Generally you won't want to use a
    /// repository directly, but would instead wrap it in an <see cref="AsyncRepository{TId, TEntity}"/>.
    /// </para>
    /// </remarks>
    /// <typeparam name="TId">
    /// The entity's ID type.
    /// </typeparam>
    /// <typeparam name="TEntity">
    /// The entity type.
    /// </typeparam>
    public abstract class Repository<TId, TEntity> : IRepository<TId, TEntity>
        where TId : struct
        where TEntity : class, IEntity<TId>
    {
        private readonly IDatabaseConnection connection;
        private readonly Lazy<IStatement> getStatement;
        private readonly Lazy<IStatement> getAllStatement;
        private readonly Lazy<IStatement> saveStatement;
        private readonly Lazy<IStatement> deleteStatement;
        private readonly Lazy<IStatement> deleteAllStatement;

        /// <summary>
        /// Creates a new instance of this class.
        /// </summary>
        /// <param name="connection">
        /// The database connection.
        /// </param>
        protected Repository(IDatabaseConnection connection)
        {
            Ensure.ArgumentNotNull(connection, nameof(connection));

            this.connection = connection;
            this.getStatement = new Lazy<IStatement>(() => this.connection.PrepareStatement(this.GetSql));
            this.getAllStatement = new Lazy<IStatement>(() => this.connection.PrepareStatement(this.GetAllSql));
            this.saveStatement = new Lazy<IStatement>(() => this.connection.PrepareStatement(this.SaveSql));
            this.deleteStatement = new Lazy<IStatement>(() => this.connection.PrepareStatement(this.DeleteSql));
            this.deleteAllStatement = new Lazy<IStatement>(() => this.connection.PrepareStatement(this.DeleteAllSql));
        }

        /// <summary>
        /// Gets the database connection.
        /// </summary>
        public IDatabaseConnection Connection => this.connection;

        /// <summary>
        /// Gets the name of the database table in which entities for this repository are stored.
        /// </summary>
        protected abstract string TableName
        {
            get;
        }

        /// <summary>
        /// Gets the columns comprising the database table in which entities for this repository are stored.
        /// </summary>
        protected abstract IEnumerable<Column> Columns
        {
            get;
        }

        /// <summary>
        /// Gets the names of all columns comprising the database table in which entities for this repository are stored.
        /// </summary>
        protected IEnumerable<string> ColumnNames =>
            this
                .Columns
                .Select(column => column.Name);

        /// <summary>
        /// Gets the names of all columns delimited by a comma.
        /// </summary>
        protected string DelimitedColumns =>
            this
                .ColumnNames
                .Join(",");

        /// <summary>
        /// Gets a SQL predicate comprised of all ID columns joined with <c>AND</c>.
        /// </summary>
        /// <remarks>
        /// <para>
        /// For example, <c>customer_id = ? AND order_id = ? AND package_id = ?</c>
        /// </para>
        /// </remarks>
        protected string DelimitedIdColumnPredicate =>
            this
                .Columns
                .Where(column => column.IsPrimaryKey)
                .Select(idColumn => idColumn.Name + "=?")
                .Join(" AND ");

        /// <summary>
        /// Gets a SQL substring comprised of all sorted columns.
        /// </summary>
        /// <remarks>
        /// <para>
        /// For example, <c>timestamp DESC, name ASC</c>
        /// </para>
        /// </remarks>
        protected string DelimitedDefaultOrder =>
            this
                .Columns
                .Where(column => column.SortOrder != null)
                .Select(sortedColumn => sortedColumn.Name + " " + (sortedColumn.SortOrder == SortOrder.Ascending ? "ASC" : "DESC"))
                .Join(",");

        /// <summary>
        /// Converts a result set into an entity.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Subclasses must implement this method such that an entity is created and populated from the given result set. Generally, this involves
        /// extracting values from the result set in the same order as specified by <see cref="Columns"/>, then passing those values into an entity
        /// implementation. To extract values, you would typically use the methods and extension methods defined against <see cref="IResultSetValue"/>.
        /// </para>
        /// </remarks>
        /// <param name="resultSet">
        /// The result set.
        /// </param>
        /// <returns>
        /// An entity whose values are populated from the result set.
        /// </returns>
        protected abstract TEntity ValuesToEntity(IReadOnlyList<IResultSetValue> resultSet);

        /// <summary>
        /// Converts an entity into a set of values.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Subclasses must implement this method such that an entity is converted into a set of values for storing in the repository. Generally, this
        /// involves extracting each stored value from the entity in the same order as specified by <see cref="Columns"/>, possibly converting types
        /// as necessary.
        /// </para>
        /// </remarks>
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <returns>
        /// The values
        /// </returns>
        protected abstract IEnumerable<object> EntityToValues(TEntity entity);

        /// <summary>
        /// Gets the SQL used to retrieve an entity.
        /// </summary>
        protected virtual string GetSql => $"SELECT {DelimitedColumns} FROM {this.TableName} WHERE {this.DelimitedIdColumnPredicate}";

        /// <summary>
        /// Gets the SQL used to retrieve all entities.
        /// </summary>
        protected virtual string GetAllSql => $"SELECT {DelimitedColumns} FROM {this.TableName} ORDER BY {this.DelimitedDefaultOrder}";

        /// <summary>
        /// Gets the SQL used to save an entity.
        /// </summary>
        protected virtual string SaveSql => $"INSERT OR REPLACE INTO {this.TableName}({DelimitedColumns}) VALUES ({this.ColumnNames.Select(_ => "?").Join(",")})";

        /// <summary>
        /// Gets the SQL used to delete an entity.
        /// </summary>
        protected virtual string DeleteSql => $"DELETE FROM {this.TableName} WHERE {this.DelimitedIdColumnPredicate}";

        /// <summary>
        /// Gets the SQL used to delete all entities.
        /// </summary>
        protected virtual string DeleteAllSql => $"DELETE FROM {this.TableName} WHERE 1";

        /// <inheritdoc/>
        public TEntity Get(TId id) =>
            this
                .connection
                .EnsureRunInTransaction(
                    _ =>
                    {
                        var args = this.FlattenId(id);
                        var resultSet = this
                            .getStatement
                            .Value
                            .Query(args)
                            .FirstOrDefault();

                        if (resultSet == null)
                        {
                            return null;
                        }

                        var entity = this.ValuesToEntity(resultSet);

                        return this.OnEntityLoaded(entity, this.connection);
                    });

        /// <inheritdoc/>
        public IImmutableList<TEntity> GetAll() =>
            this
                .connection
                .EnsureRunInTransaction(
                    _ =>
                        this
                            .getAllStatement
                            .Value
                            .Query()
                            .Select(resultSet => this.ValuesToEntity(resultSet))
                            .Select(entity => this.OnEntityLoaded(entity, this.connection))
                            .ToImmutableList());

        /// <inheritdoc/>
        public TEntity Save(TEntity entity)
        {
            Ensure.ArgumentNotNull(entity, nameof(entity));

            return this
                .connection
                .EnsureRunInTransaction(
                    _ =>
                    {
                        entity = this.OnEntitySaving(entity, this.connection);

                        var columnValues = this.EntityToValues(entity);
                        this
                            .saveStatement
                            .Value
                            .Execute(columnValues.ToArray());

                        var result = this.OnEntitySaved(entity, this.connection);

                        return result;
                    });
        }

        /// <inheritdoc/>
        public IImmutableList<TEntity> SaveAll(IEnumerable<TEntity> entities)
        {
            Ensure.ArgumentNotNull(entities, nameof(entities), assertContentsNotNull: true);

            return this
                .connection
                .EnsureRunInTransaction(
                    _ =>
                    {
                        var entitiesList = entities.ToList();
                        var savedEntities = new TEntity[entitiesList.Count];

                        for (var i = 0; i < entitiesList.Count; ++i)
                        {
                            savedEntities[i] = this.Save(entitiesList[i]);
                        }

                        return savedEntities.ToImmutableList();
                    });
        }

        /// <inheritdoc/>
        public IImmutableList<TEntity> SaveAll(params TEntity[] entities) =>
            this.SaveAll((IEnumerable<TEntity>)entities);

        /// <inheritdoc/>
        public bool Delete(TId id) =>
            this
                .connection
                .EnsureRunInTransaction(
                    _ =>
                    {
                        this
                            .deleteStatement
                            .Value
                            .Execute(id);

                        var deleted = this.connection.Changes > 0;

                        if (deleted)
                        {
                            this.OnEntityDeleted(id, this.connection);
                        }

                        return deleted;
                    });

        /// <inheritdoc/>
        public int DeleteAll() =>
            this
                .connection
                .EnsureRunInTransaction(
                    _ =>
                    {
                        this
                            .deleteAllStatement
                            .Value
                            .Execute();

                        return this
                            .connection
                            .Changes;
                    });

        /// <summary>
        /// Flattens the given ID for storage in the repository.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Subclasses can override this method if the ID type is a composite of values, each stored in a separate column. By default this method returns
        /// an array containing only <paramref name="id"/>.
        /// </para>
        /// </remarks>
        /// <param name="id">
        /// The ID to flatten.
        /// </param>
        /// <returns>
        /// An array containing all values comprising the ID.
        /// </returns>
        protected virtual object[] FlattenId(TId id) =>
            new object[] { id };

        /// <summary>
        /// Called when an entity is loaded.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Subclasses can override this method to modify an entity loaded from the database. By default, this method simply returns <paramref name="entity"/>.
        /// </para>
        /// </remarks>
        /// <param name="entity">
        /// The entity that was loaded.
        /// </param>
        /// <param name="connection">
        /// The database connection via which it was loaded.
        /// </param>
        /// <returns>
        /// The modified entity.
        /// </returns>
        protected virtual TEntity OnEntityLoaded(TEntity entity, IDatabaseConnection connection) =>
            entity;

        /// <summary>
        /// Called when an entity is about to be saved.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Subclasses can override this method to make adjustments to the entity prior to it being saved, such as updating a timestamp. By default, this
        /// method simply returns the existing entity.
        /// </para>
        /// </remarks>
        /// <param name="entity">
        /// The entity that will be saved.
        /// </param>
        /// <param name="connection">
        /// The database connection with which the entity is being saved.
        /// </param>
        /// <returns>
        /// The entity to save.
        /// </returns>
        protected virtual TEntity OnEntitySaving(TEntity entity, IDatabaseConnection connection) =>
            entity;

        /// <summary>
        /// Called when an entity has been saved.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Subclasses must override this method to make adjustments to the entity after it is saved. Typically this will entail updating the ID of the entity
        /// from the database connection.
        /// </para>
        /// </remarks>
        /// <param name="entity">
        /// The entity that will be saved.
        /// </param>
        /// <param name="connection">
        /// The database connection with which the entity is being saved.
        /// </param>
        /// <returns>
        /// The entity to pass back to callers.
        /// </returns>
        protected abstract TEntity OnEntitySaved(TEntity entity, IDatabaseConnection connection);

        /// <summary>
        /// Called when an entity has been deleted via the <see cref="Delete(TId)"/> method.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Subclasses can override this method to perform any necessary logic. However, note that this method is only called when an entity is deleted via the
        /// <see cref="Delete(TId)"/> method, not via <see cref="DeleteAll"/>.
        /// </para>
        /// </remarks>
        /// <param name="id">
        /// The ID of the deleted entity.
        /// </param>
        /// <param name="connection">
        /// The database connection with which the entity is being saved.
        /// </param>
        protected virtual void OnEntityDeleted(TId id, IDatabaseConnection connection)
        {
        }
    }
}