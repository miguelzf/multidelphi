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

	public delegate bool TreeTraverse(Node n);

	public abstract partial class Processor
	{
		public TreeTraverse traverse { get; set; }

		// dummy
		protected bool emptyTraverse(Node n)
		{
			return true;
		}

		// Instantiate Traverser class
		public Processor(System.Type t)
		{
			if (t == null || !t.IsSubclassOf(typeof(Traverser)))
				return;

			Traverser instance = (Traverser) Activator.CreateInstance(t, new object[] {this});
			traverse = instance.traverse;
		}

		// Create with given traverser object
		public Processor(Traverser trav)
		{
			traverse = trav.traverse;
		}

		/// <summary>
		/// Create with given traverser function, or with default (MapTraverser)
		/// </summary>
		public Processor(TreeTraverse t = null)
		{
			if (t == null)
				traverse = new MapTraverser(this).traverse;
			else
				traverse = t;
		}

		public virtual bool Visit(Node node)
		{
			return true;
		}


		/// <summary>
		/// Entry point
		/// </summary>
		public virtual bool StartProcessing(Node n)
		{
			return traverse(n);
		}

	}
}
