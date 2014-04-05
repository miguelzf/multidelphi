using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace crosspascal.cpp
{
	class OutputStreams
	{
		private static Dictionary<String, StreamWriter> streams = new Dictionary<String, StreamWriter>();
		private static string streamName;
		private static StreamWriter currentStream;

		public static void Select(string name)
		{
			streamName = name;
			if (streams.ContainsKey(name))
			{
				currentStream = streams[name];				
			}
			else
			{
				FileStream fs = new FileStream(name, FileMode.Append, FileAccess.Write);
				currentStream = new StreamWriter(fs);
				streams.Add(name, currentStream);
			}
		}

		public static void Output(string s)
		{
			currentStream.WriteLine(s);
		}
	}
}
