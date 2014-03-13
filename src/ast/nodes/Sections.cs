using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace crosspascal.ast.nodes
{
	//==========================================================================
	// Top-Level Source Files/Units
	//==========================================================================

	#region Compilation Units

	/// <summary>
	/// CompilationUnit: top level source file. Can be Program, Unit, Library or Package
	/// </summary>
	public abstract class CompilationUnit : Node
	{
		public String name;

		public CompilationUnit(String name) 
		{
			this.name = name;
		}
	}

	public class ProgramNode : CompilationUnit
	{
		public ProgramBody body;
		public NodeList uses;

		public ProgramNode(String name, NodeList uses, ProgramBody body) : base(name)
		{
			this.uses = uses;
			this.body = body;
		}
	}

	public class LibraryNode : CompilationUnit
	{
		public ProgramBody body;
		public UsesItem uses;

		public LibraryNode(String name, UsesItem uses, ProgramBody body) : base(name)
		{
			this.uses = uses;
			this.body = body;
		}
	}

	public class UnitNode : CompilationUnit
	{
		public InterfaceSection interfce;
		public ImplementationSection implementation;
		public Node init;

		public UnitNode(String name, InterfaceSection interfce, ImplementationSection impl, Node init) : base(name)
		{
			this.interfce = interfce;
			this.implementation = impl;
			this.init = init;
		}
	}

	public class PackageNode : CompilationUnit
	{
		public UsesItem requires;
		public UsesItem contains;

		public PackageNode(String name, UsesItem requires, UsesItem contains) : base(name)
		{
			this.requires = requires;
			this.contains = contains;
		}
	}

	#endregion


	//==========================================================================
	// Units directives
	//==========================================================================

	#region Units directives

	public abstract class UnitItem : Node
	{
		public String name;

		public UnitItem(String name)
		{
			this.name = name;
		}
	}

	public class UsesItem : UnitItem
	{
		public UsesItem(String name) : base(name) { }
	}

	public class RequiresItem : UnitItem
	{
		public RequiresItem(String name) : base(name) { }
	}

	public class ContainsItem : UnitItem
	{
		public ContainsItem(String name) : base(name) { }
	}

	public class ExportItem : UnitItem
	{
		public String exportname;
		public Expression index;

		public ExportItem(String name, String exportname) :base(name)
		{
			this.exportname = exportname;
		}

		public ExportItem(String name,  Expression index) :base(name)
		{
			this.index = index;
		}
	}

	#endregion

	
	//==========================================================================
	// Sections/Scopes
	//==========================================================================

	#region Sections/Scopes

	public abstract class Section : Node
	{
		public NodeList decls;

		public Section(NodeList decls)
		{
			this.decls = decls;
		}
	}

	public abstract class CodeSection : Section
	{
		public BlockStatement block;

		public CodeSection(NodeList decls, BlockStatement block) : base(decls)
		{
			this.block = block;
		}
	}

	public class ProgramBody : CodeSection
	{
		public ProgramBody(NodeList decls, BlockStatement block) : base(decls, block) { }
	}

	public class RoutineBody : CodeSection
	{
		public RoutineBody(NodeList decls, BlockStatement block) : base(decls, block) { }
	}

	public class InitializationSection : CodeSection
	{
		public InitializationSection(BlockStatement body) : base(null, body) { }
	}

	public class FinalizationSection : CodeSection
	{
		public FinalizationSection(BlockStatement body) : base(null, body) { }
	}


	public abstract class DeclarationSection : Section
	{
		public NodeList uses;

		public DeclarationSection(NodeList uses, NodeList decls) : base(decls)
		{
			this.uses = uses;
		}
	}
	
	public class InterfaceSection : DeclarationSection
	{
		public InterfaceSection(NodeList uses, NodeList decls) : base(uses, decls) { }
	}

	public class ImplementationSection : DeclarationSection
	{
		public ImplementationSection(NodeList uses, NodeList decls) : base(uses, decls) { }
	}


	public class ClassBody : Section
	{
		public ClassBody(NodeList decls) : base(decls) { }
	}

	public class AssemblerRoutineBody : RoutineBody
	{
		public AssemblerRoutineBody(AssemblerBlock asm) 
				: base(new DeclarationList(), asm) { }
	}

	#endregion

}