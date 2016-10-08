namespace Genesis.Repository.UnitTests
{
    using Builders;
    using global::System.Linq;
    using Xunit;

    public sealed class RepositoryFixture
    {
        [Theory]
        [InlineData(1)]
        [InlineData(13)]
        [InlineData(42)]
        public void get_returns_null_if_entity_not_found(int id)
        {
            var sut = new RepositoryBuilder()
                .Build();
            Assert.Null(sut.Get(id));
        }

        [Theory]
        [InlineData(1)]
        [InlineData(13)]
        [InlineData(42)]
        public void get_returns_entity_if_entity_is_found(int id)
        {
            var sut = new RepositoryBuilder()
                .Build();
            sut.Save(
                new TestEntity
                {
                    Id = id
                });

            Assert.NotNull(sut.Get(id));
        }

        [Fact]
        public void get_passes_entity_through_to_subclass()
        {
            var sut = new RepositoryBuilder()
                .WithOnEntityLoaded(
                    (e, c) =>
                    {
                        e.Column1 = "replacement";
                        return e;
                    })
                .Build();
            sut.Save(
                new TestEntity
                {
                    Id = 42,
                    Column1 = "original"
                });

            var entity = sut.Get(42);
            Assert.Equal("replacement", entity.Column1);
        }

        [Fact]
        public void get_all_returns_empty_list_if_no_entities_are_found()
        {
            var sut = new RepositoryBuilder()
                .Build();
            var result = sut.GetAll();

            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public void get_all_returns_all_entities()
        {
            var sut = new RepositoryBuilder()
                .Build();
            sut.SaveAll(
                Enumerable
                    .Range(0, 100)
                    .Select(i =>
                        new TestEntity
                        {
                            Id = i
                        }));

            var results = sut.GetAll();

            Assert.NotNull(results);
            Assert.Equal(100, results.Count);
        }

        [Fact]
        public void get_all_orders_entities_by_the_default_order()
        {
            var sut = new RepositoryBuilder()
                .WithColumns(
                    new[]
                    {
                        new Column("id", true),
                        new Column("column_1", sortOrder: SortOrder.Ascending),
                        new Column("column_2", sortOrder: SortOrder.Descending),
                    })
                .Build();
            sut.Save(
                new TestEntity
                {
                    Column1 = "z",
                    Column2 = "c"
                });
            sut.Save(
                new TestEntity
                {
                    Column1 = "m",
                    Column2 = "a"
                });
            sut.Save(
                new TestEntity
                {
                    Column1 = "a",
                    Column2 = "z"
                });
            sut.Save(
                new TestEntity
                {
                    Column1 = "b",
                    Column2 = "zz"
                });
            sut.Save(
                new TestEntity
                {
                    Column1 = "a",
                    Column2 = "m"
                });
            sut.Save(
                new TestEntity
                {
                    Column1 = "z",
                    Column2 = "e"
                });

            var results = sut.GetAll();

            Assert.NotNull(results);
            Assert.Equal(6, results.Count);
            Assert.Equal("a", results[0].Column1);
            Assert.Equal("z", results[0].Column2);
            Assert.Equal("a", results[1].Column1);
            Assert.Equal("m", results[1].Column2);
            Assert.Equal("b", results[2].Column1);
            Assert.Equal("zz", results[2].Column2);
            Assert.Equal("m", results[3].Column1);
            Assert.Equal("a", results[3].Column2);
            Assert.Equal("z", results[4].Column1);
            Assert.Equal("e", results[4].Column2);
            Assert.Equal("z", results[5].Column1);
            Assert.Equal("c", results[5].Column2);
        }

        [Fact]
        public void get_all_passes_entities_through_to_subclass()
        {
            var sut = new RepositoryBuilder()
                .WithOnEntityLoaded(
                    (e, c) =>
                    {
                        e.Column1 = "replacement" + e.Id;
                        return e;
                    })
                .Build();
            sut.Save(
                new TestEntity
                {
                    Id = 13,
                    Column1 = "original"
                });
            sut.Save(
                new TestEntity
                {
                    Id = 42,
                    Column1 = "original"
                });

            var results = sut.GetAll();

            Assert.NotNull(results);
            Assert.Equal(2, results.Count);
            Assert.Equal("replacement13", results[0].Column1);
            Assert.Equal("replacement42", results[1].Column1);
        }

        [Theory]
        [InlineData("foo", "bar")]
        [InlineData("fiz", "buz")]
        public void save_can_insert_new_entities(string column1, string column2)
        {
            var sut = new RepositoryBuilder()
                .Build();
            var result = sut
                .Save(
                    new TestEntity
                    {
                        Column1 = column1,
                        Column2 = column2
                    });

            Assert.NotNull(result.Id);
            var reload = sut.Get(result.Id.Value);

            Assert.NotNull(reload);
            Assert.Equal(result.Id, reload.Id);
            Assert.Equal(column1, reload.Column1);
            Assert.Equal(column2, reload.Column2);
        }

        [Theory]
        [InlineData("foo", "bar")]
        [InlineData("fiz", "buz")]
        public void save_can_update_existing_entities(string column1, string column2)
        {
            var sut = new RepositoryBuilder()
                .Build();
            var initialSave = sut
                .Save(new TestEntity());

            Assert.NotNull(initialSave.Id);
            var update = sut
                .Save(
                    new TestEntity
                    {
                        Id = initialSave.Id,
                        Column1 = column1,
                        Column2 = column2
                    });
            var load = sut.Get(update.Id.Value);

            Assert.NotNull(load);
            Assert.Equal(update.Id, load.Id);
            Assert.Equal(column1, load.Column1);
            Assert.Equal(column2, load.Column2);
        }

        [Fact]
        public void save_passes_entities_through_to_subclass_before_save()
        {
            var called = false;
            var sut = new RepositoryBuilder()
                .WithOnEntitySaving(
                    (e, c) =>
                    {
                        called = true;
                        return e;
                    })
                .Build();
            sut
                .Save(
                    new TestEntity
                    {
                        Column1 = "original"
                    });

            Assert.True(called);
        }

        [Fact]
        public void save_saves_the_entity_returned_to_it_by_on_entity_saving()
        {
            var sut = new RepositoryBuilder()
                .WithOnEntitySaving((e, c) => new TestEntity { Id = 42, Column1 = "replacement" })
                .Build();
            sut
                .Save(
                    new TestEntity
                    {
                        Id = 42,
                        Column1 = "original"
                    });
            var result = sut.Get(42);

            Assert.NotNull(result);
            Assert.Equal("replacement", result.Column1);
        }

        [Fact]
        public void save_passes_entities_through_to_subclass_after_save()
        {
            var sut = new RepositoryBuilder()
                .WithOnEntitySaved(
                    (e, c) =>
                    {
                        e.Column1 = "replacement";
                        return e;
                    })
                .Build();
            var result = sut
                .Save(
                    new TestEntity
                    {
                        Column1 = "original"
                    });

            Assert.NotNull(result);
            Assert.Equal("replacement", result.Column1);
        }

        [Fact]
        public void save_all_saves_all_provided_entities()
        {
            var sut = new RepositoryBuilder()
                .Build();
            sut.SaveAll(
                Enumerable
                    .Range(0, 100)
                    .Select(i =>
                        new TestEntity
                        {
                            Id = i
                        }));

            var results = sut.GetAll();

            Assert.NotNull(results);
            Assert.Equal(100, results.Count);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(13)]
        [InlineData(42)]
        public void delete_returns_false_if_the_entity_does_not_exist(int id)
        {
            var sut = new RepositoryBuilder()
                .Build();

            Assert.False(sut.Delete(id));
        }

        [Theory]
        [InlineData(1)]
        [InlineData(13)]
        [InlineData(42)]
        public void delete_returns_true_if_entity_is_deleted(int id)
        {
            var sut = new RepositoryBuilder()
                .Build();
            sut.Save(
                new TestEntity
                {
                    Id = id
                });

            Assert.NotNull(sut.Get(id));

            Assert.True(sut.Delete(id));

            Assert.Null(sut.Get(id));
        }

        [Fact]
        public void delete_passes_entity_through_to_subclass_if_it_is_deleted()
        {
            var called = false;
            var sut = new RepositoryBuilder()
                .WithOnEntityDeleted(
                    (e, c) => called = true)
                .Build();
            var result = sut
                .Save(
                    new TestEntity
                    {
                        Column1 = "original"
                    });

            Assert.False(called);
            sut.Delete(result.Id.Value);
            Assert.True(called);
        }

        [Fact]
        public void delete_does_not_pass_entity_through_to_subclass_if_it_is_not_deleted()
        {
            var called = false;
            var sut = new RepositoryBuilder()
                .WithOnEntityDeleted(
                    (id, c) => called = true)
                .Build();

            Assert.False(called);
            sut.Delete(42);
            Assert.False(called);
        }

        [Fact]
        public void delete_all_returns_zero_if_no_entities_exist()
        {
            var sut = new RepositoryBuilder()
                .Build();

            Assert.Equal(0, sut.DeleteAll());
        }

        [Theory]
        [InlineData(1)]
        [InlineData(13)]
        [InlineData(42)]
        public void delete_returns_number_of_entities_deleted(int entityCount)
        {
            var sut = new RepositoryBuilder()
                .Build();

            for (var i = 0; i < entityCount; ++i)
            {
                sut.Save(
                    new TestEntity
                    {
                        Id = i
                    });
            }

            Assert.Equal(entityCount, sut.DeleteAll());
        }

        [Fact]
        public void delete_all_does_not_pass_entity_through_to_subclass()
        {
            var called = false;
            var sut = new RepositoryBuilder()
                .WithOnEntityDeleted(
                    (e, c) => called = true)
                .Build();
            var result = sut
                .Save(
                    new TestEntity
                    {
                        Column1 = "original"
                    });

            Assert.False(called);
            sut.DeleteAll();
            Assert.False(called);
        }
    }
}