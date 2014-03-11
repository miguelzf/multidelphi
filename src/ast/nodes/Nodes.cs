using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace crosspascal.ast.nodes
{
	/// <summary>
	/// Node hierarchy
	/// -------------------
	/// 
	/// DelphiNode
	///		EmptyNode
	///		ListNode
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
	///				NegationExpr
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
	///			ArrayType
	///			SetType		
	///			FileType
	///			ClassType
	///			VariantType
	///			CharType
	///			BoolType
	///			IntegerType
	///				UnsignedInt8Type
	///				UnsignedInt16Type
	///				UnsignedInt32Type
	///				UnsignedInt64Type
	///				SignedInt8Type
	///				SignedInt16Type
	///				SignedInt32Type
	///				SignedInt64Type
	///			FloatingPointType
	///				FloatType
	///				DoubleType
	///				ExtendedType
	///				CurrencyType
	///			StringType
	///			InterfaceDefinition
	///			
	///  </summary>



	public abstract class DelphiNode
	{

		/// <summary>
		/// TODO make abstract
		/// </summary>
		/// <param name="visitor"></param>
		public virtual void Accept(Processor visitor)
		{

		}
	}

	public class ListNode : DelphiNode
	{
		List<DelphiNode> nodes = new List<DelphiNode>();

		public void Add(DelphiNode t)
		{
			if (t != null)
				nodes.Add(t);
		}

		public override void Accept(Processor visitor)
		{
			foreach (DelphiNode node in nodes)
				node.Accept(visitor);
		}
	}


	public class FixmeNode : DelphiNode
	{
		public FixmeNode()
		{
			Console.WriteLine("This a development temporary node that should never be used. FIX ME!!\n");
		}
	}

	public class NotSupportedNode : DelphiNode
	{
		public NotSupportedNode()
		{
			Console.WriteLine("This feature is not yet supported.\n");
		}
	}

	public class EmptyNode : DelphiNode
	{
		public EmptyNode()
		{
		}
	}

	/// <summary>
	/// CompilationUnit: top level source file. Can be Program, Unit, Library or Package
	/// </summary>
	public abstract class CompilationUnit : DelphiNode
	{

	}


}