using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MultiPascal.core;

namespace MultiPascal.AST.Nodes
{
	// override Equals but not GetHashCode warning

	//==========================================================================
	// Types' base classes
	//==========================================================================


	#region Type hierarchy
	/// <remarks>
	/// Types Hierarchy:
	/// 	ProceduralType
	/// 	CompositeType
	///			ClassType
	///			 	ClassRefType
	///			InterfaceType
	/// 	VariableType
	/// 		ScalarType
	/// 			IntegralType		: IOrdinalType
	/// 				IntegerType
	/// 					UnsignedInt
	/// 						...
	/// 					SignedInt
	/// 						...
	/// 				Bool
	/// 				Char
	/// 			RealType
	/// 				....
	/// 			StringType
	/// 			PointerType > ScalarType
	///		 		MetaclassType > id
	/// 		EnumType			: IOrdinalType
	/// 		RangeType			: IOrdinalType
	///			StructuredType > Type
	///				Array > Type
	///				Set	  > OrdinalType 
	///				File  > VariableType
	///		 		Record > TypeNode ...
	///		 			RecordRefType
	///			VariantType > VariableType
	///		 		
	/// UnresolvedTypes extend each type, so that they may be used in their place
	/// Most type descriptions were taken from the Embarcadero wiki
	/// </remarks>
	#endregion


	public abstract class TypeNode : Node
	{
		public bool IsFieldedType()
		{
			return this is RecordRefType || this is CompositeType;
		}

		public override bool Equals(Object o)
		{
			throw new InvalidAbstractException(this, "Equals");
		}

		/// <summary>
		/// WARNING!: only works for static Single types
		/// </summary>
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}

	#pragma warning disable 0659


	#region Unresolved Type Nodes

	//
	// Types unresolved during parsing, to be resolved by the TypeProcesor/Resolver
	//

	public interface IUnresolvedType
	{

	}

	public class UnresolvedType : TypeNode, IUnresolvedType
	{
		public String id;

		public UnresolvedType(String id)
		{
			this.id = id;
		}
	}

	public class UnresolvedVariableType : VariableType, IUnresolvedType
	{
		public String id;

		public UnresolvedVariableType(String id)
		{
			this.id = id;
		}
	}

	public class UnresolvedIntegralType : IntegralType, IUnresolvedType
	{
		public String id;

		public UnresolvedIntegralType(String id)
		{
			this.id = id;
		}
	}


	public class UnresolvedOrdinalType : VariableType, IOrdinalType, IUnresolvedType
	{
		public Int64 MinValue() { return 0; }

		public Int64 MaxValue() { return 0; }

		public UInt64 ValueRange() { return 0; }
		
		public String id;

		public UnresolvedOrdinalType(String id)
		{
			this.id = id;
		}
	}

	#endregion



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

		/// <summary>
		/// Classes and interfaces are always references, use reference comparison.
		/// </summary>
		public override bool Equals(Object o)
		{
			return this == o;
		}
	}


	/// <summary>
	/// Variable types: types can be used in simple variable declarations
	/// </summary>
	public abstract class VariableType : TypeNode
	{
		// TODO each derive should set the typesize
		public int typeSize;

		public override bool Equals(Object o)
		{
			throw new InvalidAbstractException(this, "Equals");
		}
	}


	#region Ordinal Types

	public interface IOrdinalType
	{
		Int64 MinValue();

		Int64 MaxValue();

		UInt64 ValueRange();
	}


	/// <summary>
	/// RangeType = (id,id,id...)
	/// </summary>
	/// <remarks>
	/// An enumerated type defines an ordered set of values by simply listing identifiers that denote these values
	/// </remarks>
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
			return (o is EnumType) && enumVals.SequenceEqual((o as EnumType).enumVals);
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


	/// <summary>
	/// RangeType = (OrdinalValue .. OrdinalValue)
	/// </summary>
	/// <remarks>
	/// Subrange type represents a subset of the values in another ordinal type (called the base type)
	/// </remarks>
	public class RangeType : VariableType, IOrdinalType
	{
		public Expression min;
		public Expression max;

		public RangeType(Expression min, Expression max)
		{
			this.min = min;
			this.max = max;
			this.max.ForcedType = this.min.ForcedType = IntegralType.Single;

			// TODO check that:
			//	min.type == max.type
			//	min.value < max.value
			//	value.range <= type.range:
		}

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

		/// <summary>
		/// Compares the Range's limits. 
		/// Limits must have been determined by constant folding prior to this
		/// </summary>
		public override bool Equals(Object o)
		{
			if (!(o is RangeType))
				return false;

			var or = (RangeType)o;
			return	MinValue() == or.MinValue() && MaxValue() == or.MaxValue();
		}
	}

	#endregion
	

	#region Scalar Types

	///	==========================================================================
	/// Scalar Types
	///	==========================================================================
	/// <remarks>
	///	ScalarType
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
			throw new InvalidAbstractException(this, "Equals");
		}
	}


	#region Integral Types

	public class IntegralType : ScalarType, IOrdinalType
	{
		public static readonly IntegralType Single = new IntegralType();

		public override bool Equals(Object o)
		{
			return (this == o);
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
			return (this == o);
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

	public class StringType : ScalarType
	{
		public static readonly StringType Single = new StringType();

		public override bool Equals(Object o)
		{
			return this == o;
		}
	}

	public class FixedStringType : StringType
	{
		public Expression expr;
		public int Len { get; set; }

		public FixedStringType(Expression expr)
		{
			this.expr = expr;

			// TODO in constant-checking
			/*	if (len > 255)
					Error("String too long. Maximum alloweed is 255");
				Len = (int)len;
			 */
		}

		/// <summary>
		/// Compares string length.
		/// Length must have been determined previously by constant folding the expr
		/// </summary>
		public override bool Equals(Object o)
		{
			return (o is FixedStringType) && Len == (o as FixedStringType).Len;
		}
	}


	/// <summary>
	/// Variant data types
	/// </summary>
	/// <remarks>
	/// Variants can hold values of any type except records, sets, static arrays, files, classes, class references, and pointers.
	/// I.e. can hold anything but structured types and pointers.
	/// They can hold interfaces, dynamic arrays, variant arrays
	/// </remarks>
	public class VariantType : VariableType
	{
		public VariableType actualtype;

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


	/// <summary>
	/// Pointer types > PointedType
	/// </summary>
	/// <remarks>
	/// PointedType may be any type.
	/// It may be a not yet declared type (forward declaration)
	/// </remarks>
	public class PointerType : ScalarType
	{
		public TypeNode pointedType;

		public static readonly PointerType Single = new PointerType(null);

		public PointerType(TypeNode pointedType)
		{
			this.pointedType = pointedType;
		}

		public override bool Equals(Object o)
		{
			return (o is PointerType) && pointedType.Equals((o as PointerType).pointedType);
		}
	}


	public class MetaclassType : ScalarType
	{
		public TypeNode baseType;

		public MetaclassType(TypeNode baseType)
		{
			this.baseType = baseType;
		}

		public override bool Equals(Object o)
		{
			return (o is MetaclassType) && baseType.Equals((o as MetaclassType).baseType);
		}
	}



	/// ==========================================================================
	/// ==========================================================================

	#endregion		// Scalar types


	#region Structured Types
	///	==========================================================================
	/// Structured Types
	///	==========================================================================
	///			StructuredType > Type
	///				Array > Type
	///				Set	  > OrdinalType 
	///				File  > VariableType
	///		 		Record > TypeNode ...

	public abstract class StructuredType : VariableType
	{
		public TypeNode basetype;
		public bool IsPacked { get; set; }

		protected StructuredType()
		{
		}

		protected StructuredType(TypeNode t)
		{
			basetype = t;
		}

		public override bool Equals(Object o)
		{
			if (!(o is StructuredType))
				return false;

			StructuredType otype = (StructuredType) o;
			if (IsPacked != otype.IsPacked)
				return false;

			return basetype.Equals(otype.basetype);
		}
	}



	/// <summary>
	/// Array [size] of (BaseType)
	/// </summary>
	/// <remarks>
	/// BaseType is any type excepts explicit/declared enums
	/// </remarks>
	public class ArrayType : StructuredType
	{
		public List<int> dimensions = new List<int>();

		// generic array type
		public static readonly ArrayType Single = new ArrayType(null);

		void AddDimension(uint size)
		{
			// TODO
		//	if (size * basetype.typeSize > (1<<9))	// 1gb max size
		//		Error("Array size too large: " + size);

			dimensions.Add((int)size);
		}

		public ArrayType(TypeNode type) : base(type)
		{
			// dynamic array
		}

		// dims is a list of ranges
		public ArrayType(TypeNode type, TypeList dims)
			: base(type)
		{
			// TODO Check constant and compute value
			foreach (Node n in dims)
			{
				RangeType range = (RangeType)n;
			//	int dim = range.max - range.min;
			//	AddDimension(dim);
			}
		}

		public ArrayType(TypeNode type, IntegralType sizeType)
			: base(type)
		{
			UInt64 size = sizeType.ValueRange();
			if (size > Int32.MaxValue) size = Int32.MaxValue;
			AddDimension((uint)size);
		}

		public override bool Equals(Object o)
		{
			return (o is ArrayType) && dimensions.SequenceEqual((o as ArrayType).dimensions);
		}
	}


	/// <summary>
	/// Set of (BaseType : IOrdinalType)
	/// </summary>
	/// <remarks>
	/// BaseType is an ordinal type.
	/// A set is a collection of values of the same ordinal type. The values have no inherent order,
	/// nor is it meaningful for a value to be included twice in a set.
	/// The	range of a set type is the power set of a specific ordinal type, called the base type; 
	/// that is, the possible values of the set type are all the subsets of the base type, including the empty set.
	/// </remarks>
	public class SetType : StructuredType
	{
		public SetType(VariableType type) : base(type)
		{
			// TODO
		//	if (!(type is IOrdinalType))
		//		Error("Base type of Set must be ordinal");
		}

		public override bool Equals(Object o)
		{
			return base.Equals(o) && (o is SetType);
		}
	}


	/// <summary>
	/// File of (BaseType : VariableType)
	/// </summary>
	/// <remarks>
	/// BaseType is a fixed-size type.
	/// Cannot be: Pointer types (implicit or explicit), dynamic arrays, long strings, 
	/// classes, objects, pointers, variants, other files, or structured types that contain any of these. 
	/// </remarks>
	public class FileType : StructuredType
	{
		public FileType(VariableType type = null) : base(type) { }

		public override bool Equals(Object o)
		{
			return base.Equals(o) && (o is FileType);
		}
	}


	#region Records

	/// <summary>
	/// RecordType, structure with a list of declared fields.
	/// Records can only have fields, not methods
	/// </summary>
	public class RecordType : StructuredType
	{
		public DeclarationList compTypes;

		public RecordType(DeclarationList compTypes)
		{
			this.compTypes = compTypes;
		}

		/// <summary>
		/// Returns a field with the given name
		/// </summary>
		public virtual RecordFieldDeclaration GetField(String id)
		{
			return compTypes.GetDeclaration(id) as RecordFieldDeclaration;
		}

		public override bool Equals(Object o)
		{
			return base.Equals(o) && (o is RecordType)
				&& compTypes.SequenceEqual((o as RecordType).compTypes);
		}
	}

	/// <summary>
	/// A reference to a record. 1 level of indirection to avoid circular dependencies
	/// </summary>
	public class RecordRefType : RecordType
	{
		public String qualifid;
		public RecordType reftype;

		public RecordRefType(String qualifid, RecordType reftype = null)
			: base(null)
		{
			this.qualifid = qualifid;
			this.reftype = reftype;
		}

		public override RecordFieldDeclaration GetField(String id)
		{
			return reftype.GetField(id);
		}
	}

	/// <summary>
	/// Composite or record field declaration
	/// </summary>
	public class RecordFieldDeclaration : ValueDeclaration
	{
		public RecordFieldDeclaration(String id, TypeNode t = null)
			: base(id, t)
		{
		}
	}

	/// <summary>
	/// Variant record field declaration
	/// </summary>
	public class VariantDeclaration : RecordFieldDeclaration
	{
		public DeclarationList varfields;

		public VariantDeclaration(String id, VariableType t, DeclarationList varfields)
			: base(id, t)
		{
			// TODO
			///	if (!(t is IOrdinalType))
			//		throw new TypeRequiredException("Ordinal");
			this.varfields = varfields;
		}

		public VariantDeclaration(String id, IntegralType t, DeclarationList varfields)
			: this(id, (VariableType)t, varfields) { }

		public VariantDeclaration(String id, EnumType t, DeclarationList varfields)
			: this(id, (VariableType)t, varfields) { }

		public VariantDeclaration(String id, RangeType t, DeclarationList varfields)
			: this(id, (VariableType)t, varfields) { }
	}

	/// <summary>
	/// Variant case entry declaration
	/// </summary>
	public class VarEntryDeclaration : RecordFieldDeclaration
	{
		public Expression tagvalue;
		public RecordType fields;

		public VarEntryDeclaration(Expression tagvalue, DeclarationList fields)
			: base(null, null)	// type must be later set to the variant type
		{
			this.tagvalue = tagvalue;
			this.fields = new RecordType(fields);
		}
	}

	#endregion	// records
	
	#endregion

}
