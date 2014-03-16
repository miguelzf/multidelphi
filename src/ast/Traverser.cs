﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using crosspascal.ast.nodes;

namespace crosspascal.ast
{
	public abstract class Traverser
	{
		public virtual Processor Processor { get; set; }

		public Traverser() { }

		public Traverser(Processor processor)
		{
			Processor = processor;
			processor.traverse = this.traverse;
		}

		public abstract bool traverse(Node n);
	}
}
