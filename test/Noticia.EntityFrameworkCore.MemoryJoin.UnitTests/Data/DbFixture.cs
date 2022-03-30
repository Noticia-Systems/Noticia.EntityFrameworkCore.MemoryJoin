using System;
using Microsoft.EntityFrameworkCore;

namespace Noticia.EntityFrameworkCore.MemoryJoin.UnitTests.Data;

/// <summary>
/// Fixture to automatically inject <see cref="TestDbContext"/>.
/// </summary>
public class DbFixture : IDisposable
{
    #region Properties
    
    /// <summary>
    /// <see cref="TestDbContext"/> for testing.
    /// </summary>
    public TestDbContext TestDbContext { get; set; }

    #endregion
    
    #region Constructors
    
    /// <summary>
    /// Initializes a new instance of the <see cref="DbFixture"/> class.
    /// </summary>
    public DbFixture()
    {
        var builder = new DbContextOptionsBuilder<TestDbContext>();

        builder.UseInMemoryDatabase("Noticia.EntityFrameworkCore.MemoryJoin");

        this.TestDbContext = new TestDbContext(builder.Options);
        
        this.TestDbContext.Database.EnsureDeleted();
        this.TestDbContext.Database.EnsureCreated();
    }

    #endregion
    
    #region Methods
    
    /// <inheritdoc />
    public void Dispose()
    {
        this.TestDbContext.Database.EnsureDeleted();
        this.TestDbContext.Dispose();
    }
    
    #endregion
}