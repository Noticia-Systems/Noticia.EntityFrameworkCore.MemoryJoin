using Microsoft.EntityFrameworkCore;

namespace Noticia.EntityFrameworkCore.MemoryJoin.Extensions;

/// <summary>
/// Extension methods for <see cref="DbContext"/>.
/// </summary>
public static class DbContextExtensions
{
    #region Static Methods

    public static IQueryable<T> FromClient<T>(this DbContext dbContext, IEnumerable<T> data)
    {
        return null;
    }
    
    #endregion
}