using Microsoft.EntityFrameworkCore;
using Noticia.EntityFrameworkCore.MemoryJoin.Data;

namespace Noticia.EntityFrameworkCore.MemoryJoin.Extensions;

/// <summary>
/// Extension methods for <see cref="DbContext"/>.
/// </summary>
public static class DbContextExtensions
{
    #region Static Methods

    /// <summary>
    /// Creates memory entities from given models and allows for memory joining.
    /// </summary>
    /// <param name="dbContext">Current <see cref="DbContext"/> to supply memory entities to.</param>
    /// <param name="models">Models to map to memory entities.</param>
    /// <typeparam name="T">Model type.</typeparam>
    /// <returns>Server-side queryable entities for EFcore.</returns>
    public static IQueryable<T> AsMemoryEntities<T>(this DbContext dbContext, IEnumerable<T> models) where T : new()
    {
        return new QueryableModelsBuilder(dbContext).Build(models);
    }

    #endregion
}