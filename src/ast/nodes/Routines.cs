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
	public partial class ProceduralType : TypeNode
	{
		public DeclarationList @params;

		/// <summary>
		/// Function's return type. Must be null for every non-function routine.
		/// </summary>
		public TypeNode funcret { get; set; }
		public OutParamDeclaration returnVar;

		CallableDirectives _directives;
		public CallableDirectives Directives
		{
			get { return _directives; }
			set
			{
				if (value != null)
					value.CheckDirectives();
				_directives = value;
			}
		}

		public ProceduralType(DeclarationList @params, TypeNode ret = null, CallableDirectives dirs = null)
		{
			this.@params = @params;
			this.funcret = ret;
			if (dirs != null)
				Directives = dirs;
			if (ret != null)
				// TODO check and emit error if any parameter is named 'Result'
				returnVar = new OutParamDeclaration("result", ret);
		}

		public override bool Equals(Object o)
		{
			if (o == null || !(o is ProceduralType))
				return false;

			ProceduralType ft = (ProceduralType)o;

			return funcret.Equals(ft.funcret) && Directives.Equals(ft.Directives)
				&& @params.SequenceEqual(ft.@params);
		}
	}

	public class MethodType : ProceduralType
	{
		public MethodType(DeclarationList @params, TypeNode ret = null, CallableDirectives dirs = null)
			: base(@params, ret, dirs)
		{ 
		}
	}

	#endregion


	#region Routines' Declarations

	/// <summary>
	/// Declaration of a procedural type, i.e. Callable unit
	/// </summary>
	public abstract partial class CallableDeclaration : TypeDeclaration
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
		public CallableDirectives Directives
		{
			get { return Type.Directives; }
			set { Type.Directives = value;}
		}

		public bool IsFunction { get { return Type.funcret != null; } }

		public CallableDeclaration(string name, DeclarationList @params, TypeNode ret = null, CallableDirectives dirs = null)
			: base(name, new ProceduralType(@params, ret, dirs))
		{
		}
	}

	/// <summary>
	/// Declaration of a global Routine
	/// </summary>
	public class RoutineDeclaration : CallableDeclaration
	{
		public RoutineDeclaration(string name, DeclarationList @params, TypeNode ret = null, RoutineDirectives dirs = null)
			: base(name, @params, ret, dirs) { }
	}

	/// <summary>
	/// Declaration of a Method
	/// </summary>
	public class MethodDeclaration : CallableDeclaration
	{
		public bool isStatic { get; set; }
		public String objname;
		public String metname;

		public MethodDeclaration(string objname, string name, DeclarationList @params, ScalarType ret = null,
								MethodDirectives dirs = null)
			: base(objname+"."+name, @params, ret, dirs)
		{
			this.metname = name;
			this.objname = objname;
			isStatic = false;

			foreach (var param in @params)
				if (param.name == "self")
					throw new IdentifierRedeclared("Method parameter cannot shadow 'self' reference");
		}
	}

	public class SpecialMethodDeclaration : MethodDeclaration
	{
		public SpecialMethodDeclaration(string objname, string name, DeclarationList @params,
										MethodDirectives dirs = null)
			: base(name, objname, @params, null, dirs)	{	}
	}

	public class ConstructorDeclaration : SpecialMethodDeclaration
	{
		public ConstructorDeclaration(string objname, string name, DeclarationList @params,
										MethodDirectives dirs = null)
			: base(name, objname, @params, dirs) { }
	}

	public class DestructorDeclaration : SpecialMethodDeclaration
	{
		public DestructorDeclaration(string objname, string name, DeclarationList @params,
										MethodDirectives dirs = null)
			: base(name, objname, @params, dirs) { }
	}

	#endregion

	// not really a declaration, not it makes the grammar cleaner..
	public class RoutineDefinition : Declaration
	{
		public CallableDeclaration header;
		public RoutineBody body;

		public RoutineDefinition(CallableDeclaration header, RoutineBody body)
		{
			this.header = header;
			this.body = body;
		}
	}


	#region Directives' Aggregators

	/// <summary>
	/// Callable Units Directives
	/// </summary>
	public abstract class CallableDirectives : Node
	{
		private CallConvention _callconv = 0;
		public CallConvention   Callconv
		{
			get { return _callconv ; }
			set {
				if (_callconv != 0) Error("Cannot specify more than 1 Call convention");
				else				_callconv = value; 
			}
		}

		HashSet<GeneralDirective> generaldirs = new HashSet<GeneralDirective>();

		public CallableDirectives(int dir = 0)
		{
			_callconv = 0;
			if (dir != 0)
				Add(dir);
		}

		public virtual void Add(CallableDirectives dirs)
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
			if (o == null || !(o is CallableDirectives))
				return false;
			CallableDirectives rtype = (CallableDirectives) o;
			return Callconv == rtype.Callconv && generaldirs.SequenceEqual(rtype.generaldirs);
		}
	}


	/// <summary>
	/// Routine Directives
	/// </summary>
	public class RoutineDirectives : CallableDirectives
	{
		private ImportDirective _importdir = ImportDirective.Default;
		public ImportDirective Importdir
		{
			get { return _importdir; }
			set
			{
				if (_importdir != 0) Error("Cannot specify more than external/forward directive");
				else _importdir = value;
			}
		}

		public ExternalDirective External { get; set; }

		public RoutineDirectives(int dir = 0) : base(dir) { }

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
			var ot = (RoutineDirectives) o;
			return Importdir == ot.Importdir && External.Equals(ot.External);
		}
	}

	/// <summary>
	/// Method Directives
	/// </summary>
	public class MethodDirectives : CallableDirectives
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

	public struct ExternalDirective
	{
		public ConstExpression File { get; set; }
		public ConstExpression Name { get; set; }

		public ExternalDirective(ConstExpression file, ConstExpression name = null) : this()
		{
			File = file;
			Name = name;
			File.ForcedType = StringType.Single;
			Name.ForcedType = StringType.Single;
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