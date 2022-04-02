using Microsoft.EntityFrameworkCore;

namespace Noticia.EntityFrameworkCore.MemoryJoin.Data.Interfaces;

/// <summary>
/// Base definition for a class building queryable models.
/// </summary>
public interface IQueryableModelsBuilder
{
    #region Methods

    /// <summary>
    /// Builds queryable models.
    /// </summary>
    /// <param name="models">Models to make queryable.</param>
    /// <typeparam name="T">Model type.</typeparam>
    /// <returns>Queryable models.</returns>
    IQueryable<T> Build<T>(IEnumerable<T> models) where T : new();

    #endregion
}

/// <summary>
/// More constrained version of <see cref="IQueryableModelsBuilder"/> to support multiple <see cref="DbContext"/>s using dependency injection.
/// </summary>
/// <typeparam name="TDbContext"></typeparam>
public interface IQueryableModelsBuilder<TDbContext> : IQueryableModelsBuilder where TDbContext : DbContext
{
}