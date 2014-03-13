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
	///			User-defined type => may be any
	///			RoutineType
	///			ClassType
	///			InterfaceType
	///			VariableType
	///				SimpleType
	///					ScalarType		: IOrdinalType
	///						IntegerType
	///							UnsignedInt	...
	///							SignedInt	...
	///						Bool
	///						Char
	///					FloatingPointType
	///						FloatType
	///						DoubleType
	///						ExtendedType
	///						CurrencyType
	///					StringType
	///					VariantType
	///					PointerType
	///				EnumType			: IOrdinalType
	///				RangeType			: IOrdinalType
	///				RefPointerType < VariableType> 
	///				MetaclassType < id>
	///				StructuredType
	///					Array < VariableType> 
	///					Set < VariableType> 
	///					File
	///					Record
	///			
	///			InterfaceDeclaration/ClassRefType
	///			
	///  </summary>



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



	// =========================================================================
	// Node Lists
	// =========================================================================

	#region Node Lists

	// Cannot use generics since YACC/JAY does not support them as rules' types

	/// <summary>
	/// Lists of Nodes, Expressions, Statements etc
	/// </summary>

	public abstract class ListNode<T> : Node, IEnumerable<T>
		where T : Node
	{
		List<T> nodes = new List<T>();

		public void Add(T t)
		{
			if (t != null)
				nodes.Add(t);
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
	}

	public class ExpressionList : ListNode<Expression>
	{
	}

	public class StatementList : ListNode<Statement>
	{
	}

	public class TypeList : ListNode<TypeNode>
	{
	}

	public class OrdinalTypeList : ListNode<IOrdinalType>
	{
	}

	public class IdentifierList : ListNode<Identifier>
	{
	}

	#endregion

}