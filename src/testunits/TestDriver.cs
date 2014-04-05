using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using crosspascal.core;
using crosspascal.parser;
using crosspascal.ast;
using crosspascal.ast.nodes;
using crosspascal.codegen.cpp;

namespace crosspascal.testunits
{

	class TestDriver
	{

		public static void Main(string[] args)
		{
			var sw = new Stopwatch();
			sw.Start();
			
			Compiler compiler = new Compiler(new string[] { "WINDOWS" });
			compiler.Compile(args);
			
			sw.Stop();

			Console.WriteLine("Compiling all files took " + (sw.ElapsedMilliseconds/1000.0) + " secs");
		}		
		


		static void TestReadAll(string[] args)
		{
			string[] fstrings = new string[args.Length];
			var sw =  new Stopwatch();
			sw.Start();

			for(int i = 0; i < args.Length; i++)
			{
				string s = args[i];
				Console.Write("####### PARSE file " + Path.GetFileName(s) + "\n");
				var sr = new StreamReader(s, DelphiPreprocessor.DefaultEncoding);
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
		
	}

}

