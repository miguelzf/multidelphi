using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using DGrok.DelphiNodes;
using DGrok.Framework;
using DGrok.Visitors;
using crosspascal.AST;

namespace crosspascal
{
	class TestProgram
	{
		static void Main(string[] args)
		{
			RuleType _ruleType = RuleType.Goal;
			try
			{
				string fileName = "tests/test_record_with_methods.dpr";
				string text = File.ReadAllText(fileName);
				Parser parser = Parser.FromText(text, fileName, CompilerDefines.CreateStandard(), new MemoryFileLoader());
				AstNode tree = parser.ParseRule(_ruleType);
				
				PrintAST proc = new PrintAST();
				//new ASTVisitorTraverser(proc);
				proc.traverse = new ASTVisitorTraverser(proc).traverse;
				proc.Visit(tree);
				Console.WriteLine(proc.ToString());
			}
			catch (DGrokException ex)
			{
				string error = "Filename: " + ex.Location.FileName + Environment.NewLine +
					"Offset: " + ex.Location.Offset + Environment.NewLine +
					ex.Message;
				Console.WriteLine(error);
			}
			Console.ReadLine();

		}
	}
}
