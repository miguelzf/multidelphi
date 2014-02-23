using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DGrok.Framework;
using System.Reflection;

namespace crosspascal
{
	// Delivers a Mapping with which to traverse the AST

	class ASTMapTraverser
	{
		Dictionary<Type, MethodInfo> methodMapping;

		ASTProcessor Processor { get; set; }

		public ASTMapTraverser(ASTProcessor processor)
		{
			Processor = processor;

			methodMapping = new Dictionary<Type, MethodInfo>();

			// Add most methods - 1 argument, derived from AstNode
			foreach (MethodInfo mi in processor.GetType().GetMethods())
				if (mi.GetParameters().Length == 1)
				{
					Type paramType = mi.GetParameters()[0].ParameterType;
					methodMapping.Add(paramType, mi);
				}
				
			// Add multi-argument methods
			//... TODO
		}

		public void traverse(AstNode n)
		{
			MethodInfo mi = methodMapping[n.GetType()];
			mi.Invoke(Processor, new object[] { n });
		}
	}
}