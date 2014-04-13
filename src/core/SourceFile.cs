using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MultiPascal.AST.Nodes;
using MultiPascal.Semantics;

namespace MultiPascal.core
{
	public class SourceFile
	{
		public String name;

		public String path;

		public String preprocText;

		public List<String> depsInterf;

		public List<String> depsImpl;

		public Dictionary<String,SourceFile> deps;

		public String type;

		public int NDeps { get { return depsInterf.Count; } }

		/// <summary>
		/// Symbol context of the Interface section, if it's a unit. To be loaded by the Uses directive
		/// </summary>
		internal SymbolContext<Declaration,Section> interfContext;

		/// <summary>
		/// Get all dependencies' names, both interface and implementation deps
		/// </summary>
		public IEnumerable<String> GetDependenciesNames()
		{
			foreach (var sf in depsInterf)
				yield return sf;
			foreach (var sf in depsImpl)
				yield return sf;
		}

		/// <summary>
		/// Get all dependencies, both interface and implementation deps
		/// </summary>
		public IEnumerable<SourceFile> GetDependencies()
		{
			return deps.Values;
		}

		/// <summary>
		/// Get a SourceFile dependency by name
		/// </summary>
		public SourceFile GetDependency(string id)
		{
			return deps[id];	// should always be in the map 
		}

		public SourceFile(String name, String path, String pptext, String file,
						List<String> depsInterf = null, List<String> depsImpl = null)
		{
			this.name = name;
			this.path = path;
			this.preprocText = pptext;
			this.type = file;

			if (depsInterf == null) depsInterf = new List<String>();
			if (depsImpl == null) depsImpl = new List<String>();
			this.depsInterf = depsInterf;
			this.depsImpl = depsImpl;
			deps = new Dictionary<String, SourceFile>();
		}
	}
}
