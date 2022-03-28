using System.Linq.Expressions;
using System.Reflection;
using Noticia.EntityFrameworkCore.MemoryJoin.Reflection;

namespace Noticia.EntityFrameworkCore.MemoryJoin.Data;

/// <summary>
/// Allows for the mapping of models to memory entity and backwards.
/// </summary>
public class MemoryEntityMapper<T>
{
    #region Fields
    
    private readonly ParameterExpression toParameter = Expression.Parameter(typeof(T), "model");
    private readonly ParameterExpression fromParameter = Expression.Parameter(MemoryEntityManager.GetMemoryEntityType<T>(), "memoryEntity");

    private readonly ConstructorInfo toConstructor = MemoryEntityManager.GetMemoryEntityType<T>().GetConstructor(BindingFlags.Instance | BindingFlags.Public, Type.EmptyTypes) ?? throw new NullReferenceException("Memory entity type doesn't have a parameterless constructor.");
    private readonly ConstructorInfo fromConstructor = typeof(T).GetConstructor(BindingFlags.Instance | BindingFlags.Public, Type.EmptyTypes) ?? throw new NullReferenceException("Model type doesn't have a parameterless constructor.");
    
    private readonly ICollection<MemberInfo> modelMembers;
    private readonly ICollection<MemberInfo> memoryEntityMembers;
    
    #endregion
    
    #region Properties

    public IDictionary<MemberInfo, Expression> ToMappings { get; } = new Dictionary<MemberInfo, Expression>();

    public IDictionary<MemberInfo, Expression> FromMappings { get; } = new Dictionary<MemberInfo, Expression>();

    public Expression ToExpression { get; }
    public Expression FromExpression { get; }
    
    #endregion
    
    #region Constructors

    public MemoryEntityMapper()
    {
        this.modelMembers = typeof(T).GetProperties().Cast<MemberInfo>().ToList();
        this.memoryEntityMembers = typeof(T).GetProperties().Cast<MemberInfo>().ToList();
        
        foreach (var modelMember in this.modelMembers)
        {
            var memoryEntityMember = memoryEntityMembers.Single(memoryEntityMember => memoryEntityMember.Name == modelMember.Name);
            
            var toExpression = Expression.MakeMemberAccess(toParameter, modelMember);
            var fromExpression = Expression.MakeMemberAccess(fromParameter, memoryEntityMember);

            this.ToMappings.Add(memoryEntityMember, toExpression);
            this.FromMappings.Add(modelMember, fromExpression);
        }

        var toNew = Expression.New(toConstructor);
        var toBind = Expression.MemberInit(toNew,
            this.ToMappings.Select(toMapping => Expression.Bind(toMapping.Key, toMapping.Value)));

        this.ToExpression = Expression.Lambda(toBind, toParameter);
            
        var fromNew = Expression.New(fromConstructor);
        var fromBind = Expression.MemberInit(fromNew,
            this.FromMappings.Select(fromMapping => Expression.Bind(fromMapping.Key, fromMapping.Value)));

        this.FromExpression = Expression.Lambda(fromBind, fromParameter);
    }
    
    #endregion
}