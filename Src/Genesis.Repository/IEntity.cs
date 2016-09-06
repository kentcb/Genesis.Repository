namespace Genesis.Repository
{
    /// <summary>
    /// Represents a persisted entity.
    /// </summary>
    /// <typeparam name="TId">
    /// The entity's ID type.
    /// </typeparam>
    public interface IEntity<TId>
        where TId : struct
    {
        /// <summary>
        /// Gets the ID of the entity, or <see langword="null"/> if no ID is yet assigned.
        /// </summary>
        TId? Id
        {
            get;
        }
    }
}