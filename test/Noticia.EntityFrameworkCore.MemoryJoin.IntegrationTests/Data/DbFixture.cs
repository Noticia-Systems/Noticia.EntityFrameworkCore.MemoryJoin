using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Noticia.EntityFrameworkCore.MemoryJoin.IntegrationTests.Data;

/// <summary>
/// Base class for different database fixtures for testing.
/// </summary>
public class DbFixture
{
    #region Fields

    /// <summary>
    /// Loaded configuration for this test project.
    /// </summary>
    private IConfiguration configuration;

    /// <summary>
    /// <see cref="DbContextOptionsBuilder"/> for connecting to the database.
    /// </summary>
    private DbContextOptionsBuilder<TestDbContext> dbContextOptionsBuilder;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="DbFixture"/> class.
    /// </summary>
    public DbFixture()
    {
        var configBuilder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddEnvironmentVariables();

        this.configuration = configBuilder.Build();
        this.dbContextOptionsBuilder = new DbContextOptionsBuilder<TestDbContext>();

        var connectionString = this.configuration.GetConnectionString("Default") ?? throw new ArgumentNullException();

        switch (this.configuration["DatabaseProvider"])
        {
            case "Postgres":
                this.dbContextOptionsBuilder.UseNpgsql(connectionString);
                break;
            case "MSSQL":
                this.dbContextOptionsBuilder.UseSqlServer(connectionString);
                break;
            case "SQLite":
                this.dbContextOptionsBuilder.UseSqlite(connectionString);
                break;
            default:
                throw new NotSupportedException("Database provider unsupported.");
        }
        
        this.PrepareDatabase();
    }

    #endregion

    #region Methods

    /// <summary>
    /// Creates a new <see cref="TestDbContext"/> for testing.
    /// </summary>
    /// <returns><see cref="TestDbContext"/>.</returns>
    public TestDbContext CreateContext()
    {
        return new TestDbContext(dbContextOptionsBuilder.Options);
    }

    /// <summary>
    /// Deletes and recreates the database for testing.
    /// </summary>
    public void PrepareDatabase()
    {
        using (var dbContext = this.CreateContext())
        {
            dbContext.Database.EnsureDeleted();
            dbContext.Database.EnsureCreated();
        }
    }

    #endregion
}