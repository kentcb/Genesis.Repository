namespace Genesis.Repository
{
    using System;

    /// <summary>
    /// Provides static construction methods for instances of <see cref="ItemChange{T}"/>.
    /// </summary>
    public static class ItemChange
    {
        /// <summary>
        /// Creates an <see cref="ItemChange{T}"/> with a type of <see cref="ItemChangeType.Add"/>.
        /// </summary>
        /// <typeparam name="T">
        /// The item type.
        /// </typeparam>
        /// <param name="item">
        /// The item.
        /// </param>
        /// <returns>
        /// The <see cref="ItemChange{T}"/> instance.
        /// </returns>
        public static ItemChange<T> CreateAdd<T>(T item) =>
            new ItemChange<T>(ItemChangeType.Add, item);

        /// <summary>
        /// Creates an <see cref="ItemChange{T}"/> with a type of <see cref="ItemChangeType.Remove"/>.
        /// </summary>
        /// <typeparam name="T">
        /// The item type.
        /// </typeparam>
        /// <param name="item">
        /// The item.
        /// </param>
        /// <returns>
        /// The <see cref="ItemChange{T}"/> instance.
        /// </returns>
        public static ItemChange<T> CreateRemove<T>(T item) =>
            new ItemChange<T>(ItemChangeType.Remove, item);

        /// <summary>
        /// Creates an <see cref="ItemChange{T}"/> with a type of <see cref="ItemChangeType.Update"/>.
        /// </summary>
        /// <typeparam name="T">
        /// The item type.
        /// </typeparam>
        /// <param name="item">
        /// The item.
        /// </param>
        /// <returns>
        /// The <see cref="ItemChange{T}"/> instance.
        /// </returns>
        public static ItemChange<T> CreateUpdate<T>(T item) =>
           new ItemChange<T>(ItemChangeType.Update, item);
    }

    /// <summary>
    /// Represents a change to an item.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the item.
    /// </typeparam>
    public class ItemChange<T> : IEquatable<ItemChange<T>>
    {
        private readonly ItemChangeType type;
        private readonly T item;

        internal ItemChange(ItemChangeType type, T item)
        {
            this.type = type;
            this.item = item;
        }

        /// <summary>
        /// Gets the type of change.
        /// </summary>
        public ItemChangeType Type => this.type;

        /// <summary>
        /// Gets the item that was changed.
        /// </summary>
        public T Item => this.item;

        /// <summary>
        /// Creates a new <see cref="ItemChange{T}"/> with the same <see cref="Type"/>, but with the specified item.
        /// </summary>
        /// <typeparam name="U">
        /// The type of the new item.
        /// </typeparam>
        /// <param name="item">
        /// The new item.
        /// </param>
        /// <returns>
        /// The <see cref="ItemChange{T}"/> instance.
        /// </returns>
        public ItemChange<U> WithItem<U>(U item) =>
            new ItemChange<U>(this.type, item);

        /// <inheritdoc/>
        public bool Equals(ItemChange<T> other)
        {
            if (other == null)
            {
                return false;
            }

            if (other == this)
            {
                return true;
            }

            if (this.type != other.type)
            {
                return false;
            }

            if (this.item == null)
            {
                return other.item == null;
            }

            return this.item.Equals(other.item);
        }

        /// <inheritdoc/>
        public override bool Equals(object obj) =>
            this.Equals(obj as ItemChange<T>);

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            var hash = 17;

            hash = (hash * 23) + this.type.GetHashCode();
            hash = (hash * 23) + (this.item == null ? 0 : this.item.GetHashCode());

            return hash;
        }
    }
}