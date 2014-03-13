using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace crosspascal.ast.nodes
{
	public abstract class Declaration : Node
	{
		public String name;
		public TypeNode type;

		public Declaration() { }

		public Declaration(String name, TypeNode t = null)
		{
			this.name = name;
			this.type = t;
		}
	}

	public class LabelDeclaration : Declaration
	{
		public LabelDeclaration(String name) : base(name, null) { }
	}

	public class VarDeclaration : Declaration
	{
		public List<String> names;
		public Expression init;
		public String shareVal;

		public VarDeclaration(List<String> ids, TypeNode t, Expression init = null)
		{
			this.name = ids[0];
			this.init = init;
		}

		public VarDeclaration(String name, TypeNode t, Expression init = null) : base(name, t)
		{
			this.init = init;
		}

		public VarDeclaration(List<String> ids, TypeNode t, String shareVal)
		{
			this.name = ids[0];
			this.shareVal = shareVal;
		}

		public VarDeclaration(String name, TypeNode t, String shareVal) : base(name, t)
		{
			this.shareVal = shareVal;
		}
	}

	public class ParameterDeclaration : VarDeclaration
	{
		public ParameterDeclaration(List<String> ids, TypeNode t = null, Expression init = null) : base(ids, t, init)
		{
			if (t == null)
				t = UndefinedType.Single;
		}

		public ParameterDeclaration(String id, TypeNode t = null, Expression init = null) : base(id, t, init)
		{
			if (t == null)
				t = UndefinedType.Single;
		}
	}

	public class VarParameterDeclaration : ParameterDeclaration
	{
		public VarParameterDeclaration(List<String> ids, TypeNode t, Expression init = null) : base(ids, t, init) { }

		public VarParameterDeclaration(String id, TypeNode t, Expression init = null) : base(id, t, init) { }
	}

	public class ConstParameterDeclaration : ParameterDeclaration
	{
		public ConstParameterDeclaration(List<String> ids, TypeNode t, Expression init = null) : base(ids, t, init) { }

		public ConstParameterDeclaration(String id, TypeNode t, Expression init = null) : base(id, t, init) { }
	}

	public class OutParameterDeclaration : ParameterDeclaration
	{
		public OutParameterDeclaration(List<String> ids, TypeNode t, Expression init = null) : base(ids, t, init) { }

		public OutParameterDeclaration(String id, TypeNode t, Expression init = null) : base(id, t, init) { }
	}



	/// <summary>
	/// TODO!! Must Derive type
	/// </summary>
	public abstract class ConstDeclaration : Declaration
	{
		public Expression init;

		public ConstDeclaration(String name, Expression init) : base(name)
		{
			this.init = init;
			init.enforceConst = true;
			this.type = init.type;
		}
	}

	public class EnumDeclaration : Declaration
	{
		public EnumValueList enumlist;

		public EnumDeclaration(EnumValueList enumlist)
		{
			this.enumlist = enumlist;
		}

		/// <summary>
		/// Determine and assign the Enum initializers, from the user-defined to the automatic
		/// </summary>
		public void AssignEnumInitializers()
		{
			int val = 0;	// default start val

			foreach (EnumValue al in enumlist)
			{
				if (al.init == null)
					al.init = new IntLiteral(val);
				else
				{
					if (al.init.constantValue.type.ISA(IntegerType.Single))
						val = (int)al.init.constantValue.value;
					else
						Error("Enum initializer must be an integer");
				}
				val++;
			}
		}
	}

	// TODO move this to types. 
	// Create initilization node

	public class EnumValue : ConstDeclaration
	{
		// Init value to be computed a posterior
		public EnumValue(string val) : base(val, null) { }

		public EnumValue(string val, Expression init) : base(val, init)
		{
			init.forcedType = IntegerType.Single;
		}
	}

	/// <summary>
	/// Creates a custom, user-defined name for some Type
	/// </summary>
	public class TypeDeclaration : Declaration
	{
		String typename;
		VariableType reftype;

		public TypeDeclaration(String name, VariableType type)
		{
			typename = name;
			reftype = type;
		}
	}

	public abstract partial class CompositeDeclaration : TypeDeclaration
	{
		// In file Composites.cs
	}

	public abstract partial class RoutineDeclaration : TypeDeclaration
	{
		// In file Routines.cs
	}
}