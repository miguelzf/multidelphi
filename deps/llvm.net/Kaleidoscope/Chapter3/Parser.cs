using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kaleidoscope.Chapter3
{
    //===----------------------------------------------------------------------===//
    // Parser
    //===----------------------------------------------------------------------===//
    class Parser
    {
        /// BinopPrecedence - This holds the precedence for each binary operator that is
        /// defined.
        private static IDictionary<char, int> s_binopPrecendence = new Dictionary<char, int>();

        /// CurTok/getNextToken - Provide a simple token buffer.  CurTok is the current
        /// token the parser is looking at.  getNextToken reads another token from the
        /// lexer and updates CurTok with its results.
        private Lexer m_lexer;
        private Token m_token;

        static Parser()
        {
            // Install standard binary operators.
            // 1 is lowest precedence.
            s_binopPrecendence['<'] = 10;
            s_binopPrecendence['+'] = 20;
            s_binopPrecendence['-'] = 20;
            s_binopPrecendence['*'] = 40;  // highest.
        }

        public Parser(Lexer lexer)
        {
            m_lexer = lexer;
        }

        public Token Token
        {
            get { return m_token; }
        }

        public TokenCode GetNextToken()
        {
            m_token = m_lexer.GetToken();
            return m_token.Code;
        }

        /// GetTokPrecedence - Get the precedence of the pending binary operator token.
        private int GetTokenPrecedence()
        {
            if(m_token.Code != TokenCode.Unknown)
                return -1;

            int prec = 0;
            if(s_binopPrecendence.TryGetValue(m_token.IdentiferText[0], out prec))
                return prec;

            return -1;
        }

        /// Error* - These are little helper functions for error handling.
        private ExprAST Error(string str)
        {
            CodeGenManager.ErrorOutput.WriteLine(str);
            return null;
        }

        private PrototypeAST ErrorP(string str)
        {
            Error(str);
            return null;
        }

        private FunctionAST ErrorF(string str)
        {
            Error(str);
            return null;
        }

        /// identifierexpr
        ///   ::= identifier
        ///   ::= identifier '(' expression* ')'
        private ExprAST ParseIdentifierExp()
        {
            string name = m_token.IdentiferText;
            GetNextToken(); // eat identifier.

            if(m_token.IdentiferText != "(") // Simple variable ref.
                return new VariableExprAST(name);

            // Call.
            GetNextToken();
            List<ExprAST> args = new List<ExprAST>();
            if(m_token.IdentiferText != ")")
            {
                while(true)
                {
                    ExprAST arg = ParseExpression();
                    if(arg == null)
                        return null;

                    args.Add(arg);

                    if(m_token.IdentiferText == ")")
                        break;

                    if(m_token.IdentiferText != ",")
                        return Error("Expected ')' or ',' in argument list");

                    GetNextToken();
                }
            }

            // Eat the ')'.
            GetNextToken();

            return new CallExprAST(name, args);
        }

        /// numberexpr ::= number
        private ExprAST ParseNumberExpr()
        {
            ExprAST result = new NumberExprAST(m_token.NumberValue);
            GetNextToken();
            return result;
        }

        /// parenexpr ::= '(' expression ')'
        private ExprAST ParseParenExpr()
        {
            GetNextToken(); // eat (.
            ExprAST v = ParseExpression();
            if(v == null)
                return null;

            if(m_token.IdentiferText != ")")
                return Error("Expected ')'");

            GetNextToken();  // eat ).

            return v;
        }

        /// primary
        ///   ::= identifierexpr
        ///   ::= numberexpr
        ///   ::= parenexpr
        private ExprAST ParsePrimary()
        {
            switch(m_token.Code)
            {
                case TokenCode.Identifier:
                    return ParseIdentifierExp();
                case TokenCode.Number:
                    return ParseNumberExpr();
                case TokenCode.Unknown:
                    switch(m_token.IdentiferText[0])
                    {
                        case '(':
                            return ParseParenExpr();
                    }
                    break;
            }

            return Error("unknown token when expecting an expression");
        }

        /// binoprhs
        ///   ::= ('+' primary)*
        private ExprAST ParseBinOpRHS(int exprPrec, ExprAST lhs)
        {
            // If this is a binop, find its precedence.
            while(true)
            {
                int tokPrec = GetTokenPrecedence();

                // If this is a binop that binds at least as tightly as the current binop,
                // consume it, otherwise we are done.
                if(tokPrec < exprPrec)
                    return lhs;

                // Okay, we know this is a binop.
                char binOp = m_token.IdentiferText[0];
                GetNextToken();

                // Parse the primary expression after the binary operator.
                ExprAST rhs = ParsePrimary();
                if(rhs == null)
                    return null;

                // If BinOp binds less tightly with RHS than the operator after RHS, let
                // the pending operator take RHS as its LHS.
                int nextPrec = GetTokenPrecedence();
                if(tokPrec < nextPrec)
                {
                    rhs = ParseBinOpRHS(tokPrec + 1, rhs);
                    if(rhs == null)
                        return null;
                }

                // Merge LHS/RHS.
                lhs = new BinaryExprAST(binOp, lhs, rhs);
            }
        }

        /// expression
        ///   ::= primary binoprhs
        ///
        private ExprAST ParseExpression()
        {
            ExprAST lhs = ParsePrimary();
            if(lhs == null)
                return null;

            return ParseBinOpRHS(0, lhs);
        }

        /// prototype
        ///   ::= id '(' id* ')'
        private PrototypeAST ParsePrototype()
        {
            if(m_token.Code != TokenCode.Identifier)
                return ErrorP("Expected function name in prototype");

            string fnName = m_token.IdentiferText;
            GetNextToken();

            if(m_token.IdentiferText != "(")
                return ErrorP("Expected '(' in prototype");

            List<string> names = new List<string>();
            while(GetNextToken() == TokenCode.Identifier)
            {
                names.Add(m_token.IdentiferText);
            }

            if(m_token.IdentiferText != ")")
                return ErrorP("Expected ')' in prototype");

            GetNextToken();

            return new PrototypeAST(fnName, names);
        }

        /// definition ::= 'def' prototype expression
        public FunctionAST ParseDefinition()
        {
            GetNextToken();
            PrototypeAST proto = ParsePrototype();
            if(proto == null)
                return null;

            ExprAST e = ParseExpression();
            if(e != null)
                return new FunctionAST(proto, e);

            return null;
        }
        /// toplevelexpr ::= expression
        public FunctionAST ParseTopLevelExpr()
        {
            ExprAST e = ParseExpression();
            if(e != null)
            {
                PrototypeAST proto = new PrototypeAST("", new string[0]);
                return new FunctionAST(proto, e);
            }

            return null;
        }

        /// external ::= 'extern' prototype
        public PrototypeAST ParseExtern()
        {
            GetNextToken();
            return ParsePrototype();
        }
    }
}
