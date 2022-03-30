using Noticia.EntityFrameworkCore.MemoryJoin.Reflection;

namespace Noticia.EntityFrameworkCore.MemoryJoin.Data;

/// <summary>
/// MsSQL builder for the memory entities.
/// </summary>
/// <typeparam name="T">Model type.</typeparam>
/// <inheritdoc cref="MemoryEntitySqlQueryBuilderBase{T}"/>
public class SqliteMemoryEntitySqlQueryBuilder<T> : MemoryEntitySqlQueryBuilderBase<T> where T : new()
{
    #region Constructors
    
    /// <inheritdoc />
    public SqliteMemoryEntitySqlQueryBuilder(MemoryEntityMapping<T> memoryEntityMapping, ICollection<object> memoryEntities) : base(
        memoryEntityMapping, memoryEntities)
    {
    }
    
    #endregion
    
    #region Methods
    
    /// <inheritdoc />
    public override string Build()
    {
        var sqlTemplate = "select * from ( WITH {0}({1}) AS {2} SELECT * FROM {3}) {4}";
        
        return string.Format(
            sqlTemplate,
            typeof(T).Name + Constants.EntitySuffix,
            string.Join(',',
                this.memoryEntityMapping.MemoryEntityProperties.Select(
                    memoryEntityProperty => memoryEntityProperty.Name)),
            BuildRows(),
            typeof(T).Name + Constants.EntitySuffix,
            (this.memoryEntities.Any()) ? string.Empty : "where 1=0"
        );
    }

    #endregion
}