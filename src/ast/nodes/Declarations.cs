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

		public Declaration(String name, TypeNode t)
		{
			this.name = name;
			this.type = t;
		}
	}

	public abstract class VarDeclaration : Declaration
	{
		List<String> names;
		Expression init;
		String shareVal;

		public VarDeclaration(List<String> ids, TypeNode t, Expression init)
		{
			this.name = ids[0];
			this.init = init;
		}

		public VarDeclaration(String name, TypeNode t, Expression init) : base(name, t)
		{
			this.init = init;
		}

		public VarDeclaration(List<String> ids, TypeNode t, String shareVal)
		{
			this.name = ids[0];
			this.shareVal = shareVal;
		}

		public VarDeclaration(String name, TypeNode t, String shareVal)
			: base(name, t)
		{
			this.shareVal = shareVal;
		}
	}



	/// <summary>
	/// TODO!! Must Derive type
	/// </summary>
	public abstract class ConstDeclaration : Declaration
	{
		Expression init;

		public ConstDeclaration(String name, Expression init)
		{
			this.name = name;
			this.init = init;
			this.type = init.type;
		}
	}



	public class EnumDeclaration
	{

	}

	public class EnumValueDeclaration
	{
		public string name;
		public Expression init;

		public EnumValueDeclaration(string val)
		{
			this.name = val;
		}

		public EnumValueDeclaration(string val, Expression init)
			: this(val)
		{
			this.init = init;
		}
	}



	public abstract class TypeDeclaration : Declaration
	{

	}






}