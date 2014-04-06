using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kaleidoscope.Chapter7
{
    //===----------------------------------------------------------------------===//
    // Parser
    //===----------------------------------------------------------------------===//
    class Parser
    {
        /// CurTok/getNextToken - Provide a simple token buffer.  CurTok is the current
        /// token the parser is looking at.  getNextToken reads another token from the
        /// lexer and updates CurTok with its results.
        private Lexer m_lexer;
        private Token m_token;

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
            if(m_token.Code != TokenCode.Symbol)
                return -1;

            int prec = 0;
            if(CodeGenManager.BinopPrecendence.TryGetValue(m_token.IdentiferText[0], out prec))
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
                case TokenCode.If:
                    return ParseIfExpr();
                case TokenCode.For:
                    return ParseForExpr();
                case TokenCode.Symbol:
                    switch(m_token.IdentiferText[0])
                    {
                        case '(':
                            return ParseParenExpr();
                    }
                    break;
            }

            return Error("unknown token when expecting an expression");
        }

        private ExprAST ParseUnary()
        {
            if(m_token.Code != TokenCode.Symbol || m_token.IdentiferText == "(" || m_token.IdentiferText == ",")
                return ParsePrimary();

            char op = m_token.IdentiferText[0];
            GetNextToken();
            ExprAST operand = ParseUnary();
            if(operand == null) return null;

            return new UnaryExprAST(op, operand);
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

                // Parse the unary expression after the binary operator.
                ExprAST rhs = ParseUnary();
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

        /// ifexpr ::= 'if' expression 'then' expression 'else' expression
        private ExprAST ParseIfExpr()
        {
            GetNextToken();

            // condition
            ExprAST cond = ParseExpression();
            if(cond == null) return null;

            if(m_token.Code != TokenCode.Then)
                Error("Expected then.");

            GetNextToken(); // eat the then

            ExprAST thenExpr = ParseExpression();
            if(thenExpr == null) return null;

            if(m_token.Code != TokenCode.Else)
                Error("Expected else.");

            GetNextToken();

            ExprAST elseExpr = ParseExpression();
            if(elseExpr == null) return null;

            return new IfExprAST(cond, thenExpr, elseExpr);
        }

        /// forexpr ::= 'for' identifier '=' expr ',' expr (',' expr)? 'in' expression
        private ExprAST ParseForExpr()
        {
            GetNextToken();

            if(m_token.Code != TokenCode.Identifier)
                return Error("Expected identifier after for.");

            string name = m_token.IdentiferText;
            GetNextToken();

            if(m_token.Code != TokenCode.Symbol || m_token.IdentiferText != "=")
                return Error("Expected '=' after for.");
            GetNextToken();

            ExprAST start = ParseExpression();
            if(start == null) return null;
            if(m_token.Code != TokenCode.Symbol || m_token.IdentiferText != ",")
                return Error("Expected ',' after start value.");
            GetNextToken();

            ExprAST end = ParseExpression();
            if(end == null) return null;

            ExprAST step = null;
            if(m_token.Code == TokenCode.Symbol && m_token.IdentiferText == ",")
            {
                GetNextToken();
                step = ParseExpression();
                if(step == null) return null;
            }

            if(m_token.Code != TokenCode.In)
                return Error("Expected 'in' after for");
            GetNextToken();

            ExprAST body = ParseExpression();
            if(body == null) return null;

            return new ForExprAST(name, start, end, step, body);
        }

        /// expression
        ///   ::= primary binoprhs
        ///
        private ExprAST ParseExpression()
        {
            ExprAST lhs = ParseUnary();
            if(lhs == null)
                return null;

            return ParseBinOpRHS(0, lhs);
        }

        /// prototype
        ///   ::= id '(' id* ')'
        ///   ::= binary LETTER number? (id, id)
        ///   ::= unary LETTER (id)
        private PrototypeAST ParsePrototype()
        {
            int kind = 0;
            int precedence = 30;
            string fnName = null;

            switch(m_token.Code)
            {
                case TokenCode.Identifier:
                    fnName = m_token.IdentiferText;
                    GetNextToken();
                    break;

                case TokenCode.Binary:
                    GetNextToken();
                    fnName = "binary" + m_token.IdentiferText;
                    kind = 2;
                    GetNextToken();

                    if(m_token.Code == TokenCode.Number)
                    {
                        if(m_token.NumberValue < 1 || m_token.NumberValue > 100)
                            return ErrorP("Invalid precedence.  Must be 1..100");
                        precedence = (int)m_token.NumberValue;
                        GetNextToken();
                    }
                    break;

                case TokenCode.Unary:
                    GetNextToken();
                    if(m_token.Code != TokenCode.Symbol)
                        return ErrorP("Expected unary operator");
                    fnName = "unary" + m_token.IdentiferText;
                    kind = 1;
                    GetNextToken();
                    break;

                default:
                    return ErrorP("Expected function name in prototype");
            }

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

            if(kind != 0 && names.Count != kind)
                return ErrorP("Invalid number of operands for operator.");

            return new PrototypeAST(fnName, names, kind != 0, precedence);
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
                PrototypeAST proto = new PrototypeAST("", new string[0], false, 0);
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
