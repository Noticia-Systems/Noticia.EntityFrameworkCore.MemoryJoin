namespace Noticia.EntityFrameworkCore.MemoryJoin.Data;

public class DbParameterBuilder
{
    /* var dbParameters = memoryEntities.SelectMany((memoryEntity, memoryEntityIndex) =>
            this.memoryEntityMapping.MemoryEntityProperties.Select((memoryEntityProperty, propertyIndex) =>
            {
                var parameter = dbCommand.CreateParameter();
                var cellIndex = memoryEntityIndex * this.memoryEntityMapping.MemoryEntityProperties.Length +
                                propertyIndex;
                
                parameter.DbType = SqlMappingHelper.GetDbType(memoryEntityProperty.PropertyType);
                parameter.ParameterName = string.Format(ParameterNameTemplate, cellIndex);
                parameter.Value = (memoryEntityProperty.Name == "Id")
                    ? cellIndex
                    : memoryEntityProperty.GetValue(memoryEntity);

                return parameter;
            })
        );
*/
}