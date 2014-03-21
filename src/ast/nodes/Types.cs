using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using crosspascal.core;

namespace crosspascal.ast.nodes
{
	// override Equals but not GetHashCode warning
	#pragma warning disable 659

	//==========================================================================
	// Types' base classes
	//==========================================================================

	#region Type hierarchy
	/// <remarks>
	/// Types
	/// 	UndefinedType	> for untyped parameters. incompatible with any type >
	/// 	ProceduralType
	/// 	CompositeType
	///			ClassType
	///			InterfaceType
	/// 	VariableType
	/// 		ScalarType
	/// 			IntegralType		: IOrdinalType
	/// 				IntegerType
	/// 					UnsignedInt	...
	/// 					SignedInt	...
	/// 				Bool
	/// 				Char
	/// 			RealType
	/// 				FloatType
	/// 				DoubleType
	/// 				ExtendedType
	/// 				CurrencyType
	/// 			StringType
	/// 			VariantType
	/// 			PointerType > ScalarType
	/// 		EnumType			: IOrdinalType
	/// 		RangeType			: IOrdinalType
	/// 		MetaclassType > id
	/// 		StructuredType
	/// 			Array > VariableType 
	/// 			Set	  > VariableType 
	/// 			File  > VariableType
	///		 		Record !??
	/// </remarks>
	#endregion


	public abstract class TypeNode : Node
	{

		public override bool Equals(Object o)
		{
			throw new InvalidAbstractException("Base class TypeNode Equals");
		}
	}

	/// <summary>
	/// Undefined, incompatible type. For untyped parameters
	/// An expression with this type must be cast to some defined type before using
	/// </summary>
	public class UndefinedType: TypeNode
	{
		public static readonly UndefinedType Single = new UndefinedType();

	}

	/// <summary>
	/// Type of a Routine (function, procedure, method, etc)
	/// </summary>
	public partial class ProceduralType : TypeNode
	{
		// In file Routines.cs
	}

	/// <summary>
	/// Type of a Class
	/// </summary>
	public partial class CompositeType : TypeNode
	{
		// In file Composites.cs
	}


	/// <summary>
	/// Variable types: types can be used in simple variable declarations
	/// </summary>
	public abstract class VariableType : TypeNode
	{
		// TODO each derive should set the typesize
		public int typeSize;
	}

	public class MetaclassType : VariableType
	{
		public TypeNode baseType;

		public MetaclassType(TypeNode baseType)
		{
			this.baseType = baseType;
		}

		public override bool Equals(Object o)
		{
			if (o == null || this.GetType() != o.GetType())
				return false;

			MetaclassType rtype = (MetaclassType) o;
			return (this.Equals(rtype));
		}
	}


	public class TypeUnknown : TypeNode
	{
		public static readonly UndefinedType Single = new UndefinedType();

	}


	#region Ordinal Types

	public interface IOrdinalType
	{
		Int64 MinValue();

		Int64 MaxValue();

		UInt64 ValueRange();
	}


	public class EnumType : VariableType, IOrdinalType
	{
		public EnumValueList enumVals;

		public Int64 MinValue() { return (long) enumVals.GetFirst().init.ValueIntegral(); }

		public Int64 MaxValue() { return (long) enumVals.GetLast ().init.ValueIntegral(); }

		public UInt64 ValueRange() {
			return (enumVals.GetLast().init.Value as IntegralValue).range(
					enumVals.GetFirst().init.Value as IntegralValue);
		}

		public override bool Equals(Object o)
		{
			if (o == null || this.GetType() != o.GetType())
				return false;

			EnumType rtype = (EnumType)o;
			var types = enumVals.Zip(rtype.enumVals,
				(x, y) => {
					return new KeyValuePair<TypeNode, TypeNode>(x.type, y.type);
				});

			foreach (var x in types)
				if (!x.Key.Equals(x.Value))
					return false;

			return true;
		}

		public EnumType(EnumValueList enumVals)
		{
			this.enumVals = enumVals;
		}

		/// <summary>
		/// Determine and assign the Enum initializers, from the user-defined to the automatic
		/// </summary>
		// TODO move this elsewhere. Constant-Inference visitor
		public void AssignEnumInitializers()
		{
			long val = 0;	// default start val

			foreach (EnumValue al in enumVals)
			{
				if (al.init == null)
					al.init = new IntLiteral((ulong) val);
				else
				{
					if (al.init.Type is IntegerType)
						val = (long) al.init.ValueIntegral();
					else
						Error("Enum initializer must be an integer");
				}
				val++;
			}
		}
	}

	public class EnumValue : ConstDeclaration
	{
		// Init value to be computed a posteriori
		public EnumValue(string val) : base(val, null) { }

		public EnumValue(string val, Expression init)
			: base(val, init)
		{
			init.ForcedType = IntegerType.Single;
		}
	}


	public class RangeType : VariableType, IOrdinalType
	{
		public ConstExpression min;
		public ConstExpression max;

		public RangeType(ConstExpression min, ConstExpression max)
		{
			this.min = min;
			this.max = max;
			this.max.ForcedType = this.min.ForcedType = IntegralType.Single;

			// TODO check that:
			//	min.type == max.type
			//	min.value < max.value
			//	value.range <= type.range:
		}

		public RangeType(Expression min, Expression max)
			: this(new ConstExpression(min), new ConstExpression(max)) { }

		public Int64 MinValue()
		{
			return (long) min.ValueIntegral();
		}

		public Int64 MaxValue()
		{
			return (long) max.ValueIntegral();
		}

		public UInt64 ValueRange()
		{
			ulong ret = (max.Value as IntegralValue).range(min.Value as IntegralValue);

			// TODO move this elsehwere
			if (ret > (max.Type as IntegralType).ValueRange())
				ErrorInternal("RangeType value range exceeds the base type value range");
			return ret;
		}

		public override bool Equals(Object o)
		{
			if (o == null || this.GetType() != o.GetType())
				return false;

			return	min.Equals(((RangeType)o).min) && max.Equals(((RangeType)o).max);
		}
	}

	#endregion
	

	#region Scalar Types

	///	==========================================================================
	/// Scalar Types
	///	==========================================================================
	/// <remarks>
	///	ScalarType
	///		ScalarTypeForward	- temporary, to be resolved when the pointed type is declared
	///		IntegralType		: IOrdinalType
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
	///		PointerType <!-- ScalarType--> 
	/// </remarks>

	public abstract class ScalarType : VariableType
	{

		/// <summary>
		/// Default Scalar Type comparison: directly compare references to singleton objects
		/// </summary>
		public override bool Equals(Object o)
		{
			throw new InvalidAbstractException("ScalarType Equals");
		}
	}

	/// <summary>
	/// Temporary scalar type to support forward declarations.
	/// This type has to be converted to its actual type by resolving the declared name.
	/// </summary>
	public class ScalarTypeForward : ScalarType
	{
		public String forwardname;

		public ScalarTypeForward(String name)
		{
			forwardname = name;
		}
	}

	public class StringType : ScalarType
	{
		public static readonly StringType Single = new StringType();

		public override bool Equals(Object o)
		{
			if (o == null)
				return false;

			return ReferenceEquals(this, o);
		}
	}

	public class FixedStringType : ScalarType
	{
		public ConstExpression expr;
		public int Len { get; set; }

		public FixedStringType(ConstExpression expr)
		{
			this.expr = expr;

			// TODO in constant-checking
		/*	if (len > 255)
				Error("String too long. Maximum alloweed is 255");
			Len = (int)len;
		 */
		}
	}

	public class VariantType : ScalarType
	{
		public ScalarType actualtype;

		/// <summary>
		/// Actual type cannot be initially known.
		/// </summary>
		public VariantType() { }

		public override bool Equals(Object o)
		{
			// TODO
			return true;
		}
	}

	public class PointerType : ScalarType
	{
		public ScalarType pointedType;

		public static readonly PointerType Single = new PointerType(null);

		public PointerType(ScalarType pointedType)
		{
			this.pointedType = pointedType;
		}

		public override bool Equals(Object o)
		{
			if (o == null || this.GetType() != o.GetType())
				return false;

			PointerType otype = (PointerType) o;
			return pointedType.Equals(otype.pointedType);
		}
	}


	#region Integral Types

	public class IntegralType : ScalarType, IOrdinalType
	{
		public static readonly IntegralType Single = new IntegralType();

		public override bool Equals(Object o)
		{
			if (o == null)
				return false;

			return ReferenceEquals(this, o);
		}

		// Should not be used
		public virtual Int64 MinValue() { return 0; }
		public virtual Int64 MaxValue() { return 0; }
		public virtual UInt64 ValueRange() { return 0; }

		protected IntegralType() { }
	}

	#region Integer Types

	public class IntegerType : IntegralType
	{
		public static new readonly IntegerType Single = new IntegerType();

		// Should not be used
		public override Int64 MinValue() { return 0; }
		public override Int64 MaxValue() { return 0; }
		public override UInt64 ValueRange() { return 0; }
	}

	public class SignedIntegerType : IntegerType
	{
		public static new readonly SignedIntegerType Single = new SignedIntegerType();

		// Should not be used
		public override Int64 MinValue() { return 0; }
		public override Int64 MaxValue() { return 0; }
		public override UInt64 ValueRange() { return 0; }
	}

	public class UnsignedIntegerType : IntegerType
	{
		public static new readonly UnsignedIntegerType Single = new UnsignedIntegerType();

		// Should not be used
		public override Int64 MinValue() { return 0; }
		public override Int64 MaxValue() { return 0; }
		public override UInt64 ValueRange() { return 0; }
	}

	public class UnsignedInt8Type : UnsignedIntegerType		// byte
	{
		public static new readonly UnsignedInt8Type Single = new UnsignedInt8Type();

		public override Int64 MinValue() { return Byte.MinValue; }

		public override Int64 MaxValue() { return Byte.MaxValue; }

		public override UInt64 ValueRange() { return Byte.MaxValue - Byte.MinValue; }
	}

	public class UnsignedInt16Type : UnsignedIntegerType	// word
	{
		public static new readonly UnsignedInt16Type Single = new UnsignedInt16Type();

		public override Int64 MinValue() { return UInt16.MinValue; }

		public override Int64 MaxValue() { return UInt16.MaxValue; }

		public override UInt64 ValueRange() { return UInt16.MaxValue - UInt16.MinValue; }
	}

	public class UnsignedInt32Type : UnsignedIntegerType	// cardinal
	{
		public new static readonly UnsignedInt32Type Single = new UnsignedInt32Type();

		public override Int64 MinValue() { return UInt32.MinValue; }

		public override Int64 MaxValue() { return UInt32.MaxValue; }

		public override UInt64 ValueRange() { return UInt32.MaxValue - UInt32.MinValue; }
	}

	public class UnsignedInt64Type : UnsignedIntegerType	// uint64
	{
		public static new readonly UnsignedInt64Type Single = new UnsignedInt64Type();

		public override Int64 MinValue() { return (long) ulong.MinValue; }

		public override Int64 MaxValue() { unchecked { return (long)ulong.MaxValue; } }

		public override UInt64 ValueRange() { return UInt64.MaxValue - UInt64.MinValue; }
	}

	public class SignedInt8Type : SignedIntegerType			// smallint
	{
		public static new readonly SignedInt8Type Single = new SignedInt8Type();

		public override Int64 MinValue() { return SByte.MinValue; }

		public override Int64 MaxValue() { return SByte.MaxValue; }

		public override UInt64 ValueRange() { return sbyte.MaxValue - (int)sbyte.MinValue; }
	}

	public class SignedInt16Type : SignedIntegerType		// smallint
	{
		public static new readonly SignedInt16Type Single = new SignedInt16Type();

		public override Int64 MinValue() { return Int16.MinValue; }

		public override Int64 MaxValue() { return Int16.MaxValue; }

		public override UInt64 ValueRange() { return short.MaxValue - (int)short.MinValue; }
	}

	public class SignedInt32Type : SignedIntegerType		// integer
	{
		public static new readonly SignedInt32Type Single = new SignedInt32Type();

		public override Int64 MinValue() { return Int32.MinValue; }

		public override Int64 MaxValue() { return Int32.MaxValue; }

		public override UInt64 ValueRange() { return int.MaxValue - (long) int.MinValue; }
	}

	public class SignedInt64Type : IntegerType				// int64
	{
		public static new readonly SignedInt64Type Single = new SignedInt64Type();

		public override Int64 MinValue() { return Int64.MinValue; }

		public override Int64 MaxValue() { return Int64.MaxValue; }

		public override UInt64 ValueRange() { return Int64.MaxValue; }
	}

	#endregion


	public class BoolType : IntegralType
	{
		public static new readonly BoolType Single = new BoolType();

		public override Int64 MinValue() { return 0; }

		public override Int64 MaxValue() { return 1; }

		public override UInt64 ValueRange() { return 2; }
	}

	public class CharType : IntegralType
	{
		public static new readonly CharType Single = new CharType();

		public override Int64 MinValue() { return Char.MinValue; }

		public override Int64 MaxValue() { return Char.MaxValue; }

		public override UInt64 ValueRange() { return Char.MaxValue - Char.MinValue; }
	}


	#endregion


	#region Floating-Point Types

	public class RealType : ScalarType
	{
		public static readonly RealType Single = new RealType();

		public override bool Equals(Object o)
		{
			if (o == null)
				return false;

			return ReferenceEquals(this, o);
		}
	}

	public class FloatType : RealType
	{
		public static new readonly FloatType Single = new FloatType();
	}

	public class DoubleType : RealType
	{
		public static new readonly DoubleType Single = new DoubleType();
	}

	public class ExtendedType : RealType
	{
		public static new readonly ExtendedType Single = new ExtendedType();
	}

	public class CurrencyType : RealType
	{
		public static new readonly CurrencyType Single = new CurrencyType();
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

	public abstract class StructuredType : VariableType
	{
		public VariableType basetype;
		public bool IsPacked { get; set; }

		protected StructuredType()
		{
		}

		protected StructuredType(VariableType t)
		{
			basetype = t;
		}

		public override bool Equals(Object o)
		{
			if (o == null || this.GetType() != o.GetType())
				return false;

			StructuredType otype = (StructuredType)o;
			return basetype.Equals(otype.basetype);
		}
	}

	public class ArrayType : StructuredType
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

		public ArrayType(VariableType type, TypeList dims) : base(type)
		{
			// TODO Check constant and compute value
			foreach (Node n in dims)
			{
				RangeType range = (RangeType)n;
			//	int dim = range.max - range.min;
			//	AddDimension(dim);
			}
		}

		public ArrayType(VariableType type, String ordinalTypeId) : base(type)
		{
			// TODO Resolve and check type size
		}

		public ArrayType(VariableType type, IntegralType sizeType): base(type)
		{
			UInt64 size = sizeType.ValueRange();
			if (size > Int32.MaxValue) size = Int32.MaxValue;
			AddDimension((uint) size);
		}

		public override bool Equals(Object o)
		{
			if (o == null || this.GetType() != o.GetType())
				return false;

			ArrayType otype = (ArrayType) o;
			if (!dimensions.Equals(otype.dimensions))
				return false;

			return basetype.Equals(otype.basetype);
		}
	}

	public class SetType : StructuredType
	{
		public SetType(VariableType type) : base(type)
		{
			if (!(type is IOrdinalType))
				Error("Base type of Set must be ordinal");
		}
	}

	public class FileType : StructuredType
	{
		public FileType(VariableType type = null) : base(type) { }
	}

	/// <summary>
	/// RecordType. Records can only have fields, not methods
	/// </summary>
	/// <remarks>
	/// Although their structure is more alike to Composite types, their use is closer to StructuredTypes:
	///		they can be defined and used anonymously, with Record-type consts
	/// </remarks>
	public class RecordType : StructuredType
	{
		public DeclarationList compTypes;

		public RecordType(DeclarationList compTypes)
		{
			this.compTypes = compTypes;
		}

		public override bool Equals(Object o)
		{
			if (o == null || this.GetType() != o.GetType())
				return false;

			RecordType rtype = (RecordType)o;
			var types = compTypes.Zip(rtype.compTypes,
				(x, y) =>
				{
					return new KeyValuePair<TypeNode, TypeNode>(x.type, y.type);
				});

			foreach (var x in types)
				if (!x.Key.Equals(x.Value))
					return false;

			// types.ForEach( (x) => {	if (!x.Key.Equals(x.Value.Equals)) return false; });
			return true;
		}
	}

	#endregion

}
