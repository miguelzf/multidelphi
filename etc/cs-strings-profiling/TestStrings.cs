using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace test
{

	class TestStrings
	{

		#region Support methods

		static void printSWTime(Stopwatch sw, string msg = "")
		{
			TimeSpan ts = sw.Elapsed;
			Console.WriteLine(string.Format("{0} took {1:00}:{2:00}.{3:00}", msg,
								ts.Minutes, ts.Seconds, ts.Milliseconds/10));
		}


		/// <summary>
		/// Test runner with a regex options
		/// </summary>
		static void RunTest(String msg, RegexOptions rgxopt, Action<RegexOptions> test)
		{
			Stopwatch sw1 = new Stopwatch();
			sw1.Start();
			test(rgxopt);		// call and run test function
			sw1.Stop();
			printSWTime(sw1, msg);
			sw1.Reset();
		}

		/// <summary>
		/// Test runner with string data
		/// </summary>
		static void RunTest(String msg, string[] testdata, Action<string[]> test)
		{
			Stopwatch sw1 = new Stopwatch();
			sw1.Start();
			test((string[])testdata.Clone());		// call and run test function
			sw1.Stop();
			printSWTime(sw1, msg);
			sw1.Reset();
		}

		/// <summary>
		/// Test runner with no args
		/// </summary>
		static void RunTest(String msg, Action test)
		{
			Stopwatch sw1 = new Stopwatch();
			sw1.Start();
			test();		// call and run test function
			sw1.Stop();
			printSWTime(sw1, msg);
			sw1.Reset();
		}

		static Random random  = new Random((int)DateTime.Now.Ticks);
		const string chars = "1234567890qwertyuiopasdfghjklzxcvbnmABCDEFGHIJKLMNOPQRSTUVWXYZ";
			// "1234567890|!:;-.,+*#$%&/()=?qwertyuiopasdfghjklzxcvbnmABCDEFGHIJKLMNOPQRSTUVWXYZ";

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

		#endregion


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

	    static void TestGenRandomString(long minsize, long maxsize, long totalmax)
	    {
	    	Stopwatch sw1 = new Stopwatch();

			for (long s = minsize; s <= maxsize; s *= 5)
			{
				int count = (int) (totalmax/s);
				
				RunTest("[Test GenRandStr] CharArray " + count + "x " + s + "-length",
					new Action(() => 
					{
						for (int i = 0; i < count; i++)
							GenRandomString(i);
					}));

				RunTest("[Test GenRandStr] StrBuilder " + count + "x " +s + "-length",
					new Action(() =>
					{
						for (int i = 0; i < count; i++)
							GenRandomStringStrBuilder(i);
					}));
			}
	    }


		/*
			[Test ConcatString] String += took 01:20.20
			[Test ConcatString] String concat incr took 01:17.75
			[Test ConcatString] String concat seq took 00:00.01
			[Test ConcatString] StringBuilder append took 00:00.04
		*/

	    static void TestConcatString(string[] testdata)
	    {
			RunTest("[Test ConcatString] String +=",
				new Action(() =>
				{
					string data = "";
					foreach (var s in testdata)
						data += s;
					Console.WriteLine(data[data.Length - 3]);
				}));

			RunTest("[Test ConcatString] String concat incr",
				new Action(() =>
				{
					string data = "";
					foreach (var s in testdata)
						data = String.Concat(data, s);
					Console.WriteLine(data[data.Length - 3]);
				}));

			RunTest("[Test ConcatString] String concat seq",
				new Action(() =>
				{
					string data = String.Concat(testdata);
					Console.WriteLine(data[data.Length - 3]);
				}));

			RunTest("[Test ConcatString] StringBuilder append",
				new Action(() =>
				{
					StringBuilder sb = new StringBuilder();
					foreach (var s in testdata)
						sb.Append(s);
					string data = sb.ToString();
					Console.WriteLine(data[data.Length - 3]);
				}));

	    }


		/*
			[Test RgxOptions Replace] compiled took 00:03.49
			[Test RgxOptions Replace] compiled + single line took 00:03.44
			[Test RgxOptions Replace] compiled + multi line took 00:03.44
			[Test RgxOptions Replace] compiled + ignore case took 00:09.99
			[Test RgxOptions Replace] compiled + CultureInvariant took 00:03.65
		*/
		static void TestRegexOptionsReplace(string[] testdata)
		{
			const string pattern1 = "nf72v83b57";
			const string pattern2 = "kgbsgfjghjghbgd";

			for (int i = 0; i < testdata.Length; i++)
				testdata[i] = testdata[i].Replace("a", pattern1);

			Action<RegexOptions> action =
				new Action<RegexOptions>((opt) =>
					{
						string[] data = (string[]) testdata.Clone();
						Regex rgx = new Regex(pattern1, opt);
						for (int i = 0; i < data.Length; i++)
							data[i] = rgx.Replace(data[i], pattern2);
					});

			RunTest("[Test RgxOptions Replace] compiled", RegexOptions.Compiled, action);

			RunTest("[Test RgxOptions Replace] compiled + single line",
					RegexOptions.Compiled | RegexOptions.Singleline, action);

			RunTest("[Test RgxOptions Replace] compiled + multi line",
					RegexOptions.Compiled | RegexOptions.Multiline, action);

			RunTest("[Test RgxOptions Replace] compiled + ignore case",
					RegexOptions.Compiled | RegexOptions.IgnoreCase, action);

			RunTest("[Test RgxOptions Replace] compiled + CultureInvariant",
					 RegexOptions.Compiled | RegexOptions.CultureInvariant, action);
		}


		/*
			[Test RgxOptions Match] compiled took 00:05.69
			[Test RgxOptions Match] compiled + single line took 00:05.64
			[Test RgxOptions Match] compiled + multi line took 00:05.60
			[Test RgxOptions Match] compiled + ignore case took 00:30.86
			[Test RgxOptions Match] compiled + CultureInvariant took 00:05.63
		*/
		static void TestRegexOptionsMatch(string[] data)
		{
			const string pattern1 = "nf72v9'#%t5yhrgdb57";

			Action<RegexOptions> action =
				new Action<RegexOptions>((opt) =>
				{
					Regex rgx = new Regex(pattern1, opt);
					bool m = false;
					for (int j = 0; j < 100; j++)
						for (int i = 0; i < data.Length; i++)
							m |= rgx.IsMatch(data[i]);
					
					if (m)
						Console.WriteLine("found: " +m);
				});

			RunTest("[Test RgxOptions Match] compiled", RegexOptions.Compiled, action);

			RunTest("[Test RgxOptions Match] compiled + single line",
					RegexOptions.Compiled | RegexOptions.Singleline, action);

			RunTest("[Test RgxOptions Match] compiled + multi line",
					RegexOptions.Compiled | RegexOptions.Multiline, action);

			RunTest("[Test RgxOptions Match] compiled + ignore case",
					RegexOptions.Compiled | RegexOptions.IgnoreCase, action);

			RunTest("[Test RgxOptions Match] compiled + CultureInvariant",
					 RegexOptions.Compiled | RegexOptions.CultureInvariant, action);
		}


		/*	difference with a null replacement
			
			Has 814661 hits
			[Test ReplaceSimple: str to dbfhdgbhdgshjgvfbnmbn] String replace took 01:07.08
			[Test ReplaceSimple: str to dbfhdgbhdgshjgvfbnmbn] Regex replace took 01:30.65
			[Test ReplaceSimple: str to dbfhdgbhdgshjgvfbnmbn] StringBuilder replace took 01:53.56
			Has 814661 hits
			[Test ReplaceSimple: str to empty ] String replace took 01:06.60
			[Test ReplaceSimple: str to empty] Regex replace took 01:21.58
			[Test ReplaceSimple: str to empty] StringBuilder replace took 01:08.71
			Has 12686 hits
			[Test ReplaceSimple: str to dbfhdgbhdgshjgvfbnmbn] String replace took 00:54.75
			[Test ReplaceSimple: str to dbfhdgbhdgshjgvfbnmbn] Regex replace took 00:33.05
			[Test ReplaceSimple: str to dbfhdgbhdgshjgvfbnmbn] StringBuilder replace took 01:09.69
			Has 12686 hits
			[Test ReplaceSimple: str to ] String replace took 00:54.53
			[Test ReplaceSimple: str to ] Regex replace took 00:32.56
			[Test ReplaceSimple: str to ] StringBuilder replace took 01:09.18
		*/
		static void TestStringReplaceSimple(string[] testdata, string pattern2)
		{
			const string pattern1 = "nf72v83b57";

			int count = 0;
			for (int i = 0; i < testdata.Length; i++)
			{
				testdata[i] = testdata[i].Replace("aa", pattern1);
				count += Regex.Matches(testdata[i], pattern1).Count;
			}
			Console.WriteLine("Has " + count + " hits");

			RunTest("[Test ReplaceSimple: str to " + pattern2 + "] String replace",
					new Action(() =>
					{
						for (int j = 0; j < 100; j++)
						{
							string[] data = (string[])testdata.Clone();
							for (int i = 0; i < data.Length; i++)
								data[i] = data[i].Replace(pattern1, pattern2);
						}
					}));

			RunTest("[Test ReplaceSimple: str to " + pattern2 + "] Regex replace",
					new Action(() =>
					{
						Regex rgx = new Regex(pattern1, RegexOptions.Compiled);
						for (int j = 0; j < 100; j++)
						{
							string[] data = (string[])testdata.Clone();
							for (int i = 0; i < data.Length; i++)
								data[i] = rgx.Replace(data[i], pattern2);
						}
					}));

			RunTest("[Test ReplaceSimple: str to " + pattern2 + "] StringBuilder replace",
					new Action(() =>
					{
						StringBuilder[] sbdata = testdata.Select(x => new StringBuilder(x)).ToArray<StringBuilder>();

						for (int j = 0; j < 100; j++)
						{
							StringBuilder[] data = (StringBuilder[])sbdata.Clone();
							for (int i = 0; i < data.Length; i++)
								data[i] = data[i].Replace(pattern1, pattern2);
						}
					}));
		}


		/*	[Test ReplaceKeys] String replace took 01:14.06
			[Test ReplaceKeys] Regex replace took 00:41.92
			[Test ReplaceKeys] StringBuilder replace took 01:13.35
		*/
		static void TestStringReplaceKeys(string[] testdata)
		{
			const int basesize = 10000;
			const int nreplaces = 3000;
			const int nrounds = 1;

			var replaces = new List<string>(nreplaces+100);

			for (int i = 0; i < nreplaces; i++)
				replaces.Add(GenRandomString((random.Next(1000) + 10)));

			var sb = new StringBuilder(GenRandomString(basesize));

			foreach (var s in replaces.Reverse<String>())
				sb.Append(s);

			string data = sb.ToString();

			Console.WriteLine("Finished generating data real");

			int sum = 0;

			RunTest("[Test ReplaceKeys] String replace",
					new Action(() =>
					{
						for (int j = 0; j < nrounds; j++)
							foreach (var s in replaces)
								sum += data.Replace(s, "bfhgf")[basesize + 1000];
					}));

			RunTest("[Test ReplaceKeys] Regex replace",
					new Action(() =>
					{
						for (int j = 0; j < nrounds; j++)
							foreach (var s in replaces)
								sum += Regex.Replace(data, s, "bfhgf")[basesize + 1000];
					}));

			RunTest("[Test ReplaceKeys] StringBuilder replace",
					new Action(() =>
					{
						for (int j = 0; j < nrounds; j++)
							foreach (var s in replaces)
								sum += sb.Replace(s, "bfhgf")[basesize + 1000];
					}));

			Console.WriteLine("" + sum);
		}



		/*	com size = 1000000, nreplaces = 10000
			[Test MatchKeys] String Match took 14:35.26
			[Test MatchKeys] Regex IsMatch took 03:29.27
			[Test MatchKeys] Regex Match took 03:29.42
			 
			com size = 20000, nreplaces = 2000
			[Test MatchKeys] String Match took 00:29.60
			[Test MatchKeys] Regex IsMatch took 00:03.56
			[Test MatchKeys] Regex Match took 00:03.55
		  
			com size = 10000, nreplaces = 10000, only 10 are added to length:
			[Test MatchKeys] String Match took 00:03.66
			[Test MatchKeys] Regex IsMatch took 00:03.76
			[Test MatchKeys] Regex Match took 00:03.74
			
			String.IndexOf is faster for small strings
			But much slower for long strings
		*/
		static void TestStringMatchKeys(string[] testdata)
		{
			const int basesize = 10000;
			const int nreplaces = 10000;
			const int nrounds = 10;

			var replaces = new List<string>(nreplaces + 100);

			for (int i = 0; i < nreplaces; i++)
				replaces.Add(GenRandomString((random.Next(1000) + 10)));

			var sb = new StringBuilder(GenRandomString(basesize));

			foreach (var s in replaces.Take(10))
				sb.Append(s);

			string data = sb.ToString();

			Console.WriteLine("Finished generating data real");

			bool sum = false;

			RunTest("[Test MatchKeys] String Match",
					new Action(() =>
					{
						for (int j = 0; j < nrounds; j++)
							foreach (var s in replaces)
								sum |= (data.IndexOf(s) != 0);
					}));

			RunTest("[Test MatchKeys] Regex IsMatch",
					new Action(() =>
					{
						for (int j = 0; j < nrounds; j++)
							foreach (var s in replaces)
								sum |= Regex.IsMatch(data,s);
					}));

			RunTest("[Test MatchKeys] Regex Match",
					new Action(() =>
					{
						for (int j = 0; j < nrounds; j++)
							foreach (var s in replaces)
								sum |= Regex.Match(data, s).Success;
					}));

			Console.WriteLine("" + sum);
		}


		public static int Main(string[] args)
		{
			Stopwatch sw1 = new Stopwatch();
			Stopwatch sw2 = new Stopwatch();
			Stopwatch wtotal = new Stopwatch();
			wtotal.Start();

			string[] testdata = new string[1000];

//			for (int i = 0; i < testdata.Length; i++)
//				testdata[i] = GenRandomString(random.Next(100000)+100);

			Console.WriteLine("Finished generating data");
			Console.Out.Flush();
		
		// TestGenRandomString(1000000000, 1000000000, 10000000000000L);
		//	TestConcatString(testdata);
		//	TestRegexOptionsReplace(testdata);
		//	TestRegexOptionsMatch(testdata);
		//	TestStringReplaceSimple(testdata,"dbfhdgbhdgshjgvfbnmbn");
		//	TestStringReplaceSimple(testdata, "");

			TestStringMatchKeys(null);

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
