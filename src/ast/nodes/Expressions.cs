using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace crosspascal.ast.nodes
{


	public class DelphiExpression : DelphiNode
	{
	}


	public class UnaryExpression : DelphiExpression
	{
	}


	public class LValueNode : UnaryExpression
	{
		public IdentifierNode ident;

		public LValueNode(IdentifierNode ident)
		{
			this.ident = ident;
		}
	}


	public class OperatorNode : DelphiNode
	{
		public string op;

		public OperatorNode(string op)
		{
			this.op = op;
		}
	}

	public class ExpressionListNode : DelphiNode
	{
		public DelphiExpression exp;
		public ExpressionListNode next;

		public ExpressionListNode(DelphiExpression exp, ExpressionListNode next)
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

	public class StringLiteralNode : DelphiLiteral
	{
		public string value;

		public StringLiteralNode(string value)
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

	public abstract class DelphiLiteral : DelphiNode
	{

	}


	public class NegationNode : DelphiExpression
	{
		public DelphiExpression exp;

		public NegationNode(DelphiExpression exp)
		{
			this.exp = exp;
		}
	}

	public class AddressNode : DelphiExpression
	{
		public DelphiExpression exp;

		public AddressNode(DelphiExpression exp)
		{
			this.exp = exp;
		}
	}

	public class ArrayAccessNode : DelphiNode
	{
		public LValueNode lvalue;
		public ExpressionListNode acessors;

		public ArrayAccessNode(LValueNode lvalue, ExpressionListNode acessors)
		{
			this.lvalue = lvalue;
			this.acessors = acessors;
		}
	}

	public class PointerDereferenceNode : DelphiNode
	{
		public DelphiExpression expr;

		public PointerDereferenceNode(DelphiExpression expr)
		{
			this.expr = expr;
		}
	}


	public class TypeCastNode : DelphiNode
	{
		public DelphiExpression expr;
		public TypeNode type;

		public TypeCastNode(TypeNode type, DelphiExpression expr)
		{
			this.type = type;
			this.expr = expr;
		}
	}

	public class ProcedureCallNode : DelphiExpression
	{
		public LValueNode function;
		public ExpressionListNode arguments;

		public ProcedureCallNode(LValueNode function, ExpressionListNode arguments)
		{
			this.function = function;
			this.arguments = arguments;
		}
	}


	public class FieldAcessNode : LValueNode
	{
		public LValueNode obj;
		public IdentifierNode field;

		public FieldAcessNode(LValueNode obj, IdentifierNode field)
			: base(obj.ident)
		{
			this.obj = obj;
			this.field = field;
		}
	}

	public class IdentifierListNode : DelphiNode
	{
		public IdentifierNode ident;
		public IdentifierListNode next;

		public IdentifierListNode(IdentifierNode ident, IdentifierListNode next)
		{
			this.ident = ident;
			this.next = next;
		}
	}

	public class UnaryOperationNode : DelphiExpression
	{
		public DelphiExpression a;
		public OperatorNode op;

		public UnaryOperationNode(DelphiExpression a, OperatorNode op)
		{
			this.a = a;
			this.op = op;
		}
	}

	public class BinaryOperationNode : DelphiExpression
	{
		public DelphiExpression a;
		public DelphiExpression b;
		public OperatorNode op;

		public BinaryOperationNode(DelphiExpression a, DelphiExpression b, OperatorNode op)
		{
			this.a = a;
			this.b = b;
			this.op = op;
		}
	}

	public class IdentifierNode : DelphiNode
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

	public class IdentifierNodeWithField : IdentifierNode
	{
		public string qualid;

		public IdentifierNodeWithField(string value, string qualid)
			: base(value)
		{
			this.qualid = qualid;
		}
	}



	public class SetElement : DelphiNode
	{
		public DelphiExpression min;
		public DelphiExpression max;

		public SetElement(DelphiExpression min, DelphiExpression max)
		{
			this.min = min;
			this.max = max;
		}

	}

	public class SetList : DelphiNode
	{
		public SetElement element;
		public SetList next;

		public SetList(SetElement element, SetList next)
		{
			this.element = element;
			this.next = next;
		}

	}


	public class ArraySizeList : DelphiNode
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