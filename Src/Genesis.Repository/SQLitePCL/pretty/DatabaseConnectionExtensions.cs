namespace SQLitePCL.pretty
{
    using System;
    using Genesis.Ensure;

    /// <summary>
    /// Defines extension method against <see cref="IDatabaseConnection"/>.
    /// </summary>
    public static class DatabaseConnectionExtensions
    {
        /// <summary>
        /// Ensures the specified action runs in a transaction, starting a new transaction as required.
        /// </summary>
        /// <param name="this">
        /// The database connection.
        /// </param>
        /// <param name="action">
        /// The action.
        /// </param>
        public static void EnsureRunInTransaction(this IDatabaseConnection @this, Action<IDatabaseConnection> action)
        {
            Ensure.ArgumentNotNull(@this, nameof(@this));
            Ensure.ArgumentNotNull(action, nameof(action));

            if (@this.IsAutoCommit)
            {
                @this.RunInTransaction(action);
            }
            else
            {
                action(@this);
            }
        }

        /// <summary>
        /// Ensures the specified action runs in a transaction, starting a new transaction as required.
        /// </summary>
        /// <param name="this">
        /// The database connection.
        /// </param>
        /// <param name="action">
        /// The action.
        /// </param>
        /// <param name="transactionMode">
        /// The mode for the transaction.
        /// </param>
        public static void EnsureRunInTransaction(this IDatabaseConnection @this, Action<IDatabaseConnection> action, TransactionMode transactionMode)
        {
            Ensure.ArgumentNotNull(@this, nameof(@this));
            Ensure.ArgumentNotNull(action, nameof(action));

            if (@this.IsAutoCommit)
            {
                @this.RunInTransaction(action, transactionMode);
            }
            else
            {
                action(@this);
            }
        }

        /// <summary>
        /// Ensures the specified function runs in a transaction, starting a new transaction as required.
        /// </summary>
        /// <param name="this">
        /// The database connection.
        /// </param>
        /// <param name="f">
        /// The function.
        /// </param>
        public static T EnsureRunInTransaction<T>(this IDatabaseConnection @this, Func<IDatabaseConnection, T> f)
        {
            Ensure.ArgumentNotNull(@this, nameof(@this));
            Ensure.ArgumentNotNull(f, nameof(f));

            if (@this.IsAutoCommit)
            {
                return @this.RunInTransaction(f);
            }
            else
            {
                return f(@this);
            }
        }

        /// <summary>
        /// Ensures the specified function runs in a transaction, starting a new transaction as required.
        /// </summary>
        /// <param name="this">
        /// The database connection.
        /// </param>
        /// <param name="f">
        /// The function.
        /// </param>
        /// <param name="transactionMode">
        /// The mode for the transaction.
        /// </param>
        public static T EnsureRunInTransaction<T>(this IDatabaseConnection @this, Func<IDatabaseConnection, T> f, TransactionMode transactionMode)
        {
            Ensure.ArgumentNotNull(@this, nameof(@this));
            Ensure.ArgumentNotNull(f, nameof(f));

            if (@this.IsAutoCommit)
            {
                return @this.RunInTransaction(f, transactionMode);
            }
            else
            {
                return f(@this);
            }
        }
    }
}