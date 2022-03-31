using System;
using Microsoft.EntityFrameworkCore;

namespace Noticia.EntityFrameworkCore.MemoryJoin.IntegrationTests.Data.Fixture;

/// <summary>
/// Fixture to automatically inject <see cref="TestDbContext"/>.
/// </summary>
public class SqliteDbFixture : DbFixtureBase
{
    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="NpgsqlDbFixture"/> class.
    /// </summary>
    public SqliteDbFixture() : base()
    {
        this.dbContextOptionsBuilder.UseSqlite(this.configuration["SqliteConnection"] ??
                                               throw new ArgumentNullException());
        this.PrepareDatabase();
    }

    #endregion
}