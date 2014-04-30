using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Text;
using MultiDelphi.core;
using MultiDelphi.AST;
using MultiDelphi.AST.Nodes;
using MultiDelphi.Parser;
using MultiDelphi.Codegen.Cpp;
using MultiDelphi.Semantics;

namespace MultiDelphi.TestUnits
{

	class CppProgram
	{
		static void Main(string[] args)
		{
			var sw = new Stopwatch();
			sw.Start();

			Compiler compiler = new Compiler(new string[] { "WINDOWS" });
			compiler.Compile(args);

			sw.Stop();

			Console.WriteLine("Compiling all files took " + (sw.ElapsedMilliseconds / 1000.0) + " secs");
			Console.ReadLine();
		}
	}

}