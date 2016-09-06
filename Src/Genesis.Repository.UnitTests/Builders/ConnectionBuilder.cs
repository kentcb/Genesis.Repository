namespace Genesis.Repository.UnitTests.Builders
{
    using global::Genesis.Util;
    using global::SQLitePCL.pretty;

    public sealed class ConnectionBuilder : IBuilder
    {
        private readonly IDatabaseConnection connection;

        public ConnectionBuilder()
        {
            this.connection = SQLite3.OpenInMemory();
        }

        public IDatabaseConnection Build() =>
            this.connection;
    }
}