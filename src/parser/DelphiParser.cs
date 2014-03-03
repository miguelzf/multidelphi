// created by jay 0.7 (c) 1998 Axel.Schreiner@informatik.uni-osnabrueck.de

#line 2 "DelphiGrammar.y"

using System;
using System.Collections;
using System.IO;
using System.Text;

namespace crosspascal
{

	namespace yydebug
	{
		class yyErrorTrace : yyDebug
		{
			const int yyFinal = 6;
			
			void println (string s) {
				Console.Error.WriteLine (s);
			}
			
			void printchecked(int state, string s)
			{
				if (state == 0 || state == yyFinal)
					println(s);
			}

			void printchecked(int from, int to, string s)
			{
				if (from == 0 || from == yyFinal || to == 0 || to == yyFinal)
					println(s);
			}
			
			public void push (int state, Object value) {
				printchecked (state, "push\tstate "+state+"\tvalue "+value);
			}
			
			public void lex (int state, int token, string name, Object value) {
				 printchecked (state, "lex\tstate "+state+"\treading "+name+"\tvalue "+value);
			}
			
			public void shift (int from, int to, int errorFlag)
			{
					switch (errorFlag) {
					default:				// normally
						printchecked (from, to, "shift\tfrom state "+from+" to "+to);
						break;
					case 0: case 1: case 2:		// in error recovery
						printchecked (from, to,"shift\tfrom state "+from+" to "+to +"\t"+errorFlag+" left to recover");
						break;
					case 3:				// normally
						printchecked (from, to, "shift\tfrom state "+from+" to "+to+"\ton error");
						break;
					}
			}
			
			public void pop (int state) {
				printchecked (state,"pop\tstate "+state+"\ton error");
			}
			
			public void discard (int state, int token, string name, Object value) {
				printchecked (state,"discard\tstate "+state+"\ttoken "+name+"\tvalue "+value);
			}
			
			public void reduce (int from, int to, int rule, string text, int len) {
				printchecked (from, to, "reduce\tstate "+from+"\tuncover "+to +"\trule ("+rule+") "+text);
			}
			
			public void shift (int from, int to) {
				printchecked (from, to, "goto\tfrom state "+from+" to "+to);
			}
			
			public void accept (Object value) {
				println("accept\tvalue "+value);
			}
			
			public void error (string message) {
				println("error\t"+message);
			}
			
			public void reject () {
				println("reject");
			}
			
		}
	}

	public class DelphiParser
	{
		// #define YYDEBUG 0
		// extern int yylineno;	// absolute, from flex
		// extern int linenum;		// custom, adjusted for includes

		/*
		extern int yydebug;
		void yyerror(char *s) {
			fprintf(stdout, "Line %d %s\n", linenum, s);
		}
		*/
		
		
		int yacc_verbose_flag = 1;

		public static void Main(string[] args)
		{
			DelphiParser parser = new DelphiParser();
			parser.eof_token = DelphiScanner.ScannerEOF;
		
			try {
				foreach (string s in args)
					parser.yyparse(	new DelphiScanner(new StreamReader(s)), new yydebug.yyErrorTrace());
				
			} catch (Exception e)
			{
				Console.Out.WriteLine(e);
				// printf("Parsing finished ok\n");
				// printf("Parsing failed\n");
			}
		}

		
#line default

  /** error output stream.
      It should be changeable.
    */
  public System.IO.TextWriter ErrorOutput = System.Console.Out;

  /** simplified error message.
      @see <a href="#yyerror(java.lang.String, java.lang.String[])">yyerror</a>
    */
  public void yyerror (string message) {
    yyerror(message, null);
  }

  /* An EOF token */
  public int eof_token;

  /** (syntax) error message.
      Can be overwritten to control message format.
      @param message text to be displayed.
      @param expected vector of acceptable tokens, if available.
    */
  public void yyerror (string message, string[] expected) {
    if ((yacc_verbose_flag > 0) && (expected != null) && (expected.Length  > 0)) {
      ErrorOutput.Write (message+", expecting");
      for (int n = 0; n < expected.Length; ++ n)
        ErrorOutput.Write (" "+expected[n]);
        ErrorOutput.WriteLine ();
    } else
      ErrorOutput.WriteLine (message);
  }

  /** debugging support, requires the package jay.yydebug.
      Set to null to suppress debugging messages.
    */
  internal yydebug.yyDebug debug;

  protected static  int yyFinal = 6;
 // Put this array into a separate class so it is only initialized if debugging is actually used
 // Use MarshalByRefObject to disable inlining
 class YYRules : MarshalByRefObject {
  public static  string [] yyRule = {
    "$accept : goal",
    "goal : file KW_DOT",
    "file : program",
    "file : package",
    "file : library",
    "file : unit",
    "program : KW_PROGRAM id SEMICOL usesclauseopt main_block",
    "program : usesclauseopt main_block",
    "library : KW_LIBRARY id SEMICOL usesclauseopt main_block",
    "unit : KW_UNIT id SEMICOL interfsec implementsec initsec",
    "package : id id SEMICOL requiresclause containsclause KW_END",
    "requiresclause : id idlist SEMICOL",
    "containsclause : id idlist SEMICOL",
    "usesclauseopt :",
    "usesclauseopt : KW_USES useidlist SEMICOL",
    "useidlist : useid",
    "useidlist : useidlist COMMA useid",
    "useid : id",
    "useid : id KW_IN string_const",
    "implementsec : KW_IMPL usesclauseopt maindecllist",
    "implementsec : KW_IMPL usesclauseopt",
    "interfsec : KW_INTERF usesclauseopt interfdecllist",
    "interfdecllist :",
    "interfdecllist : interfdecllist interfdecl",
    "initsec : KW_INIT stmtlist KW_END",
    "initsec : KW_FINALIZ stmtlist KW_END",
    "initsec : KW_INIT stmtlist KW_FINALIZ stmtlist KW_END",
    "initsec : block",
    "initsec : KW_END",
    "main_block : maindecllist block",
    "main_block : block",
    "maindecllist : maindeclsec",
    "maindecllist : maindecllist maindeclsec",
    "declseclist : funcdeclsec",
    "declseclist : declseclist funcdeclsec",
    "interfdecl : basicdeclsec",
    "interfdecl : staticclassopt procproto",
    "interfdecl : thrvarsec",
    "maindeclsec : basicdeclsec",
    "maindeclsec : thrvarsec",
    "maindeclsec : exportsec",
    "maindeclsec : staticclassopt procdeclnondef",
    "maindeclsec : staticclassopt procdefinition",
    "maindeclsec : labeldeclsec",
    "funcdeclsec : basicdeclsec",
    "funcdeclsec : labeldeclsec",
    "funcdeclsec : procdefinition",
    "funcdeclsec : procdeclnondef",
    "basicdeclsec : constsec",
    "basicdeclsec : typesec",
    "basicdeclsec : varsec",
    "typesec : KW_TYPE typedecl",
    "typesec : typesec typedecl",
    "labeldeclsec : KW_LABEL labelidlist SEMICOL",
    "labelidlist : labelid",
    "labelidlist : labelidlist COMMA labelid",
    "labelid : CONST_INT",
    "labelid : id",
    "exportsec : KW_EXPORTS exportsitemlist",
    "exportsitemlist : exportsitem",
    "exportsitemlist : exportsitemlist COMMA exportsitem",
    "exportsitem : id",
    "exportsitem : id KW_NAME string_const",
    "exportsitem : id KW_INDEX expr",
    "procdeclnondef : procdefproto func_nondef_list funcdir_strict_opt",
    "procdefinition : procdefproto proc_define SEMICOL",
    "procdefproto : procbasickind qualid formalparams funcretopt SEMICOL funcdir_strict_opt",
    "procproto : procbasickind qualid formalparams funcretopt SEMICOL funcdirectopt",
    "funcretopt :",
    "funcretopt : COLON funcrettype",
    "staticclassopt :",
    "staticclassopt : KW_CLASS",
    "procbasickind : KW_FUNCTION",
    "procbasickind : KW_PROCEDURE",
    "procbasickind : KW_CONSTRUCTOR",
    "procbasickind : KW_DESTRUCTOR",
    "proceduretype : KW_PROCEDURE formalparams ofobjectopt",
    "proceduretype : KW_FUNCTION formalparams COLON funcrettype ofobjectopt",
    "ofobjectopt :",
    "ofobjectopt : KW_OF KW_OBJECT",
    "proc_define : func_block",
    "proc_define : assemblerstmt",
    "func_block : declseclist block",
    "func_block : block",
    "formalparams :",
    "formalparams : LPAREN RPAREN",
    "formalparams : LPAREN formalparamslist RPAREN",
    "formalparamslist : formalparm",
    "formalparamslist : formalparamslist SEMICOL formalparm",
    "formalparm : paramqualif idlist paramtypeopt",
    "formalparm : idlist paramtypespec paraminitopt",
    "formalparm : KW_CONST idlist paramtypeopt paraminitopt",
    "paramqualif : KW_VAR",
    "paramqualif : KW_OUT",
    "paramtypeopt :",
    "paramtypeopt : paramtypespec",
    "paramtypespec : COLON funcparamtype",
    "paraminitopt :",
    "paraminitopt : KW_EQ expr",
    "funcdirectopt :",
    "funcdirectopt : funcdirective_list SEMICOL",
    "funcdirectopt_nonterm :",
    "funcdirectopt_nonterm : funcdirective_list",
    "funcdir_strict_opt :",
    "funcdir_strict_opt : funcdir_strict_list",
    "funcdirective_list : funcdirective",
    "funcdirective_list : funcdirective_list SEMICOL funcdirective",
    "funcdir_strict_list : funcdir_strict SEMICOL",
    "funcdir_strict_list : funcdir_strict_list funcdir_strict SEMICOL",
    "func_nondef_list : funcdir_nondef SEMICOL",
    "func_nondef_list : func_nondef_list funcdir_nondef SEMICOL",
    "funcdirective : funcdir_strict",
    "funcdirective : funcdir_nondef",
    "funcdir_strict : funcqualif",
    "funcdir_strict : funccallconv",
    "funcdir_nondef : KW_EXTERNAL string_const externarg",
    "funcdir_nondef : KW_EXTERNAL qualid externarg",
    "funcdir_nondef : KW_EXTERNAL",
    "funcdir_nondef : KW_FORWARD",
    "externarg :",
    "externarg : id string_const",
    "externarg : id qualid",
    "funcqualif : KW_ABSOLUTE",
    "funcqualif : KW_ABSTRACT",
    "funcqualif : KW_ASSEMBLER",
    "funcqualif : KW_DYNAMIC",
    "funcqualif : KW_EXPORT",
    "funcqualif : KW_INLINE",
    "funcqualif : KW_OVERRIDE",
    "funcqualif : KW_OVERLOAD",
    "funcqualif : KW_REINTRODUCE",
    "funcqualif : KW_VIRTUAL",
    "funcqualif : KW_VARARGS",
    "funccallconv : KW_PASCAL",
    "funccallconv : KW_SAFECALL",
    "funccallconv : KW_STDCALL",
    "funccallconv : KW_CDECL",
    "funccallconv : KW_REGISTER",
    "block : KW_BEGIN stmtlist KW_END",
    "stmtlist : stmt SEMICOL stmtlist",
    "stmtlist : stmt",
    "stmt : nonlbl_stmt",
    "stmt : labelid COLON nonlbl_stmt",
    "nonlbl_stmt :",
    "nonlbl_stmt : inheritstmts",
    "nonlbl_stmt : goto_stmt",
    "nonlbl_stmt : block",
    "nonlbl_stmt : ifstmt",
    "nonlbl_stmt : casestmt",
    "nonlbl_stmt : repeatstmt",
    "nonlbl_stmt : whilestmt",
    "nonlbl_stmt : forstmt",
    "nonlbl_stmt : with_stmt",
    "nonlbl_stmt : tryexceptstmt",
    "nonlbl_stmt : tryfinallystmt",
    "nonlbl_stmt : raisestmt",
    "nonlbl_stmt : assemblerstmt",
    "inheritstmts : KW_INHERITED",
    "inheritstmts : KW_INHERITED assign",
    "inheritstmts : KW_INHERITED proccall",
    "inheritstmts : proccall",
    "inheritstmts : assign",
    "assign : lvalue KW_ASSIGN expr",
    "assign : lvalue KW_ASSIGN KW_INHERITED expr",
    "goto_stmt : KW_GOTO labelid",
    "ifstmt : KW_IF expr KW_THEN nonlbl_stmt KW_ELSE nonlbl_stmt",
    "ifstmt : KW_IF expr KW_THEN nonlbl_stmt",
    "casestmt : KW_CASE expr KW_OF caseselectorlist else_case KW_END",
    "else_case :",
    "else_case : KW_ELSE nonlbl_stmt",
    "else_case : KW_ELSE nonlbl_stmt SEMICOL",
    "caseselectorlist : caseselector",
    "caseselectorlist : caseselectorlist SEMICOL caseselector",
    "caseselector :",
    "caseselector : caselabellist COLON nonlbl_stmt",
    "caselabellist : caselabel",
    "caselabellist : caselabellist COMMA caselabel",
    "caselabel : expr",
    "caselabel : expr KW_RANGE expr",
    "repeatstmt : KW_REPEAT stmtlist KW_UNTIL expr",
    "whilestmt : KW_WHILE expr KW_DO nonlbl_stmt",
    "forstmt : KW_FOR id KW_ASSIGN expr KW_TO expr KW_DO nonlbl_stmt",
    "forstmt : KW_FOR id KW_ASSIGN expr KW_DOWNTO expr KW_DO nonlbl_stmt",
    "with_stmt : KW_WITH exprlist KW_DO nonlbl_stmt",
    "tryexceptstmt : KW_TRY stmtlist KW_EXCEPT exceptionblock KW_END",
    "exceptionblock : onlist KW_ELSE stmtlist",
    "exceptionblock : onlist",
    "exceptionblock : stmtlist",
    "onlist : ondef",
    "onlist : onlist ondef",
    "ondef : KW_ON id COLON id KW_DO nonlbl_stmt SEMICOL",
    "ondef : KW_ON id KW_DO nonlbl_stmt SEMICOL",
    "tryfinallystmt : KW_TRY stmtlist KW_FINALLY stmtlist KW_END",
    "raisestmt : KW_RAISE",
    "raisestmt : KW_RAISE lvalue",
    "raisestmt : KW_RAISE KW_AT expr",
    "raisestmt : KW_RAISE lvalue KW_AT expr",
    "assemblerstmt : KW_ASM asmcode KW_END",
    "asmcode : ASM_OP",
    "asmcode : asmcode ASM_OP",
    "varsec : KW_VAR vardecllist",
    "thrvarsec : KW_THRVAR vardecllist",
    "vardecllist : vardecl",
    "vardecllist : vardecllist vardecl",
    "vardecl : idlist COLON vartype vardeclopt SEMICOL",
    "vardecl : idlist COLON proceduretype SEMICOL funcdirectopt",
    "vardecl : idlist COLON proceduretype SEMICOL funcdirectopt_nonterm KW_EQ CONST_NIL SEMICOL",
    "vardeclopt :",
    "vardeclopt : KW_ABSOLUTE id",
    "vardeclopt : KW_EQ constexpr",
    "proccall : id",
    "proccall : lvalue KW_DOT id",
    "proccall : lvalue LPAREN exprlistopt RPAREN",
    "proccall : lvalue LPAREN casttype RPAREN",
    "lvalue : proccall",
    "lvalue : expr KW_DEREF",
    "lvalue : lvalue LBRAC exprlist RBRAC",
    "lvalue : string_const LBRAC exprlist RBRAC",
    "lvalue : casttype LPAREN exprlistopt RPAREN",
    "expr : literal",
    "expr : lvalue",
    "expr : setconstructor",
    "expr : KW_ADDR expr",
    "expr : KW_NOT expr",
    "expr : sign expr",
    "expr : LPAREN expr RPAREN",
    "expr : expr relop expr",
    "expr : expr addop expr",
    "expr : expr mulop expr",
    "sign : KW_SUB",
    "sign : KW_SUM",
    "mulop : KW_MUL",
    "mulop : KW_DIV",
    "mulop : KW_QUOT",
    "mulop : KW_MOD",
    "mulop : KW_SHR",
    "mulop : KW_SHL",
    "mulop : KW_AND",
    "addop : KW_SUB",
    "addop : KW_SUM",
    "addop : KW_OR",
    "addop : KW_XOR",
    "relop : KW_EQ",
    "relop : KW_DIFF",
    "relop : KW_LT",
    "relop : KW_LE",
    "relop : KW_GT",
    "relop : KW_GE",
    "relop : KW_IN",
    "relop : KW_IS",
    "relop : KW_AS",
    "literal : CONST_INT",
    "literal : CONST_BOOL",
    "literal : CONST_REAL",
    "literal : CONST_NIL",
    "literal : string_const",
    "discrete : CONST_INT",
    "discrete : CONST_CHAR",
    "discrete : CONST_BOOL",
    "string_const : CONST_STR",
    "string_const : CONST_CHAR",
    "string_const : string_const CONST_STR",
    "string_const : string_const CONST_CHAR",
    "id : IDENTIFIER",
    "idlist : id",
    "idlist : idlist COMMA id",
    "qualid : id",
    "qualid : qualid KW_DOT id",
    "exprlist : expr",
    "exprlist : exprlist COMMA expr",
    "exprlistopt :",
    "exprlistopt : exprlist",
    "rangetype : sign rangestart KW_RANGE expr",
    "rangetype : rangestart KW_RANGE expr",
    "rangestart : discrete",
    "rangestart : qualid",
    "rangestart : id LPAREN casttype RPAREN",
    "rangestart : id LPAREN literal RPAREN",
    "enumtype : LPAREN enumtypeellist RPAREN",
    "enumtypeellist : enumtypeel",
    "enumtypeellist : enumtypeellist COMMA enumtypeel",
    "enumtypeel : id",
    "enumtypeel : id KW_EQ expr",
    "setconstructor : LBRAC RBRAC",
    "setconstructor : LBRAC setlist RBRAC",
    "setlist : setelem",
    "setlist : setlist COMMA setelem",
    "setelem : expr",
    "setelem : expr KW_RANGE expr",
    "constsec : KW_CONST constdecl",
    "constsec : constsec constdecl",
    "constdecl : id KW_EQ constexpr SEMICOL",
    "constdecl : id COLON vartype KW_EQ constexpr SEMICOL",
    "constexpr : expr",
    "constexpr : arrayconst",
    "constexpr : recordconst",
    "arrayconst : LPAREN constexpr COMMA constexprlist RPAREN",
    "constexprlist : constexpr",
    "constexprlist : constexprlist COMMA constexpr",
    "recordconst : LPAREN fieldconstlist RPAREN",
    "recordconst : LPAREN fieldconstlist SEMICOL RPAREN",
    "fieldconstlist : fieldconst",
    "fieldconstlist : fieldconstlist SEMICOL fieldconst",
    "fieldconst : id COLON constexpr",
    "classtype : class_keyword heritage class_struct_opt KW_END",
    "classtype : class_keyword heritage",
    "class_keyword : KW_CLASS",
    "class_keyword : KW_OBJECT",
    "class_keyword : KW_RECORD",
    "heritage :",
    "heritage : LPAREN idlist RPAREN",
    "class_struct_opt : fieldlist complist scopeseclist",
    "scopeseclist :",
    "scopeseclist : scopeseclist scopesec",
    "scopesec : scope_decl fieldlist complist",
    "scope_decl : KW_PUBLISHED",
    "scope_decl : KW_PUBLIC",
    "scope_decl : KW_PROTECTED",
    "scope_decl : KW_PRIVATE",
    "fieldlist :",
    "fieldlist : fieldlist objfield",
    "complist :",
    "complist : complist class_comp",
    "objfield : idlist COLON type SEMICOL",
    "class_comp : staticclassopt procproto",
    "class_comp : property",
    "interftype : KW_INTERF heritage classmethodlistopt classproplistopt KW_END",
    "classmethodlistopt : methodlist",
    "classmethodlistopt :",
    "methodlist : procproto",
    "methodlist : methodlist procproto",
    "classproplistopt : classproplist",
    "classproplistopt :",
    "classproplist : property",
    "classproplist : classproplist property",
    "property : KW_PROPERTY id SEMICOL",
    "property : KW_PROPERTY id propinterfopt COLON funcrettype indexspecopt propspecifiers SEMICOL defaultdiropt",
    "defaultdiropt :",
    "defaultdiropt : id SEMICOL",
    "propinterfopt :",
    "propinterfopt : LBRAC idlisttypeidlist RBRAC",
    "idlisttypeidlist : idlisttypeid",
    "idlisttypeidlist : idlisttypeidlist SEMICOL idlisttypeid",
    "idlisttypeid : idlist COLON funcparamtype",
    "idlisttypeid : KW_CONST idlist COLON funcparamtype",
    "indexspecopt :",
    "indexspecopt : KW_INDEX expr",
    "propspecifiers :",
    "propspecifiers : id id",
    "propspecifiers : id id id id",
    "propspecifiers : id id id id id id",
    "propspecifiers : id id id id id id id id",
    "propspecifiers : id id id id id id id id id id",
    "typedecl : id KW_EQ typeopt vartype SEMICOL",
    "typedecl : id KW_EQ typeopt proceduretype SEMICOL funcdirectopt",
    "typeopt :",
    "typeopt : KW_TYPE",
    "type : vartype",
    "type : proceduretype",
    "vartype : simpletype",
    "vartype : enumtype",
    "vartype : rangetype",
    "vartype : varianttype",
    "vartype : refpointertype",
    "vartype : classreftype",
    "vartype : KW_PACKED packedtype",
    "vartype : packedtype",
    "packedtype : structype",
    "packedtype : restrictedtype",
    "classreftype : KW_CLASS KW_OF scalartype",
    "simpletype : scalartype",
    "simpletype : realtype",
    "simpletype : stringtype",
    "simpletype : TYPE_PTR",
    "ordinaltype : enumtype",
    "ordinaltype : rangetype",
    "ordinaltype : scalartype",
    "scalartype : inttype",
    "scalartype : chartype",
    "scalartype : qualid",
    "realtype : TYPE_REAL48",
    "realtype : TYPE_FLOAT",
    "realtype : TYPE_DOUBLE",
    "realtype : TYPE_EXTENDED",
    "realtype : TYPE_CURR",
    "realtype : TYPE_COMP",
    "inttype : TYPE_BYTE",
    "inttype : TYPE_BOOL",
    "inttype : TYPE_INT",
    "inttype : TYPE_SHORTINT",
    "inttype : TYPE_SMALLINT",
    "inttype : TYPE_LONGINT",
    "inttype : TYPE_INT64",
    "inttype : TYPE_UINT64",
    "inttype : TYPE_WORD",
    "inttype : TYPE_LONGWORD",
    "inttype : TYPE_CARDINAL",
    "chartype : TYPE_CHAR",
    "chartype : TYPE_WIDECHAR",
    "stringtype : TYPE_STR",
    "stringtype : TYPE_PCHAR",
    "stringtype : TYPE_STR LBRAC expr RBRAC",
    "stringtype : TYPE_SHORTSTR",
    "stringtype : TYPE_WIDESTR",
    "varianttype : TYPE_VAR",
    "varianttype : TYPE_OLEVAR",
    "structype : arraytype",
    "structype : settype",
    "structype : filetype",
    "restrictedtype : classtype",
    "restrictedtype : interftype",
    "arraysizelist : rangetype",
    "arraysizelist : arraysizelist COMMA rangetype",
    "arraytype : TYPE_ARRAY LBRAC arraytypedef RBRAC KW_OF type",
    "arraytype : TYPE_ARRAY KW_OF type",
    "arraytypedef : arraysizelist",
    "arraytypedef : inttype",
    "arraytypedef : chartype",
    "arraytypedef : qualid",
    "settype : TYPE_SET KW_OF ordinaltype",
    "filetype : TYPE_FILE KW_OF type",
    "filetype : TYPE_FILE",
    "refpointertype : KW_DEREF type",
    "funcparamtype : simpletype",
    "funcparamtype : TYPE_ARRAY KW_OF simpletype",
    "funcrettype : simpletype",
    "casttype : inttype",
    "casttype : chartype",
    "casttype : realtype",
    "casttype : stringtype",
    "casttype : TYPE_PTR",
  };
 public static string getRule (int index) {
    return yyRule [index];
 }
}
  protected static  string [] yyNames = {    
    "end-of-file",null,null,null,null,null,null,null,null,null,null,null,
    null,null,null,null,null,null,null,null,null,null,null,null,null,null,
    null,null,null,null,null,null,null,null,null,null,null,null,null,null,
    null,null,null,null,null,null,null,null,null,null,null,null,null,null,
    null,null,null,null,null,null,null,null,null,null,null,null,null,null,
    null,null,null,null,null,null,null,null,null,null,null,null,null,null,
    null,null,null,null,null,null,null,null,null,null,null,null,null,null,
    null,null,null,null,null,null,null,null,null,null,null,null,null,null,
    null,null,null,null,null,null,null,null,null,null,null,null,null,null,
    null,null,null,null,null,null,null,null,null,null,null,null,null,null,
    null,null,null,null,null,null,null,null,null,null,null,null,null,null,
    null,null,null,null,null,null,null,null,null,null,null,null,null,null,
    null,null,null,null,null,null,null,null,null,null,null,null,null,null,
    null,null,null,null,null,null,null,null,null,null,null,null,null,null,
    null,null,null,null,null,null,null,null,null,null,null,null,null,null,
    null,null,null,null,null,null,null,null,null,null,null,null,null,null,
    null,null,null,null,null,null,null,null,null,null,null,null,null,null,
    null,null,null,null,null,null,null,null,null,null,null,null,null,null,
    null,null,null,null,null,null,null,"KW_LIBRARY","KW_UNIT",
    "KW_PROGRAM","KW_PACKAGE","KW_REQUIRES","KW_CONTAINS","KW_USES",
    "KW_EXPORTS","KW_PLATFORM","KW_DEPRECATED","KW_INTERF","KW_IMPL",
    "KW_FINALIZ","KW_INIT","KW_OBJECT","KW_RECORD","KW_CLASS",
    "KW_FUNCTION","KW_PROCEDURE","KW_PROPERTY","KW_OF","KW_OUT",
    "KW_PACKED","KW_INHERITED","KW_PROTECTED","KW_PUBLIC","KW_PUBLISHED",
    "KW_PRIVATE","KW_CONST","KW_VAR","KW_THRVAR","KW_TYPE",
    "KW_CONSTRUCTOR","KW_DESTRUCTOR","KW_ASM","KW_BEGIN","KW_END",
    "KW_WITH","KW_DO","KW_FOR","KW_TO","KW_DOWNTO","KW_REPEAT","KW_UNTIL",
    "KW_WHILE","KW_IF","KW_THEN","KW_ELSE","KW_CASE","KW_GOTO","KW_LABEL",
    "KW_RAISE","KW_AT","KW_TRY","KW_EXCEPT","KW_FINALLY","KW_ON",
    "KW_ABSOLUTE","KW_ABSTRACT","KW_ASSEMBLER","KW_DYNAMIC","KW_EXPORT",
    "KW_EXTERNAL","KW_FORWARD","KW_INLINE","KW_OVERRIDE","KW_OVERLOAD",
    "KW_REINTRODUCE","KW_VIRTUAL","KW_VARARGS","KW_PASCAL","KW_SAFECALL",
    "KW_STDCALL","KW_CDECL","KW_REGISTER","KW_NAME","KW_READ","KW_WRITE",
    "KW_INDEX","KW_STORED","KW_DEFAULT","KW_NODEFAULT","KW_IMPLEMENTS",
    "TYPE_INT64","TYPE_INT","TYPE_LONGINT","TYPE_LONGWORD",
    "TYPE_SMALLINT","TYPE_SHORTINT","TYPE_WORD","TYPE_BYTE",
    "TYPE_CARDINAL","TYPE_UINT64","TYPE_CHAR","TYPE_PCHAR",
    "TYPE_WIDECHAR","TYPE_WIDESTR","TYPE_STR","TYPE_RSCSTR",
    "TYPE_SHORTSTR","TYPE_FLOAT","TYPE_REAL48","TYPE_DOUBLE",
    "TYPE_EXTENDED","TYPE_BOOL","TYPE_COMP","TYPE_CURRENCY","TYPE_OLEVAR",
    "TYPE_VAR","TYPE_ARRAY","TYPE_CURR","TYPE_FILE","TYPE_PTR","TYPE_SET",
    "ASM_OP","LOWESTPREC","EXPR_SINGLE","CONST_INT","CONST_REAL",
    "CONST_CHAR","CONST_STR","IDENTIFIER","CONST_NIL","CONST_BOOL",
    "KW_RANGE","COMMA","COLON","SEMICOL","KW_ASSIGN","KW_EQ","KW_GT",
    "KW_LT","KW_LE","KW_GE","KW_DIFF","KW_IN","KW_IS","KW_SUM","KW_SUB",
    "KW_OR","KW_XOR","KW_MUL","KW_DIV","KW_QUOT","KW_MOD","KW_SHL",
    "KW_SHR","KW_AS","KW_AND","KW_DEREF","KW_DOT","UNARY","KW_NOT",
    "KW_ADDR","LBRAC","RBRAC","LPAREN","RPAREN","MAXPREC",
  };

  /** index-checked interface to yyNames[].
      @param token single character or %token value.
      @return token name or [illegal] or [unknown].
    */
  public static string yyname (int token) {
    if ((token < 0) || (token > yyNames.Length)) return "[illegal]";
    string name;
    if ((name = yyNames[token]) != null) return name;
    return "[unknown]";
  }

  int yyExpectingState;
  /** computes list of expected tokens on error by tracing the tables.
      @param state for which to compute the list.
      @return list of token names.
    */
  protected int [] yyExpectingTokens (int state){
    int token, n, len = 0;
    bool[] ok = new bool[yyNames.Length];
    if ((n = yySindex[state]) != 0)
      for (token = n < 0 ? -n : 0;
           (token < yyNames.Length) && (n+token < yyTable.Length); ++ token)
        if (yyCheck[n+token] == token && !ok[token] && yyNames[token] != null) {
          ++ len;
          ok[token] = true;
        }
    if ((n = yyRindex[state]) != 0)
      for (token = n < 0 ? -n : 0;
           (token < yyNames.Length) && (n+token < yyTable.Length); ++ token)
        if (yyCheck[n+token] == token && !ok[token] && yyNames[token] != null) {
          ++ len;
          ok[token] = true;
        }
    int [] result = new int [len];
    for (n = token = 0; n < len;  ++ token)
      if (ok[token]) result[n++] = token;
    return result;
  }
  protected string[] yyExpecting (int state) {
    int [] tokens = yyExpectingTokens (state);
    string [] result = new string[tokens.Length];
    for (int n = 0; n < tokens.Length;  n++)
      result[n++] = yyNames[tokens [n]];
    return result;
  }

  /** the generated parser, with debugging messages.
      Maintains a state and a value stack, currently with fixed maximum size.
      @param yyLex scanner.
      @param yydebug debug message writer implementing yyDebug, or null.
      @return result of the last reduction, if any.
      @throws yyException on irrecoverable parse error.
    */
  internal Object yyparse (yyParser.yyInput yyLex, Object yyd)
				 {
    this.debug = (yydebug.yyDebug)yyd;
    return yyparse(yyLex);
  }

  /** initial size and increment of the state/value stack [default 256].
      This is not final so that it can be overwritten outside of invocations
      of yyparse().
    */
  protected int yyMax;

  /** executed at the beginning of a reduce action.
      Used as $$ = yyDefault($1), prior to the user-specified action, if any.
      Can be overwritten to provide deep copy, etc.
      @param first value for $1, or null.
      @return first.
    */
  protected Object yyDefault (Object first) {
    return first;
  }

  /** the generated parser.
      Maintains a state and a value stack, currently with fixed maximum size.
      @param yyLex scanner.
      @return result of the last reduction, if any.
      @throws yyException on irrecoverable parse error.
    */
  internal Object yyparse (yyParser.yyInput yyLex)
  {
    if (yyMax <= 0) yyMax = 256;			// initial size
    int yyState = 0;                                   // state stack ptr
    int [] yyStates = new int[yyMax];	                // state stack 
    Object yyVal = null;                               // value stack ptr
    Object [] yyVals = new Object[yyMax];	        // value stack
    int yyToken = -1;					// current input
    int yyErrorFlag = 0;				// #tks to shift

    /*yyLoop:*/ for (int yyTop = 0;; ++ yyTop) {
      if (yyTop >= yyStates.Length) {			// dynamically increase
        int[] i = new int[yyStates.Length+yyMax];
        yyStates.CopyTo (i, 0);
        yyStates = i;
        Object[] o = new Object[yyVals.Length+yyMax];
        yyVals.CopyTo (o, 0);
        yyVals = o;
      }
      yyStates[yyTop] = yyState;
      yyVals[yyTop] = yyVal;
      if (debug != null) debug.push(yyState, yyVal);

      /*yyDiscarded:*/ for (;;) {	// discarding a token does not change stack
        int yyN;
        if ((yyN = yyDefRed[yyState]) == 0) {	// else [default] reduce (yyN)
          if (yyToken < 0) {
            yyToken = yyLex.advance() ? yyLex.token() : 0;
            if (debug != null)
              debug.lex(yyState, yyToken, yyname(yyToken), yyLex.value());
          }
          if ((yyN = yySindex[yyState]) != 0 && ((yyN += yyToken) >= 0)
              && (yyN < yyTable.Length) && (yyCheck[yyN] == yyToken)) {
            if (debug != null)
              debug.shift(yyState, yyTable[yyN], yyErrorFlag-1);
            yyState = yyTable[yyN];		// shift to yyN
            yyVal = yyLex.value();
            yyToken = -1;
            if (yyErrorFlag > 0) -- yyErrorFlag;
            goto continue_yyLoop;
          }
          if ((yyN = yyRindex[yyState]) != 0 && (yyN += yyToken) >= 0
              && yyN < yyTable.Length && yyCheck[yyN] == yyToken)
            yyN = yyTable[yyN];			// reduce (yyN)
          else
            switch (yyErrorFlag) {
  
            case 0:
              yyExpectingState = yyState;
              // yyerror(String.Format ("syntax error, got token `{0}'", yyname (yyToken)), yyExpecting(yyState));
              if (debug != null) debug.error("syntax error");
              if (yyToken == 0 /*eof*/ || yyToken == eof_token) throw new yyParser.yyUnexpectedEof ();
              goto case 1;
            case 1: case 2:
              yyErrorFlag = 3;
              do {
                if ((yyN = yySindex[yyStates[yyTop]]) != 0
                    && (yyN += Token.yyErrorCode) >= 0 && yyN < yyTable.Length
                    && yyCheck[yyN] == Token.yyErrorCode) {
                  if (debug != null)
                    debug.shift(yyStates[yyTop], yyTable[yyN], 3);
                  yyState = yyTable[yyN];
                  yyVal = yyLex.value();
                  goto continue_yyLoop;
                }
                if (debug != null) debug.pop(yyStates[yyTop]);
              } while (-- yyTop >= 0);
              if (debug != null) debug.reject();
              throw new yyParser.yyException("irrecoverable syntax error");
  
            case 3:
              if (yyToken == 0) {
                if (debug != null) debug.reject();
                throw new yyParser.yyException("irrecoverable syntax error at end-of-file");
              }
              if (debug != null)
                debug.discard(yyState, yyToken, yyname(yyToken),
  							yyLex.value());
              yyToken = -1;
              goto continue_yyDiscarded;		// leave stack alone
            }
        }
        int yyV = yyTop + 1-yyLen[yyN];
        if (debug != null)
          debug.reduce(yyState, yyStates[yyV-1], yyN, YYRules.getRule (yyN), yyLen[yyN]);
        yyVal = yyDefault(yyV > yyTop ? null : yyVals[yyV]);
        switch (yyN) {
case 1:
#line 206 "DelphiGrammar.y"
  { /* YYACCEPT; */
						
						}
  break;
#line default
        }
        yyTop -= yyLen[yyN];
        yyState = yyStates[yyTop];
        int yyM = yyLhs[yyN];
        if (yyState == 0 && yyM == 0) {
          if (debug != null) debug.shift(0, yyFinal);
          yyState = yyFinal;
          if (yyToken < 0) {
            yyToken = yyLex.advance() ? yyLex.token() : 0;
            if (debug != null)
               debug.lex(yyState, yyToken,yyname(yyToken), yyLex.value());
          }
          if (yyToken == 0) {
            if (debug != null) debug.accept(yyVal);
            return yyVal;
          }
          goto continue_yyLoop;
        }
        if (((yyN = yyGindex[yyM]) != 0) && ((yyN += yyState) >= 0)
            && (yyN < yyTable.Length) && (yyCheck[yyN] == yyState))
          yyState = yyTable[yyN];
        else
          yyState = yyDgoto[yyM];
        if (debug != null) debug.shift(yyStates[yyTop], yyState);
	 goto continue_yyLoop;
      continue_yyDiscarded: continue;	// implements the named-loop continue: 'continue yyDiscarded'
      }
    continue_yyLoop: continue;		// implements the named-loop continue: 'continue yyLoop'
    }
  }

   static  short [] yyLhs  = {              -1,
    0,    1,    1,    1,    1,    2,    2,    4,    5,    3,
   12,   13,    7,    7,   15,   15,   16,   16,   10,   10,
    9,   19,   19,   11,   11,   11,   11,   11,    8,    8,
   18,   18,   24,   24,   20,   20,   20,   23,   23,   23,
   23,   23,   23,   25,   25,   25,   25,   26,   26,   26,
   35,   35,   33,   38,   38,   39,   39,   30,   40,   40,
   41,   41,   41,   31,   32,   43,   28,   50,   50,   27,
   27,   47,   47,   47,   47,   53,   53,   54,   54,   46,
   46,   55,   55,   49,   49,   49,   57,   57,   58,   58,
   58,   59,   59,   60,   60,   61,   62,   62,   51,   51,
   65,   65,   45,   45,   64,   64,   66,   66,   44,   44,
   67,   67,   68,   68,   69,   69,   69,   69,   72,   72,
   72,   70,   70,   70,   70,   70,   70,   70,   70,   70,
   70,   70,   71,   71,   71,   71,   71,   22,   21,   21,
   73,   73,   74,   74,   74,   74,   74,   74,   74,   74,
   74,   74,   74,   74,   74,   74,   75,   75,   75,   75,
   75,   86,   86,   76,   77,   77,   78,   90,   90,   90,
   89,   89,   91,   91,   92,   92,   93,   93,   79,   80,
   81,   81,   82,   83,   95,   95,   95,   96,   96,   97,
   97,   84,   85,   85,   85,   85,   56,   98,   98,   36,
   29,   99,   99,  100,  100,  100,  102,  102,  102,   87,
   87,   87,   87,   88,   88,   88,   88,   88,   42,   42,
   42,   42,   42,   42,   42,   42,   42,   42,  108,  108,
  111,  111,  111,  111,  111,  111,  111,  110,  110,  110,
  110,  109,  109,  109,  109,  109,  109,  109,  109,  109,
  106,  106,  106,  106,  106,  112,  112,  112,   17,   17,
   17,   17,    6,   14,   14,   48,   48,   94,   94,  104,
  104,  113,  113,  114,  114,  114,  114,  115,  116,  116,
  117,  117,  107,  107,  118,  118,  119,  119,   34,   34,
  120,  120,  103,  103,  103,  121,  123,  123,  122,  122,
  124,  124,  125,  126,  126,  127,  127,  127,  128,  128,
  129,  132,  132,  133,  134,  134,  134,  134,  130,  130,
  131,  131,  135,  136,  136,  139,  140,  140,  142,  142,
  141,  141,  143,  143,  138,  138,  147,  147,  144,  144,
  148,  148,  149,  149,  145,  145,  146,  146,  146,  146,
  146,  146,   37,   37,  150,  150,  137,  137,  101,  101,
  101,  101,  101,  101,  101,  101,  155,  155,  154,  151,
  151,  151,  151,  161,  161,  161,  158,  158,  158,  159,
  159,  159,  159,  159,  159,  162,  162,  162,  162,  162,
  162,  162,  162,  162,  162,  162,  163,  163,  160,  160,
  160,  160,  160,  152,  152,  156,  156,  156,  157,  157,
  167,  167,  164,  164,  168,  168,  168,  168,  165,  166,
  166,  153,   63,   63,   52,  105,  105,  105,  105,  105,
  };
   static  short [] yyLen = {           2,
    2,    1,    1,    1,    1,    5,    2,    5,    6,    6,
    3,    3,    0,    3,    1,    3,    1,    3,    3,    2,
    3,    0,    2,    3,    3,    5,    1,    1,    2,    1,
    1,    2,    1,    2,    1,    2,    1,    1,    1,    1,
    2,    2,    1,    1,    1,    1,    1,    1,    1,    1,
    2,    2,    3,    1,    3,    1,    1,    2,    1,    3,
    1,    3,    3,    3,    3,    6,    6,    0,    2,    0,
    1,    1,    1,    1,    1,    3,    5,    0,    2,    1,
    1,    2,    1,    0,    2,    3,    1,    3,    3,    3,
    4,    1,    1,    0,    1,    2,    0,    2,    0,    2,
    0,    1,    0,    1,    1,    3,    2,    3,    2,    3,
    1,    1,    1,    1,    3,    3,    1,    1,    0,    2,
    2,    1,    1,    1,    1,    1,    1,    1,    1,    1,
    1,    1,    1,    1,    1,    1,    1,    3,    3,    1,
    1,    3,    0,    1,    1,    1,    1,    1,    1,    1,
    1,    1,    1,    1,    1,    1,    1,    2,    2,    1,
    1,    3,    4,    2,    6,    4,    6,    0,    2,    3,
    1,    3,    0,    3,    1,    3,    1,    3,    4,    4,
    8,    8,    4,    5,    3,    1,    1,    1,    2,    7,
    5,    5,    1,    2,    3,    4,    3,    1,    2,    2,
    2,    1,    2,    5,    5,    8,    0,    2,    2,    1,
    3,    4,    4,    1,    2,    4,    4,    4,    1,    1,
    1,    2,    2,    2,    3,    3,    3,    3,    1,    1,
    1,    1,    1,    1,    1,    1,    1,    1,    1,    1,
    1,    1,    1,    1,    1,    1,    1,    1,    1,    1,
    1,    1,    1,    1,    1,    1,    1,    1,    1,    1,
    2,    2,    1,    1,    3,    1,    3,    1,    3,    0,
    1,    4,    3,    1,    1,    4,    4,    3,    1,    3,
    1,    3,    2,    3,    1,    3,    1,    3,    2,    2,
    4,    6,    1,    1,    1,    5,    1,    3,    3,    4,
    1,    3,    3,    4,    2,    1,    1,    1,    0,    3,
    3,    0,    2,    3,    1,    1,    1,    1,    0,    2,
    0,    2,    4,    2,    1,    5,    1,    0,    1,    2,
    1,    0,    1,    2,    3,    9,    0,    2,    0,    3,
    1,    3,    3,    4,    0,    2,    0,    2,    4,    6,
    8,   10,    5,    6,    0,    1,    1,    1,    1,    1,
    1,    1,    1,    1,    2,    1,    1,    1,    3,    1,
    1,    1,    1,    1,    1,    1,    1,    1,    1,    1,
    1,    1,    1,    1,    1,    1,    1,    1,    1,    1,
    1,    1,    1,    1,    1,    1,    1,    1,    1,    1,
    4,    1,    1,    1,    1,    1,    1,    1,    1,    1,
    1,    3,    6,    3,    1,    1,    1,    1,    3,    3,
    1,    2,    1,    3,    1,    1,    1,    1,    1,    1,
  };
   static  short [] yyDefRed = {            0,
    0,    0,    0,    0,  263,    0,    0,    2,    3,    4,
    5,    0,    0,    0,    0,    0,    0,    0,   15,    1,
    0,    0,   71,    0,    0,    0,    0,    0,    0,    7,
    0,   30,   31,   38,    0,   39,   40,   43,    0,    0,
   50,    0,    0,    0,    0,    0,   14,    0,    0,    0,
   59,    0,  289,  264,    0,    0,  202,    0,    0,   51,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,  392,  388,  391,  395,  390,  389,  394,  386,  396,
  393,  397,  400,  398,  403,    0,  402,  381,  380,  382,
  383,  387,  385,  384,  430,    0,  253,  260,  259,  254,
  252,  230,  229,    0,    0,    0,    0,    0,    0,    0,
  146,    0,    0,  156,    0,  141,  144,  145,  147,  148,
  149,  150,  151,  152,  153,  154,  155,  161,    0,    0,
    0,  219,  221,    0,  428,  429,  426,  427,   56,   57,
    0,   54,   29,   32,   72,   73,   74,   75,   41,   42,
    0,    0,  290,   52,    0,    0,    0,    0,    0,   16,
    0,    0,    0,    0,    0,    0,    0,    0,    0,  203,
    0,  251,  210,  158,    0,  198,    0,    0,  214,    0,
    0,    0,    0,    0,    0,    0,  164,    0,    0,    0,
    0,  223,  222,  283,    0,    0,  285,    0,  262,  261,
    0,  138,    0,  242,  246,  244,  245,  247,  243,  248,
  249,  239,  238,  240,  241,  231,  232,  233,  234,  236,
  235,  250,  237,  215,    0,    0,    0,    0,    0,    0,
    0,    0,    0,  224,    0,   53,    0,  118,   83,    0,
   33,   44,   47,   46,   45,    0,    0,   80,   81,    0,
  266,    0,    8,   22,    0,    0,    6,    0,    0,    0,
    0,    0,   60,    0,  307,  308,    0,    0,  405,  404,
    0,    0,  373,    0,  256,  257,  258,    0,    0,    0,
    0,    0,    0,  274,  361,    0,  360,  409,    0,  410,
  359,  362,  363,  364,  366,  367,  368,  370,  371,  372,
  377,  378,  406,  407,  408,    0,    0,    0,  294,  295,
  265,    0,    0,    0,    0,  356,    0,  197,  199,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,  284,  225,    0,  142,    0,    0,    0,
  139,    0,    0,  211,    0,    0,    0,    0,    0,   55,
    0,    0,   82,   34,  122,  123,  124,  125,  126,  127,
  128,  129,  130,  131,  132,  133,  134,  135,  136,  137,
   64,    0,    0,    0,  113,  114,   65,  109,    0,    0,
    0,    0,    0,    0,    0,   28,    9,   27,   11,    0,
   10,    0,    0,    0,  306,  365,    0,    0,    0,    0,
  358,  357,  422,    0,    0,  279,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,  301,  291,    0,    0,
    0,    0,    0,    0,    0,    0,  183,    0,    0,    0,
  180,    0,    0,    0,  171,    0,  175,    0,    0,  187,
    0,    0,  188,    0,  401,    0,  286,  217,    0,  216,
  212,  213,  218,    0,  115,  116,    0,  107,  110,  267,
   93,    0,   92,   85,    0,    0,   87,    0,    0,    0,
   23,   35,    0,   37,    0,    0,    0,   12,    0,  329,
    0,    0,    0,    0,  369,  414,    0,  411,  416,  417,
    0,    0,  420,  375,  374,  376,  419,    0,    0,  278,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,  299,    0,    0,   76,  205,    0,    0,  105,  111,
  112,  208,  209,  204,    0,  353,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,  184,    0,  189,  192,
    0,    0,  108,    0,    0,    0,    0,   86,    0,   69,
  425,    0,   36,   25,    0,   24,  310,    0,    0,  333,
    0,    0,  330,    0,    0,    0,  280,  276,  277,  292,
    0,  304,    0,    0,  320,  303,  297,    0,  300,    0,
  302,    0,   79,    0,    0,  354,    0,    0,    0,  165,
    0,    0,  172,  167,  176,  174,    0,    0,  185,    0,
   95,    0,   96,  423,    0,   90,   88,   89,   66,    0,
    0,    0,  326,  334,  412,    0,    0,    0,    0,  322,
  325,    0,  296,   77,  106,    0,    0,    0,  170,    0,
    0,   91,    0,    0,   26,    0,  335,    0,    0,  413,
    0,  324,  317,  316,  315,  318,  313,  319,  298,  206,
  181,  182,  191,    0,  424,    0,    0,    0,    0,  341,
    0,  323,    0,    0,   67,    0,    0,    0,  340,    0,
    0,  190,    0,  343,  342,    0,    0,  344,    0,    0,
    0,    0,    0,    0,    0,  336,    0,  338,    0,    0,
    0,    0,    0,  352,
  };
  protected static  short [] yyDgoto  = {             6,
    7,    8,    9,   10,   11,  173,   13,   30,  157,  256,
  387,  162,  260,   55,   18,   19,  109,   31,  382,  471,
  110,  111,   33,  240,  241,   34,   35,  480,   36,   37,
  243,  244,   38,   39,   40,   41,   60,  141,  112,   50,
   51,  113,  151,  246,  371,  247,  481,  281,  381,  470,
  516,  550,  401,  515,  248,  114,  466,  467,  468,  600,
  601,  606,  603,  587,  518,  372,  519,  520,  521,  375,
  376,  455,  115,  116,  117,  118,  119,  120,  121,  122,
  123,  124,  125,  126,  127,  128,  179,  180,  434,  533,
  435,  436,  437,  346,  441,  442,  443,  177,   56,   57,
  402,  424,  308,  347,  131,  132,  133,  134,  225,  226,
  227,  284,  285,  286,  287,  405,  406,  196,  197,   53,
  309,  310,  578,  416,  417,  288,  289,  393,  507,  508,
  574,  619,  647,  648,  575,  620,  403,  621,  290,  482,
  561,  483,  562,  639,  677,  681,  686,  659,  660,  317,
  291,  292,  293,  294,  295,  296,  297,  298,  135,  136,
  497,  137,  138,  303,  304,  305,  491,  492,
  };
  protected static  short [] yySindex = {         -152,
 -316, -316, -316, -316,    0,    0, -307,    0,    0,    0,
    0, -316,  632, -274, -263, -206, -240,  113,    0,    0,
 -190, -316,    0, -316, -316, -316, -316, 1714, -130,    0,
  632,    0,    0,    0,  253,    0,    0,    0, -316, -316,
    0,   32,   37,   32,  -35, -316,    0, -316,   25,  -83,
    0,   53,    0,    0, -125, -316,    0, -316,  -70,    0,
 2365,  -12, 2365, -316, 1714, 2365, 2365, 2365, -130, 2193,
 1714,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,  -56,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0, 2365, 2365, 2291, 2365,    0, -123,   77,
    0,   -6, 3060,    0,   96,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,  -68,
   -8,    0,    0, 2365,    0,    0,    0,    0,    0,    0,
  133,    0,    0,    0,    0,    0,    0,    0,    0,    0,
 1404, -316,    0,    0,  632,   32,  194,  632,   39,    0,
 -316, -316,  -35, 2365, -316, 1271, 2439, -316, 1051,    0,
  200,    0,    0,    0,    0,    0, -203, 3060,    0,   76,
 -197,  107,  218, 2053, 2224,  982,    0, 2365, -186,  238,
 2365,    0,    0,    0, 2989, -273,    0, 2933,    0,    0,
 2365,    0, 1805,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0, 2365, 2365, 2365, 1714, 1880, -316,
 2365, 2365, 2365,    0, -130,    0,  193,    0,    0,  559,
    0,    0,    0,    0,    0, 3174,  118,    0,    0,  137,
    0,  -49,    0,    0,   32,  142,    0,  171, -316,  243,
   39, 3060,    0,  128,    0,    0,  254,  135,    0,    0,
 -229,  296,    0,  303,    0,    0,    0, 1051, -316,  183,
  205,  214,   78,    0,    0,  235,    0,    0,  128,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0, 2439, 3060,  223,    0,    0,
    0,  207,  207,  240, -158,    0, 1051,    0,    0, 1805,
 2365, 2365, 2365, 1805, 1805, 2365, 3060, 2365, 1623, 1714,
 2962, 2365, 2365,    0,    0, -254,    0, 1643,  732,  231,
    0, 2365, 3060,    0, -147,  251,  215,   93,  246,    0,
  227, -105,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0, 2366,  257,  268,    0,    0,    0,    0, -316, -205,
  289,  469,  793, 1714, 1714,    0,    0,    0,    0,  195,
    0, -316,  253, 2911,    0,    0, 1051, 2522, 1051, 2015,
    0,    0,    0,  312, -279,    0, 2792, 2439,  205,  301,
 2365,    0,  306, 2933,  325, -162,    0,    0,  329,  446,
 3174, -316, 2439,  341,  343,  344,    0, 3060, 2120, 3060,
    0,  425, 3015, -189,    0,  180,    0, 3060, -316,    0,
  443,  -64,    0,  450,    0, 3060,    0,    0, 3060,    0,
    0,    0,    0,  193,    0,    0,  363,    0,    0,    0,
    0, -316,    0,    0,  184, -120,    0, -316, 2872,  364,
    0,    0,  253,    0,  793,  456,  145,    0, -255,    0,
 -316,  477,  253,  205,    0,    0,  205,    0,    0,    0,
  376,  347,    0,    0,    0,    0,    0, 2365, -316,    0,
   39,  355,  356,  377, 2365, 3060,  483, -316, 2439, 2439,
 -285,    0, 2872,  506,    0,    0,  395,  394,    0,    0,
    0,    0,    0,    0, 3174,    0, 2365, 2365, 1805, 2365,
 1805, 2365,  488, 2365, 1805, -196,    0, 1714,    0,    0,
   39,  205,    0,  184, 2833,  396, -217,    0,  184,    0,
    0, 2366,    0,    0, 1714,    0,    0,  -49, -316,    0,
  499,  477,    0,  285,  517, 3060,    0,    0,    0,    0,
 3060,    0,  209,  175,    0,    0,    0,  -82,    0,  306,
    0,  446,    0, 3174,  420,    0,  395, 2074, 2095,    0,
 3060,  413,    0,    0,    0,    0, 1805, -316,    0,  396,
    0,  523,    0,    0, 2365,    0,    0,    0,    0,  510,
  289,   19,    0,    0,    0, 1051, 1051,  253,  482,    0,
    0, 2439,    0,    0,    0,  422, 1805, 1805,    0,  423,
  509,    0, 2872, 3060,    0,  424,    0, -232,  428,    0,
  429,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0, 1805,    0, 3174, -316,  284, -137,    0,
 2872,    0, -316,  431,    0,  294, 2833, -232,    0,  481,
  175,    0, 2833,    0,    0, 2365, -316,    0, 3060, -316,
  433, -316, -316, -316,  444,    0, -316,    0, -316, -316,
 -316, -316, -316,    0,
  };
  protected static  short [] yyRindex = {         2824,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,  421,    0,    0,    0,  202,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0, -262,    0,    0,
  421,    0,    0,    0,    0,    0,    0,    0, 2635, 2662,
    0, 2824,    0, 2824,    0,    0,    0,    0,  445, 2743,
    0,    0,    0,    0,    0, 2689,    0, 2716,    0,    0,
  -19,    0,    0,    0, -242,    0,    0,    0,    0,   36,
 -174,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,  -63,    0,    0,    0,    0,
    0,    0,    0,    0,    0, 3039,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0, 1357,  715,    0,
    0,    0,    0,    0,   -3,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0, 1387, 3081,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,  421, 1313,    0,  421,  226,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
 1161,    0,    0,    0, 1417,    0,    0, -207,    0,  761,
    0,    0,    0,    0,    0,    0,    0,    0, 1556,    0,
    0,    0,    0,    0,  -16,    0,    0,    0,    0,    0,
    0,    0,  687,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,  687,    0,    0,
    0,  416,  416,    0,    0,    0,  229,    0,    0,    0,
    0,    0,    0,    0,    0, 2770,    0,    0,    0,    0,
    0,  317,    0,    0, 2797,    0,    0,    0,    0,    0,
  550, 1290,    0,  684,    0,    0, -198,    0,    0,    0,
    0, -182,    0,    0,    0,    0,    0,    0,    0,    6,
  -95,    0,    0,    0,    0,    0,    0,    0, 2030,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0, -265,    0,    0,    0,
    0,  448, -188,    0,  462,    0,    0,    0,    0,  153,
    0,    0,    0,  153,  153, -237,  247,    0, -262, -262,
    0,    0,    0,    0,    0,    0,    0,  498,  953,  907,
    0,    0,  264,    0,    0,  438,    0,    0,    0,    0,
  239,  239,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0, 2568,    0,    0,    0,    0,    0,    0,    0,    0,
  471,  245, 1315, -262, -233,    0,    0,    0,    0,    0,
    0,    0, -119,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,  -81,    0,    0,    0,    0,  472,    0,
    0, 2052, 2904,  476,    0,    0,    0,    0,    0, -173,
 1180,    0,    0,    0,    0,    0,    0, -145,    0,  342,
    0,  968,  334,  566,    0,    0,    0,  563,    0,    0,
    0,  567,    0,    0,    0,    9,    0,    0,  602,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0, 1444,    0,    0,    0,    0,    0,
    0,  568,  -37,   47,    0,    0, -269,    0,    0,    0,
  452,    0,    0,    0,    0,    0,    0,    0,    0,    0,
  451,    0,    0,    0,    0, -114,    0, 1913,    0,    0,
    0,    0,    0,    0,    0,    0,  484,    0,    0,    0,
    0,    0,    0,    0, 2009,    0,    0,    0,  153,    0,
 -262, -237,    0,    0, -230,    0,    0, -262,    0,    0,
  244,  252,    0,  -10,    0,   10,    0,    0,   33,    0,
    0, 1552,    0,    0, -262,    0,    0,  317,    0,    0,
    0,  569,    0,    0,    0,  -79,    0,    0,    0,    0,
 -106,    0,    0,  802,    0,    0,    0,    0,    0,    0,
    0, -173,    0, 1609,    0,    0,    0,    0,    0,    0,
  339,  575,    0,    0,    0,    0,  485,    0,    0,   10,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
  471,  489,    0,    0,    0,    0,    0,    0,  580,    0,
    0,    0,    0,    0,    0,    0,  153,  153,    0,    0,
    0,    0,    0,   35,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,  485,    0, 1207,    0,    0,    0,    0,
    0,    0, 1913,    0,    0,    0,    0,    0,    0,   82,
  988,    0,    0,    0,    0,    0,  493,    0,  112,    0,
    0,  494, 2301,    0,    0,    0,  513,    0,    0,  514,
    0,  515,    0,    0,
  };
  protected static  short [] yyGindex = {            0,
    0,    0,    0,    0,    0,    1,  -23,  321,    0,    0,
    0,    0,    0,  -36,    0,  837,  -45,  511,    0,    0,
  -47,   21,  -25,    0,  660, -139, -375, -429,  519,    0,
  868,  869,  -96,    0,    0,    0,  867,    0,  -18,    0,
  743,  418,    0,    0,  359,    0,  -20, -135, -303,  304,
 -479, -471, -124,  348,    0,  778,    0,  384,    0,  385,
  468,  335, -163,  520,    0,    0,  352, -238, -101,    0,
    0,  588,    0, -160,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,  883,   59,   43,    0,    0,
  419,    0,  427,  -30,    0,    0,  512,    0,  927,  599,
 -134,    0, -268,  722, -210,  555,    0,  -87,    0,    0,
    0,    0, -361,  674,  564,    0,  464,    0,  634,  926,
    0,    0,    0,    0,  457,    0,    0,  680,    0,  323,
  318,    0,    0,    0,    0,    0, -383, -346,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,  307,    0,
 -449,    0,    0,    0,  723,    0,    0,   99,  276,  309,
    0,  -72,  305,    0,    0,    0,    0,    0,
  };
  protected static  short [] yyTable = {           159,
   12,   14,   15,   16,   17,  144,  473,  373,  419,  420,
  142,  242,   21,  486,  152,  493,  252,  183,  155,  551,
  158,  348,   49,  190,   52,   54,   54,   59,  108,  140,
  143,  282,  181,   32,  315,  143,  488,  415,  494,   52,
   59,  582,  337,  553,  314,  586,   17,  397,  161,  250,
  187,  143,  657,  563,  245,  173,   54,  143,   54,  143,
  461,    5,  143,  551,  182,  108,  173,  462,  463,  140,
  130,  108,  461,  143,  306,  306,  306,  306,  283,  462,
  463,  283,  306,  306,  306,  306,  129,  268,   84,  318,
  306,  306,    5,  301,  306,  604,  301,  320,  597,   20,
  242,  352,  499,  130,    1,    2,    3,  130,  333,   42,
    4,  275,  189,  130,  531,  306,  293,  261,  293,  175,
   43,  143,  328,  129,  258,   84,  168,  321,  579,  129,
  152,  421,  254,  457,  500,  560,  143,  143,  334,  504,
   78,  143,  418,  245,  374,    5,  173,  409,  293,  269,
  143,   45,  251,  143,  523,  422,  328,  448,  557,  427,
    5,   54,  259,  431,  432,   49,  280,  319,  311,  280,
  336,  239,    5,  328,  268,   32,  665,   44,   32,  306,
  341,  398,  426,  655,  321,  306,  598,  306,  642,  670,
  283,  351,  425,   48,  532,   84,  502,   84,  618,  273,
  345,  421,  615,  421,  268,  301,  268,  272,  464,  143,
   78,  551,   78,  399,  306,  614,  350,  604,  379,  152,
  230,  511,  390,  604,  231,    5,  232,  423,  108,  283,
  344,  383,  640,  641,  321,  140,  269,  251,  327,  538,
  576,  577,  472,  139,  301,  130,  668,    5,  439,  157,
  399,  512,  199,  200,  611,  327,  168,  169,  484,   54,
  353,  129,  487,  547,  450,  140,  269,  273,  269,  273,
  130,  399,    5,  157,  669,  272,  388,  272,  280,  404,
  157,  440,  444,  280,  157,  275,  129,  201,  379,  140,
  379,  157,  157,  548,    4,  618,  140,  273,  165,  622,
  281,  379,  282,  156,  193,  272,  413,  140,  140,  283,
  283,  283,  283,  373,  399,  171,  229,  280,  542,  266,
  399,  301,  399,  379,  301,  489,  301,  301,  193,  108,
  108,  623,  281,  484,  282,  193,  476,  477,  230,  193,
   98,   99,  231,  465,  232,  558,  193,  193,  399,  399,
  399,  454,  454,  649,  191,  479,  163,  379,  176,  164,
  379,  501,  130,  380,  157,  287,  130,  130,  590,  202,
  592,  130,  130,   94,  596,   94,  203,  484,  129,  460,
   54,  379,  129,  129,  108,  108,  266,  129,  129,  266,
  288,  266,   54,   97,  251,  287,  301,  280,  280,  280,
  280,  264,  637,   94,  233,  265,  266,  395,  541,  484,
  384,  385,  266,  555,  199,  200,   94,  266,   98,  193,
  288,  143,  522,   97,  379,  544,  130,  130,  409,  638,
  379,  549,  379,   28,  386,  166,  630,  556,  167,  536,
  301,  299,  129,  129,  299,  143,   94,   23,   98,  144,
  559,  275,  143,  276,  251,    5,  143,  277,  379,  345,
  379,  255,   54,  143,  143,  345,  651,  652,   54,  251,
  302,  573,  301,  302,  300,  253,  283,  300,  257,  228,
  178,  251,  230,  184,  185,  186,  231,  316,  232,  346,
  599,  322,  485,  664,   46,  346,   47,  484,  496,  404,
  271,  377,  272,  674,  274,  233,  452,  610,   54,  678,
  465,  580,   21,  251,  235,  195,  236,  323,   70,   70,
  378,  192,  193,  195,  198,  484,  145,  146,  283,  283,
  394,  484,  162,   70,   70,  391,  143,  484,  108,  195,
  392,  147,  148,  301,  301,  251,  195,   54,  329,  330,
  195,  234,  168,  299,  389,  108,  162,  195,  195,  612,
  301,  534,  535,  162,  280,  168,  545,  162,   98,   99,
    5,  130,  399,  130,  162,  162,  168,  130,  478,  400,
  130,  262,  302,   17,  307,   17,  300,  129,  301,  129,
  168,  617,  299,  129,  301,  407,  129,  130,  631,  408,
  301,  658,  199,  200,    5,  327,  418,   18,  331,   18,
  179,  379,  117,  129,  117,  411,  280,  280,  178,  380,
  666,  302,  119,  421,  119,  300,  573,  120,  451,  120,
  195,  658,  321,  251,  179,  121,  224,  121,   54,  130,
  458,  179,  338,  339,  340,  179,  343,  162,  178,  178,
  178,  459,  179,  179,  170,  129,  170,   54,  275,  453,
  276,  251,    5,   54,  277,  168,  667,  251,   54,  130,
  130,  469,  299,  251,  299,  168,  673,  680,  102,  103,
  682,  505,  684,  685,  687,  129,  129,  689,  509,  690,
  691,  692,  693,  694,   70,   70,  130,  498,  302,   84,
   84,  302,  490,  302,  302,  300,  510,  300,   61,   70,
   70,  513,  129,   61,   61,  177,  177,   61,   61,   61,
  178,  178,  514,  414,  524,  179,  525,  526,  529,   61,
   61,   61,   61,   61,   61,  537,   61,   61,  428,  429,
  430,   23,  540,  433,  299,  438,  543,  552,  554,  446,
  195,   61,  559,   24,   25,   26,   27,  564,  565,  449,
  570,  226,  643,  644,  645,  646,  226,  226,  568,  569,
  226,  226,  226,  302,  226,  572,  583,  300,  584,  585,
  594,  605,  226,  226,  226,  226,  226,  226,  299,  226,
  226,  613,  226,  616,  226,  226,  629,  226,  626,  633,
  226,  226,  635,  654,  226,  650,  653,  656,  226,  226,
  661,  226,  662,   62,  672,  676,  683,  302,   62,   62,
  299,  300,   62,   62,   62,  307,   61,  688,  506,  270,
   84,  196,  145,  146,   62,   62,   62,   62,   62,   62,
  307,   62,   62,   24,   25,  207,   27,  147,  148,  302,
   28,  271,  275,  300,   68,  196,   62,  293,  168,  186,
  332,  331,  196,  415,  255,   29,  196,  169,  143,  102,
  163,  339,  311,  196,  196,  226,  347,  348,  226,  226,
  226,  226,  160,  226,  226,  226,  226,  226,  226,  226,
  226,  299,  299,  475,  163,   22,  349,  350,  351,  354,
  474,  163,  149,  150,   23,  163,  154,  263,  299,  226,
  609,  226,  163,  163,  636,  566,   24,   25,   26,   27,
  302,  302,  571,   28,  300,  300,  307,  307,  249,  624,
  607,   62,  546,  608,  632,  625,  299,  302,   29,  456,
  517,  300,  299,  174,  588,  589,  196,  591,  299,  433,
  593,  433,   58,  539,  349,  143,  410,  309,  309,  309,
  595,  503,  567,  495,  153,  302,  447,  581,  412,  300,
  663,  302,  309,  309,  675,  300,  309,  302,  255,  143,
  671,  300,    0,  255,  255,  163,  143,  255,  255,  255,
  396,  255,    0,    0,    0,    0,    0,  143,  143,  255,
  255,  255,  255,  255,  255,    0,  255,  255,    0,  255,
    0,  255,  255,    0,  255,    0,    0,  255,  255,    0,
    0,  255,  634,    0,  220,  255,  255,    0,  255,  220,
  220,    0,    0,  220,  220,  220,    0,  220,    0,  307,
    0,    0,    0,    0,    0,  220,  220,  220,  220,  220,
  220,    0,  220,  220,    0,  220,   22,  220,  220,    0,
  220,    0,    0,  220,  220,   23,    0,  220,    0,    0,
  143,  220,  220,    0,  220,   70,   70,   24,   25,   26,
   27,    0,  312,  312,  312,  312,    0,    0,    0,    0,
   70,   70,  255,  679,  312,  255,  255,  255,  255,   29,
  255,  255,  255,  255,  255,  255,  255,  255,  255,  255,
  255,  255,  255,  255,  255,  255,  255,  255,  255,  255,
  255,    0,    0,    0,    0,    0,  255,    0,  255,  216,
  217,  218,  219,  220,  221,  222,  223,  224,  220,    0,
    0,  220,  220,  220,  220,    0,  220,  220,  220,  220,
  220,  220,  220,  220,  220,  220,  220,  220,  220,  220,
  220,  220,  220,  220,  220,  220,  220,    0,    0,    0,
  228,    0,  220,    0,  220,  228,  228,    0,    0,  228,
  228,  228,    0,  228,    0,    0,    0,    0,    0,    0,
    0,  228,  228,  228,  228,  228,  228,    0,  228,  228,
    0,  228,    0,  228,  228,    0,  228,    0,    0,  228,
  228,    0,    0,  228,    0,    0,  227,  228,  228,    0,
  228,  227,  227,    0,    0,  227,  227,  227,    0,  227,
    0,    0,    0,    0,    0,    0,  166,  227,  227,  227,
  227,  227,  227,    0,  227,  227,    0,  227,    0,  227,
  227,    0,  227,    0,    0,  227,  227,    0,  326,  227,
  166,   70,   70,  227,  227,    0,  227,  166,  314,  314,
  314,  314,    0,    0,    0,    0,   70,   70,  166,  166,
  314,    0,    0,    0,  228,    0,    0,  228,  228,  228,
  228,    0,  228,  228,  228,  228,  228,  228,  228,  228,
  228,  228,  228,  228,  228,  228,  228,  228,  228,  228,
  228,  228,    0,    0,    0,    0,    0,  264,  228,    0,
  228,  265,  266,  267,  312,  313,    0,    0,    0,  268,
  227,    0,    0,  227,  227,  227,  227,    0,  227,  227,
  227,  227,  227,  227,  227,  227,  227,  227,  227,  227,
    0,  166,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,  227,    0,  227,  204,  205,  206,
  207,  208,  209,  210,  211,  212,  213,  214,  215,  216,
  217,  218,  219,  220,  221,  222,  223,  224,    0,    0,
   72,   73,   74,   75,   76,   77,   78,   79,   80,   81,
   82,   83,   84,   85,   86,    0,   87,   88,   89,   90,
   91,   92,   93,    0,  269,  270,  271,   94,  272,  273,
  274,    0,    0,    0,  275,    0,  276,  355,    5,    0,
  277,  355,  355,  355,  355,  355,    0,    0,    0,  355,
    0,    0,    0,   99,  102,  103,    0,   99,   99,   99,
    0,    0,   99,   99,   99,    0,  278,    0,    0,    0,
    0,    0,    0,  279,   99,   99,   99,   99,   99,   99,
    0,   99,   99,    0,   99,    0,    0,    0,    0,   99,
   99,   99,   99,    0,    0,    0,   99,   99,   99,   99,
   99,   99,   99,   99,   99,   99,   99,    0,    0,   99,
  355,  355,  355,  355,  355,  355,  355,  355,  355,  355,
  355,  355,  355,  355,  355,    0,  355,  355,  355,  355,
  355,  355,  355,    0,  355,  355,  355,  355,  355,  355,
  355,    0,    0,    0,  355,    0,  355,  264,  355,    0,
  355,  265,  266,  267,    0,    0,    0,    0,    0,  268,
    0,    0,    0,   63,  355,  355,    0,   99,   63,   63,
    0,    0,   63,   63,   63,  101,  355,    0,    0,    0,
    0,    0,    0,  355,   63,   63,   63,   63,   63,   63,
   13,   63,   63,   20,   20,   13,   13,   13,   70,   70,
    0,    0,    0,    0,    0,    0,   63,   13,   13,   13,
   13,   13,   13,   70,   70,    0,   20,   20,    0,    0,
   72,   73,   74,   75,   76,   77,   78,   79,   80,   81,
   82,   83,   84,   85,   86,  210,   87,   88,   89,   90,
   91,   92,   93,    0,  269,  270,  271,   94,  272,  273,
  274,    0,    0,    0,  275,    0,  276,    0,    5,  210,
  277,    0,    0,    0,    0,  160,  210,    0,    0,    0,
    0,    0,    0,    0,  102,  103,    0,  210,  210,    0,
    0,   63,    0,    0,    0,    0,  278,  145,  146,  160,
    0,    0,    0,  279,    0,  159,  160,    0,   24,   25,
  160,   27,  147,  148,   62,   28,    0,  160,  160,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,  159,
   29,    0,   19,   19,    0,    0,  159,   70,   70,    0,
  159,    0,  237,  238,    0,    0,    0,  159,  159,    0,
    0,    0,   70,   70,    0,   19,   19,    0,    0,   57,
  210,  210,  210,  210,  210,  210,  210,  210,  210,  210,
  210,  210,  210,  210,  210,  210,  210,  210,  210,  210,
  210,  210,  210,  210,    0,    0,    0,  210,    0,  210,
  160,  214,  214,  214,  214,  214,  214,  214,  214,  214,
  214,  214,  214,  214,  214,  214,  214,  214,  214,  214,
  214,  214,  214,  214,    0,    0,    0,  214,    0,  214,
  159,  214,  214,  214,  214,  214,  214,  214,  214,  214,
  214,  214,  214,  214,  214,  214,  214,  214,  214,  214,
  214,  214,  214,  214,  194,  103,  103,  214,    0,  214,
    0,    0,    0,    0,    0,    0,  103,  103,    0,  103,
  103,  103,  103,  103,    0,    0,    0,    0,  194,    0,
    0,    0,    0,    0,    0,  194,    0,    0,  103,  194,
    0,    0,    0,    0,    0,    0,  194,  194,    0,    0,
  103,  103,  100,    0,    0,    0,  100,  100,  100,    0,
    0,  100,  100,  100,  100,    0,    0,    0,    0,  100,
  100,  100,  100,  100,  100,  100,  100,  100,  100,    0,
  100,  100,   61,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,   62,   28,  100,   63,    0,   64,    0,
    0,   65,    0,   66,   67,    0,    0,   68,   69,    0,
   70,    0,   71,    0,    0,  439,    0,    0,    0,  194,
    0,  220,  220,  220,  220,  220,  220,  220,  220,  220,
  220,  220,  220,  220,  220,  220,  220,  220,  220,  220,
  220,  220,   72,   73,   74,   75,   76,   77,   78,   79,
   80,   81,   82,   83,   84,   85,   86,    0,   87,   88,
   89,   90,   91,   92,   93,    0,  100,    0,    0,   94,
    0,   95,    0,   61,    0,    0,   96,   97,   98,   99,
    5,  100,  101,    0,   62,   28,    0,   63,    0,   64,
    0,    0,   65,    0,   66,   67,  102,  103,   68,   69,
    0,   70,    0,   71,    0,    0,    0,    0,    0,    0,
    0,  104,  105,  106,    0,  107,  212,  213,  214,  215,
  216,  217,  218,  219,  220,  221,  222,  223,  224,    0,
    0,    0,    0,   72,   73,   74,   75,   76,   77,   78,
   79,   80,   81,   82,   83,   84,   85,   86,    0,   87,
   88,   89,   90,   91,   92,   93,    0,    0,    0,    0,
   94,    0,   95,    0,   61,    0,    0,   96,   97,   98,
   99,    5,  100,  101,    0,   62,   28,    0,   63,    0,
   64,    0,    0,   65,    0,   66,   67,  102,  103,   68,
   69,    0,   70,    0,   71,    0,    0,    0,    0,    0,
    0,    0,  104,  105,  106,    0,  107,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,   72,   73,   74,   75,   76,   77,
   78,   79,   80,   81,   82,   83,   84,   85,   86,  342,
   87,   88,   89,   90,   91,   92,   93,    0,    0,    0,
    0,   94,    0,   95,    0,    0,    0,    0,  172,   97,
   98,   99,    5,  100,  101,  321,  321,  321,  321,    0,
    0,    0,    0,  321,  321,  321,  321,    0,  102,  103,
    0,  321,  321,    0,    0,  321,    0,    0,    0,    0,
    0,    0,    0,  104,  105,  106,    0,  107,    0,   72,
   73,   74,   75,   76,   77,   78,   79,   80,   81,   82,
   83,   84,   85,   86,    0,   87,   88,   89,   90,   91,
   92,   93,    0,    0,    0,    0,   94,    0,   95,    0,
    0,    0,    0,  172,   97,   98,   99,    5,  100,  101,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,   99,  102,  103,    0,   99,   99,   99,    0,
    0,   99,   99,   99,    0,    0,    0,    0,  104,  105,
  106,    0,  107,   99,   99,   99,   99,   99,   99,    0,
   99,   99,  309,  309,  309,  309,    0,    0,    0,    0,
  309,  309,  309,  309,    0,   99,    0,    0,  309,  309,
    0,    0,  309,    0,  319,  319,  319,  319,    0,    0,
    0,    0,  319,  319,  319,  319,    0,    0,    0,    0,
  319,  319,    0,  309,  319,    0,    0,  324,    0,    0,
    0,    0,    0,    0,   72,   73,   74,   75,   76,   77,
   78,   79,   80,   81,   82,  305,   84,    0,  627,    0,
    0,    0,    0,    0,    0,   92,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,   99,    0,  275,  628,
  276,    0,    5,    0,  277,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,  309,  102,  103,
    0,    0,    0,  309,    0,  309,  527,  528,    0,    0,
    0,    0,    0,    0,    0,    0,    0,  279,    0,  319,
    0,    0,    0,    0,    0,  305,    0,  305,  204,  205,
  206,  207,  208,  209,  210,  211,  212,  213,  214,  215,
  216,  217,  218,  219,  220,  221,  222,  223,  224,  204,
  205,  206,  207,  208,  209,  210,  211,  212,  213,  214,
  215,  216,  217,  218,  219,  220,  221,  222,  223,  224,
  204,  205,  206,  207,  208,  209,  210,  211,  212,  213,
  214,  215,  216,  217,  218,  219,  220,  221,  222,  223,
  224,  188,    0,    0,    0,  204,  205,  206,  207,  208,
  209,  210,  211,  212,  213,  214,  215,  216,  217,  218,
  219,  220,  221,  222,  223,  224,  325,    0,    0,    0,
    0,    0,   72,   73,   74,   75,   76,   77,   78,   79,
   80,   81,   82,   83,   84,   85,   86,    0,   87,   88,
   89,   90,   91,   92,   93,    0,    0,    0,    0,   94,
    0,   95,    0,    0,    0,    0,  172,   97,   98,   99,
    5,  100,  101,  337,  337,  337,  337,    0,    0,    0,
    0,  337,  337,  337,  337,    0,  102,  103,    0,  337,
  337,    0,    0,  337,    0,    0,    0,    0,    0,    0,
    0,  104,  105,  106,    0,  107,    0,    0,    0,  204,
  205,  206,  207,  208,  209,  210,  211,  212,  213,  214,
  215,  216,  217,  218,  219,  220,  221,  222,  223,  224,
   72,   73,   74,   75,   76,   77,   78,   79,   80,   81,
   82,   83,   84,   85,   86,    0,   87,   88,   89,   90,
   91,   92,   93,    0,    0,    0,    0,   94,    0,   95,
    0,    0,    0,    0,  172,   97,   98,   99,    5,  100,
  101,    0,    0,    0,    0,    0,    0,    0,    0,  355,
  356,  357,  358,  359,  102,  103,  360,  361,  362,  363,
  364,  365,  366,  367,  368,  369,  370,    0,    0,  104,
  105,  106,  194,  107,   72,   73,   74,   75,   76,   77,
   78,   79,   80,   81,   82,   83,   84,   85,   86,    0,
   87,   88,   89,   90,   91,   92,   93,    0,    0,    0,
    0,   94,    0,   95,    0,    0,    0,    0,  172,   97,
   98,   99,    5,  100,  101,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,  102,  103,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,  104,  105,  106,    0,  107,   72,   73,
   74,   75,   76,   77,   78,   79,   80,   81,   82,   83,
   84,   85,   86,    0,   87,   88,   89,   90,   91,   92,
   93,    0,    0,    0,    0,   94,    0,   95,    0,    0,
    0,    0,  172,   97,   98,   99,    5,  100,  101,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,  104,  102,  103,    0,    0,  104,  104,    0,    0,
  104,  104,  104,    0,    0,    0,    0,  104,  105,  106,
    0,  306,  104,  104,  104,  104,  104,  104,  104,  104,
  104,   72,   73,   74,   75,   76,   77,   78,   79,   80,
   81,   82,    0,   84,  104,    0,    0,    0,    0,    0,
    0,    0,   92,    0,    0,    0,  104,  104,    0,    0,
    0,    0,    0,    0,    0,  275,    0,  276,   48,    5,
    0,  277,   48,   48,   48,    0,    0,   48,   48,   48,
    0,    0,    0,    0,    0,  102,  103,    0,    0,   48,
   48,   48,   48,   48,   48,   49,   48,   48,    0,   49,
   49,   49,    0,    0,   49,   49,   49,    0,    0,    0,
    0,   48,    0,    0,    0,    0,   49,   49,   49,   49,
   49,   49,  200,   49,   49,    0,  200,  200,  200,    0,
    0,  200,  200,  200,    0,    0,    0,    0,   49,    0,
    0,    0,    0,  200,  200,  200,  200,  200,  200,  201,
  200,  200,    0,  201,  201,  201,    0,    0,  201,  201,
  201,    0,    0,    0,    0,  200,    0,    0,    0,    0,
  201,  201,  201,  201,  201,  201,   58,  201,  201,    0,
    0,   58,   58,    0,    0,   58,   58,   58,    0,    0,
    0,    0,  201,    0,    0,    0,    0,   58,   58,   58,
   58,   58,   58,  103,   58,   58,    0,    0,  103,  103,
    0,    0,  103,  103,  103,    0,    0,    0,    0,   58,
    0,    0,    0,    0,  103,  103,  103,  103,  103,  103,
   13,  103,  103,    0,    0,   13,   13,    0,    0,   13,
   13,   13,    0,    0,    0,    0,  103,    0,    0,    0,
    0,   13,   13,   13,   13,   13,   13,   13,   13,   13,
    0,    0,    0,    0,    0,    0,   13,   13,   13,    0,
    0,    0,    0,   13,    0,    0,    0,    0,   13,   13,
   13,   13,   13,   13,    0,   13,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
   13,   72,   73,   74,   75,   76,   77,   78,   79,   80,
   81,   82,   83,   84,   85,   86,    0,   87,   88,   89,
   90,   91,   92,   93,    0,    0,    0,    0,   94,    0,
   95,    0,    0,    0,    0,  172,   97,   98,   99,    0,
  100,  101,   72,   73,   74,   75,   76,   77,   78,   79,
   80,   81,   82,   83,   84,   85,   86,    0,   87,   88,
   89,   90,   91,   92,   93,    0,    0,    0,  602,   94,
    0,  273,    0,    0,    0,    0,    0,    0,    0,    0,
    5,   72,   73,   74,   75,   76,   77,   78,   79,   80,
   81,   82,   83,   84,   85,   86,    0,   87,   88,   89,
   90,   91,   92,   93,    0,    0,    0,    0,   94,    0,
  273,    0,    0,    0,    0,    0,    0,    0,    0,    5,
   72,   73,   74,   75,   76,   77,   78,   79,   80,   81,
   82,    0,   84,    0,    0,    0,    0,    0,    0,    0,
    0,   92,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,  210,    0,    0,    5,  210,
  210,  210,  210,  210,  210,  210,  210,  210,  210,  210,
  210,  210,  210,  210,  210,  210,  210,  210,  210,  210,
  210,    0,    0,    0,  210,    0,  210,  210,  204,  205,
  206,  207,  208,  209,  210,  211,  212,  213,  214,  215,
  216,  217,  218,  219,  220,  221,  222,  223,  224,    0,
    0,    0,    0,    0,    0,    0,  335,  204,  205,  206,
  207,  208,  209,  210,  211,  212,  213,  214,  215,  216,
  217,  218,  219,  220,  221,  222,  223,  224,    0,  332,
    0,    0,    0,  445,  204,  205,  206,  207,  208,  209,
  210,  211,  212,  213,  214,  215,  216,  217,  218,  219,
  220,  221,  222,  223,  224,  530,    0,    0,    0,    0,
  204,  205,  206,  207,  208,  209,  210,  211,  212,  213,
  214,  215,  216,  217,  218,  219,  220,  221,  222,  223,
  224,   56,    0,    0,  251,  251,  251,  251,  251,  251,
  251,  251,  251,  251,  251,  251,  251,  251,  251,  251,
  251,  251,  251,  251,  251,  204,  205,  206,  207,  208,
  209,  210,  211,  212,  213,  214,  215,  216,  217,  218,
  219,  220,  221,  222,  223,  224,  220,  220,  220,  220,
  220,  220,  220,  220,  220,  220,  220,  220,  220,  220,
  220,  220,  220,  220,  220,  220,  220,  355,  356,  357,
  358,  359,  237,  238,  360,  361,  362,  363,  364,  365,
  366,  367,  368,  369,  370,
  };
  protected static  short [] yyCheck = {            45,
    0,    1,    2,    3,    4,   31,  382,  246,  312,  313,
   29,  151,   12,  397,   35,  399,  152,   65,   42,  469,
   44,  232,   22,   71,   24,   25,   26,   27,   28,   29,
  293,  166,   63,   13,  169,  269,  398,  306,  400,   39,
   40,  513,  203,  473,  169,  525,   46,  277,   48,  151,
   69,   31,  285,  483,  151,  293,   56,  300,   58,  293,
  278,  378,  293,  513,   64,   65,  304,  285,  286,   69,
   28,   71,  278,  304,  273,  274,  275,  276,  166,  285,
  286,  169,  281,  282,  283,  284,   28,  295,  277,  293,
  289,  290,  378,  166,  293,  545,  169,  295,  295,  407,
  240,  237,  382,   61,  257,  258,  259,   65,  382,  384,
  263,  381,   70,   71,  304,  314,  382,  163,  384,   61,
  384,  384,  309,   65,  161,  314,  382,  382,  414,   71,
  151,  314,  156,  372,  414,  482,  311,  312,  412,  408,
  314,  384,  412,  240,  246,  378,  384,  283,  414,  295,
  384,  392,  152,  384,  423,  314,  276,  412,  414,  320,
  378,  161,  162,  324,  325,  165,  166,  371,  168,  169,
  201,  151,  378,  293,  382,  155,  656,  384,  158,  378,
  228,  411,  317,  633,  382,  384,  383,  386,  618,  661,
  278,  237,  317,  384,  384,  384,  407,  386,  574,  314,
  231,  384,  564,  386,  412,  278,  414,  314,  414,  384,
  384,  661,  386,  277,  413,  562,  235,  667,  314,  240,
  407,  384,  259,  673,  411,  378,  413,  386,  228,  317,
  230,  255,  616,  617,  382,  235,  382,  237,  276,  304,
  509,  510,  382,  374,  317,  203,  384,  378,  313,  269,
  314,  414,  376,  377,  558,  293,  382,  383,  394,  259,
  240,  203,  398,  384,  412,  269,  412,  382,  414,  384,
  228,  335,  378,  293,  412,  382,  256,  384,  278,  279,
  300,  329,  330,  283,  304,  381,  228,  411,  384,  293,
  386,  311,  312,  414,  263,  671,  300,  412,  382,  382,
  382,  407,  382,  267,  269,  412,  306,  311,  312,  397,
  398,  399,  400,  552,  378,  386,  385,  317,  454,  314,
  384,  394,  386,  277,  397,  398,  399,  400,  293,  329,
  330,  414,  414,  469,  414,  300,  384,  385,  407,  304,
  376,  377,  411,  380,  413,  481,  311,  312,  412,  413,
  414,  351,  352,  622,  411,  392,  332,  407,  371,  335,
  314,  407,  320,  413,  384,  382,  324,  325,  529,  293,
  531,  329,  330,  384,  535,  386,  383,  513,  320,  379,
  380,  335,  324,  325,  384,  385,  381,  329,  330,  384,
  382,  386,  392,  384,  394,  412,  469,  397,  398,  399,
  400,  267,  384,  414,  413,  271,  272,  273,  454,  545,
  269,  270,  407,  269,  376,  377,  384,  412,  384,  384,
  412,  269,  422,  414,  378,  462,  384,  385,  564,  411,
  384,  468,  386,  292,  293,  383,  597,  293,  386,  439,
  513,  166,  384,  385,  169,  293,  414,  273,  414,  475,
  276,  374,  300,  376,  454,  378,  304,  380,  412,  378,
  414,  268,  462,  311,  312,  384,  627,  628,  468,  469,
  166,  508,  545,  169,  166,  155,  564,  169,  158,  384,
   63,  481,  407,   66,   67,   68,  411,  288,  413,  378,
  538,  385,  394,  654,  382,  384,  384,  633,  400,  499,
  366,  384,  368,  667,  370,  413,  414,  555,  508,  673,
  547,  511,  268,  513,  382,  269,  384,  300,  274,  275,
  384,  104,  105,  106,  107,  661,  274,  275,  616,  617,
  277,  667,  269,  289,  290,  293,  384,  673,  538,  293,
  413,  289,  290,  616,  617,  545,  300,  547,  311,  312,
  304,  134,  382,  278,  384,  555,  293,  311,  312,  559,
  633,  382,  383,  300,  564,  382,  383,  304,  376,  377,
  378,  529,  277,  531,  311,  312,  382,  535,  384,  277,
  538,  164,  278,  382,  167,  384,  278,  529,  661,  531,
  382,  383,  317,  535,  667,  413,  538,  555,  598,  386,
  673,  638,  376,  377,  378,  188,  384,  382,  191,  384,
  269,  407,  384,  555,  386,  381,  616,  617,  201,  413,
  657,  317,  384,  384,  386,  317,  663,  384,  414,  386,
  384,  668,  382,  633,  293,  384,  406,  386,  638,  597,
  384,  300,  225,  226,  227,  304,  229,  384,  231,  232,
  233,  384,  311,  312,   56,  597,   58,  657,  374,  414,
  376,  661,  378,  663,  380,  382,  383,  667,  668,  627,
  628,  383,  397,  673,  399,  382,  383,  677,  394,  395,
  680,  381,  682,  683,  684,  627,  628,  687,  383,  689,
  690,  691,  692,  693,  274,  275,  654,  386,  394,  383,
  384,  397,  398,  399,  400,  397,  382,  399,  264,  289,
  290,  383,  654,  269,  270,  382,  383,  273,  274,  275,
  382,  383,  277,  306,  384,  384,  384,  384,  304,  285,
  286,  287,  288,  289,  290,  293,  292,  293,  321,  322,
  323,  273,  293,  326,  469,  328,  384,  384,  293,  332,
  333,  307,  276,  285,  286,  287,  288,  382,  412,  342,
  384,  264,  281,  282,  283,  284,  269,  270,  414,  414,
  273,  274,  275,  469,  277,  293,  271,  469,  384,  386,
  293,  386,  285,  286,  287,  288,  289,  290,  513,  292,
  293,  293,  295,  277,  297,  298,  384,  300,  379,  277,
  303,  304,  293,  295,  307,  384,  384,  384,  311,  312,
  383,  314,  384,  264,  384,  335,  384,  513,  269,  270,
  545,  513,  273,  274,  275,  408,  382,  384,  411,  414,
  383,  269,  274,  275,  285,  286,  287,  288,  289,  290,
  423,  292,  293,  285,  286,  384,  288,  289,  290,  545,
  292,  414,  381,  545,  384,  293,  307,  382,  293,  293,
  293,  293,  300,  412,  414,  307,  304,  293,  384,  386,
  269,  383,  293,  311,  312,  378,  384,  384,  381,  382,
  383,  384,   46,  386,  387,  388,  389,  390,  391,  392,
  393,  616,  617,  383,  293,  264,  384,  384,  384,  240,
  382,  300,   35,   35,  273,  304,   40,  165,  633,  412,
  552,  414,  311,  312,  611,  498,  285,  286,  287,  288,
  616,  617,  505,  292,  616,  617,  509,  510,  151,  582,
  547,  382,  465,  549,  600,  584,  661,  633,  307,  352,
  421,  633,  667,   61,  527,  528,  384,  530,  673,  532,
  532,  534,   26,  442,  233,  269,  283,  274,  275,  276,
  534,  407,  499,  400,   39,  661,  333,  511,  289,  661,
  648,  667,  289,  290,  668,  667,  293,  673,  264,  293,
  663,  673,   -1,  269,  270,  384,  300,  273,  274,  275,
  268,  277,   -1,   -1,   -1,   -1,   -1,  311,  312,  285,
  286,  287,  288,  289,  290,   -1,  292,  293,   -1,  295,
   -1,  297,  298,   -1,  300,   -1,   -1,  303,  304,   -1,
   -1,  307,  605,   -1,  264,  311,  312,   -1,  314,  269,
  270,   -1,   -1,  273,  274,  275,   -1,  277,   -1,  622,
   -1,   -1,   -1,   -1,   -1,  285,  286,  287,  288,  289,
  290,   -1,  292,  293,   -1,  295,  264,  297,  298,   -1,
  300,   -1,   -1,  303,  304,  273,   -1,  307,   -1,   -1,
  384,  311,  312,   -1,  314,  274,  275,  285,  286,  287,
  288,   -1,  281,  282,  283,  284,   -1,   -1,   -1,   -1,
  289,  290,  378,  676,  293,  381,  382,  383,  384,  307,
  386,  387,  388,  389,  390,  391,  392,  393,  394,  395,
  396,  397,  398,  399,  400,  401,  402,  403,  404,  405,
  406,   -1,   -1,   -1,   -1,   -1,  412,   -1,  414,  398,
  399,  400,  401,  402,  403,  404,  405,  406,  378,   -1,
   -1,  381,  382,  383,  384,   -1,  386,  387,  388,  389,
  390,  391,  392,  393,  394,  395,  396,  397,  398,  399,
  400,  401,  402,  403,  404,  405,  406,   -1,   -1,   -1,
  264,   -1,  412,   -1,  414,  269,  270,   -1,   -1,  273,
  274,  275,   -1,  277,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,  285,  286,  287,  288,  289,  290,   -1,  292,  293,
   -1,  295,   -1,  297,  298,   -1,  300,   -1,   -1,  303,
  304,   -1,   -1,  307,   -1,   -1,  264,  311,  312,   -1,
  314,  269,  270,   -1,   -1,  273,  274,  275,   -1,  277,
   -1,   -1,   -1,   -1,   -1,   -1,  269,  285,  286,  287,
  288,  289,  290,   -1,  292,  293,   -1,  295,   -1,  297,
  298,   -1,  300,   -1,   -1,  303,  304,   -1,  277,  307,
  293,  274,  275,  311,  312,   -1,  314,  300,  281,  282,
  283,  284,   -1,   -1,   -1,   -1,  289,  290,  311,  312,
  293,   -1,   -1,   -1,  378,   -1,   -1,  381,  382,  383,
  384,   -1,  386,  387,  388,  389,  390,  391,  392,  393,
  394,  395,  396,  397,  398,  399,  400,  401,  402,  403,
  404,  405,   -1,   -1,   -1,   -1,   -1,  267,  412,   -1,
  414,  271,  272,  273,  274,  275,   -1,   -1,   -1,  279,
  378,   -1,   -1,  381,  382,  383,  384,   -1,  386,  387,
  388,  389,  390,  391,  392,  393,  394,  395,  396,  397,
   -1,  384,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,  412,   -1,  414,  386,  387,  388,
  389,  390,  391,  392,  393,  394,  395,  396,  397,  398,
  399,  400,  401,  402,  403,  404,  405,  406,   -1,   -1,
  340,  341,  342,  343,  344,  345,  346,  347,  348,  349,
  350,  351,  352,  353,  354,   -1,  356,  357,  358,  359,
  360,  361,  362,   -1,  364,  365,  366,  367,  368,  369,
  370,   -1,   -1,   -1,  374,   -1,  376,  267,  378,   -1,
  380,  271,  272,  273,  274,  275,   -1,   -1,   -1,  279,
   -1,   -1,   -1,  264,  394,  395,   -1,  268,  269,  270,
   -1,   -1,  273,  274,  275,   -1,  406,   -1,   -1,   -1,
   -1,   -1,   -1,  413,  285,  286,  287,  288,  289,  290,
   -1,  292,  293,   -1,  268,   -1,   -1,   -1,   -1,  273,
  274,  275,  276,   -1,   -1,   -1,  307,  281,  282,  283,
  284,  285,  286,  287,  288,  289,  290,   -1,   -1,  293,
  340,  341,  342,  343,  344,  345,  346,  347,  348,  349,
  350,  351,  352,  353,  354,   -1,  356,  357,  358,  359,
  360,  361,  362,   -1,  364,  365,  366,  367,  368,  369,
  370,   -1,   -1,   -1,  374,   -1,  376,  267,  378,   -1,
  380,  271,  272,  273,   -1,   -1,   -1,   -1,   -1,  279,
   -1,   -1,   -1,  264,  394,  395,   -1,  378,  269,  270,
   -1,   -1,  273,  274,  275,  386,  406,   -1,   -1,   -1,
   -1,   -1,   -1,  413,  285,  286,  287,  288,  289,  290,
  268,  292,  293,  269,  270,  273,  274,  275,  274,  275,
   -1,   -1,   -1,   -1,   -1,   -1,  307,  285,  286,  287,
  288,  289,  290,  289,  290,   -1,  292,  293,   -1,   -1,
  340,  341,  342,  343,  344,  345,  346,  347,  348,  349,
  350,  351,  352,  353,  354,  269,  356,  357,  358,  359,
  360,  361,  362,   -1,  364,  365,  366,  367,  368,  369,
  370,   -1,   -1,   -1,  374,   -1,  376,   -1,  378,  293,
  380,   -1,   -1,   -1,   -1,  269,  300,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,  394,  395,   -1,  311,  312,   -1,
   -1,  382,   -1,   -1,   -1,   -1,  406,  274,  275,  293,
   -1,   -1,   -1,  413,   -1,  269,  300,   -1,  285,  286,
  304,  288,  289,  290,  291,  292,   -1,  311,  312,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  293,
  307,   -1,  269,  270,   -1,   -1,  300,  274,  275,   -1,
  304,   -1,  319,  320,   -1,   -1,   -1,  311,  312,   -1,
   -1,   -1,  289,  290,   -1,  292,  293,   -1,   -1,  383,
  384,  385,  386,  387,  388,  389,  390,  391,  392,  393,
  394,  395,  396,  397,  398,  399,  400,  401,  402,  403,
  404,  405,  406,  407,   -1,   -1,   -1,  411,   -1,  413,
  384,  385,  386,  387,  388,  389,  390,  391,  392,  393,
  394,  395,  396,  397,  398,  399,  400,  401,  402,  403,
  404,  405,  406,  407,   -1,   -1,   -1,  411,   -1,  413,
  384,  385,  386,  387,  388,  389,  390,  391,  392,  393,
  394,  395,  396,  397,  398,  399,  400,  401,  402,  403,
  404,  405,  406,  407,  269,  274,  275,  411,   -1,  413,
   -1,   -1,   -1,   -1,   -1,   -1,  285,  286,   -1,  288,
  289,  290,  291,  292,   -1,   -1,   -1,   -1,  293,   -1,
   -1,   -1,   -1,   -1,   -1,  300,   -1,   -1,  307,  304,
   -1,   -1,   -1,   -1,   -1,   -1,  311,  312,   -1,   -1,
  319,  320,  264,   -1,   -1,   -1,  268,  269,  270,   -1,
   -1,  273,  274,  275,  276,   -1,   -1,   -1,   -1,  281,
  282,  283,  284,  285,  286,  287,  288,  289,  290,   -1,
  292,  293,  280,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,  291,  292,  307,  294,   -1,  296,   -1,
   -1,  299,   -1,  301,  302,   -1,   -1,  305,  306,   -1,
  308,   -1,  310,   -1,   -1,  313,   -1,   -1,   -1,  384,
   -1,  386,  387,  388,  389,  390,  391,  392,  393,  394,
  395,  396,  397,  398,  399,  400,  401,  402,  403,  404,
  405,  406,  340,  341,  342,  343,  344,  345,  346,  347,
  348,  349,  350,  351,  352,  353,  354,   -1,  356,  357,
  358,  359,  360,  361,  362,   -1,  378,   -1,   -1,  367,
   -1,  369,   -1,  280,   -1,   -1,  374,  375,  376,  377,
  378,  379,  380,   -1,  291,  292,   -1,  294,   -1,  296,
   -1,   -1,  299,   -1,  301,  302,  394,  395,  305,  306,
   -1,  308,   -1,  310,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,  409,  410,  411,   -1,  413,  394,  395,  396,  397,
  398,  399,  400,  401,  402,  403,  404,  405,  406,   -1,
   -1,   -1,   -1,  340,  341,  342,  343,  344,  345,  346,
  347,  348,  349,  350,  351,  352,  353,  354,   -1,  356,
  357,  358,  359,  360,  361,  362,   -1,   -1,   -1,   -1,
  367,   -1,  369,   -1,  280,   -1,   -1,  374,  375,  376,
  377,  378,  379,  380,   -1,  291,  292,   -1,  294,   -1,
  296,   -1,   -1,  299,   -1,  301,  302,  394,  395,  305,
  306,   -1,  308,   -1,  310,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,  409,  410,  411,   -1,  413,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,  340,  341,  342,  343,  344,  345,
  346,  347,  348,  349,  350,  351,  352,  353,  354,  280,
  356,  357,  358,  359,  360,  361,  362,   -1,   -1,   -1,
   -1,  367,   -1,  369,   -1,   -1,   -1,   -1,  374,  375,
  376,  377,  378,  379,  380,  273,  274,  275,  276,   -1,
   -1,   -1,   -1,  281,  282,  283,  284,   -1,  394,  395,
   -1,  289,  290,   -1,   -1,  293,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,  409,  410,  411,   -1,  413,   -1,  340,
  341,  342,  343,  344,  345,  346,  347,  348,  349,  350,
  351,  352,  353,  354,   -1,  356,  357,  358,  359,  360,
  361,  362,   -1,   -1,   -1,   -1,  367,   -1,  369,   -1,
   -1,   -1,   -1,  374,  375,  376,  377,  378,  379,  380,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,  264,  394,  395,   -1,  268,  269,  270,   -1,
   -1,  273,  274,  275,   -1,   -1,   -1,   -1,  409,  410,
  411,   -1,  413,  285,  286,  287,  288,  289,  290,   -1,
  292,  293,  273,  274,  275,  276,   -1,   -1,   -1,   -1,
  281,  282,  283,  284,   -1,  307,   -1,   -1,  289,  290,
   -1,   -1,  293,   -1,  273,  274,  275,  276,   -1,   -1,
   -1,   -1,  281,  282,  283,  284,   -1,   -1,   -1,   -1,
  289,  290,   -1,  314,  293,   -1,   -1,  295,   -1,   -1,
   -1,   -1,   -1,   -1,  340,  341,  342,  343,  344,  345,
  346,  347,  348,  349,  350,  314,  352,   -1,  295,   -1,
   -1,   -1,   -1,   -1,   -1,  361,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,  378,   -1,  374,  295,
  376,   -1,  378,   -1,  380,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,  378,  394,  395,
   -1,   -1,   -1,  384,   -1,  386,  297,  298,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,  413,   -1,  378,
   -1,   -1,   -1,   -1,   -1,  384,   -1,  386,  386,  387,
  388,  389,  390,  391,  392,  393,  394,  395,  396,  397,
  398,  399,  400,  401,  402,  403,  404,  405,  406,  386,
  387,  388,  389,  390,  391,  392,  393,  394,  395,  396,
  397,  398,  399,  400,  401,  402,  403,  404,  405,  406,
  386,  387,  388,  389,  390,  391,  392,  393,  394,  395,
  396,  397,  398,  399,  400,  401,  402,  403,  404,  405,
  406,  309,   -1,   -1,   -1,  386,  387,  388,  389,  390,
  391,  392,  393,  394,  395,  396,  397,  398,  399,  400,
  401,  402,  403,  404,  405,  406,  303,   -1,   -1,   -1,
   -1,   -1,  340,  341,  342,  343,  344,  345,  346,  347,
  348,  349,  350,  351,  352,  353,  354,   -1,  356,  357,
  358,  359,  360,  361,  362,   -1,   -1,   -1,   -1,  367,
   -1,  369,   -1,   -1,   -1,   -1,  374,  375,  376,  377,
  378,  379,  380,  273,  274,  275,  276,   -1,   -1,   -1,
   -1,  281,  282,  283,  284,   -1,  394,  395,   -1,  289,
  290,   -1,   -1,  293,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,  409,  410,  411,   -1,  413,   -1,   -1,   -1,  386,
  387,  388,  389,  390,  391,  392,  393,  394,  395,  396,
  397,  398,  399,  400,  401,  402,  403,  404,  405,  406,
  340,  341,  342,  343,  344,  345,  346,  347,  348,  349,
  350,  351,  352,  353,  354,   -1,  356,  357,  358,  359,
  360,  361,  362,   -1,   -1,   -1,   -1,  367,   -1,  369,
   -1,   -1,   -1,   -1,  374,  375,  376,  377,  378,  379,
  380,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  314,
  315,  316,  317,  318,  394,  395,  321,  322,  323,  324,
  325,  326,  327,  328,  329,  330,  331,   -1,   -1,  409,
  410,  411,  412,  413,  340,  341,  342,  343,  344,  345,
  346,  347,  348,  349,  350,  351,  352,  353,  354,   -1,
  356,  357,  358,  359,  360,  361,  362,   -1,   -1,   -1,
   -1,  367,   -1,  369,   -1,   -1,   -1,   -1,  374,  375,
  376,  377,  378,  379,  380,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  394,  395,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,  409,  410,  411,   -1,  413,  340,  341,
  342,  343,  344,  345,  346,  347,  348,  349,  350,  351,
  352,  353,  354,   -1,  356,  357,  358,  359,  360,  361,
  362,   -1,   -1,   -1,   -1,  367,   -1,  369,   -1,   -1,
   -1,   -1,  374,  375,  376,  377,  378,  379,  380,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,  264,  394,  395,   -1,   -1,  269,  270,   -1,   -1,
  273,  274,  275,   -1,   -1,   -1,   -1,  409,  410,  411,
   -1,  413,  285,  286,  287,  288,  289,  290,  291,  292,
  293,  340,  341,  342,  343,  344,  345,  346,  347,  348,
  349,  350,   -1,  352,  307,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,  361,   -1,   -1,   -1,  319,  320,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,  374,   -1,  376,  264,  378,
   -1,  380,  268,  269,  270,   -1,   -1,  273,  274,  275,
   -1,   -1,   -1,   -1,   -1,  394,  395,   -1,   -1,  285,
  286,  287,  288,  289,  290,  264,  292,  293,   -1,  268,
  269,  270,   -1,   -1,  273,  274,  275,   -1,   -1,   -1,
   -1,  307,   -1,   -1,   -1,   -1,  285,  286,  287,  288,
  289,  290,  264,  292,  293,   -1,  268,  269,  270,   -1,
   -1,  273,  274,  275,   -1,   -1,   -1,   -1,  307,   -1,
   -1,   -1,   -1,  285,  286,  287,  288,  289,  290,  264,
  292,  293,   -1,  268,  269,  270,   -1,   -1,  273,  274,
  275,   -1,   -1,   -1,   -1,  307,   -1,   -1,   -1,   -1,
  285,  286,  287,  288,  289,  290,  264,  292,  293,   -1,
   -1,  269,  270,   -1,   -1,  273,  274,  275,   -1,   -1,
   -1,   -1,  307,   -1,   -1,   -1,   -1,  285,  286,  287,
  288,  289,  290,  264,  292,  293,   -1,   -1,  269,  270,
   -1,   -1,  273,  274,  275,   -1,   -1,   -1,   -1,  307,
   -1,   -1,   -1,   -1,  285,  286,  287,  288,  289,  290,
  264,  292,  293,   -1,   -1,  269,  270,   -1,   -1,  273,
  274,  275,   -1,   -1,   -1,   -1,  307,   -1,   -1,   -1,
   -1,  285,  286,  287,  288,  289,  290,  264,  292,  293,
   -1,   -1,   -1,   -1,   -1,   -1,  273,  274,  275,   -1,
   -1,   -1,   -1,  307,   -1,   -1,   -1,   -1,  285,  286,
  287,  288,  289,  290,   -1,  292,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
  307,  340,  341,  342,  343,  344,  345,  346,  347,  348,
  349,  350,  351,  352,  353,  354,   -1,  356,  357,  358,
  359,  360,  361,  362,   -1,   -1,   -1,   -1,  367,   -1,
  369,   -1,   -1,   -1,   -1,  374,  375,  376,  377,   -1,
  379,  380,  340,  341,  342,  343,  344,  345,  346,  347,
  348,  349,  350,  351,  352,  353,  354,   -1,  356,  357,
  358,  359,  360,  361,  362,   -1,   -1,   -1,  366,  367,
   -1,  369,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
  378,  340,  341,  342,  343,  344,  345,  346,  347,  348,
  349,  350,  351,  352,  353,  354,   -1,  356,  357,  358,
  359,  360,  361,  362,   -1,   -1,   -1,   -1,  367,   -1,
  369,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  378,
  340,  341,  342,  343,  344,  345,  346,  347,  348,  349,
  350,   -1,  352,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,  361,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,  382,   -1,   -1,  378,  386,
  387,  388,  389,  390,  391,  392,  393,  394,  395,  396,
  397,  398,  399,  400,  401,  402,  403,  404,  405,  406,
  407,   -1,   -1,   -1,  411,   -1,  413,  414,  386,  387,
  388,  389,  390,  391,  392,  393,  394,  395,  396,  397,
  398,  399,  400,  401,  402,  403,  404,  405,  406,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,  414,  386,  387,  388,
  389,  390,  391,  392,  393,  394,  395,  396,  397,  398,
  399,  400,  401,  402,  403,  404,  405,  406,   -1,  381,
   -1,   -1,   -1,  412,  386,  387,  388,  389,  390,  391,
  392,  393,  394,  395,  396,  397,  398,  399,  400,  401,
  402,  403,  404,  405,  406,  381,   -1,   -1,   -1,   -1,
  386,  387,  388,  389,  390,  391,  392,  393,  394,  395,
  396,  397,  398,  399,  400,  401,  402,  403,  404,  405,
  406,  383,   -1,   -1,  386,  387,  388,  389,  390,  391,
  392,  393,  394,  395,  396,  397,  398,  399,  400,  401,
  402,  403,  404,  405,  406,  386,  387,  388,  389,  390,
  391,  392,  393,  394,  395,  396,  397,  398,  399,  400,
  401,  402,  403,  404,  405,  406,  386,  387,  388,  389,
  390,  391,  392,  393,  394,  395,  396,  397,  398,  399,
  400,  401,  402,  403,  404,  405,  406,  314,  315,  316,
  317,  318,  319,  320,  321,  322,  323,  324,  325,  326,
  327,  328,  329,  330,  331,
  };

#line 1332 "DelphiGrammar.y"

}	// close parser class

#line default
namespace yydebug {
        using System;
	 internal interface yyDebug {
		 void push (int state, Object value);
		 void lex (int state, int token, string name, Object value);
		 void shift (int from, int to, int errorFlag);
		 void pop (int state);
		 void discard (int state, int token, string name, Object value);
		 void reduce (int from, int to, int rule, string text, int len);
		 void shift (int from, int to);
		 void accept (Object value);
		 void error (string message);
		 void reject ();
	 }
	 
	 class yyDebugSimple : yyDebug {
		 void println (string s){
			 Console.Error.WriteLine (s);
		 }
		 
		 public void push (int state, Object value) {
			 println ("push\tstate "+state+"\tvalue "+value);
		 }
		 
		 public void lex (int state, int token, string name, Object value) {
			 println("lex\tstate "+state+"\treading "+name+"\tvalue "+value);
		 }
		 
		 public void shift (int from, int to, int errorFlag) {
			 switch (errorFlag) {
			 default:				// normally
				 println("shift\tfrom state "+from+" to "+to);
				 break;
			 case 0: case 1: case 2:		// in error recovery
				 println("shift\tfrom state "+from+" to "+to
					     +"\t"+errorFlag+" left to recover");
				 break;
			 case 3:				// normally
				 println("shift\tfrom state "+from+" to "+to+"\ton error");
				 break;
			 }
		 }
		 
		 public void pop (int state) {
			 println("pop\tstate "+state+"\ton error");
		 }
		 
		 public void discard (int state, int token, string name, Object value) {
			 println("discard\tstate "+state+"\ttoken "+name+"\tvalue "+value);
		 }
		 
		 public void reduce (int from, int to, int rule, string text, int len) {
			 println("reduce\tstate "+from+"\tuncover "+to
				     +"\trule ("+rule+") "+text);
		 }
		 
		 public void shift (int from, int to) {
			 println("goto\tfrom state "+from+" to "+to);
		 }
		 
		 public void accept (Object value) {
			 println("accept\tvalue "+value);
		 }
		 
		 public void error (string message) {
			 println("error\t"+message);
		 }
		 
		 public void reject () {
			 println("reject");
		 }
		 
	 }
}
// %token constants
 class Token {
  public const int KW_LIBRARY = 257;
  public const int KW_UNIT = 258;
  public const int KW_PROGRAM = 259;
  public const int KW_PACKAGE = 260;
  public const int KW_REQUIRES = 261;
  public const int KW_CONTAINS = 262;
  public const int KW_USES = 263;
  public const int KW_EXPORTS = 264;
  public const int KW_PLATFORM = 265;
  public const int KW_DEPRECATED = 266;
  public const int KW_INTERF = 267;
  public const int KW_IMPL = 268;
  public const int KW_FINALIZ = 269;
  public const int KW_INIT = 270;
  public const int KW_OBJECT = 271;
  public const int KW_RECORD = 272;
  public const int KW_CLASS = 273;
  public const int KW_FUNCTION = 274;
  public const int KW_PROCEDURE = 275;
  public const int KW_PROPERTY = 276;
  public const int KW_OF = 277;
  public const int KW_OUT = 278;
  public const int KW_PACKED = 279;
  public const int KW_INHERITED = 280;
  public const int KW_PROTECTED = 281;
  public const int KW_PUBLIC = 282;
  public const int KW_PUBLISHED = 283;
  public const int KW_PRIVATE = 284;
  public const int KW_CONST = 285;
  public const int KW_VAR = 286;
  public const int KW_THRVAR = 287;
  public const int KW_TYPE = 288;
  public const int KW_CONSTRUCTOR = 289;
  public const int KW_DESTRUCTOR = 290;
  public const int KW_ASM = 291;
  public const int KW_BEGIN = 292;
  public const int KW_END = 293;
  public const int KW_WITH = 294;
  public const int KW_DO = 295;
  public const int KW_FOR = 296;
  public const int KW_TO = 297;
  public const int KW_DOWNTO = 298;
  public const int KW_REPEAT = 299;
  public const int KW_UNTIL = 300;
  public const int KW_WHILE = 301;
  public const int KW_IF = 302;
  public const int KW_THEN = 303;
  public const int KW_ELSE = 304;
  public const int KW_CASE = 305;
  public const int KW_GOTO = 306;
  public const int KW_LABEL = 307;
  public const int KW_RAISE = 308;
  public const int KW_AT = 309;
  public const int KW_TRY = 310;
  public const int KW_EXCEPT = 311;
  public const int KW_FINALLY = 312;
  public const int KW_ON = 313;
  public const int KW_ABSOLUTE = 314;
  public const int KW_ABSTRACT = 315;
  public const int KW_ASSEMBLER = 316;
  public const int KW_DYNAMIC = 317;
  public const int KW_EXPORT = 318;
  public const int KW_EXTERNAL = 319;
  public const int KW_FORWARD = 320;
  public const int KW_INLINE = 321;
  public const int KW_OVERRIDE = 322;
  public const int KW_OVERLOAD = 323;
  public const int KW_REINTRODUCE = 324;
  public const int KW_VIRTUAL = 325;
  public const int KW_VARARGS = 326;
  public const int KW_PASCAL = 327;
  public const int KW_SAFECALL = 328;
  public const int KW_STDCALL = 329;
  public const int KW_CDECL = 330;
  public const int KW_REGISTER = 331;
  public const int KW_NAME = 332;
  public const int KW_READ = 333;
  public const int KW_WRITE = 334;
  public const int KW_INDEX = 335;
  public const int KW_STORED = 336;
  public const int KW_DEFAULT = 337;
  public const int KW_NODEFAULT = 338;
  public const int KW_IMPLEMENTS = 339;
  public const int TYPE_INT64 = 340;
  public const int TYPE_INT = 341;
  public const int TYPE_LONGINT = 342;
  public const int TYPE_LONGWORD = 343;
  public const int TYPE_SMALLINT = 344;
  public const int TYPE_SHORTINT = 345;
  public const int TYPE_WORD = 346;
  public const int TYPE_BYTE = 347;
  public const int TYPE_CARDINAL = 348;
  public const int TYPE_UINT64 = 349;
  public const int TYPE_CHAR = 350;
  public const int TYPE_PCHAR = 351;
  public const int TYPE_WIDECHAR = 352;
  public const int TYPE_WIDESTR = 353;
  public const int TYPE_STR = 354;
  public const int TYPE_RSCSTR = 355;
  public const int TYPE_SHORTSTR = 356;
  public const int TYPE_FLOAT = 357;
  public const int TYPE_REAL48 = 358;
  public const int TYPE_DOUBLE = 359;
  public const int TYPE_EXTENDED = 360;
  public const int TYPE_BOOL = 361;
  public const int TYPE_COMP = 362;
  public const int TYPE_CURRENCY = 363;
  public const int TYPE_OLEVAR = 364;
  public const int TYPE_VAR = 365;
  public const int TYPE_ARRAY = 366;
  public const int TYPE_CURR = 367;
  public const int TYPE_FILE = 368;
  public const int TYPE_PTR = 369;
  public const int TYPE_SET = 370;
  public const int ASM_OP = 371;
  public const int LOWESTPREC = 372;
  public const int EXPR_SINGLE = 373;
  public const int CONST_INT = 374;
  public const int CONST_REAL = 375;
  public const int CONST_CHAR = 376;
  public const int CONST_STR = 377;
  public const int IDENTIFIER = 378;
  public const int CONST_NIL = 379;
  public const int CONST_BOOL = 380;
  public const int KW_RANGE = 381;
  public const int COMMA = 382;
  public const int COLON = 383;
  public const int SEMICOL = 384;
  public const int KW_ASSIGN = 385;
  public const int KW_EQ = 386;
  public const int KW_GT = 387;
  public const int KW_LT = 388;
  public const int KW_LE = 389;
  public const int KW_GE = 390;
  public const int KW_DIFF = 391;
  public const int KW_IN = 392;
  public const int KW_IS = 393;
  public const int KW_SUM = 394;
  public const int KW_SUB = 395;
  public const int KW_OR = 396;
  public const int KW_XOR = 397;
  public const int KW_MUL = 398;
  public const int KW_DIV = 399;
  public const int KW_QUOT = 400;
  public const int KW_MOD = 401;
  public const int KW_SHL = 402;
  public const int KW_SHR = 403;
  public const int KW_AS = 404;
  public const int KW_AND = 405;
  public const int KW_DEREF = 406;
  public const int KW_DOT = 407;
  public const int UNARY = 408;
  public const int KW_NOT = 409;
  public const int KW_ADDR = 410;
  public const int LBRAC = 411;
  public const int RBRAC = 412;
  public const int LPAREN = 413;
  public const int RPAREN = 414;
  public const int MAXPREC = 415;
  public const int yyErrorCode = 256;
 }
 namespace yyParser {
  using System;
  /** thrown for irrecoverable syntax errors and stack overflow.
    */
  internal class yyException : System.Exception {
    public yyException (string message) : base (message) {
    }
  }
  internal class yyUnexpectedEof : yyException {
    public yyUnexpectedEof (string message) : base (message) {
    }
    public yyUnexpectedEof () : base ("") {
    }
  }

  /** must be implemented by a scanner object to supply input to the parser.
    */
  internal interface yyInput {
    /** move on to next token.
        @return false if positioned beyond tokens.
        @throws IOException on input error.
      */
    bool advance (); // throws java.io.IOException;
    /** classifies current token.
        Should not be called if advance() returned false.
        @return current %token or single character.
      */
    int token ();
    /** associated with current token.
        Should not be called if advance() returned false.
        @return value for token().
      */
    Object value ();
  }
 }
} // close outermost namespace, that MUST HAVE BEEN opened in the prolog
