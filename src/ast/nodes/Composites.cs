using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace crosspascal.ast.nodes
{

	public enum Scope
	{
		Public,
		Protected,
		Private,
		Published
	}

	//
	// Composite Types
	//

	public abstract partial class CompositeType : TypeNode
	{
		ArrayList heritage;

		public CompositeType(ArrayList heritage)
		{
			this.heritage = heritage;
		}
	}

	public class ClassType : CompositeType
	{
		public ClassType(ArrayList heritage)
			: base(heritage)
		{
		}
	}

	public class InterfaceType : CompositeType
	{
		public InterfaceType(ArrayList heritage) 
			: base(heritage)
		{
		}
	}


	//
	// Composite Declarations
	//

	public abstract partial class CompositeDeclaration : TypeDeclaration
	{
		public bool IsPacked { get; set; }

		/// <summary>
		/// Class/interface body, with scoped sections of declarations.
		/// This is null in the case of forward class/interface declarations.
		/// </summary>
		public CompositeBody Body { get; set; }

		public CompositeDeclaration(String name, CompositeType ctype, CompositeBody body = null)
			: base(name, ctype)
		{
			this.Body = body;
		}
	}
	
	public class ClassDeclaration : CompositeDeclaration
	{
		public ClassDeclaration(String name, ArrayList heritage, ClassBody body)
			: base(name, new ClassType(heritage), body = null)
		{
		}
	}

	public class InterfaceDeclaration : CompositeDeclaration
	{
		String guid;

		public InterfaceDeclaration(String name, ArrayList heritage, 
									String guid = null, InterfaceBody body = null)
			: base(name, new ClassType(heritage), body)
		{
			this.guid = guid;
		}
	}


	#region Composite Sections
	//
	// Composite sections
	//

	public class ScopedSection : Section
	{
		Scope scope;

		DeclarationList fields;

		ScopedSection(Scope scope, DeclarationList fields, DeclarationList components)
			: base(components)
 		{
			this.scope	= scope;
			this.fields = fields;
		}
	}

	public class ScopedSectionList : ListNode<ScopedSection>
	{
		public ScopedSectionList() : base() { }

		public ScopedSectionList(ScopedSection s) : base(s) { }
	}


	public abstract class CompositeBody : Section
	{
		ScopedSectionList sections;

		public CompositeBody(ScopedSectionList sections)
		{
			this.sections = sections;
		}
	}

	public class ClassBody : CompositeBody
	{

		public ClassBody(ScopedSectionList sslist)
			: base(sslist)
		{

		}
	}

	public class InterfaceBody : CompositeBody
	{
		public InterfaceBody(ScopedSectionList sslist)
			: base(sslist)
		{
		}

		public InterfaceBody(ScopedSection ss)
			: base(new ScopedSectionList(ss))
		{
		}
	}

	#endregion


	#region Properties

	public class ClassProperty : ClassContent
	{
		public String ident;
		public TypeNode type;
		public PropertyIndex index;
		public PropertySpecifiers specs;
		public PropertyDefault def;

		public ClassProperty(String ident, TypeNode type, PropertyIndex index, PropertySpecifiers specs, PropertyDefault def)
		{
			this.ident = ident;
			this.type = type;
			this.index = index;
			this.specs = specs;
			this.def = def;
		}
	}

	public class PropertyReadNode : Node
	{
		public String ident;

		public PropertyReadNode(String ident)
		{
			this.ident = ident;
		}
	}

	public class PropertyWriteNode : Node
	{
		public String ident;

		public PropertyWriteNode(String ident)
		{
			this.ident = ident;
		}
	}

	public class PropertySpecifiers : Node
	{
		public PropertySpecifier index;
		public PropertySpecifier read;
		public PropertySpecifier write;
		public PropertySpecifier stored;
		public PropertySpecifier def;
		public PropertySpecifier impl;

		public PropertySpecifiers(PropertySpecifier index, PropertySpecifier read, PropertySpecifier write,
			PropertySpecifier stored, PropertySpecifier def, PropertySpecifier impl)
		{
			this.index = index;
			this.read = read;
			this.write = write;
			this.stored = stored;
			this.def = def;
			this.impl = impl;
		}
	}

	public abstract class PropertySpecifier : Node
	{

	}

	public class PropertyDefault : PropertySpecifier
	{
		public Literal lit;

		public PropertyDefault(Literal lit)
		{
			this.lit = lit;
		}
	}

	public class PropertyImplements : PropertySpecifier
	{
		public String ident;

		public PropertyImplements(String ident)
		{
			this.ident = ident;
		}
	}

	public class PropertyStored : PropertySpecifier
	{
		public String ident;

		public PropertyStored(String ident)
		{
			this.ident = ident;
		}
	}

	public class PropertyIndex : PropertySpecifier
	{
		public uint value;

		public PropertyIndex(uint value)
		{
			this.value = value;
		}
	}

	#endregion

}