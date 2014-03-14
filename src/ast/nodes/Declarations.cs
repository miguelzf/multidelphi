using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace crosspascal.ast.nodes
{

	#region Declarations hierarchy
	/// <remarks>
	///	Declaration
	///		Constant
	///		RscStr
	///		Variable
	///			Parameter	// of routines
	///				DefaultParam
	///				VarParam
	///				OutParam
	///				ConstParam
	///		ObjectField
	///		TypeDecl
	///			Custom-Type
	///			
	///			CallableUnit
	///				Routine
	///				Method
	///					SpecialMethod
	///						Constructor
	///						Destructor
	///			ObjectDecl
	///				Interf
	///				Record
	///				Class/Object
	///			
	/// </remarks>
	#endregion

	public abstract class Declaration : Node
	{
		public List<String> names;
		public TypeNode type;

		protected Declaration() { }

		protected Declaration(TypeNode t = null)
		{
			if (t != null)	type = t;
			else	type = UndefinedType.Single;
		}

		public Declaration(List<String> names, TypeNode t = null) : this(t)
		{
			names.AddRange(names);
		}

		public Declaration(String name, TypeNode t = null) : this(t)
		{
			names.Add(name);
		}
	}

	public class LabelDeclaration : Declaration
	{
		public LabelDeclaration(String name) : base(name, null) { }

		public LabelDeclaration(List<String> names) : base(names, null) { }
	}

	public class VarDeclaration : Declaration
	{
		public Expression init;
		public String shareVal;

		public VarDeclaration(List<String> ids, TypeNode t, Expression init = null)
			: base(ids, t)
		{
			this.init = init;
		}

		public VarDeclaration(List<String> ids, TypeNode t, String shareVal) 
			: base(ids, t)
		{
			this.shareVal = shareVal;
		}
	}

	/// <summary>
	/// Routine parameters
	/// </summary>

	public class ParameterDeclaration : VarDeclaration
	{
		public ParameterDeclaration(List<String> ids, ScalarType t = null, Expression init = null) : base(ids, t, init)
		{
		}
	}

	public class VarParameterDeclaration : ParameterDeclaration
	{
		public VarParameterDeclaration(List<String> ids, ScalarType t, Expression init = null) : base(ids, t, init) { }
	}

	public class ConstParameterDeclaration : ParameterDeclaration
	{
		public ConstParameterDeclaration(List<String> ids, ScalarType t, Expression init = null) : base(ids, t, init) { }
	}

	public class OutParameterDeclaration : ParameterDeclaration
	{
		public OutParameterDeclaration(List<String> ids, ScalarType t, Expression init = null) : base(ids, t, init) { }
	}


	/// <summary>
	/// Composite object field declaration
	/// </summary>
	public class FieldDeclaration : Declaration
	{
		public FieldDeclaration(List<String> ids, VariableType t = null)
			: base(ids, t)
		{
		}
	}

	/// <summary>
	/// TODO!! Must Derive type
	/// </summary>
	public class ConstDeclaration : Declaration
	{
		public Expression init;

		public ConstDeclaration(String name, Expression init) : base(name)
		{
			this.init = init;
			init.EnforceConst = true;
			this.type = init.Type;
		}
	}


	// TODO move this to types. 
	// Create initilization node

	public class EnumValue : ConstDeclaration
	{
		// Init value to be computed a posteriori
		public EnumValue(string val) : base(val, null) { }

		public EnumValue(string val, Expression init) : base(val, init)
		{
			init.ForcedType = IntegerType.Single;
		}
	}

	/// <summary>
	/// Creates a custom, user-defined name for some Type
	/// 
	/// TODO fetch custom type for typename
	/// </summary>
	public class TypeDeclaration : Declaration
	{
		String typename;

		protected TypeDeclaration() { }

		public TypeDeclaration(String name, TypeNode type) : base(name, type) { }

		public TypeDeclaration(String name, String typename) : base(name)
		{
			this.typename = typename;
		}
	}

	public abstract partial class CompositeDeclaration : TypeDeclaration
	{
		// In file Composites.cs
	}

	public abstract partial class CallableDeclaration : TypeDeclaration
	{
		// In file Routines.cs
	}
}