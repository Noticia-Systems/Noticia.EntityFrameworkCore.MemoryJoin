/*
 License: http://www.apache.org/licenses/LICENSE-2.0
 Home page: https://github.com/DapperLib/Dapper-dot-net
 
 Mappings and GetDbType method largely taken from https://github.com/DapperLib/Dapper/blob/main/Dapper/SqlMapper.cs.
*/

using System.Data;

namespace Noticia.EntityFrameworkCore.MemoryJoin.Data.Helper;

/// <summary>
/// Helper class to map .NET types to their respective SQL types.
/// </summary>
public static class SqlMappingHelper
{
    #region Static Fields
    
    /// <summary>
    /// Mappings of .NET types to their respective SQL types.
    /// </summary>
    private static readonly IDictionary<Type, DbType> SqlTypeMappings = new Dictionary<Type, DbType>()
    {
        { typeof(byte), DbType.Byte },
        { typeof(sbyte), DbType.SByte },
        { typeof(short), DbType.Int16 },
        { typeof(ushort), DbType.UInt16 },
        { typeof(int), DbType.Int32 },
        { typeof(uint), DbType.UInt32 },
        { typeof(long), DbType.Int64 },
        { typeof(ulong), DbType.UInt64 },
        { typeof(float), DbType.Single },
        { typeof(double), DbType.Double },
        { typeof(decimal), DbType.Decimal },
        { typeof(bool), DbType.Boolean },
        { typeof(string), DbType.String },
        { typeof(char), DbType.StringFixedLength },
        { typeof(Guid), DbType.Guid },
        { typeof(DateTime), DbType.DateTime },
        { typeof(DateTimeOffset), DbType.DateTimeOffset },
        { typeof(TimeSpan), DbType.Time },
        { typeof(byte[]), DbType.Binary },
        { typeof(byte?), DbType.Byte },
        { typeof(sbyte?), DbType.SByte },
        { typeof(short?), DbType.Int16 },
        { typeof(ushort?), DbType.UInt16 },
        { typeof(int?), DbType.Int32 },
        { typeof(uint?), DbType.UInt32 },
        { typeof(long?), DbType.Int64 },
        { typeof(ulong?), DbType.UInt64 },
        { typeof(float?), DbType.Single },
        { typeof(double?), DbType.Double },
        { typeof(decimal?), DbType.Decimal },
        { typeof(bool?), DbType.Boolean },
        { typeof(char?), DbType.StringFixedLength },
        { typeof(Guid?), DbType.Guid },
        { typeof(DateTime?), DbType.DateTime },
        { typeof(DateTimeOffset?), DbType.DateTimeOffset },
        { typeof(TimeSpan?), DbType.Time },
        { typeof(Object), DbType.Object }
    };
    
    #endregion
    
    #region Static Methods
    
    /// <summary>
    /// Retrieves the SQL type for a given .NET type.
    /// </summary>
    /// <param name="type">.NET type to find SQL equivalent for.</param>
    /// <returns>SQL type for the given .NET type.</returns>
    /// <exception cref="ArgumentException">Thrown when no mapping for the given type has been found.</exception>
    public static DbType GetDbType(Type type)
    {
        var nullUnderlyingType = Nullable.GetUnderlyingType(type);
        
        if (nullUnderlyingType != null) type = nullUnderlyingType;
        
        if (type.IsEnum)
        {
            type = Enum.GetUnderlyingType(type);
        }

        if (SqlTypeMappings.TryGetValue(type, out var dbType))
        {
            return dbType;
        }

        throw new ArgumentException($"Cannot map type {type.Name} to a sql type.");
    }
    
    #endregion
}