using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using crosspascal.ast.nodes;

namespace crosspascal.ast
{
	// Delivers a visitor pattern with which to traverse the AST

	class VisitorTraverser : Traverser
	{
		public VisitorTraverser() : base() { }

		public VisitorTraverser(Processor processor) : base(processor) { }

		public override bool traverse(Node n)
		{
			if (n == null)
				return true;
			else
				return n.Accept(Processor);

		}
	}
}
