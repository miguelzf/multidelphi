using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using crosspascal.ast.nodes;
using System.Reflection;

namespace crosspascal.ast
{
	class PostProcessing
	{
		/// <summary>
		/// Sets parent back reference in each AST node through the reflection API
		/// </summary>
		public static void SetParents(Node root)
		{
			SetParents(root, root.GetType());
		}

		/// <summary>
		/// Internal implementation of SetParents
		/// </summary>
		static void SetParents(Node root, Type ntype)
		{
			BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy;

			// do not recurse on Refs to classes or records, avoid circular deps
			if (ntype == typeof(ClassRefType) || ntype == typeof(RecordRefType))
				return;

			foreach (FieldInfo f in ntype.GetFields(flags))
				if (typeof(Node).IsAssignableFrom(f.FieldType))
				{
					Node fi = (Node) f.GetValue(root);
					if (fi != null)
					{
						// ignore VariableType nodes, that may be reused. no single parent
						if (!typeof(VariableType).IsAssignableFrom(f.FieldType))
							fi.Parent = root;

						if (f.FieldType.BaseType.IsGenericType)
						{
							Type bt = f.FieldType;
						/* TODO
							if (bt.IsSubclassOf(ListNode<Node>))
								SetParentsList<Node>(fi, root);
							else if (f.FieldType is NodeList)
								SetParentsList<Node>(fi, root);
							if (f.FieldType is NodeList)
								SetParentsList<Node>(fi, root);
							if (f.FieldType is NodeList)
								SetParentsList<Node>(fi, root);
								
						*/
						}
						else
							SetParents(fi, f.FieldType);
					}
				}
		}

		static void SetParentsList<T>(Node nlist, Node par) where T : Node
		{
			ListNode<T> list = (ListNode<T>) nlist;

			foreach (Node n in list)
			{
				SetParents(n, n.GetType());
				n.Parent = par;
			}
		}
	}
}
