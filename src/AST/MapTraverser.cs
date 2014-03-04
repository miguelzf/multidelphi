using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DGrok.Framework;
using System.Reflection;
using crosspascal.utils;

namespace crosspascal.AST
{
	// Delivers a Mapping with which to traverse the AST

	class MapTraverser : Traverser
	{
		Dictionary<System.Type, MethodInfo> methodMapping;

		private void CreateMappings(Processor proc)
		{
			methodMapping = new Dictionary<System.Type, MethodInfo>();

			Type procType = proc.GetType();

			// Add most methods - 1 argument, derived from AstNode
			foreach (MethodInfo mi in procType.GetMethods())
				if (mi.DeclaringType == procType && mi.GetParameters().Length == 1)
				{
					System.Type paramType = mi.GetParameters()[0].ParameterType;
					methodMapping.Add(paramType, mi);
				}
		}

		public override Processor Processor
		{
			set { base.Processor = value; CreateMappings(value); }
		}

		public MapTraverser() : base() { }

		public MapTraverser(Processor processor) : base(processor)	{}


		public override void traverse(AstNode n)
		{
			if (n == null)
				return;

			System.Type nodeType = n.GetType();

			if (nodeType.IsGenericType)
			{
				System.Type nodetype = n.GetType();
				System.Type paramType = nodetype.GenericTypeArguments[0];

				Logger.log.Write(nodetype.Name);

				if (nodetype.Name.StartsWith("ListNode"))
				{
				//	ListNode<AstNode> actualNode = (ListNode<AstNode> actualNode) Convert.ChangeType(n, nodetype);
					dynamic actualNode = n;
					Processor.VisitListNode(n, actualNode.ItemsAsBase);
				}
				else if (nodetype.Name.StartsWith("DelimitedItemNode"))
				{
				//	DelimitedItemNode<AstNode> actualNode = n as DelimitedItemNode<AstNode>;
					dynamic actualNode = n;
					Processor.VisitDelimitedItemNode(n, actualNode.ItemNode, actualNode.DelimiterNode);
				}
				else
				{	// should never happen
					n.Accept(Processor);	// fallback to basic visitor
				}
				return;
			}

			if (!methodMapping.ContainsKey(nodeType))
				return;		// method not mapped. Nothing to do

			MethodInfo mi = methodMapping[nodeType];
			mi.Invoke(Processor, new object[] { n });
		}
	}
}