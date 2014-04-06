using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kaleidoscope.Chapter7
{
    enum TokenCode
    {
        Eof,
        Symbol,
        Def,
        Extern,
        Identifier,
        Number,
        If,
        Then,
        Else,
        For,
        In,
        Binary,
        Unary
    }

    struct Token
    {
        public TokenCode Code { get; private set; }
        public string IdentiferText { get; private set; }
        public double NumberValue { get; private set; }

        public Token(TokenCode code)
            : this()
        {
            this.Code = code;
        }

        public Token(char ch)
            : this()
        {
            this.Code = TokenCode.Symbol;
            this.IdentiferText = ch.ToString();
        }

        public Token(string text)
            : this()
        {
            this.Code = TokenCode.Identifier;
            this.IdentiferText = text;
        }

        public Token(double value)
            : this()
        {
            this.Code = TokenCode.Number;
            this.NumberValue = value;
            this.IdentiferText = value.ToString();
        }
    }

    class Lexer
    {
        private readonly TextReader m_reader;
        private string m_line = string.Empty;
        private int m_position = 0;
        private bool m_bEof;

        public Lexer(TextReader reader)
        {
            m_reader = reader;
        }

        public Token GetToken()
        {
            if(m_bEof || !AdvanceReader())
                return new Token(TokenCode.Eof);

            char ch = m_line[m_position];

            // identifier: [a-zA-Z][a-zA-Z0-9]*
            if(char.IsLetter(ch)) 
            {
                StringBuilder sb = new StringBuilder();
                do
                {
                    sb.Append(ch);
                    ch = NextChar();
                }
                while(char.IsLetterOrDigit(ch));

                string identifier = sb.ToString();
                switch(identifier)
                {
                    case "def":
                        return new Token(TokenCode.Def);
                    case "extern":
                        return new Token(TokenCode.Extern);
                    case "if":
                        return new Token(TokenCode.If);
                    case "then":
                        return new Token(TokenCode.Then);
                    case "else":
                        return new Token(TokenCode.Else);
                    case "for":
                        return new Token(TokenCode.For);
                    case "in":
                        return new Token(TokenCode.In);
                    case "binary":
                        return new Token(TokenCode.Binary);
                    case "unary":
                        return new Token(TokenCode.Unary);
                }

                return new Token(identifier); 
            }

            // Number: [0-9.]+
            if(char.IsNumber(ch) || ch == '.')
            {
                StringBuilder sb = new StringBuilder();
                do
                {
                    sb.Append(ch);
                    ch = NextChar();
                }
                while(char.IsDigit(ch) || ch == '.');

                return new Token(double.Parse(sb.ToString()));
            }

            if(ch == '#')
            {
                // Comment until end of line.
                m_position = m_line.Length;
                return GetToken();
            }

            m_position++;

            return new Token(ch);
        }

        private char NextChar()
        {
            m_position++;

            if(m_position >= m_line.Length)
                return char.MinValue;

            return m_line[m_position];
        }

        private bool AdvanceReader()
        {
            while(!m_bEof)
            {
                if(m_position == m_line.Length)
                {
                    m_line = m_reader.ReadLine();
                    m_position = 0;

                    if(string.IsNullOrWhiteSpace(m_line))
                    {
                        m_bEof = true;
                        break;
                    }
                }

                // Skip any whitespace.
                while(m_position < m_line.Length && char.IsWhiteSpace(m_line[m_position]))
                {
                    m_position++;
                }

                if(m_position < m_line.Length)
                    break;
            }

            return !m_bEof;
        }
    }
}
