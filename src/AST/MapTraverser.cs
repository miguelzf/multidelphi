using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DGrok.Framework;
using System.Reflection;

namespace crosspascal.AST
{
	// Delivers a Mapping with which to traverse the AST

	class MapTraverser : GenericTraverser
	{
		Dictionary<Type, MethodInfo> methodMapping;

		public MapTraverser(Processor processor) : base(processor)
		{
			methodMapping = new Dictionary<Type, MethodInfo>();

			// Add most methods - 1 argument, derived from AstNode
			foreach (MethodInfo mi in processor.GetType().GetMethods())
				if (mi.GetParameters().Length == 1)
				{
					Type paramType = mi.GetParameters()[0].ParameterType;
					methodMapping.Add(paramType, mi);
				}
		}

		public override void traverse(AstNode n)
		{
			if (n == null)
				return;

			if (n.GetType().IsGenericType)
			{
				Type nodetype = n.GetType();
				Type paramType = nodetype.GenericTypeArguments[0];

//				if (nodetype.FullName.Contains("ListNode"))
	
				n.Accept(Processor);	// fallback to basic visitor
				return;
			}

			MethodInfo mi = methodMapping[n.GetType()];
			mi.Invoke(Processor, new object[] { n });
		}
	}
}