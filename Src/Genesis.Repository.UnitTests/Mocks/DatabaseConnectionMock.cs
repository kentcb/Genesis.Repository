namespace Genesis.Repository.UnitTests.Mocks
{
    using System.IO;
    using global::SQLitePCL.pretty;
    using PCLMock;

    public sealed class DatabaseConnectionMock : MockBase<IDatabaseConnection>, IDatabaseConnection
    {
        public DatabaseConnectionMock(MockBehavior behavior)
            : base(behavior)
        {
            if (behavior == MockBehavior.Loose)
            {
                this.ConfigureLooseBehavior();
            }
        }

        public int Changes => this.Apply(x => x.Changes);

        public bool IsAutoCommit => this.Apply(x => x.IsAutoCommit);

        public bool IsReadOnly => this.Apply(x => x.IsReadOnly);

        public long LastInsertedRowId => this.Apply(x => x.LastInsertedRowId);

        public int TotalChanges => this.Apply(x => x.TotalChanges);

        public TableColumnMetadata GetTableColumnMetadata(string dbName, string tableName, string columnName) =>
            this.Apply(x => x.GetTableColumnMetadata(dbName, tableName, columnName));

        public bool IsDatabaseReadOnly(string dbName) =>
            this.Apply(x => x.IsDatabaseReadOnly(dbName));

        public Stream OpenBlob(string database, string tableName, string columnName, long rowId, bool canWrite) =>
            this.Apply(x => x.OpenBlob(database, tableName, columnName, rowId, canWrite));

        public IStatement PrepareStatement(string sql, out string tail)
        {
            string tailOut;
            tail = this.GetOutParameterValue<string>(x => x.PrepareStatement(sql, out tailOut), 1);
            return this.Apply(x => x.PrepareStatement(sql, out tailOut));
        }

        public void Status(DatabaseConnectionStatusCode statusCode, out int current, out int highwater, bool reset)
        {
            int currentOut;
            int highwaterOut;
            current = this.GetOutParameterValue<int>(x => x.Status(statusCode, out currentOut, out highwaterOut, reset), 1);
            highwater = this.GetOutParameterValue<int>(x => x.Status(statusCode, out currentOut, out highwaterOut, reset), 2);
            this.Apply(x => x.Status(statusCode, out currentOut, out highwaterOut, reset));
        }

        public void WalCheckPoint(string dbName, WalCheckPointMode mode, out int nLog, out int nCkpt)
        {
            int nLogOut;
            int nCkptOut;
            nLog = this.GetOutParameterValue<int>(x => x.WalCheckPoint(dbName, mode, out nLogOut, out nCkptOut), 2);
            nCkpt = this.GetOutParameterValue<int>(x => x.WalCheckPoint(dbName, mode, out nLogOut, out nCkptOut), 3);
            this.Apply(x => x.WalCheckPoint(dbName, mode, out nLogOut, out nCkptOut));
        }

        private void ConfigureLooseBehavior()
        {
        }
    }
}