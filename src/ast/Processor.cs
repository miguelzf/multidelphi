using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using crosspascal.ast.nodes;


//
// Implements a general AST Processor, without requiring the implementation of a Visitor pattern. 
// Uses a Delegate to a generic Tree Traversal method instead.
// The Traversing order needs to be defined by each Processor in the Process/Visit methods 
// since it is heavily depending on each Processor's logic.
//

namespace crosspascal.ast
{
	public delegate T TreeTraverse<T>(Node n);

	public abstract partial class Processor<T>
	{
		public TreeTraverse<T> traverse { get; set; }

		// backup of traverse function. 
		// Useful for processors that want to implement a strategy method pattern in the traverser
		protected TreeTraverse<T> realTraverse { get; set; }

		// dummy
		protected T emptyTraverse(Node n)
		{
			return default(T);
		}

		// Create with given traverser object
		public Processor(Traverser<T> trav)
		{
			traverse = trav.traverse;
		}

		/// <summary>
		/// Create with given traverser function, or with default (MapTraverser)
		/// </summary>
		public Processor(TreeTraverse<T> t = null)
		{
			if (t == null)
				traverse = new MapTraverser<T>(this).traverse;
			else
				traverse = t;
		}

		public virtual T Visit(Node node)
		{
			return default(T);
		}


		public abstract T DefaultReturnValue();

		/// <summary>
		/// Entry point
		/// </summary>
		public virtual T Process(Node n)
		{
			return traverse(n);
		}

	}
}
