using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MultiPascal.AST;
using MultiPascal.AST.Nodes;
using MultiPascal.Semantics;


namespace MultiPascal.Semantics
{
	class NestingProcessor : Processor<bool>
	{
		private Dictionary<string, string> replacements = new Dictionary<string, string>();

		private DeclarationsEnvironment declEnv;

		public NestingProcessor(Traverser<bool> t) : base(t) { }

		public NestingProcessor(TreeTraverse<bool> t = null) : base(t) { }

		public NestingProcessor(DeclarationsEnvironment env)
			: base((TreeTraverse<bool>) null)
		{
			this.declEnv = env;
		}


		public override bool DefaultReturnValue()
		{
			return true;
		}


		public override bool Process(Node n)
		{
			return traverse(n);
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
			CallableDeclaration insideFunction = declEnv.GetDeclaringRoutine();
			Section outside = insideFunction.declaringSection;

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


		#region Context Management

		public override bool Visit(ProgramNode node)
		{
			declEnv.EnterNextContext();
			traverse(node.section);
			return true;
		}

		public override bool Visit(LibraryNode node)
		{
			declEnv.EnterNextContext();
			traverse(node.section);
			return true;
		}

		public override bool Visit(UnitNode node)
		{
			declEnv.EnterNextContext();
			traverse(node.@interface);
			traverse(node.implementation);
			traverse(node.initialization);
			traverse(node.finalization);
			return true;
		}

		public override bool Visit(InterfaceSection node)
		{
			declEnv.EnterNextContext();
			Visit((TopLevelDeclarationSection)node);
			declEnv.EnterNextContext();
			return true;
		}

		public override bool Visit(ImplementationSection node)
		{
			declEnv.EnterNextContext();
			Visit((TopLevelDeclarationSection)node);
			declEnv.EnterNextContext();
			return true;
		}

		public override bool Visit(RoutineDeclaration node)
		{
			declEnv.EnterNextContext();
			Visit((CallableDeclaration)node);
			declEnv.EnterNextContext();
			return true;
		}

		public override bool Visit(RoutineDefinition node)
		{
			declEnv.EnterNextContext();
			Visit((CallableDeclaration)node);
			traverse(node.body);
			declEnv.EnterNextContext();
			return true;
		}

		public override bool Visit(MethodDeclaration node)
		{
			declEnv.EnterNextContext();
			Visit((CallableDeclaration)node);
			declEnv.EnterNextContext();
			return true;
		}

		public override bool Visit(MethodDefinition node)
		{
			declEnv.EnterNextContext();
			declEnv.EnterNextContext();
			Visit((CallableDeclaration)node);
			traverse(node.body);
			declEnv.EnterNextContext();
			declEnv.EnterNextContext();
			return true;
		}

		public override bool Visit(RoutineSection node)
		{
			declEnv.EnterNextContext();
			Visit((Section)node);
			traverse(node.block);
			declEnv.EnterNextContext();
			return true;
		}

		public override bool Visit(CompositeType node)
		{
			Visit((TypeNode)node);
			declEnv.EnterNextContext();
			traverse(node.section);
			declEnv.EnterNextContext();
			return true;
		}

		public override bool Visit(ClassRefType node)
		{
			//	Do not traverse this node! circular dependency
			//	traverse(node.reftype);
			return DefaultReturnValue();
		}

		#endregion

	} 
}
