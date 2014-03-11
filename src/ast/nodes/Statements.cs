using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace crosspascal.ast.nodes
{

	public class Statement : DelphiNode
	{
		public string label;

		public void SetLabel(string label)
		{
			this.label = label;
		}
	}


	public abstract class LabelNode : DelphiNode
	{
	}

	public class StringLabel : LabelNode
	{
		public IdentifierNode name;

		public StringLabel(IdentifierNode name)
		{
			this.name = name;
		}
	}

	public class NumberLabel : LabelNode
	{
		public int number;

		public NumberLabel(int number)
		{
			this.number = number;
		}
	}

	public class LabelDeclarationNode : DeclarationNode
	{
		public LabelNode label;
		public LabelDeclarationNode next;

		public LabelDeclarationNode(LabelNode label, LabelDeclarationNode next)
		{
			this.label = label;
			this.next = next;
		}
	}


	public class AssignementStatement : Statement
	{
		public LValueNode lvalue;
		public DelphiExpression expr;
		public bool inherited;

		public AssignementStatement(LValueNode lvalue, DelphiExpression expr, bool inherited)
		{
			this.lvalue = lvalue;
			this.expr = expr;
			this.inherited = inherited;
		}
	}

	public class GotoStatement : Statement
	{
		public LabelNode gotolabel;

		public GotoStatement(LabelNode label)
		{
			this.gotolabel = label;
		}
	}

	public class IfStatement : Statement
	{
		public DelphiExpression condition;
		public Statement ifTrue;
		public Statement ifFalse;

		public IfStatement(DelphiExpression condition, Statement ifTrue, Statement ifFalse)
		{
			this.condition = condition;
			this.ifTrue = ifTrue;
			this.ifFalse = ifFalse;
		}
	}

	public class InheritedStatement : Statement
	{
		public Statement body;

		public InheritedStatement(Statement body)
		{
			this.body = body;
		}
	}

	public class OnStatement : Statement
	{
		public IdentifierNode ident;
		public IdentifierNode type;
		public Statement body;

		public OnStatement(IdentifierNode ident, IdentifierNode type, Statement body)
		{
			this.ident = ident;
			this.type = type;
			this.body = body;
		}
	}

	public class OnListNode : DelphiNode
	{
		public OnStatement stmt;
		public OnListNode next;

		public OnListNode(OnStatement stmt, OnListNode next)
		{
			this.stmt = stmt;
			this.next = next;
		}
	}

	public class ExceptionBlockNode : DelphiNode
	{
		public OnListNode stmts;
		public Statement onElse;

		public ExceptionBlockNode(OnListNode stmts, Statement onElse)
		{
			this.stmts = stmts;
			this.onElse = onElse;
		}
	}

	public class RaiseStatement : Statement
	{
		public LValueNode lvalue;
		public DelphiExpression expr;

		public RaiseStatement(LValueNode lvalue, DelphiExpression expr)
		{
			this.lvalue = lvalue;
			this.expr = expr;
		}
	}

	public class CaseLabel : DelphiNode
	{
		public DelphiExpression minRange;
		public DelphiExpression maxRange;

		public CaseLabel(DelphiExpression minRange, DelphiExpression maxRange)
		{
			this.minRange = minRange;
			this.maxRange = maxRange;
		}
	}

	public class CaseLabelList : DelphiNode
	{
		public CaseLabel caselabel;
		public CaseLabelList next;

		public CaseLabelList(CaseLabel caselabel, CaseLabelList next)
		{
			this.caselabel = caselabel;
			this.next = next;
		}
	}

	public class CaseSelectorNode : DelphiNode
	{
		public CaseLabelList list;
		public Statement stmt;

		public CaseSelectorNode(CaseLabelList list, Statement stmt)
		{
			this.list = list;
			this.stmt = stmt;
		}
	}

	public class CaseSelectorList : DelphiNode
	{

	}

	public class CaseStatement : Statement
	{
		public DelphiExpression condition;
		public CaseSelectorList selectors;
		public Statement caseelse;

		public CaseStatement(DelphiExpression condition, CaseSelectorList selectors, Statement caseelse)
		{
			this.condition = condition;
			this.selectors = selectors;
			this.caseelse = caseelse;
		}
	}

	public class RepeatStatement : Statement
	{
		public DelphiExpression condition;
		public Statement block;

		public RepeatStatement(Statement block, DelphiExpression condition)
		{
			this.condition = condition;
			this.block = block;
		}
	}

	public class WhileStatement : Statement
	{
		public DelphiExpression condition;
		public Statement block;

		public WhileStatement(DelphiExpression condition, Statement block)
		{
			this.condition = condition;
			this.block = block;
		}
	}







	public class StatementBlock : Statement
	{
		public Statement stmt;
		public StatementBlock next;

		public StatementBlock(Statement stmt, StatementBlock next)
		{
			this.stmt = stmt;
			this.next = next;
		}
	}

	public class WithStatement : Statement
	{
		public Statement body;
		public DelphiExpression with;

		public WithStatement(DelphiExpression with, Statement body)
		{
			this.body = body;
			this.with = with;
		}
	}

	public class ForStatement : Statement
	{
		public Statement body;
		public IdentifierNode var;
		public DelphiExpression start;
		public DelphiExpression end;
		public int direction;

		public ForStatement(IdentifierNode var, DelphiExpression start, DelphiExpression end, Statement body)
		{
			this.body = body;
			this.var = var;
			this.start = start;
			this.end = end;
		}
	}

	public class TryFinallyStatement : Statement
	{
		public Statement body;
		public Statement final;

		public TryFinallyStatement(Statement body, Statement final)
		{
			this.body = body;
			this.final = final;
		}
	}

	public class TryExceptStatement : Statement
	{
		public Statement body;
		public Statement final;

		public TryExceptStatement(Statement body, Statement final)
		{
			this.body = body;
			this.final = final;
		}
	}

	public class AssemblerListNode : DelphiNode
	{
		public string asmop;
		public AssemblerListNode next;

		public AssemblerListNode(string asmop, AssemblerListNode next)
		{
			this.asmop = asmop;
			this.next = next;
		}
	}




}
