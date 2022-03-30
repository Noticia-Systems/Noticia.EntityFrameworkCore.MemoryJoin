namespace Noticia.EntityFrameworkCore.MemoryJoin.Data.Interfaces;

/// <summary>
/// Base definition for building the SQL query used for memory data.
/// </summary>
public interface IMemoryEntitySqlQueryBuilder
{
    #region Methods

    /// <summary>
    /// Builds a SQL query that provides the memory data.
    /// </summary>
    /// <returns></returns>
    public string Build();

    #endregion
}