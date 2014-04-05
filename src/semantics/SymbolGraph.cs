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
	public class SymbolGraph<SymbolT, CtxKey> 
		where SymbolT : class
		where CtxKey : class
	{
		SymbolContext<SymbolT,CtxKey> root;

		SymbolContext<SymbolT,CtxKey> current;

		// Context path taken, excluding current context
		Stack<SymbolContext<SymbolT, CtxKey>> path;

		int numContexts = 0;

		public SymbolGraph()
		{
			root = new SymbolContext<SymbolT,CtxKey>("initial: empty default context");
			Reset();
		}


		/// <summary>
		/// </summary>
		public void Reset()
		{
			current = root;
			path = new Stack<SymbolContext<SymbolT, CtxKey>>(1024); 
		}

		/// <summary>
		/// Enters in the next context after the current one.
		/// Can be called repeatedly to continue from context to context
		/// </summary>
		public IEnumerable<bool> LoadNextContext()
		{
			return LoadNextContext(current);
		}

		/// <summary>
		/// Traverse the whole DAG, in BFS order, from top to down
		/// </summary>
		internal IEnumerable<bool> LoadNextContext(SymbolContext<SymbolT, CtxKey> ctx)
		{
			for (int i = ctx.children.Count - 1; i >= 0; i--)
			{
				var child = ctx.children[i];
				EnterContext(child);
				yield return true;

				foreach (var c in LoadNextContext(child))
					yield return c;

				ExitContext();
				yield return false;
			}
		}

		public CtxKey CurrentCtxKey()
		{
			return current.Key;
		}


		#region Creating and Importing contexts

		/// <summary>
		/// Create a new child Symbol context and enters it
		/// </summary>
		public void CreateContext(string id = null, CtxKey key = default(CtxKey), bool shadowing = true)
		{
			var ctx = new SymbolContext<SymbolT,CtxKey>(id, key, shadowing);
			current.AddChild(ctx);
			numContexts++;
			EnterContext(ctx);
		}

		/// <summary>
		/// Create a new child Symbol context and enters it
		/// </summary>
		public void CreateParentContext(string id = null, CtxKey key = default(CtxKey), bool shadowing = true)
		{
			var ctx = new SymbolContext<SymbolT,CtxKey>(id, key, shadowing);
			current.AddParent(ctx);
			numContexts++;
			EnterContext(ctx);
		}

		/// <summary>
		/// Import external child context and enters it
		/// </summary>
		internal void ImportContext(SymbolContext<SymbolT,CtxKey> ctx)
		{
			current.AddChild(ctx);
			numContexts++;
			EnterContext(0);
		}

		/// <summary>
		/// Import external parent context to maximum precedence, and enters it
		/// </summary>
		internal void ImportParentCtxToFirst(SymbolContext<SymbolT,CtxKey> ctx)
		{
			current.parents.Insert(0, ctx);
			numContexts++;
			EnterContext(ctx);
		}

		/// <summary>
		/// Import external parent context to lowest precedence, and enters it
		/// </summary>
		internal void ImportParentCtxToLast(SymbolContext<SymbolT, CtxKey> ctx)
		{
			current.AddParent(ctx);
			numContexts++;
			EnterContext(ctx);
		}


		/// <summary>
		/// Export current context by clonning: creates new context structures
		/// </summary>
		internal SymbolContext<SymbolT,CtxKey> ExportCloneContext(Func<SymbolT,bool> pred = null)
		{
			if (pred == null)
				pred = new Func<SymbolT, bool>(x => true);
			return current.Clone(pred);
		}

		/// <summary>
		/// Export current context by copying: keeps the context structures
		/// </summary>
		internal SymbolContext<SymbolT, CtxKey> ExportCopyContext()
		{
			return current.Copy();
		}

		/// <summary>
		/// Get current context.  Should only be used for querying. Use with caution
		/// </summary>
		internal SymbolContext<SymbolT,CtxKey> GetContext()
		{
			return current;
		}

		#endregion


		#region Contexts Management

		/// <summary>
		/// Enters the next context in the DAG, the first of the current context's children
		/// </summary>
		public string EnterContext()
		{
			return EnterContext(0);
		}

		/// <summary>
		/// Enters the child context with the given 'id'
		/// </summary>
		public string EnterContext(string id)
		{
			return EnterContext(current.GetChild(id));
		}

		/// <summary>
		/// Enters the child context in the given index
		/// </summary>
		public string EnterContext(int idx)
		{
			return EnterContext(current.GetChild(idx));
		}

		/// <summary>
		/// Enters the context passed as argument
		/// </summary>
		internal string EnterContext(SymbolContext<SymbolT,CtxKey> ctx)
		{
			path.Push(current);
			current = ctx;
			return ctx.Id;
		}


		/// <summary>
		/// Leaves the current Symbol context and switches to the last context
		/// </summary>
		public String ExitContext()
		{
			string currid = current.Id;
			current = path.Pop();
			return currid;
		}

		#endregion


		#region Management of symbols, Insertions and Lookups

		bool CheckValidId(String id)
		{
			if (id == null || id == "")
				return false;
			return true;
		}

		/// <summary>
		/// Recursive DFS from bottom to top (children to parents).
		/// (takes each parent in depth)
		/// </summary>
		internal SymbolT LookupRec(SymbolContext<SymbolT, CtxKey> ctx, String symbName)
		{
			SymbolT t = ctx.Lookup(symbName);
			if (t != null)
				return t;

			foreach (var p in ctx.parents)
				if ((t = LookupRec(p, symbName)) != null)
					return t;

			return null;
		}

		SymbolT LookupRec(SymbolContext<SymbolT, CtxKey> ctx, String symbName, CtxKey key)
		{
			SymbolT t;
			if (ReferenceEquals(ctx.Key,key))
				if ((t = ctx.Lookup(symbName)) != null)
					return t;

			foreach (var p in ctx.parents)
				if ((t = LookupRec(p, symbName)) != null)
					return t;

			return null;
		}

		public SymbolT Lookup(String symbName, CtxKey key = null)
		{
			if (!CheckValidId(symbName))
				return null;

			if (key == null)
				return LookupRec(current, symbName);
			else
				return LookupRec(current, symbName, key);
		}

		public SymbolT LookupCurrent(String symbName)
		{
			if (!CheckValidId(symbName))
				return null;
			return current.Lookup(symbName);
		}

		/// <summary>
		/// Checks if symbol can be added to the present context, by fulfilling the conditions:
		/// 1) not being already defined in the current context
		/// 2) not being defined in any parent context that does now allow shadowing
		/// </summary>
		public bool CanAddSymbol(String key)
		{
			if (!CheckValidId(key))
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
		public bool Add(String key, SymbolT symbol)
		{
			if (!CheckValidId(key))
				return false;

			if (!CanAddSymbol(key))
				return false;

			return current.Add(key, symbol);
		}

		/// <summary>
		/// Replaces (redefines) a symbol in the current context
		/// </summary>
		public bool Replace(String key, SymbolT symbol)
		{
			if (!CheckValidId(key))
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
		/// Recursively traverse the whole DAG, in a DFS from top to bottom, up to a height limit
		/// </summary>
		String OutputGraphTopDown(SymbolContext<SymbolT,CtxKey> ctx,  int maxheight)
		{
			string text = ctx.ListContext() + Environment.NewLine;
			if (maxheight > 0)
				foreach (var p in ctx.children)
					text += OutputGraphTopDown(p, maxheight - 1);
			return text;
		}

		internal String ListGraphFromRoot(int maxdepth = Int32.MaxValue)
		{
			string sep = Environment.NewLine;
			return ToString() + sep + OutputGraphTopDown(root, maxdepth) + sep;
		}


		/// <summary>
		/// Recursively traverse the whole DAG, in a DFS from bottom to top, up to a height limit
		/// </summary>
		String OutputGraphBottomUp(SymbolContext<SymbolT, CtxKey> ctx, int maxheight)
		{
			string text = ctx.ListContext() + Environment.NewLine;
			if (maxheight > 0)
				foreach (var p in ctx.parents)
					text += OutputGraphBottomUp(p, maxheight - 1);
			return text;
		}

		internal String ListGraphFromCurrent(int maxdepth = Int32.MaxValue)
		{
			string sep = Environment.NewLine;
			return ToString() + sep + OutputGraphBottomUp(current, maxdepth) + sep;
		}

		#endregion
	}



	/// <summary>
	/// Context of declared symbols.
	/// Implemented as a node of a DAG
	/// </summary>
	class SymbolContext<T,CtxKey> where T : class
	{
		internal List<SymbolContext<T, CtxKey>> parents;
		internal List<SymbolContext<T, CtxKey>> children;

		internal Dictionary<String, T> symbols;
		internal T lastInserted;

		internal bool allowShadowing;

		public CtxKey Key { get; set; }
		public String Id  { get; set; }

		const int DefaultAllocNLinks = 16;

		internal SymbolContext(List<SymbolContext<T,CtxKey>> parents, String id = null, 
									CtxKey key = default(CtxKey), bool allowShadowing = true)
		{
			this.Id = id;
			this.allowShadowing = allowShadowing;
			this.lastInserted = null;
			this.parents = parents;
			this.Key = key;
			children = new List<SymbolContext<T, CtxKey>>(DefaultAllocNLinks);
			symbols = new Dictionary<String, T>(32);
		}

		internal SymbolContext(String id = null, CtxKey key = default(CtxKey), bool allowShadowing = true)
			: this(new List<SymbolContext<T, CtxKey>>(DefaultAllocNLinks), id, key, allowShadowing)
		{
		}


		#region Access to Parents and Children

		internal SymbolContext<T, CtxKey> GetFirstParent()
		{
			return parents[0];
		}

		internal void AddParent(SymbolContext<T,CtxKey> parent)
		{
			parents.Add(parent);
			parent.children.Add(this);
		}

		internal void AddChild(SymbolContext<T,CtxKey> child)
		{
			child.parents.Add(this);
			children.Add(child);
		}

		internal SymbolContext<T,CtxKey> GetParent(int idx)
		{
			return parents.ElementAt(idx);
		}

		internal SymbolContext<T,CtxKey> GetParent(String id)
		{
			foreach (var c in parents)
				if (c.Id == id)
					return c;
			return null;
		}

		internal SymbolContext<T,CtxKey> GetChild(int idx)
		{
			return children.ElementAt(idx);
		}

		internal SymbolContext<T,CtxKey> GetChild(String id)
		{
			foreach (var c in children)
				if (c.Id == id)
					return c;
			return null;
		}

		#endregion


		#region Access and Lookup

		internal T Lookup(String key)
		{
			return (symbols.ContainsKey(key) ? symbols[key] : null);
		}

		/// <summary>
		/// Adds new or resets already added symbol
		/// </summary>
		internal bool Add(String key, T symbol)
		{
			// if (symbols.ContainsKey(key)) return false;

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
		/// Clones context without the parents or children
		/// </summary>
		internal SymbolContext<T,CtxKey> Clone(Func<T,bool> pred)
		{
			var ctx = new SymbolContext<T,CtxKey>(Id, Key, allowShadowing);
			ctx.lastInserted = lastInserted;

			ctx.symbols = new Dictionary<string, T>(symbols.Count);
			foreach (var s in symbols)
				if (pred(s.Value))
					ctx.symbols.Add(s.Key, s.Value);
			return ctx;
		}

		/// <summary>
		/// Returns shallow copy of context without the parents or children
		/// </summary>
		internal SymbolContext<T, CtxKey> Copy()
		{
			var ctx = new SymbolContext<T, CtxKey>(Id, Key, allowShadowing);
			ctx.lastInserted = lastInserted;
			ctx.symbols = symbols;
			return ctx;
		}

		public override string ToString()
		{
			return "Context " + Id + " with " + symbols.Count + " symbols";
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


