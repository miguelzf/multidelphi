using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Text;
using crosspascal.core;
using crosspascal.ast;
using crosspascal.ast.nodes;
using crosspascal.parser;
using crosspascal.codegen.cpp;
using crosspascal.semantics;

namespace crosspascal.testunits
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