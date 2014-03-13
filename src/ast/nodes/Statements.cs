using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace crosspascal.ast.nodes
{

	public abstract class Statement : Node
	{
		public string label;
	}

	public class LabelStatement : Statement
	{
		public Identifier label;
		public Statement stmt;

		public LabelStatement(Identifier label, Statement stmt)
		{
			this.label = label;
			this.stmt = stmt;
		}
	}

	public class EmptyStatement : Statement
	{
		// Do nothing
	}

	public abstract class LabelNode : Node
	{
	}

	public class StringLabel : LabelNode
	{
		public Identifier name;

		public StringLabel(Identifier name)
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

	public class LabelDeclaration : DeclarationNode
	{
		public LabelNode label;
		public LabelDeclaration next;

		public LabelDeclaration(LabelNode label, LabelDeclaration next)
		{
			this.label = label;
			this.next = next;
		}
	}


	public class AssignementStatement : Statement
	{
		public LvalueExpression lvalue;
		public Expression expr;
		public bool inherited;

		public AssignementStatement(LvalueExpression lvalue, Expression expr, bool inherited)
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
		public Expression condition;
		public Statement ifTrue;
		public Statement ifFalse;

		public IfStatement(Expression condition, Statement ifTrue, Statement ifFalse)
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
		public Identifier ident;
		public Identifier type;
		public Statement body;

		public OnStatement(Identifier ident, Identifier type, Statement body)
		{
			this.ident = ident;
			this.type = type;
			this.body = body;
		}
	}

	public class OnNodeList : Node
	{
		public OnStatement stmt;
		public OnNodeList next;

		public OnNodeList(OnStatement stmt, OnNodeList next)
		{
			this.stmt = stmt;
			this.next = next;
		}
	}

	public class ExceptionBlock : Node
	{
		public OnNodeList stmts;
		public Statement onElse;

		public ExceptionBlock(OnNodeList stmts, Statement onElse)
		{
			this.stmts = stmts;
			this.onElse = onElse;
		}
	}

	public class RaiseStatement : Statement
	{
		public LvalueExpression lvalue;
		public Expression expr;

		public RaiseStatement(LvalueExpression lvalue, Expression expr)
		{
			this.lvalue = lvalue;
			this.expr = expr;
		}
	}

	public class CaseLabel : Node
	{
		public Expression minRange;
		public Expression maxRange;

		public CaseLabel(Expression minRange, Expression maxRange)
		{
			this.minRange = minRange;
			this.maxRange = maxRange;
		}
	}

	public class CaseLabelList : Node
	{
		public CaseLabel caselabel;
		public CaseLabelList next;

		public CaseLabelList(CaseLabel caselabel, CaseLabelList next)
		{
			this.caselabel = caselabel;
			this.next = next;
		}
	}

	public class CaseSelectorNode : Node
	{
		public CaseLabelList list;
		public Statement stmt;

		public CaseSelectorNode(CaseLabelList list, Statement stmt)
		{
			this.list = list;
			this.stmt = stmt;
		}
	}

	public class CaseSelectorList : Node
	{

	}

	public class CaseStatement : Statement
	{
		public Expression condition;
		public CaseSelectorList selectors;
		public Statement caseelse;

		public CaseStatement(Expression condition, CaseSelectorList selectors, Statement caseelse)
		{
			this.condition = condition;
			this.selectors = selectors;
			this.caseelse = caseelse;
		}
	}

	public class RepeatLoop : Statement
	{
		public Expression condition;
		public Statement block;

		public RepeatLoop(Statement block, Expression condition)
		{
			this.condition = condition;
			this.block = block;
		}
	}

	public class WhileLoop : Statement
	{
		public Expression condition;
		public Statement block;

		public WhileLoop(Expression condition, Statement block)
		{
			this.condition = condition;
			this.block = block;
		}
	}







	public class BlockStatement : Statement
	{
		public Statement stmt;
		public BlockStatement next;

		public BlockStatement(Statement stmt, BlockStatement next)
		{
			this.stmt = stmt;
			this.next = next;
		}
	}

	public class WithStatement : Statement
	{
		public Statement body;
		public Expression with;

		public WithStatement(Expression with, Statement body)
		{
			this.body = body;
			this.with = with;
		}
	}

	public class ForLoop : Statement
	{
		public Statement body;
		public Identifier var;
		public Expression start;
		public Expression end;
		public int direction;

		public ForLoop(Identifier var, Expression start, Expression end, Statement body)
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

	public class AssemblerNodeList : Node
	{
		public string asmop;
		public AssemblerNodeList next;

		public AssemblerNodeList(string asmop, AssemblerNodeList next)
		{
			this.asmop = asmop;
			this.next = next;
		}
	}




}
