using System;
using System.Collections;
using System.IO;
using System.Text;
using System.Diagnostics;


namespace MultiPascal.Parser
{
	public interface ParserDebug
	{
		void push (int state, Object value);
		void lex (int state, int token, string name, Object value);
		void shift (int from, int to, int errorFlag);
		void pop (int state);
		void discard (int state, int token, string name, Object value);
		void reduce (int from, int to, int rule, string text, int len);
		void shift (int from, int to);
		void accept (Object value);
		void error (string message);
		void reject ();
	}
	
	public class DebugPrintAll : ParserDebug
	{
		void println (string s){
			Console.Error.WriteLine (s);
		}
		
		public void push (int state, Object value) {
			println ("push\tstate "+state+"\tvalue "+value);
		}
		
		public void lex (int state, int token, string name, Object value) {
			println("lex\tstate "+state+"\treading "+name+"\tvalue "+value);
		}
		
		public void shift (int from, int to, int errorFlag) {
			switch (errorFlag) {
			default:				// normally
				println("shift\tfrom state "+from+" to "+to);
				break;
			case 0: case 1: case 2:		// in error recovery
				println("shift\tfrom state "+from+" to "+to
				+"\t"+errorFlag+" left to recover");
				break;
			case 3:				// normally
				println("shift\tfrom state "+from+" to "+to+"\ton error");
				break;
			}
		}
		
		public void pop (int state) {
			println("pop\tstate "+state+"\ton error");
		}
		
		public void discard (int state, int token, string name, Object value) {
			println("discard\tstate "+state+"\ttoken "+name+"\tvalue "+value);
		}
		
		public void reduce (int from, int to, int rule, string text, int len) {
			println("reduce\tstate "+from+"\tuncover "+to
			+"\trule ("+rule+") "+text);
		}
		
		public void shift (int from, int to) {
			println("goto\tfrom state "+from+" to "+to);
		}
		
		public void accept (Object value) {
			println("accept\tvalue "+value);
		}
		
		public void error (string message) {
			println("error\t"+message);
		}
		
		public void reject () {
			println("reject");
		}
	}


	// Internal for Debug prints
	class DebugPrintFinal : ParserDebug
	{
		const int yyFinal = 6;
		
		void println (string s) {
			Console.Error.WriteLine (s);
		}
		
		void printchecked(int state, string s)
		{
			if (state == 0 || state == yyFinal)
			println(s);
		}

		void printchecked(int from, int to, string s)
		{
			if (from == 0 || from == yyFinal || to == 0 || to == yyFinal)
			println(s);
		}
		
		public void push (int state, Object value) {
			printchecked (state, "push\tstate "+state+"\tvalue "+value);
		}
		
		public void lex (int state, int token, string name, Object value) {
			printchecked (state, "lex\tstate "+state+"\treading "+name+"\tvalue "+value);
		}
		
		public void shift (int from, int to, int errorFlag)
		{
			switch (errorFlag) {
			default:				// normally
				printchecked (from, to, "shift\tfrom state "+from+" to "+to);
				break;
			case 0: case 1: case 2:		// in error recovery
				printchecked (from, to,"shift\tfrom state "+from+" to "+to +"\t"+errorFlag+" left to recover");
				break;
			case 3:				// normally
				printchecked (from, to, "shift\tfrom state "+from+" to "+to+"\ton error");
				break;
			}
		}
		
		public void pop (int state) {
			printchecked (state,"pop\tstate "+state+"\ton error");
		}
		
		public void discard (int state, int token, string name, Object value) {
			printchecked (state,"discard\tstate "+state+"\ttoken "+name+"\tvalue "+value);
		}
		
		public void reduce (int from, int to, int rule, string text, int len) {
			printchecked (from, to, "reduce\tstate "+from+"\tuncover "+to +"\trule ("+rule+") "+text);
		}
		
		public void shift (int from, int to) {
			printchecked (from, to, "goto\tfrom state "+from+" to "+to);
		}
		
		public void accept (Object value) {
			println("accept\tvalue "+value);
		}
		
		public void error (string message) {
			println("error\t"+message);
		}
		
		public void reject () {
			println("reject");
		}
		
	}
}
