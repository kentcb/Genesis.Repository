namespace Genesis.Repository.UnitTests.Mocks
{
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using PCLMock;

    partial class RepositoryMock<TId, TEntity>
    {
        partial void ConfigureLooseBehavior()
        {
            this
                .When(x => x.Save(It.IsAny<TEntity>()))
                .Return<TEntity>(entity => entity);
            this
                .When(x => x.SaveAll(It.IsAny<IEnumerable<TEntity>>()))
                .Return<IEnumerable<TEntity>>(entities => entities.ToImmutableList());
        }
    }
}