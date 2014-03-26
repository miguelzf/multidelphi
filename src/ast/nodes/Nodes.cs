using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace crosspascal.ast.nodes
{
	/// <summary>
	/// Delphi Nodes hierarchy
	/// -------------------
	/// 
	/// Node
	///		EmptyNode
	///		NodeList
	///		CompilationUnit	
	///			Program
	///			Unit
	///			Library
	///			Package
	///		Section
	///			Interface
	///			Implementation
	///			Initialization
	///			Finalization
	///			Main
	///			Uses
	///			Exports
	///			RoutineBody
	///	
	///  </summary>



	public abstract class Node
	{
		public Node Parent { get; set; }

		/// <summary>
		/// Report and propagate errors from the syntactic and simple-semantic checks done during the creation of the AST
		/// </summary>
		/// <param name="visitor"></param>
		static protected bool Error(string msg)
		{
			string outp = "[ERROR] " + msg;
			// throw new crosspascal.parser.AstNodeException(outp);
			Console.Error.WriteLine(outp);
			return false;
		}

		/// <summary>
		/// Report internal errors
		/// </summary>
		/// <param name="visitor"></param>
		static protected bool ErrorInternal(string msg)
		{
			string outp = "[ERROR internal] " + msg;
			// throw new crosspascal.parser.AstNodeException(outp);
			Console.Error.WriteLine(outp);
			return false;
		}

		/// <summary>
		/// Determines whether the current class is a, or derives, from the class of the specified System.Type
		/// </summary>
		public virtual bool ISA(System.Type o)
		{
			return this.GetType().IsSubclassOf(o) || this.GetType() == o;
		}

		/// <summary>
		/// Determines whether the current class is a, or derives, from the class of the specified Node
		/// </summary>
		public virtual bool ISA(Node o)
		{
			return ISA(o.GetType());
		}


		/// <summary>
		/// TODO make abstract
		/// </summary>
		/// <param name="visitor"></param>
		public virtual bool Accept(Processor visitor)
		{
			return visitor.Visit(this);
		}
	}

	public class FixmeNode : Node
	{
		public FixmeNode()
		{
			Console.WriteLine("This a development temporary node that should never be used. FIX ME!!\n");
		}
	}

	public class NotSupportedNode : Node
	{
		public NotSupportedNode()
		{
			Console.WriteLine("This feature is not yet supported.\n");
		}
	}

	public class EmptyNode : Node
	{
		public EmptyNode()
		{
		}
	}



	// =========================================================================
	// Node Lists
	// =========================================================================

	#region Node Lists

	/// <summary>
	/// Lists of Nodes, Expressions, Statements etc
	/// </summary>
	public interface IListNode<T> : IEnumerable<T> where T : Node
	{
		void Add(T t);

		void Add(IEnumerable<T> t);

		void InsertAt(int idx, T t);
	}

	public abstract class ListNode<T> : Node, IListNode<T> where T : Node
	{
		public List<T> nodes = new List<T>();

		public ListNode()
		{
		}
		public ListNode(T t)
		{
			Add(t);
		}
		public ListNode(IEnumerable<T> ts)
		{
			Add(ts);
		}

		public T Get(int idx)
		{
			if (idx < 0 || idx > nodes.Count - 1)
				return null;
			else
				return nodes.ElementAt(idx);
		}

		public T GetFirst()
		{
			return nodes.ElementAt(0);
		}

		public T GetLast()
		{
			return nodes.ElementAt(nodes.Count-1);
		}

		/// <summary>
		/// Adds element to start of list
		/// </summary>
		public void AddStart(T t)
		{
			this.InsertAt(0, t);
		}

		/// <summary>
		/// Adds elements to start of list
		/// </summary>
		public void AddStart(IEnumerable<T> col)
		{
			if (col != null && col.Count<T>() > 0)
				foreach (T t in col.Reverse<T>())
					this.InsertAt(0, t);
		}

		public void Add(T t)
		{
			if (t != null)
				nodes.Add(t);
		}

		public void Add(IEnumerable<T> t)
		{
			if (t != null && t.Count<T>() > 0)
				nodes.AddRange(t);
		}

		public void InsertAt(int idx, T t)
		{
			if ( t!= null)
				nodes.Insert(idx, t);
		}

		public IEnumerator<T> GetEnumerator()
		{
			return nodes.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return nodes.GetEnumerator();
		}

		public override bool Accept(Processor visitor)
		{
			bool ret = true;
			foreach (T node in nodes)
				ret &= node.Accept(visitor);
			return ret;
		}
	}


	/// <remarks>
	/// Do not use generics, since Yacc/Jay doesn't support them as nodes' types
	/// </remarks>

	public class NodeList : ListNode<Node>
	{
		public NodeList() : base() { }

		public NodeList(Node t) : base(t) { }
	}

	public class StatementList : ListNode<Statement>
	{
		public StatementList() : base() { }

		public StatementList(Statement t) : base(t) { }

		public StatementList(IEnumerable<Statement> ts) : base(ts) { }
	}

	public class TypeList : ListNode<TypeNode>
	{
		public TypeList() { }

		public TypeList(TypeNode t) : base(t) { }
	}

	public class IntegralTypeList : ListNode<IntegralType>
	{
		public IntegralTypeList() { }
	}

	public class IdentifierList : ListNode<Identifier>
	{
		public IdentifierList() { }
	}

	public class DeclarationList : ListNode<Declaration>
	{
		public DeclarationList() : base() { }

		public DeclarationList(Declaration t) : base(t) { }

		public DeclarationList(IEnumerable<Declaration> ts) : base(ts) { }

		public Declaration GetDeclaration(String name)
		{
			return GetDeclarations(name).ElementAt(0);
		}

		public IEnumerable<Declaration> GetDeclarations(String name)
		{
			return nodes.Where(x => x.name == name);
		}
	}

	public class EnumValueList : ListNode<EnumValue>
	{
		public EnumValueList() { }

		public EnumValueList(EnumValue t) : base(t) { }
	}

	#endregion

}