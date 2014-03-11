using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace crosspascal.ast.nodes
{


	public class UsesNode : DelphiNode
	{
		public IdentifierNode ident;
		public UsesNode next;

		public UsesNode(IdentifierNode ident, UsesNode next)
		{
			this.ident = ident;
			this.next = next;
		}
	}

	public class BlockWithDeclarationsNode : DelphiNode
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

	public class DeclarationListNode : DeclarationNode
	{
		public DeclarationNode decl;
		public DeclarationListNode next;

		public DeclarationListNode(DeclarationNode decl, DeclarationListNode next)
		{
			this.decl = decl;
			this.next = next;
		}
	}

	public class UnitInterfaceNode : DelphiNode
	{
		public UsesNode uses;
		public DeclarationNode decls;

		public UnitInterfaceNode(UsesNode uses, DeclarationNode decls)
		{
			this.uses = uses;
			this.decls = decls;
		}
	}

	public class UnitImplementationNode : DelphiNode
	{
		public UsesNode uses;
		public DeclarationNode decls;

		public UnitImplementationNode(UsesNode uses, DeclarationNode decls)
		{
			this.uses = uses;
			this.decls = decls;
		}
	}

	public class UnitInitialization : DelphiNode
	{
		public Statement initialization;
		public Statement finalization;

		public UnitInitialization(Statement initialization, Statement finalization)
		{
			this.initialization = initialization;
			this.finalization = finalization;
		}
	}

	public class UnitNode : CompilationUnit
	{
		public IdentifierNode identifier;
		public UnitInterfaceNode interfce;
		public UnitImplementationNode implementation;
		public DelphiNode init;

		public UnitNode(IdentifierNode ident, UnitInterfaceNode interfce, UnitImplementationNode impl, DelphiNode init)
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

	public class ExportItemListNode : DeclarationNode
	{
		public ExportItem export;
		public ExportItemListNode next;


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

	public class TypeDeclarationListNode : DeclarationNode
	{
		public TypeDeclarationNode decl;
		public TypeDeclarationListNode next;

		public TypeDeclarationListNode(TypeDeclarationNode decl, TypeDeclarationListNode next)
		{
			this.decl = decl;
			this.next = next;
		}
	}




	public class VarDeclarationOption : DelphiNode
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

	public class VarDeclarationNode : DeclarationNode
	{
		public IdentifierListNode ids;
		public TypeNode type;
		public VarDeclarationOption option;

		public VarDeclarationNode(IdentifierListNode ids, TypeNode type, VarDeclarationOption option)
		{
			this.ids = ids;
			this.type = type;
			this.option = option;
		}
	}

	public class VarDeclarationList : DeclarationNode
	{
		public VarDeclarationNode vardecl;
		public VarDeclarationList next;

		public VarDeclarationList(VarDeclarationNode vardecl, VarDeclarationList next)
		{
			this.vardecl = vardecl;
			this.next = next;
		}
	}

	public class ConstDeclarationNode : DeclarationNode
	{
		public IdentifierNode ident;
		public DelphiExpression expr;
		public TypeNode type;

		public ConstDeclarationNode(IdentifierNode ident, TypeNode type, DelphiExpression expr)
		{
			this.ident = ident;
			this.type = type;
			this.expr = expr;
		}
	}

	public class ConstDeclarationList : DeclarationNode
	{
		public ConstDeclarationNode constdecl;
		public ConstDeclarationList next;

		public ConstDeclarationList(ConstDeclarationNode constdecl, ConstDeclarationList next)
		{
			this.constdecl = constdecl;
			this.next = next;
		}
	}

}