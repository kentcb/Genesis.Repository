namespace Genesis.Repository.UnitTests.Builders
{
    using System.Collections.Generic;
    using global::SQLitePCL.pretty;

    public sealed class TestEntity : IEntity<int>
    {
        public TestEntity()
        {
        }

        public TestEntity(IReadOnlyList<IResultSetValue> resultSet)
        {
            this.Id = resultSet[0].ToInt();
            this.Column1 = resultSet[1].ToNullableString();
            this.Column2 = resultSet[2].ToNullableString();
        }

        public int? Id
        {
            get;
            set;
        }

        public string Column1
        {
            get;
            set;
        }

        public string Column2
        {
            get;
            set;
        }
    }
}