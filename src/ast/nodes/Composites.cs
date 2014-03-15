using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace crosspascal.ast.nodes
{
	public abstract partial class CompositeDeclaration : TypeDeclaration
	{
		public bool IsPacked;

	}

	public enum Scope
	{
		Public,
		Protected,
		Private,
		Published
	}

	public enum ClassKind
	{
		Class,
		Object
	}

	public class ClassBody : Section
	{
		// TODO rever

		public Scope scope;
		public NodeList fields;
		public NodeList content;

		public ClassBody(Scope scope, NodeList fields, NodeList content)
		{
			this.scope = scope;
			this.fields = fields;
			this.content = content;
		}
		public ClassBody(NodeList decls) { }
	}


	public class ClassDefinition : CompositeDeclaration
	{
		public ClassKind classType;
		public ArrayList heritage;
		public ClassBody ClassBody;

		public ClassDefinition(ClassKind classType, ArrayList heritage, ClassBody ClassBody)
		{
			this.classType = classType;
			this.heritage = heritage;
			this.ClassBody = ClassBody;
		}
	}

	public class InterfaceDefinition : CompositeDeclaration
	{
		public ArrayList heritage;
		public NodeList methods;
		public NodeList properties;

		public InterfaceDefinition(ArrayList heritage, NodeList methods, NodeList properties)
		{
			this.heritage = heritage;
			this.methods = methods;
			this.properties = properties;
		}
	}

	public abstract class ClassContent : Node
	{

	}

	public class ClassMethod : ClassContent
	{
		public RoutineDeclaration decl;

		public ClassMethod(RoutineDeclaration decl)
		{
			this.decl = decl;
		}
	}


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

	public abstract class PropertySpecifier
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