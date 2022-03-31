using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Noticia.EntityFrameworkCore.MemoryJoin.IntegrationTests.Data.Fixture;

/// <summary>
/// Fixture to automatically inject <see cref="TestDbContext"/>.
/// </summary>
public class NpgsqlDbFixture : DbFixtureBase
{
    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="NpgsqlDbFixture"/> class.
    /// </summary>
    public NpgsqlDbFixture() : base()
    {
        this.dbContextOptionsBuilder.UseNpgsql(this.configuration["PostgresConnection"] ?? throw new ArgumentNullException());
        this.PrepareDatabase();
    }

    #endregion
}