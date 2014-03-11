using System;
using System.Collections;
using System.IO;
using System.Text;

namespace crosspascal.parser
{	
	class LexErrorMessages
	{
		const string UnkownChar = "Unknown character";
		
		const string InvalidInteger = "Invalid Integer Format";
		const string InvalidReal = "Invalid Real Format";
		const string UnterminatedComment = "Block Comment not closed";
		
		// TODO all the Scanner's error messages here
	}
}