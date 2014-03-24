using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Threading;

namespace crosspascal.parser
{
	public partial class DelphiPreprocessor	
	{
		Dictionary<String, String> IncludesCache = new Dictionary<string, string>();

		//Encoding.Default;	// typically Single-Bye char set
		// TODO change charset to unicode, use %unicode in flex
		public static readonly Encoding DefaultEncoding = Encoding.GetEncoding("iso-8859-1");

		void ResetPreprocessor()
		{	// to be resetted each time this function is called
			definedValues = new List<string>();
		}


		/// <summary>
		/// Queries the cache for an include file.
		/// If not found, prepares to add one when the preprocessor finishes the file
		/// </summary>
		bool FetchInclude2(string fname, out string include)
		{
			if (IncludesCache.ContainsKey(fname))
			{	include = IncludesCache[fname];
				return true;
			}
			else {
				// File not in cache, load it and prepare for processing
				include = SearchFile(fname);
				return false;
			}
		}

		string FetchInclude(string fname)
		{
			string ret;
			IncludesCache.TryGetValue(fname, out ret);
			return ret;		// null if not found
		}
		
		void CacheInclude(string fname, string text)
		{
			IncludesCache[fname] = text;
		}


		#region Include Directories

		private List<string> includePaths = new List<string>();

		/// <summary>
		/// Searches one file in the filesystem, in the include directories
		/// </summary>
		public string SearchFile(string fileName)
		{
			if (File.Exists(fileName))
				return fileName;

			foreach (string path in includePaths)
			{
				string s = path + fileName;
				if (File.Exists(s))
					return s;
			}

			fileName = fileName.ToLower();
			foreach (string path in includePaths)
			{
				string s = path + fileName;
				if (File.Exists(s))
					return s;
			}

			// throw new System.ArgumentException("File not found: " + fileName);
			return null;
		}

		public bool LoadIncludePaths(string fpath)
		{
			string line;

			if (!File.Exists(fpath))
			{
				pperror("File " + fpath + " not found");
				return false;
			}

			using (StreamReader file = new StreamReader(fpath, DefaultEncoding))
				while ((line = file.ReadLine()) != null)
				{
					string path = line.Trim();
					if (path.Length == 0)
						continue;
					
					// TODO
					// Regex.Replace(path, @"\$\(TERRA\)", 
					if (Directory.Exists(path))
						AddIncludePath(path);
				}
			return true;
		}

		public void AddIncludePath(string path)
		{
			path = path.Replace('\\', Path.DirectorySeparatorChar);
			path = path.Replace('/' , Path.DirectorySeparatorChar);
			if (path[path.Length-1]!= Path.DirectorySeparatorChar)
				path += Path.DirectorySeparatorChar;
			includePaths.Add(path);
		}

		#endregion


		#region Preprocessor Defined Values 

		private List<string> definedValues;

		public void AddDefine(string define)
		{ 
			definedValues.Add(define.ToLower());
		}

		public void AddDefines(IEnumerable<String> defines)
		{
			foreach (String s in defines)
				definedValues.Add(s.ToLower());
		}

		public void RemoveDefine(string define)
		{
			string def = define.ToLower();
			if (definedValues.Contains(def))
				definedValues.Remove(def);
		}

		public bool IsDefined(string s)
		{
			s = s.ToLower();
			foreach (string def in definedValues)
				if (def.Equals(s))
					return true;
			return false;
		}

		#endregion
	}
}
