namespace Noticia.EntityFrameworkCore.MemoryJoin.Data.Interfaces;

/// <summary>
/// Base definition for a class building queryable models.
/// </summary>
public interface IQueryableModelsBuilder
{
    #region Methods

    /// <summary>
    /// Builds queryable models.
    /// </summary>
    /// <param name="models">Models to make queryable.</param>
    /// <typeparam name="T">Model type.</typeparam>
    /// <returns>Queryable models.</returns>
    IQueryable<T> Build<T>(IEnumerable<T> models) where T : new();

    #endregion
}