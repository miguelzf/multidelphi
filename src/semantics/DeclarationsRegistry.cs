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
		public SymbolTable<Declaration> symtab = new SymbolTable<Declaration>();

		// probably won't be needed
		Dictionary<String, TypeDeclaration> builtinTypes = new Dictionary<String, TypeDeclaration>();
		Dictionary<String, RoutineDeclaration> builtinFunctions = new Dictionary<String, RoutineDeclaration>();

		public DeclarationsRegistry()
		{
		}

		public void LoadRuntimeNames()
		{
			symtab.Push("runtime");
			LoadBuiltinTypesBasic();
			LoadBuiltinTypesPointer();
			LoadBuiltinTypesWindows();	// test
			LoadBuiltinFunctions();
			symtab.Push("global");
		}


		/// <summary>
		/// Import external context. To load used/imported units
		/// </summary>
		internal void ImportContext(SymbolContext<Declaration> ctx)
		{
			symtab.ImportCurrentContext(ctx);
		}

		/// <summary>
		/// Export current context, should be a unit interface context,
		/// to later be able to load used/import this unit
		/// </summary>
		internal SymbolContext<Declaration> ExportContext()
		{
			return symtab.ExportCurrentContext();
		}



		/// <summary>
		/// Checks if a declaration is a context-creation declaration (routine or object).
		/// If it is, then it has already opened a new context, and now the
		/// declaration must be registered in the previous context
		/// </summary>
		bool CheckContextCreation(String name, Declaration decl)
		{
			TypeNode tdecl = decl.type;

			if (decl is CallableDeclaration || decl is CompositeDeclaration
				// re-used record types have been already declared, so they never come here
			|| (decl is VariantDeclaration && tdecl is RecordType))
			{
				if (!symtab.AddToPrevious(name, decl))
					throw new IdentifierRedeclared(name);
				return true;
			}
			return false;
		}

		
		/// <summary>
		/// Checks if an assignment's lvalue is a call to its declaring function,
		/// in which case, it is actually a reference to the function's return var.
		/// Create the aproppriate node
		/// </summary>
		public LvalueExpression CheckSameFunctionAssignment(LvalueExpression lval)
		{
			// check that the expr is a funcall with a single id
			if (lval is RoutineCall && ((RoutineCall)lval).func is Identifier)
			{
				string name = ((Identifier)((RoutineCall)lval).func).name;
				// check that the id refers to the declaring function
				CallableDeclaration decl = GetDeclaratingRoutine();
				if (decl.IsFunction && decl.name == name)
					return new Identifier("result");	// standard name for func ret
			}

			return lval;
		}

		/// <summary>
		/// Register new declaration, of any kind
		/// </summary>
		public void RegisterDeclaration(String name, Declaration decl)
		{
			// check if the declaration creates a context (classes, functions, records)
		//	if (CheckContextCreation(name, decl))	// old code. TODO remove
	
			if (!symtab.Add(name, decl))
				throw new IdentifierRedeclared(name);

			Console.WriteLine("Register Decl " + name); // + Environment.NewLine + symtab.ListTable(3));
		}

		/// <summary>
		/// Fetch declaration denoted by a given name.
		/// General method, for any kind of declaration.
		/// </summary>
		public Declaration GetDeclaration(String name)
		{
			return symtab.Lookup(name);
		}

		public void EnterContext(string id = null)
		{
			symtab.Push(id);
			Console.WriteLine("CREATE CONTEXT " + id);
		}
		public String LeaveContext()
		{
			string id = symtab.Pop();
			Console.WriteLine("DESTROY CONTEXT " + id);
			return id;
		}


		#region Loading of Class/Interface Contexts

		/// <summary>
		/// Searches for the encloding declaring context function, if any.
		/// </summary>
		private CallableDeclaration GetDeclaratingRoutine()
		{
			Declaration decl = symtab.GetLastSymbolFromContext(1);
			if (decl is CallableDeclaration)
				return decl as CallableDeclaration;
			return null;
		}

		/// <summary>
		/// Searches for a declared name, in a composite-defined declaring context 
		/// (class or interface) that has declared the current method
		/// </summary>
		/// <returns></returns>
		private Declaration GetEnclosedDeclaration(String name)
		{
			var decl = GetDeclaratingRoutine() as MethodDeclaration;
			if (decl == null)
				return null;

			var cldecl = GetDeclaration(decl.objname) as CompositeDeclaration;
			if (cldecl == null)
				return null;

			return cldecl.Type.GetInheritableMembers().Where(x => x.name == decl.name).ElementAt(0);
		}

		public void LoadCompositeContext(CompositeDeclaration cdecl)
		{
			EnterContext(cdecl.name);
			foreach (var decl in cdecl.Type.GetInheritableMembers())
				symtab.Add(decl.name, decl);
		}

		public int LoadHeritageContext(String cname)
		{
			int nstates = 0;
			var decl = GetDeclaration(cname) as ClassDeclaration;
			if (decl == null)
				throw new CompositeNotFound(cname);

			// No real multiple inheritance in Delphi. Load only the 1st inherit, the class
			if (decl.Type.heritage.Count > 0)
				nstates += LoadHeritageContext(decl.Type.heritage[0]);

			LoadCompositeContext(decl);
			return nstates+1;
		}
		
		public void EnterMethodContext(String cname)
		{
			int nstatesLoaded = LoadHeritageContext(cname);		// class+heritage
			EnterContext(""+nstatesLoaded);	// top-level contetx
			EnterContext("method-params");	// method params context
		}

		public void LeaveMethodContext()
		{
			LeaveContext();	// method params
			string id = LeaveContext();
			Console.WriteLine("ID = " + id);
			int nInherited = Int32.Parse(id);
			for (int i = 0; i < nInherited; i++)
				LeaveContext();
		}

		#endregion


		Declaration FetchMethodOrField(String name)
		{
			var decl = GetDeclaration(name);
			if (decl is FieldDeclaration || decl is MethodDeclaration)
				return decl;
			throw new MethordOrFieldNotFound(name);
		}

		Declaration FetchField(String name)
		{
			var decl = GetDeclaration(name);
			if (decl is FieldDeclaration)
				return decl;
			throw new FieldNotFound(name);
		}


		#region Lookup of Type Declarations

		/// <summary>
		/// Parametric Check function for TypeDeclaration. Returns null if not found
		/// </summary>
		public T CheckType<T>(String name) where T : TypeNode
		{
			TypeDeclaration decl = GetDeclaration(name) as TypeDeclaration;
			if (decl == null || !(decl.type is T))
				return null;

			return (T)decl.type;
		}

		/// <summary>
		/// Fetch Type Declaration. 
		/// Fails if declaration is not of a Type
		/// </summary>
		public TypeNode FetchType(String name)
		{
			Declaration decl = GetDeclaration(name);
			if (decl == null)
				throw new DeclarationNotFound(name);

			if (!(decl is TypeDeclaration))
				throw new InvalidIdentifier("Identifier '" + name + "' does not refer to a type");

			return (decl as TypeDeclaration).type;
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
			ValueDeclaration decl = GetDeclaration(name) as ValueDeclaration;
			if (decl == null || !(decl is T))
				return null;
			return (T)decl;
		}

		/// <summary>
		/// Fetch Value Declaration. 
		/// Fails if declaration is not of a Type
		/// </summary>
		public ValueDeclaration FetchValue(String name)
		{
			Declaration decl = GetDeclaration(name);

			if (decl == null)
				throw new DeclarationNotFound(name);

			if (!(decl is ValueDeclaration))
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

		public ParamDeclaration FetchParameter(String name)
		{
			return (ParamDeclaration)FetchValue(name, typeof(ParamDeclaration));
		}

		#endregion


		#region Loading of Built-in types

		void CreateBuiltinType(String name, VariableType type)
		{
			var decl = new TypeDeclaration(name, type);
			builtinTypes.Add(name, decl);
			RegisterDeclaration(name, decl);
		}

		void CreateBuiltinFunction(RoutineDeclaration routine)
		{
			builtinFunctions.Add(routine.name, routine);
			RegisterDeclaration(routine.name, routine);
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
			CreateBuiltinType("real", DoubleType.Single);
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

		public void LoadBuiltinFunctions()
		{
			EnterContext(); CreateBuiltinFunction(new RoutineDeclaration("cos", new DeclarationList(new ParamDeclaration("x", FloatType.Single)), FloatType.Single)); LeaveContext();
			EnterContext(); CreateBuiltinFunction(new RoutineDeclaration("sin", new DeclarationList(new ParamDeclaration("x", FloatType.Single)), FloatType.Single)); LeaveContext();
			EnterContext(); CreateBuiltinFunction(new RoutineDeclaration("trunc", new DeclarationList(new ParamDeclaration("x", FloatType.Single)), FloatType.Single)); LeaveContext();
			EnterContext(); CreateBuiltinFunction(new RoutineDeclaration("round", new DeclarationList(new ParamDeclaration("x", FloatType.Single)), FloatType.Single)); LeaveContext();
			EnterContext(); CreateBuiltinFunction(new RoutineDeclaration("frac", new DeclarationList(new ParamDeclaration("x", FloatType.Single)), FloatType.Single)); LeaveContext();
			EnterContext(); CreateBuiltinFunction(new RoutineDeclaration("exp", new DeclarationList(new ParamDeclaration("x", FloatType.Single)), FloatType.Single)); LeaveContext();
			EnterContext(); CreateBuiltinFunction(new RoutineDeclaration("writeln", new DeclarationList(new ParamDeclaration("x", FloatType.Single)), FloatType.Single)); LeaveContext();
			EnterContext(); CreateBuiltinFunction(new RoutineDeclaration("readln", new DeclarationList(new ParamDeclaration("x", FloatType.Single)), FloatType.Single)); LeaveContext();
		}

		#endregion

	}
}
