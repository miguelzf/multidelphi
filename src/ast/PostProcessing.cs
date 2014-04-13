using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MultiPascal.AST.Nodes;
using System.Reflection;

namespace MultiPascal.AST
{
	/// <summary>
	/// Static methods that process the AST
	/// </summary>

	class PostProcessing
	{
		static public void GenericTraverse(Node node, Func<Node, Node, bool> evalFunc)
		{
			// do not recurse on Refs to classes or records, avoid circular deps
			if (node == null)
				return;

			Type ntype = node.GetType();
			if (ntype == typeof(ClassRefType) || ntype == typeof(RecordRefType))
				return;

			BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy;

			foreach (FieldInfo f in ntype.GetFields(flags))
				if (typeof(Node).IsAssignableFrom(f.FieldType))
				{
					Node fi = (Node) f.GetValue(node);

					if (fi != null)
					{
						Type bt = f.FieldType;

						if (!evalFunc(node, fi))
							continue;

						// Lists
						if (bt.BaseType.IsGenericType && bt.BaseType.Name.StartsWith("ListNode"))
						{
							var nodeslist = (IEnumerable<Node>)bt.GetField("nodes").GetValue(fi);
							foreach (Node n in nodeslist)
								if (evalFunc(fi, n))
									GenericTraverse(n, evalFunc);
						}

						// Non-lists
						else
							GenericTraverse(fi, evalFunc);
					}
				}
		}


		/// <summary>
		/// Sets parent back reference in each AST node through the reflection API
		/// </summary>
		public static void SetParents(Node root)
		{
			GenericTraverse(root,
				new Func<Node, Node, bool>(
					(par, node) => 
					{
						if (node is VariableType || node.Parent != null)
							return false;
						if (node is CompositeType && !(par is CompositeDeclaration))
							return false;
						if (node is Section && par is CallableDeclaration)
							return false;
						node.Parent = par;
						return true;
					} ));

			TestSetParents(root);
		}


		/// <summary>
		/// Sets parent back reference in each AST node through the reflection API
		/// </summary>
		public static void TestSetParents(Node root)
		{
			GenericTraverse(root, 
				new Func<Node, Node, bool>(
					(par, node) =>
					{
						if (node is CompositeType && !(par is CompositeDeclaration))
							return false;
						if (node is VariableType || par.Parent == node)		// called with parent ptr
							return false;
						if (node is Section && par is CallableDeclaration)
							return false;
						if (node.Parent == null)
							Console.Error.WriteLine("[ERROR testsetparents] parent "
													+ par.NodeName() + " is null in " + node);
						return true;
					}));
		}


	}
}
