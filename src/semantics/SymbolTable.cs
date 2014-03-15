using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace crosspascal.semantics
{
	/// <summary>
	/// Stack-based symbol table
	/// </summary>
	public class SymbolTable<T> where T : class
	{

		Stack<SymbolContext<T>> contexts = new Stack<SymbolContext<T>>();
		SymbolContext<T> current;

		public SymbolTable()
		{
			Push("initial defaul");
		}

		/// <summary>
		/// Create a new Symbol context and push it onto the stack
		/// </summary>
		public void Push(string id = null, bool shadowing = false)
		{
			current = new SymbolContext<T>(id, shadowing);
			contexts.Push(current);
		}

		/// <summary>
		/// Destroyy the current Symbol context and pop it from the stack
		/// </summary>
		public void Pop()
		{
			contexts.Pop();
			current = contexts.Peek();
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
			foreach (var c in contexts.Reverse())
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
		public T LookupUnshadowable(String key)
		{
			if (!CheckValidKey(key))
				return null;

			T t;
			foreach (var c in contexts.Reverse())
				if ((t = c.Lookup(key)) != null && !c.allowShadowing)
					return t;
			return null;
		}

		/// <summary>
		/// Add symbol to current context.
		/// Checks if symbol has not been defined in any previous context that does allow shadowing
		/// </summary>
		public bool Add(String key, T symbol)
		{
			if (!CheckValidKey(key))
				return false;

			if (LookupUnshadowable(key) != null)
				return false;

			return current.Add(key, symbol);
		}



		/// <summary>
		/// Context of declared symbols
		/// </summary>
		struct SymbolContext<iT> where iT : class
		{
			Dictionary<String, iT> symbols;

			internal bool allowShadowing;
			internal string id;

			internal SymbolContext(String id = null, bool allowShadowing = false)
			{
				this.id = id;
				this.allowShadowing = allowShadowing;
				symbols = new Dictionary<String, iT>();
			}

			internal iT Lookup(String key)
			{
				return (symbols.ContainsKey(key) ? symbols[key] : null);
			}

			internal bool Add(String key, iT symbol)
			{
				if (symbols.ContainsKey(key))
					return false;

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

			bool Remove(String key, iT symbol)
			{
				return symbols.Remove(key);
			}
		}
		// end Symbol Context

	}

}


