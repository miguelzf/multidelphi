using System;
using System.Collections;
using System.IO;
using System.Text;
using crosspascal.AST;

namespace crosspascal.parser
{

	class ParserDriverTest
	{
		public static void Main(string[] args)
		{
			DelphiParser parser = new DelphiParser();
			parser.eof_token = DelphiScanner.ScannerEOF;
				
			try {
				foreach (string s in args)
					parser.yyparse(	new DelphiScanner(new StreamReader(s)), new yydebug.yyErrorTrace());
				
			} catch (Exception e)
			{
				Console.Out.WriteLine(e);
				// printf("Parsing finished ok\n");
				// printf("Parsing failed\n");
			}
		}
	}


}

