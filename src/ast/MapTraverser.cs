using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Runtime.Serialization;
using crosspascal.utils;
using crosspascal.ast.nodes;
using crosspascal.core;


namespace crosspascal.ast
{
	// Delivers a Mapping with which to traverse the AST

	class MapTraverser<T> : Traverser<T>
	{
		Dictionary<System.Type, MethodInfo> methodsMapping;

		private void CreateMappings(Processor<T> proc)
		{
			methodsMapping = new Dictionary<System.Type, MethodInfo>();

			Type nodeType = typeof(Node);
			Type procType = typeof(Processor<T>);
			BindingFlags bflags = BindingFlags.Public	| BindingFlags.NonPublic
								| BindingFlags.Instance | BindingFlags.FlattenHierarchy;

			// Add most methods - 1 argument, derived from AstNode
			foreach (MethodInfo mi in procType.GetMethods(bflags))
				if (procType.IsAssignableFrom(mi.DeclaringType) && mi.Name == "Visit")
				{
					ParameterInfo[] pars = mi.GetParameters();
					if (pars.Length == 1 && nodeType.IsAssignableFrom(pars[0].ParameterType))
						methodsMapping.Add(pars[0].ParameterType, mi);
				}
		}

		public override Processor<T> Processor
		{
			set { base.Processor = value; CreateMappings(value); }
		}

		public MapTraverser() : base() { }

		public MapTraverser(Processor<T> processor) : base(processor) { }


		public override T traverse(Node n)
		{
			if (n == null)
				return default(T);

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
				return DefaultReturnValue();		// method not mapped. Nothing to do

			try
			{
				Object oret = mi.Invoke(Processor, new object[] { n });

				if (oret == null)
				{
					Logger.log.Error("Process method " + mi + " invocation failed");
					return default(T);
				}

				return (T) oret;
			}
			catch (TargetInvocationException e)	// wrapper for an exception occurring in the invocation
			{
				var realExc = e.InnerException;

				if (realExc is CrossPascalException)
					throw realExc;
				else
					throw new Exception("wrapper", e.InnerException);


				// TODO
			/*
				var ctx = new StreamingContext(StreamingContextStates.CrossAppDomain);
				var mgr = new ObjectManager(null, ctx);
				var si = new SerializationInfo(e.InnerException.GetType(), new FormatterConverter());
				e.InnerException.GetObjectData(si, ctx);
				mgr.RegisterObject(e.InnerException, 1, si);
				mgr.DoFixups();
				throw e.InnerException;
			 */
			}

		}
	}
}