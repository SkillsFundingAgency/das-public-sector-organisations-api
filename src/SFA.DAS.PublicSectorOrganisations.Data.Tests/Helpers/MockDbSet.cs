using System.Collections;
using System.Data.Entity.Infrastructure;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Query;

public class MockDbSet<T> : DbSet<T>, IQueryable<T>, IDbAsyncEnumerable<T>
    where T : class
{
    private readonly List<T> _data;

    public MockDbSet()
    {
        _data = new List<T>();
    }

    public MockDbSet(List<T> source)
    {
        _data = source;
    }

    public Expression Expression => _data.AsQueryable().Expression;

    public override IEntityType EntityType { get; }
    public Type ElementType => _data.AsQueryable().ElementType;

    public IQueryProvider Provider => new MockAsyncQueryProvider<T>(_data.AsQueryable().Provider);

    public IEnumerator<T> GetEnumerator() => _data.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public IDbAsyncEnumerator<T> GetAsyncEnumerator() => new MockAsyncEnumerator<T>(_data.GetEnumerator());
    IDbAsyncEnumerator IDbAsyncEnumerable.GetAsyncEnumerator()
    {
        return GetAsyncEnumerator();
    }
}

public class MockAsyncQueryProvider<T> : IAsyncQueryProvider
{
    private readonly IQueryProvider _inner;

    internal MockAsyncQueryProvider(IQueryProvider inner)
    {
        _inner = inner;
    }

    public IQueryable CreateQuery(Expression expression)
    {
        return new MockAsyncEnumerable<T>(expression);
    }

    public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
    {
        return new MockAsyncEnumerable<TElement>(expression);
    }

    public object Execute(Expression expression)
    {
        return _inner.Execute(expression);
    }

    public TResult Execute<TResult>(Expression expression)
    {
        return _inner.Execute<TResult>(expression);
    }

    public TResult ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken = new CancellationToken())
    {
        throw new NotImplementedException();
    }

    public Task<object> ExecuteAsync(Expression expression, CancellationToken cancellationToken)
    {
        return Task.FromResult(Execute(expression));
    }

}

public class MockAsyncEnumerable<T> : EnumerableQuery<T>, IDbAsyncEnumerable<T>, IQueryable<T>
{
    public MockAsyncEnumerable(IEnumerable<T> enumerable)
        : base(enumerable)
    { }

    public MockAsyncEnumerable(Expression expression)
        : base(expression)
    { }

    public IDbAsyncEnumerator<T> GetAsyncEnumerator()
    {
        return new MockAsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());
    }

    IDbAsyncEnumerator IDbAsyncEnumerable.GetAsyncEnumerator()
    {
        return GetAsyncEnumerator();
    }

    IQueryProvider IQueryable.Provider => new MockAsyncQueryProvider<T>(this);
}

public class MockAsyncEnumerator<T> : IDbAsyncEnumerator<T>
{
    private readonly IEnumerator<T> _inner;

    public MockAsyncEnumerator(IEnumerator<T> inner)
    {
        _inner = inner;
    }

    public void Dispose()
    {
        _inner.Dispose();
    }

    public Task<bool> MoveNextAsync(CancellationToken cancellationToken)
    {
        return Task.FromResult(_inner.MoveNext());
    }

    public T Current => _inner.Current;

    object IDbAsyncEnumerator.Current => Current;
}
