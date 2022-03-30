﻿using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Noticia.EntityFrameworkCore.MemoryJoin.IntegrationTests.Data;

/// <summary>
/// Fixture to automatically inject <see cref="TestDbContext"/>.
/// </summary>
public class NpgsqlDbFixture
{
    #region Properties
    
    /// <summary>
    /// <see cref="TestDbContext"/> for testing.
    /// </summary>
    public TestDbContext TestDbContext { get; set; }

    #endregion
    
    #region Constructors
    
    /// <summary>
    /// Initializes a new instance of the <see cref="NpgsqlDbFixture"/> class.
    /// </summary>
    public NpgsqlDbFixture()
    {
        var configBuilder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddEnvironmentVariables();

        var config = configBuilder.Build();
        
        var builder = new DbContextOptionsBuilder<TestDbContext>();
        builder.UseNpgsql(config["PostgresConnection"]);

        var dbContextOptions = builder.Options;
        
        this.TestDbContext = new TestDbContext(dbContextOptions);
        
        this.TestDbContext.Database.EnsureDeleted();
        this.TestDbContext.Database.EnsureCreated();
    }

    #endregion
    
    #region Methods

    /// <summary>
    /// Deletes and recreates the database.
    /// </summary>
    public void Reset()
    {
        this.TestDbContext.Database.EnsureDeleted();
        this.TestDbContext.Database.EnsureCreated();
    }
    
    #endregion
}