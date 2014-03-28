using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace crosspascal.semantics
{
	/// <summary>
	/// DAG-based symbol manager
	/// </summary>
	public class SymbolEnvironment<T> where T : class
	{
		SymbolContextNode<T> root;

		SymbolContextNode<T> current;

		int numContexts = 0;

		public SymbolEnvironment()
		{
			CreateContext("initial: empty default context");
		}


		#region Creating and Importing contexts

		/// <summary>
		/// Create a new child Symbol context and enters it
		/// </summary>
		public void CreateContext(string id = null, bool shadowing = true)
		{
			var ctx = new SymbolContextNode<T>(id, shadowing);
			ctx.AddParent(current);
			numContexts++;
			EnterContext(ctx);
		}

		/// <summary>
		/// Create a new child Symbol context and enters it
		/// </summary>
		public void CreateParentContext(string id = null, bool shadowing = true)
		{
			var ctx = new SymbolContextNode<T>(id, shadowing);
			current.AddParent(ctx);
			numContexts++;
			EnterContext(ctx);
		}

		/// <summary>
		/// Import external chuld context and enter it
		/// </summary>
		internal void ImportContext(SymbolContextNode<T> ctx)
		{
			ctx.AddParent(current);
			current = ctx;
			EnterContext(0);
		}

		/// <summary>
		/// Import external parent context and enter it
		/// </summary>
		internal void ImportParentContext(SymbolContextNode<T> ctx)
		{
			current.AddParent(ctx);
			numContexts++;
			EnterContext(ctx);
		}

		/// <summary>
		/// Export current context by cloning
		/// </summary>
		internal SymbolContextNode<T> ExportCurrentContext()
		{
			return current.Clone();
		}

		/// <summary>
		/// Get current context. Useful for re-using contetxs. Use with caution
		/// </summary>
		internal SymbolContextNode<T> GetCurrentContext()
		{
			return current;
		}

		#endregion


		#region Contexts Management

		/// <summary>
		/// Enters the next context in the DAG, the first of the current context's children
		/// </summary>
		public void EnterContext()
		{
			EnterContext(0);
		}

		/// <summary>
		/// Enters the child context with the given 'id'
		/// </summary>
		public void EnterContext(string id)
		{
			EnterContext(current.GetChild(id));
		}

		/// <summary>
		/// Enters the child context in the given index
		/// </summary>
		public void EnterContext(int idx)
		{
			EnterContext(current.GetChild(idx));
		}

		/// <summary>
		/// Enters the context passed as argument
		/// </summary>
		public void EnterContext(SymbolContextNode<T> ctx)
		{
			current = ctx;
		}

		/// <summary>
		/// Leaves the current Symbol context and switches to its 1st parent
		/// </summary>
		public String ExitContext()
		{
			return ExitContext(0);
		}

		/// <summary>
		/// Leaves the current Symbol context and switches to the parent with the index
		/// </summary>
		public String ExitContext(int idx)
		{
			string id = current.id;
			current = current.GetParent(idx);
			return id;
		}

		#endregion


		#region Management of symbols, Insertions and Lookups

		bool CheckValidKey(String key)
		{
			if (key == null || key == "")
				return false;
			return true;
		}

		T LookupRec(SymbolContextNode<T> ctx,  String key)
		{
			T t = ctx.Lookup(key);
			if (t != null)
				return t;

			foreach (var p in ctx.parents)
				if ((t = LookupRec(ctx, key)) != null)
					return t;

			return null;
		}

		public T Lookup(String key)
		{
			if (!CheckValidKey(key))
				return null;
			return LookupRec(current, key);
		}

		public T LookupCurrent(String key)
		{
			if (!CheckValidKey(key))
				return null;
			return current.Lookup(key);
		}

		/// <summary>
		/// Checks if symbol can be added to the present context, by fulfilling the conditions:
		/// 1) not being already defined in the current context
		/// 2) not being defined in any parent context that does now allow shadowing
		/// </summary>
		public bool CanAddSymbol(String key)
		{
			if (!CheckValidKey(key))
				return false;

			if (current.Lookup(key) != null)
				return false;

			foreach (var c in current.parents)
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

		/// <summary>
		/// Replaces (redefines) a symbol in the current context
		/// </summary>
		public bool Replace(String key, T symbol)
		{
			if (!CheckValidKey(key))
				return false;

			return current.Replace(key, symbol);
		}

		#endregion


		#region Printing and Inspecting

		public override string ToString()
		{
			return "SymTab with " + numContexts + " contexts";
		}

		/// <summary>
		/// Recursively traverse the whole DAG, in a DFS from bottom to top, up to a height limit
		/// </summary>
		String OutputGraph(SymbolContextNode<T> ctx,  int maxheight)
		{
			string text = ctx.ListContext() + Environment.NewLine;
			if (maxheight > 0)
				foreach (var p in ctx.parents)
					text += OutputGraph(p, maxheight - 1);
			return text;
		}

		internal String ListGraph(int maxdepth = Int32.MaxValue)
		{
			string sep = Environment.NewLine;
			return ToString() + sep + OutputGraph(current, maxdepth) + sep;
		}

		#endregion
	}



	/// <summary>
	/// Context of declared symbols.
	/// Implemented as a node of a DAG
	/// </summary>
	class SymbolContextNode<T> where T : class
	{
		internal List<SymbolContextNode<T>> parents;
		internal List<SymbolContextNode<T>> children;

		Dictionary<String, T> symbols;
		internal T lastInserted;

		internal bool allowShadowing;
		internal string id;

		internal SymbolContextNode(List<SymbolContextNode<T>> parents,
								String id = null, bool allowShadowing = true)
		{
			this.id = id;
			this.allowShadowing = allowShadowing;
			this.lastInserted = null;
			this.parents = parents;
			symbols = new Dictionary<String, T>();
		}

		internal SymbolContextNode(String id = null, bool allowShadowing = true)
			: this(new List<SymbolContextNode<T>>(10), id, allowShadowing)
		{
		}


		#region Access to Parents and Children

		internal void AddParent(SymbolContextNode<T> parent)
		{
			parents.Add(parent);
		}

		internal void AddChild(SymbolContextNode<T> parent)
		{
			children.Add(parent);
		}

		internal SymbolContextNode<T> GetParent(int idx)
		{
			return parents.ElementAt(idx);
		}

		internal SymbolContextNode<T> GetParent(String id)
		{
			foreach (var c in parents)
				if (c.id == id)
					return c;
			return null;
		}

		internal SymbolContextNode<T> GetChild(int idx)
		{
			return children.ElementAt(idx);
		}

		internal SymbolContextNode<T> GetChild(String id)
		{
			foreach (var c in children)
				if (c.id == id)
					return c;
			return null;
		}

		#endregion


		#region Access and Lookup

		internal T Lookup(String key)
		{
			return (symbols.ContainsKey(key) ? symbols[key] : null);
		}

		internal bool Add(String key, T symbol)
		{
			if (symbols.ContainsKey(key))
				return false;

			lastInserted = symbol;
			symbols[key] = symbol;
			return true;
		}

		internal bool Replace(String key, T symbol)
		{
			if (!symbols.ContainsKey(key))
				return false;

			symbols[key] = symbol;
			return true;
		}

		internal bool Remove(String key, T symbol)
		{
			return symbols.Remove(key);
		}

		#endregion


		/// <summary>
		/// clones without the parents
		/// </summary>
		internal SymbolContextNode<T> Clone()
		{
			var ctx = new SymbolContextNode<T>(id, allowShadowing);
			ctx.lastInserted = lastInserted;
			ctx.symbols = new Dictionary<String, T>(symbols);
			return ctx;
		}


		public override string ToString()
		{
			return "Context " + id + " with " + symbols.Count + " symbols";
		}

		internal String ListContext()
		{
			string sep = Environment.NewLine;
			string output = ToString() + ":" +sep;

			foreach (var k in symbols)
				output += "\t" + k.Key + " - " + k.Value.ToString() + sep;
			return output;
		}
	}
	// end Symbol Context

}


