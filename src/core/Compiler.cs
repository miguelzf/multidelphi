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
		DelphiParser parser;
		DelphiPreprocessor preprocessor;

		const int DefaultDebugLevel = 0;

		public int DebugLevel;

		public Compiler(int debuglevel = DefaultDebugLevel)
		{
			DebugLevel = debuglevel;
			preprocessor = new DelphiPreprocessor();
			parser = new DelphiParser(DebugLevel);

			preprocessor.LoadIncludePaths("include-paths.txt");
		}

		List<String> Files = new List<String>();


		public bool Compile(string[] filenames)
		{
			bool success = true;
			Console.WriteLine("CrossPascal Delphi compiler");

			foreach (string s in filenames)
			{
				Console.Write("####### Compile file " + Path.GetFileName(s) + ": ");

				preprocessor.InitPreprocessor(s);
				preprocessor.AddDefine("WINDOWS");	// test

				try
				{
					preprocessor.Preprocess();
				}
				catch (PreprocessorException e)
				{
					Console.Error.WriteLine("Preprocessing failed");
					success = false;
					continue;
				}

				string preprocfiletext = preprocessor.GetOutput();
				StringReader sr = new StringReader(preprocfiletext);

				CompilationUnit ast;

				try {
					ast = parser.Parse(sr);
					Console.WriteLine("Parsed OK: " + ast.name + " " + ast.ToString());
				}
				catch (ParserException e)
				{
					Console.Error.WriteLine(e);
					Console.Error.WriteLine("Parsing failed");
					success = false; 
				}


				// TODO Process AST

			}

			return success;
		}

		public bool Compile(String filename)
		{
			return Compile(new string[]{filename});
		}


	}
}



