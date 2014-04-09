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

		String ident;
		ContextEnumerator DefaultContextTraverser;

		/// <summary>
		/// </summary>
		public void Reset()
		{
			current = root;
			path = new Stack<SymbolContext<SymbolT, CtxKey>>(1024);
			DefaultContextTraverser = null;
			ident = "";
		}

		public CtxKey CurrentCtxKey()
		{
			return current.Key;
		}

		void Debug(String msg)
		{
		//	Console.WriteLine(msg);
		}


		#region Context Tree Traversing
		//
		// Traverse the whole Context Tree, from the current context to the bottom,
		// entering and exiting each context in turn
		//

		/// <summary>
		/// Restart context iteration
		/// </summary>
		public bool RestartContextIterator()
		{
			DefaultContextTraverser = null;
			return EnterNextContext();
		}

		/// <summary>
		/// Enter next context, starting from the current if the traversing has not been started
		/// </summary>
		public bool EnterNextContext()
		{
			if (DefaultContextTraverser == null)
				DefaultContextTraverser = GetContextEnumerator();
			return DefaultContextTraverser.MoveToNextContext();
		}

		/// <summary>
		/// Wrapper to emulate a co-routine to load each context in turn
		/// </summary>
		public class ContextEnumerator
		{
			IEnumerator<bool> contextsEnum;

			public ContextEnumerator(SymbolGraph<SymbolT, CtxKey> graph)
			{
				contextsEnum = graph.GenEnterContexts().GetEnumerator();
			}

			public bool MoveToNextContext()
			{
				return contextsEnum.MoveNext();
			}
		}

		internal ContextEnumerator GetContextEnumerator()
		{
			return new ContextEnumerator(this);
		}

		/// <summary>
		/// Co-routine/generator to enter each context in each, by enumerating all.
		/// Starts in current context.
		/// Return value should be discarded.
		/// </summary>
		internal IEnumerable<bool> GenEnterContexts()
		{
			return GenEnterContexts(current);
		}

		/// <summary>
		/// Co-routine/generator to enter each context in each, by enumerating all
		/// Return value should be discarded
		/// </summary>
		internal IEnumerable<bool> GenEnterContexts(SymbolContext<SymbolT, CtxKey> ctx)
		{
			foreach (var child in ctx.children)
			{
				ident += "  ";
				Debug(ident + "Loaded " + child);
				EnterContext(child);
				yield return true;

				foreach (var c in GenEnterContexts(child))
					yield return c;

				ExitContext();
 				Debug(ident + "Exited " + child + " back to " + current);
				ident = ident.Substring(0, ident.Length - 2);
				yield return false;
			}
		}

		#endregion


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
			EnterContext(ctx);
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

		delegate IEnumerable<SymbolContext<SymbolT, CtxKey>> TraverseFunc(SymbolContext<SymbolT, CtxKey> ctx);

		String OutputGraph(TraverseFunc traverseFunc, SymbolContext<SymbolT, CtxKey> ctx,
									int depth, int maxh, bool symbs)
		{
			string text = (symbs ? ctx.ListContext() : ctx.ToString());
			text = String.Concat(Enumerable.Repeat("  ", depth)) + text + Environment.NewLine;
			if (maxh > 0)
				foreach (var p in traverseFunc(ctx))
					text += OutputGraph(traverseFunc, p, depth + 1, maxh - 1, symbs);
			return text;
		}

		/// <summary>
		/// Recursively traverse the whole DAG, in a DFS from top to bottom, up to a height limit
		/// </summary>
		internal String ListTreeFromRoot(int maxdepth = Int32.MaxValue, bool symbs = true)
		{
			string sep = Environment.NewLine;
			string graph = OutputGraph(new TraverseFunc((ctx) => { return ctx.children; }),
										root, 0, maxdepth, symbs);
			return this + sep + graph + sep;
		}
		/// <summary>
		/// Recursively traverse the whole DAG, in a DFS from bottom to top, up to a height limit
		/// </summary>
		internal String ListTreeFromCurrent(int maxdepth = Int32.MaxValue, bool symbs = true)
		{
			string sep = Environment.NewLine;
			string graph = OutputGraph(new TraverseFunc((ctx) => { return ctx.parents; }),
										current, 0, maxdepth, symbs);
			return this + sep + graph + sep;
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
			string keyname = "";
			if (Key != null)
			{
				keyname = Key.ToString();
				keyname = keyname.Substring(keyname.LastIndexOf('.')+1);
			}

			return "Context " + Id + ", key " + keyname + " with " + symbols.Count + " symbols";
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


