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
		List<String> names;
		Expression init;
		String shareVal;

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
		public ParameterDeclaration(List<String> ids, TypeNode t = UndefinedType, Expression init = null) : base(ids, t, init) { }

		public ParameterDeclaration(String id, TypeNode t = UndefinedType, Expression init = null) : base(id, t, init) { }
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
		Expression init;

		public ConstDeclaration(String name, Expression init) : base(name)
		{
			this.init = init;
			this.type = init.type;
		}
	}

	public class EnumDeclaration : Declaration
	{

	}

	public class EnumValueDeclaration : VarDeclaration
	{
		public Expression init;

		public EnumValueDeclaration(string val) : base(val, new IntegerType()) { }

		public EnumValueDeclaration(string val, Expression init) : this(val)
		{
			this.init = init;
		}
	}


	public abstract class TypeDeclaration : Declaration
	{
		// TODO
	}

}