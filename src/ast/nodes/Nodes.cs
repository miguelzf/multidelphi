using System;
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
			// throw new crosspascal.parser.AstNodeException(msg);
			Console.Error.WriteLine(msg);
			return false;
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
	/// 
	public interface IListNode<T> : IEnumerable<T> where T : Node
	{
		void Add(T t);

		void InsertAt(int idx, T t);
	
		// IEnumerator<T> IEnumerable<T>.GetEnumerator();

		void Accept(Processor visitor);
	}

	public abstract class ListNode<T> : Node, IListNode<T> where T : Node
	{
		List<T> nodes = new List<T>();

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

		public void InsertAt(int idx, T t)
		{
			if ( t!= null)
				nodes.Insert(idx, t);
		}

		IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
			return nodes.GetEnumerator();
		}

		public override void Accept(Processor visitor)
		{
			foreach (T node in nodes)
				node.Accept(visitor);
		}
	}



	public class NodeList : ListNode<Node>
	{
		public NodeList() { }
	}

	public class StatementList : ListNode<Statement>
	{
		public StatementList() { }
	}

	public class TypeList : ListNode<TypeNode>
	{
		public TypeList() { }
	}

	public class OrdinalTypeList : Node, System.Collections.IEnumerable
	{
		public System.Collections.IEnumerator GetEnumerator()
		{
			// TODO
			return null;
		}

		public OrdinalTypeList() { }
	}

	public class IdentifierList : ListNode<Identifier>
	{
		public IdentifierList() { }
	}

	public class DeclarationList : ListNode<Declaration>
	{
		public DeclarationList() { }
	}

	public class EnumValueList : ListNode<EnumValue>
	{
		public EnumValueList() { }
	}

	public class ParameterList : ListNode<ParameterDeclaration>
	{
		public ParameterList() { }
	}

	public class FieldList : ListNode<FieldDeclaration>
	{
		public FieldList() { }
	}

	#endregion

}