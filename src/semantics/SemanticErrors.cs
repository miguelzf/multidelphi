using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using crosspascal.core;

namespace crosspascal.semantics
{
	public class SemanticException : CrossPascalException
	{
		const string DefaultMsg = "Semantic Error";

		public SemanticException (int lineno, string message = DefaultMsg) : base (lineno,message) { }

		public SemanticException(string message = DefaultMsg) : base(message) { }
	}


	public class IdentifierRedeclared : SemanticException
	{
		const string DefaultMsg = "Identifier already declared";

		public IdentifierRedeclared(int lineno, string idname) : base(lineno, DefaultMsg + ": "+ idname) { }

		public IdentifierRedeclared(string idname) : base(DefaultMsg + ": " + idname) { }
	}

	public class InvalidIdentifier : SemanticException
	{
		const string DefaultMsg = "Invalid Identifier";

		public InvalidIdentifier(int lineno, string message = DefaultMsg) : base (lineno,message) { }

		public InvalidIdentifier(string message = DefaultMsg) : base(message) { }
	}

	public class DeclarationNotFound : SemanticException
	{
		const string DefaultMsg = "Undeclared Identifier: ";

		public DeclarationNotFound(int lineno, string idname) : base(lineno, DefaultMsg + idname) { }

		public DeclarationNotFound(string idname) : base(DefaultMsg + idname) { }
	}


	public class CompositeNotFound : DeclarationNotFound
	{
		const string DefaultMsg = "Class or Interface not found";

		public CompositeNotFound(int lineno, string idname) : base(lineno, DefaultMsg + ": "+ idname) { }

		public CompositeNotFound(string idname) : base(DefaultMsg + ": " + idname) { }
	}

	public class MethordOrFieldNotFound : DeclarationNotFound
	{
		const string DefaultMsg = "No Method or Field found";

		public MethordOrFieldNotFound(int lineno, string idname) : base(lineno, DefaultMsg + ": "+ idname) { }

		public MethordOrFieldNotFound(string idname) : base(DefaultMsg + ": " + idname) { }
	}

	public class FieldNotFound : DeclarationNotFound
	{
		const string DefaultMsg = "No Method or Field found";

		public FieldNotFound(int lineno, string idname) : base(lineno, DefaultMsg + ": " + idname) { }

		public FieldNotFound(string idname) : base(DefaultMsg + ": " + idname) { }
	}


	#region Type Exceptions
	//
	// Type Exceptions
	//

	public class TypeException : SemanticException
	{
		const string DefaultMsg = "Incompatible Types ";

		public TypeException(int lineno, string message = DefaultMsg) : base (lineno,message) { }

		public TypeException(string message = DefaultMsg) : base(message) { }
	}

	public class TypeRequiredException : TypeException 
	{
		const string DefaultMsg = " type requied";

		public TypeRequiredException(int lineno, string idname) : base(lineno, idname + DefaultMsg) { }

		public TypeRequiredException(string idname) : base(idname + DefaultMsg) { }

		public TypeRequiredException(int lineno, Type type) : base(lineno, type.Name + DefaultMsg) { }

		public TypeRequiredException(Type type) : base(type.Name + DefaultMsg) { }
	}

	#endregion
}
