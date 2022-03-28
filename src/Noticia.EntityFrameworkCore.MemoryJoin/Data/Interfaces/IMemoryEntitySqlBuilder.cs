namespace Noticia.EntityFrameworkCore.MemoryJoin.Data.Interfaces;

public interface IMemoryDataSqlService
{
    #region Methods

    public string GetSql();

    public void AppendNullRow();

    #endregion
}