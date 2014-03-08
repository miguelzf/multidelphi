using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using crosspascal.cpp;
using crosspascal.ast;
using crosspascal.parser;

namespace crosspascal
{
	class Program
	{
		static void Main(string[] args)
		{
			string test = "Program bla; {$DEFINE foo} const a = 2; {$IFDEF FOO} const b = 3; {$ENDIF}"
				+ "{$IFDEF BAR} const c = 4;{$ENDIF} {$IFDEF BAR} LOL = 3; {$ELSE} LOL = 4; {$ENDIF}";

			Console.ReadLine();

			/*RuleType _ruleType = RuleType.Goal;
			try
			{
				string fileName = "tests/basic_test.dpr";
				string text = File.ReadAllText(fileName);
				Parser parser = Parser.FromText(text, fileName, CompilerDefines.CreateStandard(), new MemoryFileLoader());
				AstNode tree = parser.ParseRule(_ruleType);
				
				Console.WriteLine(tree.Inspect());

				CppCodegen proc = new CppCodegen();
				proc.traverse = new VisitorTraverser(proc).traverse;
				proc.Visit(tree);
				
				Console.WriteLine("Done!");

			}
			catch (DGrokException ex)
			{
				string error = "Filename: " + ex.Location.FileName + Environment.NewLine +
					"Offset: " + ex.Location.Offset + Environment.NewLine +
					ex.Message;
				Console.WriteLine(error);
			}
			Console.ReadLine();
			*/
		}
	}
}

