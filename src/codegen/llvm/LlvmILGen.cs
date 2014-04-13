#define passes
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

	class LlvmILGen : Processor<LLVM.Value>
	{
		private int ident = 0;

		public LlvmILGen()
		{
			realTraverse = traverse;
			traverse = traverseDebug;
		}

		public override Value DefaultReturnValue()
		{
			return Value.Null;
		}


		LlvmIRBuilder builder;
		PassManager passManager;
		ExecutionEngine execEngine;
		Module module;

		// map of ID-declarations to LLVM values
		Dictionary<Declaration, LLVM.Value> values = new Dictionary<Declaration, Value>(1024);

		Function main;

		public override Value Process(Node n)
		{
			InitTypeMap();

			LLVM.Native.LinkInJIT();
			//LLVM.Native.InitializeNativeTarget(); // Declared in bindings but not exported from the shared library.
			LLVM.Native.InitializeX86TargetInfo();
			LLVM.Native.InitializeX86Target();
			LLVM.Native.InitializeX86TargetMC();

			using (module = new Module("llvm compiler"))
			using (builder = new LlvmIRBuilder())
			{
				execEngine = new ExecutionEngine(module);
			/*
				passManager = new PassManager(module);
				passManager.AddTargetData(execEngine.GetTargetData());
			
				// optimizations
			#if false
				passManager.AddBasicAliasAnalysisPass();
				passManager.AddPromoteMemoryToRegisterPass();
				passManager.AddInstructionCombiningPass();
				passManager.AddReassociatePass();
				passManager.AddGVNPass();
				passManager.AddCFGSimplificationPass();
			#endif
				passManager.Initialize();
			*/
				// main = EmitTestLLVMIR();
				Value valRet = traverse(n);

				module.Dump();

			//	if (valRet.Equals(Value.Null))
			//		return Value.Null;

				GenericValue val = execEngine.RunFunction(main, new GenericValue[0]);
				Console.WriteLine("Evaluated to " + val.ToUInt());
			}

			return default(Value);
		}


		#region Debug

		Function EmitTestLLVMIR()
		{
			/* define i32 @main() {
				entry:
				  %a = alloca i32
				  %c = alloca i32
				  %p = alloca i32*
				  store i32 1, i32* %a
				  store i32* %a, i32** %p
				  %0 = load i32** %p
				  %1 = load i32* %0
				  store i32 %1, i32* %c
				  ret i32 0
				}
			*/

			var tint = TypeRef.CreateInt32();
			TypeRef intptr = TypeRef.CreatePointer(tint);

			Function func = new Function(module, "main", tint, new TypeRef[0]);
			func.SetLinkage(LLVMLinkage.CommonLinkage);

			// Create a new basic block to start insertion into.
			BasicBlock bb = func.AppendBasicBlock("entry");
			builder.SetInsertPoint(bb);

			Value a = builder.BuildAlloca(tint, "a");	//  %a = alloca i32
			Value c = builder.BuildAlloca(tint, "c");	//  %c = alloca i32

			Value p = builder.BuildAlloca(intptr, "p");	//  %p = alloca i32*
			
			builder.BuildStore(Value.CreateConstInt32(111), a);	// store i32 1, i32* %a
			builder.BuildStore(a, p);							// store i32* %a, i32** %p
			// Value va = builder.BuildLoad(a);
			Value lp = builder.BuildLoad(p);					// %0 = load i32** %p
			Value llp= builder.BuildLoad(lp);					// %1 = load i32* %0
			builder.BuildStore(llp, c);

			builder.BuildReturn(builder.BuildLoad(c));
			return func;
		}

		bool Error(string msg, Node n = null)
		{
			string outp = "[ERROR in LLVM IR generator] " + msg;
			if (n != null)
				outp += n.Loc.ToString();

			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine(outp);
			Console.ResetColor();
			return false;
		}
		
		#endregion


		#region Helpers

		Dictionary<TypeNode, LLVM.TypeRef> typeMap;

		unsafe void InitTypeMap()
		{
			typeMap = new Dictionary<TypeNode, TypeRef>();

			// ints
			typeMap.Add(  SignedInt8Type.Single	, TypeRef.CreateInt8 ());
			typeMap.Add(  SignedInt16Type.Single, TypeRef.CreateInt16());
			typeMap.Add(  SignedInt32Type.Single, TypeRef.CreateInt32());
			typeMap.Add(  SignedInt64Type.Single, TypeRef.CreateInt64());
			typeMap.Add(UnsignedInt8Type.Single	, TypeRef.CreateInt8 ());
			typeMap.Add(UnsignedInt16Type.Single, TypeRef.CreateInt16());
			typeMap.Add(UnsignedInt32Type.Single, TypeRef.CreateInt32());
			typeMap.Add(UnsignedInt64Type.Single, TypeRef.CreateInt64());

			typeMap.Add(BoolType.Single,  new TypeRef(Native.Int1Type()));

			// reals
			typeMap.Add(FloatType.Single, TypeRef.CreateFloat());
			typeMap.Add(DoubleType.Single, TypeRef.CreateDouble());
			typeMap.Add(ExtendedType.Single, new TypeRef(Native.X86FP80Type()));
				// TODO change this type
			typeMap.Add(CurrencyType.Single, TypeRef.CreateDouble());

			// string with dynamic size. implemented as a char* for now
			typeMap.Add(StringType.Single, TypeRef.CreatePointer(TypeRef.CreateInt8()));
		}
		
		LLVM.TypeRef GetLLVMType(TypeNode type)
		{
			TypeRef llvmtype;

			if (type == null)	// Debug for now
				type = SignedInt32Type.Single;

			if (typeMap.TryGetValue(type, out llvmtype))
				return llvmtype;

			if (type is FixedStringType)
				return typeMap[StringType.Single];

			if (type is PointerType)
				return TypeRef.CreatePointer(GetLLVMType((type as PointerType).pointedType));

			// TODO finish
			Error("Non-implemented type: " + type, type);
			return TypeRef.Null;
		}

		#endregion




		public override Value Visit(ProgramSection node)
		{
			Function func = new Function(module, "main", TypeRef.CreateInt32(), new TypeRef[0]);
			//TypeRef.CreateVoid(), new TypeRef[0]);
			main = func;
			func.SetLinkage(LLVMLinkage.CommonLinkage);

			// Create a new basic block to start insertion into.
			BasicBlock bb = func.AppendBasicBlock("entry");
			builder.SetInsertPoint(bb);

			traverse(node.decls);
			traverse(node.block);

			Value ret = builder.BuildLoad(values.ElementAt(values.Count - 1).Value);
			//	return builder.BuildReturn(Value.CreateConstInt32(0));
			return builder.BuildReturn(ret);
			//	return builder.BuildReturn();
		}



		#region Declarations

		public override Value Visit(VarDeclaration node)
		{
			var llvmtype = GetLLVMType(node.type);
			var vdecl = builder.AddGlobal(module, llvmtype, node.name);
			Utils.SetInitializer(vdecl, llvmtype.CreateNullValue());

			//	var vdecl = builder.BuildAlloca(llvmtype, node.name);

			values.Add(node, vdecl);

			Console.WriteLine(node.name + " with LLVM TYpe: " + Utils.Tostring(llvmtype));
			return vdecl;
		}

		#endregion






		public override Value Visit(Assignment node)
		{
			Value rvalue = traverse(node.expr);
			Value lvalue = traverse(node.lvalue);

			if (rvalue.IsNull || lvalue.IsNull)
				return Value.Null;

			return builder.BuildStore(rvalue, lvalue);
		}

		#region Lvalues

		public override Value Visit(Identifier node)
		{
			// ID has been previously validated
			return values[node.decl];
		}

		public override Value Visit(AddressLvalue node)
		{
			Value addr = traverse(node.expr);
			// Address of an lvalue is an expression (rvalue). Load and it and leave
			return addr;
		}

		public override Value Visit(PointerDereference node)
		{
			Value arg = traverse(node.expr);
			return builder.BuildLoad(arg);
		}

		public override Value Visit(LvalueAsExpr node)
		{
			Value addr = traverse(node.lval);
			Value ret = builder.BuildLoad(addr);
			return ret;
		}

		// Load a value to use as address of a store
		public override Value Visit(ExprAsLvalue node)
		{
			return traverse(node.expr);
		}
		
		#endregion


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
			return Value.CreateConstUInt32((uint)node.Value.Val<ulong>());
		}

		public override Value Visit(CharLiteral node)
		{
			return Value.CreateConstInt8((sbyte)node.Value.Val<char>());
		}

		public override Value Visit(PointerLiteral node)
		{
			return Value.CreateConstUInt64(node.Value.Val<ulong>());
		}

		// TODO strings

		#endregion

		
		#region Expressions

		public override Value Visit(UnaryMinus node)
		{
			Value arg = traverse(node.expr);
			Debug.Assert(!arg.IsNull);

			// 0 - arg
			return builder.BuildSub(Value.CreateConstInt32(0), arg);
		}

		public override Value Visit(UnaryPlus node)
		{
			return traverse(node.expr);
		}

		public override Value Visit(LogicalNot node)
		{
			Value arg = traverse(node.expr);

			return builder.BuildNot(arg);
		}
		
		// Currently only working with ints
		public override Value Visit(ArithmethicBinaryExpression node)
		{
			Visit((BinaryExpression)node);
			Value l = traverse(node.left);
			Value r = traverse(node.right);

			if (l.IsNull || r.IsNull)
				return Value.Null;

			switch (node.op)
			{
				case ArithmeticBinaryOp.ADD:
					return builder.BuildAdd(l, r);
				case ArithmeticBinaryOp.SUB:
					return builder.BuildSub(l, r);
				case ArithmeticBinaryOp.MUL:
					return builder.BuildMul(l, r);
				case ArithmeticBinaryOp.DIV:
					return builder.BuildUDiv(l, r);
				case ArithmeticBinaryOp.QUOT:
					return builder.BuildMul(l, r);
				case ArithmeticBinaryOp.MOD:
					return builder.BuildURem(l, r);
				case ArithmeticBinaryOp.SHR:
					return builder.BuildLShr(l, r);
				case ArithmeticBinaryOp.SHL:
					return builder.BuildFAdd(l, r);

				default:	// never happens
					Error("Invalid arithmetic binary operator: " + node.op, node);
					return Value.Null;
			}
		}

		// Currently only working with ints
		public override Value Visit(LogicalBinaryExpression node)
		{
			Visit((BinaryExpression)node);
			Value l = traverse(node.left);
			Value r = traverse(node.right);

			if (l.IsNull || r.IsNull)
				return Value.Null;

			switch (node.op)
			{
				case LogicalBinaryOp.AND:
					return builder.BuildAnd(l, r);
				case LogicalBinaryOp.OR:
					return builder.BuildOr(l, r);
				case LogicalBinaryOp.XOR:
					return builder.BuildXor(l, r);
				case LogicalBinaryOp.EQ:
				case LogicalBinaryOp.NE:
				case LogicalBinaryOp.LE:
				case LogicalBinaryOp.LT:
				case LogicalBinaryOp.GE:
				case LogicalBinaryOp.GT:
					// LogicalBinaryOp values for comparison operands match LLVMIntPredicate values
					return builder.BuildICmp(l, (LLVMIntPredicate)node.op, r);

				default:	// never happens
					return Value.Null;
			}
		}

		#endregion


		public Value Visit(RoutineDeclaration node)
		{
			ProceduralType functype = node.Type;
			TypeRef llvmfrettype = (node.IsFunction? GetLLVMType(functype.funcret) : TypeRef.CreateVoid());

			var @params = functype.@params.decls.nodes.Cast<ParamDeclaration>();
			var args = @params.Select(a => GetLLVMType(a.type));

			Function func = new Function(module, node.name, llvmfrettype, args.ToArray());

			func.SetLinkage(LLVMLinkage.ExternalLinkage);

			// If F conflicted, there was already something named 'Name'.  If it has a
			// body, don't allow redefinition or reextern.
			if(func.IsDuplicate())
			{
				// Delete the one we just made and get the existing one.
				func.Delete();
				func = module.GetFunction(node.name);
			}

			// Set names for all arguments.
			uint i = 0;
			foreach (var param in @params)
			{
				Value val = func.GetParameter(i++);
				val.Name = param.name;	// calls llvm
			}

			return Value.Null;
		}

		/*
		public void CreateArgAllocas(Function function, IRBuilder builder)
		{
			for(int i = 0; i < function.ArgCount; ++i)
			{
				Value alloca = builder.BuildEntryBlockAlloca(function, TypeRef.CreateDouble(), node.Args[i]);
				builder.BuildStore(function.GetParameter((uint)i), alloca);
				values[node.Args[i]] = alloca;
			}
		}

		public Function Visit(RoutineDefinition node)
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

						// If it wasn't a builtin binary operator, it must be a user defined one. Emit a call to it.
			Function f = CodeGenManager.Module.GetFunction("binary" + node.Op);
			Debug.Assert(f != null);
			Value[] ops = new Value[] { l, r };
			Value ret = builder.BuildCall(f, ops, "binop");
			return true;


						// If it wasn't a builtin binary operator, it must be a user defined one. Emit a call to it.
			Function f = CodeGenManager.Module.GetFunction("binary" + node.Op);
			Debug.Assert(f != null);
			Value[] ops = new Value[] { l, r };
			Value ret = builder.BuildCall(f, ops, "binop");
			return true;


			return builder.BuildCall(func, args.ToArray());
		}
	*/


		 #region Statements 
	/*
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

	*/
		#endregion

	}
}
