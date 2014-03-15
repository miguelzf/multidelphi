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
		public ConstantValue Value { get; set; }

		/// <summary>
		/// Indicates if expression must be constant
		/// </summary>
		public bool EnforceConst { get; set; }

		/// <summary>
		/// Type that must be enforced/checked
		/// </summary>
		public TypeNode ForcedType { get; set; }


		/// <summary>
		/// Downcast the ConstantValue to an IntegralType, and return the content
		/// The type of the expression must be checked first to ensure
		/// </summary>
		public ulong GetIntegralValue()
		{
			if (!Type.ISA(typeof(IntegralType)))
			{
				ErrorInternal("Attempt to downcast ConstantValue to Integral Value, real type is " + Type);
				return 0;
			}

			return ((IntegralValue) Value).val;
		}


		public bool Equals(Expression obj)
		{
			return obj != null && Value.Equals(obj) && Type.Equals(obj);
		}
	}

	public class EmptyExpression : Expression
	{
		// Do nothing
	}


	public class ExpressionList : Expression, IListNode<Expression>
	{
		List<Expression> nodes = new List<Expression>();

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

		public virtual void Add(IEnumerable<Expression> t)
		{
			nodes.AddRange(t);
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

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
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

	public class SetRange : BinaryExpression
	{
		public SetRange(RangeType type)
		{
			this.ForcedType = this.Type = type;
			this.EnforceConst = true;
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
		public TypeCast(TypeNode t,  Expression e) : base(e, t) { }
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

	public abstract class SimpleUnaryExpression : Expression
	{
		public Expression expr;

		public SimpleUnaryExpression() { }

		public SimpleUnaryExpression(Expression expr)
		{
			this.expr = expr;
		}
	}

	public class UnaryPlus : SimpleUnaryExpression
	{
		public UnaryPlus(Expression expr) : base(expr) { }
	}

	public class UnaryMinus : SimpleUnaryExpression
	{
		public UnaryMinus(Expression expr) : base(expr) { }
	}

	public class LogicalNot : SimpleUnaryExpression
	{
		public LogicalNot(Expression expr) : base(expr) { }
	}

	public class AddressLvalue : SimpleUnaryExpression
	{
		public AddressLvalue(Expression expr) : base(expr) { }
	}

	public class Set : UnaryExpression
	{
		public ExpressionList setelems;

		public Set()
		{
			setelems = new ExpressionList();
		}

		public Set(ExpressionList elems)
		{
			setelems = elems;
		}
	}

	#endregion

	#region Compile-time computed values
	#pragma warning disable 659

	public abstract class ConstantValue : Node
	{
	}

	// For ints, chars and bools
	public class IntegralValue : ConstantValue
	{
		public ulong val;
		public IntegralValue(ulong val) { this.val = val; }

		public override bool Equals(object obj)
		{
			if (obj == null || obj.GetType() != this.GetType())
				return false;

			return val == ((IntegralValue)obj).val;
		}

/*		public override ulong range(OrdinalLiteral l2)
		{
			if (base.range(l2) == 0)
				return 0;

			long val1 = (long)Value;
			if (l2.GetType() == typeof(SIntLiteral))
			{
				long val2 = (long) l2.Value;
				if (val2 < val1)
				{	Error("Invalid range: upper limit less than lower");
					return 0;
				}

				if ((val1 < 0) == (val2 < 0))	// same sign
					return (ulong) (val2 - val1);
				else	// diff sign
					return (ulong)val1 + (ulong)val2;
			}
			else	// l1 == UIntLiteral
			{
				ulong val2 = (ulong)l2.Value;
				if (val1 < 0)	// negative
				{
					return val2. + (ulong)val1;
				}
				else	// positive
				if (val2 < (ulong) val1)
				{	Error("Invalid range: upper limit less than lower");
					return 0;
				}

			}
		}
		protected override ulong range(OrdinalLiteral l1)
		{
			if (!l1.ISA(typeof(IntLiteral)))
			{
				ErrorInternal("Int Literal range with non integer type");
				return 0;
			}
			return 1;
		}
		// range(this... other)
		public abstract ulong range(OrdinalLiteral l1);

		public override ulong range(OrdinalLiteral l1)
		{
			if (base.range(l1) == 0)
				return 0;

			if ((long)Value - (long)l1.Value)

			return Math.Abs((long)Value - (long)l1.Value);
		}
*/
	}

	public class StringValue : ConstantValue
	{
		public string val;
		public StringValue(string val) { this.val = val; }

		public override bool Equals(object obj)
		{
			if (obj == null || obj.GetType() != this.GetType())
				return false;

			return val == ((StringValue)obj).val;
		}
	}

	public class RealValue : ConstantValue
	{
		public double val;
		public RealValue(double val) { this.val = val; }

		public override bool Equals(object obj)
		{
			if (obj == null || obj.GetType() != this.GetType())
				return false;

			return val == ((RealValue)obj).val;
		}
	}

	#pragma warning restore 659
	#endregion


	#region Literals
	//==========================================================================
	// Literals
	//==========================================================================

	public abstract class Literal : UnaryExpression
	{
		public Literal() { }

		public Literal(ConstantValue val, ScalarType t)
		{
			this.Type = this.ForcedType = t;
			this.Value = val;
			this.IsConst = this.EnforceConst = true;
		}
	}

	public abstract class OrdinalLiteral : Literal
	{
		public OrdinalLiteral() { }

		public OrdinalLiteral(ulong v, IntegralType t) : base(new IntegralValue(v),t) { }
	}

	public class IntLiteral : OrdinalLiteral
	{
		public IntLiteral(ulong val)	: base((ulong) val, IntegerType.Single) { }
	}

	public class CharLiteral : OrdinalLiteral
	{
		public CharLiteral(char value)	: base((ulong)value, CharType.Single) { }
	}

	public class BoolLiteral : OrdinalLiteral
	{
		public BoolLiteral(bool value)	: base(Convert.ToUInt64(value), BoolType.Single) { }
	}

	public class StringLiteral : Literal
	{
		public bool isChar;

		public StringLiteral(string value)
			: base(new StringValue(value), StringType.Single)
		{
			if (value.Length == 1)
				isChar = true;
		}
	}

	public class RealLiteral : Literal
	{
		public RealLiteral(double value) : base(new RealValue(value),RealType.Single) { }
	}

	public class PointerLiteral : Literal
	{
		public PointerLiteral(ulong value)
			: base(new IntegralValue(value), PointerType.Single)
		{
			if (value != 0x0)	// Const_NIl
				Error("Pointer constant can only be nil");
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
		// object alternative to lvalue
		public ArrayConst array;

		public ArrayAccess(ArrayConst array, ExpressionList acessors)
		{
			this.array = array;
			this.acessors = acessors;
		}

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
		public LvalueExpression func;
		public ExpressionList args;
		public ScalarType basictype;

		public RoutineCall(LvalueExpression func)
		{
			this.func = func;
			args = new ExpressionList();
		}

		public RoutineCall(LvalueExpression fname, ExpressionList args)
			: this(fname)
		{
			this.args = args;
		}

		public RoutineCall(LvalueExpression fname, ScalarType t)
			: this(fname)
		{
			basictype = t;
		}
	}

	public class FieldAcess : LvalueExpression
	{
		public LvalueExpression obj;
		public string field;

		public FieldAcess(LvalueExpression obj, string field)
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