using System;
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

	public class FieldInit : Node
	{
		public IdentifierNode ident;
		public Expression expr;

		public FieldInit(IdentifierNode ident, Expression expr)
		{
			this.ident = ident;
			this.expr = expr;
		}
	}

	public class FieldInitList : Node
	{
		public FieldInit init;
		public FieldInitList next;

		public FieldInitList(FieldInit init, FieldInitList next)
		{
			this.init = init;
			this.next = next;
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
		public IdentifierNode ident;
		public TypeNode type;
		public PropertyIndex index;
		public PropertySpecifierNode specs;
		public PropertyDefault def;

		public ClassProperty(IdentifierNode ident, TypeNode type, PropertyIndex index, PropertySpecifierNode specs, PropertyDefault def)
		{
			this.ident = ident;
			this.type = type;
			this.index = index;
			this.specs = specs;
			this.def = def;
		}
	}

	public class ClassBody : ClassContent
	{
		public Scope scope;
		public ClassFieldList fields;
		public ClassContentList content;

		public ClassBody(Scope scope, ClassFieldList fields, ClassContentList content)
		{
			this.scope = scope;
			this.fields = fields;
			this.content = content;
		}
	}


	public class PropertyReadNode : Node
	{
		public IdentifierNode ident;

		public PropertyReadNode(IdentifierNode ident)
		{
			this.ident = ident;
		}
	}

	public class PropertyWriteNode : Node
	{
		public IdentifierNode ident;

		public PropertyWriteNode(IdentifierNode ident)
		{
			this.ident = ident;
		}
	}

	public class PropertySpecifierNode : Node
	{
		public PropertyReadNode read;
		public PropertyWriteNode write;
		// add more here as necessary

		public PropertySpecifierNode(PropertyReadNode read, PropertyWriteNode write)
		{
			this.read = read;
			this.write = write;
		}
	}

	public class PropertyDefault : Node
	{
		public IdentifierNode ident;

		public PropertyDefault(IdentifierNode ident)
		{
			this.ident = ident;
		}
	}

	public class PropertyIndex : Node
	{
		public Expression expr;

		public PropertyIndex(Expression expr)
		{
			this.expr = expr;
		}
	}

}