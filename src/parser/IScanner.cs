using System;

namespace MultiPascal.Parser
{
	// must be implemented by a scanner object to supply input to the parser.

	internal interface IScanner
	{
		/** move on to next token.
			@return false if positioned beyond tokens.
			@throws IOException on input error.
		*/
		bool advance (); // throws java.io.IOException;
		/** classifies current token.
			Should not be called if advance() returned false.
			@return current %token or single character.
		*/
		int token ();
		/** associated with current token.
			Should not be called if advance() returned false.
			@return value for token().
		*/
		Object value ();
		/**	Return line number of last scanned token
			@return line number
		*/
		int yylineno();
	}

}
