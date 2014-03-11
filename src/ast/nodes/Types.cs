using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace crosspascal.ast.nodes
{
	public class TypeNode : Node
	{

	}

	public class ArrayType : TypeNode
	{
		public ArraySizeList size;
		public TypeNode type;

		public ArrayType(ArraySizeList size, TypeNode type)
		{
			this.size = size;
			this.type = type;
		}
	}

	public class SetType : TypeNode
	{
		public TypeNode type;

		public SetType(TypeNode type)
		{
			this.type = type;
		}
	}

	public class FileType : TypeNode
	{
		public TypeNode type;

		public FileType(TypeNode type)
		{
			this.type = type;
		}
	}

	public class ClassType : TypeNode
	{
		public TypeNode baseType;

		public ClassType(TypeNode baseType)
		{
			this.baseType = baseType;
		}
	}

	public class VariantType : TypeNode
	{
	}

	public class PointerType : TypeNode
	{
		public TypeNode type;

		public PointerType(TypeNode type)
		{
			this.type = type;
		}
	}

	public class IntegerType : TypeNode
	{
	}

	public class FloatingPointType : TypeNode
	{
	}

	public class FloatType : FloatingPointType
	{
	}

	public class DoubleType : FloatingPointType
	{
	}

	public class ExtendedType : FloatingPointType
	{
	}

	public class CurrencyType : FloatingPointType
	{
	}

	public class CharType : TypeNode
	{
	}

	public class BoolType : TypeNode
	{
	}

	public class UnsignedInt8Type : IntegerType // byte
	{
	}

	public class UnsignedInt16Type : IntegerType // word
	{
	}

	public class UnsignedInt32Type : IntegerType // cardinal
	{
	}

	public class UnsignedInt64Type : IntegerType // uint64
	{
	}

	public class SignedInt8Type : IntegerType // smallint
	{
	}

	public class SignedInt16Type : IntegerType // smallint
	{
	}

	public class SignedInt32Type : IntegerType // integer
	{
	}

	public class SignedInt64Type : IntegerType // int64
	{
	}

	public class StringType : TypeNode
	{
		public Expression size;

		public StringType(Expression size)
		{
			this.size = size;
		}
	}


	public class ClassDefinition : TypeNode
	{
		public ClassType classType;
		public IdentifierNodeList heritage;
		public ClassBody ClassBody;

		public ClassDefinition(ClassType classType, IdentifierNodeList heritage, ClassBody ClassBody)
			: base()
		{
			this.classType = classType;
			this.heritage = heritage;
			this.ClassBody = ClassBody;
		}

	}


	public class InterfaceDefinition : TypeNode
	{
		public IdentifierNodeList heritage;
		public ClassContentList methods;
		public ClassContentList properties;

		public InterfaceDefinition(IdentifierNodeList heritage, ClassContentList methods, ClassContentList properties)
			: base()
		{
			this.heritage = heritage;
			this.methods = methods;
			this.properties = properties;
		}

	}
}
