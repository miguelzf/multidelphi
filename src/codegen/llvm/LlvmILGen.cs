#define passes
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MultiPascal.AST;
using MultiPascal.AST.Nodes;
using MultiPascal.Semantics;

using LLVM;
using System.IO;
using System.Diagnostics;

namespace MultiPascal.Codegen.LlvmIL
{

	class LlvmILGen : Processor<LLVM.Value>
	{
		LlvmIRBuilder builder;
		ExecutionEngine execEngine;
		Module module;
		PassManager passManager;
		LLVMValueMap valuemap = new LLVMValueMap();
		GeneratorContext evalCtx = new GeneratorContext();

		public LlvmILGen()
		{
			realTraverse = traverse;
		//	traverse = traverseDebug;
		}

		public override Value DefaultReturnValue()
		{
			return Value.NonNull;
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


		#region Map of LLVM values

		class LLVMValueMap
		{

			// Global Map of all ID-declarations to LLVM values.
			// All the declarations must have been previously resolved and checked,
			// hence there should not occurr any conflicts now
			Dictionary<Declaration, Value> values
				= new Dictionary<Declaration, Value>(8 * 1024);

			unsafe String PtrToStr(Value val)
			{
				return "" + (ulong)val.Handle;
			}
			internal bool AddValue(Declaration d, Value val)
			{
				//	Console.WriteLine("ADD DECL " + d.DeclName() + " VAL " + val.ToString() + " HAND: " + PtrToStr(val));
				values.Add(d, val);
				return true;
			}

			internal Value GetValue(Declaration d)
			{
				Value val = null;
				bool ret = values.TryGetValue(d, out val);
				Debug.Assert(!ret || val != null, "LLVM value not found for declaration: " + d.DeclName());
				return val;
			}

			internal Value TryGetValue(Declaration d)
			{
				Value val = null;
				bool ret = values.TryGetValue(d, out val);
				if (ret)return val;
				else	return Value.Null;
			}
		}
		#endregion


		#region Generator Context

		// Ad-hoc eval context. quick and dirty hack. FIXME
		class GeneratorContext
		{
			// ProgramaSection return == main function
			internal Function main;

			// Marks if we are currently evaluating top-level declarations
			internal bool inTopLevel;

			// Current function being evaluated.
			internal Function func;

			// BasicBlock labels to jump to in continues and breaks
			Stack<BasicBlock> continuebbs;
			Stack<BasicBlock> breakbbs;

			internal BasicBlock ContinueBB	{ get { return continuebbs.Peek(); } }
			internal BasicBlock BreakBB		{ get { return	  breakbbs.Peek(); } }

			internal GeneratorContext()
			{
				continuebbs = new Stack<BasicBlock>(16);
				breakbbs = new Stack<BasicBlock>(16);
				main = null;
				func = null;
				inTopLevel = true;
			}

			internal void Reset()
			{
				inTopLevel = true;
				continuebbs.Clear();
				breakbbs.Clear();
			}

			internal void EnterLoop(BasicBlock loopstart, BasicBlock loopend)
			{
				continuebbs.Push(loopstart);
				breakbbs.Push(loopend);
			}

			internal void LeaveLoop()
			{
				continuebbs.Pop();
				breakbbs.Pop();
			}
		}

		#endregion


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
			
			// main = EmitTestLLVMIR();
				Value valRet = traverse(n);

				module.Dump();

			//	if (valRet.Equals(Value.Null))
			//		return Value.Null;

				GenericValue val = execEngine.RunFunction(evalCtx.main, new GenericValue[0]);
				Console.WriteLine("Evaluated to " + val.ToUInt());
			}

			return default(Value);
		}


		#region Handlers of LLVM Types

		Dictionary<TypeNode, LLVM.TypeRef> typeMap;

		unsafe void InitTypeMap()
		{
			typeMap = new Dictionary<TypeNode, TypeRef>();

			// ints
			typeMap.Add(SignedInt8Type.Single, TypeRef.CreateInt8());
			typeMap.Add(SignedInt16Type.Single, TypeRef.CreateInt16());
			typeMap.Add(SignedInt32Type.Single, TypeRef.CreateInt32());
			typeMap.Add(SignedInt64Type.Single, TypeRef.CreateInt64());
			typeMap.Add(UnsignedInt8Type.Single, TypeRef.CreateInt8());
			typeMap.Add(UnsignedInt16Type.Single, TypeRef.CreateInt16());
			typeMap.Add(UnsignedInt32Type.Single, TypeRef.CreateInt32());
			typeMap.Add(UnsignedInt64Type.Single, TypeRef.CreateInt64());

			typeMap.Add(BoolType.Single, new TypeRef(Native.Int1Type()));

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


		#region Debug

		Function EmitTestLLVMIR()
		{
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
			Value llp = builder.BuildLoad(lp);					// %1 = load i32* %0
			builder.BuildStore(llp, c);

			builder.BuildReturn(builder.BuildLoad(c));
			return func;
		}

		#endregion


		#region Sections

		public override Value Visit(ProgramSection node)
		{
			traverse(node.decls);
			evalCtx.inTopLevel = false;

			Function func = new Function(module, "main", TypeRef.CreateInt32(), new TypeRef[0]);
			func.SetLinkage(LLVMLinkage.CommonLinkage);
			// set 'func' and 'main' instance vars (current function and main function)
			evalCtx.main = evalCtx.func = func;

			// Create a new basic block to start insertion into.
			BasicBlock bb = func.AppendBasicBlock("entry");
			builder.SetInsertPoint(bb);

			traverse(node.block);

		//	Value ret = builder.BuildLoad(values.ElementAt(values.Count - 1).Value);
			//	return builder.BuildReturn(Value.CreateConstInt32(0));
			Value ret = Value.CreateConstInt32(0);
			return builder.BuildReturn(ret);
		}

		#endregion


		#region Routines

		public override Value Visit(RoutineDeclaration node)
		{
			ProceduralType functype = node.Type;
			TypeRef llvmfrettype = (node.IsFunction ? GetLLVMType(functype.funcret) : TypeRef.CreateVoid());

			var @params = functype.@params.Parameters();
			var args = @params.Select(a => GetLLVMType(a.type));

			Function func = new Function(module, node.name, llvmfrettype, args.ToArray());

			func.SetLinkage(LLVMLinkage.ExternalLinkage);

			// If F conflicted, there was already something named 'Name'.  If it has a
			// body, don't allow redefinition or reextern.
			if (func.IsDuplicate())
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

			valuemap.AddValue(node, func);
			return func;
		}


		private Value CreateRoutineArgument(Function func, int i, ParamDeclaration param)
		{
			TypeRef type = GetLLVMType(param.type);
			Value alloca = builder.BuildEntryBlockAlloca(func, type, param.name);
			builder.BuildStore(func.GetParameter((uint)i), alloca);

			Debug.Assert(alloca != null);
			valuemap.AddValue(param, alloca);
			return alloca;
		}

		public override Value Visit(RoutineDefinition node)
		{
			// set instance var func
			Function func = (Function)Visit((RoutineDeclaration)node);
			Debug.Assert(!func.IsNull, "Invalid function declaration " + node.name);
			evalCtx.inTopLevel = false;
			evalCtx.func = func;

			// Create a new basic block to start insertion into.
			BasicBlock bb = func.AppendBasicBlock("entry");
			builder.SetInsertPoint(bb);

			int i = 0;
			var @params = node.Type.@params;
			foreach (var param in @params.Parameters())
				CreateRoutineArgument(func, i++, param);

			Value retVal = null;
			if (node.IsFunction)
			{	// Create local var for the return value (Result)
				var retvar = @params.returnVar;
				var llvmtype = GetLLVMType(retvar.type);
				retVal = builder.BuildAlloca(llvmtype, retvar.name);
				valuemap.AddValue(retvar, retVal);
			}

			Value ret = traverse(node.body);
			if (ret.IsNull)
			{	// Error reading body, remove function.
				Error("Could not generate routine " + node.name);
				func.Delete();
				return null;
			}
			evalCtx.inTopLevel = true;

			if (node.IsFunction)
				builder.BuildReturn(builder.BuildLoad(retVal));
			else
				builder.BuildReturn();

			// Validate the generated code, checking for consistency.
			func.Validate(LLVMVerifierFailureAction.PrintMessageAction);
			// Optimize the function.
			passManager.Run(func);
			return func;
		}

		public override Value Visit(RoutineCall node)
		{
			Function func = (Function)traverse(node.func);
			Debug.Assert(func != null);

			int i = 0;
			Value[] args = new Value[func.ArgCount];
			foreach (var arg in node.args)
				args[i++] = traverse(arg);

			Value funcretval = builder.BuildCall(func, args);

			if (func.ReturnType.TypeKind == LLVMTypeKind.VoidTypeKind)
			{	// procedure
				return Value.NonNull;
			}

			// Create temp alloca to hold address of value
			return builder.BuildAlloca(func.ReturnType, "tmpcallret");
		}

		#endregion


		#region Declarations

		public override Value Visit(VarDeclaration node)
		{
			var llvmtype = GetLLVMType(node.type);
			
			Value vdecl;
			if (evalCtx.inTopLevel)
			{
				vdecl = builder.AddGlobal(module, llvmtype, node.name);
				Utils.SetInitializer(vdecl, llvmtype.CreateNullValue());
			}
			else
				vdecl = builder.BuildAlloca(llvmtype, node.name);

			valuemap.AddValue(node, vdecl);
			return vdecl;
		}
		
		#endregion


		#region Lvalues

		public override Value Visit(Identifier node)
		{
			// ID has been previously validated
			return valuemap.GetValue(node.decl);
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
					return builder.BuildShl(l, r);

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


		#region Statements

		public override Value Visit(Assignment node)
		{
			Value rvalue = traverse(node.expr);
			Value lvalue = traverse(node.lvalue);

			if (rvalue.IsNull || lvalue.IsNull)
				return Value.Null;

			return builder.BuildStore(rvalue, lvalue);
		}

		public override Value Visit(IfStatement node)
		{
			// func was set in Visit(RoutineDefinition node)
			Function func = evalCtx.func;
			
			Value cond = traverse(node.condition);
			Debug.Assert(!cond.IsNull, "Invalid If Condition");

			BasicBlock thenblock = func.AppendBasicBlock("if_then");
			BasicBlock mergeblock = func.AppendBasicBlock("ifcont");
			BasicBlock elseblock = (node.elseblock.IsEmpty ? mergeblock 
										: func.AppendBasicBlock("if_else"));
			builder.BuildCondBr(cond, thenblock, elseblock);

			builder.ResetInsertPoint(func, thenblock);
			Value thenV = traverse(node.thenblock);
			Debug.Assert(!thenV.IsNull, "Invalid If Then block");
			builder.BuildBr(mergeblock);

			if (!node.elseblock.IsEmpty)
			{
				builder.ResetInsertPoint(func, elseblock);
				Value elseV = traverse(node.elseblock);
				Debug.Assert(!thenV.IsNull, "Invalid If Else block");
				builder.BuildBr(mergeblock);
			}

			builder.ResetInsertPoint(func, mergeblock);
			return Value.NonNull;
		}
	
		public override Value Visit(ForLoop node)
		{
			//	Output this as:
			//		store startval, id
			//	forloop: 
			//		body
			//		idval = load id
			//		incr/decr idval
			//		store idval, id
			//		cmp idval, endval
			//		br slt/sgt forloop, forend
			//	forend:
			// NOTE: it is invalid to modify the id var inside the loop

			// func was set in Visit(RoutineDefinition node)
			Function func = evalCtx.func;

			Value id = traverse(node.var);
			Value startval = traverse(node.start);
			Value endval = traverse(node.end);
			builder.BuildStore(startval, id);

			Debug.Assert(!id.IsNull, "Invalid For var id");
			Debug.Assert(!startval.IsNull, "Invalid For start value");
			Debug.Assert(!endval.IsNull, "Invalid For end value");
			
			BasicBlock forloop = func.AppendBasicBlock("forloop");
			BasicBlock forend = func.AppendBasicBlock("forend");

			evalCtx.EnterLoop(forloop, forend);
			builder.BuildBr(forloop);
			builder.ResetInsertPoint(func, forloop);

			Value body = traverse(node.block);
			Debug.Assert(!body.IsNull, "Invalid For loop body");

			// emit the step and test
			Value idval = builder.BuildLoad(id);
			if (node.direction > 0)	// incr
				idval = builder.BuildAdd(idval, Value.CreateConstInt32(1));
			else	// decr
				idval = builder.BuildSub(idval, Value.CreateConstInt32(1));

			builder.BuildStore(idval, id);	// persist the final value after the For
			var op =  (node.direction > 0) ? LLVMIntPredicate.IntULT : LLVMIntPredicate.IntUGT;
			Value cmpres = builder.BuildICmp(idval, op, endval);
			builder.BuildCondBr(cmpres, forloop, forend);

			builder.ResetInsertPoint(func, forend);
			evalCtx.LeaveLoop();
			return Value.NonNull;
		}

		public override Value Visit(WhileLoop node)
		{
			//	Output this as:
			//	br whileloop	; llvm demands it
			//	whileloop: 
			//		br cond, whilebody, whileend
			//	whilebody:
			//		body
			//		br whileloop
			//	whileend:

			// func was set in Visit(RoutineDefinition node)
			Function func = evalCtx.func;

			BasicBlock whileloop = func.AppendBasicBlock("whileloop");
			BasicBlock whilebody = func.AppendBasicBlock("whilebody");
			BasicBlock whileend  = func.AppendBasicBlock("whileend");

			evalCtx.EnterLoop(whileloop, whileend);
			builder.BuildBr(whileloop);

			builder.ResetInsertPoint(func, whileloop);
			Value cond = traverse(node.condition);
			builder.BuildCondBr(cond, whilebody, whileend);

			builder.ResetInsertPoint(func, whilebody);
			Value block = traverse(node.block);
			builder.BuildBr(whileend);

			Debug.Assert(!block.IsNull, "Invalid While loop block");
			Debug.Assert(!cond.IsNull, "Invalid While loop condition");

			builder.ResetInsertPoint(func, whileend);
			evalCtx.LeaveLoop();
			return Value.NonNull;
		}

		public override Value Visit(RepeatLoop node)
		{
			//	Output this as:
			//	br repeatloop	; llvm demands it
			//	repeatloop:
			//		body
			//		br cond, repeatend, repeatloop
			//	repeatend:
			// NOTE: it is invalid to modify the id var inside the loop

			// func was set in Visit(RoutineDefinition node)
			Function func = evalCtx.func;

			BasicBlock repeatloop = func.AppendBasicBlock("repeatloop");
			BasicBlock repeatend  = func.AppendBasicBlock("repeatend");

			evalCtx.EnterLoop(repeatloop, repeatend);
			builder.BuildBr(repeatloop);
			builder.ResetInsertPoint(func, repeatloop);

			Value block = traverse(node.block);
			Value cond = traverse(node.condition);
			builder.BuildCondBr(cond, repeatend, repeatloop);

			Debug.Assert(!block.IsNull, "Invalid Repeat loop block");
			Debug.Assert(!cond .IsNull, "Invalid Repeat loop condition");

			builder.ResetInsertPoint(func, repeatend);
			evalCtx.LeaveLoop();
			return Value.NonNull;
		}

		#endregion


		#region Labels and Control Flow instructions

		public override Value Visit(BreakStatement node)
		{
			return builder.BuildBr(evalCtx.BreakBB);
		}

		public override Value Visit(ContinueStatement node)
		{
			return builder.BuildBr(evalCtx.ContinueBB);
		}

		public override Value Visit(LabelDeclaration node)
		{
			Function func = evalCtx.func;
			valuemap.AddValue(node, func.AppendBasicBlock(node.name));
			return Value.NonNull;
		}

		public override Value Visit(LabelStatement node)
		{
			Function func = evalCtx.func;
			BasicBlock label = valuemap.GetValue(node.decl) as BasicBlock;
			func.AppendBasicBlock(label);
			builder.BuildBr(label);
			builder.ResetInsertPoint(func,label);
			return traverse(node.stmt);
		}

		public override Value Visit(GotoStatement node)
		{
			BasicBlock label = valuemap.GetValue(node.decl) as BasicBlock;
			return builder.BuildBr(label);
		}
		
		#endregion

	}
}
