using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace crosspascal.ast.nodes
{
	#region Statements hierarchy
	/// <remarks>
	///		Statement
	///			EmptyStmt
	///			LabeledStmt	- Label, Stmt 
	///			ProcCallStmt -RoutineCall expr
	///			Assign - lvalue, expression
	///			InheritedStmt - Statement 
	///			Block
	///			With
	///			AsmBlock
	///			If
	///			TryExcept
	///			TryFinally
	///			Raise
	///			Loop
	///				For
	///				While
	///				RepeatUntil
	///			Case
	///			ControlFlowStmt
	///				Break
	///				Continue
	///				Goto
	/// </remarks>
	#endregion

	public abstract class Statement : Node
	{

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

	public class Assignement : Statement
	{
		public LvalueExpression lvalue;
		public Expression expr;

		public Assignement(LvalueExpression lvalue, Expression expr)
		{
			this.lvalue = lvalue;
			this.expr = expr;
		}
	}

	public class GotoStatement : Statement
	{
		public string gotolabel;

		public GotoStatement(string label)
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

	public class ExpressionStatement : Statement
	{
		Expression expr;

		public ExpressionStatement(Expression expr)
		{
			this.expr = expr;
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

	public class ExceptionBlock : Node
	{
		public NodeList stmts;
		public Statement onElse;

		public ExceptionBlock(NodeList stmts, Statement onElse)
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

	public class CaseSelectorNode : Node
	{
		public NodeList list;
		public Statement stmt;

		public CaseSelectorNode(NodeList list, Statement stmt)
		{
			this.list = list;
			this.stmt = stmt;
		}
	}


	public class CaseStatement : Statement
	{
		public Expression condition;
		public NodeList selectors;
		public Statement caseelse;

		public CaseStatement(Expression condition, NodeList selectors, Statement caseelse)
		{
			this.condition = condition;
			this.selectors = selectors;
			this.caseelse = caseelse;
		}
	}

	public class LoopStatement : Statement
	{
		public Expression condition;
		public Statement block;

		public LoopStatement(Statement block, Expression condition)
		{
			this.condition = condition;
			this.block = block;
		}
	}

	public class RepeatLoop : LoopStatement
	{
		public RepeatLoop(Statement block, Expression condition) 
				: base(block, condition) { }
	}

	public class WhileLoop : LoopStatement
	{
		public WhileLoop(Expression condition, Statement block)
				: base(block, condition) { }
	}

	public class ForLoop : LoopStatement
	{
		public Identifier var;
		public Expression start;
		public Expression end;
		public int direction;

		public ForLoop(Identifier var, Expression start, Expression end, Statement body)
				: base(body, null)
		{
			this.var = var;
			this.start = start;
			this.end = end;
		}
	}

	public class BlockStatement : Statement
	{
		public StatementList stmts;

		protected BlockStatement() { }

		public BlockStatement(StatementList stmts)
		{
			this.stmts = stmts;
		}
	}

	public class WithStatement : BlockStatement
	{
		public Statement body;
		public Expression with;

		public WithStatement(Expression with, Statement body)
		{
			this.body = body;
			this.with = with;
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

	public class AssemblerBlock : BlockStatement
	{
		public NodeList asmInstrs;

		public AssemblerBlock(NodeList asm)
		{
			this.asmInstrs = asm;
		}
	}



}
