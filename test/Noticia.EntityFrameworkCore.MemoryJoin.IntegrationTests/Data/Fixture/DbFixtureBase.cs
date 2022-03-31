using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Noticia.EntityFrameworkCore.MemoryJoin.IntegrationTests.Data.Fixture;

/// <summary>
/// Base class for different database fixtures for testing.
/// </summary>
public abstract class DbFixtureBase
{
    #region Fields

    /// <summary>
    /// Loaded configuration for this test project.
    /// </summary>
    protected IConfiguration configuration;

    /// <summary>
    /// <see cref="DbContextOptionsBuilder"/> for connecting to the database.
    /// </summary>
    protected DbContextOptionsBuilder<TestDbContext> dbContextOptionsBuilder;
    
    #endregion
    
    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="DbFixtureBase"/> class.
    /// </summary>
    protected DbFixtureBase()
    {
        var configBuilder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

        this.configuration = configBuilder.Build();
        
        this.dbContextOptionsBuilder = new DbContextOptionsBuilder<TestDbContext>();
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