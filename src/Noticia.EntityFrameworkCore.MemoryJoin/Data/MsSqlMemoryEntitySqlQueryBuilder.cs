using Noticia.EntityFrameworkCore.MemoryJoin.Reflection;

namespace Noticia.EntityFrameworkCore.MemoryJoin.Data;

/// <summary>
/// MsSQL builder for the memory entities.
/// </summary>
/// <typeparam name="T">Model type.</typeparam>
/// <inheritdoc cref="MemoryEntitySqlQueryBuilderBase{T}"/>
public class MsSqlMemoryEntitySqlQueryBuilder<T> : MemoryEntitySqlQueryBuilderBase<T> where T : new()
{
    #region Constructors
    
    /// <inheritdoc />
    public MsSqlMemoryEntitySqlQueryBuilder(MemoryEntityMapping<T> memoryEntityMapping, ICollection<object> memoryEntities) : base(
        memoryEntityMapping, memoryEntities)
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