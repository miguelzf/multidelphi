using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace crosspascal.ast.nodes
{


	public class UsesNode : Node
	{
		public IdentifierNode ident;
		public UsesNode next;

		public UsesNode(IdentifierNode ident, UsesNode next)
		{
			this.ident = ident;
			this.next = next;
		}
	}

	public class BlockWithDeclarationsNode : Node
	{
		public DeclarationNode decls;
		public StatementBlock block;

		public BlockWithDeclarationsNode(DeclarationNode decls, StatementBlock block)
		{
			this.decls = decls;
			this.block = block;
		}
	}

	public class ProgramNode : CompilationUnit
	{
		public IdentifierNode identifier;
		public BlockWithDeclarationsNode body;
		public UsesNode uses;

		public ProgramNode(IdentifierNode ident, UsesNode uses, BlockWithDeclarationsNode body)
		{
			this.identifier = ident;
			this.uses = uses;
			this.body = body;
		}
	}

	public class LibraryNode : CompilationUnit
	{
		public IdentifierNode identifier;
		public BlockWithDeclarationsNode body;
		public UsesNode uses;

		public LibraryNode(IdentifierNode ident, UsesNode uses, BlockWithDeclarationsNode body)
		{
			this.identifier = ident;
			this.uses = uses;
			this.body = body;
		}
	}

	public class DeclarationNodeList : DeclarationNode
	{
		public DeclarationNode decl;
		public DeclarationNodeList next;

		public DeclarationNodeList(DeclarationNode decl, DeclarationNodeList next)
		{
			this.decl = decl;
			this.next = next;
		}
	}

	public class InterfaceSection : Node
	{
		public UsesNode uses;
		public DeclarationNode decls;

		public InterfaceSection(UsesNode uses, DeclarationNode decls)
		{
			this.uses = uses;
			this.decls = decls;
		}
	}

	public class ImplementationSection : Node
	{
		public UsesNode uses;
		public DeclarationNode decls;

		public ImplementationSection(UsesNode uses, DeclarationNode decls)
		{
			this.uses = uses;
			this.decls = decls;
		}
	}

	public class InitializationSection : Node
	{
		public Statement initialization;
		public Statement finalization;

		public InitializationSection(Statement initialization, Statement finalization)
		{
			this.initialization = initialization;
			this.finalization = finalization;
		}
	}

	public class UnitNode : CompilationUnit
	{
		public IdentifierNode identifier;
		public InterfaceSection interfce;
		public ImplementationSection implementation;
		public Node init;

		public UnitNode(IdentifierNode ident, InterfaceSection interfce, ImplementationSection impl, Node init)
		{
			this.identifier = ident;
			this.interfce = interfce;
			this.implementation = impl;
			this.init = init;
		}
	}

	public class PackageNode : CompilationUnit
	{
		public IdentifierNode identifier;
		public UsesNode requires;
		public UsesNode contains;

		public PackageNode(IdentifierNode ident, UsesNode requires, UsesNode contains)
		{
			this.identifier = ident;
			this.requires = requires;
			this.contains = contains;
		}
	}

	public class ExportItem : DeclarationNode
	{
		public IdentifierNode ident;
		public string name;
		public DelphiExpression index;

		public ExportItem(IdentifierNode ident, string name, DelphiExpression index)
		{
			this.ident = ident;
			this.name = name;
			this.index = index;
		}
	}

	public class ExportItemNodeList : DeclarationNode
	{
		public ExportItem export;
		public ExportItemNodeList next;


	}

	public class TypeDeclarationNode : DeclarationNode
	{
		public IdentifierNode ident;
		public TypeNode type;

		public TypeDeclarationNode(IdentifierNode ident, TypeNode type)
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
		public DelphiExpression expr;

		public VariableInitNode(DelphiExpression expr)
		{
			this.expr = expr;
		}
	}

	public class VariableAbsoluteNode : VarDeclarationOption
	{
		public IdentifierNode ident;

		public VariableAbsoluteNode(IdentifierNode ident)
		{
			this.ident = ident;
		}
	}

	public class VarDeclaration : DeclarationNode
	{
		public IdentifierNodeList ids;
		public TypeNode type;
		public VarDeclarationOption option;

		public VarDeclaration(IdentifierNodeList ids, TypeNode type, VarDeclarationOption option)
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
		public IdentifierNode ident;
		public DelphiExpression expr;
		public TypeNode type;

		public ConstDeclaration(IdentifierNode ident, TypeNode type, DelphiExpression expr)
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