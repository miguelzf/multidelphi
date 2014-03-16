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

		// Create with given traverser function
		public Processor(TreeTraverse t = null)
		{
			if (t == null)
				traverse = emptyTraverse;
			else
				traverse = t;
		}

/*
		// =========================================================================
		//	Complete interface to be implemented by any specific AST processor
		// =========================================================================

		public override void VisitListNode(Node node, IEnumerable<Node> items)
		{
			foreach (Node item in items)
				traverse(item);
		}

		// Only called from Visit(CodeBase codeBase)
		public override void VisitSourceFile(string fileName, Node node)
		{
			traverse(node);
		}

*/
	}
}
