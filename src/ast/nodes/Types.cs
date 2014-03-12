using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace crosspascal.ast.nodes
{

	//==========================================================================
	// Types' base classes
	//==========================================================================

	#region Type hierarchy
	///		Types
	///			DeclaredType => may be any, user-defined
	///			UndefinedType	< for untyped parameters. incompatible with any type >
	///			RoutineType
	///			ClassType
	///			VariableType
	///				ScalarType
	///					SimpleType		: IOrdinalType
	///						IntegerType
	///							UnsignedInt	...
	///							SignedInt	...
	///						Bool
	///						Char
	///					RealtType
	///						FloatType
	///						DoubleType
	///						ExtendedType
	///						CurrencyType
	///					StringType
	///					VariantType
	///					PointerType <ScalarType> 
	///				EnumType			: IOrdinalType
	///				RangeType			: IOrdinalType
	///				MetaclassType < id>
	///				StructuredType
	///					Array < VariableType> 
	///					Set	  < VariableType> 
	///					File  < VariableType> 
	///				Record
	#endregion


	public abstract class TypeNode : Node
	{

	}

	/// <summary>
	/// Undefined, incompatible type. For untyped parameters
	/// An expression with this type must be cast to some defined type before using
	/// </summary>
	public class UndefinedType: TypeNode
	{
		public static readonly UndefinedType Default = new UndefinedType();
	}

	/// <summary>
	/// Custom/User-defined type
	/// </summary>
	public class DeclaredType : TypeNode
	{
		String name;

		public DeclaredType(String name)
		{
			this.name = name;
		}
	}

	public class RoutineType : TypeNode
	{
		// TODO
	}

	public class ClassType : DeclaredType
	{
		public ClassType(String name) : base(name) { }
	}

	public abstract class VariableType : TypeNode
	{
		public int typeSize;
	}

	public class RecordType : VariableType
	{
		TypeList compTypes;

		public RecordType(TypeList compTypes)
		{
			this.compTypes = compTypes;
		}
	}



	public interface IOrdinalType<T> 
	{
		public T MinValue();

		public T MaxValue();

		public UInt64 ValueRange();
	}

	public class MetaclassType : TypeNode
	{
		public TypeNode baseType;

		public MetaclassType(TypeNode baseType)
		{
			this.baseType = baseType;
		}
	}


	public class TypeUnknown : TypeNode
	{
		public static readonly UndefinedType Default = new UndefinedType();	
	}



	#region Scalar Types

	///	==========================================================================
	/// Scalar Types
	///	==========================================================================
	/// <summary>
	///	ScalarType
	///		DiscreteType		: IOrdinalType
	///			IntegerType
	///				UnsignedInt	...
	///				SignedInt	...
	///			Bool
	///			Char
	///		RealType
	///			FloatType
	///			DoubleType
	///			ExtendedType
	///			CurrencyType
	///		StringType
	///		VariantType
	///		PointerType <ScalarType> 
	/// </summary>

	public abstract class ScalarType : VariableType
	{

	}

	public class StringType : ScalarType
	{

	}

	public class VariantType : ScalarType
	{

	}

	public class PointerType<T> : ScalarType where T : ScalarType
	{
		ScalarType pointedType;

		public PointerType(ScalarType pointedType)
		{
			this.pointedType = pointedType;
		}
	}


	#region Discrete Types

	public abstract class IntegralType<ValueType> : ScalarType, IOrdinalType<ValueType>
	{
		public abstract ValueType MinValue();

		public abstract ValueType MaxValue();

		public abstract UInt64 ValueRange(); 
	}
	

	#region Integer Types

	public abstract class IntegerType<T> : IntegralType<T>
	{

	}

	public abstract class SignedIntegerType<T> : IntegerType<T>
	{

	}

	public abstract class UnsignedIntegerType<T> : IntegerType<T>
	{

	}

	public class UnsignedInt8Type : UnsignedIntegerType<Byte> // byte
	{
		public static readonly UnsignedInt8Type Default = new UnsignedInt8Type();

		public Byte MinValue() { return Byte.MinValue; }

		public Byte MaxValue() { return Byte.MaxValue; }

		public UInt64 ValueRange() { return (UInt64) (MaxValue() - MinValue()); }
	}

	public class UnsignedInt16Type : UnsignedIntegerType<UInt16> // word
	{
		public static readonly UnsignedInt16Type Default = new UnsignedInt16Type();

		public UInt16 MinValue() { return UInt16.MinValue; }

		public UInt16 MaxValue() { return UInt16.MaxValue; }

		public UInt64 ValueRange() { return (UInt64)(MaxValue() - MinValue()); }
	}

	public class UnsignedInt32Type : UnsignedIntegerType<UInt32> // cardinal
	{
		public static readonly UnsignedInt32Type Default = new UnsignedInt32Type();

		public UInt32 MinValue() { return UInt32.MinValue; }

		public UInt32 MaxValue() { return UInt32.MaxValue; }

		public UInt64 ValueRange() { return (UInt64)(MaxValue() - MinValue()); }
	}

	public class UnsignedInt64Type : UnsignedIntegerType<UInt64> // uint64
	{
		public static readonly UnsignedInt64Type Default = new UnsignedInt64Type();

		public UInt64 MinValue() { return UInt64.MinValue; }

		public UInt64 MaxValue() { return UInt64.MaxValue; }

		public UInt64 ValueRange() { return (UInt64)(MaxValue() - MinValue()); }
	}

	public class SignedInt8Type : SignedIntegerType<SByte> // smallint
	{
		public static readonly SignedInt8Type Default = new SignedInt8Type();

		public SByte MinValue() { return SByte.MinValue; }

		public SByte MaxValue() { return SByte.MaxValue; }

		public UInt64 ValueRange() { return (UInt64)(MaxValue() - MinValue()); }
	}

	public class SignedInt16Type : SignedIntegerType<Int16> // smallint
	{
		public static readonly SignedInt16Type Default = new SignedInt16Type();

		public Int16 MinValue() { return Int16.MinValue; }

		public Int16 MaxValue() { return Int16.MaxValue; }

		public UInt64 ValueRange() { return (UInt64)(MaxValue() - MinValue()); }
	}

	public class SignedInt32Type : SignedIntegerType<Int32> // integer
	{
		public static readonly SignedInt32Type Default = new SignedInt32Type();

		public Int32 MinValue() { return Int32.MinValue; }

		public Int32 MaxValue() { return Int32.MaxValue; }

		public UInt64 ValueRange() { return (UInt64)(MaxValue() - MinValue()); }
	}

	public class SignedInt64Type : IntegerType<Int64> // int64
	{
		public static readonly SignedInt64Type Default = new SignedInt64Type();

		public Int64 MinValue() { return Int64.MinValue; }

		public Int64 MaxValue() { return Int64.MaxValue; }

		public UInt64 ValueRange() { return (UInt64)(MaxValue() - MinValue()); }
	}

	#endregion


	public class BoolType : IntegralType<Boolean>
	{
		public static readonly BoolType Default = new BoolType();

		public Boolean MinValue() { return false; }

		public Boolean MaxValue() { return true; }

		public UInt64 ValueRange() { return 2; }
	}

	public class CharType : IntegralType<Char>
	{
		public static readonly CharType Default = new CharType();

		public Char MinValue () { return Char.MinValue; }

		public Char MaxValue () { return Char.MaxValue; }

		public UInt64 ValueRange() { return (UInt64)(MaxValue() - MinValue()); }
	}


	#endregion


	#region Floating-Point Types

	public abstract class RealType : ScalarType
	{

	}

	public class FloatType : RealType
	{
		public static readonly FloatType Default = new FloatType();
	}

	public class DoubleType : RealType
	{
		public static readonly DoubleType Default = new DoubleType();
	}

	public class ExtendedType : RealType
	{
		public static readonly ExtendedType Default = new ExtendedType();
	}

	public class CurrencyType : RealType
	{
		public static readonly CurrencyType Default = new CurrencyType();
	}

	#endregion

	/// ==========================================================================
	/// ==========================================================================

	#endregion		// Scalar types


	#region Structured Types
	///	==========================================================================
	/// Structured Types
	///	==========================================================================
	///		StructuredType
	///			Array < VariableType> 
	///			Set	  < VariableType> 
	///			File  < VariableType> 

	public abstract class StructuredType<T> : VariableType where T : VariableType
	{
		public VariableType basetype;

		protected StructuredType(VariableType t)
		{
			basetype = t;
		}
	}

	public class ArrayType<T> : StructuredType<T> where T : VariableType
	{
		public List<int> dimensions = new List<int>();

		void AddDimension(uint size)
		{
			if (size * basetype.typeSize > (1<<9))	// 1gb max size
				Error("Array size too large: " + size);

			dimensions.Add((int)size);
		}

		public ArrayType(VariableType type) : base(type)
		{
			// dynamic array
		}

		public ArrayType(VariableType type, NodeList dims) : base(type)
		{
			// TODO Check constant and compute value
			foreach (Node n in dims)
			{
				SetRange range = (SetRange)n;
			//	int dim = range.max - range.min;
			//	AddDimension(dim);
			}
		}

		public ArrayType(VariableType type, String ordinalTypeId) : base(type)
		{
			// TODO Resolve and check type size
		}

		public ArrayType(VariableType type, IntegralType<T> sizeType): base(type)
		{
			UInt64 size = sizeType.ValueRange();
			if (size > Int32.MaxValue) size = Int32.MaxValue;
			AddDimension((uint) size);
		}
	}

	public class SetType<T> : StructuredType<T> where T : VariableType
	{
		public SetType(VariableType type) : base(type) { }
	}

	public class FileType<T> : StructuredType<T> where T : VariableType
	{
		public FileType(VariableType type = null) : base(type) { }
	}

	#endregion

}
