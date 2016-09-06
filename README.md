![Logo](Art/Logo150x150.png "Logo")

# Genesis.Repository

[![Build status](https://ci.appveyor.com/api/projects/status/0g7hq7welsnre1gv?svg=true)](https://ci.appveyor.com/project/kentcb/genesis-repository)

## What?

> All Genesis.* projects are formalizations of small pieces of functionality I find myself copying from project to project. Some are small to the point of triviality, but are time-savers nonetheless. They have a particular focus on performance with respect to mobile development, but are certainly applicable outside this domain.
 
**Genesis.Repository** is a library that makes it simpler to implement the repository pattern within your application, using SQLite as a backing store.

* .NET 4.5
* Windows 8
* Windows Store
* Windows Phone 8
* Xamarin iOS
* Xamarin Android

Note that **Genesis.Repository** does nothing to help with getting your data store created and maintained. For that, see [Genesis.DataStore](https://github.com/kentcb/Genesis.DataStore).

## Why?

ORMs are great, except when they're not. Often I found myself living outside the capabilities of the ORM, and writing a bunch of infrastructure to support it. I moved away from using an ORM for this reason and found the greater control empowering. However, I still wanted _some_ help so that I wasn't writing so much boilerplate code. **Genesis.Repository** is that help.

## Where?

The easiest way to get **Genesis.Repository** is via [NuGet](http://www.nuget.org/packages/Genesis.Repository/):

```PowerShell
Install-Package Genesis.Repository
```

## How?

To use **Genesis.Repository**, you first define one or more entities representing the data you wish to store: 

```C#
public sealed class CustomerEntity : IEntity<int>
{
    public CustomerEntity(
        int? id,
        string name)
    {
        Id = id;
        Name = name;
    }

    public int? Id { get; }

    public string Name { get; }

    public CustomerEntity WithId(int? id) =>
        new CustomerEntity(
            id,
            Name);
}
```

Note that the ID must be a value type and must be declared nullable as dictated by the `IEntity<TId>` interface. You are free to define a more complex structure representing your entity ID, but you will be responsible for flattening that ID in your repository.

Speaking of a repository, that's what you would implement next:

```C#
public sealed class CustomerRepository : Repository<int, CustomerEntity>
{
    public CustomerRepository(
        IDatabaseConnection connection)
        : base(connection)
    {
    }

    protected override string TableName => "customer";

    protected override IEnumerable<Column> Columns =>
        new[]
        {
            new Column("id", true),
            new Column("name", sortOrder: SortOrder.Ascending)
        };

    protected override CustomerEntity ValuesToEntity(IReadOnlyList<IResultSetValue> resultSet) =>
        new CustomerEntity(
            resultSet[0].ToInt(),
            resultSet[1].ToNullableString());

    protected override IEnumerable<object> EntityToValues(CustomerEntity entity) =>
        new object[]
        {
            entity.Id,
            entity.Name
        };

    protected override CustomerEntity OnEntitySaved(CustomerEntity entity, IDatabaseConnection connection) =>
        entity.WithId((int)connection.LastInsertedRowId);
}
```

By inheriting from `Repository<TId, TEntity>`, you will obtain a lot of functionality for free. Everything defined by the `IRepository<TId, TEntity>` interface is implemented for you, and you need only fill in a few missing pieces as shown above. There are other optional methods you can override to further control the behavior of the repository.

You can, of course, supplement the implementation with your own repository- and application-specific operations, such as finding the best customer for this month. The `Repository<TId, TEntity>` base class serves as a useful reference for when you wish to do so.

Once you have a repository instance, you don't necessarily want to use it directly. All operations it performs are done on the caller's thread. SQLite likes to be accessed from the same thread, and you want to be sure that thread isn't your UI thread. To that end, **Genesis.Repository** provides the `AsyncRepository<TId, TEntity>` wrapper class:

```C#
var repository = new CustomerRepository();
var asyncRepository = new AsyncRepository(repository, scheduler);
```

Now you can use `asyncRepository` and all operations will be performed on the specified scheduler. It also provides some pretty cool stuff, like an observable that gives you all future changes (adds, removes, deletions), or an observable that gives you all existing and future changes.

If you supplemented the default repository implementation with your own operations, you might want to consider also extending `AsyncRepository<TId, TEntity>` accordingly.

## Who?

**Genesis.Repository** is created and maintained by [Kent Boogaart](http://kent-boogaart.com). Issues and pull requests are welcome.