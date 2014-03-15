using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using crosspascal.ast.nodes;

namespace crosspascal.parser
{
	public class TypeRegistry
	{
		Dictionary<String, Declaration> types = new Dictionary<String, Declaration>();

		public void RegisterDeclaration(String name, Declaration decl)
		{
			types.Add(name, decl);
		}

		public ScalarType FetchTypeScalar(String name)
		{
			return (ScalarType) FetchType(name, typeof(ScalarType));
		}

		public IntegralType FetchTypeIntegral(String name)
		{
			return (IntegralType)FetchType(name, typeof(IntegralType));
		}

		public TypeNode FetchType(String name, System.Type expected)
		{
			TypeNode type = FetchType(name);
			if (!type.ISA(expected))
				throw new ParserException("Invalid type '" + name + "'. Required '" + expected + "'");
			return type;
		}

		public TypeNode FetchType(String name)
		{
			if (!types.ContainsKey(name))
				throw new ParserException("Type '" + name + "' is unknown");

			Declaration decl = types[name];
			if (!decl.ISA(typeof(TypeDeclaration)))
				throw new ParserException("Identifier '" + name + "' does not refer to a type");

			TypeDeclaration tdecl = (TypeDeclaration)decl;
			return tdecl.type;
		}

		void CreateBuiltinType(String name, VariableType type)
		{
			types.Add(name, new TypeDeclaration(name, type));
		}

		void LoadBuiltinTypes()
		{
			CreateBuiltinType("boolean", BoolType.Single);
			CreateBuiltinType("pchar", StringType.Single);
			CreateBuiltinType("shortstring", StringType.Single);
			CreateBuiltinType("widestr", StringType.Single);
			CreateBuiltinType("real48", DoubleType.Single);
			CreateBuiltinType("float", FloatType .Single);
			CreateBuiltinType("double", DoubleType.Single);
			CreateBuiltinType("extended", ExtendedType.Single);
			CreateBuiltinType("curryency", CurrencyType.Single);
			CreateBuiltinType("byte", UnsignedInt8Type.Single);
			CreateBuiltinType("integer",  SignedInt32Type.Single);
			CreateBuiltinType("shortint", SignedInt8Type .Single);
			CreateBuiltinType("smallint",  SignedInt16Type.Single);
			CreateBuiltinType("longint", SignedInt32Type.Single);
			CreateBuiltinType("int64", SignedInt64Type.Single);
			CreateBuiltinType("uint64", UnsignedInt64Type.Single);
			CreateBuiltinType("word", UnsignedInt16Type.Single);
			CreateBuiltinType("longword", UnsignedInt32Type.Single);
			CreateBuiltinType("cardinal", UnsignedInt32Type.Single);
			CreateBuiltinType("comp", SignedInt64Type.Single);
			CreateBuiltinType("char", CharType.Single);
			CreateBuiltinType("widechar", CharType.Single);
			CreateBuiltinType("variant", new VariantType());
			CreateBuiltinType("olevariant", new VariantType());
		}

		public TypeRegistry()
		{
			LoadBuiltinTypes();
		}
	}
}
