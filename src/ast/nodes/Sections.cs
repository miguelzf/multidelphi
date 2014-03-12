using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace crosspascal.ast.nodes
{


	public class UsesNode : Node
	{
		public Identifier ident;
		public UsesNode next;

		public UsesNode(Identifier ident, UsesNode next)
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
		public Identifier identifier;
		public BlockWithDeclarationsNode body;
		public UsesNode uses;

		public ProgramNode(Identifier ident, UsesNode uses, BlockWithDeclarationsNode body)
		{
			this.identifier = ident;
			this.uses = uses;
			this.body = body;
		}
	}

	public class LibraryNode : CompilationUnit
	{
		public Identifier identifier;
		public BlockWithDeclarationsNode body;
		public UsesNode uses;

		public LibraryNode(Identifier ident, UsesNode uses, BlockWithDeclarationsNode body)
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
		public Identifier identifier;
		public InterfaceSection interfce;
		public ImplementationSection implementation;
		public Node init;

		public UnitNode(Identifier ident, InterfaceSection interfce, ImplementationSection impl, Node init)
		{
			this.identifier = ident;
			this.interfce = interfce;
			this.implementation = impl;
			this.init = init;
		}
	}

	public class PackageNode : CompilationUnit
	{
		public Identifier identifier;
		public UsesNode requires;
		public UsesNode contains;

		public PackageNode(Identifier ident, UsesNode requires, UsesNode contains)
		{
			this.identifier = ident;
			this.requires = requires;
			this.contains = contains;
		}
	}

	public class ExportItem : DeclarationNode
	{
		public Identifier ident;
		public string name;
		public Expression index;

		public ExportItem(Identifier ident, string name, Expression index)
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

}