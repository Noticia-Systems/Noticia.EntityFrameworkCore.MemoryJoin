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
    private static readonly MemoryEntityBuilder memoryEntityBuilder = new MemoryEntityBuilder();

    /// <summary>
    /// Dictionary containing the models and their assigned memory entities.
    /// </summary>
    private static readonly Dictionary<Type, Type> modelsToMemoryEntities = new Dictionary<Type, Type>();

    #endregion

    #region Static Methods

    /// <summary>
    /// Gets (and builds) the memory entity for the given model type.
    /// </summary>
    /// <typeparam name="T">Model type.</typeparam>
    /// <returns>Memory entity type for the given model.</returns>
    public static Type GetMemoryEntityType<T>()
    {
        if (!modelsToMemoryEntities.ContainsKey(typeof(T)))
        {
            modelsToMemoryEntities.Add(typeof(T), memoryEntityBuilder.Build<T>());
        }

        return modelsToMemoryEntities[typeof(T)];
    }

    #endregion
}