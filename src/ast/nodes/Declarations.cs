using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MultiPascal.Parser;

namespace MultiPascal.AST.Nodes
{

	#region Declarations hierarchy
	///	Declaration
	///		TranslationUnit
	///			Program
	///			Library
	///			Unit
	///		LabelDeclaration
	///		ValueDeclaration
	///			Constant
	///			Variable
	///			Parameter (of routines)
	///				DefaultParam
	///				VarParam
	///				OutParam
	///				ConstParam
	///			Field
	///		TypeDecl
	///			[default, value type]
	///			CompositeDecl
	///				Class/Object
	///				Interf
	///		CallableDecl
	///			RoutineDecl
	///				RoutineDefinition
	///			MethodDecl
	///				MethodDefitnion
	#endregion

	/// <summary>
	/// A named declaration. Binds an entity to a name
	/// </summary>
	public abstract class Declaration : Node
	{
		public String name;
		public TypeNode type;

		public Declaration(String name, TypeNode t = null)
		{
			this.type = t;
			this.name = name;
		}

		public override string ToString()
		{
			return "("+ name + " decl type " + (type == null? "null" : type.GetType().Name) + ")";
		}
	}

	/// <summary>
	/// 
	/// </summary>
	public class LabelDeclaration : Declaration
	{
		public LabelDeclaration(String name) : base(name, null) { }
	}


	public abstract class ValueDeclaration : Declaration
	{
		public ValueDeclaration(String name, TypeNode t = null) : base(name, t)
		{
		}
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
	/// Routine parameters. May be value (default), variable, constant, or out.
	/// Param types must be an id, string or open array (array of paramtype)
	/// </summary>

	public class ParamDeclaration : ValueDeclaration
	{
		public Expression init;

		public ParamDeclaration(String id, TypeNode t, Expression init = null)
			: base(id, t)
		{
			this.init = init;
		}

		// Use with caution!! does not clone init
		public ParamDeclaration Clone()
		{
			return new ParamDeclaration(name, type, null);
		}
	}

	public class VarParamDeclaration : ParamDeclaration
	{
		public VarParamDeclaration(String id, TypeNode t, Expression init = null) : base(id, t, init) { }
	}

	public class ConstParamDeclaration : ParamDeclaration
	{
		public ConstParamDeclaration(String id, TypeNode t, Expression init = null) : base(id, t, init) { }
	}

	public class OutParamDeclaration : ParamDeclaration
	{
		public OutParamDeclaration(String id, TypeNode t, Expression init = null) : base(id, t, init) { }
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
			
			if (init != null)	// else, it's an enumvalue
			{
				init.EnforceConst = true;
				if (t == null)
					type = init.Type;
			}
		}
	}

	public class EnumValue : ConstDeclaration
	{
		// Init value to be computed a posteriori
		public EnumValue(string val) : base(val, null) { }

		public EnumValue(string val, Expression init)
			: base(val, init)
		{
			init.ForcedType = IntegerType.Single;
		}
	}


	/// <summary>
	/// Creates a custom, user-defined name for some Type
	/// </summary>
	public class TypeDeclaration : Declaration
	{
		public TypeDeclaration(String name, TypeNode type) : base(name, type) { }
	}

}