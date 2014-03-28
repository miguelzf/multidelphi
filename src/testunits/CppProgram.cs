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

			args = new string[2];
            //args[0] = "d:\\code\\crosspascal\\tests\\test_constant_folding.dpr";
			//args[0] = "d:\\code\\crosspascal\\tests\\test_function_pointers.dpr";
			args[0] = "d:\\code\\crosspascal\\tests\\test_classes.dpr";
			args[1] = "d:\\code\\crosspascal\\tests\\unit1.pas";		
            //args[0] = "d:\\code\\crosspascal\\tests\\test_control_structures.dpr";

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

				NameResolver resolver = new NameResolver();
				resolver.Reset(sf);
				resolver.StartProcessing(tree);

				Processor constfolder = new ConstantFolder();
				constfolder.StartProcessing(tree);

				Processor myProcessor = new CppCodegen(resolver.nameReg);
				Console.WriteLine("Now compiling...");
				myProcessor.StartProcessing(tree);

				Console.WriteLine(myProcessor.ToString());
			}

			Console.WriteLine("Done!");
			Console.ReadLine();

		}
	}

}