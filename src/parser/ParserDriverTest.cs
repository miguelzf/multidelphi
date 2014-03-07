using System;
using System.Collections;
using System.IO;
using System.Text;

namespace crosspascal.parser
{

	class ParserDriverTest
	{
		public static void Main(string[] args)
		{
			DelphiParser parser = new DelphiParser();
			parser.LoadIncludePaths("include-paths.txt");
			
			try {
				foreach (string s in args)
				{
					Console.Error.Write("PARSE file " + Path.GetFileName(s) + ": ");

					Object tree = parser.Parse(s, new yydebug.yyErrorTrace());
					Console.Error.WriteLine("Parsing finished ok");
				}
			} catch (Exception e) {
				Console.Error.WriteLine(e);
				Console.Error.WriteLine("Parsing failed");
			}
		}
	}

}

