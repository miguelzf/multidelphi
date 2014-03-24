using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using crosspascal.semantics;

namespace crosspascal.ast.nodes
{

	//
	// Composite declarations
	//

	public abstract partial class CompositeDeclaration : TypeDeclaration
	{
		public CompositeType Type { get { return type as CompositeType; } }

		public CompositeDeclaration(String name, CompositeType ctype)
			: base(name, ctype)
		{
		}
	}

	public class ClassDeclaration : CompositeDeclaration
	{
		public new ClassType Type { get { return type as ClassType; } }

		public ClassDeclaration(String name, ClassType ctype)
			: base(name, ctype)
		{
		}
	}

	public class InterfaceDeclaration : CompositeDeclaration
	{
		public new InterfaceType Type { get { return type as InterfaceType; } }

		public InterfaceDeclaration(String name, InterfaceType ctype)
			: base(name, ctype)
		{
		}
	}



	#region Composite Types

	//
	// Composite Types
	//

	public partial class CompositeType : TypeNode
	{
		public List<String> heritage;

		public ScopedSectionList sections;

		public bool IsPacked { get; set; }

		// optional
		public String Name { get; set; }

		/// <summary>
		/// Returns public and published methods
		/// </summary>
		public IEnumerable<Declaration> GetPublicMembers()
		{
			var list = sections.Where(x => x.scope == Scope.Public || x.scope == Scope.Published);
			return list.SelectMany(x => x.Decls());
		}

		/// <summary>
		/// Returns public, protected and published methods
		/// </summary>
		public IEnumerable<Declaration> GetInheritableMembers()
		{
			var list = sections.Where(x => x.scope != Scope.Private);
			return list.SelectMany(x => x.Decls());
		}

		public CompositeType(ArrayList heritage, ScopedSectionList seclist)
		{
			this.heritage = new List<String>();

			if (heritage != null)
				foreach (String s in heritage)
					this.heritage.Add(s);

			sections = seclist;
			if (sections == null)
				sections = new ScopedSectionList();
		}
	}

	public class ClassType : CompositeType
	{
		public FieldDeclaration self;

		public ClassType(ArrayList heritage, ScopedSectionList seclist = null)
			: base(heritage, seclist)
		{
			self = new FieldDeclaration("self", this);

			sections.Add(new ScopedSection(Scope.Protected, new DeclarationList(self), null));
		}
	}

	public class InterfaceType : CompositeType
	{
		public UnaryExpression guid;

		public InterfaceType(ArrayList heritage, ScopedSectionList ssec = null, UnaryExpression guid = null)
			: base(heritage, ssec)
		{
			this.guid = guid;
		}
	}

	#endregion



	#region Composite Sections

	//
	// Composite sections
	//

	public enum Scope
	{
		Public,
		Protected,
		Private,
		Published
	}

	public class ScopedSection : Section
	{
		public Scope scope;

		public DeclarationList fields;

		public ScopedSection(Scope scope, DeclarationList fields, DeclarationList components)
			: base(components)
 		{
			this.scope	= scope;
			this.fields = fields;
			if (fields == null)
				fields = new DeclarationList();
		}

		/// <summary>
		/// Fields, Methods and Properties
		/// </summary>
		public IEnumerable<Declaration> Decls()
		{
			foreach (var f in fields)
				yield return f;
			foreach (var d in decls)
				yield return d;
		}
	}

	public class ScopedSectionList : ListNode<ScopedSection>
	{
		public ScopedSectionList() : base() { }

		public ScopedSectionList(ScopedSection s) : base(s) { }
	}

	#endregion


	#region Object Fields

	//
	// Object fields (in classes and records)
	//

	/// <summary>
	/// Composite or record field declaration
	/// </summary>
	public class FieldDeclaration : ValueDeclaration
	{
		public bool isStatic;

		public FieldDeclaration(String id, TypeNode t = null, bool isStatic =false)
			: base(id, t)
		{
			this.isStatic = false;
		}
	}

	/// <summary>
	/// Variant record field declaration
	/// </summary>
	public class VariantDeclaration : FieldDeclaration
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
	public class VarEntryDeclaration : FieldDeclaration
	{
		public ConstExpression tagvalue;
		public RecordType fields;

		public VarEntryDeclaration(ConstExpression tagvalue, DeclarationList fields)
			: base(null, null)	// type must be later set to the variant type
		{
			this.tagvalue = tagvalue;
			this.fields = new RecordType(fields);
		}
	}

	#endregion


	#region Properties

	public class PropertyDeclaration : FieldDeclaration
	{
		public PropertySpecifiers specifiers;

		public bool IsStatic;

		public PropertyDeclaration(String ident, TypeNode type, PropertySpecifiers specs = null)
			: base(ident, type)
		{
			this.specifiers = specs;

			if (type != null) // no override
				if (specs.read == null && specs.write == null)
					Error("Class property must have at least a Read of Write specified");
		}
	}

	public class ArrayProperty : PropertyDeclaration
	{
		public DeclarationList indexes;
		public bool isDefault;

		public ArrayProperty(String ident, TypeNode type, DeclarationList indexes, 
									PropertySpecifiers specs, bool def)
			: base(ident, type, specs)
		{
			this.indexes = indexes;
			this.specifiers = specs;
			this.isDefault = def;
		}
	}

	public class PropertySpecifiers : Node
	{
		public IntLiteral index;
		public String read;
		public String write;
		public ConstExpression stored;
		public Literal @default;	// nodefault == Int32.MaxValue 
		public String impl;

		public PropertySpecifiers(String read, String write)
		{
			this.read = read;
			this.write = write;
		}

		public PropertySpecifiers(IntLiteral index, String read, String write, 
									ConstExpression stored, Literal @default)
			: this(read, write)
		{
			this.index = index;
			this.stored = stored;
			this.@default = @default;
		}

		public PropertySpecifiers(IntLiteral index, String read, String write, 
									ConstExpression stored, Literal @default, String impl)
			: this(index, read, write, stored, @default)
		{
			this.impl = impl;
		}
	}

	#endregion

}