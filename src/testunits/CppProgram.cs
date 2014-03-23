using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Text;
using crosspascal.ast;
using crosspascal.ast.nodes;
using crosspascal.parser;
using crosspascal.cpp;

namespace crosspascal.testunits
{

	class CppProgram
	{
		static void Main(string[] args)
		{
			var sw = new Stopwatch();

			Console.WriteLine("CrossPascal Delphi compiler");

			DelphiParser parser = new DelphiParser(0);
			DelphiPreprocessor preproc = new DelphiPreprocessor();
			preproc.LoadIncludePaths("include-paths.txt");

			CompilationUnit tree = null;

			args = new string[1];
            args[0] = "d:\\code\\crosspascal\\tests\\test_constant_folding.dpr";
            //args[0] = "d:\\code\\crosspascal\\tests\\test_control_structures.dpr";


			// TestReadAll(args);
			foreach (string s in args)
			{
				Console.Write("####### PARSE file " + Path.GetFileName(s) + ": ");

			//	preproc.ResetPreprocessor(s);
				preproc.AddDefine("LINUX");	// test
				try
				{
					preproc.Preprocess();
				}
				catch (PreprocessorException)
				{
					Console.Error.WriteLine("Parsing failed");
					continue;
				}

				string preprocfiletext = preproc.GetOutput().ToLowerInvariant();	// Delphi is case-insensitive
				StringReader sr = new StringReader(preprocfiletext);
				try
				{
			//		tree = (CompilationUnit)parser.Parse(sr);
					Console.WriteLine("Parsed OK: " + tree.name + " " + tree.ToString());
				}
				catch (ParserException e)
				{
					Console.Error.WriteLine(e);
					Console.Error.WriteLine("Parsing failed");
				}
			}

			sw.Stop();
			Console.WriteLine("READING from StringStream all files took " + (sw.ElapsedMilliseconds / 1000.0) + " secs");

           
            AstPrinter astPrinter = new AstPrinter();
            MapTraverser mt = new MapTraverser(astPrinter);
            mt.traverse(tree);
            Console.WriteLine(astPrinter);
            
            
            Processor myProcessor = new CppCodegen();			
			Console.WriteLine("Now compiling...");
			mt = new MapTraverser(myProcessor);
			mt.traverse(tree);
            

			Console.WriteLine("Done!");
			Console.ReadLine();
		}
	}

}