using System.Linq.Expressions;
using System.Reflection;
using Noticia.EntityFrameworkCore.MemoryJoin.Reflection;

namespace Noticia.EntityFrameworkCore.MemoryJoin.Data;

/// <summary>
/// Allows for the mapping of models to memory entity and backwards.
/// </summary>
public class MemoryEntityMapping<T> where T : new()
{
    #region Fields

    /// <summary>
    /// LINQ expression to get the model.
    /// </summary>
    private readonly ParameterExpression toParameter = Expression.Parameter(typeof(T), "model");

    /// <summary>
    /// LINQ expression to get the memory entity.
    /// </summary>
    private readonly ParameterExpression fromParameter =
        Expression.Parameter(MemoryEntityManager.GetMemoryEntityType<T>(), "memoryEntity");

    /// <summary>
    /// Constructor for the model.
    /// </summary>
    private readonly ConstructorInfo toConstructor =
        MemoryEntityManager.GetMemoryEntityType<T>()
            .GetConstructor(BindingFlags.Instance | BindingFlags.Public, Type.EmptyTypes) ??
        throw new NullReferenceException("Memory entity type doesn't have a parameterless constructor.");

    /// <summary>
    /// Constructor for the memory entity.
    /// </summary>
    private readonly ConstructorInfo fromConstructor =
        typeof(T).GetConstructor(BindingFlags.Instance | BindingFlags.Public, Type.EmptyTypes) ??
        throw new NullReferenceException("Model type doesn't have a parameterless constructor.");

    /// <summary>
    /// All mappings to convert the model to the memory entity.
    /// </summary>
    private readonly IDictionary<PropertyInfo, Expression> toMappings = new Dictionary<PropertyInfo, Expression>();

    /// <summary>
    /// All mappings to convert a memory entity back to its model.
    /// </summary>
    private readonly IDictionary<PropertyInfo, Expression> fromMappings = new Dictionary<PropertyInfo, Expression>();

    #endregion

    #region Properties

    /// <summary>
    /// All <see cref="PropertyInfo"/>s of the model
    /// </summary>
    public PropertyInfo[] ModelProperties { get; }
    
    /// <summary>
    /// All <see cref="PropertyInfo"/>s of the memory entity.
    /// </summary>
    public PropertyInfo[] MemoryEntityProperties { get; }

    /// <summary>
    /// LINQ expression to convert the model to a memory entity.
    /// </summary>
    public LambdaExpression ToExpression { get; }

    /// <summary>
    /// Compiled LINQ expression to convert the model to a memory entity.
    /// </summary>
    public Func<T, object> ToFunc { get; }

    /// <summary>
    /// LINQ expression to convert a memory entity back to its model.
    /// </summary>
    public LambdaExpression FromExpression { get; }

    #endregion

    #region Constructors
    
    /// <summary>
    /// Initializes a new instance of the <see cref="MemoryEntityMapping{T}"/> class.
    /// </summary>
    public MemoryEntityMapping()
    {
        this.ModelProperties = typeof(T).GetProperties().ToArray();
        this.MemoryEntityProperties = MemoryEntityManager.GetMemoryEntityType<T>().GetProperties().ToArray();

        foreach (var modelMember in this.ModelProperties)
        {
            var memoryEntityMember =
                MemoryEntityProperties.Single(memoryEntityMember => memoryEntityMember.Name == modelMember.Name);

            var toExpression = Expression.MakeMemberAccess(toParameter, modelMember);
            var fromExpression = Expression.MakeMemberAccess(fromParameter, memoryEntityMember);

            this.toMappings.Add(memoryEntityMember, toExpression);
            this.fromMappings.Add(modelMember, fromExpression);
        }

        var toNew = Expression.New(toConstructor);
        var toBind = Expression.MemberInit(toNew,
            this.toMappings.Select(toMapping => Expression.Bind(toMapping.Key, toMapping.Value)));

        this.ToExpression = Expression.Lambda(toBind, toParameter);
        this.ToFunc = (Func<T, object>)this.ToExpression.Compile();

        var fromNew = Expression.New(fromConstructor);
        var fromBind = Expression.MemberInit(fromNew,
            this.fromMappings.Select(fromMapping => Expression.Bind(fromMapping.Key, fromMapping.Value)));

        this.FromExpression = Expression.Lambda(fromBind, fromParameter);
    }

    #endregion
}