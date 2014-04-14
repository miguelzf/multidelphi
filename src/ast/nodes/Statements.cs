using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MultiPascal.Parser;
using MultiPascal.Semantics;

namespace MultiPascal.AST.Nodes
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
		public virtual bool IsEmpty { get { return false; } }
	}

	public class LabelStatement : Statement
	{
		public String label;
		public Statement stmt;

		// to be set by the resolver
		public LabelDeclaration decl;

		public LabelStatement(String label, Statement stmt)
		{
			this.label = label;
			this.stmt = stmt;
		}
	}


	public class GotoStatement : Statement
	{
		public String gotolabel;

		// to be set by the resolver
		public LabelDeclaration decl;

		public GotoStatement(String label)
		{
			this.gotolabel = label;
		}
	}

	public class EmptyStatement : Statement
	{
		public override bool IsEmpty { get { return true; } }
	}

	public class BreakStatement : Statement
	{
	}

	public class ContinueStatement : Statement
	{
	}

	public class Assignment : Statement
	{
		public LvalueExpression lvalue;
		public Expression expr;

		public Assignment(LvalueExpression lvalue, Expression expr)
		{
			this.lvalue = lvalue;
			this.expr = expr;
		}
	}

	public class IfStatement : Statement
	{
		public Expression condition;
		public Statement thenblock;
		public Statement elseblock;

		public IfStatement(Expression condition, Statement ifTrue, Statement ifFalse)
		{
			this.condition = condition;
			this.thenblock = ifTrue;
			this.elseblock = ifFalse;
			if (elseblock == null)
				elseblock = new EmptyStatement();
		}
	}

	public class ExpressionStatement : Statement
	{
		public Expression expr;

		public ExpressionStatement(Expression expr)
		{
			this.expr = expr;
		}
	}

	public class CaseSelector : Statement
	{
		public ExpressionList list;
		public Statement stmt;

		public CaseSelector(ExpressionList list, Statement stmt)
		{
			this.list = list;
			this.stmt = stmt;
		}
	}

	public class CaseStatement : Statement
	{
		public Expression condition;
		public StatementList selectors;
		public Statement caseelse;

		public CaseStatement(Expression condition, StatementList selectors, Statement caseelse)
		{
			this.condition = condition;
			this.selectors = selectors;
			this.caseelse = caseelse;
		}
	}


	#region Loops

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

		public ForLoop(Identifier var, Expression start, Expression end, Statement body, int dir)
				: base(body, null)
		{
			this.var = var;
			this.start = start;
			this.end = end;
			direction = dir;
		}
	}

	#endregion


	public class BlockStatement : Statement
	{
		public StatementList stmts;

		protected BlockStatement() { }

		public BlockStatement(StatementList stmts)
		{
			this.stmts = stmts;
		}
	}

	public class WithStatement : Statement
	{
		public ExpressionList with;
		public Statement body;

		public WithStatement(ExpressionList with, Statement body)
		{
			this.with = with;
			this.body = body;
		}
	}


	#region Exception handling

	public class TryFinallyStatement : Statement
	{
		public BlockStatement body;
		public BlockStatement final;

		public TryFinallyStatement(BlockStatement body, BlockStatement final)
		{
			this.body = body;
			this.final = final;
		}
	}

	public class TryExceptStatement : Statement
	{
		public BlockStatement body;
		public ExceptionBlock final;

		public TryExceptStatement(BlockStatement body, ExceptionBlock final)
		{
			this.body = body;
			this.final = final;
		}
	}

	public class ExceptionBlock : Statement
	{
		public StatementList onList;
		public BlockStatement @default;	// else or default, same semantics
		
		public ExceptionBlock(StatementList onList, BlockStatement @default = null)
		{
			this.onList = onList;
			this.@default = @default;
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

	public class OnStatement : Statement
	{
		public String ident;
		public String type;
		public Statement body;

		public OnStatement(String ident, String type, Statement body)
		{
			this.ident = ident;
			this.type = type;
			this.body = body;
		}
	}

	#endregion

	public class AssemblerBlock : BlockStatement
	{
		public AssemblerBlock(StatementList asmInstrs)
			: base(asmInstrs)
		{
		}
	}

}
