using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MultiDelphi.AST.Nodes;
using MultiDelphi.AST;
using MiscUtil;

namespace MultiDelphi.Semantics
{

	/// <summary>
	/// Compile-time folding of constant values.
	/// The computed values are stored in Expression.Value, currently *no* expression nodes are replaced here.
	/// </summary>
	class ConstantFolder : Processor<bool>
	{
		//
		// Assumes that the values have been previously type-checked and are valid
		//

		public override bool DefaultReturnValue()
		{
			return true;
		}

	/*
		private static bool isReal(double x)
		{
			double frac = x - Math.Floor(x);
			return (frac != 0.0);
		}
	*/

		//
		// Processor Interface
		//

		public override bool Visit(ExprAsLvalue node)
		{
			traverse(node.expr);
			if (node.expr.Value != null)
				node.Value = node.expr.Value;
			return true;
		}

		public override bool Visit(Set node)
		{
			return traverse(node.setelems);
		}

		public override bool Visit(AddressLvalue node)
		{
			return traverse(node.expr);
		}

		public override bool Visit(LogicalNot node)
		{
			traverse(node.expr);
			if (node.expr.Value != null)
			{
				node.Value = (ConstantValue) node.expr.Value.Clone();
				node.Value.SetVal<bool>(!node.Value.Val<bool>());
			}
			return true;
		}

		public override bool Visit(UnaryMinus node)
		{
			traverse(node.expr);
			if (node.expr.Value != null)
			{
				node.Value = (ConstantValue)node.expr.Value.Clone();
				node.Value.SetVal<long>(- node.Value.Val<long>());
			}
			return true;
		}

		public override bool Visit(UnaryPlus node)
		{
			traverse(node.expr);
			if (node.expr.Value != null)
				node.Value = node.expr.Value;
			return true;
		}



		#region ArithmeticBinaryExpression

		T Fold<T>(ArithmeticBinaryExpression n) where T : struct
		{
			var vleft  = n.left .Value.Val<T>();
			var vright = n.right.Value.Val<T>();

			switch (n.op)
			{
				case ArithmeticBinaryOp.ADD: return Operator.Add(vleft, vright);
				case ArithmeticBinaryOp.SUB: return Operator.Subtract(vleft, vright);
				case ArithmeticBinaryOp.MUL: return Operator.Multiply(vleft, vright);
				case ArithmeticBinaryOp.QUOT:return Operator.Add(vleft, vright);
				case ArithmeticBinaryOp.DIV: return Operator.Divide(vleft, vright);
				case ArithmeticBinaryOp.MOD: return Operator.DivideInt32(vleft, n.right.Value.Val<int>());
				
				// TODO
			//	case ArithmeticBinaryOp.SHL: return Operator
			//	case ArithmeticBinaryOp.SHR: return Operator
				default:	throw new SemanticException("Constant folding of operator not yet supported");
			}
		}

		public override bool Visit(ArithmeticBinaryExpression node)
		{
			Visit(node.left);
			Visit(node.right);
			
			if (node.left.Value == null || node.right == null)
				return false;

			if (node.Type is IntegerType)
				node.Value = new IntegralValue(Fold<long>(node));
			if (node.Type is CharType)
				node.Value = new IntegralValue(Fold<char>(node));
			if (node.Type is BoolType)
				node.Value = new IntegralValue(Fold<bool>(node));
			else if (node.Type is RealType)
				node.Value = new RealValue(Fold<double>(node));

			// TODO strings

			return true;
		}

		#endregion


		#region Boolean expressions

		bool Fold(LogicalBinaryExpression n)
		{
			bool vleft = n.left.Value.Val<bool>();
			bool vright = n.right.Value.Val<bool>();

			switch (n.op)
			{
				case LogicalBinaryOp.AND:
					return Operator.And(vleft, vright);
				case LogicalBinaryOp.OR:
					return Operator.Or(vleft, vright);
				case LogicalBinaryOp.XOR:
					return Operator.Xor(vleft, vright);
				default:
					throw new SemanticException("Invalid operator " + n.op + " in logical binary expr");
			}
		}

		bool Fold<T>(ComparisonBinaryExpression n) where T : struct
		{
			T vleft = n.left.Value.Val<T>();
			T vright = n.right.Value.Val<T>();

			switch (n.op)
			{
				case ComparisonBinaryOp.EQ:
					return Operator.Equal(vleft, vright);
				case ComparisonBinaryOp.GE:
					return Operator.GreaterThanOrEqual(vleft, vright);
				case ComparisonBinaryOp.GT:
					return Operator.GreaterThan(vleft, vright);
				case ComparisonBinaryOp.LE:
					return Operator.LessThanOrEqual(vleft, vright);
				case ComparisonBinaryOp.LT:
					return Operator.LessThan(vleft, vright);
				case ComparisonBinaryOp.NE:
					return Operator.NotEqual(vleft, vright);

				// TODO
				case ComparisonBinaryOp.SGE:
					return Operator.GreaterThanOrEqual(vleft, vright);
				case ComparisonBinaryOp.SGT:
					return Operator.GreaterThan(vleft, vright);
				case ComparisonBinaryOp.SLE:
					return Operator.LessThanOrEqual(vleft, vright);
				case ComparisonBinaryOp.SLT:
					return Operator.LessThan(vleft, vright);

				default:
					throw new SemanticException("Invalid operator " + n.op + " in comparison binary expr");
			}
		}

		public override bool Visit(LogicalBinaryExpression node)
		{
			Visit(node.left);
			Visit(node.right);

			if (node.left.Value == null || node.right == null)
				return false;

			node.Value = new IntegralValue(Fold(node));
			return true;
		}

		public override bool Visit(ComparisonBinaryExpression node)
		{
			Visit(node.left);
			Visit(node.right);

			if (node.left.Value == null || node.right == null)
				return false;

			// resulting value is always bool
			if (node.Type is IntegerType)
				node.Value = new IntegralValue(Fold<long>(node));
			if (node.Type is CharType)
				node.Value = new IntegralValue(Fold<char>(node));
			if (node.Type is BoolType)
				node.Value = new IntegralValue(Fold<bool>(node));
			else if (node.Type is RealType)
				node.Value = new IntegralValue(Fold<double>(node));
			else if (node.Type is RealType)
				node.Value = new IntegralValue(Fold<double>(node));

			return true;
		}

		public override bool Visit(TypeBinaryExpression node)
		{
			Visit(node.expr);
			Visit(node.types);
			return true;
		}

		#endregion

	}

}
