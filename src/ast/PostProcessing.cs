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

					if (fi == null || fi.Parent != null)
						continue;

					if (fi != null)
					{
						Type bt = f.FieldType;

						// ignore VariableType nodes, that may be reused. no single parent
						if (!typeof(VariableType).IsAssignableFrom(bt))
							fi.Parent = root;

						if (bt.BaseType.IsGenericType && bt.BaseType.Name.StartsWith("ListNode"))
						{
						//	Type genParamType = bt.BaseType.GetGenericArguments()[0];
						//	Type genListType  = typeof(ListNode<>).MakeGenericType(genParamType);
						//	object list = Convert.ChangeType(fi, genListType);

							if (fi is DeclarationList)
								SetParentsList<Declaration>(fi, root);
							else if (fi is ExpressionList)
								SetParentsList<Expression>(fi, root);
							else if (fi is StatementList)
								SetParentsList<Statement>(fi, root);
							else if (fi is FieldInitList)
								SetParentsList<FieldInit>(fi, root);
							else if (fi is EnumValueList)
								SetParentsList<EnumValue>(fi, root);
							else if (fi is NodeList)
								SetParentsList<Node>(fi, root);
						}
						else
							SetParents(fi, bt);
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
