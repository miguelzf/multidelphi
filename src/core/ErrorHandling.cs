using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace crosspascal.core
{
	public class CrossPascalException : Exception
	{
		const string DefaultMsg = "Compilation Error";
	
		public CrossPascalException (int lineno, string message = DefaultMsg)
			: base (message + " in line " + lineno) { }

		public CrossPascalException(string message = DefaultMsg)
			: base (message) { }
	}

	public class AstNodeException : CrossPascalException
	{
		const string DefaultMsg = "Error creating an AST Node";

		public AstNodeException (int lineno, string message = DefaultMsg) : base (lineno,message) { }
		
		public AstNodeException (string message = DefaultMsg) : base (message) { }
	}


}
