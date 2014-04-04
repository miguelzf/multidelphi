using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace test
{

	class Test
	{

		static void printSWTime(Stopwatch sw, string msg = "")
		{
			TimeSpan ts = sw.Elapsed;
			Console.WriteLine(string.Format("{0} took {1:00}:{2:00}.{3:00}", msg,
								ts.Minutes, ts.Seconds, ts.Milliseconds/10));
		}

	
	  	static Random random  = new Random((int)DateTime.Now.Ticks);
		const string chars = "1234567890|!:;-.,+*#$%&/()=?qwertyuiopasdfghjklzxcvbnmABCDEFGHIJKLMNOPQRSTUVWXYZ";

		static string GenRandomString(int size)
		{
		    char[] buffer = new char[size];

		    for (int i = 0; i < size; i++)
		    {
		        buffer[i] = chars[random.Next(chars.Length)];
		    }
		    return new string(buffer);
		}


		static string GenRandomStringStrBuilder(int size)
	    {
	        StringBuilder builder = new StringBuilder();
	        char ch;
	        for (int i = 0; i < size; i++)
	        {
	            ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));                 
	            builder.Append(ch);
	        }

	        return builder.ToString();
	    }


		/*	// Results: CharArray is faster to create (initialize), and faster to execute/generate

			GenRandom CharArray 10-length took 00:01.28
			RandomString StringBuilder10-length took 00:06.23

			[Test GenRandStr] CharArray  5000x 100-length took 00:00.38
			[Test GenRandStr] StrBuilder 5000x 100-length took 00:01.66

			[Test GenRandStr] CharArray  10000x 100000-length took 00:01.26
			[Test GenRandStr] StrBuilder 10000x 100000-length took 00:06.26
			[Test GenRandStr] CharArray  2000x 500000-length took 00:00.05
			[Test GenRandStr] StrBuilder 2000x 500000-length took 00:00.25

			[Test GenRandStr] CharArray	 10000x 1000000000-length took 00:01.26
			[Test GenRandStr] StrBuilder 10000x 1000000000-length took 00:06.48
		*/


		void RunTest(String msg, Action<Object> test)
		{


		}

	    static void TestGenRandomString(long minsize, long maxsize, long totalmax)
	    {
	    	Stopwatch sw1 = new Stopwatch();

			for (long s = minsize; s <= maxsize; s *= 5)
			{
				int count = (int) (totalmax/s);
				
				sw1.Start();
				for (int i = 0; i < count; i++)
					GenRandomString(i);

				sw1.Stop();
				printSWTime(sw1, "[Test GenRandStr] CharArray " + count + "x " + s + "-length");
				sw1.Reset();

				sw1.Start();
				for (int i = 0; i < count; i++)
					GenRandomStringStrBuilder(i);

				sw1.Stop();
				printSWTime(sw1, "[Test GenRandStr] StrBuilder " + count + "x " +s + "-length");
				sw1.Reset();
			}
	    }

		
	    static void TestConcatString(string[] testdata)
	    {
	    	Stopwatch sw1 = new Stopwatch();

				sw1.Start();
				string data = "";
				foreach (var s in testdata)
					data += testdata;
				sw1.Stop();
				printSWTime(sw1, "[Test ConcatString] String += ");
				sw1.Reset();

	    }

		public static int Main(string[] args)
		{
			Stopwatch sw1 = new Stopwatch();
			Stopwatch sw2 = new Stopwatch();
			Stopwatch wtotal = new Stopwatch();
			wtotal.Start();

			string[] testdata = new string[100];

			for (int i = 0, len = 100; i < 10; i++, len *= 10)
				testdata[i] = GenRandomString(len);

			// TestGenRandomString(1000000000, 1000000000, 10000000000000L);
			

		//	Console.Out.WriteLine(output);

			wtotal.Stop();
			printSWTime(wtotal, "Main");
			return 0;
		}

	}

/*



		Regex rgxNLs = new Regex("[\n\r]+", RegexOptions.Compiled);
		Regex rgxLineCom = new Regex("//.*", RegexOptions.Compiled);
		Regex rgxBlockCom1 = new Regex(@"\(\*.*?\*\)", RegexOptions.Singleline | RegexOptions.Compiled);
		Regex rgxBlockCom2 = new Regex(@"{[^$][^}]*}", RegexOptions.Singleline | RegexOptions.Compiled);
		Regex incRegex = new Regex(@"{\$[iI] [^}]+}", RegexOptions.Compiled);
		Regex dirRegex = new Regex(@"{\$[^}]+}", RegexOptions.Compiled);
	
	
		while ((m = incRegex.Match(source)).Success)
			{
				string Spath = m.Value.Substring(4, m.Length-5).Trim();
				Spath = SearchFile(Spath);

				string content;
				using (var reader = new StreamReader(Spath, Encoding.UTF8))
					content = reader.ReadToEnd();
	
				swii3.Start();
				// Remove newlines to preserve line numbers
				content = RemoveComments(content);	// 1st need to remove 1-line comments
				content = RemoveNewLines(content);
				source = source.Replace(m.Value, content);
				swii3.Stop();
			}


		string RemoveNewLines(string source)
		{
			return rgxNLs.Replace(source, " ");
		}

		string RemoveComments(string source)
		{
			return rgxBlockCom1.Replace(rgxBlockCom2.Replace(rgxLineCom.Replace(source, ""), ""), "");
		}

		public string RemoveNewLines(string source)
		{
			Regex  regex = new Regex("[\n\r]+", RegexOptions.Compiled);
			return regex.Replace(source, " ");
		}

		public string RemoveCommentsSrc(string source)
		{
			Regex regex1 = new Regex("//.*", RegexOptions.Compiled);
			Regex regex2 = new Regex(@"\(\*.*?\*\)", RegexOptions.Compiled);
			Regex regex3 = new Regex(@"{[^$][^}]*}", RegexOptions.Compiled);
			return regex3.Replace(regex2.Replace(regex1.Replace(source, ""), ""), "");
		}

		public string RemoveCommentsInc(string source)
		{
			Regex regex1 = new Regex("//.*", RegexOptions.Compiled);
			Regex regex2 = new Regex(@"\(\*.*?\*\)|{[^$][^}]*}",
									RegexOptions.Singleline | RegexOptions.Compiled);
			return regex2.Replace(regex1.Replace(source, ""), "");
		}

		public string InlineIncludes(string source)
		{
			sw2.Start();

			Regex incRegex = new Regex(@"{\$[iI] [^}]+}", RegexOptions.Compiled);
		
			Match m;
			while ((m = incRegex.Match(source)).Success)
			{
				string Spath = m.Value.Substring(4, m.Length-5).Trim();
				Spath = SearchFile(Spath);

				sw3.Start();
				string content;
				using (var reader = new StreamReader(Spath, Encoding.UTF8))
					content = reader.ReadToEnd();
				sw3.Stop();
	
				sw4.Start();
				// Remove newlines to preserve line numbers
				content = RemoveCommentsInc(content);	// 1st need to remove 1-line comments
				content = RemoveNewLines(content);
				sw4.Stop();

				source = source.Replace(m.Value, content);
			}
			
			sw2.Stop();
			return source;
		}

		public string PreProcess(string source)
		{
			int Count = 0;
			string output = "", directiveStr="";
			DefineEntry[] tempDefines = new DefineEntry[1024];
			var directives = new Dictionary<string,Action<string>>();

			sw1.Start();
			source = RemoveCommentsSrc(source);
			sw1.Stop();
    
			sw7.Start();

			// parse $Ifdefs
			do {
				tempDefines[0].Ignore = false;

				if (string.IsNullOrEmpty(source))
					break;

				sw5.Start();
				string S2 = GetNextToken(ref source);
				directiveStr = S2;
				sw5.Stop();
				
				if (S2.IndexOf("{$") !=0)
				{
					if (!tempDefines[Count].Ignore) 
						output += S2;
				}
				else
				{
					sw6.Start();
					
					S2 = S2.Substring(1, S2.Length-2);
					S2 = S2.ToLower();	
					char delimiter = ' ';
					string S3 = GetNextWord(ref S2, delimiter).Trim();

					if (directives.ContainsKey(S3))
						directives[S3](S2);
					else
					{	// echo directive unchanged
						output += directiveStr;
					}

					sw6.Stop();
				}

			} while (true);

			sw7.Stop();			
			return output;
		}
*/

}
