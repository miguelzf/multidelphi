using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace crosspascal.ast.nodes
{

	public enum FunctionKind
	{
		Procedure,
		Function,
		Constructor,
		Destructor
	}

	public enum CallConvention
	{
		Pascal,
		SafeCall,
		StdCall,
		CDecl,
		Register
	}


	public class CallConventionNode : DelphiNode
	{
		public CallConvention convention;

		public CallConventionNode(CallConvention convention)
		{
			this.convention = convention;
		}
	}



	public abstract class DeclarationNode : DelphiNode
	{

	}
	public class ProcedureHeaderNode : DelphiNode
	{
		public bool isStatic;
		public FunctionKind kind;
		public IdentifierNode ident;
		public TypeNode returnType;
		public bool obj;
		public ParameterNodeList parameters;
		public ProcedureDirectiveList directives;

		public ProcedureHeaderNode(FunctionKind kind, IdentifierNode ident, ParameterNodeList parameters, TypeNode returnType, bool obj, ProcedureDirectiveList directives)
		{
			this.isStatic = false;
			this.kind = kind;
			this.ident = ident;
			this.obj = obj;
			this.parameters = parameters;
			this.returnType = returnType;
			this.directives = directives;
		}
	}

	public class ProcedureBodyNode : DelphiNode
	{
		public DeclarationListNode decls;
		public StatementBlock body;

		public ProcedureBodyNode(DeclarationListNode decls, StatementBlock body)
		{
			this.decls = decls;
			this.body = body;
		}
	}

	public class AssemblerProcedureBodyNode : ProcedureBodyNode
	{
		public AssemblerListNode asm;

		public AssemblerProcedureBodyNode(AssemblerListNode asm)
			: base(null, null)
		{
			this.asm = asm;
		}
	}

	public class ProcedureDefinitionNode : DelphiNode
	{
		public ProcedureHeaderNode header;
		public ProcedureBodyNode body;

		public ProcedureDefinitionNode(ProcedureHeaderNode header, ProcedureBodyNode body)
		{
			this.header = header;
			this.body = body;
		}
	}

	public enum ProcedureDirectiveEnum
	{
		Absolute,
		Abstract,
		Assembler,
		Dynamic,
		Export,
		Inline,
		Override,
		Overload,
		Reintroduce,
		External,
		Forward,
		Virtual,
		VarArgs
	}

	public class ProcedureDirective : DelphiNode
	{
		public ProcedureDirectiveEnum type;

		public ProcedureDirective(ProcedureDirectiveEnum type)
		{
			this.type = type;
		}
	}

	public class ProcedureDirectiveList : DelphiNode
	{
		public ProcedureDirective dir;
		public ProcedureDirectiveList next;

		public ProcedureDirectiveList(ProcedureDirective dir, ProcedureDirectiveList next)
		{
			this.dir = dir;
			this.next = next;
		}
	}

	public class ExternalProcedureDirective : ProcedureDirective
	{
		public IdentifierNode importLib;
		public string importName;

		public ExternalProcedureDirective(IdentifierNode importLib, string importName)
			: base(ProcedureDirectiveEnum.External)
		{
			this.importLib = importLib;
			this.importName = importName;
		}
	}

	public class ProcedurePointerDeclarationNode : DeclarationNode
	{
		public IdentifierListNode ids;
		public ProcedureHeaderNode proc;
		public ProcedureDirectiveList dirs;

		public ProcedurePointerDeclarationNode(IdentifierListNode ids, ProcedureHeaderNode proc, ProcedureDirectiveList dirs, DelphiNode init)
		{
			this.ids = ids;
			this.proc = proc;
			this.dirs = dirs;
			//this.init = init;
		}
	}

	public class ProcedureTypeDeclarationNode : TypeDeclarationNode
	{
		public ProcedureDirectiveList dirs;

		public ProcedureTypeDeclarationNode(IdentifierNode ident, TypeNode type, ProcedureDirectiveList dirs)
			: base(ident, type)
		{
			this.dirs = dirs;
		}
	}

	public abstract class ParameterQualifierNode : DelphiNode
	{
	}

	public class ConstParameterQualifier : ParameterQualifierNode
	{
	}

	public class VarParameterQualifier : ParameterQualifierNode
	{
	}

	public class OutParameterQualifier : ParameterQualifierNode
	{
	}

	public class ParameterNode : DelphiNode
	{
		public IdentifierListNode idlist;
		public IdentifierNode type;
		public ParameterQualifierNode qualifier;
		public DelphiExpression init;

		public ParameterNode(ParameterQualifierNode qualifier, IdentifierListNode idlist, IdentifierNode type, DelphiExpression init)
		{
			this.idlist = idlist;
			this.type = type;
			this.qualifier = qualifier;
			this.init = init;
		}
	}

	public class ParameterNodeList : DelphiNode
	{
		public ParameterNode param;
		public ParameterNodeList next;

		public ParameterNodeList(ParameterNode param, ParameterNodeList next)
		{
			this.param = param;
			this.next = next;
		}
	}


}