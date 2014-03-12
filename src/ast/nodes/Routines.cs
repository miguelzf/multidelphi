using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace crosspascal.ast.nodes
{

	public enum RoutineReturnType
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


	public class CallConventionNode : Node
	{
		public CallConvention convention;

		public CallConventionNode(CallConvention convention)
		{
			this.convention = convention;
		}
	}



	public abstract class DeclarationNode : Node
	{

	}
	public class RoutineDeclaration : Node
	{
		public bool isStatic;
		public RoutineReturnType kind;
		public Identifier ident;
		public TypeNode returnType;
		public bool obj;
		public ParamDeclarationList parameters;
		public ProcedureDirectiveList directives;

		public RoutineDeclaration(RoutineReturnType kind, Identifier ident, ParamDeclarationList parameters, TypeNode returnType, bool obj, ProcedureDirectiveList directives)
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

	public class ProcedureBodyNode : Node
	{
		public DeclarationNodeList decls;
		public StatementBlock body;

		public ProcedureBodyNode(DeclarationNodeList decls, StatementBlock body)
		{
			this.decls = decls;
			this.body = body;
		}
	}

	public class AssemblerProcedureBodyNode : ProcedureBodyNode
	{
		public AssemblerNodeList asm;

		public AssemblerProcedureBodyNode(AssemblerNodeList asm)
			: base(null, null)
		{
			this.asm = asm;
		}
	}

	public class RoutineDefinition : Node
	{
		public RoutineDeclaration header;
		public ProcedureBodyNode body;

		public RoutineDefinition(RoutineDeclaration header, ProcedureBodyNode body)
		{
			this.header = header;
			this.body = body;
		}
	}

	public enum RoutineDirective
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

	public class ProcedureDirective : Node
	{
		public RoutineDirective type;

		public ProcedureDirective(RoutineDirective type)
		{
			this.type = type;
		}
	}

	public class ProcedureDirectiveList : Node
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
		public Identifier importLib;
		public string importName;

		public ExternalProcedureDirective(Identifier importLib, string importName)
			: base(RoutineDirective.External)
		{
			this.importLib = importLib;
			this.importName = importName;
		}
	}

	public class CallPointerDeclaration : DeclarationNode
	{
		public IdentifierList ids;
		public RoutineDeclaration proc;
		public ProcedureDirectiveList dirs;

		public CallPointerDeclaration(IdentifierList ids, RoutineDeclaration proc, ProcedureDirectiveList dirs, Node init)
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

		public ProcedureTypeDeclarationNode(Identifier ident, TypeNode type, ProcedureDirectiveList dirs)
			: base(ident, type)
		{
			this.dirs = dirs;
		}
	}

	public abstract class ParameterQualifierNode : Node
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

	public class ParamDeclaration : Node
	{
		public IdentifierList idlist;
		public Identifier type;
		public ParameterQualifierNode qualifier;
		public Expression init;

		public ParamDeclaration(ParameterQualifierNode qualifier, IdentifierList idlist, Identifier type, Expression init)
		{
			this.idlist = idlist;
			this.type = type;
			this.qualifier = qualifier;
			this.init = init;
		}
	}

	public class ParamDeclarationList : Node
	{
		public ParamDeclaration param;
		public ParamDeclarationList next;

		public ParamDeclarationList(ParamDeclaration param, ParamDeclarationList next)
		{
			this.param = param;
			this.next = next;
		}
	}


}