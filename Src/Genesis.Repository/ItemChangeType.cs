namespace Genesis.Repository
{
    /// <summary>
    /// Defines the possible change types for an <see cref="ItemChange{T}"/>.
    /// </summary>
    public enum ItemChangeType
    {
        /// <summary>
        /// The item was added.
        /// </summary>
        Add,

        /// <summary>
        /// The item was updated.
        /// </summary>
        Update,

        /// <summary>
        /// The item was removed.
        /// </summary>
        Remove
    }
}