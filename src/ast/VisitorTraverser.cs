using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MultiPascal.AST.Nodes;

namespace MultiPascal.AST
{
	// Delivers a visitor pattern with which to traverse the AST

	class VisitorTraverser<T> : Traverser<T>
	{
		public VisitorTraverser() : base() { }

		public VisitorTraverser(Processor<T> processor) : base(processor) { }

		public override T traverse(Node n)
		{
			if (n == null)
				return default(T);
			return
				n.Accept<T>(Processor);
		}
	}
}
