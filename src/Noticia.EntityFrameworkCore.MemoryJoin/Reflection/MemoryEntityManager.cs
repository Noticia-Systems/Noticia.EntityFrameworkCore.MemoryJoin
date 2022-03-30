using System.Collections.Concurrent;
using Noticia.EntityFrameworkCore.MemoryJoin.Data;

namespace Noticia.EntityFrameworkCore.MemoryJoin.Reflection;

/// <summary>
/// Manager class for caching the generated memory entities.
/// </summary>
public static class MemoryEntityManager
{
    #region Static Fields

    /// <summary>
    /// <see cref="MemoryEntityBuilder"/> to generate new memory entities.
    /// </summary>
    private static readonly MemoryEntityBuilder memoryEntityBuilder = new();

    /// <summary>
    /// Dictionary containing the models and their assigned memory entities.
    /// </summary>
    private static readonly Dictionary<Type, Type> modelsToMemoryEntities = new();

    /// <summary>
    /// Dictionary containing assignments of models to their <see cref="MemoryEntityMapping{T}"/>.
    /// </summary>
    private static readonly Dictionary<Type, object> modelsToMappings = new();

    #endregion

    #region Static Methods

    /// <summary>
    /// Gets (and builds) the memory entity for the given model type.
    /// </summary>
    /// <typeparam name="T">Model type.</typeparam>
    /// <returns>Memory entity type for the given model.</returns>
    public static Type GetMemoryEntityType<T>()
    {
        lock (modelsToMemoryEntities)
        {
            if (!modelsToMemoryEntities.ContainsKey(typeof(T)))
            {
                modelsToMemoryEntities.Add(typeof(T), memoryEntityBuilder.Build<T>());
            }

            return modelsToMemoryEntities[typeof(T)];
        }
    }

    /// <summary>
    /// Gets (and creates) the <see cref="MemoryEntityMapping{T}"/> for a given model type.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static MemoryEntityMapping<T> GetMemoryEntityMapping<T>() where T : new()
    {
        lock (modelsToMappings)
        {
            if (!modelsToMappings.ContainsKey(typeof(T)))
            {
                modelsToMappings.Add(typeof(T), new MemoryEntityMapping<T>());
            }

            return (MemoryEntityMapping<T>)modelsToMappings[typeof(T)];
        }
    }

    #endregion
}