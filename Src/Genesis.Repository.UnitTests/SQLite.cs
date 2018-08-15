namespace Genesis.Repository.UnitTests
{
    public sealed class SQLite
    {
        public SQLite() => SQLitePCL.Batteries_V2.Init();
    }
}