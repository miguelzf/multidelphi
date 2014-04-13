using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiPascal.Semantics
{
	// Not used now

	/// <summary>
	/// Stack-based symbol table
	/// </summary>
	public class SymbolTable<T> where T : class
	{
		Stack<SymbolContext<T>> contexts = new Stack<SymbolContext<T>>();
		SymbolContext<T> current;
		bool allAllowShadowing = true;	// optimization: typically all contexts allow shadowing
		// no test to test them all then
		public SymbolTable()
		{
			Push("initial: empty default context");
		}

		/// <summary>
		/// Create a new Symbol context and push it onto the stack
		/// </summary>
		public void Push(string id = null, bool shadowing = true)
		{
			current = new SymbolContext<T>(id, shadowing);
			contexts.Push(current);
			if (!shadowing)
				allAllowShadowing = false;
		}

		/// <summary>
		/// Destroy the current Symbol context and pop it from the stack
		/// </summary>
		public String Pop()
		{
			string ret = contexts.Peek().id;
			contexts.Pop();
			current = contexts.Peek();
			return ret;
		}

		bool CheckValidKey(String key)
		{
			if (key == null || key == "")
				return false;
			return true;
		}

		public T Lookup(String key)
		{
			if (!CheckValidKey(key))
				return null;

			T t;
			foreach (var c in contexts)
				if ((t = c.Lookup(key)) != null)
					return t;
			return null;
		}

		public T LookupCurrent(String key)
		{
			if (!CheckValidKey(key))
				return null;
			return current.Lookup(key);
		}

		/// <summary>
		/// Looks up symbol in contexts that allow shadowing, i.e. check that symbol can be safely defined
		/// </summary>
		public bool CanAddSymbol(String key)
		{
			if (!CheckValidKey(key))
				return false;

			if (current.Lookup(key) != null)
				return false;

			if (!allAllowShadowing)
				foreach (var c in contexts)
					if (!c.allowShadowing && c.Lookup(key) != null)
						return false;

			return true;
		}

		/// <summary>
		/// Add symbol to current context. Checks that the symbol has not been defined 
		/// in any previous context that does allow shadowing
		/// </summary>
		public bool Add(String key, T symbol)
		{
			if (!CheckValidKey(key))
				return false;

			if (!CanAddSymbol(key))
				return false;

			return current.Add(key, symbol);
		}

		/*
		/// <summary>
		/// Import external context
		/// </summary>
		internal void ImportCurrentContext(SymbolContext<T> ctx)
		{
			contexts.Push(ctx);
			current = ctx;
			if (!ctx.allowShadowing)
				allAllowShadowing = false;
		}

		/// <summary>
		/// Export current context
		/// </summary>
		SymbolContext<T> ExportCurrentContext()
		{
			return current.Clone();
		}
		 */

		public override string ToString()
		{
			return "SymTab with " + contexts.Count + " contexts";
		}

		internal String ListTable(int nctxs = Int32.MaxValue)
		{
			string sep = Environment.NewLine;
			string outp = ToString() + sep;

			int i = 0;
			foreach (var c in contexts)
			{
				if (i++ == nctxs || c.id == "runtime")
					break;
				outp += c.ListContext() + sep;
			}
			return outp;
		}

		/// <summary>
		/// Context of declared symbols
		/// </summary>
		class SymbolContext<iT>
			where iT : class
		{
			Dictionary<String, iT> symbols;
			internal iT lastInserted;

			internal bool allowShadowing;
			internal string id;

			internal SymbolContext(String id = null, bool allowShadowing = true)
			{
				this.id = id;
				this.allowShadowing = allowShadowing;
				this.lastInserted = null;
				symbols = new Dictionary<String, iT>();
			}

			internal SymbolContext<iT> Clone()
			{
				var ctx = new SymbolContext<iT>(id, allowShadowing);
				ctx.lastInserted = lastInserted;
				ctx.symbols = new Dictionary<String, iT>(symbols);
				return ctx;
			}

			internal iT Lookup(String key)
			{
				return (symbols.ContainsKey(key) ? symbols[key] : null);
			}

			internal bool Add(String key, iT symbol)
			{
				if (symbols.ContainsKey(key))
					return false;

				lastInserted = symbol;
				symbols[key] = symbol;
				return true;
			}

			internal bool Replace(String key, iT symbol)
			{
				if (!symbols.ContainsKey(key))
					return false;

				symbols[key] = symbol;
				return true;
			}

			internal bool Remove(String key, iT symbol)
			{
				return symbols.Remove(key);
			}

			public override string ToString()
			{
				return "Context " + id + " with " + symbols.Count + " symbols";
			}

			internal String ListContext()
			{
				string sep = Environment.NewLine;
				string output = ToString() + ":" + sep;

				foreach (var k in symbols)
					output += "\t" + k.Key + " - " + k.Value.ToString() + sep;
				return output;
			}
		}
		// end Symbol Context
	}
}


