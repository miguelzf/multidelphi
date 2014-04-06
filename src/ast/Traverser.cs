using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using crosspascal.ast.nodes;

namespace crosspascal.ast
{
	public abstract class Traverser<T>
	{
		public virtual Processor<T> Processor { get; set; }

		public Traverser() { }

		public Traverser(Processor<T> processor)
		{
			Processor = processor;
			processor.traverse = this.traverse;
		}

		public abstract T traverse(Node n);
	}
}
