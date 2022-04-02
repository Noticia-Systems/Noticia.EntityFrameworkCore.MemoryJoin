using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Noticia.EntityFrameworkCore.MemoryJoin.Data.Interfaces;

namespace Noticia.EntityFrameworkCore.MemoryJoin.Data;

/// <summary>
/// Builds queryable models.
/// </summary>
public class QueryableModelsBuilder : IQueryableModelsBuilder
{
    #region Static Fields

    /// <summary>
    /// Reflection reference to the DbSet FromSqlRaw method.
    /// </summary>
    private static MethodInfo fromSqlRawMethod = typeof(RelationalQueryableExtensions)
        .GetTypeInfo()
        .GetMethods()
        .Where(methodInfo => methodInfo.Name == "FromSqlRaw")
        .OrderByDescending(methodInfo => methodInfo.GetParameters().Length)
        .First();

    /// <summary>
    /// Reflection reference to the DbContext Set method.
    /// </summary>
    private static MethodInfo setMethod =
        typeof(DbContext).GetMethod("Set", new Type[] { }) ?? throw new NullReferenceException();

    /// <summary>
    /// Reflection reference to the LINQ Select method.
    /// </summary>
    private static MethodInfo selectMethod = typeof(Queryable)
        .GetTypeInfo()
        .GetMethods()
        .Where(methodInfo => methodInfo.Name == "Select")
        .OrderBy(methodInfo => methodInfo.GetParameters().Length)
        .First();

    #endregion

    #region Fields

    /// <summary>
    /// <see cref="dbContext"/> used to create the queryable models.
    /// </summary>
    private readonly DbContext dbContext;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="QueryableModelsBuilder"/> class.
    /// </summary>
    /// <param name="dbContext"><see cref="DbContext"/> used to create the queryable models.</param>
    public QueryableModelsBuilder(DbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    #endregion

    #region Methods

    /// <inheritdoc />
    public IQueryable<T> Build<T>(IEnumerable<T> models) where T : new()
    {
        using (var dbCommand = this.dbContext.Database.GetDbConnection().CreateCommand())
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

            var set = setMethod.MakeGenericMethod(memoryEntityType).Invoke(this.dbContext, Array.Empty<object?>()) ??
                      throw new NullReferenceException();
            var rawSqlResult = fromSqlRawMethod.MakeGenericMethod(memoryEntityType).Invoke(null,
                                   new object[] { set, sqlQuery, dbParameterBuilder.Build().ToArray() }) ??
                               throw new NullReferenceException();

            var mappedModels = selectMethod.MakeGenericMethod(memoryEntityType, typeof(T))
                                   .Invoke(null, new object[] { rawSqlResult, mappings.FromExpression }) ??
                               throw new NullReferenceException();

            return (IQueryable<T>)mappedModels;
        }
    }

    #endregion
}