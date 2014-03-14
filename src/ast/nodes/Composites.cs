using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace crosspascal.ast.nodes
{
	public abstract partial class CompositeDeclaration : TypeDeclaration
	{
	
	}

	public enum Scope
	{
		Public,
		Protected,
		Private,
		Published
	}


	public class ClassDefinition : CompositeDeclaration
	{
		public ClassType classType;
		public IdentifierList heritage;
		public ClassBody ClassBody;

		public ClassDefinition(ClassType classType, IdentifierList heritage, ClassBody ClassBody)
		{
			this.classType = classType;
			this.heritage = heritage;
			this.ClassBody = ClassBody;
		}
	}

	public class InterfaceDefinition : CompositeDeclaration
	{
		public IdentifierList heritage;
		public ClassContentList methods;
		public ClassContentList properties;

		public InterfaceDefinition(IdentifierList heritage, ClassContentList methods, ClassContentList properties)
		{
			this.heritage = heritage;
			this.methods = methods;
			this.properties = properties;
		}
	}

	public class ClassFieldList : Node
	{
		public VarDeclaration decl;
		public ClassFieldList next;

		public ClassFieldList(VarDeclaration decl, ClassFieldList next)
		{
			this.decl = decl;
			this.next = next;
		}
	}

	public abstract class ClassContent : Node
	{

	}

	public class ClassContentList : ClassContent
	{
		public ClassContent content;
		public ClassContentList next;

		public ClassContentList(ClassContent content, ClassContentList next)
		{
			this.content = content;
			this.next = next;
		}
	}

	public class ClassMethod : ClassContent
	{
		public RoutineDeclaration decl;

		public ClassMethod(RoutineDeclaration decl)
		{
			this.decl = decl;
		}
	}

	public class ClassProperty : ClassContent
	{
		public Identifier ident;
		public TypeNode type;
		public PropertyIndex index;
		public PropertySpecifiers specs;
		public PropertyDefault def;

		public ClassProperty(Identifier ident, TypeNode type, PropertyIndex index, PropertySpecifiers specs, PropertyDefault def)
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
		public Identifier ident;

		public PropertyReadNode(Identifier ident)
		{
			this.ident = ident;
		}
	}

	public class PropertyWriteNode : Node
	{
		public Identifier ident;

		public PropertyWriteNode(Identifier ident)
		{
			this.ident = ident;
		}
	}

	public class PropertySpecifiers : Node
	{
		public PropertyIndex index;
		public PropertyReadNode read;
		public PropertyWriteNode write;
		public PropertyStored stored;
		public PropertyDefault def;
		public PropertyImplements impl;

		public PropertySpecifiers(PropertyIndex index, PropertyReadNode read, PropertyWriteNode write, 
			PropertyStored stored, PropertyDefault def, PropertyImplements impl)
		{
			this.index = index;
			this.read = read;
			this.write = write;
			this.stored = stored;
			this.def = def;
			this.impl = impl;
		}
	}

	public class PropertyDefault : Node
	{
		public Identifier ident;

		public PropertyDefault(Identifier ident)
		{
			this.ident = ident;
		}
	}

	public class PropertyImplements : Node
	{
		public Identifier ident;

		public PropertyImplements(Identifier ident)
		{
			this.ident = ident;
		}
	}

	public class PropertyStored : Node
	{
		public Identifier ident;

		public PropertyStored(Identifier ident)
		{
			this.ident = ident;
		}
	}

	public class PropertyIndex : Node
	{
		public int value;

		public PropertyIndex(int value)
		{
			this.value = value;
		}
	}

}