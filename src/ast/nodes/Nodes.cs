using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MultiPascal.Parser;

namespace MultiPascal.AST.Nodes
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


	public struct Location
	{
		public int line;

		public string file;

		public Location(int l, string f)
		{
			line = l;
			file = f;
		}

		public override String ToString()
		{
			return " in file " + file + " line " + line;
		}
	}

	public abstract class Node
	{
		public Node Parent { get; set; }

		public Location Loc { get; set; }

		public Node()
		{
			if (DelphiParser.Instance.IsParsing())
				Loc = DelphiParser.Instance.CurrentLocation();
			else
				Loc = new Location(0, "runtime");
		}



		/// <summary>
		/// Returns type name without qualifier part
		/// </summary>
		public String NodeName()
		{
			String fullname = this.ToString();
			return fullname.Substring(fullname.LastIndexOf('.') + 1);
		}

		/// <summary>
		/// Returns name + location
		/// </summary>
		public String NameLoc()
		{
			return NodeName() + Loc.ToString();
		}

		/// <summary>
		/// Report and propagate errors from the syntactic and simple-semantic checks done during the creation of the AST
		/// </summary>
		/// <param name="visitor"></param>
		protected bool Error(string msg)
		{
			string outp = "[ERROR in node creation] " + msg + Loc.ToString();
			// throw new MultiPascal.Parser.AstNodeException(outp);
			Console.ForegroundColor = ConsoleColor.Red;
			Console.Error.WriteLine(outp);
			Console.ResetColor();
			return false;
		}

		/// <summary>
		/// Report internal errors
		/// </summary>
		/// <param name="visitor"></param>
		protected bool ErrorInternal(string msg)
		{
			string outp = "[ERROR internal] " + msg + Loc.ToString();
			// throw new MultiPascal.parser.AstNodeException(outp);
			Console.ForegroundColor = ConsoleColor.Red;
			Console.Error.WriteLine(outp);
			Console.ResetColor();
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
		public virtual T Accept<T>(Processor<T> visitor)
		{
			// TODO create 1 accept for each specific node to implement the visitor pattern
			// so much hassle...
			return visitor.Visit(this);
		}
	}

	public class FixmeNode : Node
	{
		public FixmeNode()
		{
			Error("This a development temporary node that should never be used. FIX ME!!\n");
		}
	}

	public class NotSupportedNode : Node
	{
		public NotSupportedNode()
		{
			Error("This feature is not yet supported.\n");
		}
	}

	/// <summary>
	/// Useful to avoid nulls
	/// </summary>
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

		public void Clear()
		{
			nodes.Clear();
		}

		public void Replace(IEnumerable<T> newelems)
		{
			nodes.Clear();
			nodes.AddRange(newelems);
		}

		public IEnumerator<T> GetEnumerator()
		{
			return nodes.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return nodes.GetEnumerator();
		}

		public override VT Accept<VT>(Processor<VT> visitor)
		{
			foreach (T node in nodes)
				node.Accept(visitor);
			return default(VT);
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

	public class DeclarationList : ListNode<Declaration>
	{
		public static DeclarationList Empty = new DeclarationList();

		public DeclarationList() : base() { }

		public DeclarationList(Declaration t) : base(t) { }

		public DeclarationList(IEnumerable<Declaration> ts) : base(ts) { }

		public Declaration GetDeclaration(String name)
		{
			return nodes.FirstOrDefault(x => x.name == name);
		}

		public IEnumerable<Declaration> GetDeclarations(String name)
		{
			return nodes.Where(x => x.name == name);
		}
	}

	public class ExpressionList : ListNode<Expression>
	{
		public ExpressionList()
		{
		}
		public ExpressionList(Expression t)
		{
			Add(t);
		}
		public ExpressionList(IEnumerable<Expression> t)
		{
			Add(t);
		}
	}

	public class FieldInitList : ExpressionList
	{
		public void Add(FieldInit t)
		{
			base.Add(t);
		}

		public FieldInitList(FieldInit t)
			: base(t)
		{
		}
	}
	
	public class EnumValueList : ListNode<EnumValue>
	{
		public EnumValueList() { }

		public EnumValueList(EnumValue t) : base(t) { }
	}

	#endregion

}