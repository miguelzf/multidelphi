using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using crosspascal.ast.nodes;
using crosspascal.parser;
using System.Collections.Specialized;
using System.Collections;

namespace crosspascal.core
{

	class TranslationPlanner
	{
		OrderedDictionary filesOrdered = new OrderedDictionary();

		DelphiPreprocessor preprocessor;
		String[] defaultDefines;

		public TranslationPlanner(string[] defines = null)
		{
			if (defines == null)
				defines = new string[0];

			preprocessor = new DelphiPreprocessor();
			defaultDefines = defines;
			if (defines == null)
				defaultDefines = new string[0];
		}

		public bool LoadIncludePaths(string fpath)
		{
			return preprocessor.LoadIncludePaths(fpath);
		}

		public void AddIncludePath(string path)
		{
			preprocessor.AddIncludePath(path);
		}

		public void AddDefine(string def)
		{
			preprocessor.AddDefine(def);
		}

		SourceFile Error(String msg)
		{
			Console.ForegroundColor = ConsoleColor.Red;
			Console.Error.WriteLine("[ERROR planner] " + msg);
			Console.ResetColor();
			return null;
		}


		/// <summary>
		/// Main method to prepare files for compilation.
		/// Loads, preprocesses and resolves dependencies
		/// </summary>
		public void LoadFiles(String[] filenames)
		{
			var files = MapDependencies(filenames);
			filesOrdered = OrderDependencies(files);
		}

		/// <summary>
		/// Access source files preprocessed, in topological order
		/// </summary>
		public IEnumerable<SourceFile> GetSourceFiles()
		{
			foreach (var f in filesOrdered.Values.Cast<SourceFile>())
				yield return f;
		}

		public int GetNumFiles()
		{
			return filesOrdered.Count;
		}

		/// <summary>
		/// Lists filesin topological order to a string output
		/// </summary>
		public String ListFiles()
		{
			string outp = "Ordered Source Files:";
			foreach (string s in filesOrdered.Keys)
				outp += " " + s;
			return outp;
		}


		/// <summary>
		/// Resolves an imported/used Unit dependency, by fetching it from the map of SourceFiles.
		/// Since the dependencies are topoligally sorted, it is guaranteed to have been previously processed
		/// (if the user of this class is accessing it thru the iterators, as it should).
		/// </summary>
		public SourceFile FetchFile(String file)
		{
			if (file == null || !filesOrdered.Contains(file))
				return null;
			else
				return filesOrdered[file] as SourceFile;
		}


		#region Processing of File Dependencies

		/// <summary>
		/// Topological sorting of the interface' dependencencies' DAG.
		/// The implementation dependencies are fetched as well, but are count for the topological order.
		/// </summary>
		OrderedDictionary OrderDependencies(Dictionary<string, SourceFile> files)
		{
			var remaining = new LinkedList<SourceFile>(files.Values);
			var ordered = new OrderedDictionary(remaining.Count);

			for (int c = 0; (c = remaining.Count) > 0; )
			{
				for (var p = remaining.Last; p != null; )
				{
					var f = p.Value;
					foreach (String d in f.depsInterf)
						if (!ordered.Contains(d))
							goto pass;

					// File has no dependencies remaining. Order it
					ordered.Add(f.name, f);
					var pdel = p;
					p = p.Previous;
					remaining.Remove(pdel);
					continue;

				pass:
					p = p.Previous;
				}

				if (c == remaining.Count)
				{	Error("Circular dependency in imported units");
					return new OrderedDictionary();
				}
			}

			return ordered;
		}


		/// <summary>
		/// Creates a dependency graph (DAG), supported by a resolver map of file names => Files,
		/// and additionally links the dependencies by SourceFile references
		/// </summary>
		Dictionary<string, SourceFile> MapDependencies(IEnumerable<string> filepaths)
		{
			Queue<SourceFile> cfiles = new Queue<SourceFile>();
			var files = new Dictionary<string, SourceFile>();

			foreach (string s in filepaths)
			{
				var file = LoadFileFromPath(s);
				if (file != null)
				{
					cfiles.Enqueue(file);
					files.Add(file.name, file);
				}
			}

			while (cfiles.Count > 0)
			{
				var cf = cfiles.Dequeue();

				foreach (string s in cf.GetDependenciesNames())
				{
					if (!files.ContainsKey(s))
					{
						var file = LoadFile(s);
						if (file == null)	// file could not be found/preprocessed
						{	Error("File " + file.name + " imported from " + cf.name + " failed to load");
							continue;
						}

						if (file.type != "unit")
						{	Error("Attempt to import non-unit file " + s + " in " + cf.name);
							continue;
						}

						cfiles.Enqueue(file);
						files.Add(file.name, file);
					}

					if (!cf.deps.ContainsKey(s))
						cf.deps.Add(s, files[s]);
					// else, it's a Unit imported in both interface and implementation
				}
			}

			return files;
		}


		SourceFile LoadFileFromPath(String fpath)
		{
			string fname = Path.GetFileNameWithoutExtension(fpath);

			if (!File.Exists(fpath))
				return Error("File " + fpath + " not found.");

			return LoadFile(fname, fpath);
		}

		SourceFile LoadFile(String fname)
		{
			string fpath;
			fpath = preprocessor.SearchFile(fname + ".pas");

			if (fpath == null)
				return Error("File " + fname + " does not exist");

			return LoadFile(fname, fpath);
		}

		const RegexOptions rgxOptions = RegexOptions.Compiled | RegexOptions.CultureInvariant;
		const StringSplitOptions splitOptions = StringSplitOptions.RemoveEmptyEntries;
		Regex rgxStart= new Regex(@"\w+", rgxOptions);
		Regex rgxStrs = new Regex(@"'[^']*'", rgxOptions);
		Regex rgxUses = new Regex(@"[^\w_0-9]uses\s+\w+(\s*,\s*\w+)*", rgxOptions);
		Regex rgxInterfUses = new Regex(@"[^\w_0-9]interface\s+(uses\s+\w+(\s*,\s*\w+)*)?", rgxOptions);

		SourceFile LoadFile(String fname, String fpath)
		{
			preprocessor.ResetPreprocessor(fpath);
			preprocessor.AddDefines(defaultDefines);

			try {
				preprocessor.Preprocess();
			}
			catch (PreprocessorException)
			{
				return Error("Preprocessing of file " + fpath + " failed");
			}

			String text = preprocessor.GetOutput();
			// remove strings
			String usetext = rgxStrs.Replace(text, "");

			String type = rgxStart.Match(text).Value;

			char[] seps = new char[] { ' ', ',', '\t', '\n', '\r' };

			string[] uses = new string[0];
			string[] interfuses = new string[0];
			int rgxstart = 0;

			if (type == "unit")
			{
				Match minterf = rgxInterfUses.Match(usetext);
				if (!minterf.Success)
					return Error("Missing Interface section in Unit " + fpath);

				string mitext = minterf.Value.Substring("_interface".Length).TrimStart();
				if (mitext.Length > 0)	// else, no uses in interface
					interfuses = mitext.Substring("uses".Length).Split(seps, splitOptions);
				rgxstart = minterf.Index + minterf.Length+1;
			}

			Match m = rgxUses.Match(usetext, rgxstart);
			if (m.Success)
				uses = m.Value.Substring("_uses".Length).Split(seps, splitOptions);

			return new SourceFile(fname, fpath, text, type, interfuses.ToList(), uses.ToList());
		}

		#endregion


	}
}
