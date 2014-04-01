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
			ctype.self = new FieldDeclaration("self", new ClassRefType(name, ctype));
			ctype.self.scope = Scope.Protected;
			ctype.section.fields.Add(ctype.self);
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

		public ObjectSection section;

		public bool IsPacked { get; set; }

		// optional
		public String Name { get; set; }
		
		// to be set by the Resolver
		public int numAncestors { get; set; }
		// to be set by the Resolver
		public List<CompositeType> ancestors;


		public CompositeType(ArrayList heritage, ObjectSection sec)
		{
			this.heritage = new List<String>(heritage.Cast<String>());

			section = sec;

			// to be filled during resolving
			ancestors = new List<CompositeType>(heritage.Count);
		}


		#region Accessors to declared Members

		/// <summary>
		/// Returns public and published methods
		/// </summary>
		public virtual IEnumerable<MethodDeclaration> GetPublicMethods()
		{
			return GetAllMethods(Scope.Public | Scope.Published);
		}

		/// <summary>
		/// Returns public and published members
		/// </summary>
		public virtual IEnumerable<Declaration> GetPublicMembers()
		{
			return GetAllMembers(Scope.Public | Scope.Published);
		}

		/// <summary>
		/// Returns public, protected and published methods
		/// </summary>
		public virtual IEnumerable<MethodDeclaration> GetInheritableMethods()
		{
			return GetAllMethods(Scope.Public | Scope.Published | Scope.Protected);
		}

		/// <summary>
		/// Returns public, protected and published members
		/// </summary>
		public virtual IEnumerable<Declaration> GetInheritableMembers()
		{
			return GetAllMembers(Scope.Public | Scope.Published | Scope.Protected);
		}

		/// <summary>
		/// Returns all members with given scope
		/// </summary>
		public virtual IEnumerable<Declaration> GetAllMembers(Scope s = (Scope) 0xffffff)
		{
			foreach (var d in section.Decls(s))
				yield return d;

			foreach (var a in ancestors)
				foreach (var d in a.GetAllMembers(s))
					yield return d;
		}

		/// <summary>
		/// Returns all methods in the given scopes
		/// </summary>
		public virtual IEnumerable<MethodDeclaration> GetAllMethods(Scope s = (Scope) 0xffffff)
		{
			foreach (var f in section.decls.Cast<MethodDeclaration>())
				if ((f.scope & s) != 0)
					yield return f;

			foreach (var a in ancestors)
				foreach (var d in a.GetAllMethods(s))
					yield return d;
		}

		/// <summary>
		/// Returns all fields in the given scopes
		/// </summary>
		public virtual IEnumerable<FieldDeclaration> GetAllFields(Scope s = (Scope) 0xffffff)
		{
			foreach (var f in section.fields.Cast<FieldDeclaration>())
				if ((f.scope & s) != 0)
					yield return f;

			foreach (var a in ancestors)
				foreach (var d in a.GetAllFields(s))
					yield return d;
		}

		/// <summary>
		/// Returns a member with the given name
		/// </summary>
		public virtual Declaration GetMember(String id)
		{
			Declaration d;
			if ((d = section.GetMember(id)) != null)
				return d;

			foreach (var a in ancestors)
				if ((d = a.GetMember(id)) != null)
					return d;

			return null;
		}

		/// <summary>
		/// Returns a method with the given name
		/// </summary>
		public virtual MethodDeclaration GetMethod(String id)
		{
			MethodDeclaration d;
			if ((d = section.GetMethod(id)) != null)
				return d;

			foreach (var a in ancestors)
				if ((d = a.GetMethod(id)) != null)
					return d;

			return null;
		}

		/// <summary>
		/// Returns a field with the given name
		/// </summary>
		public virtual FieldDeclaration GetField(String id)
		{
			FieldDeclaration d;
			if ((d = section.GetField(id)) != null)
				return d;

			foreach (var a in ancestors)
				if ((d = a.GetField(id)) != null)
					return d;

			return null;
		}

		#endregion
	}


	public class ClassType : CompositeType
	{
		public FieldDeclaration self;

		public ClassType(ArrayList heritage, ObjectSection seclist = null)
			: base(heritage, seclist)
		{
		}
	}

	public class InterfaceType : CompositeType
	{
		public UnaryExpression guid;

		public InterfaceType(ArrayList heritage, ObjectSection ssec = null, UnaryExpression guid = null)
			: base(heritage, ssec)
		{
			this.guid = guid;
		}
	}


	/// <summary>
	/// A reference to a class. 1 level of indirection to avoid circular dependencies
	/// </summary>
	public class ClassRefType : ClassType
	{
		public String qualifid;
		public ClassType reftype;


		#region Accessors to declared Members

		/// <summary>
		/// Returns public and published methods
		/// </summary>
		public override IEnumerable<MethodDeclaration> GetPublicMethods()
		{
			return reftype.GetPublicMethods();
		}

		/// <summary>
		/// Returns public and published members
		/// </summary>
		public override IEnumerable<Declaration> GetPublicMembers()
		{
			return reftype.GetPublicMembers();
		}

		/// <summary>
		/// Returns public, protected and published methods
		/// </summary>
		public override IEnumerable<MethodDeclaration> GetInheritableMethods()
		{
			return reftype.GetInheritableMethods();
		}

		/// <summary>
		/// Returns public, protected and published members
		/// </summary>
		public override IEnumerable<Declaration> GetInheritableMembers()
		{
			return reftype.GetInheritableMembers();
		}

		/// <summary>
		/// Returns all methods
		/// </summary>
		public override IEnumerable<MethodDeclaration> GetAllMethods(Scope s = (Scope) 0xffffff)
		{
			return reftype.GetAllMethods(s);
		}

		/// <summary>
		/// Returns all fields
		/// </summary>
		public override IEnumerable<FieldDeclaration> GetAllFields(Scope s = (Scope) 0xffffff)
		{
			return reftype.GetAllFields(s);
		}

		/// <summary>
		/// Returns all members
		/// </summary>
		public override IEnumerable<Declaration> GetAllMembers(Scope s = (Scope) 0xffffff)
		{
			return reftype.GetAllMembers(s);
		}

		/// <summary>
		/// Returns a member with the given name
		/// </summary>
		public override Declaration GetMember(String id)
		{
			return reftype.GetMember(id);
		}

		/// <summary>
		/// Returns a method with the given name
		/// </summary>
		public override MethodDeclaration GetMethod(String id)
		{
			return reftype.GetMethod(id);
		}

		/// <summary>
		/// Returns a field with the given name
		/// </summary>
		public override FieldDeclaration GetField(String id)
		{
			return reftype.GetField(id);
		}

		#endregion

		
		public ClassRefType(ClassType reftype)
			: base(new ArrayList())
		{
			this.qualifid = reftype.Name;
			this.reftype = reftype;
		}

		public ClassRefType(String qualifid, ClassType reftype = null)
			: base(new ArrayList())
		{
			this.qualifid = qualifid;
			this.reftype = reftype;
		}
	}

	#endregion


	#region Composite Sections

	//
	// Composite sections
	//

	public enum Scope
	{
		Public = 9000,
		Protected,
		Private,
		Published
	}

	public interface IScopedDeclaration
	{
		void SetScope(Scope s);

		Scope GetScope();
	}

	public class ObjectSection : Section
	{
		public DeclarationList fields;

		public DeclarationList properties;

		// In the baseclass, 'decls' field
	//	public DeclarationList methods;

		public ObjectSection(DeclarationList fs = null, DeclarationList decls = null, Scope s = Scope.Published)
			: base(new DeclarationList())
 		{
			fields = fs;
			if (fields == null)
				fields = new DeclarationList();
			properties = new DeclarationList();

			if (Enum.IsDefined(typeof(Scope), s))
				AddDecls(decls, s);	// methods and properties

			if (Enum.IsDefined(typeof(Scope), s))
				foreach (FieldDeclaration d in fields)
					d.scope = s;
		}


		#region Adders

		//
		// Utilities: add declarations with a given scope
		//

		public void Add(ObjectSection sec)
		{
			if (sec == null)
				return;

			fields.Add(sec.fields);
			decls.Add(sec.decls);
			properties.Add(sec.properties);
		}

		public void AddFields(DeclarationList fs, Scope s)
		{
			if (fs != null)
				foreach (FieldDeclaration d in fs)
					d.scope = s;
			fields.Add(fs);
		}

		public void AddMethods(DeclarationList fs, Scope s)
		{
			if (fs != null)
				foreach (MethodDeclaration d in fs)
					d.scope = s;
			decls.Add(fs);
		}

		public void AddProperties(DeclarationList fs, Scope s)
		{
			if (fs != null)
				foreach (PropertyDeclaration d in fs)
					d.scope = s;
			properties.Add(fs);
		}

		/// <summary>
		/// Add unknown-type declarations
		/// </summary>
		public void AddDecls(DeclarationList fs, Scope s)
		{
			if (fs == null)
				return;

			foreach (IScopedDeclaration d in fs)
				d.SetScope(s);

			foreach (var d in fs)
			{
				if (d is MethodDeclaration)
					decls.Add(d);
				else if (d is FieldDeclaration)	// a property is a field too
					fields.Add(d);
			}
		}

		#endregion


		#region Accessors

		/// <summary>
		/// Fields, Methods and Properties
		/// </summary>
		public IEnumerable<Declaration> Decls(Scope s = (Scope) 0xffffff)
		{
			foreach (var f in fields.Cast<FieldDeclaration>().Where(f => (f.scope & s) != 0))
				yield return f;
			foreach (var d in decls.Cast<MethodDeclaration>().Where(d => (d.scope & s) != 0))
				yield return d;
			foreach (var p in properties.Cast<PropertyDeclaration>().Where(p => (p.scope & s) != 0))
				yield return p;
		}


		/// <summary>
		/// Returns a member with the given name
		/// </summary>
		public Declaration GetMember(String id)
		{
			Declaration d;
			if ((d = fields.GetDeclaration(id)) != null)
				return d;
			if ((d = decls.GetDeclaration(id)) != null)
				return d;
			if ((d = properties.GetDeclaration(id)) != null)
				return d;
			return null;
		}

		/// <summary>
		/// Returns a method with the given name
		/// </summary>
		public MethodDeclaration GetMethod(String id)
		{
			return decls.GetDeclaration(id) as MethodDeclaration;
		}

		/// <summary>
		/// Returns a field with the given name
		/// </summary>
		public FieldDeclaration GetField(String id)
		{
			return fields.GetDeclaration(id) as FieldDeclaration;
		}

		/// <summary>
		/// Returns a property with the given name
		/// </summary>
		public PropertyDeclaration GetProperty(String id)
		{
			return properties.GetDeclaration(id) as PropertyDeclaration;
		}

		#endregion
	}

	#endregion


	#region Object Fields

	//
	// Object fields (in classes and records)
	//

	/// <summary>
	/// Composite or record field declaration
	/// </summary>
	public class FieldDeclaration : ValueDeclaration, IScopedDeclaration
	{
		public bool isStatic;

		public Scope scope;

		public void SetScope(Scope s)
		{
			scope = s;
		}

		public Scope GetScope()
		{
			return scope;
		}

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
		public Expression tagvalue;
		public RecordType fields;

		public VarEntryDeclaration(Expression tagvalue, DeclarationList fields)
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