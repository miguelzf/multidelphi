using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using crosspascal.utils;
using crosspascal.ast.nodes;

namespace crosspascal.ast
{
	// Delivers a Mapping with which to traverse the AST

	class MapTraverser : Traverser
	{
		Dictionary<System.Type, MethodInfo> methodsMapping;

		private void CreateMappings(Processor proc)
		{
			methodsMapping = new Dictionary<System.Type, MethodInfo>();

			Type procType = proc.GetType();

			// Add most methods - 1 argument, derived from AstNode
			foreach (MethodInfo mi in procType.GetMethods(BindingFlags.FlattenHierarchy))
				if (mi.DeclaringType == procType && mi.Name == "Visit" && mi.GetParameters().Length == 1)
				{
					System.Type paramType = mi.GetParameters()[0].ParameterType;
					methodsMapping.Add(paramType, mi);
				}
		}

		public override Processor Processor
		{
			set { base.Processor = value; CreateMappings(value); }
		}

		public MapTraverser() : base() { }

		public MapTraverser(Processor processor) : base(processor)	{}


		public override bool traverse(Node n)
		{
			if (n == null)
				return true;

			System.Type nodeType = n.GetType();

			if (nodeType.IsGenericType)
			{
				System.Type nodetype = n.GetType();
				System.Type paramType = nodetype.GetGenericArguments()[0];

				Logger.log.Write(nodetype.Name);

		/*		if (nodetype.Name.StartsWith("ListNode"))
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
		 */
			}


			MethodInfo mi;
			if (!methodsMapping.TryGetValue(nodeType, out mi))
				return true;		// method not mapped. Nothing to do
			
			Object oret = mi.Invoke(Processor, new object[] { n });

			if (oret == null)
			{	Logger.log.Error("Process method " + mi + " invocation failed");
				return false;
			}

			return (bool) oret;
		}
	}
}