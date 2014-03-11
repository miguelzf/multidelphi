using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace crosspascal.ast.nodes
{


	public class Expression : Node
	{
	}


	public class UnaryExpression : Expression
	{
	}


	public class LvalueExpression : UnaryExpression
	{
		public IdentifierNode ident;

		public LvalueExpression(IdentifierNode ident)
		{
			this.ident = ident;
		}
	}


	public class OperatorNode : Node
	{
		public string op;

		public OperatorNode(string op)
		{
			this.op = op;
		}
	}

	public class ExpressionNodeList : Node
	{
		public Expression exp;
		public ExpressionNodeList next;

		public ExpressionNodeList(Expression exp, ExpressionNodeList next)
		{
			this.exp = exp;
			this.next = next;
		}
	}

	public class EnumList : DeclarationNode
	{
		public FieldInit element;
		public EnumList next;

		public EnumList(FieldInit element, EnumList next)
		{
			this.element = element;
			this.next = next;
		}
	}

	public class IntLiteral : DelphiLiteral
	{
		public int value;

		public IntLiteral(int value)
		{
			this.value = value;
		}
	}

	public class CharLiteralNode : DelphiLiteral
	{
		public char value;

		public CharLiteralNode(char value)
		{
			this.value = value;
		}
	}

	public class StringLiteral : DelphiLiteral
	{
		public string value;

		public StringLiteral(string value)
		{
			this.value = value;
		}
	}

	public class BoolLiteral : DelphiLiteral
	{
		public bool value;

		public BoolLiteral(bool value)
		{
			this.value = value;
		}
	}

	public class RealLiteral : DelphiLiteral
	{
		public double value;

		public RealLiteral(double value)
		{
			this.value = value;
		}
	}

	public class PointerLiteral : DelphiLiteral
	{
	}

	public abstract class DelphiLiteral : Node
	{

	}


	public class LogicalNot : Expression
	{
		public Expression exp;

		public LogicalNot(Expression exp)
		{
			this.exp = exp;
		}
	}

	public class AddressLvalue : Expression
	{
		public Expression exp;

		public AddressLvalue(Expression exp)
		{
			this.exp = exp;
		}
	}

	public class ArrayAccess : Node
	{
		public LvalueExpression lvalue;
		public ExpressionNodeList acessors;

		public ArrayAccess(LvalueExpression lvalue, ExpressionNodeList acessors)
		{
			this.lvalue = lvalue;
			this.acessors = acessors;
		}
	}

	public class PointerDereference : Node
	{
		public Expression expr;

		public PointerDereference(Expression expr)
		{
			this.expr = expr;
		}
	}


	public class TypeCast : Node
	{
		public Expression expr;
		public TypeNode type;

		public TypeCast(TypeNode type, Expression expr)
		{
			this.type = type;
			this.expr = expr;
		}
	}

	public class ProcedureCallNode : Expression
	{
		public LvalueExpression function;
		public ExpressionNodeList arguments;

		public ProcedureCallNode(LvalueExpression function, ExpressionNodeList arguments)
		{
			this.function = function;
			this.arguments = arguments;
		}
	}


	public class FieldAcessNode : LvalueExpression
	{
		public LvalueExpression obj;
		public IdentifierNode field;

		public FieldAcessNode(LvalueExpression obj, IdentifierNode field)
			: base(obj.ident)
		{
			this.obj = obj;
			this.field = field;
		}
	}

	public class IdentifierNodeList : Node
	{
		public IdentifierNode ident;
		public IdentifierNodeList next;

		public IdentifierNodeList(IdentifierNode ident, IdentifierNodeList next)
		{
			this.ident = ident;
			this.next = next;
		}
	}

	public class UnaryOperationNode : Expression
	{
		public Expression a;
		public OperatorNode op;

		public UnaryOperationNode(Expression a, OperatorNode op)
		{
			this.a = a;
			this.op = op;
		}
	}

	public class BinaryOperationNode : Expression
	{
		public Expression a;
		public Expression b;
		public OperatorNode op;

		public BinaryOperationNode(Expression a, Expression b, OperatorNode op)
		{
			this.a = a;
			this.b = b;
			this.op = op;
		}
	}

	public class IdentifierNode : Node
	{
		public string value;

		public IdentifierNode(string val)
		{
			this.value = val;
		}
	}

	public class IdentifierNodeWithLocation : IdentifierNode
	{
		public string location;

		public IdentifierNodeWithLocation(string value, string location)
			: base(value)
		{
			this.location = location;
		}
	}

	public class FieldAccess : IdentifierNode
	{
		public string qualid;

		public FieldAccess(string value, string qualid)
			: base(value)
		{
			this.qualid = qualid;
		}
	}



	public class SetElement : Node
	{
		public Expression min;
		public Expression max;

		public SetElement(Expression min, Expression max)
		{
			this.min = min;
			this.max = max;
		}

	}

	public class SetList : Node
	{
		public SetElement element;
		public SetList next;

		public SetList(SetElement element, SetList next)
		{
			this.element = element;
			this.next = next;
		}

	}


	public class ArraySizeList : Node
	{
	}

	public class ArrayRangeList : ArraySizeList
	{
		public SetElement range;
		public ArraySizeList next;

		public ArrayRangeList(SetElement range, ArraySizeList next)
		{
			this.range = range;
			this.next = next;
		}
	}






	public class ArrayTypeList : ArraySizeList
	{
		public TypeNode range;

		public ArrayTypeList(TypeNode range)
		{
			this.range = range;
		}
	}
	
}