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
	public class DeclarationsEnvironment
	{
		public SymbolGraph<Declaration, Section> symEnv = new SymbolGraph<Declaration, Section>();

		// probably won't be needed
		Dictionary<String, Declaration> builtinDecls = new Dictionary<String, Declaration>();

		int DebugLevel = 1;

		public DeclarationsEnvironment()
		{
			LoadRuntimeNames();
		}

		void Debug(string msg)
		{
			if (DebugLevel > 0)
				Console.WriteLine(msg);
		}


		/// <summary>
		/// Call this method before using the environment.
		/// Resets the current context to the start
		/// </summary>
		public void InitEnvironment()
		{
			symEnv.Reset();
			EnterNextContext();	// runtime
			EnterNextContext();	// global
		}

		public bool EnterNextContext()
		{
			return symEnv.EnterNextContext();
		}

		void LoadRuntimeNames()
		{
			symEnv.CreateContext("runtime");
			LoadBuiltinTypesBasic();
			LoadBuiltinTypesPointer();
			LoadBuiltinTypesWindows();	// test
			LoadbuiltinDecls();
			LoadBuiltinComposites();
			symEnv.CreateContext("global");
		}



		/// <summary>
		/// Register new declaration, of any kind
		/// </summary>
		public void RegisterDeclaration(Declaration decl)
		{
			RegisterDeclaration(decl.name, decl);
		}

		/// <summary>
		/// Register new declaration, of any kind
		/// </summary>
		public void RegisterDeclaration(String name, Declaration decl, bool checkCanAdd = true)
		{
			if (name == null || decl == null)
				throw new InternalSemanticError("trying to register null declaration");
			
			if (checkCanAdd)
			{
				if (!symEnv.Add(name, decl))
					throw new IdentifierRedeclared(name);
			}
			else	// add without checking
				symEnv.GetContext().Add(name, decl);

			Debug("Register Decl " + name); // + Environment.NewLine + symtab.ListTable(3));
		}

		/// <summary>
		/// Fetch declaration denoted by a given name.
		/// General method, for any kind of declaration.
		/// </summary>
		public Declaration GetDeclaration(String name)
		{
			var ret = symEnv.Lookup(name);
			return ret;
		}


		
		#region Contexts Lookup

		/// <summary>
		/// Gets the section that encloses the current context
		/// </summary>
		public Section GetDeclaringSection()
		{
			return symEnv.GetContext().Key;
		}

		/// <summary>
		/// Searches for a specific type section enclosing the current context
		/// </summary>
		public Sec GetDeclaringContext<Sec>() where Sec : Section
		{
			var ctx = symEnv.GetContext();
			while (ctx != null && !(ctx.Key is Sec))
				ctx = ctx.GetFirstParent();
			return ctx.Key as Sec;
		}

		/// <summary>
		/// Gets the object section (class, interface or record) enclosing the current context, if any
		/// </summary>
		public ObjectSection GetDeclaringObjectSection()
		{
			return GetDeclaringContext<ObjectSection>();
		}

		/// <summary>
		/// Gets the callable section (routine or method) enclosing the current context, if any
		/// </summary>
		public RoutineSection GetDeclaringCallableSection()
		{
			return GetDeclaringContext<RoutineSection>();
		}

		/// <summary>
		/// Gets the object (class, interface or record) that encloses the current context, if any
		/// </summary>
		public CompositeType GetDeclaringObject()
		{
			var sec = GetDeclaringObjectSection();
			if (sec == null)
				return null;
			return sec.declaringObject;
		}

		/// <summary>
		/// Gets the declaration of the object (class, interface or record) that encloses the current context, if any
		/// </summary>
		public CompositeDeclaration GetDeclaringObjectDeclaration()
		{
			var ctx = symEnv.GetContext();
			while (ctx != null && !(ctx.Key is ObjectSection))
				ctx = ctx.GetFirstParent();

			if (ctx == null || ctx.Key == null)
				return null;
			// get the last declaration of the previous context
			return ctx.GetFirstParent().lastInserted as CompositeDeclaration;
		}

		/// <summary>
		/// Checks if current context is inside an object (method usually).
		/// If given an argument, checks if this ObjectSection is the defining scope
		/// </summary>
		public bool IsContextInObject(ObjectSection sec = null)
		{
			var osec = GetDeclaringObjectSection();
			return (sec != null && !ReferenceEquals(osec, sec));
		}

		/// <summary>
		/// Fetch the current context's declaring routine
		/// </summary>
		public CallableDeclaration GetDeclaringRoutine()
		{
			var sec = GetDeclaringCallableSection();
			if (sec == null)
				return null;
			return sec.declaringCallable;
		}

		/// <summary>
		/// Fetch declaration denoted by a given name, starting the search in the parent class
		/// </summary>
		public Declaration GetInheritedDeclaration(String name)
		{
			CompositeType obj = GetDeclaringObject();
			if (obj == null)
				return null;
			return obj.GetInheritableMember(name);
		}
		
		/// <summary>
		/// Fetch declaration denoted by a given name in the given section
		/// </summary>
		public Declaration GetDeclarationInScope(String name, Section sec)
		{
			return symEnv.Lookup(name, sec);
		}

		#endregion

		
		#region Context Management

		public void CreateContext(string id = null, Section sec = null, bool allowShadowing = true)
		{
			symEnv.CreateContext(id, sec, allowShadowing);
			Debug("CREATE CONTEXT " + id);
		//	Debug(symEnv.ListGraphFromCurrent(3));
		}

		public void CreateParentContext(string id = null, Section sec = null, bool allowShadowing = true)
		{
			symEnv.CreateParentContext(id, sec, allowShadowing);
			Debug("CREATE PARENT CONTEXT " + id);
		}

		public void EnterContext()
		{
			string id = symEnv.EnterContext();
			Debug("ENTER CONTEXT " + id);
		}

		public String ExitContext()
		{
			string id = symEnv.ExitContext();
			Debug("EXIT CONTEXT " + id);
			return id;
		}

		#endregion	// Context Management


		#region Management of Interface Contexts

		/// <summary>
		/// Import external context. To load used/imported units
		/// </summary>
		internal void ImportUsedContext(SymbolContext<Declaration, Section> ctx)
		{
			ctx.allowShadowing = true;
			Debug("IMPORT CONTEXT " + ctx.Id);
			symEnv.ImportParentCtxToFirst(ctx);
		}

		/// <summary>
		/// Export current context. Should be a unit interface context,
		/// to later be able to load used/import this unit
		/// </summary>
		internal SymbolContext<Declaration, Section> ExportInterfaceContext()
		{
			var ctx = symEnv.ExportCopyContext();
			Debug("EXPORT CONTEXT " + ctx.Id);
			return ctx;
		}

		#endregion	// Management of Interface Contexts


		#region Loading of Class/Interface Contexts

		/// <summary>
		/// recursively Import contexts of ancestors as parent contexts of current
		/// </summary>
		CompositeType LoadAncestors(CompositeType type)
		{
			// Load the inherited interfaces as parallel contexts
			for (int i = 1; i < type.ancestors.Count; i++)
			{
				symEnv.ImportParentCtxToFirst(type.ancestors[i].inheritableContext);
				symEnv.ExitContext();
			}

			// No real multiple inheritance in Delphi. Load recursively only the 1st inherit
			if (type.ancestors.Count > 0)
			{
				symEnv.ImportParentCtxToFirst(type.ancestors[0].inheritableContext);
				LoadAncestors(type.ancestors[0]);
				symEnv.ExitContext();
			}

			return type;
		}

		/// <summary>
		/// Sets references to inherited types
		/// </summary> 
		void SetAncestors(CompositeType type)
		{
			foreach (string s in type.heritage)
			{
				var cdecl = GetDeclaration(s) as CompositeDeclaration;
				if (cdecl == null)
					throw new CompositeNotFound(s);
				type.ancestors.Add(cdecl.Type);
			}
		}

		/// <summary>
		/// Loads all inherited contexts, creates and loads all members in its own context
		/// For method definitions, that need to access their declaring class context
		/// </summary>
		public CompositeType CreateCompositeContext(String cname)
		{
			var decl = GetDeclaration(cname) as CompositeDeclaration;
			if (decl == null)
				throw new CompositeNotFound(cname);
			CompositeType type = decl.Type;

			// create context of its own class
			symEnv.ImportContext(type.privateContext);
			// load inheritable contexts
			LoadAncestors(type);
			return type;
		}

		/// <summary>
		/// Loads all herited ancestor contexts, and creates in its own context
		/// For classes/interfaces to access their inherited ancestors
		/// </summary>
		public void CreateInheritedContext(CompositeType type)
		{
			SetAncestors(type);

			// create own context
			CreateContext(type.Name, type.section);
			// load inheritable contexts
			LoadAncestors(type);
		}

		/// <summary>
		/// Exits all inherited contexts + its own
		/// </summary>
		public void ExitInheritedContext(CompositeType type)
		{
			// export inheritable context to be loaded by subclasses
			var pred = new Func<Declaration, bool>(x => (x as IScopedDeclaration).GetScope() != Scope.Private);
			type.inheritableContext = symEnv.ExportCloneContext(pred);
			type.privateContext = symEnv.ExportCopyContext();

			ExitContext();	// exit own class context
		}

		/// <summary>
		/// Enters all inherited contexts + its own
		/// </summary>
		public void EnterCompositeContext(CompositeType type)
		{
			EnterContext();		// enter class context
		}

		/// <summary>
		/// Exits all inherited contexts + its own
		/// </summary>
		public void ExitCompositeContext(CompositeType type)
		{
			ExitContext();	// exit own class context
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
			builtinDecls.Add(name, decl);
			if (!symEnv.Add(name, decl))
				throw new IdentifierRedeclared(name);
		}

		void CreateBuiltinFunction(RoutineDeclaration routine)
		{
			builtinDecls.Add(routine.name, routine);
			RegisterDeclaration(routine);
		}

		/// <summary>
		/// Load the built-in basic types.
		/// </summary>
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

		public void LoadbuiltinDecls()
		{
			var tfloat	= FloatType.Single;
			var tint	= IntegerType.Single;
			var tstring = StringType.Single;
			var tarray  = ArrayType.Single;
			var p = new ParamDeclaration("x", tfloat);

			string[] floatFuncs = { "cos", "sin", "trunc", "round", "frac", "exp" };
			string[] intordFuncs  = { "succ", "pred"};
			string[] stringsFuncs = { "writeln", "readln"};

			foreach (var s in floatFuncs)
				CreateBuiltinFunction(new RoutineDeclaration(s, new ParametersSection(
										new DeclarationList(new ParamDeclaration("x", tfloat))), tfloat));

			foreach (var s in intordFuncs)
				CreateBuiltinFunction(new RoutineDeclaration(s, new ParametersSection(
										new DeclarationList(new ParamDeclaration("x", null))), tint));

			foreach (var s in stringsFuncs)
				CreateBuiltinFunction(new RoutineDeclaration(s, new ParametersSection(
										new DeclarationList(new ParamDeclaration("x", tstring)))));

			CreateBuiltinFunction(new RoutineDeclaration("assigned", new ParametersSection(
									new DeclarationList(new ParamDeclaration("p", PointerType.Single))), BoolType.Single));

			CreateBuiltinFunction(new RoutineDeclaration("length", new ParametersSection(
									new DeclarationList(new ParamDeclaration("x", tarray))), tint));

			var decls = new DeclarationList(new ParamDeclaration("arr", tarray));
			decls.Add(new ParamDeclaration("len", tint));
			CreateBuiltinFunction(new RoutineDeclaration("setlength", new ParametersSection(decls)));
		}

		public void LoadBuiltinComposites()
		{
			var decl = new InterfaceDeclaration("tinterfacedobject", new InterfaceType(null, new ObjectSection()));
			decl.Type.inheritableContext = new SymbolContext<Declaration, Section>(decl.name, decl.Type.section);
			decl.Type.privateContext	 = new SymbolContext<Declaration, Section>(decl.name, decl.Type.section);

			decl.Type.Name = decl.name;
			builtinDecls.Add(decl.name, decl);
			if (!symEnv.Add(decl.name, decl))
				throw new IdentifierRedeclared(decl.name);
			
		}

		#endregion

	}
}
