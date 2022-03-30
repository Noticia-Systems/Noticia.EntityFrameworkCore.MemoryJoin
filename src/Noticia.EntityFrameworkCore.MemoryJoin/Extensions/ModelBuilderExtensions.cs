using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Noticia.EntityFrameworkCore.MemoryJoin.Reflection;

namespace Noticia.EntityFrameworkCore.MemoryJoin.Extensions;

/// <summary>
/// Extension methods for <see cref="ModelBuilder"/>.
/// </summary>
public static class ModelBuilderExtensions
{
    #region Static Methods

    /// <summary>
    /// Creates and adds a memory entity to the <see cref="DbContext"/>.
    /// </summary>
    /// <param name="modelBuilder"><see cref="ModelBuilder"/> for the <see cref="DbContext"/>.</param>
    /// <typeparam name="T">Model type the memory entity shall be based on.</typeparam>
    /// <returns><see cref="EntityTypeBuilder"/>.</returns>
    public static EntityTypeBuilder MemoryEntity<T>(this ModelBuilder modelBuilder)
    {
        return modelBuilder.Entity(MemoryEntityManager.GetMemoryEntityType<T>());
    }
    
    #endregion
}