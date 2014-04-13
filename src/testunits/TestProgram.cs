using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using MultiPascal.AST;
using MultiPascal.AST.Nodes;
using MultiPascal.utils;
using MultiPascal.Parser;
using MultiPascal.Semantics;


namespace MultiPascal.core
{
	class TestProgram
	{

		static void Main(string[] args)
		{
		//	RuleType _ruleType = RuleType.Goal;
			Logger.log.AddFile("compiler-log.txt");		// general log
			Logger.log.UseConsole(true);

			try
			{
				string fileName = "tests/test_record_with_methods.dpr";
				string text = File.ReadAllText(fileName);
				// Parser parser = Parser.FromText(text, fileName, CompilerDefines.CreateStandard(), new MemoryFileLoader());
			//	Node tree = parser.ParseRule(_ruleType);
		/*
				CppCodegen proc = new CppCodegen();
				proc.traverse = new VisitorTraverser(proc).traverse;
				proc.Visit(tree);
		*/	
			//	String treestr = printTree(tree, typeof(MapTraverser), 4);
			/*	PrintAST proc = new PrintAST();
				new MapTraverser(proc);
				proc.Visit(tree);
				String treestr = proc.ToString();
			*/	
			//	Logger.log.Write(tree.Inspect());
			}
			catch (Exception)
			{
			// string error = "Filename: " + ex.Location.FileName + Environment.NewLine + "Offset: " + ex.Location.Offset + Environment.NewLine + ex.Message;
			//	Console.WriteLine(error);
			}
			Console.ReadLine();

		}



		// Usage example of processors and traversers

		static string printTree(Node tree, System.Type traverserType, int method)
		{
			AstPrinter proc = null;

			switch (method)
			{	// Possible methods to create a processor:

				// use traverser object
				case 1:
					Traverser<bool> vtrav = (Traverser<bool>)Activator.CreateInstance(traverserType);
					proc = new AstPrinter(vtrav);
					vtrav.Processor = proc;
					break;

				// use 2 steps
				case 2:
					proc = new AstPrinter();
					Activator.CreateInstance(traverserType, proc);
					break;

				// explicitly set traverser
				case 3:
					proc = new AstPrinter();
					Traverser<bool> trav = (Traverser<bool>)Activator.CreateInstance(traverserType, proc);
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
