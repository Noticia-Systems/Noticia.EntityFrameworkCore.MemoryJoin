using System.Data.Common;

namespace Noticia.EntityFrameworkCore.MemoryJoin.Data.Interfaces;

/// <summary>
/// Builds the <see cref="DbParameter"/>s for given models.
/// </summary>
public interface IDbParameterBuilder
{
    #region Methods

    /// <summary>
    /// Builds the <see cref="DbParameter"/>s for given models.
    /// </summary>
    /// <returns><see cref="DbParameter"/>s generated from the models.</returns>
    IEnumerable<DbParameter> Build();

    #endregion
}