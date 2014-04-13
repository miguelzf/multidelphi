using crosspascal.parser;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using crosspascal.semantics;

namespace crosspascal.ast.nodes
{
	// override Equals but not GetHashCode warning
	#pragma warning disable 659


	#region ProceduralTypes

	/// <summary>
	/// Type of a Routine (function, procedure, method, etc)
	/// </summary>
	/// <remarks>
	/// The function return type must be an id.
	/// </remarks>
	public partial class ProceduralType : TypeNode
	{
		public ParametersSection @params;

		/// <summary>
		/// Function's return type. Must be null for every non-function routine.
		/// </summary>
		public TypeNode funcret { get; set; }

		public RoutineDirectives Directives { get; set; }

		public bool IsFunction { get { return funcret != null; } }

		public String KindName { get { return (IsFunction ? "function" : "procedure"); } }

		public ProceduralType(ParametersSection @params, TypeNode ret = null, RoutineDirectives dirs = null)
		{
			this.@params = @params;
			this.funcret = ret;
			if (dirs != null)
				Directives = dirs;
			if (ret != null)
				// TODO check and emit error if any parameter is named 'Result'
				@params.returnVar = new OutParamDeclaration("result", ret);
		}

		public override bool Equals(Object o)
		{
			if (o == null || !(o is ProceduralType))
				return false;

			ProceduralType ft = (ProceduralType)o;

			return funcret.Equals(ft.funcret) && Directives.Equals(ft.Directives) && @params.Equals(ft.@params);
		}
	}

	public class MethodType : ProceduralType
	{
		public MethodKind kind;

		public bool IsDefault { get { return kind == MethodKind.Default; } }
		public bool IsConstructor { get { return kind == MethodKind.Constructor; } }
		public bool IsDestructor { get { return kind == MethodKind.Destructor; } }

		public MethodType(ParametersSection @params, TypeNode ret = null,
						RoutineDirectives dirs = null, MethodKind kind = MethodKind.Default )
			: base(@params, ret, dirs)
		{
			this.kind = kind;
		}
	}

	#endregion


	#region Routines' Declarations

	/// <summary>
	/// Declaration of a Callable unit, i.e. a global routine or method
	/// </summary>
	public abstract partial class CallableDeclaration : Declaration
	{
		/// <summary>
		/// Gets this callable Procedural Type (downcasted from the Declaration's base type)
		/// </summary>
		public ProceduralType Type
		{
			get { return (ProceduralType) this.type; }
		}

		/// <summary>
		/// Gets this callable type's Directives
		/// </summary>
		public RoutineDirectives Directives
		{
			get { return Type.Directives; }
			set {
				if (value != null)	// cannot set to null!
					Type.Directives = value;
				}
		}

		/// <summary>
		/// Returns wether this callable is a function or procedue
		/// </summary>
		public bool IsFunction { get { return Type.IsFunction; } }

		/// <summary>
		/// Gets the fully qualified name of this callable 
		/// (obj+metname for methods, plus parameter types for overloads)
		/// To be set by the Resolver
		/// </summary>
		public String QualifiedName;

		/// <summary>
		/// Gets the full method name, withhout parameters
		/// </summary>
		public abstract String Fullname();

		/// <summary>
		/// Section that declares this callable. 
		/// To be set by resolver
		/// </summary>
		public Section declaringSection;

		public CallableDeclaration(string name, ParametersSection @params, TypeNode ret = null,
									RoutineDirectives dirs = null)
			: base(name, new ProceduralType(@params, ret, dirs))
		{
		}

		/// <summary>
		/// Construct from a procedural type
		/// </summary>
		public CallableDeclaration(string name, ProceduralType type)
			: base(name, type)
		{
		}
	}

	/// <summary>
	/// Declaration of a global Routine
	/// </summary>
	public class RoutineDeclaration : CallableDeclaration
	{
		public override string Fullname()
		{
			return name;
		}

		public RoutineDeclaration(string name, ParametersSection @params, TypeNode ret = null, 
									RoutineDirectives dirs = null)
			: base(name, @params, ret, dirs)
		{
			if (Directives == null)
				Directives = new RoutineDirectives();
		}

		/// <summary>
		/// Construct from a procedural type
		/// </summary>
		public RoutineDeclaration(string name, ProceduralType type)
			: base(name, type)
		{
			if (Directives == null)
				Directives = new RoutineDirectives();
		}
	}

	/// <summary>
	/// Declaration of a Method
	/// </summary>
	public class MethodDeclaration : CallableDeclaration, IScopedDeclaration
	{
		public bool isStatic { get; set; }
		public String objname;

		private string fullname;
		public override string Fullname()
		{
			return fullname;
		}

		public new MethodType Type
		{
			get { return (MethodType)this.type; }
		}

		#region IScopedDeclaration implementation

		public Scope scope;
		public void SetScope(Scope s)
		{
			scope = s;
		}
		public Scope GetScope()
		{
			return scope;
		}

		public CompositeType declaringObject;
		public CompositeType GetDeclaringObject()
		{
			return declaringObject;
		}
		public void SetDeclaringObject(CompositeType t)
		{
			declaringObject = t;
		}

		#endregion

		public MethodDeclaration(string objname, string name, ParametersSection @params, TypeNode ret = null,
								RoutineDirectives dirs = null, MethodKind kind = MethodKind.Default)
			: base(name, new MethodType(@params, ret, dirs, kind))
		{
			this.objname = objname;
			isStatic = false;
			fullname = objname + "." + name;

			if (Directives == null)
				Directives = new MethodDirectives();

			foreach (var param in @params.decls)
				if (param.name == "self")
					throw new IdentifierRedeclared("Method parameter cannot shadow 'self' reference");
		}
	}

	public enum MethodKind
	{
		Default,
		Constructor,
		Destructor
	}
	

	/// <summary>
	/// Routine definition (implementation)
	/// </summary>
	public class RoutineDefinition : RoutineDeclaration
	{
		public RoutineSection body;

		public RoutineDefinition(string name, ParametersSection @params, TypeNode ret = null,
								RoutineDirectives dirs = null,  RoutineSection body = null)
			: base(name, @params, ret, dirs)
		{
			this.body = body;
			body.declaringCallable = this;
		}

		/// <summary>
		/// Construct from a procedural type
		/// </summary>
		public RoutineDefinition(string name, ProceduralType type,
								RoutineDirectives dirs = null, RoutineSection body = null)
			: base(name, type)
		{
			this.Directives.Add(dirs);
			this.body = body;
		}
	}

	/// <summary>
	/// Method definition (implementation)
	/// </summary>
	public class MethodDefinition : MethodDeclaration
	{
		private RoutineSection _body;
		public RoutineSection body
		{
			get { return _body; }
			set
			{	_body = value;
				if (value != null)
					_body.declaringCallable = this;
			}
		}

		public MethodDefinition(string objname, string name, ParametersSection @params,
								TypeNode ret = null,  RoutineDirectives dirs = null,
								MethodKind kind = MethodKind.Default, RoutineSection body = null)
			: base(objname, name, @params, ret, dirs, kind)
		{
			this.body = body;
		}
	}

	#endregion



	#region Directives' Aggregators

	/// <summary>
	/// Callable Units Directives
	/// </summary>
	public class RoutineDirectives : Node
	{
		private CallConvention _callconv = 0;
		public CallConvention   Callconv
		{
			get { return _callconv ; }
			set {
				if (_callconv != 0)
					Error("Cannot specify more than one Call convention (previous: " 
						+ Enum.GetName(typeof(CallConvention), _callconv) + ", new: "
						+  Enum.GetName(typeof(CallConvention), value) +")");
				else
					_callconv = value; 
			}
		}

		HashSet<GeneralDirective> generaldirs = new HashSet<GeneralDirective>();

		public RoutineDirectives(int dir = 0)
		{
			_callconv = 0;
			if (dir != 0)
				Add(dir);
		}

		public virtual void Add(RoutineDirectives dirs)
		{
			if (dirs == null)
				return;
			if (dirs.Callconv != 0)
				Callconv = dirs.Callconv;
			foreach (GeneralDirective dir in dirs.generaldirs)
				generaldirs.Add(dir);
		}

		public virtual void Add(int dir)
		{
			if (Enum.IsDefined(typeof(GeneralDirective), dir))
				generaldirs.Add((GeneralDirective)dir);
			else if (Enum.IsDefined(typeof(CallConvention), dir))
				Callconv = (CallConvention)dir;
			else
				Error("Invalid routine diretive");
		}

		public virtual bool Contains(int dir)
		{
			if (Enum.IsDefined(typeof(GeneralDirective), dir))
				return generaldirs.Contains((GeneralDirective)dir);
			else if (Enum.IsDefined(typeof(CallConvention), dir))
				return Callconv == (CallConvention)dir;
			else
				return Error("Invalid routine diretive");
		}

		/// <summary>
		/// Checks the immediate coherence between function directives.
		/// Must be called after all directives are added
		/// </summary>
		/// <returns></returns>
		internal virtual bool CheckDirectives()
		{
			bool ret = true;
			if (_callconv == 0)
				_callconv = CallConvention.Register;

			if (generaldirs.Contains(GeneralDirective.VarArgs) && Callconv != CallConvention.CDecl)
				ret |= Error("Varargs directive can only be used with the Cdecl calling convention");

			return ret;
		}

		public override bool Equals(object o)
		{
			if (o == null || !(o is RoutineDirectives))
				return false;
			RoutineDirectives rtype = (RoutineDirectives) o;
			return Callconv == rtype.Callconv && generaldirs.SequenceEqual(rtype.generaldirs);
		}
	}


	/// <summary>
	/// Routine Directives
	/// </summary>
	public class ImportDirectives : RoutineDirectives
	{
		private ImportDirective _importdir = ImportDirective.Default;
		public ImportDirective Importdir
		{
			get { return _importdir; }
			set
			{
				if (_importdir != ImportDirective.Default)
					Error("Cannot specify more than one external/forward directive");
				else _importdir = value;
			}
		}

		private ExternalDirective _external;
		public ExternalDirective External
		{
			get { return _external; }
			set
			{	_external = value;
				Importdir = ImportDirective.External;
			}
		}

		public ImportDirectives(ImportDirective dir)
		{
			Importdir = dir;
		}

		public ImportDirectives(int dir = 0)
		{
			Add(dir);
		}

		public override void Add(int dir = 0)
		{
			if (Enum.IsDefined(typeof(ImportDirective), dir))
				Importdir = (ImportDirective) dir;
			else 
				base.Add(dir);
		}

		public override void Add(RoutineDirectives dirs)
		{
			base.Add(dirs);
		}

		public override bool Contains(int dir)
		{
			if (Enum.IsDefined(typeof(ImportDirective), dir))
				return Importdir == (ImportDirective)dir;
			else
				return base.Contains(dir);
		}

		/// <summary>
		/// Checks the immediate coherence between function directives.
		/// Must be called after all directives are added
		/// </summary>
		/// <returns></returns>
		internal override bool CheckDirectives()
		{
			return base.CheckDirectives();
		}

		public override bool Equals(object o)
		{
			if (!base.Equals(o))
				return false;
			var ot = (ImportDirectives) o;
			return Importdir == ot.Importdir && External.Equals(ot.External);
		}
	}

	/// <summary>
	/// Method Directives
	/// </summary>
	public class MethodDirectives : RoutineDirectives
	{
		public HashSet<MethodDirective> methoddirs = new HashSet<MethodDirective>();

		public MethodDirectives(int dir = 0) : base(dir) { }

		public override void Add(int dir)
		{
			if (Enum.IsDefined(typeof(MethodDirective), dir))
				methoddirs.Add((MethodDirective)dir);
			else
				base.Add(dir);
		}

		public void Add(MethodDirectives dirs)
		{
			base.Add(dirs);
			foreach (MethodDirective dir in dirs.methoddirs)
				methoddirs.Add(dir);
		}

		public override bool Contains(int dir)
		{
			if (Enum.IsDefined(typeof(MethodDirective), dir))
				return methoddirs.Contains((MethodDirective)dir);
			else
				return base.Contains(dir);
		}

		/// <summary>
		/// Checks the immediate coherence between function directives.
		/// Must be called after all directives are added
		/// </summary>
		/// <returns></returns>
		internal override bool CheckDirectives()
		{
			base.CheckDirectives();

			if (methoddirs.Contains(MethodDirective.Override) && methoddirs.Contains(MethodDirective.Abstract))
				Error("Method cannot be have both Override and Abstract directives");

			if (methoddirs.Contains(MethodDirective.Abstract) && !methoddirs.Contains(MethodDirective.Virtual))
				Error("Abstract Method must also be Virtual");

			if (methoddirs.Contains(MethodDirective.Dynamic) && !methoddirs.Contains(MethodDirective.Virtual))
				Error("Method cannot be both Dynamic and Virtual");

			return true;
		}

		public override bool Equals(object o)
		{
			return base.Equals(o) && methoddirs.SequenceEqual(((MethodDirectives) o).methoddirs);
		}
	}

	#endregion


	#region Directives' Constants

	public class ExternalDirective
	{
		public Expression File { get; set; }
		public Expression Name { get; set; }

		public ExternalDirective(Expression file, Expression name = null)
		{
			File = file;
			Name = name;

			File.EnforceConst = true;
			File.ForcedType = StringType.Single;

			if (name != null)
			{
				Name.EnforceConst = true;
				Name.ForcedType = StringType.Single;
			}
		}

		/// <summary>
		/// Compares strings File and Name
		/// Previous constant folding required.
		/// </summary>
		public override bool Equals(object obj)
		{
			if (!(obj is ExternalDirective))
				return false;

			var ed = (ExternalDirective)obj;
			return File.Equals(ed.File) && Name.Equals(ed.Name);
		}
	}


	/// <summary>
	/// Directives constraints:
	///		Override | Abstract
	///		Abstract => virtual
	///		varargs => cdecl
	/// </summary>

	public enum MethodDirective
	{
		Abstract = 1000,
		Override,
		Virtual,		// optimised for memory
		Dynamic,		// same as Virtual. optimised for speed
		Reintroduce,	// suppress warnings when shadowing inherited methods (~= C#'s 'new' qualifier)
	}

	public enum GeneralDirective
	{
		Overload = 2000,
		Assembler,		// routine body must be defined in ASM
		Export,		// export function 
		Inline,
		VarArgs,		// for C Cdecl varargs
		Far,
		Near,
		Resident,
	}

	public enum ImportDirective
	{
		Default = 3000,
		External,
		Forward,
	}

	public enum CallConvention
	{
		Pascal = 4000,
		SafeCall,
		StdCall,
		CDecl,
		Register
	}

	#endregion

}