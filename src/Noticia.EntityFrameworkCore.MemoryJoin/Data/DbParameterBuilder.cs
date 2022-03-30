using System.Data.Common;
using Noticia.EntityFrameworkCore.MemoryJoin.Data.Helper;
using Noticia.EntityFrameworkCore.MemoryJoin.Data.Interfaces;

namespace Noticia.EntityFrameworkCore.MemoryJoin.Data;

/// <inheritdoc cref="IDbParameterBuilder"/>
public class DbParameterBuilder<T> : IDbParameterBuilder where T : new()
{
    #region Fields

    /// <summary>
    /// Mapping used to interchange models and memory entities.
    /// </summary>
    private readonly MemoryEntityMapping<T> memoryEntityMapping;

    /// <summary>
    /// Memory entities built from the given models.
    /// </summary>
    private readonly ICollection<object> memoryEntities;

    /// <summary>
    /// <see cref="DbCommand"/> to create the <see cref="DbParameter"/> for.
    /// </summary>
    private readonly DbCommand dbCommand;
    
    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="DbParameterBuilder{T}"/> class.
    /// </summary>
    /// <param name="memoryEntityMapping">Mapping used to interchange models and memory entities.</param>
    /// <param name="memoryEntities">Memory entities to create <see cref="DbParameter"/>s for.</param>
    /// <param name="dbCommand"><see cref="DbCommand"/> to create the <see cref="DbParameter"/> for.</param>
    /// <exception cref="ArgumentNullException">Thrown when <see cref="memoryEntityMapping"/> is null.</exception>
    public DbParameterBuilder(MemoryEntityMapping<T> memoryEntityMapping, ICollection<object> memoryEntities, DbCommand dbCommand)
    {
        this.memoryEntityMapping = memoryEntityMapping ?? throw new ArgumentNullException(nameof(memoryEntityMapping));
        this.memoryEntities = memoryEntities;
        this.dbCommand = dbCommand;
    }

    #endregion

    #region Methods

    /// <inheritdoc />
    public IEnumerable<DbParameter> Build()
    {
        return this.memoryEntities.SelectMany((memoryEntity, memoryEntityIndex) =>
            this.memoryEntityMapping.MemoryEntityProperties.Select((memoryEntityProperty, propertyIndex) =>
            {
                var parameter = dbCommand.CreateParameter();
                var cellIndex = memoryEntityIndex * this.memoryEntityMapping.MemoryEntityProperties.Length +
                                propertyIndex;

                parameter.DbType = SqlMappingHelper.GetDbType(memoryEntityProperty.PropertyType);
                parameter.ParameterName = string.Format(Constants.ParameterNameTemplate, cellIndex);
                parameter.Value = (memoryEntityProperty.Name == "Id")
                    ? memoryEntityIndex + 1
                    : memoryEntityProperty.GetValue(memoryEntity);

                return parameter;
            })
        );
    }

    #endregion
}