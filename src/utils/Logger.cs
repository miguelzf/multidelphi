using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;

namespace MultiPascal.utils
{
	// semi-singleton
	public class Logger
	{
		// to be changed by the users of the Log
		private bool useConsole = false;
		private Dictionary<string, StreamWriter> files = new Dictionary<string, StreamWriter>();

		public static readonly Logger mainLog = new Logger();

		public static readonly Logger dumplog = new Logger();

		public static Logger log
		{
			get { return mainLog; }
		}

		private Logger() { }


		
		// =========================================================================
		// Configuration and Auxiliary functions
		// =========================================================================

		[MethodImpl(MethodImplOptions.Synchronized)]
		public bool UseConsole(bool b)
		{
			bool oldStatus = useConsole;
			useConsole = b;
			return oldStatus;
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		public bool CheckFileName(string fileName)
		{
			Regex hasBadChar = new Regex("[" + Regex.Escape(new string(Path.GetInvalidFileNameChars())) + "]");
			return !hasBadChar.IsMatch(fileName);
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		public bool CheckPathName(string pathName)
		{
			Regex containsABadCharacter = new Regex("[" + Regex.Escape(new string(Path.GetInvalidPathChars())) + "]");
			if (containsABadCharacter.IsMatch(pathName))
				return false;

			string root = Path.GetPathRoot(pathName);
			if ((root.Length > 0 && !System.IO.Directory.GetLogicalDrives().Contains(root))
			|| (root.Length < 1 && pathName.Contains(":")))
				return false;

			return CheckFileName(Path.GetFileName(pathName));
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		public bool AddFile(string fileName)
		{
			if (!CheckPathName(fileName))
				return false;
			try {
				files[fileName] = new StreamWriter(fileName);
			}
			catch (Exception) {
				return false;
			}
			return true;
		}


		// =========================================================================
		// Protocol Network Messages
		// =========================================================================

		[MethodImpl(MethodImplOptions.Synchronized)]
		public void Dump(String data, String msg = "message")
		{
			if (msg == null)
				msg = "message";

			string finalData = "===============================================\n"
								+ "[ " + msg + " ] " + data;
			WriteAll(finalData);
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		public void DumpBinary(byte[] data, String msg = null)
		{
			Dump(BitConverter.ToString(data), data.Length + " bytes | " + msg);
		}




		// =========================================================================
		// General data
		// =========================================================================

		// Call these methods for applicational errors or exceptions

		[MethodImpl(MethodImplOptions.Synchronized)]
		public void Warning(string msg)
		{
			WriteAll("WARNING! " + msg);
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		public void AppException(Exception e)
		{
			Warning(e.Message);
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		public void Exception(Exception ee)
		{
			Exception("", ee);
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		public void Exception(String msg, Exception ee)
		{
			WriteAll("[exception] " + msg + ": " + ee.GetType().ToString() + " : " + ee.Message);
			WriteAll("[stacktrace] " + ee.StackTrace);
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		public bool Error(string msg)
		{
			return WriteAll("[error] " + msg);
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		public void Terminate(string msg)
		{
			WriteAll("[error] " + msg);
			WriteAll("Application is going to shutdown now.");
			System.Environment.Exit(0);
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		public void printStrings(String msg, List<String> args)
		{
			WriteAllNoNL(msg + ":");
			foreach (string s in args)
				WriteAllNoNL(" " + s);
			WriteAll("");
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		public void printBools(String msg, List<bool> args)
		{
			WriteAllNoNL(msg + ":");
			foreach (bool s in args)
				WriteAllNoNL(" " + s);
			WriteAll("");
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		public void printInts(String msg, List<int> args)
		{
			WriteAllNoNL(msg + ":");
			foreach (int s in args)
				WriteAllNoNL(" " + s);
			WriteAll("");
		}



		// =========================================================================
		// Data writing
		// =========================================================================

		[MethodImpl(MethodImplOptions.Synchronized)]
		public bool Write(string data, string msg = null)
		{
			if (msg != null)
				data = msg + " : " + data;

			return WriteAll(data);
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		public bool WriteBinary(byte[] data, string msg = null)
		{
			return Write("< " + data.Length + " bytes > " + BitConverter.ToString(data), msg);
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		public bool Write(int[] data, string msg = null)
		{
			string ss = "";
			foreach (int i in data)
				ss += i + " ";
			return Write(data, msg);
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		public bool Write(int val, string msg = null)
		{
			return Write("" + val, msg);
		}

		// returns true if there is any logging stream to write to
		[MethodImpl(MethodImplOptions.Synchronized)]
		public bool WriteAll(string msg)
		{
			bool written = false;

			if (useConsole)
			{
				System.Console.WriteLine(msg);
				written = true;
			}

			foreach (KeyValuePair<String, StreamWriter> entry in files)
			{
				entry.Value.WriteLine(msg);
				entry.Value.Flush();
				written = true;
			}

			return written;
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		public void WriteNoNL(string msg)
		{
			WriteAllNoNL(msg);
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		public bool WriteAllNoNL(string msg)
		{
			bool written = false;

			if (useConsole)
			{
				System.Console.Write(msg);
				written = true;
			}

			foreach (KeyValuePair<String, StreamWriter> entry in files)
			{
				entry.Value.Write(msg);
				entry.Value.Flush();
				written = true;
			}

			return written;
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		public bool WriteCons(string msg)
		{
			System.Console.WriteLine(msg);
			return true;
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		public bool WriteFile(string fileName, string msg)
		{
			if (files.ContainsKey(fileName))
			{
				files[fileName].WriteLine(msg);
				files[fileName].Flush();
				return true;
			}
			else
				return false;
		}




		// =========================================================================
		//	Misc
		// =========================================================================

		private static void TestLogClass()
		{
			Console.WriteLine(System.IO.Path.GetInvalidFileNameChars());
			Console.WriteLine(new string(System.IO.Path.GetInvalidFileNameChars()));

			Logger.log.UseConsole(true);
			Logger.log.AddFile("teste @£}§§@£§@£e+2ewewrgfrhg");
			Logger.log.AddFile("aa");
			Logger.log.AddFile("fail \\32'1~~~´:wkre");
			Logger.log.AddFile("fail dfdf*dfdg");
			Logger.log.AddFile("fail dfdfdf?dg");
			Logger.log.AddFile("fail //32'1~~~´:wkre");
			Logger.log.AddFile("C:\\teste\\mfsdgiu");

			Logger.log.Write("qwertyuioplkjhgfdsazxcvbnm");
			Logger.log.Write("098765432112345678900987654321");
		}
	}
}
