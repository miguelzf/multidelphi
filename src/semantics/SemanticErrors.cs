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
		const string DefaultMsg = "Identifier already declared: ";

		public IdentifierRedeclared(int lineno, string idname) : base(lineno, DefaultMsg + idname) { }

		public IdentifierRedeclared(string idname) : base(DefaultMsg+idname) { }
	}

	public class IdentifierUndeclared : SemanticException
	{
		const string DefaultMsg = "Undeclared Identifier: ";

		public IdentifierUndeclared(int lineno, string idname) : base(lineno, DefaultMsg + idname) { }

		public IdentifierUndeclared(string idname) : base(DefaultMsg + idname) { }
	}

	public class InvalidIdentifier : SemanticException
	{
		const string DefaultMsg = "Invalid Identifier";

		public InvalidIdentifier(int lineno, string message = DefaultMsg) : base (lineno,message) { }

		public InvalidIdentifier(string message = DefaultMsg) : base(message) { }
	}




	public class TypeException : SemanticException
	{
		const string DefaultMsg = "Incompatible Types ";

		public TypeException(int lineno, string message = DefaultMsg) : base (lineno,message) { }

		public TypeException(string message = DefaultMsg) : base(message) { }
	}



}
