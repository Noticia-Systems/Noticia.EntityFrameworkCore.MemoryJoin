using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Noticia.EntityFrameworkCore.MemoryJoin.IntegrationTests.Data.Fixture;

namespace Noticia.EntityFrameworkCore.MemoryJoin.IntegrationTests.Data.Fixture;

/// <summary>
/// Fixture to automatically inject <see cref="TestDbContext"/>.
/// </summary>
public class MsSqlDbFixture : DbFixtureBase
{
    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="MsSqlDbFixture"/> class.
    /// </summary>
    public MsSqlDbFixture() : base()
    {
        this.dbContextOptionsBuilder.UseSqlServer(this.configuration["MsSqlConnection"] ??
                                                  throw new ArgumentNullException());
        this.PrepareDatabase();
    }

    #endregion
}