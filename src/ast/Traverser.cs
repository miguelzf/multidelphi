using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MultiPascal.AST.Nodes;

namespace MultiPascal.AST
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

		public T DefaultReturnValue()
		{
			return Processor.DefaultReturnValue();
		}
	}
}
