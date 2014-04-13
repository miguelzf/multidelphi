using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Text;
using MultiPascal.core;
using MultiPascal.AST;
using MultiPascal.AST.Nodes;
using MultiPascal.Parser;
using MultiPascal.Codegen.Cpp;
using MultiPascal.Semantics;

namespace MultiPascal.TestUnits
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