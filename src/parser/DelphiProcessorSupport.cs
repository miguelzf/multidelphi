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
		private List<string> _paths = new List<string>();
		private List<string> _defines;

		void InitPreprocessor()
		{	// to be resetted each time this function is called
			_defines = new List<string>();
		}
			
		public void LoadIncludePaths(string fname)
		{
			string line;
			try 
			{	using (StreamReader file = new StreamReader(fname, DelphiParser.DefaultEncoding))
					while((line = file.ReadLine()) != null)
					{
						string path = line.Trim();
						if (path.Length > 0 && Directory.Exists(path))
							AddIncludePath(path);
					}
			}
			catch (FileNotFoundException)
			{	pperror("File " + fname + " not found");
			}
		}

		public void AddIncludePath(string path)
		{
			if (path.EndsWith("\\") || path.EndsWith("/"))
				return ;
			else
				_paths.Add(path+Path.DirectorySeparatorChar);
		}

		public void AddDefine(string define)
		{ 
			_defines.Add(define.ToLower());
		}

		public void RemoveDefine(string define)
		{
			string def = define.ToLower();
			if (_defines.Contains(def))
				_defines.Remove(def);
		}

		public bool IsDefined(string s)
		{
			s = s.ToLower();
			foreach (string def in _defines)
				if (def.Equals(s))
					return true;
			return false;
		}

		public string SearchFile(string fileName)
		{
			if (File.Exists(fileName))
				return fileName;

			foreach (string path in _paths)
			{
				string s = path + fileName;
				if (File.Exists(s))
					return s;
			}

			fileName = fileName.ToLower();
			foreach (string path in _paths)
			{
				string s = path + fileName;
				if (File.Exists(s))
					return s;
			}

			// throw new System.ArgumentException("File not found: " + fileName);
			return null;
		}			
	}
}
