using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Text;
using crosspascal.ast.nodes;

namespace crosspascal.parser
{

	class ParserDriverTest
	{
		static void TestReadAll(string[] args)
		{
			string[] fstrings = new string[args.Length];
			var sw =  new Stopwatch();
			sw.Start();

			for(int i = 0; i < args.Length; i++)
			{
				string s = args[i];
				Console.Write("####### PARSE file " + Path.GetFileName(s) + "\n");
				var sr = new StreamReader(s, DelphiParser.DefaultEncoding);
				fstrings[i] = sr.ReadToEnd();
			}

			sw.Stop();
			Console.WriteLine("READING all files took " + sw.ElapsedMilliseconds + " milisecs");

			sw.Restart();
			for (int i = 0; i < 10; i++)
			foreach (string s in fstrings)
				new StringReader(s).ReadToEnd();

			Console.WriteLine("READING from StringStream all files took " + sw.ElapsedMilliseconds + " milisecs");
		}

		public static void Main(string[] args)
		{
			var sw = new Stopwatch();

			Console.WriteLine("CrossPascal Delphi compiler");

			DelphiParser parser = new DelphiParser();
			DelphiPreprocessor preproc = new DelphiPreprocessor();
			preproc.LoadIncludePaths("include-paths.txt");

			// TestReadAll(args);
			foreach (string s in args)
			{
				Console.Write("####### PARSE file " + Path.GetFileName(s) + ": ");

				preproc.InitLexer(s);
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
					CompilationUnit tree = (CompilationUnit) parser.Parse(sr);
					Console.WriteLine("Parsed OK: " + tree.name + " " + tree.ToString());
				}
				catch (ParserException e)
				{
					Console.Error.WriteLine(e);
					Console.Error.WriteLine("Parsing failed");
				}
			}

			sw.Stop();
			Console.WriteLine("READING from StringStream all files took " + (sw.ElapsedMilliseconds/1000.0) + " secs");
		}
	}

}

