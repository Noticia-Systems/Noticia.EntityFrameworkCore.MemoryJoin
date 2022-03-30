using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Noticia.EntityFrameworkCore.MemoryJoin.Data;
using Noticia.EntityFrameworkCore.MemoryJoin.Data.Interfaces;
using Noticia.EntityFrameworkCore.MemoryJoin.Reflection;

namespace Noticia.EntityFrameworkCore.MemoryJoin.Extensions;

/// <summary>
/// Extension methods for <see cref="DbContext"/>.
/// </summary>
public static class DbContextExtensions
{
    #region Static Fields

    private static MethodInfo fromSqlRawMethod = typeof(RelationalQueryableExtensions)
        .GetTypeInfo()
        .GetMethods()
        .Where(x => x.Name == "FromSqlRaw")
        .OrderByDescending(x => x.GetParameters().Length)
        .FirstOrDefault();

    private static MethodInfo setMethod = typeof(DbContext).GetMethod("Set", new Type[] { });

    private static MethodInfo selectMethod = typeof(Queryable)
        .GetTypeInfo()
        .GetMethods()
        .Where(x => x.Name == "Select")
        .OrderBy(x => x.GetParameters().Length)
        .First();

    #endregion

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
        using (var dbCommand = dbContext.Database.GetDbConnection().CreateCommand())
        {
            var memoryEntityType = MemoryEntityManager.GetMemoryEntityType<T>();
            var mappings = MemoryEntityManager.GetMemoryEntityMapping<T>();
            var memoryEntities = models.Select(model => mappings.ToFunc(model)).ToList();
            var commandTypeName = dbCommand.GetType().Name;
            var dbParameterBuilder = new DbParameterBuilder<T>(mappings, memoryEntities, dbCommand);

            IMemoryEntitySqlQueryBuilder memoryEntitySqlQueryBuilder;

            if (commandTypeName.StartsWith("Npgsql"))
            {
                memoryEntitySqlQueryBuilder = new NpgsqlMemoryEntitySqlQueryBuilder<T>(mappings, memoryEntities);
            }
            else if (commandTypeName.StartsWith("SqlCommand"))
            {
                memoryEntitySqlQueryBuilder = new MsSqlMemoryEntitySqlQueryBuilder<T>(mappings, memoryEntities);
            }
            else if (commandTypeName.Contains("SqliteCommand"))
            {
                memoryEntitySqlQueryBuilder = new SqliteMemoryEntitySqlQueryBuilder<T>(mappings, memoryEntities);
            }
            else
            {
                throw new InvalidOperationException("No SqlQueryBuilder found for the given database adapter.");
            }

            var sqlQuery = memoryEntitySqlQueryBuilder.Build();

            var set = setMethod.MakeGenericMethod(memoryEntityType).Invoke(dbContext, Array.Empty<object?>());
            var rawSqlResult = fromSqlRawMethod.MakeGenericMethod(memoryEntityType).Invoke(null,
                new object[] { set, sqlQuery, dbParameterBuilder.Build().ToArray() });

            var mappedModels = selectMethod.MakeGenericMethod(memoryEntityType, typeof(T))
                .Invoke(null, new object[] { rawSqlResult, mappings.FromExpression });

            return (IQueryable<T>)mappedModels;
        }
    }

    #endregion
}