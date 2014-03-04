using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace crosspascal.AST
{
	public enum FunctionClass
	{
		Procedure,
		Function,
		Constructor,
		Destructor
	}

	public enum Visibilty
	{
		Published,
		Public,
		Private,
		Protected 
	}

	public abstract class Node
	{ 
	}

	public abstract class GoalNode : Node
	{
	}

	public abstract class LiteralNode : Node
	{ 
		
	}

	public class IntegerLiteralNode : LiteralNode
	{
		public int value;

		public IntegerLiteralNode(int value)
		{
			this.value = value;
		}
	}

	public class StringLiteralNode : LiteralNode
	{
		public string value;

		public StringLiteralNode(string value)
		{
			this.value = value;
		}
	}

	public class BoolLiteralNode : LiteralNode
	{
		public bool value;

		public BoolLiteralNode(bool value)
		{
			this.value = value;
		}
	}

	public class RealLiteralNode : LiteralNode
	{
		public double value;

		public RealLiteralNode(double value)
		{
			this.value = value;
		}
	}

	public class NilLiteralNode : LiteralNode
	{
	}

	public class LValueNode : Node
	{
		public string ident;

		public LValueNode(string ident)
		{
			this.ident = ident;
		}
	}


	public class OperatorNode : Node
	{
		public string op;

		public OperatorNode(string op)
		{
			this.op = op;
		}
	}

	public class IdentifierNode : Node
	{
		public string value;

		public IdentifierNode(string val)
		{
			this.value = val;
		}
	}

	public class IdentifierNodeWithLocation : IdentifierNode
	{
		public string location;

		public IdentifierNodeWithLocation(string value, string location) : base(value)
		{
			this.location = location;
		}
	}

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

	public class ProgramNode : GoalNode
	{
		public IdentifierNode identifier;
		public Node body;
		public UsesNode uses;

		public ProgramNode(IdentifierNode ident, UsesNode uses, Node body)
		{
			this.identifier = ident;
			this.uses = uses;
			this.body = body;
		}
	}

	public class LibraryNode: GoalNode
	{
		public IdentifierNode identifier;
		public Node body;
		public UsesNode uses;

		public LibraryNode(IdentifierNode ident, UsesNode uses, Node body)
		{
			this.identifier = ident;
			this.uses = uses;
			this.body = body;
		}
	}

	public class InterfaceNode: Node
	{
	}

	public class ImplementationNode : Node
	{
	}

	public class UnitNode : GoalNode
	{
		public IdentifierNode identifier;
		public UsesNode uses;
		public InterfaceNode interfce;
		public ImplementationNode implementation;
		public Node init;

		public UnitNode(IdentifierNode ident, UsesNode uses, InterfaceNode interfce, ImplementationNode impl, Node init)
		{
			this.identifier = ident;
			this.uses = uses;
			this.interfce = interfce;
			this.implementation = impl;
			this.init = init;
		}
	}

	public class PackageNode : GoalNode
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

}
