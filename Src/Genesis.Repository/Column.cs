namespace Genesis.Repository
{
    using Genesis.Ensure;

    /// <summary>
    /// Represents a column.
    /// </summary>
    public sealed class Column
    {
        private readonly string name;
        private readonly bool isPrimaryKey;
        private readonly SortOrder? sortOrder;

        /// <summary>
        /// Creates a new instance of this type.
        /// </summary>
        /// <param name="name">
        /// The column's name.
        /// </param>
        /// <param name="isPrimaryKey">
        /// <see langword="true"/> if the column comprises part of the primary key, otherwise <see langword="false"/>.
        /// </param>
        /// <param name="sortOrder">
        /// The sort order for the column, if any.
        /// </param>
        public Column(
            string name,
            bool isPrimaryKey = false,
            SortOrder? sortOrder = null)
        {
            Ensure.ArgumentNotNull(name, nameof(name));

            this.name = name;
            this.isPrimaryKey = isPrimaryKey;
            this.sortOrder = sortOrder;
        }

        /// <summary>
        /// Gets the column's name.
        /// </summary>
        public string Name => this.name;

        /// <summary>
        /// Gets a value indicating whether the column comprises part of the primary key.
        /// </summary>
        public bool IsPrimaryKey => this.isPrimaryKey;

        /// <summary>
        /// Gets the column's sort order, if any.
        /// </summary>
        public SortOrder? SortOrder => this.sortOrder;
    }
}