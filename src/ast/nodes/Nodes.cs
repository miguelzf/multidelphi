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
	///		Declaration
	///			FuncRetDecl
	///			FuncParamDel	// of routines
	///				DefaultParam
	///				VarParam
	///				OutParam
	///				ConstParam
	///			TypeDecl
	///				ObjectDecl
	///					Interf
	///					Record
	///					Class
	///					Object
	///			VarDecl
	///			ConstDecl
	///			RscStrDecl
	///			RoutineDecl
	///				Function
	///				Procedure
	///				Constructor
	///				Destructor
	///				Property !? or PropertyAcessor/Specified??
	///				
	///			ObjFieldDecl !?
	///			
	///		Statement
	///			EmptyStmt
	///			LabeledStmt		// Label + Stmt
	///			ProcCallStmt	// includes inherits
	///			Assign
	///			Block
	///			With
	///			AsmBlock
	///			If
	///			TryExcept
	///			TryFinally
	///			Raise
	///			Loop
	///				For
	///				While
	///				RepeatUntil
	///			Case
	///			ControlFlowStmt
	///				Break
	///				Continue
	///				Goto
	/// 
	///		Expression
	///			EmptyExpr
	///			UnaryExpr
	///				Literal
	///					Int
	///					Char
	///					String
	///					Real
	///					Ptr (nil)
	///					Bool
	///				PtrDeref
	///				LogicalNotExpr
	///				AddrExpr
	///				Lvalue
	///					Identifier
	///					RoutineCall
	///					CastExpr
	///					FieldAccess
	///					ArrayAcess
	///			BinaryExpr
	///				Additive
	///				Multiplicative
	///				Conditional
	///				
	///				enums, sets, ranges, initializers TODO
	///				
	///		Types
	///			....	TODO
	///			
	/// </summary>



	public abstract class Node
	{

		/// <summary>
		/// Report and propagate errors ocurred during the syntactic 
		/// and simple-semantic checks done at the construction of nodes
		/// </summary>
		/// <param name="visitor"></param>
		protected void Error(string msg)
		{
			throw new crosspascal.parser.AstNodeException(msg);
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

	/// <summary>
	/// CompilationUnit: top level source file. Can be Program, Unit, Library or Package
	/// </summary>
	public abstract class CompilationUnit : Node
	{

	}


	// =========================================================================
	// Node Lists
	// =========================================================================

	#region Node Lists

	// Cannot use generics since YACC/JAY does not support them as rules' types

	/// <summary>
	/// Lists of Nodes, Expressions, Statements etc
	/// </summary>

	public class NodeList : Node
	{
		List<Node> nodes = new List<Node>();

		public void Add(Node t)
		{
			if (t != null)
				nodes.Add(t);
		}

		public override void Accept(Processor visitor)
		{
			foreach (Node node in nodes)
				node.Accept(visitor);
		}
	}

	public class ExpressionList : NodeList
	{
		List<Expression> nodes = new List<Expression>();

		public void Add(Expression t)
		{
			if (t != null)
				nodes.Add(t);
		}

		public override void Accept(Processor visitor)
		{
			foreach (Expression node in nodes)
				node.Accept(visitor);
		}
	}

	public class StatementList : NodeList
	{
		List<Statement> nodes = new List<Statement>();

		public void Add(Statement t)
		{
			if (t != null)
				nodes.Add(t);
		}

		public override void Accept(Processor visitor)
		{
			foreach (Statement node in nodes)
				node.Accept(visitor);
		}
	}

	public class TypeList : NodeList
	{
		List<TypeNode> nodes = new List<TypeNode>();

		public void Add(TypeNode t)
		{
			if (t != null)
				nodes.Add(t);
		}

		public override void Accept(Processor visitor)
		{
			foreach (TypeNode node in nodes)
				node.Accept(visitor);
		}
	}

	public class IdentifierList : ExpressionList
	{
		List<Identifier> nodes = new List<Identifier>();

		public void Add(Identifier t)
		{
			if (t != null)
				nodes.Add(t);
		}

		public override void Accept(Processor visitor)
		{
			foreach (Identifier node in nodes)
				node.Accept(visitor);
		}
	}

	#endregion

}