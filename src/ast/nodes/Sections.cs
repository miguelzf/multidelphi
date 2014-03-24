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
		public NodeList uses;
		public ProgramBody body;

		public ProgramNode(String name, NodeList uses, ProgramBody body) : base(name)
		{
			this.uses = uses;
			this.body = body;
		}
	}

	public class LibraryNode : CompilationUnit
	{
		public ProgramBody body;
		public NodeList uses;

		public LibraryNode(String name, NodeList uses, ProgramBody body) : base(name)
		{
			this.uses = uses;
			this.body = body;
		}
	}

	public class UnitNode : CompilationUnit
	{
		public InterfaceSection @interface;
		public ImplementationSection implementation;
		public Node init;

		public UnitNode(String name, InterfaceSection interfce, ImplementationSection impl, Node init) : base(name)
		{
			this.@interface = interfce;
			this.implementation = impl;
			this.init = init;
		}
	}

	public class PackageNode : CompilationUnit
	{
		public NodeList requires;
		public NodeList contains;

		public PackageNode(String name, NodeList requires, NodeList contains)
			: base(name)
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
		public String location;
	
		public UsesItem(String name) : base(name) { }

		public UsesItem(String name, String location) : base(name)
		{
			this.location = location;
		}
	}

	public class RequiresItem : UnitItem
	{
		public RequiresItem(String name) : base(name) { }
	}

	public class ContainsItem : UnitItem
	{
		public String location;

		public ContainsItem(String name) : base(name) { }

		public ContainsItem(String name, String location) : base(name)
		{
			this.location = location;
		}
	}

	public class ExportItem : UnitItem
	{
		public DeclarationList formalparams;
		public String exportname;
		public int index;

		public ExportItem(String name, DeclarationList pars, String exportname = null)
			: base(name)
		{
			this.formalparams = pars;
			this.exportname = exportname;
		}

		public ExportItem(String name, DeclarationList pars, int index)
			: base(name)
		{
			this.formalparams = pars;
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
		public DeclarationList decls;

		public Section()
		{
			decls = new DeclarationList();
		}

		public Section(DeclarationList decls)
		{
			this.decls = decls;
		}
	}

	public abstract class CodeSection : Section
	{
		public Statement block;

		public CodeSection(DeclarationList decls, Statement block)
			: base(decls)
		{
			this.block = block;
		}
	}

	public class ProgramBody : CodeSection
	{
		public ProgramBody(DeclarationList decls, Statement block) : base(decls, block) { }
	}

	public class RoutineBody : CodeSection
	{
		public RoutineBody(DeclarationList decls, Statement block) : base(decls, block) { }
	}

	public class InitializationSection : CodeSection
	{
		public InitializationSection(Statement body) : base(null, body) { }
	}

	public class FinalizationSection : CodeSection
	{
		public FinalizationSection(Statement body) : base(null, body) { }
	}


	public abstract class DeclarationSection : Section
	{
		public NodeList uses;

		public DeclarationSection(NodeList uses, DeclarationList decls)
			: base(decls)
		{
			this.uses = uses;
		}
	}
	
	public class InterfaceSection : DeclarationSection
	{
		public InterfaceSection(NodeList uses, DeclarationList decls) : base(uses, decls) { }
	}

	public class ImplementationSection : DeclarationSection
	{
		public ImplementationSection(NodeList uses, DeclarationList decls) : base(uses, decls) { }
	}

	public class AssemblerRoutineBody : RoutineBody
	{
		public AssemblerRoutineBody(AssemblerBlock asm) 
				: base(new DeclarationList(), asm) { }
	}

	#endregion

}