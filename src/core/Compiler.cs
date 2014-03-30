﻿using System;
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

		public const int DefaultDebugLevel = 0;

		public static int DebugLevel;

		public Compiler(string[] globalDefines = null) : this(DefaultDebugLevel, globalDefines)
		{
		}

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
			Console.WriteLine(planner.ListFiles());

			AstPrinter astPrinter = new AstPrinter();
			NameResolver resolver = new NameResolver();

			int skip = 0;
			foreach (SourceFile sf in planner.GetSourceFiles())
			{
				if (++skip < 0) continue;

				Console.Write("####### Compile file " + Path.GetFileName(sf.name) + ": ");

				if (sf.preprocText == null)		// preprocessing failed
				{	success = false;
					break;
				}

				StringReader sr = new StringReader(sf.preprocText);

				TranslationUnit ast = null;

				try {
					ast = parser.Parse(sr, sf);
				}
				catch (ParserException e)
				{
					Console.Error.WriteLine(e);
					Console.Error.WriteLine("Parsing failed");
					success = false; 
					break;
				}

				if (ast == null)
				{
					Console.ReadLine();
					continue;
				}

				Console.ForegroundColor = ConsoleColor.Green;
				Console.WriteLine("Parsed OK: " + ast.name + " " + ast.ToString());
				Console.ResetColor();

				PostProcessing.SetParents(ast);
			//	new ParentProcessor().StartProcessing(ast);
				Console.WriteLine("SET parents OK: " + ast.name + " " + ast.ToString());
				resolver.Reset(sf);
				resolver.StartProcessing(ast);
				Console.WriteLine("Name Resolving OK: " + ast.name + " " + ast.ToString());

				astPrinter.StartProcessing(ast);

				Console.WriteLine(astPrinter);
			//	Console.ReadLine();
			}

			return success;
		}

		public bool Compile(String filename)
		{
			return Compile(new string[]{filename});
		}


	}
}



