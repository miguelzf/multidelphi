using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using crosspascal.ast;
using crosspascal.ast.nodes;
using crosspascal.semantics;


namespace crosspascal.semantics
{
	class NestingProcessor : Processor<bool>
	{
		private Dictionary<string, string> replacements = new Dictionary<string, string>();

		private CallableDeclaration GetInsideFunction(Node node)
		{
			if (node == null)
				return null;

			if (node is CallableDeclaration)
				return node as CallableDeclaration;

			return GetInsideFunction(node.Parent);
		}

		private Section GetOutsideSection(Node node)
		{
			if (node == null)
				return null;

			if (node is Section)
			{
				Section result = node as Section;
				Section temp = GetOutsideSection(node.Parent);
				if (temp != null)
					return temp;
				else
					return result;			
			}

			return GetOutsideSection(node.Parent);
		}

		public override bool Visit(Identifier node)
		{
			if (node.name.Equals("b"))
			foreach (KeyValuePair<string, string> entry in replacements)
			{
				Console.WriteLine(entry.Key + " -> " + entry.Value);
			}



			if (replacements.ContainsKey(node.name))
				node.name = replacements[node.name];

			return true;
		}

		public override bool Visit(DeclarationList node)
		{
			CallableDeclaration insideFunction = GetInsideFunction(node);
			Section outside = GetOutsideSection(node);

			for (int i= node.nodes.Count-1; i>=0; i--)
			{
				if (insideFunction!=null && node.nodes[i] is CallableDeclaration)
				{
					string name = node.nodes[i].ToString();
					Console.WriteLine("Nesting: " + name);
					CallableDeclaration decl = node.nodes[i] as CallableDeclaration;

					string oldname = decl.name;
					string newname = insideFunction.name+ "_" + decl.name;
					replacements[oldname] = newname;
					decl.name =  newname;
					
					node.nodes.RemoveAt(i);
					if (!outside.decls.Contains(decl))
						outside.decls.AddStart(decl);

					traverse(decl);

					//replacements.Remove(oldname);
				}
				else
					traverse(node.nodes[i]);
			}
			return true;
		}

	} 
}
