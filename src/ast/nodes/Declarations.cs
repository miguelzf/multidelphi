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
		public ArrayList names = new ArrayList();
		public TypeNode type;

		public void AddName(String name)
		{
			names.Add(name);

			DelphiParser.DeclRegistry.RegisterDeclaration(name, this);
		}

		protected Declaration() { }

		protected Declaration(TypeNode t = null)
		{
			if (t != null)	type = t;
			else	type = UndefinedType.Single;
		}

		public Declaration(ArrayList names, TypeNode t = null) : this(t)
		{
			foreach (String name in names)
				AddName(name);
		}

		public Declaration(String name, TypeNode t = null) : this(t)
		{
			AddName(name);
		}
	}

	public class LabelDeclaration : Declaration
	{
		public LabelDeclaration(String name) : base(name, null) { }

		public LabelDeclaration(ArrayList names) : base(names, null) { }
	}


	public abstract class ValueDeclaration : Declaration
	{
		public ValueDeclaration(ArrayList names, TypeNode t = null) : base(names, t) { }

		public ValueDeclaration(String name, TypeNode t = null) : base(name, t) { }
	}


	public class VarDeclaration : ValueDeclaration
	{
		public Expression init;
		public String shareVal;
		public bool isThrVar;

		public VarDeclaration(ArrayList ids, TypeNode t, Expression init = null)
			: base(ids, t)
		{
			this.init = init;
		}

		public VarDeclaration(ArrayList ids, TypeNode t, String shareVal) 
			: base(ids, t)
		{
			this.shareVal = shareVal;
		}
	}


	#region Parameters' Declarations
	/// <summary>
	/// Routine parameters
	/// </summary>

	public class ParameterDeclaration : ValueDeclaration
	{
		public Expression init;

		public ParameterDeclaration(ArrayList ids, ScalarType t, Expression init = null)
			: base(ids, t)
		{
			this.init = init;
		}
	}

	public class VarParameterDeclaration : ParameterDeclaration
	{
		public VarParameterDeclaration(ArrayList ids, ScalarType t, Expression init = null) : base(ids, t, init) { }
	}

	public class ConstParameterDeclaration : ParameterDeclaration
	{
		public ConstParameterDeclaration(ArrayList ids, ScalarType t, Expression init = null) : base(ids, t, init) { }
	}

	public class OutParameterDeclaration : ParameterDeclaration
	{
		public OutParameterDeclaration(ArrayList ids, ScalarType t, Expression init = null) : base(ids, t, init) { }
	}

	#endregion


	/// <summary>
	/// Composite object field declaration
	/// </summary>
	public class FieldDeclaration : ValueDeclaration
	{
		public FieldDeclaration(ArrayList ids, TypeNode t = null)
			: base(ids, t)
		{
		}
	}

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