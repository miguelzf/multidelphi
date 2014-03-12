using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace crosspascal.ast.nodes
{

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

	public class TypeDeclarationNode : DeclarationNode
	{
		public Identifier ident;
		public TypeNode type;

		public TypeDeclarationNode(Identifier ident, TypeNode type)
		{
			this.ident = ident;
			this.type = type;
		}
	}

	public class TypeDeclarationNodeList : DeclarationNode
	{
		public TypeDeclarationNode decl;
		public TypeDeclarationNodeList next;

		public TypeDeclarationNodeList(TypeDeclarationNode decl, TypeDeclarationNodeList next)
		{
			this.decl = decl;
			this.next = next;
		}
	}




	public class VarDeclarationOption : Node
	{
	}

	public class VariableInitNode : VarDeclarationOption
	{
		public Expression expr;

		public VariableInitNode(Expression expr)
		{
			this.expr = expr;
		}
	}

	public class VariableAbsoluteNode : VarDeclarationOption
	{
		public Identifier ident;

		public VariableAbsoluteNode(Identifier ident)
		{
			this.ident = ident;
		}
	}

	public class VarDeclaration : DeclarationNode
	{
		public IdentifierList ids;
		public TypeNode type;
		public VarDeclarationOption option;

		public VarDeclaration(IdentifierList ids, TypeNode type, VarDeclarationOption option)
		{
			this.ids = ids;
			this.type = type;
			this.option = option;
		}
	}

	public class VarDeclarationList : DeclarationNode
	{
		public VarDeclaration vardecl;
		public VarDeclarationList next;

		public VarDeclarationList(VarDeclaration vardecl, VarDeclarationList next)
		{
			this.vardecl = vardecl;
			this.next = next;
		}
	}

	public class ConstDeclaration : DeclarationNode
	{
		public Identifier ident;
		public Expression expr;
		public TypeNode type;

		public ConstDeclaration(Identifier ident, TypeNode type, Expression expr)
		{
			this.ident = ident;
			this.type = type;
			this.expr = expr;
		}
	}

	public class ConstDeclarationList : DeclarationNode
	{
		public ConstDeclaration constdecl;
		public ConstDeclarationList next;

		public ConstDeclarationList(ConstDeclaration constdecl, ConstDeclarationList next)
		{
			this.constdecl = constdecl;
			this.next = next;
		}
	}

}