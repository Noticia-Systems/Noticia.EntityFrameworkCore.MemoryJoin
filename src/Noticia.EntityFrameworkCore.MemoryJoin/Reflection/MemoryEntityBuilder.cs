using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using System.Reflection.Emit;

namespace Noticia.EntityFrameworkCore.MemoryJoin.Reflection;

/// <summary>
/// The <see cref="MemoryEntityBuilder"/> builds new entities for passing memory values to EFcore for joining.
/// </summary>
public class MemoryEntityBuilder
{
    #region Constants

    /// <summary>
    /// <see cref="TypeAttributes"/> used to create the dynamic types.
    /// </summary>
    private const TypeAttributes TypeAttributes = System.Reflection.TypeAttributes.Public |
                                                  System.Reflection.TypeAttributes.Class |
                                                  System.Reflection.TypeAttributes.AutoClass |
                                                  System.Reflection.TypeAttributes.AnsiClass |
                                                  System.Reflection.TypeAttributes.BeforeFieldInit |
                                                  System.Reflection.TypeAttributes.AutoLayout;

    /// <summary>
    /// <see cref="MethodAttributes"/> used to create the Id property on the dynamic types.
    /// </summary>
    private const MethodAttributes PropertyMethodAttributes = MethodAttributes.Public |
                                                              MethodAttributes.SpecialName |
                                                              MethodAttributes.HideBySig;

    /// <summary>
    /// <see cref="MethodAttributes"/> used to create the dynamic types constructor.
    /// </summary>
    private const MethodAttributes ConstructorAttributes = MethodAttributes.Public |
                                                           MethodAttributes.SpecialName |
                                                           MethodAttributes.RTSpecialName;

    #endregion

    #region Static Fields

    /// <summary>
    /// Reflection reference to the <see cref="KeyAttribute"/> constructor.
    /// </summary>
    private static readonly ConstructorInfo KeyAttributeConstructor =
        typeof(KeyAttribute).GetConstructor(Type.EmptyTypes) ?? throw new InvalidOperationException();

    /// <summary>
    /// Reflection reference to the <see cref="DatabaseGeneratedAttribute"/> constructor.
    /// </summary>
    private static readonly ConstructorInfo DatabaseGeneratedAttributeConstructor =
        typeof(DatabaseGeneratedAttribute).GetConstructor(new[] { typeof(DatabaseGeneratedOption) }) ??
        throw new InvalidOperationException();

    /// <summary>
    /// Reflection reference to the <see cref="ColumnAttributeConstructor"/> constructor.
    /// </summary>
    private static readonly ConstructorInfo ColumnAttributeConstructor =
        typeof(ColumnAttribute).GetConstructor(new[] { typeof(string) }) ??
        throw new InvalidOperationException();

    #endregion

    #region Fields

    /// <summary>
    /// <see cref="AssemblyBuilder"/> used to generate a dynamic assembly.
    /// </summary>
    private readonly AssemblyBuilder assemblyBuilder;

    /// <summary>
    /// <see cref="ModuleBuilder"/> used to generate the main module within the dynamic assembly.
    /// </summary>
    private readonly ModuleBuilder moduleBuilder;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="MemoryEntityBuilder"/> class.
    /// </summary>
    public MemoryEntityBuilder()
    {
        this.assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(
            new AssemblyName("MemoryEntities"),
            AssemblyBuilderAccess.Run
        );

        this.moduleBuilder = assemblyBuilder.DefineDynamicModule("MainModule");
    }

    #endregion

    #region Methods

    /// <summary>
    /// Builds a new dynamic entity type based on a given model.
    /// </summary>
    /// <typeparam name="T">Type of the model.</typeparam>
    /// <returns>Type of the built dynamic entity.</returns>
    /// <exception cref="InvalidOperationException">Thrown whenever the type could not be built successfully and thus returned null.</exception>
    public Type Build<T>()
    {
        return Build(typeof(T));
    }
    
    /// <summary>
    /// Builds a new dynamic entity type based on a given model.
    /// </summary>
    /// <param name="type">Type of the model.</param>
    /// <returns>Type of the built dynamic entity.</returns>
    /// <exception cref="InvalidOperationException">Thrown whenever the type could not be built successfully and thus returned null.</exception>
    public Type Build(Type type)
    {
        if (type.GetProperty("Id") is not null)
        {
            throw new InvalidOperationException("Model for memory entity generation must not have the property 'Id'.");
        }

        var typeBuilder =
            this.moduleBuilder.DefineType(type.Name + Constants.EntitySuffix, TypeAttributes); //, typeof(T));

        var idPropertyBuilder = AddProperty(typeBuilder, "Id", typeof(int));
        
        var keyAttributeBuilder = new CustomAttributeBuilder(KeyAttributeConstructor, Array.Empty<object>());
        var databaseGeneratedAttributeBuilder = new CustomAttributeBuilder(DatabaseGeneratedAttributeConstructor,
            new object[] { DatabaseGeneratedOption.Identity });
        var columnAttributeBuilder = new CustomAttributeBuilder(ColumnAttributeConstructor, new object[] { "id" });

        idPropertyBuilder.SetCustomAttribute(keyAttributeBuilder);
        idPropertyBuilder.SetCustomAttribute(databaseGeneratedAttributeBuilder);
        idPropertyBuilder.SetCustomAttribute(columnAttributeBuilder);
        
        foreach (var propertyInfo in type.GetProperties())
        {
            var attributeBuilder = new CustomAttributeBuilder(ColumnAttributeConstructor,
                new object[] { propertyInfo.Name.ToLower() });
            var propertyBuilder = AddProperty(typeBuilder, propertyInfo.Name, propertyInfo.PropertyType);

            propertyBuilder.SetCustomAttribute(attributeBuilder);
        }

        typeBuilder.DefineDefaultConstructor(ConstructorAttributes);

        return typeBuilder.CreateType() ?? throw new InvalidOperationException();
    }

    private static PropertyBuilder AddProperty(TypeBuilder typeBuilder, string propertyName, Type propertyType)
    {
        var fieldBuilder = typeBuilder.DefineField($"_{propertyName}", propertyType, FieldAttributes.Private);
        var propertyBuilder =
            typeBuilder.DefineProperty(propertyName, PropertyAttributes.HasDefault, propertyType, null);

        var getMethodBuilder =
            typeBuilder.DefineMethod($"get_{propertyName}", PropertyMethodAttributes, propertyType, Type.EmptyTypes);
        var getIl = getMethodBuilder.GetILGenerator();

        getIl.Emit(OpCodes.Ldarg_0);
        getIl.Emit(OpCodes.Ldfld, fieldBuilder);
        getIl.Emit(OpCodes.Ret);

        var setMethodBuilder =
            typeBuilder.DefineMethod($"set_{propertyName}", PropertyMethodAttributes, null, new[] { propertyType });
        var setIl = setMethodBuilder.GetILGenerator();
        var modifyLabel = setIl.DefineLabel();
        var exitLabel = setIl.DefineLabel();

        setIl.MarkLabel(modifyLabel);
        setIl.Emit(OpCodes.Ldarg_0);
        setIl.Emit(OpCodes.Ldarg_1);
        setIl.Emit(OpCodes.Stfld, fieldBuilder);
        setIl.Emit(OpCodes.Nop);
        setIl.MarkLabel(exitLabel);
        setIl.Emit(OpCodes.Ret);

        propertyBuilder.SetGetMethod(getMethodBuilder);
        propertyBuilder.SetSetMethod(setMethodBuilder);

        return propertyBuilder;
    }

    #endregion
}