namespace Genesis.Repository.UnitTests
{
    using Builders;
    using global::System;
    using global::System.Collections.Generic;
    using global::System.Collections.Immutable;
    using global::System.Reactive.Concurrency;
    using global::System.Reactive.Linq;
    using Microsoft.Reactive.Testing;
    using Mocks;
    using PCLMock;
    using Xunit;

    public sealed class AsyncRepositoryFixture
    {
        [Fact]
        public void future_items_ticks_any_items_added_after_the_subscription()
        {
            var repository = new RepositoryMock<int, TestEntity>(MockBehavior.Loose);
            repository
                .When(x => x.GetAll())
                .Return(ImmutableList<TestEntity>.Empty);
            var sut = new AsyncRepositoryBuilder()
                .WithDataStoreScheduler(ImmediateScheduler.Instance)
                .WithRepository(repository)
                .Build();

            var futureItems = new List<ItemChange<TestEntity>>();
            sut
                .FutureItems
                .Subscribe(futureItems.Add);

            Assert.Equal(0, futureItems.Count);

            sut
                .Save(new TestEntity())
                .Subscribe();

            Assert.Equal(1, futureItems.Count);
            Assert.Equal(ItemChangeType.Add, futureItems[0].Type);
        }

        [Fact]
        public void future_items_ticks_any_items_updated_after_the_subscription()
        {
            var repository = new RepositoryMock<int, TestEntity>(MockBehavior.Loose);
            repository
                .When(x => x.GetAll())
                .Return(ImmutableList<TestEntity>.Empty);
            var sut = new AsyncRepositoryBuilder()
                .WithDataStoreScheduler(ImmediateScheduler.Instance)
                .WithRepository(repository)
                .Build();

            var futureItems = new List<ItemChange<TestEntity>>();
            sut
                .FutureItems
                .Subscribe(futureItems.Add);

            Assert.Equal(0, futureItems.Count);

            sut
                .Save(new TestEntity { Id = 42 })
                .Subscribe();

            Assert.Equal(1, futureItems.Count);
            Assert.Equal(ItemChangeType.Update, futureItems[0].Type);
        }

        [Fact]
        public void future_items_ticks_any_items_removed_after_the_subscription()
        {
            var repository = new RepositoryMock<int, TestEntity>(MockBehavior.Loose);
            repository
                .When(x => x.GetAll())
                .Return(ImmutableList<TestEntity>.Empty);
            repository
                .When(x => x.Get(42))
                .Return(new TestEntity { Id = 42 });
            var sut = new AsyncRepositoryBuilder()
                .WithDataStoreScheduler(ImmediateScheduler.Instance)
                .WithRepository(repository)
                .Build();

            var futureItems = new List<ItemChange<TestEntity>>();
            sut
                .FutureItems
                .Subscribe(futureItems.Add);

            Assert.Equal(0, futureItems.Count);

            sut
                .Delete(42)
                .Subscribe();

            Assert.Equal(1, futureItems.Count);
            Assert.Equal(ItemChangeType.Remove, futureItems[0].Type);
        }

        [Fact]
        public void items_does_nothing_without_a_subscription()
        {
            var scheduler = new TestScheduler();
            var repository = new RepositoryMock<int, TestEntity>(MockBehavior.Loose);
            var sut = new AsyncRepositoryBuilder()
                .WithDataStoreScheduler(scheduler)
                .WithRepository(repository)
                .Build();

            var items = sut.Items;
            scheduler.AdvanceMinimal();

            repository
                .Verify(x => x.GetAll())
                .WasNotCalled();
        }

        [Fact]
        public void items_retrieves_entities_on_provided_scheduler()
        {
            var scheduler = new TestScheduler();
            var repository = new RepositoryMock<int, TestEntity>(MockBehavior.Loose);
            repository
                .When(x => x.GetAll())
                .Return(ImmutableList<TestEntity>.Empty);
            var sut = new AsyncRepositoryBuilder()
                .WithDataStoreScheduler(scheduler)
                .WithRepository(repository)
                .Build();

            sut
                .Items
                .Subscribe();

            repository
                .Verify(x => x.GetAll())
                .WasNotCalled();
            scheduler.AdvanceMinimal();
            repository
                .Verify(x => x.GetAll())
                .WasCalledExactlyOnce();
        }

        [Fact]
        public void items_ticks_any_items_added_after_the_subscription()
        {
            var repository = new RepositoryMock<int, TestEntity>(MockBehavior.Loose);
            repository
                .When(x => x.GetAll())
                .Return(ImmutableList<TestEntity>.Empty);
            var sut = new AsyncRepositoryBuilder()
                .WithDataStoreScheduler(ImmediateScheduler.Instance)
                .WithRepository(repository)
                .Build();

            var items = new List<ItemChange<TestEntity>>();
            sut
                .Items
                .Subscribe(items.Add);

            Assert.Equal(0, items.Count);

            sut
                .Save(new TestEntity())
                .Subscribe();

            Assert.Equal(1, items.Count);
            Assert.Equal(ItemChangeType.Add, items[0].Type);
        }

        [Fact]
        public void items_ticks_any_items_updated_after_the_subscription()
        {
            var repository = new RepositoryMock<int, TestEntity>(MockBehavior.Loose);
            repository
                .When(x => x.GetAll())
                .Return(ImmutableList<TestEntity>.Empty);
            var sut = new AsyncRepositoryBuilder()
                .WithDataStoreScheduler(ImmediateScheduler.Instance)
                .WithRepository(repository)
                .Build();

            var items = new List<ItemChange<TestEntity>>();
            sut
                .Items
                .Subscribe(items.Add);

            Assert.Equal(0, items.Count);

            sut
                .Save(new TestEntity { Id = 42 })
                .Subscribe();

            Assert.Equal(1, items.Count);
            Assert.Equal(ItemChangeType.Update, items[0].Type);
        }

        [Fact]
        public void items_ticks_any_items_removed_after_the_subscription()
        {
            var repository = new RepositoryMock<int, TestEntity>(MockBehavior.Loose);
            repository
                .When(x => x.GetAll())
                .Return(ImmutableList<TestEntity>.Empty);
            repository
                .When(x => x.Get(42))
                .Return(new TestEntity { Id = 42 });
            var sut = new AsyncRepositoryBuilder()
                .WithDataStoreScheduler(ImmediateScheduler.Instance)
                .WithRepository(repository)
                .Build();

            var items = new List<ItemChange<TestEntity>>();
            sut
                .Items
                .Subscribe(items.Add);

            Assert.Equal(0, items.Count);

            sut
                .Delete(42)
                .Subscribe();

            Assert.Equal(1, items.Count);
            Assert.Equal(ItemChangeType.Remove, items[0].Type);
        }

        [Fact]
        public void get_does_nothing_without_a_subscription()
        {
            var scheduler = new TestScheduler();
            var repository = new RepositoryMock<int, TestEntity>(MockBehavior.Loose);
            var sut = new AsyncRepositoryBuilder()
                .WithDataStoreScheduler(scheduler)
                .WithRepository(repository)
                .Build();

            sut.Get(42);
            scheduler.AdvanceMinimal();

            repository
                .Verify(x => x.Get(It.IsAny<int>()))
                .WasNotCalled();
        }

        [Fact]
        public void get_retrieves_entity_on_provided_scheduler()
        {
            var scheduler = new TestScheduler();
            var repository = new RepositoryMock<int, TestEntity>(MockBehavior.Loose);
            var sut = new AsyncRepositoryBuilder()
                .WithDataStoreScheduler(scheduler)
                .WithRepository(repository)
                .Build();

            sut
                .Get(42)
                .Subscribe();

            repository
                .Verify(x => x.Get(It.IsAny<int>()))
                .WasNotCalled();
            scheduler.AdvanceMinimal();
            repository
                .Verify(x => x.Get(It.IsAny<int>()))
                .WasCalledExactlyOnce();
        }

        [Fact]
        public void get_all_does_nothing_without_a_subscription()
        {
            var scheduler = new TestScheduler();
            var repository = new RepositoryMock<int, TestEntity>(MockBehavior.Loose);
            var sut = new AsyncRepositoryBuilder()
                .WithDataStoreScheduler(scheduler)
                .WithRepository(repository)
                .Build();

            sut.GetAll();
            scheduler.AdvanceMinimal();

            repository
                .Verify(x => x.GetAll())
                .WasNotCalled();
        }

        [Fact]
        public void get_all_retrieves_entity_on_provided_scheduler()
        {
            var scheduler = new TestScheduler();
            var repository = new RepositoryMock<int, TestEntity>(MockBehavior.Loose);
            var sut = new AsyncRepositoryBuilder()
                .WithDataStoreScheduler(scheduler)
                .WithRepository(repository)
                .Build();

            sut
                .GetAll()
                .Subscribe();

            repository
                .Verify(x => x.GetAll())
                .WasNotCalled();
            scheduler.AdvanceMinimal();
            repository
                .Verify(x => x.GetAll())
                .WasCalledExactlyOnce();
        }

        [Fact]
        public void save_does_nothing_without_a_subscription()
        {
            var scheduler = new TestScheduler();
            var repository = new RepositoryMock<int, TestEntity>(MockBehavior.Loose);
            var sut = new AsyncRepositoryBuilder()
                .WithDataStoreScheduler(scheduler)
                .WithRepository(repository)
                .Build();

            sut.Save(new TestEntity());
            scheduler.AdvanceMinimal();

            repository
                .Verify(x => x.Save(It.IsAny<TestEntity>()))
                .WasNotCalled();
        }

        [Fact]
        public void save_saves_entity_on_provided_scheduler()
        {
            var scheduler = new TestScheduler();
            var repository = new RepositoryMock<int, TestEntity>(MockBehavior.Loose);
            var sut = new AsyncRepositoryBuilder()
                .WithDataStoreScheduler(scheduler)
                .WithRepository(repository)
                .Build();

            sut
                .Save(new TestEntity())
                .Subscribe();

            repository
                .Verify(x => x.Save(It.IsAny<TestEntity>()))
                .WasNotCalled();
            scheduler.AdvanceMinimal();
            repository
                .Verify(x => x.Save(It.IsAny<TestEntity>()))
                .WasCalledExactlyOnce();
        }

        [Fact]
        public void save_all_does_nothing_without_a_subscription()
        {
            var scheduler = new TestScheduler();
            var repository = new RepositoryMock<int, TestEntity>(MockBehavior.Loose);
            var sut = new AsyncRepositoryBuilder()
                .WithDataStoreScheduler(scheduler)
                .WithRepository(repository)
                .Build();

            sut.SaveAll(new[] { new TestEntity() });
            scheduler.AdvanceMinimal();

            repository
                .Verify(x => x.SaveAll(It.IsAny<IEnumerable<TestEntity>>()))
                .WasNotCalled();
        }

        [Fact]
        public void save_all_saves_entities_on_provided_scheduler()
        {
            var scheduler = new TestScheduler();
            var repository = new RepositoryMock<int, TestEntity>(MockBehavior.Loose);
            var sut = new AsyncRepositoryBuilder()
                .WithDataStoreScheduler(scheduler)
                .WithRepository(repository)
                .Build();

            sut
                .SaveAll(new[] { new TestEntity() })
                .Subscribe();

            repository
                .Verify(x => x.SaveAll(It.IsAny<IEnumerable<TestEntity>>()))
                .WasNotCalled();
            scheduler.AdvanceMinimal();
            repository
                .Verify(x => x.SaveAll(It.IsAny<IEnumerable<TestEntity>>()))
                .WasCalledExactlyOnce();
        }

        [Fact]
        public void delete_does_nothing_without_a_subscription()
        {
            var scheduler = new TestScheduler();
            var repository = new RepositoryMock<int, TestEntity>(MockBehavior.Loose);
            var sut = new AsyncRepositoryBuilder()
                .WithDataStoreScheduler(scheduler)
                .WithRepository(repository)
                .Build();

            sut.Delete(42);
            scheduler.AdvanceMinimal();

            repository
                .Verify(x => x.Delete(It.IsAny<int>()))
                .WasNotCalled();
        }

        [Fact]
        public void delete_deletes_entity_on_provided_scheduler()
        {
            var scheduler = new TestScheduler();
            var repository = new RepositoryMock<int, TestEntity>(MockBehavior.Loose);
            repository
                .When(x => x.Get(42))
                .Return(new TestEntity());
            var sut = new AsyncRepositoryBuilder()
                .WithDataStoreScheduler(scheduler)
                .WithRepository(repository)
                .Build();

            sut
                .Delete(42)
                .Subscribe();

            repository
                .Verify(x => x.Delete(It.IsAny<int>()))
                .WasNotCalled();
            scheduler.AdvanceMinimal();
            repository
                .Verify(x => x.Delete(It.IsAny<int>()))
                .WasCalledExactlyOnce();
        }

        [Fact]
        public void delete_all_does_nothing_without_a_subscription()
        {
            var scheduler = new TestScheduler();
            var repository = new RepositoryMock<int, TestEntity>(MockBehavior.Loose);
            var sut = new AsyncRepositoryBuilder()
                .WithDataStoreScheduler(scheduler)
                .WithRepository(repository)
                .Build();

            sut.DeleteAll();
            scheduler.AdvanceMinimal();

            repository
                .Verify(x => x.DeleteAll())
                .WasNotCalled();
        }

        [Fact]
        public void delete_all_deletes_all_entities_on_provided_scheduler()
        {
            var scheduler = new TestScheduler();
            var repository = new RepositoryMock<int, TestEntity>(MockBehavior.Loose);
            repository
                .When(x => x.Get(42))
                .Return(new TestEntity());
            var sut = new AsyncRepositoryBuilder()
                .WithDataStoreScheduler(scheduler)
                .WithRepository(repository)
                .Build();

            sut
                .DeleteAll()
                .Subscribe();

            repository
                .Verify(x => x.DeleteAll())
                .WasNotCalled();
            scheduler.AdvanceMinimal();
            repository
                .Verify(x => x.DeleteAll())
                .WasCalledExactlyOnce();
        }
    }
}