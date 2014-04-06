using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using crosspascal.ast;
using crosspascal.ast.nodes;
using crosspascal.semantics;

using LLVM;
using System.IO;
using System.Diagnostics;

namespace crosspascal.codegen.llvm
{

	static unsafe class CodeGenManager
    {
        public static Module Module { get; set; }
        public static IDictionary<string, Value> NamedValues { get; private set; }
        public static TextWriter ErrorOutput { get; set; }
        /// BinopPrecedence - This holds the precedence for each binary operator that is
        /// defined.
        public static IDictionary<char, int> BinopPrecendence = new Dictionary<char, int>();

        static CodeGenManager()
        {
            NamedValues = new Dictionary<string, Value>();
            ErrorOutput = Console.Out;

            // Install standard binary operators.
            // 1 is lowest precedence.
            BinopPrecendence['<'] = 10;
            BinopPrecendence['+'] = 20;
            BinopPrecendence['-'] = 20;
            BinopPrecendence['*'] = 40;  // highest.
        }
    }


	class LlvmILGen : Processor<LLVM.Value>
	{
		private int ident = 0;

		public LlvmILGen()
		{
		}

		private IRBuilder builder;
		private PassManager passManager;
		private ExecutionEngine execEngine;

		public override Value Process(Node n)
		{
			using(Module module = new Module("my cool jit"))
			using(builder = new IRBuilder())
			{
				CodeGenManager.Module = module;
				execEngine = new ExecutionEngine(module);
				passManager = new PassManager(module);
				passManager.AddTargetData(execEngine.GetTargetData());
				passManager.AddBasicAliasAnalysisPass();
				passManager.AddPromoteMemoryToRegisterPass();
				passManager.AddInstructionCombiningPass();
				passManager.AddReassociatePass();
				passManager.AddGVNPass();
				passManager.AddCFGSimplificationPass();
				passManager.Initialize();

				module.Dump();

				Function func = null;
				func.Dump();

				GenericValue val = execEngine.RunFunction(func, new GenericValue[0]);
				Console.WriteLine("Evaluated to " + val.ToReal().ToString());
			}

			return default(Value);
		}



		#region Load literals

		public override Value Visit(RealLiteral node)
		{
			return Value.CreateConstDouble(node.Value.Val<double>());
		}

		public override Value Visit(BoolLiteral node)
		{
			return Value.CreateConstBool(node.Value.Val<bool>());
		}

		public override Value Visit(IntLiteral node)
		{
			return Value.CreateConstUInt64(node.Value.Val<ulong>());
		}

		public override Value Visit(CharLiteral node)
		{
			return Value.CreateConstInt8((sbyte) node.Value.Val<char>());
		}

		public override Value Visit(PointerLiteral node)
		{
			return Value.CreateConstUInt64(node.Value.Val<ulong>());
		}

		// TODO strings

		#endregion


		/// VariableExprAST - Expression class for referencing a variable, like "a".

		public override Value Visit(Identifier node)
		{
			Value value = Value.Null;

			if(!CodeGenManager.NamedValues.TryGetValue(node.name, out value))
				CodeGenManager.ErrorOutput.WriteLine("Unknown variable name.");

			return builder.BuildLoad(value, node.name);
		}

		public override Value Visit(ArithmethicBinaryExpression node)
		{
			Value l = traverse(node.left);
			Value r = traverse(node.right);
			if (l.IsNull || r.IsNull) return Value.Null;


			switch(node.op)
			{
				case ArithmeticBinaryOp.SHR:
					return builder.BuildFAdd(l, r);
				default:
					return Value.Null;
			}
		}

/*
				case '<':
					// Convert bool 0/1 to double 0.0 or 1.0
					return builder.BuildFCmpAndPromote(l, LLVMRealPredicate.RealULT, 
													   r, TypeRef.CreateDouble());

			}
 * 
		/// BinaryExprAST - Expression class for a binary operator.
		public override Value Visit(BinaryExpression node)
		{
			Value l = traverse(node);
			Value r = node.RHS.CodeGen(builder);
			if(l.IsNull || r.IsNull) return Value.Null;

			switch(node.)
			{
				case '+':
					return builder.BuildFAdd(l, r);
				case '-':
					return builder.BuildFSub(l, r);
				case '*':
					return builder.BuildFMul(l, r);
				case '<':
					// Convert bool 0/1 to double 0.0 or 1.0
					return builder.BuildFCmpAndPromote(l, LLVMRealPredicate.RealULT, 
													   r, TypeRef.CreateDouble());
			}

			// If it wasn't a builtin binary operator, it must be a user defined one. Emit
			// a call to it.
			Function f = CodeGenManager.Module.GetFunction("binary" + node.Op);
			Debug.Assert(f != null);

			Value[] ops = new Value[] { l, r };
			Value ret = builder.BuildCall(f, ops, "binop");
			return true;
		}
	

		/// UnaryExprAST - Expression class for a unary operator.
		public char Op { get; set; }
		public ExprAST Operand { get; set; }

		public UnaryExprAST(char op, ExprAST operand)
		{
			node.Op = op;
			node.Operand = operand;
		}

		public override Value Visit(UnaryExpression node)
		{
			Value operandV = node.Operand.CodeGen(builder);
			if(operandV.IsNull) return operandV;

			Function f = CodeGenManager.Module.GetFunction("unary" + node.Op);
			Debug.Assert(f != null);

			Value[] ops = new Value[] { operandV };
			return builder.BuildCall(f, ops, "unop");
		}
	

		/// CallExprAST - Expression class for function calls.
		/// 
		public string Callee { get; set; }
		public List<ExprAST> Args { get; private set; }

		public CallExprAST(string callee, IEnumerable<ExprAST> args)
		{
			node.Callee = callee;
			node.Args = new List<ExprAST>(args);
		}

		public override Value Visit(RoutineCall node)
		{
			// Look up the name in the global module table.
			Function func = CodeGenManager.Module.GetFunction(node.Callee);
			if(func == null)
			{
				CodeGenManager.ErrorOutput.WriteLine("Unknown function referenced.");
				return Value.Null;
			}

			// If argument mismatch error.
			if(func.ArgCount != Args.Count)
			{
				CodeGenManager.ErrorOutput.WriteLine("Incorrect # arguments passed.");
				return Value.Null;
			}

			List<Value> args = new List<Value>();
			foreach(var arg in node.Args)
			{
				Value val = arg.CodeGen(builder);
				if(val.IsNull)
					return val;

				args.Add(val);
			}

			return builder.BuildCall(func, args.ToArray());
		}
	

		/// IfExprAST - Expression class for if/then/else.

		public ExprAST Cond { get; set; }
		public ExprAST Then { get; set; }
		public ExprAST Else { get; set; }

		public override Value Visit(IfStatement node)
		{
			Value condV = node.Cond.CodeGen(builder);
			if(condV.IsNull) return condV;

			condV = builder.BuildFCmp(condV, LLVMRealPredicate.RealONE, 
									  Value.CreateConstDouble(0));
			
			BasicBlock startBlock = builder.GetInsertPoint();
			Function func = startBlock.GetParent();

			BasicBlock thenBB = func.AppendBasicBlock("then");
			builder.SetInsertPoint(thenBB);

			Value thenV = node.Then.CodeGen(builder);
			if(thenV.IsNull) return thenV;
	  
			// Codegen of 'then' can change the current block, update then_bb for the
			// phi. We create a new name because one is used for the phi node, and the
			// other is used for the conditional branch.
			BasicBlock newThenBB = builder.GetInsertPoint();

			// Emit else block
			BasicBlock elseBB = func.AppendBasicBlock("else");
			func.AppendBasicBlock(elseBB);
			builder.SetInsertPoint(elseBB);

			Value elseV = node.Else.CodeGen(builder);
			if(elseV.IsNull) return elseV;

			// Codegen of 'Else' can change the current block, update ElseBB for the PHI.
			BasicBlock newElseBB = builder.GetInsertPoint();

			// Emit merge block
			BasicBlock mergeBB = func.AppendBasicBlock("ifcont");
			func.AppendBasicBlock(mergeBB);
			builder.SetInsertPoint(mergeBB);

			PhiIncoming incoming = new PhiIncoming();
			incoming.Add(thenV, thenBB);
			incoming.Add(elseV, elseBB);
			Value phi = builder.BuildPhi(TypeRef.CreateDouble(), "iftmp", incoming);

			builder.SetInsertPoint(startBlock);
			builder.BuildCondBr(condV, thenBB, elseBB);

			builder.SetInsertPoint(thenBB);
			builder.BuildBr(mergeBB);

			builder.SetInsertPoint(elseBB);
			builder.BuildBr(mergeBB);

			builder.SetInsertPoint(mergeBB);

			return phi;
		}
	


		/// forexpr ::= 'for' identifier '=' expr ',' expr (',' expr)? 'in' expression
		public string VarName { get; set; }
		public ExprAST Start { get; set; }
		public ExprAST End { get; set; }
		public ExprAST Step { get; set; }
		public ExprAST Body { get; set; }

		public override Value Visit(ForLoop node)
		{
			// Output this as:
			//   var = alloca double
			//   ...
			//   start = startexpr
			//   store start -> var
			//   goto loop
			// loop: 
			//   ...
			//   bodyexpr
			//   ...
			// loopend:
			//   step = stepexpr
			//   endcond = endexpr
			//
			//   curvar = load var
			//   nextvar = curvar + step
			//   store nextvar -> var
			//   br endcond, loop, endloop
			// outloop:

			BasicBlock startBlock = builder.GetInsertPoint();
			Function func = startBlock.GetParent();

			Value alloca = builder.BuildEntryBlockAlloca(func, TypeRef.CreateDouble(), node.VarName);

			Value startV = node.Start.CodeGen(builder);
			if(startV.IsNull) return startV;

			builder.BuildStore(startV, alloca);

			BasicBlock loopBB = func.AppendBasicBlock("loop");
			builder.BuildBr(loopBB);
			builder.SetInsertPoint(loopBB);

			// Within the loop, the variable is defined equal to the PHI node. If it
			// shadows an existing variable, we have to restore it, so save it
			// now.
			Value oldVal = Value.Null;
			CodeGenManager.NamedValues.TryGetValue(node.VarName, out oldVal);
			CodeGenManager.NamedValues[node.VarName] = alloca;

			// Emit the body of the loop.  This, like any other expr, can change the
			// current BB.  Note that we ignore the value computed by the body, but
			// don't allow an error 
			Body.CodeGen(builder);

			// Emit the step value;
			Value stepV = Value.Null;

			if(node.Step != null)
				stepV = node.Step.CodeGen(builder);
			else
				stepV = Value.CreateConstDouble(1);

			// Compute the end condition
			Value endCond = node.End.CodeGen(builder);
			endCond = builder.BuildFCmp(endCond, LLVMRealPredicate.RealONE, Value.CreateConstDouble(0), "loopcond");

			Value curvar = builder.BuildLoad(alloca, VarName);
			Value nextVar = builder.BuildFAdd(curvar, stepV, "nextvar");
			builder.BuildStore(nextVar, alloca);

			BasicBlock loopEndBB = builder.GetInsertPoint();
			BasicBlock afterBB = func.AppendBasicBlock("afterloop");
			builder.BuildCondBr(endCond, loopBB, afterBB);
			builder.SetInsertPoint(afterBB);

			if(!oldVal.IsNull)
				CodeGenManager.NamedValues[node.VarName] = oldVal;
			else
				CodeGenManager.NamedValues.Remove(node.VarName);

			return Value.CreateConstDouble(0);
		}



	/// PrototypeAST - This class represents the "prototype" for a function,
	/// which captures its name, and its argument names (thus implicitly the number
	/// of arguments the function takes).
		public string Name { get; set; }
		public List<string> Args { get; private set; }
		public bool IsOperator { get; set; }
		public int Precedence { get; set; }

		public bool IsUnaryOp
		{
			get { return node.IsOperator && node.Args.Count == 1; }
		}

		public bool IsBinaryOp
		{
			get { return node.IsOperator && node.Args.Count == 2; }
		}

		public char OperatorName
		{
			get
			{
				Debug.Assert(IsOperator);
				return node.Name[0];
			}
		}

		public PrototypeAST(string name, IEnumerable<string> args, bool isOp, int precedence)
		{
			node.Name = name;
			node.Args = new List<string>(args);
			node.IsOperator = isOp;
			node.Precedence = precedence;
		}

		public Function ProcessFuncProto(IRBuilder builder)
		{
			List<TypeRef> args = new List<TypeRef>();
			node.Args.ForEach(a => args.Add(TypeRef.CreateDouble()));

			Function func = new Function(CodeGenManager.Module, node.Name,
												 TypeRef.CreateDouble(), args.ToArray());
			func.SetLinkage(LLVMLinkage.ExternalLinkage);

			// If F conflicted, there was already something named 'Name'.  If it has a
			// body, don't allow redefinition or reextern.
			if(func.IsDuplicate())
			{
				// Delete the one we just made and get the existing one.
				func.Delete();
				func = CodeGenManager.Module.GetFunction(node.Name);

				// If F already has a body, reject node.
				if(func.HasBody)
				{
					CodeGenManager.ErrorOutput.WriteLine("redefinition of function.");
					return null;
				}

				// If F took a different number of args, reject.
				if(func.ArgCount != node.Args.Count)
				{
					CodeGenManager.ErrorOutput.WriteLine("redefinition of function with different # args.");
					return null;
				}
			}

			// Set names for all arguments.
			for(int i = 0; i < func.ArgCount; ++i)
			{
				Value val = func.GetParameter((uint)i);
				val.Name = node.Args[i];
			}

			return func;
		}

		public void CreateArgAllocas(Function function, IRBuilder builder)
		{
			for(int i = 0; i < function.ArgCount; ++i)
			{
				Value alloca = builder.BuildEntryBlockAlloca(function, TypeRef.CreateDouble(), node.Args[i]);
				builder.BuildStore(function.GetParameter((uint)i), alloca);
				CodeGenManager.NamedValues[node.Args[i]] = alloca;
			}
		}

	
		public PrototypeAST Proto { get; set; }
		public ExprAST Body { get; set; }


		public Function processFunction(IRBuilder builder, PassManager passManager)
		{
			CodeGenManager.NamedValues.Clear();
			Function func = node.Proto.CodeGen(builder);
			if(func == null)
				return null;

			// If this is an operator, install it.
			if(node.Proto.IsBinaryOp)
				CodeGenManager.BinopPrecendence[Proto.OperatorName] = Proto.Precedence;

			// Create a new basic block to start insertion into.
			BasicBlock bb = func.AppendBasicBlock("entry");
			builder.SetInsertPoint(bb);

			Proto.CreateArgAllocas(func, builder);

			Value retVal = Body.CodeGen(builder);

			if(!retVal.IsNull)
			{
				builder.BuildReturn(retVal);

				// Validate the generated code, checking for consistency.
				func.Validate(LLVMVerifierFailureAction.PrintMessageAction);

				// Optimize the function.
				passManager.Run(func);

				return func;
			}

			// Error reading body, remove function.
			func.Delete();
			return null;
		}
	};
		*/


		//
		// Processor interface
		//



	}
}
