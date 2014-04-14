using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiPascal.AST.Nodes
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
	///					UnresolvedCastorCall
	///					UnresolvedIdOrCall
	///					UnresolvedAccess
	///			BinaryExpr
	///				Arithmetic
	///				Type
	///				Logical
	/// </remarks>
	#endregion

	#pragma warning disable 659

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

		public virtual bool IsEmpty { get { return false; } }

		/// <summary>
		/// Downcast the ConstantValue to an IntegralType, and return the content.
		/// The type of the expression must be checked first to ensure
		/// </summary>
		public ulong ValueIntegral()
		{
			if (!(Type is IntegralType))
			{
				ErrorInternal("Attempt to downcast ConstantValue to Integral Value, real type is " + Type);
				return 0;
			}

			return (Value as IntegralValue).val;
		}


		/// <summary>
		/// Compare constants
		/// </summary>
		public override bool Equals(object obj)
		{
			if (!(obj is Expression))
				return false;

			var ot = (obj as Expression);
			if (!Type.Equals(ot.Type))
				return false;

			return Value.Equals(ot.Value);
		}
	}

	public class EmptyExpression : Expression
	{
		public override bool IsEmpty { get { return true; } }
	}


	#region Constant and initiliazer expressions
	//==========================================================================
	// Constants and Initializers
	//==========================================================================

	/// <summary>
	/// Constant expressions
	/// Should be used for all initializations
	/// </summary>
	public abstract class ConstExpression : Expression
	{
		public ConstExpression()
		{
			this.EnforceConst = true;
		}

		/// <summary>
		/// Compare constants
		/// </summary>
		public override bool Equals(object obj)
		{
			if (!(obj is ConstExpression))
				return false;

			var ot  = (obj as ConstExpression);
			if (!Type.Equals(ot.Type))
				return false;

			return Value.Equals(ot.Value);
		}
	}

	public abstract class StructuredConstant : ConstExpression
	{
		public ExpressionList exprlist;

		public StructuredConstant(ExpressionList exprlist)
		{
			this.exprlist = exprlist;
		}
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

	public class FieldInit : ConstExpression
	{
		public String fieldname;
		public Expression expr;

		public FieldInit(string name, Expression expr)
		{
			fieldname = name;
			this.expr = expr;
		}
	}

	public class ConstIdentifier : ConstExpression
	{		
		public string name;

		public ConstIdentifier(string val)
		{
			this.name = val;
		}
	}

	#endregion


	#region Literals
	//==========================================================================
	// Literals
	//==========================================================================

	public abstract class Literal : ConstExpression
	{
		public Literal() { }

		public Literal(ConstantValue val, ScalarType t)
		{
			this.Type = this.ForcedType = t;
			this.Value = val;
			this.IsConst = true;
		}

		public abstract ConstantValue Default();
	}

	public abstract class OrdinalLiteral : Literal
	{
		public OrdinalLiteral() { }

		public OrdinalLiteral(ulong v, IntegralType t) : base(new IntegralValue(v), t) { }

		public override ConstantValue Default()
		{
			return new IntegralValue(0);
		}
	}

	public class IntLiteral : OrdinalLiteral
	{
		public IntLiteral(ulong val) : base((ulong)val, IntegerType.Single) { }
	}

	public class CharLiteral : OrdinalLiteral
	{
		public CharLiteral(char value) : base((ulong)value, CharType.Single) { }
	}

	public class BoolLiteral : OrdinalLiteral
	{
		public BoolLiteral(bool value) : base(Convert.ToUInt64(value), BoolType.Single) { }
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

		public override ConstantValue Default()
		{
			return new StringValue("");
		}
	}

	public class RealLiteral : Literal
	{
		public RealLiteral(double value) : base(new RealValue(value), RealType.Single) { }

		public override ConstantValue Default()
		{
			return new RealValue(0.0);
		}
	}

	public class PointerLiteral : Literal
	{
		public PointerLiteral(ulong value)
			: base(new IntegralValue(value), PointerType.Single)
		{
			if (value != 0x0)	// Const_NIl
				Error("Pointer constant can only be nil");
		}

		public override ConstantValue Default()
		{
			return new IntegralValue(0);
		}
	}

	#endregion


	#region Compile-time computed values

	public abstract class ConstantValue : Node
	{
		public T Val<T>()
		{
			return (T) Val();
		}

		public abstract Object Val();
	}

	// For ints, chars and bools
	public class IntegralValue : ConstantValue
	{
		public ulong val;

		public IntegralValue(ulong val) { this.val = val; }

		public override Object Val() { return val; }

		public override bool Equals(object obj)
		{
			return (obj is IntegralValue) && val == ((IntegralValue)obj).val;
		}

		// Assumes the values belong same-type constants
		// this < other
		public ulong range(IntegralValue l2)
		{
			ulong range = l2.val - this.val;

			if (range > l2.val)
				ErrorInternal("IntegralValue negative range: " + l2.val + " - " + this.val);

			return range;
		}

		/*			long val1 = (long)Value;
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
					if (reftype.range(l1) == 0)
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

		public override Object Val() { return val; }

		public override bool Equals(object obj)
		{
			return (obj is StringValue) && val == ((StringValue)obj).val;
		}
	}

	public class RealValue : ConstantValue
	{
		public double val;
		public RealValue(double val) { this.val = val; }

		public override Object Val() { return val; }

		public override bool Equals(object obj)
		{
			return (obj is RealValue) && val == ((RealValue)obj).val;
		}
	}

	#pragma warning restore 659
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
		public Expression expr;
		public Expression set;		// enforce that 'set' is in fact a set

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

	public enum ArithmeticBinaryOp
	{
		ADD, SUB,
		DIV, MUL,
		QUOT /* integer quotient */, MOD,
		SHR, SHL,
	}

	public abstract class ArithmethicBinaryExpression : BinaryExpression
	{
		public Expression left;
		public Expression right;
		public ArithmeticBinaryOp op;

		public ArithmethicBinaryExpression(Expression e1, Expression e2)
		{
			left = e1;
			right= e2;
		}
	}

	public class Subtraction : ArithmethicBinaryExpression
	{
		public Subtraction	(Expression e1, Expression e2) : base(e1, e2) { this.op = ArithmeticBinaryOp.SUB; }
	}

	public class Addition : ArithmethicBinaryExpression
	{
		public Addition		(Expression e1, Expression e2) : base(e1, e2) { this.op = ArithmeticBinaryOp.ADD; }
	}

	public class Product : ArithmethicBinaryExpression
	{
		public Product		(Expression e1, Expression e2) : base(e1, e2) { this.op = ArithmeticBinaryOp.MUL; }
	}

	public class Division: ArithmethicBinaryExpression
	{
		public Division		(Expression e1, Expression e2) : base(e1, e2) { this.op = ArithmeticBinaryOp.DIV; }
	}

	public class Quotient: ArithmethicBinaryExpression
	{
		public Quotient		(Expression e1, Expression e2) : base(e1, e2) { this.op = ArithmeticBinaryOp.QUOT; }
	}

	public class Modulus: ArithmethicBinaryExpression
	{
		public Modulus		(Expression e1, Expression e2) : base(e1, e2) { this.op = ArithmeticBinaryOp.MOD; }
	}

	public class ShiftRight: ArithmethicBinaryExpression
	{
		public ShiftRight	(Expression e1, Expression e2) : base(e1, e2) { this.op = ArithmeticBinaryOp.SHR; }
	}

	public class ShiftLeft: ArithmethicBinaryExpression
	{
		public ShiftLeft	(Expression e1, Expression e2) : base(e1, e2) { this.op = ArithmeticBinaryOp.SHL; }
	}
	#endregion


	#region Logical Binary Expressions

	public enum LogicalBinaryOp
	{
		AND,
		OR,
		XOR,
		// int values match LLVM's constants
		EQ = 32,
		NE,
			// unsigned compare
		LT,
		LE,
		GT,
		GE,
			// signed compare
		SGT,
		SGE,
		SLT,
		SLE,
	}

	public abstract class LogicalBinaryExpression : BinaryExpression
	{
		public Expression left;
		public Expression right;
		public LogicalBinaryOp op;

		public LogicalBinaryExpression(Expression e1, Expression e2)
		{
			left = e1;
			right= e2;
		}
	}

	public class LogicalAnd : LogicalBinaryExpression
	{
		public LogicalAnd	(Expression e1, Expression e2) : base(e1, e2) { this.op = LogicalBinaryOp.AND; }
	}

	public class LogicalOr : LogicalBinaryExpression
	{
		public LogicalOr	(Expression e1, Expression e2) : base(e1, e2) { this.op = LogicalBinaryOp.OR; }
	}

	public class LogicalXor : LogicalBinaryExpression
	{
		public LogicalXor	(Expression e1, Expression e2) : base(e1, e2) { this.op = LogicalBinaryOp.XOR; }
	}

	public class Equal : LogicalBinaryExpression
	{
		public Equal		(Expression e1, Expression e2) : base(e1, e2) { this.op = LogicalBinaryOp.EQ; }
	}

	public class NotEqual : LogicalBinaryExpression
	{
		public NotEqual		(Expression e1, Expression e2) : base(e1, e2) { this.op = LogicalBinaryOp.NE; }
	}

	public class LessThan : LogicalBinaryExpression
	{
		public LessThan		(Expression e1, Expression e2) : base(e1, e2) { this.op = LogicalBinaryOp.LT; }
	}

	public class LessOrEqual : LogicalBinaryExpression
	{
		public LessOrEqual	(Expression e1, Expression e2) : base(e1, e2) { this.op = LogicalBinaryOp.LE; }
	}

	public class GreaterThan : LogicalBinaryExpression
	{
		public GreaterThan	(Expression e1, Expression e2) : base(e1, e2) { this.op = LogicalBinaryOp.GT; }
	}

	public class GreaterOrEqual : LogicalBinaryExpression
	{
		public GreaterOrEqual(Expression e1, Expression e2) : base(e1, e2) { this.op = LogicalBinaryOp.GE; }
	}

	#endregion


	#region Typing Binary Expressions

	public abstract class TypeBinaryExpression : BinaryExpression
	{
		public Expression expr;
		public TypeNode types;

		public TypeBinaryExpression(Expression e1, TypeNode e2)
		{
			expr = e1;
			Type = e2;
		}
	}

	/// <summary>
	/// 'Expr IS CompositeType'
	/// </summary>
	public class TypeIs : TypeBinaryExpression
	{
		public TypeIs(Expression e1, ClassType e2) : base(e1, e2) { }
	}

	/// <summary>
	/// 'Expr AS CompositeType'
	/// </summary>
	public class RuntimeCast : TypeBinaryExpression
	{
		public RuntimeCast(Expression e, ClassType t) : base(e, t) { }
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


	#region Left-value expressions
	//==========================================================================
	// Lvalue Expressions
	//==========================================================================

	/// <summary>
	/// Cast an lvalue to an rvalue (Expr) 
	/// </summary>
	public class LvalueAsExpr : UnaryExpression
	{
		public LvalueExpression lval;

		public LvalueAsExpr(LvalueExpression lval)
		{
			this.lval = lval;
		}
	}


	public abstract class LvalueExpression : UnaryExpression
	{
	}

	/// <summary>
	/// Cast an rvalue (Expr) to an lvalue
	/// </summary>
	public class ExprAsLvalue : LvalueExpression
	{
		public Expression expr;

		public ExprAsLvalue(Expression expr)
		{
			this.expr = expr;
		}
	}

	/// <summary>
	/// VarType(expr)
	/// </summary>
	public class StaticCast : LvalueExpression
	{
		public TypeNode casttype;
		public Expression expr;

		public StaticCast(TypeNode t, Expression e)
		{
			this.casttype = t;
			this.expr = e;
		}
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

	public class InheritedCall : RoutineCall
	{
		public String funcname;

		// to be set by resolver
		public CompositeType declaringObject;

		public InheritedCall(String funcname, ExpressionList args = null)
			: base(null, args)
		{
			this.funcname = funcname;
		}
	}

	public class RoutineCall : LvalueExpression
	{
		public LvalueExpression func;
		public ExpressionList args;

		public RoutineCall(LvalueExpression func, TypeNode retType = null)
		{
			this.func = func;
			args = new ExpressionList();
			this.Type = retType;
		}

		public RoutineCall(LvalueExpression func, ExpressionList args, TypeNode retType = null)
			: this(func, retType)
		{
			this.args = args;
			if (this.args == null)
				this.args = new ExpressionList();
		}
	}

	/// <summary>
	/// An access to a member in an object (record, class or interface)
	/// </summary>
	public class ObjectAccess : LvalueExpression
	{
		public LvalueExpression obj;
		public string field;

		public ObjectAccess(LvalueExpression obj, string field)
		{
			this.obj = obj;
			this.field = field;
		}
	}

	/// <summary>
	/// Identifier that refers to a named declaration
	/// </summary>
	public class Identifier : LvalueExpression
	{
		public string name;

		/// <summary>
		/// Declarationn refered to by this identifier. To be set by resolver
		/// </summary>
		public Declaration decl
		{
			get { return _decl; }
			set { _decl = value; Type = value.type; }
		}
		Declaration _decl;

		public Identifier(string val, TypeNode t = null)
		{
			this.name = val;
			Type = t;
		}
	}

	/// <summary>
	/// Identifier that refers to a named class. Static access
	/// </summary>
	public class IdentifierStatic : Identifier
	{
		public IdentifierStatic(string val, CompositeType t) 
			: base(val)
		{
			Type = t;
		}

		public IdentifierStatic(CompositeType t)
			: base(t.Name)
		{
			Type = t;
		}
	}



	#region Unresolved Lvalue expressions

	/// <summary>
	/// Base class of unresolved lvalues
	/// </summary>
	public abstract class UnresolvedLvalue : LvalueExpression
	{
	}

	/// <summary>
	/// Identifier, to be resolver after parsing
	/// </summary>
	public class UnresolvedId : UnresolvedLvalue
	{
		public Identifier id;

		public UnresolvedId(Identifier val)
		{
			id = val;
		}
	}

	/// <summary>
	/// Call, to be resolver after parsing
	/// </summary>
	public class UnresolvedCall : UnresolvedLvalue
	{
		public LvalueExpression func;
		public ExpressionList args;

		public UnresolvedCall(LvalueExpression lval, ExpressionList args = null)
		{
			this.func = lval;
			this.args = args;
			if (args == null)
				this.args = new ExpressionList();
		}
	}

	#endregion


	#endregion		// lvalues

}