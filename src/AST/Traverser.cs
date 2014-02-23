using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DGrok.Framework;

namespace crosspascal.AST
{
	abstract class Traverser
	{
		public virtual Processor Processor { get; set; }

		public Traverser() { }

		public Traverser(Processor processor)
		{
			Processor = processor;
			processor.traverse = this.traverse;
		}

		public abstract void traverse(AstNode n);
	}
}
