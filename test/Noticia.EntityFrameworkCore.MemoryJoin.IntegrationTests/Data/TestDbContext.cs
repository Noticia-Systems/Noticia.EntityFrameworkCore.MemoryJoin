using Microsoft.EntityFrameworkCore;
using Noticia.EntityFrameworkCore.MemoryJoin.Extensions;
using Noticia.EntityFrameworkCore.MemoryJoin.UnitTests.Models;

namespace Noticia.EntityFrameworkCore.MemoryJoin.IntegrationTests.Data;

/// <summary>
/// <see cref="DbContext"/> for unit testing.
/// </summary>
public class TestDbContext : DbContext
{
    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="TestDbContext"/> class.
    /// </summary>
    /// <param name="options">Options to initialize the <see cref="TestDbContext"/> with.</param>
    public TestDbContext(
        DbContextOptions<TestDbContext> options
    ) : base(options)
    {
    }

    #endregion

    #region Methods

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.MemoryEntity<TestModel>();
    }

    #endregion
    
    #region Properties
    
    public virtual DbSet<JoinableTableModel>? JoinableTableModels { get; set; }
    
    #endregion
}