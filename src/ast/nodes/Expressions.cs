using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace crosspascal.ast.nodes
{
	#region Expressions hierarchy
	/// <remarks>
	///		Expression
	///			EmptyExpr
	///			ExpressionList
	///			ConstExpression
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
	///				Arithmetic
	///				Type
	///				Logical
	/// </remarks>
	#endregion

	//==========================================================================
	// Expressions' base classes
	//==========================================================================
	public abstract class Expression : Node
	{
		/// <summary>
		/// Expression type, to be checked or determined in static type-validation
		/// </summary>
		public TypeNode Type { get; set; }

		/// <summary>
		/// Indicates if expression is constant
		/// </summary>
		public bool IsConst { get; set; }

		/// <summary>
		/// Expression value, if constant
		/// </summary>
		public Literal Value { get; set; }

		/// <summary>
		/// Indicates if expression must be constant
		/// </summary>
		public bool EnforceConst { get; set; }

		/// <summary>
		/// Type that must be enforced/checked
		/// </summary>
		public TypeNode ForcedType { get; set; }
	}

	public class EmptyExpression : Expression
	{
		// Do nothing
	}


	public class ExpressionList : Expression, IListNode<Expression>
	{
		List<Expression> nodes = new List<Expression>();

		public virtual void Add(IEnumerable<Expression> t)
		{
			nodes.AddRange(t);
		}
		
		public ExpressionList()
		{
		}

		public ExpressionList(IEnumerable<Expression> t)
		{
			Add(t);
		}

		public virtual void Add(Expression t)
		{
			if (t != null)
				nodes.Add(t);
		}

		public virtual void InsertAt(int idx, Expression t)
		{
			if (t != null)
				nodes.Insert(idx, t);
		}

		IEnumerator<Expression> IEnumerable<Expression>.GetEnumerator()
		{
			return nodes.GetEnumerator();
		}

		public override void Accept(Processor visitor)
		{
			foreach (Expression node in nodes)
				node.Accept(visitor);
		}
	}


	#region Constant and initiliazer expressions
	//==========================================================================
	// Constants and Initializers
	//==========================================================================

	/// <summary>
	/// Wrapper node for const expressions
	/// Should be used for all initializations
	/// </summary>
	public class ConstExpression : Expression
	{
		Expression expr;

		public ConstExpression(Expression expr)
		{
			this.expr = expr;
			this.EnforceConst = true;
			expr.EnforceConst = true;
		}
	}

	public abstract class StructuredConstant : ConstExpression
	{
		public StructuredConstant(ExpressionList exprlist) : base(exprlist) { }
	}

	public class ArrayConst : StructuredConstant
	{
		public ArrayConst(ExpressionList exprlist) : base(exprlist) { }

		/// <summary>
		/// String initializer for Char Arrays
		/// </summary>
		/// <param name="arrayElems"></param>
		public ArrayConst(String arrayElems)
			: base(new ExpressionList(arrayElems.ToCharArray().Select(x => new CharLiteral(x)))){ }
	}

	public class RecordConst : StructuredConstant
	{
		public RecordConst(FieldInitList exprlist) : base(exprlist) { }
	}

	public class FieldInitList : ExpressionList
	{
		List<FieldInit> nodes = new List<FieldInit>();

		public void Add(FieldInit t)
		{
			if (t != null)
				nodes.Add(t);
		}
	}

	public class FieldInit : ConstExpression
	{
		string fieldname;

		public FieldInit(string name, Expression expr)
			: base(expr)
		{
			fieldname = name;
		}
	}

	#endregion


	#region Binary Expressions
	//==========================================================================
	// Binary Expressions
	//==========================================================================

	public abstract class BinaryExpression : Expression
	{
	}

	public class SetIn : BinaryExpression
	{
		Expression expr;
		Expression set;		// enforce that 'set' is in fact a set

		public SetIn(Expression e1, Expression e2)
		{
			expr = e1;
			set = e2;
		}
	}

	#region Arithmetic Binary Expressions

	public abstract class ArithmethicBinaryExpression : BinaryExpression
	{
		Expression left;
		Expression right;

		public ArithmethicBinaryExpression(Expression e1, Expression e2)
		{
			left = e1;
			right= e2;
		}
	}

	public class Subtraction : ArithmethicBinaryExpression
	{
		public Subtraction(Expression e1, Expression e2) : base(e1, e2) { }
	}

	public class Addition : ArithmethicBinaryExpression
	{
		public Addition(Expression e1, Expression e2) : base(e1, e2) { }
	}

	public class Product : ArithmethicBinaryExpression
	{
		public Product(Expression e1, Expression e2) : base(e1, e2) { }
	}

	public class Division: ArithmethicBinaryExpression
	{
		public Division(Expression e1, Expression e2) : base(e1, e2) { }
	}

	public class Quotient: ArithmethicBinaryExpression
	{
		public Quotient(Expression e1, Expression e2) : base(e1, e2) { }
	}

	public class Modulus: ArithmethicBinaryExpression
	{
		public Modulus(Expression e1, Expression e2) : base(e1, e2) { }
	}

	public class ShiftRight: ArithmethicBinaryExpression
	{
		public ShiftRight(Expression e1, Expression e2) : base(e1, e2) { }
	}

	public class ShiftLeft: ArithmethicBinaryExpression
	{
		public ShiftLeft(Expression e1, Expression e2) : base(e1, e2) { }
	}
	#endregion

	#region Logical Binary Expressions

	public abstract class LogicalBinaryExpression : BinaryExpression
	{
		Expression left;
		Expression right;

		public LogicalBinaryExpression(Expression e1, Expression e2)
		{
			left = e1;
			right= e2;
		}
	}

	public class LogicalAnd : LogicalBinaryExpression
	{
		public LogicalAnd(Expression e1, Expression e2) : base(e1, e2) { }
	}

	public class LogicalOr : LogicalBinaryExpression
	{
		public LogicalOr(Expression e1, Expression e2) : base(e1, e2) { }
	}

	public class LogicalXor : LogicalBinaryExpression
	{
		public LogicalXor(Expression e1, Expression e2) : base(e1, e2) { }
	}

	public class Equal : LogicalBinaryExpression
	{
		public Equal(Expression e1, Expression e2) : base(e1, e2) { }
	}

	public class NotEqual : LogicalBinaryExpression
	{
		public NotEqual(Expression e1, Expression e2) : base(e1, e2) { }
	}

	public class LessThan : LogicalBinaryExpression
	{
		public LessThan(Expression e1, Expression e2) : base(e1, e2) { }
	}

	public class LessOrEqual : LogicalBinaryExpression
	{
		public LessOrEqual(Expression e1, Expression e2) : base(e1, e2) { }
	}

	public class GreaterThan : LogicalBinaryExpression
	{
		public GreaterThan(Expression e1, Expression e2) : base(e1, e2) { }
	}

	public class GreaterOrEqual : LogicalBinaryExpression
	{
		public GreaterOrEqual(Expression e1, Expression e2) : base(e1, e2) { }
	}

	#endregion

	#region Typing Binary Expressions

	public abstract class TypeBinaryExpression : BinaryExpression
	{
		Expression expr;
		TypeNode types;

		public TypeBinaryExpression(Expression e1, TypeNode e2)
		{
			expr = e1;
			Type = e2;
		}
	}

	public class TypeIs : TypeBinaryExpression
	{
		public TypeIs(Expression e1, TypeNode e2) : base(e1, e2) { }
	}

	/// <summary>
	/// 'Expr AS Type'
	/// </summary>
	public class TypeCast : TypeBinaryExpression
	{
		public TypeCast(Expression e1, TypeNode e2) : base(e1, e2) { }
	}
	#endregion

	#endregion	// Binary expressions


	#region Unary Expressions
	//==========================================================================
	// Unary Expressions
	//==========================================================================

	public abstract class UnaryExpression : Expression
	{
	}

	public class LogicalNot : UnaryExpression
	{
		public Expression exp;

		public LogicalNot(Expression exp)
		{
			this.exp = exp;
		}
	}

	public class AddressLvalue : UnaryExpression
	{
		public LvalueExpression exp;

		public AddressLvalue(LvalueExpression exp)
		{
			this.exp = exp;
		}
	}

	public class SetRange : UnaryExpression
	{
		public Expression min;
		public Expression max;

		public SetRange(Expression min, Expression max)
		{
			this.min = min;
			this.max = max;
		}
	}

	public class Set : UnaryExpression
	{
		public ExpressionList setelems;

		public Set(ExpressionList elems)
		{
			setelems = elems;
		}
	}

	#endregion


	#region Literals
	//==========================================================================
	// Literals
	//==========================================================================

	public abstract class Literal : UnaryExpression
	{
		public Object value;

		public Literal() { }

		public Literal(Object t)
		{
			value = t;
		}
	}

	public class IntLiteral : Literal
	{
		public IntLiteral(Int32 value) : base(value) { }
	}

	public class CharLiteral : Literal
	{
		public CharLiteral(Char value) : base(value) { }
	}

	public class StringLiteral : Literal
	{
		public bool isChar;

		public StringLiteral(string value) : base(value)
		{
			if (value.Length == 1)
				isChar = true;
		}
	}

	public class BoolLiteral : Literal
	{
		public BoolLiteral(Boolean value) : base(value) { }
	}

	public class RealLiteral : Literal
	{
		public RealLiteral(Double value) : base(value) { }
	}

	public class PointerLiteral : Literal
	{
		public PointerLiteral(uint value)
		{
			if (value != 0x0)	// Const_NIl
				Error("Pointer constant can only be nil");
			this.value = value;
		}
	}

	#endregion


	#region Left-value expressions
	//==========================================================================
	// Lvalue Expressions
	//==========================================================================

	public abstract class LvalueExpression : UnaryExpression
	{
	}

	public class ArrayAccess : LvalueExpression
	{
		public LvalueExpression lvalue;
		public ExpressionList acessors;

		public ArrayAccess(LvalueExpression lvalue, ExpressionList acessors)
		{
			this.lvalue = lvalue;
			this.acessors = acessors;
		}
	}

	public class PointerDereference : LvalueExpression
	{
		public Expression expr;

		public PointerDereference(Expression expr)
		{
			this.expr = expr;
		}
	}

	public class InheritedCall : LvalueExpression
	{
		RoutineCall call;

		public InheritedCall(RoutineCall call)
		{
			this.call = call;
		}
	}

	public class RoutineCall : LvalueExpression
	{
		public Identifier fname;
		public ExpressionList args;

		public RoutineCall(Identifier fname)
		{
			this.fname = fname;
			args = new ExpressionList();
		}

		public RoutineCall(Identifier fname, ExpressionList args) :this(fname)
		{
			this.args = args;
		}
	}

	public class FieldAcess : LvalueExpression
	{
		public LvalueExpression obj;
		public Identifier field;

		public FieldAcess(LvalueExpression obj, Identifier field)
		{
			this.obj = obj;
			this.field = field;
		}
	}

	/// <summary>
	/// TO BE resolved into one of Id, FuncCall, or Typecast after creation 
	/// </summary>
	public class Identifier : LvalueExpression
	{
		public string name;

		public Identifier(string val)
		{
			this.name = val;
		}
	}

	#endregion

}