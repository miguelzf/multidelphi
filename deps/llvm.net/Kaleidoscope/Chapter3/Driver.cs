using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kaleidoscope.Chapter3
{
    //===----------------------------------------------------------------------===//
    // Top-Level parsing and JIT Driver
    //===----------------------------------------------------------------------===//
    class Driver : IDriver
    {
        private Parser m_parser;
        private LLVM.IRBuilder m_builder;

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

                module.Dump();
            }
        }

        private void HandleDefinition()
        {
            FunctionAST f = m_parser.ParseDefinition();
            if(f != null)
            {
                LLVM.Function func = f.CodeGen(m_builder);
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
                LLVM.Function func = f.CodeGen(m_builder);
                if(func != null)
                {
                    Console.WriteLine("Read top-level expression:");
                    func.Dump();
                }
            }
            else
                m_parser.GetNextToken();
        }
    }
}
