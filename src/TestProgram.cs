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
using crosspascal.utils;


namespace crosspascal
{
	class TestProgram
	{

		static void Main(string[] args)
		{
			RuleType _ruleType = RuleType.Goal;
			Logger.log.AddFile("compiler-log.txt");		// general log
			Logger.log.UseConsole(true);

			try
			{
				string fileName = "tests/test_record_with_methods.dpr";
				string text = File.ReadAllText(fileName);
				Parser parser = Parser.FromText(text, fileName, CompilerDefines.CreateStandard(), new MemoryFileLoader());
				AstNode tree = parser.ParseRule(_ruleType);

			//	String treestr = printTree(tree, typeof(MapTraverser), 4);
			/*	PrintAST proc = new PrintAST();
				new MapTraverser(proc);
				proc.Visit(tree);
				String treestr = proc.ToString();
			*/	
				Logger.log.Write(tree.Inspect());
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



		// Usage example of processors and traversers

		static string printTree(AstNode tree, System.Type traverserType, int method)
		{
			PrintAST proc;

			// Initialize object:
			// new Traverser(arg) == (Traverser)  Activator.CreateInstance(type,proc);

			switch (method)
			{	// Possible methods to create a processor:

				// use traverser type
				case 1:
					proc = new PrintAST(traverserType);
					break;

				// use traverser object
				case 2:
					Traverser vtrav = (Traverser)Activator.CreateInstance(traverserType);
					proc = new PrintAST(vtrav);
					vtrav.Processor = proc;
					break;

				// use 2 steps
				case 3:
					proc = new PrintAST();
					Activator.CreateInstance(traverserType, proc);
					break;

				// explicitly set traverser
				case 4:
					proc = new PrintAST();
					Traverser trav = (Traverser)Activator.CreateInstance(traverserType, proc);
					trav.Processor = proc;
					proc.traverse = trav.traverse;
					break;

				default:
					return "";
			}

			proc.Visit(tree);
			return proc.ToString();
		}

	}
}
