using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using crosspascal.parser;
using crosspascal.semantics;
using crosspascal.ast;
using crosspascal.ast.nodes;
using System.Diagnostics;
using System.IO;

namespace crosspascal.core
{
	public class Compiler
	{

		TranslationPlanner planner;
		DelphiParser parser;

		public const int DefaultDebugLevel = 1;

		public int DebugLevel;

		public Compiler(int debuglevel = DefaultDebugLevel, string[] globalDefines = null)
		{
			DebugLevel = debuglevel;
			parser = new DelphiParser(DebugLevel);
			planner = new TranslationPlanner(globalDefines);

			planner.LoadIncludePaths("include-paths.txt");
		}


		public bool Compile(string[] filenames)
		{
			bool success = true;
			Console.WriteLine("CrossPascal Delphi compiler");

			// Load, preprocess and order them
			planner.LoadFiles(filenames);

			AstPrinter astPrinter = new AstPrinter();
			MapTraverser mt = new MapTraverser(astPrinter);

			foreach (SourceFile sf in planner.GetSourceFiles())
			{
				Console.Write("####### Compile file " + Path.GetFileName(sf.name) + ": ");

				if (sf.preprocText == null)		// preprocessing failed
				{	success = false;
					break;
				}

				StringReader sr = new StringReader(sf.preprocText);

				CompilationUnit ast;

				try {
					ast = parser.Parse(sr, sf);
					Console.WriteLine("Parsed OK: " + ast.name + " " + ast.ToString());
				}
				catch (ParserException e)
				{
					Console.Error.WriteLine(e);
					Console.Error.WriteLine("Parsing failed");
					success = false; 
					break;
				}

				astPrinter.StartProcessing(ast);
				Console.WriteLine(astPrinter);
			}

			return success;
		}

		public bool Compile(String filename)
		{
			return Compile(new string[]{filename});
		}


	}
}



