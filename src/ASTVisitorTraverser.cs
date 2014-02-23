using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DGrok.Framework;

namespace crosspascal
{
	// Delivers a visitor pattern with which to traverse the AST

	class ASTVisitorTraverser
	{
		ASTProcessor Processor { get; set; }

		public ASTVisitorTraverser(ASTProcessor processor)
		{
			Processor = processor;
		}

		public void traverse(AstNode n)
		{
			n.Accept(Processor);
		}
	}
}
