using System.Data.Common;

namespace Noticia.EntityFrameworkCore.MemoryJoin.Data.Interfaces;

public interface IDbParameterBuilder
{
    #region Methods

    IEnumerable<DbParameter> Build<T>(IEnumerable<T> models);

    #endregion
}