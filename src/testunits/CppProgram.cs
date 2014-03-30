using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Text;
using crosspascal.core;
using crosspascal.ast;
using crosspascal.ast.nodes;
using crosspascal.parser;
using crosspascal.cpp;
using crosspascal.semantics;

namespace crosspascal.testunits
{

	class CppProgram
	{
		static void Main(string[] args)
		{
			var sw = new Stopwatch();

			Console.WriteLine("CrossPascal Delphi compiler");

			TranslationPlanner planner;
			DelphiParser parser;

			parser = new DelphiParser(0);
			planner = new TranslationPlanner(null);

			planner.LoadIncludePaths("include-paths.txt");


			planner.LoadFiles(args);

			// TestReadAll(args);
			foreach (SourceFile sf in planner.GetSourceFiles())
			{
				Console.Write("####### Compile file " + Path.GetFileName(sf.name) + ": ");

				if (sf.preprocText == null)		// preprocessing failed
					break;

				StringReader sr = new StringReader(sf.preprocText);

				TranslationUnit tree;


				try
				{
					tree = parser.Parse(sr);
					Console.WriteLine("Parsed OK: " + tree.name + " " + tree.ToString());
				}
				catch (ParserException e)
				{
					Console.Error.WriteLine(e);
					Console.Error.WriteLine("Parsing failed");
					break;
				}

				ParentProcessor pp = new ParentProcessor();
				pp.StartProcessing(tree);

				NameResolver nr = new NameResolver();
				nr.Reset(sf);
				nr.StartProcessing(tree);
				
				AstPrinter astPrinter = new AstPrinter();
				astPrinter.StartProcessing(tree);
				Console.WriteLine(astPrinter);

				Processor constfolder = new ConstantFolder();
				constfolder.StartProcessing(tree);

				DeclarationsRegistry reg = nr.nameReg;
				reg.InitEnvironment();

				Processor myProcessor = new CppCodegen(reg);
				Console.WriteLine("Now compiling...");
				myProcessor.StartProcessing(tree);

				Console.WriteLine(myProcessor.ToString());
			}

			Console.WriteLine("Done!");
			Console.ReadLine();

		}
	}

}