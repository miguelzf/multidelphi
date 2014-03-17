using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using crosspascal.ast;
using crosspascal.ast.nodes;
using crosspascal.semantics;
using System.IO;
using System.Diagnostics;

namespace crosspascal.parser
{
	/// <summary>
	/// Internal support utils for DelphiParser
	/// </summary>
	/// <remarks>
	/// Moved here to unclutter the YACC grammar, and allow for VS's intellisense
	/// </remarks>
	public partial class DelphiParser
	{
		// Internal helper functions

		string GetErrorMessage(ParserException e)
		{
			StackTrace st = new StackTrace(e, true);
			StackFrame frame = st.GetFrame(st.FrameCount - 1);
			return "[ERROR] " + e.Message + " in " + Path.GetFileName(frame.GetFileName())
					+ ": line " + frame.GetFileLineNumber();
		}


		//Encoding.Default;	// typically Single-Bye char set
		// TODO change charset to unicode, use %unicode in flex
		public static readonly Encoding DefaultEncoding = Encoding.GetEncoding("iso-8859-1");

		public static int DebugLevel;

		// to be fetched in the AST Declaration node
		public static DeclarationsRegistry DeclRegistry;

		DelphiScanner lexer;

		//
		// Entry point and public interface
		//

		public DelphiParser(ParserDebug dgb)
		{
			if (dgb != null)
			{
				this.debug = (ParserDebug)dgb;
			}

			eof_token = DelphiScanner.YYEOF;
		}

		public DelphiParser(int dgbLevel)
			: this(new Func<ParserDebug>(
					() =>
					{
						switch (dgbLevel)
						{
							case 1: return new DebugPrintFinal();
							case 2: return new DebugPrintAll();
							default: return null;
						}
					})())
		{
			DebugLevel = dgbLevel;
		}

		// wrapper for yyparse
		public CompilationUnit Parse(TextReader tr, ParserDebug dgb = null)
		{
			if (dgb != null)
			{
				this.debug = (ParserDebug)dgb;
				DebugLevel = 1;
			}

			DeclRegistry = new DeclarationsRegistry();
			DeclRegistry.LoadRuntimeNames();

			lexer = new DelphiScanner(tr);
			Object parserRet;
			try
			{
				parserRet = yyparse(lexer);
			}
			catch (Exception yye)
			{
				ErrorOutput.WriteLine(yye.Message + " in line " + lexer.yylineno());
				ErrorOutput.WriteLine(yye.StackTrace);
				// only clean way to signal error. null is the default yyVal
				throw yye; // new InputRejected(GetErrorMessage(yye));
			}

			if (!parserRet.GetType().IsSubclassOf(typeof(CompilationUnit)))
				throw new ParserException();

			return (CompilationUnit)parserRet;
		}



		/// <summary>
		/// Resolves an id, disambiguating between RoutineCalls and Identifiers
		/// </summary>
		LvalueExpression ResolveId(Identifier id)
		{
			String name = id.name;
			if (DeclRegistry.CheckType<ProceduralType>(name) != null)
				return new RoutineCall(id);
			else
				if (DeclRegistry.CheckValue<ValueDeclaration>(name) != null)
					return id;
				else
					throw new IdentifierUndeclared(name);
		}


		// Internal helpers

		string lastObjectName = null;	// keeps track of current class/object/interface being parsed

		T ListAdd<T,N>(T bodylst, N elem)
			where N : Node
			where T : ListNode<N>
		{
			bodylst.Add(elem);
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

		BinaryExpression CreateBinaryExpression(Expression e1, int token, Expression e2)
		{
			switch (token)
			{
				case Token.KW_MUL: return new Product(e1, e2);
				case Token.KW_DIV: return new Division(e1, e2);
				case Token.KW_QUOT: return new Quotient(e1, e2);
				case Token.KW_MOD: return new Modulus(e1, e2);
				case Token.KW_SHR: return new ShiftRight(e1, e2);
				case Token.KW_SHL: return new ShiftLeft(e1, e2);
				case Token.KW_AND: return new LogicalAnd(e1, e2);
				case Token.KW_SUB: return new Subtraction(e1, e2);
				case Token.KW_SUM: return new Addition(e1, e2);
				case Token.KW_OR: return new LogicalOr(e1, e2);
				case Token.KW_XOR: return new LogicalXor(e1, e2);
				case Token.KW_EQ: return new Equal(e1, e2);
				case Token.KW_NE: return new NotEqual(e1, e2);
				case Token.KW_LT: return new LessThan(e1, e2);
				case Token.KW_LE: return new LessOrEqual(e1, e2);
				case Token.KW_GT: return new GreaterThan(e1, e2);
				case Token.KW_GE: return new GreaterOrEqual(e1, e2);
				default: throw new ParserException("Invalid Binary Operation token: " + token);
			}
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

	}
}
