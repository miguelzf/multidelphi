using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using crosspascal.ast.nodes;
using crosspascal.ast;

namespace crosspascal.semantics
{
	class ConstantFolder : Processor<bool>
	{

		public override bool DefaultReturnValue()
		{
			return true;
		}

		private static bool isReal(double x)
		{
			double frac = x - Math.Floor(x);
			return (frac != 0.0);
		}

		private static bool isNumberLiteral(Expression expr)
		{ 
			if (expr is RealLiteral || expr is OrdinalLiteral)
				return true;

			if (expr is SimpleUnaryExpression)
				return isNumberLiteral((expr as SimpleUnaryExpression).expr);

			if (expr is ExprAsLvalue)
				return isNumberLiteral((expr as ExprAsLvalue).expr);

			return false;
		}

		private static double getNumberValue(Expression expr)
		{
			if (expr is OrdinalLiteral)
				return ((expr as OrdinalLiteral).Value as IntegralValue).val;

			if (expr is RealLiteral)
				return ((expr as RealLiteral).Value as RealValue).val;
			
			if (expr is UnaryPlus)
				return getNumberValue((expr as UnaryPlus).expr);

			if (expr is UnaryMinus)
				return -getNumberValue((expr as UnaryMinus).expr);

			if (expr is ExprAsLvalue)
				return getNumberValue((expr as ExprAsLvalue).expr);

			return 0.0;
		}

		public static Expression FoldExpression(Expression expr)
		{
			if (expr is ExprAsLvalue)
			{
				return new ExprAsLvalue(FoldExpression( (expr as ExprAsLvalue).expr));
			}
			else
			if (expr is LogicalBinaryExpression)
			{
				LogicalBinaryExpression bin = (expr as LogicalBinaryExpression);

				bin.left = FoldExpression(bin.left);
				bin.right = FoldExpression(bin.right);

				if (!(bin.left is Literal))
					return expr;
				if (!(bin.right is Literal))
					return expr;

				if (bin.left is BoolLiteral && bin.right is BoolLiteral)
				{
					bool a = ((bin.left as BoolLiteral).Value as IntegralValue).val != 0;
					bool b = ((bin.right as BoolLiteral).Value as IntegralValue).val != 0;
					if (bin is LogicalAnd)
					{
						return new BoolLiteral(a && b);
					}
					else
					if (bin is LogicalOr)
					{
						return new BoolLiteral(a || b);
					}
					else
					if (bin is LogicalXor)
					{
						return new BoolLiteral(a ^ b);
					}
					else
						return expr;
				}
				else
					return expr;
			}
			else
			if (expr is ArithmethicBinaryExpression)
			{
				ArithmethicBinaryExpression bin = (expr as ArithmethicBinaryExpression);
				bin.left = FoldExpression(bin.left);
				bin.right = FoldExpression(bin.right);
				
				if (bin.left is StringLiteral && bin.right is StringLiteral)
				{
					if (bin is Addition)
					{
						string a = (bin.left.Value as StringValue).val;
						string b = (bin.right.Value as StringValue).val;
						return new StringLiteral(a + b);
					}
					else
						return expr;
				}
				else
				if (isNumberLiteral(bin.left) && isNumberLiteral(bin.right))
				{
					double a, b;

					a = getNumberValue(bin.left);
					b = getNumberValue(bin.right);

					double result;

					if (bin is Addition)
					{
						result = a + b;
					}
					else
						if (bin is Subtraction)
						{
							result = a - b;
						}
						else
							if (bin is Product)
							{
								result = a * b;
							}
							else
								if (bin is Division)
								{
									result = (double)a / (double)b;
								}
								else
									if (bin is Quotient)
									{
										result = a / b;
									}
									else
										if (bin is Modulus)
										{
											result = a % b;
										}
										else
										{
											return expr;
										}

					Expression res;
					if (isReal(result) || (bin.left is RealLiteral) || (bin.right is RealLiteral))
					{
						res = new RealLiteral(Math.Abs(result));
					}
					else
					{
						res = new IntLiteral((ulong) Math.Abs(result));
					}

					if (result < 0)
						return new UnaryMinus(res);
					else
						return res;
				}
				else
					return expr;
			}
			else
				return expr;
		}


		private void fold(ref Expression expr)
		{
			Expression temp = expr;
			expr = FoldExpression(expr);
			if (temp != expr)
				Console.WriteLine("Folded expression: " + temp.GetType().Name + " into " + expr.GetType().Name);			
		}

/*
		public override bool Visit(VarDeclaration node)
		{
			fold(ref node.init);
			return true;
		}
		
		public override bool Visit(ConstDeclaration node)
		{
			fold(ref node.init);
			return true;
		}
		
		public override bool Visit(ExpressionList node)
		{
			foreach (Node n in node.nodes)
			{
				// FIX ME				
			}

			return true;
		}

		public override bool Visit(ConstExpression node)
		{
			fold(ref node.expr);
			return true;
		}

		public override bool Visit(Assignement node)
		{
			fold(ref node.expr);
			return true;
		}

		public override bool Visit(BinaryExpression node)
		{
			Visit((Expression) node);
			return true;
		}

		public override bool Visit(ArithmethicBinaryExpression node)
		{
			Visit((BinaryExpression) node);
			//traverse(node.left);
			//traverse(node.right);
			return true;
		}

		public override bool Visit(Subtraction node)
		{
			fold(ref node.left);
			fold(ref node.right);
			//Visit((ArithmethicBinaryExpression) node);
			return true;
		}

		public override bool Visit(Addition node)
		{
			fold(ref node.left);
			fold(ref node.right);
			//Visit((ArithmethicBinaryExpression) node);
			return true;
		}

		public override bool Visit(Product node)
		{
			fold(ref node.left);
			fold(ref node.right);
			//Visit((ArithmethicBinaryExpression) node);
			return true;
		}

		public override bool Visit(Division node)
		{
			fold(ref node.left);
			fold(ref node.right);
			//Visit((ArithmethicBinaryExpression) node);
			return true;
		}

		public override bool Visit(Quotient node)
		{
			fold(ref node.left);
			fold(ref node.right);
			//Visit((ArithmethicBinaryExpression) node);
			return true;
		}

		public override bool Visit(Modulus node)
		{
			fold(ref node.left);
			fold(ref node.right);
			//Visit((ArithmethicBinaryExpression) node);
			return true;
		}

		public override bool Visit(ShiftRight node)
		{
			fold(ref node.left);
			fold(ref node.right);
			//Visit((ArithmethicBinaryExpression) node);
			return true;
		}

		public override bool Visit(ShiftLeft node)
		{
			fold(ref node.left);
			fold(ref node.right);
			//Visit((ArithmethicBinaryExpression) node);
			return true;
		}

		public override bool Visit(LogicalBinaryExpression node)
		{
			Visit((BinaryExpression) node);
			return true;
		}

		public override bool Visit(LogicalAnd node)
		{
			fold(ref node.left);
			fold(ref node.right);
			//Visit((LogicalBinaryExpression) node);
			return true;
		}

		public override bool Visit(LogicalOr node)
		{
			fold(ref node.left);
			fold(ref node.right);
			//Visit((LogicalBinaryExpression) node);
			return true;
		}

		public override bool Visit(LogicalXor node)
		{
			fold(ref node.left);
			fold(ref node.right);
			//Visit((LogicalBinaryExpression) node);
			return true;
		}

		public override bool Visit(Equal node)
		{
			fold(ref node.left);
			fold(ref node.right);
			//Visit((LogicalBinaryExpression) node);
			return true;
		}

		public override bool Visit(NotEqual node)
		{
			fold(ref node.left);
			fold(ref node.right);
			//Visit((LogicalBinaryExpression) node);
			return true;
		}

		public override bool Visit(LessThan node)
		{
			fold(ref node.left);
			fold(ref node.right);
			//Visit((LogicalBinaryExpression) node);
			return true;
		}

		public override bool Visit(LessOrEqual node)
		{
			fold(ref node.left);
			fold(ref node.right);
			//Visit((LogicalBinaryExpression) node);
			return true;
		}


		public override bool Visit(UnaryExpression node)
		{
			Visit((Expression) node);
			return true;
		}

		public override bool Visit(SimpleUnaryExpression node)
		{
			Visit((Expression) node);
			traverse(node.expr);
			return true;
		}

		public override bool Visit(UnaryPlus node)
		{
			Visit((SimpleUnaryExpression) node);
			fold(ref node.expr);
			return true;
		}

		public override bool Visit(UnaryMinus node)
		{
			Visit((SimpleUnaryExpression) node);
			fold(ref node.expr);
			return true;
		}

		public override bool Visit(LogicalNot node)
		{
			Visit((SimpleUnaryExpression) node);
			fold(ref node.expr);
			return true;
		}
*/

	}

}
