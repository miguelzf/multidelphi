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
		public virtual void Accept(Processor visitor)
		{

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

		void InsertAt(int idx, T t);
	
		void Accept(Processor visitor);
	}

	public abstract class ListNode<T> : Node, IListNode<T> where T : Node
	{
		List<T> nodes = new List<T>();

		public ListNode()
		{
		}
		public ListNode(T t)
		{
			Add(t);
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

		IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
			return nodes.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return nodes.GetEnumerator();
		}

		public override void Accept(Processor visitor)
		{
			foreach (T node in nodes)
				node.Accept(visitor);
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
	}

	public class EnumValueList : ListNode<EnumValue>
	{
		public EnumValueList() { }

		public EnumValueList(EnumValue t) : base(t) { }
	}

	public class ParameterList : ListNode<ParameterDeclaration>
	{
		public ParameterList() { }

		public ParameterList(ParameterDeclaration t) : base(t) { }
	}

	public class FieldList : ListNode<FieldDeclaration>
	{
		public FieldList() { }
	}

	#endregion

}