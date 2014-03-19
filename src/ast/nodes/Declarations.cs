using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using crosspascal.parser;

namespace crosspascal.ast.nodes
{

	#region Declarations hierarchy
	/// <remarks>
	///	Declaration
	///		LabelDeclaration
	///		ValueDeclaration
	///			Constant
	///			Variable
	///			Parameter	// of routines
	///				DefaultParam
	///				VarParam
	///				OutParam
	///				ConstParam
	///			Field
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
		public String name;
		public TypeNode type;

		public void AddName(String name)
		{
			this.name = name;
			
			if (name != null)
				DelphiParser.DeclReg.RegisterDeclaration(name, this);
		}

		protected Declaration() { }

		protected Declaration(TypeNode t = null)
		{
			if (t != null)	type = t;
			else	type = UndefinedType.Single;
		}

		public Declaration(String name, TypeNode t = null) : this(t)
		{
			AddName(name);
		}
	}

	public class LabelDeclaration : Declaration
	{
		public LabelDeclaration(String name) : base(name, null) { }
	}


	public abstract class ValueDeclaration : Declaration
	{
		public ValueDeclaration(String name, TypeNode t = null) : base(name, t) { }
	}


	public class VarDeclaration : ValueDeclaration
	{
		public Expression init;
		public String shareVal;
		public bool isThrVar;

		public VarDeclaration(String id, TypeNode t, Expression init = null)
			: base(id, t)
		{
			this.init = init;
		}

		public VarDeclaration(String id, TypeNode t, String shareVal) 
			: base(id, t)
		{
			this.shareVal = shareVal;
		}
	}


	#region Parameters' Declarations
	/// <summary>
	/// Routine parameters
	/// </summary>

	public class ParamDeclaration : ValueDeclaration
	{
		public Expression init;

		public ParamDeclaration(String id, VariableType t, Expression init = null)
			: base(id, t)
		{
			this.init = init;
		}
	}

	public class VarParamDeclaration : ParamDeclaration
	{
		public VarParamDeclaration(String id, VariableType t, Expression init = null) : base(id, t, init) { }
	}

	public class ConstParamDeclaration : ParamDeclaration
	{
		public ConstParamDeclaration(String id, VariableType t, Expression init = null) : base(id, t, init) { }
	}

	public class OutParamDeclaration : ParamDeclaration
	{
		public OutParamDeclaration(String id, VariableType t, Expression init = null) : base(id, t, init) { }
	}

	#endregion

	/// <summary>
	/// TODO!! Must Derive type
	/// </summary>
	public class ConstDeclaration : ValueDeclaration
	{
		public Expression init;

		public ConstDeclaration(String name, Expression init, TypeNode t = null)
			: base(name, t)
		{
			this.init = init;
			init.EnforceConst = true;

			if (t == null)
				type = init.Type;
		}
	}


	/// <summary>
	/// Creates a custom, user-defined name for some Type
	/// </summary>
	public class TypeDeclaration : Declaration
	{
		protected TypeDeclaration() { }

		public TypeDeclaration(String name, TypeNode type) : base(name, type) { }
	}

	public abstract partial class CallableDeclaration : TypeDeclaration
	{
		// In file Routines.cs
	}

}