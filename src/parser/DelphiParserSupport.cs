using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MultiPascal.AST;
using MultiPascal.AST.Nodes;
using MultiPascal.Semantics;
using System.IO;
using System.Diagnostics;
using System.Collections;
using MultiPascal.core;

namespace MultiPascal.Parser
{
	/// <summary>
	/// Internal support utils for DelphiParser
	/// </summary>
	/// <remarks>
	/// Moved here to unclutter the JAY/YACC grammar, and allow for VS's intellisense
	/// </remarks>
	public partial class DelphiParser
	{
		//
		// Entry point and public interface
		//

		public static int DebugLevel;

		// TODO improve/cleanup this
		public static DelphiParser Instance { get; set; }
		SourceFile currentFile;
		
		public Location CurrentLocation()
		{
			return new Location(yyline(), currentFile.path);
		}

		public bool IsParsing()
		{
			return lexer != null;
		}

		public DelphiParser(ParserDebug dgb)
		{
			if (dgb != null)
			{
				this.debug = (ParserDebug)dgb;
			}

			eof_token = DelphiScanner.YYEOF;
			Instance = this;
		}

		public DelphiParser(int dgbLevel)
			: this(new Func<ParserDebug>(
					() => {
						switch (dgbLevel)
						{	case 1: return new DebugPrintFinal();
							case 2: return new DebugPrintAll();
							default: return null;
						}
					})())
		{
			DebugLevel = dgbLevel;
		}


		public int yyline()
		{
			return lexer.yylineno();
		}


		//
		// Per-file mutable parsing state
		//

		DelphiScanner lexer;

		// wrapper for yyparse
		public TranslationUnit Parse(TextReader tr, SourceFile file = null, ParserDebug dgb = null)
		{
			//if (dgb != null)
			{
				this.debug = (ParserDebug)dgb;
				DebugLevel = 1;
			}

			if (file != null)
				currentFile = file;
			lexer = new DelphiScanner(tr);
			lexer.yyLexDebugLevel = DebugLevel;

			Object parserRet;
			try
			{
				parserRet = yyparse(lexer);
			}
			catch (MultiPascalException yye)
			{
				yyerror(yye.Message);
				return null;
			}
			catch (Exception e)
			{
				ErrorOutput.WriteLine(e.Message + " in line " + lexer.yylineno());
				ErrorOutput.WriteLine(e.StackTrace);
				return null;
			}
			finally {
				lexer = null;
			}

			if (!(parserRet is TranslationUnit))
				throw new ParserException("Non-final node derived from parsing:"  + parserRet.GetType());

			return (TranslationUnit)parserRet;
		}


		// Internal helpers

		T ListAdd<T,N>(T bodylst, N elem)
			where N : Node
			where T : ListNode<N>
		{
			bodylst.Add(elem);
			return bodylst;
		}

		T ListAdd<T, N>(T bodylst, T elems)
			where N : Node
			where T : ListNode<N>
		{
			bodylst.Add(elems);
			return bodylst;
		}

		T ListAddRange<T, N>(T bodylst, T elems)
			where N : Node
			where T : ListNode<N>
		{
			bodylst.Add(elems);
			return bodylst;
		}

		T ListAddReverse<T, N>(T bodylst, N elem) 
			where N : Node 
			where T : ListNode<N>
		{
			bodylst.AddStart(elem);
			return bodylst;
		}

		delegate Declaration CreateDecl(String s);

		DeclarationList CreateDecls(ArrayList ids, CreateDecl func)
		{
			var list = new DeclarationList();
			foreach (String s in ids)
				list.Add(func(s));
			return list;
		}


		BinaryExpression CreateBinaryExpression(Expression e1, int token, Expression e2)
		{
			switch (token)
			{
				case Token.KW_MUL: return new Product(e1, e2);
				case Token.KW_DIV: return new Division(e1, e2);
				case Token.KW_QUOT:return new Quotient(e1, e2);
				case Token.KW_MOD: return new Modulus(e1, e2);
				case Token.KW_SHR: return new ShiftRight(e1, e2);
				case Token.KW_SHL: return new ShiftLeft(e1, e2);
				case Token.KW_AND: return new LogicalAnd(e1, e2);
				case Token.KW_SUB: return new Subtraction(e1, e2);
				case Token.KW_SUM: return new Addition(e1, e2);
				case Token.KW_OR : return new LogicalOr(e1, e2);
				case Token.KW_XOR: return new LogicalXor(e1, e2);
				case Token.KW_EQ : return new Equal(e1, e2);
				case Token.KW_NE : return new NotEqual(e1, e2);
				case Token.KW_LT : return new LessThan(e1, e2);
				case Token.KW_LE : return new LessOrEqual(e1, e2);
				case Token.KW_GT : return new GreaterThan(e1, e2);
				case Token.KW_GE : return new GreaterOrEqual(e1, e2);
				default: throw new ParserException("Invalid Binary Operation token: " + token);
			}
		}


		// Internal helpers

		ImportDirectives JoinImportDirectives(RoutineDirectives d1, RoutineDirectives d2, ExternalDirective e)
		{
			var id = JoinImportDirectives(d1, d2, ImportDirective.External);
			id.External = e;
			return id;
		}

		ImportDirectives JoinImportDirectives(RoutineDirectives d1, RoutineDirectives d2, ImportDirective i)
		{
			var id = new ImportDirectives(i);
			id.Add(d1);
			id.Add(d2);
			return id;
		}

		bool CheckDirectiveId(String expected, String idtoken)
		{
			if (String.Compare(expected, idtoken, true) != 0)
			{
				yyerror("Invalid directive '" + idtoken + "', expected: " + expected);
				return false;
			}
			return true;
		}

		string GetErrorMessage(ParserException e)
		{
			StackTrace st = new StackTrace(e, true);
			StackFrame frame = st.GetFrame(st.FrameCount - 1);
			return "[ERROR] " + e.Message + " in " + Path.GetFileName(frame.GetFileName())
					+ ": line " + frame.GetFileLineNumber();
		}

	}
}
