using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiPascal.AST.Nodes
{

	//==========================================================================
	// File Translation Units
	//==========================================================================

	#region Translation Units

	/// <summary>
	/// CompilationUnit: top level source file. Can be Program, Unit, Library or Package
	/// </summary>
	public abstract class TranslationUnit : Declaration
	{
		public TranslationUnit(String name) : base(name)
		{
			this.name = name;
		}
	}

	public class ProgramNode : TranslationUnit
	{
		public ProgramSection section;

		public ProgramNode(String name, NodeList uses, DeclarationList decls, BlockStatement body)
			: base( (name== null)? "$untitled$" : name)
		{
			section = new ProgramSection(uses, decls, body);
		}
	}

	public class LibraryNode : TranslationUnit
	{
		public ProgramSection section;

		public LibraryNode(String name, NodeList uses, DeclarationList decls, BlockStatement body)
			: base( (name== null)? "$untitled$" : name)
		{
			section = new ProgramSection(uses, decls, body);
		}
	}

	public class UnitNode : TranslationUnit
	{
		public InterfaceSection @interface;
		public ImplementationSection implementation;
		public BlockStatement initialization;
		public BlockStatement finalization;

		public UnitNode(String name, InterfaceSection interfce, ImplementationSection impl,
						BlockStatement init = null, BlockStatement final = null)
			: base(name)
		{
			@interface = interfce;
			implementation = impl;
			initialization = init;
			finalization   = final;
		}
	}

	public class PackageNode : TranslationUnit
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

		public Section(DeclarationList dls)
		{
			decls = dls;
			if (decls == null)
				decls = new DeclarationList();
			else
				foreach (var d in decls)
					if (d is CallableDeclaration)
						(d as CallableDeclaration).declaringSection = this;
		}
	}


	public abstract class TopLevelDeclarationSection : Section
	{
		public NodeList uses;

		public TopLevelDeclarationSection(NodeList uses, DeclarationList decls)
			: base(decls)
		{
			this.uses = uses;
		}
	}

	public class InterfaceSection : TopLevelDeclarationSection
	{
		public InterfaceSection(NodeList uses, DeclarationList decls) : base(uses, decls) { }
	}

	public class ImplementationSection : TopLevelDeclarationSection
	{
		public ImplementationSection(NodeList uses, DeclarationList decls) : base(uses, decls) { }
	}

	public class ProgramSection : TopLevelDeclarationSection
	{
		public BlockStatement block;

		public ProgramSection(NodeList uses, DeclarationList decls, BlockStatement code)
			: base(uses, decls)
		{
			this.block = code;
		}
	}

	public class RoutineSection : Section
	{
		public Statement block;

		// to be set by resolver
		public CallableDeclaration declaringCallable;

		public RoutineSection(DeclarationList decls, Statement block)
			: base(decls)
		{
			this.block = block;
		}
	}

	public class ParametersSection : Section
	{
		public OutParamDeclaration returnVar;

		public ParametersSection(DeclarationList decls = null)
			: base(decls)
		{
			returnVar = null;
			if (decls == null)
				decls = new DeclarationList();
		}

		public override bool Equals(object obj)
		{
			ParametersSection sec = obj as ParametersSection;
			return sec != null && returnVar.Equals(sec.returnVar) && decls.SequenceEqual(sec.decls);
		}
	}


	#endregion



	//==========================================================================
	// Imports
	//==========================================================================

	#region Imports

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

		public UsesItem(String name, String location)
			: base(name)
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

		public ContainsItem(String name, String location)
			: base(name)
		{
			this.location = location;
		}
	}

	public class ExportItem : UnitItem
	{
		public ParametersSection formalparams;
		public String exportname;
		public int index;

		public ExportItem(String name, ParametersSection pars, String exportname = null)
			: base(name)
		{
			this.formalparams = pars;
			this.exportname = exportname;
		}

		public ExportItem(String name, ParametersSection pars, int index)
			: base(name)
		{
			this.formalparams = pars;
			this.index = index;
		}
	}

	#endregion

}