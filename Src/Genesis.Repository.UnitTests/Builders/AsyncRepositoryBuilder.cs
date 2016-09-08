namespace Genesis.Repository.UnitTests.Builders
{
    using global::Genesis.Util;
    using global::System.Reactive.Concurrency;
    using global::System.Reactive.Concurrency.Mocks;
    using PCLMock;

    public sealed class AsyncRepositoryBuilder : IBuilder
    {
        private IRepository<int, TestEntity> repository;
        private IScheduler dataStoreScheduler;

        public AsyncRepositoryBuilder()
        {
            this.repository = new RepositoryBuilder()
                .Build();
            this.dataStoreScheduler = new SchedulerMock(MockBehavior.Loose);
        }

        public AsyncRepositoryBuilder WithRepository(IRepository<int, TestEntity> repository) =>
            this.With(ref this.repository, repository);

        public AsyncRepositoryBuilder WithDataStoreScheduler(IScheduler dataStoreScheduler) =>
            this.With(ref this.dataStoreScheduler, dataStoreScheduler);

        public AsyncRepository<int, TestEntity> Build() =>
            new AsyncRepository<int, TestEntity>(
                this.repository,
                this.dataStoreScheduler);

        public static implicit operator AsyncRepository<int, TestEntity>(AsyncRepositoryBuilder builder) =>
            builder.Build();
    }
}