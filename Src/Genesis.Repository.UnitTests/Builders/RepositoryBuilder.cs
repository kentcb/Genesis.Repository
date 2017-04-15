namespace Genesis.Repository.UnitTests.Builders
{
    using Genesis.TestUtil;
    using global::SQLitePCL.pretty;
    using global::System;
    using global::System.Collections.Generic;

    public sealed class RepositoryBuilder : IBuilder
    {
        private IDatabaseConnection connection;
        private string tableName;
        private IEnumerable<Column> columns;
        private Func<IReadOnlyList<IResultSetValue>, TestEntity> valuesToEntity;
        private Func<TestEntity, IEnumerable<object>> entityToValues;
        private Func<TestEntity, IDatabaseConnection, TestEntity> onEntityLoaded;
        private Func<TestEntity, IDatabaseConnection, TestEntity> onEntitySaving;
        private Func<TestEntity, IDatabaseConnection, TestEntity> onEntitySaved;
        private Action<int?, IDatabaseConnection> onEntityDeleted;

        public RepositoryBuilder()
        {
            this.connection = new ConnectionBuilder()
                .Build();
            this.tableName = "test";
            this.columns = new[]
            {
                new Column("id", true, SortOrder.Ascending),
                new Column("column_1"),
                new Column("column_2")
            };
            this.valuesToEntity = resultSet => new TestEntity(resultSet);
            this.entityToValues = entity => new object[] { entity.Id, entity.Column1, entity.Column2 };
            this.onEntityLoaded = (entity, connection) => entity;
            this.onEntitySaving = (entity, connection) => entity;
            this.onEntitySaved = (entity, connection) =>
            {
                entity.Id = (int)connection.LastInsertedRowId;
                return entity;
            };
            this.onEntityDeleted = (id, connection) => { };

            this.connection.Execute(@"
CREATE TABLE test
(
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    column_1 TEXT,
    column_2 TEXT
);");
        }

        public RepositoryBuilder WithConnection(IDatabaseConnection connection) =>
            this.With(ref this.connection, connection);

        public RepositoryBuilder WithTableName(string tableName) =>
            this.With(ref this.tableName, tableName);

        public RepositoryBuilder WithColumns(IEnumerable<Column> columns) =>
            this.With(ref this.columns, columns);

        public RepositoryBuilder WithValuesToEntity(Func<IReadOnlyList<IResultSetValue>, TestEntity> valuesToEntity) =>
            this.With(ref this.valuesToEntity, valuesToEntity);

        public RepositoryBuilder WithEntityToValues(Func<TestEntity, IEnumerable<object>> entityToValues) =>
            this.With(ref this.entityToValues, entityToValues);

        public RepositoryBuilder WithOnEntityLoaded(Func<TestEntity, IDatabaseConnection, TestEntity> onEntityLoaded) =>
            this.With(ref this.onEntityLoaded, onEntityLoaded);

        public RepositoryBuilder WithOnEntitySaving(Func<TestEntity, IDatabaseConnection, TestEntity> onEntitySaving) =>
            this.With(ref this.onEntitySaving, onEntitySaving);

        public RepositoryBuilder WithOnEntitySaved(Func<TestEntity, IDatabaseConnection, TestEntity> onEntitySaved) =>
            this.With(ref this.onEntitySaved, onEntitySaved);

        public RepositoryBuilder WithOnEntityDeleted(Action<int?, IDatabaseConnection> onEntityDeleted) =>
            this.With(ref this.onEntityDeleted, onEntityDeleted);

        public Repository<int, TestEntity> Build() =>
             new ConcreteRepository(
                 this.connection,
                 this.tableName,
                 this.columns,
                 this.valuesToEntity,
                 this.entityToValues,
                 this.onEntityLoaded,
                 this.onEntitySaving,
                 this.onEntitySaved,
                 this.onEntityDeleted);

        public static implicit operator Repository<int, TestEntity>(RepositoryBuilder builder) =>
            builder.Build();

        private sealed class ConcreteRepository : Repository<int, TestEntity>
        {
            private readonly string tableName;
            private readonly IEnumerable<Column> columns;
            private readonly Func<IReadOnlyList<IResultSetValue>, TestEntity> valuesToEntity;
            private readonly Func<TestEntity, IEnumerable<object>> entityToValues;
            private readonly Func<TestEntity, IDatabaseConnection, TestEntity> onEntityLoaded;
            private readonly Func<TestEntity, IDatabaseConnection, TestEntity> onEntitySaving;
            private readonly Func<TestEntity, IDatabaseConnection, TestEntity> onEntitySaved;
            private readonly Action<int?, IDatabaseConnection> onEntityDeleted;

            public ConcreteRepository(
                IDatabaseConnection connection,
                string tableName,
                IEnumerable<Column> columns,
                Func<IReadOnlyList<IResultSetValue>, TestEntity> valuesToEntity,
                Func<TestEntity, IEnumerable<object>> entityToValues,
                Func<TestEntity, IDatabaseConnection, TestEntity> onEntityLoaded,
                Func<TestEntity, IDatabaseConnection, TestEntity> onEntitySaving,
                Func<TestEntity, IDatabaseConnection, TestEntity> onEntitySaved,
                Action<int?, IDatabaseConnection> onEntityDeleted)
                : base(connection)
            {
                this.tableName = tableName;
                this.columns = columns;
                this.valuesToEntity = valuesToEntity;
                this.entityToValues = entityToValues;
                this.onEntityLoaded = onEntityLoaded;
                this.onEntitySaving = onEntitySaving;
                this.onEntitySaved = onEntitySaved;
                this.onEntityDeleted = onEntityDeleted;
            }

            protected override string TableName => this.tableName;

            protected override IEnumerable<Column> Columns => this.columns;

            protected override TestEntity ValuesToEntity(IReadOnlyList<IResultSetValue> resultSet) =>
                this.valuesToEntity(resultSet);

            protected override IEnumerable<object> EntityToValues(TestEntity entity) =>
                this.entityToValues(entity);

            protected override TestEntity OnEntityLoaded(TestEntity entity, IDatabaseConnection connection) =>
                this.onEntityLoaded(entity, connection);

            protected override TestEntity OnEntitySaving(TestEntity entity, IDatabaseConnection connection) =>
                this.onEntitySaving(entity, connection);

            protected override TestEntity OnEntitySaved(TestEntity entity, IDatabaseConnection connection) =>
                this.onEntitySaved(entity, connection);

            protected override void OnEntityDeleted(int id, IDatabaseConnection connection) =>
                this.onEntityDeleted(id, connection);
        }
    }
}