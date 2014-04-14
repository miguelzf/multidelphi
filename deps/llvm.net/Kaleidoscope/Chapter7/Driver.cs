using LLVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kaleidoscope.Chapter7
{
    //===----------------------------------------------------------------------===//
    // Top-Level parsing and JIT Driver
    //===----------------------------------------------------------------------===//
    class Driver : IDriver
    {
        private Parser m_parser;
        private LLVM.IRBuilder m_builder;
        private LLVM.PassManager m_passMgr;
        private LLVM.ExecutionEngine m_engine;
		Module module;
		IRBuilder builder;

        public Driver()
        {
        }

        public void Run()
        {
					//	string ex1 = "def binary : 1 (x y) y;";
		//	string ex2 = "def fibi(x) var a = 1, b = 1, c in  (for i = 3, i < x in c = a + b :  a = b :  b = c) : b;";
		//	var ss = new StringReader(ex1+"  " + ex2);

            Lexer lexer = new Lexer(Console.In);
            m_parser = new Parser(lexer);
            using(module = new LLVM.Module("my cool jit"))
			using (builder = m_builder = new LLVM.IRBuilder())
            {
                CodeGenManager.Module = module;
                m_engine = new LLVM.ExecutionEngine(module);
                m_passMgr = new LLVM.PassManager(module);
                m_passMgr.AddTargetData(m_engine.GetTargetData());
                m_passMgr.AddBasicAliasAnalysisPass();
                m_passMgr.AddPromoteMemoryToRegisterPass();
                m_passMgr.AddInstructionCombiningPass();
                m_passMgr.AddReassociatePass();
                m_passMgr.AddGVNPass();
                m_passMgr.AddCFGSimplificationPass();
                m_passMgr.Initialize();


				Function main = EmitTestLLVMIR();
				module.Dump();

				//	if (valRet.Equals(Value.Null))
				//		return Value.Null;

				GenericValue val = m_engine.RunFunction(main, new GenericValue[0]);
				Console.WriteLine("Evaluated to " + val.ToUInt());


                while(true)
                {
                    Console.Write("ready>");
                    switch(m_parser.GetNextToken())
                    {
                        case TokenCode.Eof:
                            // module.Dump();
                            break;

                        case TokenCode.Def:
                            HandleDefinition();
                            break;

                        case TokenCode.Symbol:
                            if(m_parser.Token.IdentiferText == ";")
                                m_parser.GetNextToken();
                            else
                                HandleTopLevelExpression();
                            break;

                        default:
                            HandleTopLevelExpression();
                            break;
                    }
                }
            }
        }


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
		//	TypeRef ptr = TypeRef.CreatePointer(tint);

			Function func = new Function(module, "main", tint, new TypeRef[0]);
			func.SetLinkage(LLVMLinkage.CommonLinkage);

			// Create a new basic block to start insertion into.
			BasicBlock bb = func.AppendBasicBlock("entry");
			builder.SetInsertPoint(bb);

			Value a = builder.BuildAlloca(tint, "a");	//  %a = alloca i32
			Value c = builder.BuildAlloca(tint, "c");	//  %c = alloca i32

			//	Value p = builder.BuildAlloca(ptr, "p");	//  %p = alloca i32*

			builder.BuildStore(Value.CreateConstInt32(111), a);	// store i32 1, i32* %a
			//	builder.BuildStore(a, p);							// store i32* %a, i32** %p
			Value va = builder.BuildLoad(a,"");
			//	Value lp = builder.BuildLoad(p);					// %0 = load i32** %p
			//	Value llp= builder.BuildLoad(lp);					// %1 = load i32* %0
			//	builder.BuildStore(llp, c);

			builder.BuildReturn(va);
			return func;
		}


        private void HandleDefinition()
        {
            FunctionAST f = m_parser.ParseDefinition();
            if(f != null)
            {
                LLVM.Function func = f.CodeGen(m_builder, m_passMgr);
                if(func != null)
                {
                    Console.WriteLine("Read function definition:");
                    func.Dump();
                }
            }
            else
                m_parser.GetNextToken();
        }

        private void HandleTopLevelExpression()
        {
            FunctionAST f = m_parser.ParseTopLevelExpr();
            if(f != null)
            {
                LLVM.Function func = f.CodeGen(m_builder, m_passMgr);
                if(func != null)
                {
                    func.Dump();

                    LLVM.GenericValue val = m_engine.RunFunction(func, new LLVM.GenericValue[0]);
                    Console.WriteLine("Evaluated to " + val.ToReal().ToString());
                }
            }
            else
                m_parser.GetNextToken();
        }
    }
}
