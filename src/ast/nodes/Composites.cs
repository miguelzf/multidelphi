using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MultiPascal.Semantics;

namespace MultiPascal.AST.Nodes
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

		public ObjectSection section;

		public bool IsPacked { get; set; }

		public bool IsForward { get { return section == null; } }

		// optional
		public String Name { get; set; }
		

		//
		// Resolving data
		// To be set and used by the resolver
		// 

		public int numAncestors { get; set; }

		public List<CompositeType> ancestors;
	
		/// <summary>
		/// Context with inheritable members (non-private)
		/// </summary>
		internal SymbolContext<Declaration, Section> inheritableContext { get; set; }
		
		/// <summary>
		/// Context with all members (including private)
		/// </summary>
		private SymbolContext<Declaration, Section> _privateContext;
		internal SymbolContext<Declaration, Section> privateContext
		{
			get { return _privateContext; }
			set
			{
				_privateContext = value;
				_privateContext.Id = _privateContext.Id + " private";
			}
		}


		public CompositeType(ArrayList inherits, ObjectSection sec)
		{
			if (inherits == null)
				heritage = new List<String>();
			else
				heritage = new List<String>(inherits.Cast<String>());

			section = sec;
			if (!IsForward)
			{
				section.declaringObject = this;
				foreach (var d in section.Decls().Cast<IScopedDeclaration>())
					d.SetDeclaringObject(this);
			}

			// to be filled during resolving
			ancestors = new List<CompositeType>(heritage.Count);
		}


		#region Accessors of Public Members

		/// <summary>
		/// Returns public and published members
		/// </summary>
		public virtual IEnumerable<Declaration> GetPublicMembers()
		{
			return GetAllMembers(Scope.Public | Scope.Published);
		}

		/// <summary>
		/// Returns public and published methods
		/// </summary>
		public virtual IEnumerable<MethodDeclaration> GetPublicMethods()
		{
			return GetAllMethods(Scope.Public | Scope.Published);
		}

		/// <summary>
		/// Returns public and published methods
		/// </summary>
		public virtual IEnumerable<FieldDeclaration> GetPublicFields()
		{
			return GetAllFields(Scope.Public | Scope.Published);
		}

		/// <summary>
		/// Returns public and published methods
		/// </summary>
		public virtual IEnumerable<PropertyDeclaration> GetPublicProperties()
		{
			return GetAllProperties(Scope.Public | Scope.Published);
		}

		#endregion	// Accessors of Public Members


		#region Accessors of Own Members

		/// <summary>
		/// Returns all own members with the given scope
		/// </summary>
		public virtual IEnumerable<Declaration> GetOwnMembers(Scope s = (Scope) 0xffffff)
		{
			if (!IsForward)
				return section.Decls(s);
			else
				return DeclarationList.Empty.Cast<MethodDeclaration>();
		}

		/// <summary>
		/// Returns all own methods with the given scope
		/// </summary>
		public virtual IEnumerable<MethodDeclaration> GetOwnMethods(Scope s = (Scope) 0xffffff)
		{
			if (!IsForward)
				return section.fields.Cast<MethodDeclaration>().Where(f => (f.scope & s) != 0);
			else
				return DeclarationList.Empty.Cast<MethodDeclaration>();
		}

		/// <summary>
		/// Returns all own fields with the given scope
		/// </summary>
		public virtual IEnumerable<FieldDeclaration> GetOwnFields(Scope s = (Scope) 0xffffff)
		{
			if (!IsForward)
				return section.fields.Cast<FieldDeclaration>().Where(f => (f.scope & s) != 0);
			else
				return DeclarationList.Empty.Cast<FieldDeclaration>();
		}

		/// <summary>
		/// Returns all own fields with the given scope
		/// </summary>
		public virtual IEnumerable<PropertyDeclaration> GetOwnProperties(Scope s = (Scope) 0xffffff)
		{
			if (!IsForward)
				return section.fields.Cast<PropertyDeclaration>().Where(f => (f.scope & s) != 0);
			else
				return DeclarationList.Empty.Cast<PropertyDeclaration>();
		}

		#endregion	// own members


		#region Accessors of Inherited Members

		const Scope nonPrivateScope = Scope.Protected | Scope.Public | Scope.Published;

		/// <summary>
		/// Returns inherited (public, protected and published) members
		/// </summary>
		public virtual IEnumerable<Declaration> GetInheritedMembers()
		{
			foreach (var a in ancestors)
				foreach (var d in a.GetAllMembers(nonPrivateScope))
					yield return d;
		}

		/// <summary>
		/// Returns inherited (public, protected and published) methods
		/// </summary>
		public virtual IEnumerable<MethodDeclaration> GetInheritedMethods()
		{
			foreach (var a in ancestors)
				foreach (var d in a.GetAllMethods(nonPrivateScope))
					yield return d;
		}

		/// <summary>
		/// Returns inherited (public, protected and published) methods
		/// </summary>
		public virtual IEnumerable<FieldDeclaration> GetInheritedFields()
		{
			foreach (var a in ancestors)
				foreach (var d in a.GetAllFields(nonPrivateScope))
					yield return d;
		}

		/// <summary>
		/// Returns inherited (public, protected and published) properties
		/// </summary>
		public virtual IEnumerable<PropertyDeclaration> GetInheritedProperties()
		{
			foreach (var a in ancestors)
				foreach (var d in a.GetAllProperties(nonPrivateScope))
					yield return d;
		}

		#endregion	// Accessors of Inherited Members


		#region Accessors of Accessible Members

		// Iterate over inherited members first, then own members

		/// <summary>
		/// Returns own and inherited (public, protected and published) members
		/// </summary>
		public virtual IEnumerable<Declaration> GetAccessibleMembers()
		{
			foreach (var d in GetInheritedMembers())
				yield return d;

			if (!IsForward)
				foreach (var d in GetOwnMembers())
					yield return d;
		}

		/// <summary>
		/// Returns own and inherited (public, protected and published) methods
		/// </summary>
		public virtual IEnumerable<MethodDeclaration> GetAccessibleMethods()
		{
			foreach (var d in GetInheritedMethods())
				yield return d;

			if (!IsForward)
				foreach (var d in GetOwnMethods())
					yield return d;
		}

		/// <summary>
		/// Returns own and inherited (public, protected and published) fields
		/// </summary>
		public virtual IEnumerable<FieldDeclaration> GetAccessibleFields()
		{
			foreach (var d in GetInheritedFields())
				yield return d;

			if (!IsForward)
				foreach (var d in GetOwnFields())
					yield return d;
		}

		/// <summary>
		/// Returns own and inherited (public, protected and published) members
		/// </summary>
		public virtual IEnumerable<PropertyDeclaration> GetAccessibleProperties()
		{
			foreach (var d in GetInheritedProperties())
				yield return d;

			if (!IsForward)
				foreach (var d in GetOwnProperties())
					yield return d;
		}

		#endregion	// Accessible members


		#region Accessors of All Members

		// Iterate over inherited members first, then own members

		/// <summary>
		/// Returns all members with given scope, including inherited
		/// </summary>
		public virtual IEnumerable<Declaration> GetAllMembers(Scope s = (Scope) 0xffffff)
		{
			foreach (var a in ancestors)
				foreach (var d in a.GetAllMembers(s))
					yield return d;

			if (!IsForward)
				foreach (var f in section.Decls(s))
					yield return f;
		}

		/// <summary>
		/// Returns all methods in the given scopes, including inherited
		/// </summary>
		public virtual IEnumerable<MethodDeclaration> GetAllMethods(Scope s = (Scope) 0xffffff)
		{
			foreach (var a in ancestors)
				foreach (var d in a.GetAllMethods(s))
					yield return d;

			if (!IsForward)
				foreach (var f in section.decls.Cast<MethodDeclaration>())
					if ((f.scope & s) != 0)
						yield return f;
		}

		/// <summary>
		/// Returns all fields in the given scopes, including inherited
		/// </summary>
		public virtual IEnumerable<FieldDeclaration> GetAllFields(Scope s = (Scope) 0xffffff)
		{
			foreach (var a in ancestors)
				foreach (var d in a.GetAllFields(s))
					yield return d;

			if (!IsForward)
				foreach (var f in section.fields.Cast<FieldDeclaration>())
					if ((f.scope & s) != 0)
						yield return f;
		}

		/// <summary>
		/// Returns all fields in the given scopes, including inherited
		/// </summary>
		public virtual IEnumerable<PropertyDeclaration> GetAllProperties(Scope s = (Scope) 0xffffff)
		{
			foreach (var a in ancestors)
				foreach (var d in a.GetAllProperties(s))
					yield return d;

			if (!IsForward)
				foreach (var f in section.properties.Cast<PropertyDeclaration>())
					if ((f.scope & s) != 0)
						yield return f;
		}

		#endregion		// Accessors of All Members


		#region Accessors of individual Members

		/// <summary>
		/// Returns a member with the given name
		/// </summary>
		public virtual Declaration GetMember(String id)
		{
			Declaration d;
			if (!IsForward)
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
			if (!IsForward)
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
			if (!IsForward)
				if ((d = section.GetField(id)) != null)
					return d;

			foreach (var a in ancestors)
				if ((d = a.GetField(id)) != null)
					return d;

			return null;
		}

		/// <summary>
		/// Returns a member with the given name
		/// </summary>
		public virtual Declaration GetInheritableMember(String id)
		{
			var decl = GetMember(id) as IScopedDeclaration;
			if (decl == null || decl.GetScope() == Scope.Private)
				return null;
			return decl as Declaration;
		}

		/// <summary>
		/// Returns a method with the given name
		/// </summary>
		public virtual MethodDeclaration GetInheritableethod(String id)
		{
			var decl = GetMethod(id) as MethodDeclaration;
			if (decl == null || decl.GetScope() == Scope.Private)
				return null;
			return decl;
		}

		/// <summary>
		/// Returns a field with the given name
		/// </summary>
		public virtual FieldDeclaration GetInheritableField(String id)
		{
			var decl = GetField(id) as FieldDeclaration;
			if (decl == null || decl.GetScope() == Scope.Private)
				return null;
			return decl;
		}

		#endregion	// Accessors of individual Members

	}


	public class ClassType : CompositeType
	{
		public FieldDeclaration self;

		public ClassType(ArrayList heritage, ObjectSection sec = null)
			: base(heritage, sec)
		{
			if (!IsForward)
			{
				self = new FieldDeclaration("self", new ClassRefType(this.Name, this));
				self.SetScope(Scope.Protected);
				section.fields.Add(self);
			}
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


		#region Accessors of Public Members

		/// <summary>
		/// Returns public and published members
		/// </summary>
		public override IEnumerable<Declaration> GetPublicMembers()
		{
			return reftype.GetAllMembers(Scope.Public | Scope.Published);
		}

		/// <summary>
		/// Returns public and published methods
		/// </summary>
		public override IEnumerable<MethodDeclaration> GetPublicMethods()
		{
			return reftype.GetAllMethods(Scope.Public | Scope.Published);
		}

		/// <summary>
		/// Returns public and published methods
		/// </summary>
		public override IEnumerable<FieldDeclaration> GetPublicFields()
		{
			return reftype.GetAllFields(Scope.Public | Scope.Published);
		}

		/// <summary>
		/// Returns public and published methods
		/// </summary>
		public override IEnumerable<PropertyDeclaration> GetPublicProperties()
		{
			return reftype.GetAllProperties(Scope.Public | Scope.Published);
		}

		#endregion	// Accessors of Public Members

		#region Accessors of Own Members

		/// <summary>
		/// Returns all own members with the given scope
		/// </summary>
		public override IEnumerable<Declaration> GetOwnMembers(Scope s = (Scope) 0xffffff)
		{
			return reftype.GetOwnMembers(s);
		}

		/// <summary>
		/// Returns all own methods with the given scope
		/// </summary>
		public override IEnumerable<MethodDeclaration> GetOwnMethods(Scope s = (Scope) 0xffffff)
		{
			return reftype.GetOwnMethods(s);
		}

		/// <summary>
		/// Returns all own fields with the given scope
		/// </summary>
		public override IEnumerable<FieldDeclaration> GetOwnFields(Scope s = (Scope) 0xffffff)
		{
			return reftype.GetOwnFields(s);
		}

		/// <summary>
		/// Returns all own fields with the given scope
		/// </summary>
		public override IEnumerable<PropertyDeclaration> GetOwnProperties(Scope s = (Scope) 0xffffff)
		{
			return reftype.GetOwnProperties(s);
		}

		#endregion	// own members
		
		#region Accessors of Inherited Members

		/// <summary>
		/// Returns inherited (public, protected and published) members
		/// </summary>
		public override IEnumerable<Declaration> GetInheritedMembers()
		{
			return reftype.GetInheritedMembers();
		}

		/// <summary>
		/// Returns inherited (public, protected and published) methods
		/// </summary>
		public override IEnumerable<MethodDeclaration> GetInheritedMethods()
		{
			return reftype.GetInheritedMethods();
		}

		/// <summary>
		/// Returns inherited (public, protected and published) methods
		/// </summary>
		public override IEnumerable<FieldDeclaration> GetInheritedFields()
		{
			return reftype.GetInheritedFields();
		}

		/// <summary>
		/// Returns inherited (public, protected and published) properties
		/// </summary>
		public override IEnumerable<PropertyDeclaration> GetInheritedProperties()
		{
			return reftype.GetInheritedProperties();
		}

		#endregion	// Accessors of Inherited Members

		#region Accessors of Accessible Members

		// Iterate over inherited members first, then own members

		/// <summary>
		/// Returns own and inherited (public, protected and published) members
		/// </summary>
		public override IEnumerable<Declaration> GetAccessibleMembers()
		{
			return reftype.GetAccessibleProperties();
		}

		/// <summary>
		/// Returns own and inherited (public, protected and published) methods
		/// </summary>
		public override IEnumerable<MethodDeclaration> GetAccessibleMethods()
		{
			return reftype.GetAccessibleMethods();
		}

		/// <summary>
		/// Returns own and inherited (public, protected and published) fields
		/// </summary>
		public override IEnumerable<FieldDeclaration> GetAccessibleFields()
		{
			return reftype.GetAccessibleFields();
		}

		/// <summary>
		/// Returns own and inherited (public, protected and published) members
		/// </summary>
		public override IEnumerable<PropertyDeclaration> GetAccessibleProperties()
		{
			return reftype.GetAccessibleProperties();
		}

		#endregion	// Accessible members

		#region Accessors of All Members

		// Iterate over inherited members first, then own members

		/// <summary>
		/// Returns all members with given scope, including inherited
		/// </summary>
		public override IEnumerable<Declaration> GetAllMembers(Scope s = (Scope) 0xffffff)
		{
			return reftype.GetAllMembers(s);
		}

		/// <summary>
		/// Returns all methods in the given scopes, including inherited
		/// </summary>
		public override IEnumerable<MethodDeclaration> GetAllMethods(Scope s = (Scope) 0xffffff)
		{
			return reftype.GetAllMethods(s);
		}

		/// <summary>
		/// Returns all fields in the given scopes, including inherited
		/// </summary>
		public override IEnumerable<FieldDeclaration> GetAllFields(Scope s = (Scope) 0xffffff)
		{
			return reftype.GetAccessibleProperties();
		}

		/// <summary>
		/// Returns all fields in the given scopes, including inherited
		/// </summary>
		public override IEnumerable<PropertyDeclaration> GetAllProperties(Scope s = (Scope) 0xffffff)
		{
			return reftype.GetAllProperties(s);
		}

		#endregion		// Accessors of All Members

		#region Accessors of individual Members

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

		#endregion	// Accessors of individual Members

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

		CompositeType GetDeclaringObject();

		void SetDeclaringObject(CompositeType d);
	}

	public class ObjectSection : Section
	{
		public DeclarationList fields;

		public DeclarationList properties;

		public CompositeType declaringObject { get; set; }

		// Kept in the baseclass, 'decls' list
	//	public DeclarationList methods;

		public ObjectSection(DeclarationList fs = null, DeclarationList ds = null, Scope s = Scope.Published)
			: base(new DeclarationList())
 		{
			fields = fs;
			if (fields == null)
				fields = new DeclarationList();
			properties = new DeclarationList();

			AddDecls(ds, s);	// methods and properties

			if (Enum.IsDefined(typeof(Scope), s))
				foreach (FieldDeclaration d in fields)
					d.SetScope(s);
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
					d.SetScope(s);
			fields.Add(fs);
		}

		public void AddMethods(DeclarationList fs, Scope s)
		{
			if (fs != null)
				foreach (MethodDeclaration d in fs)
					d.SetScope(s);
			decls.Add(fs);
		}

		public void AddProperties(DeclarationList fs, Scope s)
		{
			if (fs != null)
				foreach (PropertyDeclaration d in fs)
					d.SetScope(s);
			properties.Add(fs);
		}

		/// <summary>
		/// Add unknown-type declarations
		/// </summary>
		public void AddDecls(DeclarationList fs, Scope s = 0)
		{
			if (fs == null)
				return;

			if (s != 0)
				foreach (IScopedDeclaration d in fs)
					d.SetScope(s);

			foreach (var d in fs)
			{
				if (d is MethodDeclaration)
					decls.Add(d);
				else if (d is PropertyDeclaration)	// a property is a field too
					properties.Add(d);
				else if (d is FieldDeclaration)	// a property is a field too
					fields.Add(d);
				else
					ErrorInternal("Unknown Declaration in ObjectSection AddDecls");
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

		#region IScopedDeclaration implementation

		internal Scope scope;
		public void SetScope(Scope s)
		{
			scope = s;
		}
		public Scope GetScope()
		{
			return scope;
		}

		private CompositeType declaringObject;
		public CompositeType GetDeclaringObject()
		{
			return declaringObject;
		}
		public void SetDeclaringObject(CompositeType t)
		{
			declaringObject = t;
		}

		#endregion

		public FieldDeclaration(String id, TypeNode t = null, bool isStatic =false)
			: base(id, t)
		{
			this.isStatic = false;
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