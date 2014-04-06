using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kaleidoscope.Chapter4
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

        public Driver()
        {
        }

        public void Run()
        {
            Lexer lexer = new Lexer(Console.In);
            m_parser = new Parser(lexer);
            using(LLVM.Module module = new LLVM.Module("my cool jit"))
            using(m_builder = new LLVM.IRBuilder())
            {
                CodeGenManager.Module = module;
                m_engine = new LLVM.ExecutionEngine(module);
                m_passMgr = new LLVM.PassManager(module);
                m_passMgr.AddTargetData(m_engine.GetTargetData());
                m_passMgr.AddBasicAliasAnalysisPass();
                m_passMgr.AddInstructionCombiningPass();
                m_passMgr.AddReassociatePass();
                m_passMgr.AddGVNPass();
                m_passMgr.AddCFGSimplificationPass();
                m_passMgr.Initialize();

                while(true)
                {
                    Console.Write("ready>");
                    switch(m_parser.GetNextToken())
                    {
                        case TokenCode.Eof:
                            module.Dump();
                            return;

                        case TokenCode.Def:
                            HandleDefinition();
                            break;

                        case TokenCode.Unknown:
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
