using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using crosspascal.ast.nodes;
using crosspascal.semantics;

namespace crosspascal.parser
{
	partial class DelphiScanner
	{
		
		Dictionary<System.Type,int> IdResolverMap = new Dictionary<Type,int>(); 

		void InitIdResolver()
		{
			// The most common types are not added. It's faster to test with 'is'

		/*	// scalar
			IdResolverMap.Add(typeof(StringType), ID_SCALAR);
			IdResolverMap.Add(typeof(FixedStringType), ID_SCALAR);
			IdResolverMap.Add(typeof(PointerType), ID_SCALAR);
			// structured
			IdResolverMap.Add(typeof(ArrayType), ID_STRUCTURED);
			IdResolverMap.Add(typeof(SetType), ID_STRUCTURED);
			IdResolverMap.Add(typeof(FileType), ID_STRUCTURED);
			IdResolverMap.Add(typeof(RecordType), ID_STRUCTURED);

			IdResolverMap.Add(typeof(ClassType), ID_COMPOSITE);
			IdResolverMap.Add(typeof(InterfaceType), ID_COMPOSITE);
		*/
			// uncommon types
			IdResolverMap.Add(typeof(MetaclassType), ID_METACLASS);
			IdResolverMap.Add(typeof(EnumType), ID_ORDINAL);
			IdResolverMap.Add(typeof(RangeType), ID_ORDINAL);
			IdResolverMap.Add(typeof(VariantType), ID_VARIANT);
		}

		int ProcessIdentifier(string id)
		{
			Declaration decl = DelphiParser.DeclReg.GetDeclaration(id);

			if (decl is TypeDeclaration && !(decl is CallableDeclaration))
			{
				yylval = decl.type;

				if (decl.type is IntegerType)
					return ID_INTEGRAL;
				if (decl.type is ScalarType)
					return ID_SCALAR;
				if (decl.type is StructuredType)
					return ID_STRUCTURED;
				if (decl.type is CompositeType)
					return ID_COMPOSITE;
			
				System.Type rtype = decl.type.GetType();
				if (!IdResolverMap.ContainsKey(rtype))
					yyerror("Type unknown: " + decl);
				else
					return IdResolverMap[rtype];
			}

			yylval = id;

			// if decl is null, ID has not yet been declare:
			//	it may be an error, a uses clause, a declaration, or a forward declaration

			return IDENTIFIER;
		}

	}
}
