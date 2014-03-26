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

			foreach (FieldInfo f in ntype.GetFields(flags))
				if (typeof(Node).IsAssignableFrom(f.FieldType))
				{
					Node fi = (Node) f.GetValue(root);
					if (fi != null)
					{
						fi.Parent = root;
						SetParents(fi, f.FieldType);
					}
				}
		}
	}
}
