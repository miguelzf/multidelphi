using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace crosspascal.ast.nodes
{

	//==========================================================================
	// Expressions' base classes
	//==========================================================================

	public abstract class Expression : Node
	{
		/// <summary>
		/// Expression type, to be checked or determined in static type-validation
		/// </summary>
		public TypeNode type { get; set; }

		/// <summary>
		/// Indicates if expression is be constant
		/// </summary>
		public bool isConst { get; set; }

		/// <summary>
		/// Indicates if expression must be constant
		/// </summary>
		public bool enforceConst { get; set; }

		/// <summary>
		/// Type that must be enforced/checked
		/// </summary>
		public TypeNode forcedType { get; set; }
	}

	public class EmptyExpression : Expression
	{
		// Do nothing
	}


	public abstract class UnaryExpression : Expression
	{

	}

	public abstract class BinaryExpression : Expression
	{
	}

	public abstract class LvalueExpression : UnaryExpression
	{

	}

	public abstract class Literal<T> : UnaryExpression
	{
		public T value;

		public Literal() { }

		public Literal(T t)
		{
			value = t;
		}
	}


	//==========================================================================
	// Binary Expressions
	//==========================================================================

	#region Binary Expressions

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

	public class SetIn : BinaryExpression
	{
		Expression expr;
		Expression set;		// enforce that 'set' is in fact a set

		public SetIn(Expression e1, Expression e2)
		{
			expr = e1;
			set  = e2;
		}
	}

	public abstract class TypeBinaryExpression : BinaryExpression
	{
		Expression expr;
		TypeNode types;

		public TypeBinaryExpression(Expression e1, TypeNode e2)
		{
			expr = e1;
			type = e2;
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


	#region Literals

	//==========================================================================
	// Literals
	//==========================================================================


	public class IntLiteral : Literal<Int32>
	{
		public IntLiteral(Int32 value) : base(value) { }
	}

	public class CharLiteral : Literal<Char>
	{
		public CharLiteral(Char value) : base(value) { }
	}

	public class StringLiteral : Literal<String>
	{
		public bool isChar;

		public StringLiteral(String value) : base(value)
		{
			if (value.Length == 1)
				isChar = true;
		}
	}

	public class BoolLiteral : Literal<Boolean>
	{
		public BoolLiteral(Boolean value) : base(value) { }
	}

	public class RealLiteral : Literal<Double>
	{
		public RealLiteral(Double value) : base(value) { }
	}

	public class PointerLiteral : Literal<uint>
	{
		public PointerLiteral(uint value)
		{
			if (value != 0x0)	// Const_NIl
				Error("Pointer constant can only be nil");
			this.value = value;
		}
	}

	#endregion

	#region Unary Expressions

	//==========================================================================
	// Unary Expressions
	//==========================================================================


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

	#endregion

	#region L-values

	//==========================================================================
	// Lvalue Expressions
	//==========================================================================

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


	public class RoutineCall : LvalueExpression
	{
		public Identifier fname;
		public ExpressionList args;
		public bool inherited;

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


	public class Set : UnaryExpression
	{
		public Expression setelems;

		public Set(Expression elems)
		{
			setelems = elems;
		}
	}

}