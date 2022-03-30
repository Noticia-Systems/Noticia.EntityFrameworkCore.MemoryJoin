namespace Noticia.EntityFrameworkCore.MemoryJoin;

/// <summary>
/// Constants used in this library.
/// </summary>
public static class Constants
{
    #region Constants
    
    /// <summary>
    /// Template for naming the SQL parameters.
    /// </summary>
    public const string ParameterNameTemplate = "@memoryEntity_{0}";

    /// <summary>
    /// Suffix that is applied to the base types name for differentiation.
    /// </summary>
    public const string EntitySuffix = "_entity";

    #endregion
}