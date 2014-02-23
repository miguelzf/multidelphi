using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DGrok.Framework;

namespace crosspascal.AST
{
	abstract class GenericTraverser
	{
		protected ASTProcessor Processor { get; set; }

		public GenericTraverser(ASTProcessor processor)
		{
			Processor = processor;
		}

		public abstract void traverse(AstNode n);
	}
}
