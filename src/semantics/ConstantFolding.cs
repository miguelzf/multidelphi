using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using crosspascal.ast.nodes;

namespace crosspascal.semantics
{
    class ConstantFolder
    {
        private static bool isReal(double x)
        {
            double frac = x - Math.Floor(x);
            return (frac != 0.0);
        }

        public static Expression FoldExpression(Expression expr)
        {
            if (expr is LogicalBinaryExpression)
            {
                LogicalBinaryExpression bin = (expr as LogicalBinaryExpression);

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
                
                if (!(bin.left is Literal))
                    return expr;
                if (!(bin.right is Literal))
                    return expr;

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
                    if ((bin.left is RealLiteral && bin.left is OrdinalLiteral)
                        && (bin.right is RealLiteral || bin.right is OrdinalLiteral)
                        )
                    {
                        double a, b;

                        if (bin.left is OrdinalLiteral)
                            a = ((bin.left as OrdinalLiteral).Value as IntegralValue).val;
                        else
                            a = ((bin.left as RealLiteral).Value as RealValue).val;

                        if (bin.right is OrdinalLiteral)
                            b = ((bin.right as OrdinalLiteral).Value as IntegralValue).val;
                        else
                            b = ((bin.right as RealLiteral).Value as RealValue).val;

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

                        if (isReal(result) || (bin.left is RealLiteral) || (bin.right is RealLiteral))
                        {
                            return new RealLiteral(result);
                        }
                        else
                        {
                            return new IntLiteral((ulong)result);
                        }
                    }
                    else
                        return expr;
            }
            else
                return expr;
        }
    }
}
