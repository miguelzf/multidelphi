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
	///			LabeledStmt	< Label, Stmt >
	///			ProcCallStmt <RoutineCall expr>
	///			Assign < lvalue, expression>
	///			Inherited stmt < Statement >
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
	///				
	/// Types
	/// 	DeclaredType => may be any, user-defined
	/// 	UndefinedType	< for untyped parameters. incompatible with any type >
	/// 	RoutineType
	/// 	ClassType
	/// 	VariableType
	/// 		ScalarType
	/// 			SimpleType		: IOrdinalType
	/// 				IntegerType
	/// 					UnsignedInt	...
	/// 					SignedInt	...
	/// 				Bool
	/// 				Char
	/// 			RealType
	/// 				FloatType
	/// 				DoubleType
	/// 				ExtendedType
	/// 				CurrencyType
	/// 			StringType
	/// 			VariantType
	/// 			PointerType <ScalarType> 
	/// 		EnumType			: IOrdinalType
	/// 		RangeType			: IOrdinalType
	/// 		MetaclassType < id>
	/// 		StructuredType
	/// 			Array < VariableType> 
	/// 			Set	  < VariableType> 
	/// 			File  < VariableType> 
	/// 		Record
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
		public NodeList() { }
	}

	public class ExpressionList : ListNode<Expression>
	{
		public ExpressionList() { }
	}

	public class StatementList : ListNode<Statement>
	{
		public StatementList() { }
	}

	public class TypeList : ListNode<TypeNode>
	{
		public TypeList() { }
	}

	public class OrdinalTypeList : ListNode<IOrdinalType>
	{
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

	#endregion

}