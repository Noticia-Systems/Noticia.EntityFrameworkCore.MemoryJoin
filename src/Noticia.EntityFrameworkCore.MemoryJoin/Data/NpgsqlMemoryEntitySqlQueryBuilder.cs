using Noticia.EntityFrameworkCore.MemoryJoin.Reflection;

namespace Noticia.EntityFrameworkCore.MemoryJoin.Data;

/// <summary>
/// PostgresSQL builder for the memory entities.
/// </summary>
/// <typeparam name="T">Model type.</typeparam>
/// <inheritdoc cref="MemoryEntitySqlQueryBuilderBase{T}"/>
public class NpgsqlMemoryEntitySqlQueryBuilder<T> : MemoryEntitySqlQueryBuilderBase<T> where T : new()
{
    #region Constructors
    
    /// <inheritdoc />
    public NpgsqlMemoryEntitySqlQueryBuilder(MemoryEntityMapping<T> memoryEntityMapping, ICollection<T> data) : base(
        memoryEntityMapping, data)
    {
    }
    
    #endregion
    
    #region Methods
    
    /// <inheritdoc />
    public override string Build()
    {
        var sqlTemplate = "select * from {0} as {1}({2}) {3}";

        return string.Format(
            sqlTemplate,
            BuildRows(),
            typeof(T).Name + Constants.EntitySuffix,
            string.Join(',',
                this.memoryEntityMapping.MemoryEntityProperties.Select(
                    memoryEntityProperty => memoryEntityProperty.Name)),
            (this.memoryEntities.Any()) ? string.Empty : "where 1=0"
        );
    }

    #endregion
}