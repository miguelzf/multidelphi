using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using crosspascal.ast;
using crosspascal.ast.nodes;
using crosspascal.semantics;
using System.IO;
using System.Diagnostics;
using System.Collections;
using crosspascal.core;

namespace crosspascal.parser
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
		// Immutable Parser state 
		//

		public static int DebugLevel;

		// to be fetched in the AST Declaration node
		public static DeclarationsRegistry DeclReg;


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



		//
		// Per-file mutable parsing state
		//

		SourceFile source;

		DelphiScanner lexer;

		// wrapper for yyparse
		public CompilationUnit Parse(TextReader tr, SourceFile sf, ParserDebug dgb = null)
		{
			if (dgb != null)
			{
				this.debug = (ParserDebug)dgb;
				DebugLevel = 1;
			}

			source = sf;
			DeclReg = new DeclarationsRegistry();
			DeclReg.LoadRuntimeNames();

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

			if (!(parserRet is CompilationUnit))
				throw new ParserException("Non-final node derived from parsing:"  + parserRet.GetType());

			return (CompilationUnit)parserRet;
		}



		//
		// AST cleaning/shaping up
		// 

		/// <summary>
		/// Finalize processing of an Unit's interface section, by saving its symbol context
		/// </summary>
		void FinishInterfaceSection()
		{
			source.interfContext = DeclReg.ExportContext();
		}

		/// <summary>
		/// Import dependency from a Unit SourceFile already parsed,
		/// by loading its interface context
		/// </summary>
		void ImportDependency(string id)
		{
			var ctx = source.GetDependency(id).interfContext;
			ctx.id = id;
			DeclReg.ImportContext(ctx);
		}


		/// <summary>
		/// Resolves an id, disambiguating between RoutineCalls and Identifiers
		/// </summary>
		LvalueExpression ResolveId(Identifier id)
		{
			String name = id.name;
			if (DeclReg.CheckType<ProceduralType>(name) != null)
				return new RoutineCall(id);
			else
				if (DeclReg.CheckValue<ValueDeclaration>(name) != null)
					return id;
				else
					throw new DeclarationNotFound(name);
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
