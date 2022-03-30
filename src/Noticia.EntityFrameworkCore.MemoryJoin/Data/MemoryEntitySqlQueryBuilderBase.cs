using Noticia.EntityFrameworkCore.MemoryJoin.Data.Interfaces;

namespace Noticia.EntityFrameworkCore.MemoryJoin.Data;

/// <summary>
/// Shared base for building the SQL queries for memory data.
/// </summary>
/// <typeparam name="T">Model type.</typeparam>
/// <inheritdoc cref="IMemoryEntitySqlQueryBuilder"/>
public abstract class MemoryEntitySqlQueryBuilderBase<T> : IMemoryEntitySqlQueryBuilder where T : new()
{
    #region Fields
    
    /// <summary>
    /// Mapping used to interchange models and memory entities.
    /// </summary>
    protected readonly MemoryEntityMapping<T> memoryEntityMapping;

    /// <summary>
    /// Memory entities built from the given models.
    /// </summary>
    protected readonly ICollection<object> memoryEntities;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="MemoryEntitySqlQueryBuilderBase{T}"/> class.
    /// </summary>
    /// <param name="memoryEntityMapping">Mapping used to interchange models and memory entities.</param>
    /// <param name="memoryEntities">Memory entities to supply to the SQL server.</param>
    /// <exception cref="ArgumentNullException">Thrown when <see cref="memoryEntityMapping"/> is null.</exception>
    protected MemoryEntitySqlQueryBuilderBase(MemoryEntityMapping<T> memoryEntityMapping, ICollection<object> memoryEntities)
    {
        this.memoryEntityMapping = memoryEntityMapping ?? throw new ArgumentNullException(nameof(memoryEntityMapping));

        this.memoryEntities = memoryEntities;
    }
    
    #endregion

    #region Methods
    
    /// <inheritdoc />
    public abstract string Build();

    /// <summary>
    /// Builds the SQL for the memory entities as rows.
    /// </summary>
    /// <returns>SQL query for the memory entities as rows.</returns>
    protected string BuildRows()
    {
        if (this.memoryEntities.Any())
        {
            return
                $"(values {string.Join(',', memoryEntities.Select((memoryEntity, memoryEntityIndex) => $"({string.Join(',', this.memoryEntityMapping.MemoryEntityProperties.Select((memoryEntityProperty, propertyIndex) => string.Format(Constants.ParameterNameTemplate, memoryEntityIndex * memoryEntityMapping.MemoryEntityProperties.Length + propertyIndex)))})"))})";
        }

        return
            $"(values ({string.Join(',', Enumerable.Repeat("NULL", this.memoryEntityMapping.MemoryEntityProperties.Length))}))";
    }
    
    #endregion
}