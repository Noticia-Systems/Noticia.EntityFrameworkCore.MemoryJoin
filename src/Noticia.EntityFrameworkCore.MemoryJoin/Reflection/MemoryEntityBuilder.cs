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
        if (typeof(T).GetProperty("Id") is not null)
        {
            throw new InvalidOperationException("Model for memory entity generation must not have the property 'Id'.");
        }

        var typeBuilder = this.moduleBuilder.DefineType(typeof(T).Name + Constants.EntitySuffix, TypeAttributes, typeof(T));
        var fieldBuilder = typeBuilder.DefineField("_Id", typeof(int), FieldAttributes.Private);
        var propertyBuilder = typeBuilder.DefineProperty("Id", PropertyAttributes.HasDefault, typeof(int), null);

        var getMethodBuilder =
            typeBuilder.DefineMethod("get_Id", PropertyMethodAttributes, typeof(int), Type.EmptyTypes);
        var getIl = getMethodBuilder.GetILGenerator();

        getIl.Emit(OpCodes.Ldarg_0);
        getIl.Emit(OpCodes.Ldfld, fieldBuilder);
        getIl.Emit(OpCodes.Ret);

        var setMethodBuilder =
            typeBuilder.DefineMethod("set_Id", PropertyMethodAttributes, null, new[] { typeof(int) });
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

        var keyAttributeBuilder = new CustomAttributeBuilder(KeyAttributeConstructor, Array.Empty<object>());
        var databaseGeneratedAttributeBuilder = new CustomAttributeBuilder(DatabaseGeneratedAttributeConstructor,
            new object[] { DatabaseGeneratedOption.Identity });

        propertyBuilder.SetCustomAttribute(keyAttributeBuilder);
        propertyBuilder.SetCustomAttribute(databaseGeneratedAttributeBuilder);

        typeBuilder.DefineDefaultConstructor(ConstructorAttributes);

        return typeBuilder.CreateType() ?? throw new InvalidOperationException();
    }

    #endregion
}