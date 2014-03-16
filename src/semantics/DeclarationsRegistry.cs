using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using crosspascal.ast.nodes;

namespace crosspascal.semantics
{
	/// <summary>
	/// Registry for declarations.
	/// Wrapper over a symbol table
	/// </summary>
	public class DeclarationsRegistry
	{
		SymbolTable<Declaration> symtab = new SymbolTable<Declaration>();
		// probably won't be needed
		Dictionary<String, TypeDeclaration> builtinTypes = new Dictionary<String, TypeDeclaration>();

		public DeclarationsRegistry()
		{
		}

		public void LoadRuntimeNames()
		{
			symtab.Push("runtime");
			LoadBuiltinTypesBasic();
			LoadBuiltinTypesPointer();
			LoadBuiltinTypesWindows();	// test
		}

		/// <summary>
		/// FIXME!!
		/// </summary>
		public void RegisterPreviousDeclaration(String name, Declaration decl)
		{
			if (!symtab.AddToPrevious(name, decl))
				throw new IdentifierRedeclared(name);
		}


		/// <summary>
		/// Register new declaration, of any kind
		/// </summary>
		public void RegisterDeclaration(String name, Declaration decl)
		{
			if (!symtab.Add(name, decl))
				throw new IdentifierRedeclared(name);
		}

		/// <summary>
		/// Fetch declaration denoted by a given name.
		/// General method, for any kind of declaration.
		/// </summary>
		public Declaration GetDeclaration(String name)
		{
			return symtab.Lookup(name);
		}

		public void EnterContext()
		{
			symtab.Push();
		}
		public void LeaveContext()
		{
			symtab.Pop();
		}


		#region Lookup of Type Declarations

		/// <summary>
		/// Parametric Check function for TypeDeclaration. Returns null if not found
		/// </summary>
		public T CheckType<T>(String name) where T : TypeNode
		{
			Declaration decl = GetDeclaration(name);
			if (decl == null || !decl.ISA(typeof(TypeDeclaration)))
				return null;

			TypeNode tdecl = ((TypeDeclaration)decl).type;
			if (!tdecl.ISA( typeof(T)))
				return null;
			return (T) tdecl;
		}

		/// <summary>
		/// Fetch Type Declaration. 
		/// Fails if declaration is not of a Type
		/// </summary>
		public TypeNode FetchType(String name)
		{
			Declaration decl = GetDeclaration(name);

			if (decl == null)
				throw new IdentifierUndeclared(name);

			if (!decl.ISA(typeof(TypeDeclaration)))
				throw new InvalidIdentifier("Identifier '" + name + "' does not refer to a type");

			TypeDeclaration tdecl = (TypeDeclaration)decl;
			return tdecl.type;
		}

		/// <summary>
		/// Fetch Type Declaration, and tests if it's equal or derived from a specific TypeNode
		/// Fails if declaration is not of a Type
		/// </summary>
		public TypeNode FetchType(String name, System.Type expected)
		{
			TypeNode type = FetchType(name);
			if (type == null || !type.ISA(expected))
				throw new InvalidIdentifier("Invalid type '" + type + "'. Required '" + expected + "'");
			return type;
		}

		/// <summary>
		/// Parametric Fetch function for TypeDeclaration
		/// </summary>
		public T FetchType<T>(String name) where T : TypeNode
		{
			return (T)FetchType(name, typeof(T));
		}

		public ScalarType FetchTypeScalar(String name)
		{
			return (ScalarType) FetchType(name, typeof(ScalarType));
		}

		public IntegralType FetchTypeIntegral(String name)
		{
			return (IntegralType)FetchType(name, typeof(IntegralType));
		}

		public ProceduralType FetchTypeProcedural(String name)
		{
			return (ProceduralType)FetchType(name, typeof(ProceduralType));
		}

		#endregion


		#region Lookup of Value Declarations

		/// <summary>
		/// Parametric Check function for ValueDeclaration. Returns null if not found
		/// </summary>
		public T CheckValue<T>(String name) where T : ValueDeclaration
		{
			Declaration decl = GetDeclaration(name);
			if (decl == null || !decl.ISA(typeof(ValueDeclaration)))
				return null;

			ValueDeclaration vdecl = ((ValueDeclaration)decl);
			if (!vdecl.ISA(typeof(T)))
				return null;
			return (T) vdecl;
		}

		/// <summary>
		/// Fetch Type Declaration. 
		/// Fails if declaration is not of a Type
		/// </summary>
		public ValueDeclaration FetchValue(String name)
		{
			Declaration decl = GetDeclaration(name);

			if (decl == null)
				throw new IdentifierUndeclared(name);

			if (!decl.ISA(typeof(ValueDeclaration)))
				throw new InvalidIdentifier("Identifier '" + name + "' does not refer to a value decla");

			ValueDeclaration tdecl = (ValueDeclaration)decl;
			return tdecl;
		}

		/// <summary>
		/// Fetch Type Declaration, and tests if it's equal or derived from a specific TypeNode
		/// Fails if declaration is not of a Type
		/// </summary>
		public ValueDeclaration FetchValue(String name, System.Type expected)
		{
			ValueDeclaration vdecl = FetchValue(name);
			if (vdecl == null || !vdecl.ISA(expected))
				throw new InvalidIdentifier("Invalid value '" + vdecl + "'. Required '" + expected + "'");
			return vdecl;
		}

		/// <summary>
		/// Parametric Fetch function for ValueDeclarations
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public T FetchValue<T>(String name) where T : ValueDeclaration
		{
			return (T) FetchValue(name, typeof(T));
		}

		public VarDeclaration FetchVariable(String name)
		{
			return (VarDeclaration)FetchValue(name, typeof(VarDeclaration));
		}

		public ConstDeclaration FetchConstant(String name)
		{
			return (ConstDeclaration)FetchValue(name, typeof(ConstDeclaration));
		}

		public ParameterDeclaration FetchParameter(String name)
		{
			return (ParameterDeclaration)FetchValue(name, typeof(ParameterDeclaration));
		}

		#endregion


		#region Loading of Built-in types

		void CreateBuiltinType(String name, VariableType type)
		{
			builtinTypes.Add(name, new TypeDeclaration(name, type));
		}

		/// <summary>
		/// Load the built-in basic types.
		/// </summary>
		/// <remarks>
		/// Warning! Cannot be called from the constructor. Creates a circular dependency where 
		/// for each created built-in, the Declaration tries to register the typename in the global TRegister
		/// </remarks>
		public void LoadBuiltinTypesBasic()
		{
			CreateBuiltinType("boolean", BoolType.Single);
			CreateBuiltinType("shortstring", StringType.Single);
			CreateBuiltinType("widestr", StringType.Single);
			CreateBuiltinType("real48", DoubleType.Single);
			CreateBuiltinType("single", FloatType .Single);
			CreateBuiltinType("double", DoubleType.Single);
			CreateBuiltinType("extended", ExtendedType.Single);
			CreateBuiltinType("currency", CurrencyType.Single);
			CreateBuiltinType("byte", UnsignedInt8Type.Single);
			CreateBuiltinType("integer",  SignedInt32Type.Single);
			CreateBuiltinType("shortint", SignedInt8Type .Single);
			CreateBuiltinType("smallint",  SignedInt16Type.Single);
			CreateBuiltinType("longint", SignedInt32Type.Single);
			CreateBuiltinType("int64", SignedInt64Type.Single);
			CreateBuiltinType("uint64", UnsignedInt64Type.Single);
			CreateBuiltinType("word", UnsignedInt16Type.Single);
			CreateBuiltinType("longword", UnsignedInt32Type.Single);
			CreateBuiltinType("cardinal", UnsignedInt32Type.Single);
			CreateBuiltinType("comp", SignedInt64Type.Single);
			CreateBuiltinType("char", CharType.Single);
			CreateBuiltinType("widechar", CharType.Single);
			CreateBuiltinType("variant", new VariantType());
			CreateBuiltinType("olevariant", new VariantType());
		}

		/// <summary>
		/// Load the built-in pointer types.
		/// </summary>
		/// <remarks>
		/// Warning! Cannot be called from the constructor. Creates a circular dependency where 
		/// for each created built-in, the Declaration tries to register the typename in the global TRegister
		/// </remarks>
		public void LoadBuiltinTypesPointer()
		{
			CreateBuiltinType("pchar", new PointerType(StringType.Single));
			CreateBuiltinType("pboolean", new PointerType(BoolType.Single));
			CreateBuiltinType("pbyte", new PointerType(BoolType.Single));

			CreateBuiltinType("pshortstring", new PointerType(StringType.Single));
			CreateBuiltinType("pwidestr", new PointerType(StringType.Single));
			CreateBuiltinType("preal48", new PointerType(DoubleType.Single));
			CreateBuiltinType("psingle", new PointerType(FloatType.Single));
			CreateBuiltinType("pdouble", new PointerType(DoubleType.Single));
			CreateBuiltinType("pextended", new PointerType(ExtendedType.Single));
			CreateBuiltinType("pcurryency", new PointerType(CurrencyType.Single));
			CreateBuiltinType("pinteger", new PointerType(SignedInt32Type.Single));
			CreateBuiltinType("pshortint", new PointerType(SignedInt8Type.Single));
			CreateBuiltinType("psmallint", new PointerType(SignedInt16Type.Single));
			CreateBuiltinType("plongint", new PointerType(SignedInt32Type.Single));
			CreateBuiltinType("pint64", new PointerType(SignedInt64Type.Single));
			CreateBuiltinType("puint64", new PointerType(UnsignedInt64Type.Single));
			CreateBuiltinType("pword", new PointerType(UnsignedInt16Type.Single));
			CreateBuiltinType("plongword", new PointerType(UnsignedInt32Type.Single));
			CreateBuiltinType("pcardinal", new PointerType(UnsignedInt32Type.Single));
			CreateBuiltinType("pcomp", new PointerType(SignedInt64Type.Single));
			CreateBuiltinType("pwidechar", new PointerType(CharType.Single));
			CreateBuiltinType("pvariant", new PointerType(new VariantType()));
			CreateBuiltinType("polevariant", new PointerType(new VariantType()));
		}

		public void LoadBuiltinTypesWindows()
		{
			CreateBuiltinType("thandle", IntegerType.Single);
		}

		#endregion

	}
}
