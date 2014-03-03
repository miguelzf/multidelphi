using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace crosspascal.parser
{

	struct DefineEntry 
	{
		public string Name;
		public bool Ignore;
	}

	class PreProcessor
	{
		private List<string> _paths;
		private List<string> _defines;

		public PreProcessor()
		{ 
			_paths = new List<string>();
			_defines = new List<string>();
		}

		public void AddPath(string path)
		{ 
			_paths.Add(path);
		}

		public void AddDefine(string define)
		{ 
			define = define.ToLower();
			_defines.Add(define);
		}

		private bool IsDefined(string s)
		{
			s = s.ToLower();
			foreach (string def in _defines)
			if (def.Equals(s))
			{
				return true;
			}
			return false;
		}

		private string SearchFile(string fileName)
		{
			foreach (string path in _paths)
			{
				string s = path+Path.PathSeparator + fileName;
				if (File.Exists(s))
					return s;
			}

			throw new System.ArgumentException("File not found: " + fileName);
		}

		private string GetNextToken(ref string S)
		{
			int i = S.IndexOf("{");
			if (i<0)
			{
				string temp = S;
				S = "";
				return temp;
			}
			else
			if (i==0)
			{
				int j = S.IndexOf("}");
				string temp = S.Substring(0, j+1);
				S = S.Substring(j+1);
				return temp;
			}
			else
			{
				string temp = S.Substring(0, i-1);
				S = S.Substring(i);
				return temp;
			}
		}

		private string GetNextWord(ref string S, char delimiter)
		{
			S = S.Trim();
			if (String.IsNullOrEmpty(S))
			{
				return "";
			}
  
			int I = S.IndexOf(delimiter);
			if (I<0)
			{
				string temp = S;
				S = "";
				return temp;
			}
			else
			{
				string result = S.Substring(0, I);
				S = S.Substring((I+1));
				S = S.Trim();
				return result;
			}			
		}

		public string PreProcess(string source)
		{
			string S;
			int I,J,K;
			bool Passing = false;
			bool isDirective = false;			

			// inline all include files
			I = source.IndexOf("$I ");
			while (I>0)
			{
				S = source.Substring(I +3);
				I = S.IndexOf("}");
				S = S.Substring(1, I-1);
				string S2 = SearchFile(S);
	
				System.IO.StreamReader myFile = new System.IO.StreamReader(S2);
				string content = myFile.ReadToEnd();			
				myFile.Close();

				source.Replace("{$I "+S+"}", content);
				I = source.IndexOf("$I ");
			}
    
			int Count = 0;
			int N = 0;
			string output = "";
			DefineEntry[] tempDefines = new DefineEntry[1024];
			// parse $Ifdefs
			do {
				tempDefines[0].Ignore = false;
				isDirective = false;

				if (String.IsNullOrEmpty(source))
					break;

				string S2 = GetNextToken(ref source);
				Console.WriteLine(S2);
				if (S2.IndexOf("{$")==0)
				{
					S2 = S2.Substring(1, S2.Length-2);
					S2 = S2.ToLower();					
					char delimiter = ' ';
					string S3 = GetNextWord(ref S2, delimiter);
					isDirective = false;
					if (S3.Equals("$define")) 
					{
						isDirective = true;
						if (!tempDefines[Count].Ignore) 
							AddDefine(S2);
					}
					else
					if (S3.Equals("$ifdef"))
					{
						isDirective = true;
						Count++;
						Passing = true;
						tempDefines[Count].Name = S2;
						tempDefines[Count].Ignore = (!IsDefined(S2));
					}
					else
					if (S3.Equals("$ifndef"))
					{
						isDirective = true;
						Count++;
						Passing = true;
						tempDefines[Count].Name = S2;
						tempDefines[Count].Ignore = (IsDefined(S2));
					}
					else
					if (S3.Equals("$else"))
					{
						isDirective = true;
						tempDefines[Count].Ignore = !tempDefines[Count].Ignore;
					}
					else
					if (S3.Equals("$endif"))
					{
						isDirective = true;
						if (Count>0) 
							Count--;

						if (Count<=0) 
							Passing = false;
					}
					else
					{
						// do nothing
						//StringToInt(S3);
					}
				}

				if (isDirective || tempDefines[Count].Ignore) 
				{
					// lol do nothing
				}
				else
				{
					output = output + S2;
				}
			} while (true);

			return output;
		}

	}
}
