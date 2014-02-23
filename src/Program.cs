using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using DGrok.DelphiNodes;
using DGrok.Framework;
using DGrok.Visitors;

namespace crosspascal
{
	class Program
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
				Console.WriteLine(tree.Inspect());
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

