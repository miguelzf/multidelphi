%{

using System;
using System.Collections;
using System.IO;
using System.Text;
using System.Diagnostics;

namespace crosspascal.parser
{

	// Open main Parser class
	public class DelphiParser
	{
		
		// Emulate YACC
		
		int yacc_verbose_flag = 0;
		
		void ACCEPT()
		{	// make scanner emit EOF, ends scanning and parsing
			lexer.Accept();
			yyState = yyFinal;
		}
		
		void REJECT(string msg = "")
		{	
			throw new yyParser.InputRejected(msg, lexer.yylineno());
		}
		
		
		
		// Internal helper functions
		
		string GetErrorMessage(yyParser.yyException e)
		{
			StackTrace st = new StackTrace(e, true);
			StackFrame frame = st.GetFrame(st.FrameCount-1);
			return "[ERROR] " + e.Message + " in " + Path.GetFileName(frame.GetFileName())
					+ ": line " + frame.GetFileLineNumber();
		}

		
		//	Encoding.Default);	// typically Single-Bye char set
		// TODO change charset to unicode, use %unicode in flex
		public static readonly Encoding DefaultEncoding = Encoding.GetEncoding("iso-8859-1");

		
		// Parser-Lexer communication
			
		PreProcessor preproc = new PreProcessor();
		DelphiScanner lexer;
		
		public void AddIncludePath(string path)
		{
			preproc.AddPath(path);
		}

		public void LoadIncludePaths(string fname)
		{
			string line;
			using (StreamReader file = new StreamReader(fname, DefaultEncoding))
				while((line = file.ReadLine()) != null)
				{
					string path = line.Trim();
					if (path.Length > 0 && Directory.Exists(path))
						preproc.AddPath(path);
				}
		}
		
		// Entry point and public interface
		
		internal DelphiParser(yydebug.yyDebug dgb = null)
		{
			if (dgb != null) {
				this.debug = (yydebug.yyDebug) dgb;
				yacc_verbose_flag = 1;
			}
			
			eof_token = DelphiScanner.YYEOF;
			
		}
		
		// wrapper for yyparse
		internal Object Parse(string fname, yydebug.yyDebug dgb = null)
		{
			StreamReader sr;
			try {
				sr = new StreamReader(fname, DefaultEncoding);
				// Console.WriteLine("File " + fname + " has enconding: " + sr.CurrentEncoding);
			} 
			catch (IOException ioe) {
				ErrorOutput.WriteLine("Failure to open input file: " + fname);
				return null;
			}
			
			if (dgb != null) {
				this.debug = (yydebug.yyDebug) dgb;
				yacc_verbose_flag = 1;
			}

			lexer = new DelphiScanner(sr);
			lexer.preproc = this.preproc;
			
			try {
				Object ret = yyparse(lexer);
				return ret;
			} 
			catch (yyParser.yyException yye) {
				// ErrorOutput.WriteLine(GetErrorMessage(yye));
				// only clean way to signal error. null is the default yyVal
				throw new yyParser.InputRejected(GetErrorMessage(yye));
			}
		}
		
%}


	// ==============================================================
	// Rules declarations
	// ==============================================================

%start goal
	// file type
	
	// EXEMPLOS!:
	
	// %type<String> string_const id
	// %type<Int32> intliteral 
	// %type<Statement> stmt  nonlbl_stmt



	
	// ==============================================================
	// Tokens declarations
	// ==============================================================

%token KW_LIBRARY KW_UNIT  KW_PROGRAM
	// packages
%token KW_PACKAGE KW_REQUIRES KW_CONTAINS
	// dependencies
%token KW_USES KW_EXPORTS
	// file warnings
%token KW_PLATFORM KW_DEPRECATED
	// units keywords
%token KW_INTERF KW_IMPL KW_FINALIZ KW_INIT
	// objects
%token KW_OBJECT KW_RECORD KW_CLASS
	// functions
%token KW_FUNCTION KW_PROCEDURE KW_PROPERTY
	// general
%token KW_OF KW_OUT KW_PACKED KW_INHERITED
	// scopre qualifiers
%token KW_PROTECTED KW_PUBLIC KW_PUBLISHED KW_PRIVATE
	// sec headers
%token KW_CONST KW_VAR KW_THRVAR KW_TYPE KW_CONSTRUCTOR KW_DESTRUCTOR KW_ASM
	//main_blocks
%token KW_BEGIN KW_END KW_WITH KW_DO
	// control flow loops
%token KW_FOR KW_TO KW_DOWNTO KW_REPEAT KW_UNTIL KW_WHILE
	// control flow others
%token KW_IF KW_THEN KW_ELSE KW_CASE KW_GOTO KW_LABEL
	// control flow exceptions
%token KW_RAISE KW_AT KW_TRY KW_EXCEPT KW_FINALLY KW_ON
	// function qualifiers
%token KW_ABSOLUTE KW_ABSTRACT KW_ASSEMBLER KW_DYNAMIC KW_EXPORT KW_EXTERNAL KW_FORWARD KW_INLINE KW_OVERRIDE KW_OVERLOAD KW_REINTRODUCE KW_VIRTUAL KW_VARARGS
	// function call types
%token KW_PASCAL KW_SAFECALL KW_STDCALL KW_CDECL KW_REGISTER
	// properties keywords
%token KW_NAME KW_READ KW_WRITE KW_INDEX KW_STORED KW_DEFAULT KW_NODEFAULT KW_IMPLEMENTS
	// types
%token TYPE_INT64 TYPE_INT TYPE_LONGINT TYPE_LONGWORD TYPE_SMALLINT TYPE_SHORTINT TYPE_WORD TYPE_BYTE TYPE_CARDINAL TYPE_UINT64
%token TYPE_CHAR TYPE_PCHAR TYPE_WIDECHAR TYPE_WIDESTR TYPE_STR TYPE_RSCSTR TYPE_SHORTSTR
%token TYPE_FLOAT TYPE_REAL48 TYPE_DOUBLE TYPE_EXTENDED
%token TYPE_BOOL TYPE_COMP TYPE_CURRENCY TYPE_OLEVAR TYPE_VAR TYPE_ARRAY TYPE_CURR TYPE_FILE TYPE_PTR TYPE_SET
	// pseudo, hints, added, etc
%token ASM_OP


	// ==============================================================
	// Precedence and associativity
	// ==============================================================

		// lowest precedence |
		//					 v


%nonassoc LOWESTPREC  EXPR_SINGLE

	// dangling else
%right KW_THEN 
%right KW_ELSE 

	// literais
%token CONST_INT CONST_REAL CONST_CHAR CONST_STR IDENTIFIER CONST_NIL CONST_BOOL
	// misc, separators
%nonassoc KW_RANGE COMMA COLON SEMICOL KW_ASSIGN
	// relational/comparative
%left KW_EQ KW_GT KW_LT KW_LE KW_GE KW_DIFF KW_IN KW_IS
	// additive
%left KW_SUM KW_SUB KW_OR KW_XOR
	// multiplicative
%left KW_MUL KW_DIV KW_QUOT KW_MOD KW_SHL KW_SHR KW_AS KW_AND

%left KW_DEREF KW_DOT

%left UNARY KW_NOT KW_ADDR

%nonassoc LBRAC RBRAC LPAREN RPAREN

%nonassoc MAXPREC

	//		Highest precedence ^
	// ==============================================================
	// ==============================================================

%%

goal: file KW_DOT		{	$$.val = new Node($$1.val); 
							YYACCEPT();
						}
	;

file
	: program	{ $$.val = $$1.val; }
	| package	{ $$.val = $$1.val; }
	| library	{ $$.val = $$1.val; }
	| unit		{ $$.val = $$1.val; }
	;

	/*
	portability
		: KW_PLATFORM
		| KW_DEPRECATED
		| KW_LIBRARY
		;

	port_opt
		:
		| portability
		;
	*/


	// ========================================================================
	// Top-level Sections
	// ========================================================================

program
	: KW_PROGRAM id SEMICOL	usesclauseopt main_block	{ $$.val = new ProgramNode($$2.val, $$4.val, $$5.val); }
	| 						usesclauseopt main_block	{ $$.val = new ProgramNode("untitled", $$1.val, $$2.val); }
	;

library
	: KW_LIBRARY id SEMICOL usesclauseopt main_block	{ $$.val = new LibraryNode($$2.val, $$4.val, $$5.val); }
	;

unit
	: KW_UNIT id /*port_opt*/ SEMICOL interfsec implementsec initsec	{ $$.val = new UnitNode($$2.val, $$4.val, $$5.val, $$6.val); }
	;

package
	: id id SEMICOL requiresclause containsclause KW_END	{ $$.val = new PackageNode($$2.val, $$4.val, $$5.val); }
	;

requiresclause
	: id idlist SEMICOL	{ $$.val = new UsesNode($$1.val, $$2.val);}
	;

containsclause
	: id idlist SEMICOL	{ $$.val = new UsesNode($$1.val, $$2.val);}
	;

usesclauseopt
	:							{ $$.val = null; }
	| KW_USES useidlist SEMICOL	{ $$.val = $$2.val; }
	;

useidlist
	: useid						{ $$.val = new UsesNode($$1.val, null);}
	| useidlist COMMA useid		{ $$.val = new UsesNode($$1.val, $$3.val);}
	;
	
useid
	: id						{ $$.val = new IdentifierNode($$1.val);}
	| id KW_IN string_const		{ $$.val = new IdentifierNodeWithLocation($$1.val, $$3.val);}
	;

implementsec
	: KW_IMPL usesclauseopt	maindecllist
	| KW_IMPL usesclauseopt
	;

interfsec
	: KW_INTERF usesclauseopt interfdecllist
	;

interfdecllist
	:
	| interfdecllist interfdecl
	;

initsec
	: KW_INIT stmtlist KW_END
	| KW_FINALIZ stmtlist KW_END
	| KW_INIT stmtlist KW_FINALIZ stmtlist KW_END
	| block
	| KW_END
	;

main_block
	: maindecllist	block
	|				block
	;
	
maindecllist
	: maindeclsec
	| maindecllist maindeclsec
	;

declseclist
	: funcdeclsec
	| declseclist funcdeclsec
	;

	

	// ========================================================================
	// Declaration sections
	// ========================================================================

interfdecl
	: basicdeclsec
	| staticclassopt procproto
	| thrvarsec
//	| rscstringsec
	;

maindeclsec
	: basicdeclsec
	| thrvarsec
	| exportsec
	| staticclassopt procdeclnondef
	| staticclassopt procdefinition
	| labeldeclsec
	;

funcdeclsec
	: basicdeclsec
	| labeldeclsec
	| procdefinition
	| procdeclnondef
	;

basicdeclsec
	: constsec
	| typesec
	| varsec
	;

typesec
	: KW_TYPE typedecl
	| typesec typedecl
	;

	
	// labels
	
labeldeclsec
	: KW_LABEL labelidlist SEMICOL
	;
	
labelidlist 
	: labelid
	| labelidlist COMMA labelid
	;

labelid
	: CONST_INT /* must be decimal integer in the range 0..9999 */
	| id
	;

	// Exports
		
exportsec
	: KW_EXPORTS exportsitemlist
	;

exportsitemlist
	: exportsitem
	| exportsitemlist COMMA exportsitem
	;

exportsitem
	: id
	| id KW_NAME  string_const
	| id KW_INDEX expr
	;
	



	// ========================================================================
	// Functions
	// ========================================================================

	// Prototypes/headings/signatures

	// proc decl for impl sections, needs a external/forward
procdeclnondef
	: procdefproto func_nondef_list funcdir_strict_opt
	;

procdefinition
	: procdefproto proc_define SEMICOL
	;

	// proc proto for definitions or external/forward decls
procdefproto
	: procbasickind qualid formalparams funcretopt SEMICOL funcdir_strict_opt
	;

	// check that funcrecopt is null for every kind except FUNCTION
procproto
	: procbasickind qualid formalparams funcretopt SEMICOL funcdirectopt  /*port_opt*/ 
	;

funcretopt
	:
	| COLON funcrettype 
	;

staticclassopt
	:
	| KW_CLASS
	;

procbasickind
	: KW_FUNCTION
	| KW_PROCEDURE
	| KW_CONSTRUCTOR
	| KW_DESTRUCTOR
	;

proceduretype
	: KW_PROCEDURE formalparams ofobjectopt
	| KW_FUNCTION  formalparams COLON funcrettype ofobjectopt
	;

ofobjectopt
	:
	| KW_OF KW_OBJECT
	;

	// Function blocks and parameters

proc_define
	: func_block
	| assemblerstmt
	;


func_block
	: declseclist block
	| 			  block
	;

formalparams
	: 
	| LPAREN RPAREN
	| LPAREN formalparamslist RPAREN
	;

formalparamslist
	: formalparm
	| formalparamslist SEMICOL formalparm
	;

formalparm
	: paramqualif	idlist paramtypeopt
	| 				idlist paramtypespec paraminitopt
	| KW_CONST		idlist paramtypeopt paraminitopt
	;

paramqualif
	: KW_VAR
	| KW_OUT
	;

paramtypeopt
	: 
	| paramtypespec
	;

paramtypespec
	: COLON funcparamtype
	;

paraminitopt
	: 
	| KW_EQ expr	// evaluates to constant
	;

	
	// Function directives
	
funcdirectopt
	:
	| funcdirective_list SEMICOL
	;

funcdirectopt_nonterm
	:
	| funcdirective_list 
	;

funcdir_strict_opt
	: 
	| funcdir_strict_list
	;

funcdirective_list
	: funcdirective 
	| funcdirective_list SEMICOL funcdirective
	;

funcdir_strict_list
	: funcdir_strict SEMICOL
	| funcdir_strict_list funcdir_strict SEMICOL
	;

func_nondef_list
	: funcdir_nondef SEMICOL
	| func_nondef_list funcdir_nondef SEMICOL 
	;

funcdirective
	: funcdir_strict
	| funcdir_nondef
	;

funcdir_strict
	: funcqualif
	| funccallconv
	;

funcdir_nondef
	: KW_EXTERNAL string_const externarg
	| KW_EXTERNAL qualid externarg
	| KW_EXTERNAL
	| KW_FORWARD
	;

externarg
	: 
	| id string_const 	// id == NAME
	| id qualid		 	// id == NAME
	;

funcqualif
	: KW_ABSOLUTE
	| KW_ABSTRACT
	| KW_ASSEMBLER
	| KW_DYNAMIC
	| KW_EXPORT
	| KW_INLINE
	| KW_OVERRIDE
	| KW_OVERLOAD
	| KW_REINTRODUCE
	| KW_VIRTUAL
	| KW_VARARGS
	;

funccallconv
	: KW_PASCAL
	| KW_SAFECALL
	| KW_STDCALL
	| KW_CDECL
	| KW_REGISTER
	;



	
	// ========================================================================
	// Statements
	// ========================================================================
	
block
	: KW_BEGIN stmtlist KW_END
	;

stmtlist
	: stmt SEMICOL stmtlist
	| stmt
	;

stmt
	: nonlbl_stmt
	| labelid COLON nonlbl_stmt
	;

nonlbl_stmt
	:
	| inheritstmts
	| goto_stmt
	| block
	| ifstmt
	| casestmt
	| repeatstmt
	| whilestmt
	| forstmt
	| with_stmt
	| tryexceptstmt		// purely Delphi stuff!
	| tryfinallystmt
	| raisestmt
	| assemblerstmt
	;

inheritstmts
	: KW_INHERITED
	| KW_INHERITED assign
	| KW_INHERITED proccall
	| proccall
	| assign
	;

assign
	: lvalue KW_ASSIGN expr
	| lvalue KW_ASSIGN KW_INHERITED expr
	;


goto_stmt
	: KW_GOTO labelid
	;

ifstmt
	: KW_IF expr KW_THEN nonlbl_stmt KW_ELSE nonlbl_stmt
	| KW_IF expr KW_THEN nonlbl_stmt
	;

casestmt
	: KW_CASE expr KW_OF caseselectorlist else_case KW_END
	;

else_case
	:
	| KW_ELSE nonlbl_stmt
	| KW_ELSE nonlbl_stmt SEMICOL
	;

caseselectorlist
	: caseselector
	| caseselectorlist SEMICOL caseselector
	;

caseselector
	:
	| caselabellist COLON nonlbl_stmt
	;

caselabellist
	: caselabel
	| caselabellist COMMA caselabel
	;

	// all exprs must be evaluate to const
caselabel
	: expr
	| expr KW_RANGE expr
	;

repeatstmt
	: KW_REPEAT stmtlist KW_UNTIL expr
	;

whilestmt
	: KW_WHILE expr KW_DO nonlbl_stmt
	;

forstmt
	: KW_FOR id KW_ASSIGN expr KW_TO	 expr KW_DO nonlbl_stmt
	| KW_FOR id KW_ASSIGN expr KW_DOWNTO expr KW_DO nonlbl_stmt
	;

	// expr must yield a ref to a record, object, class, interface or class type
with_stmt
	: KW_WITH exprlist KW_DO nonlbl_stmt
	;

tryexceptstmt
	: KW_TRY stmtlist KW_EXCEPT exceptionblock KW_END
	;

exceptionblock
	: onlist KW_ELSE stmtlist
	| onlist
	| stmtlist
	;

onlist
	: ondef
	| onlist ondef
	;

ondef
	: KW_ON id COLON id KW_DO nonlbl_stmt SEMICOL
	| KW_ON 		 id KW_DO nonlbl_stmt SEMICOL
	;

tryfinallystmt
	: KW_TRY  stmtlist KW_FINALLY stmtlist KW_END
	;

raisestmt
	: KW_RAISE
	| KW_RAISE lvalue
	| KW_RAISE			KW_AT expr
	| KW_RAISE lvalue	KW_AT expr
	;

assemblerstmt
	: KW_ASM asmcode KW_END		// not supported
	;

asmcode
	: ASM_OP
	| asmcode ASM_OP
	;





	// ========================================================================
	// Variables and Expressions
	// ========================================================================

varsec
	: KW_VAR vardecllist
	;
	
thrvarsec
	: KW_THRVAR vardecllist
	;

vardecllist
	: vardecl
	| vardecllist vardecl
	;

	/* VarDecl
		On Windows -> idlist ':' Type [(ABSOLUTE (id | ConstExpr))	| '=' ConstExpr] [portability]
		On Linux   -> idlist ':' Type [ ABSOLUTE (Ident)			| '=' ConstExpr] [portability]
	*/

vardecl
	: idlist COLON vartype vardeclopt SEMICOL
	| idlist COLON proceduretype SEMICOL funcdirectopt
	| idlist COLON proceduretype SEMICOL funcdirectopt_nonterm KW_EQ CONST_NIL SEMICOL
	;

vardeclopt
	: /*port_opt*/
	| KW_ABSOLUTE id  /*port_opt*/
	| KW_EQ constexpr /*portabilityonapt*/
	;

	// func call, type cast or identifier
proccall
	: id
	| lvalue KW_DOT id						// field access
	| lvalue LPAREN exprlistopt RPAREN
	| lvalue LPAREN casttype RPAREN			// for funcs like High, Low, Sizeof etc
	;

lvalue
	: proccall						// proc_call or cast
//	| KW_INHERITED proccall		// TODO
	| expr KW_DEREF 						// pointer deref
	| lvalue LBRAC exprlist RBRAC			// array access
	| string_const LBRAC exprlist RBRAC		// string access
	| casttype LPAREN exprlistopt RPAREN	// cast with pre-defined type

//	| LPAREN lvalue RPAREN		// TODO
	;

expr
	: literal							{ $$.val = $$1.val; }
	| lvalue							{ $$.val = new LValueNode($$1.val); }
	| setconstructor
	| KW_ADDR expr
	| KW_NOT expr
	| sign	 expr %prec UNARY
	| LPAREN expr RPAREN
	| expr relop expr %prec KW_EQ
	| expr addop expr %prec KW_SUB
	| expr mulop expr %prec KW_MUL
	;

sign
	: KW_SUB		{ $$.val = new OperatorNode("-");}
	| KW_SUM		{ $$.val = new OperatorNode("+");}
	;
mulop
	: KW_MUL		{ $$.val = new OperatorNode("*");}
	| KW_DIV		{ $$.val = new OperatorNode("/");}
	| KW_QUOT		{ $$.val = new OperatorNode("div");}
	| KW_MOD		{ $$.val = new OperatorNode("mod");}
	| KW_SHR		{ $$.val = new OperatorNode("shr");}
	| KW_SHL		{ $$.val = new OperatorNode("shl");}
	| KW_AND		{ $$.val = new OperatorNode("and");}
	;
addop
	: KW_SUB	{ $$.val = new OperatorNode("-");}
	| KW_SUM	{ $$.val = new OperatorNode("+");}
	| KW_OR		{ $$.val = new OperatorNode("or");}
	| KW_XOR	{ $$.val = new OperatorNode("xor");}
	;
relop
	: KW_EQ		{ $$.val = new OperatorNode("=");}
	| KW_DIFF	{ $$.val = new OperatorNode("<>");}
	| KW_LT		{ $$.val = new OperatorNode("<");}
	| KW_LE		{ $$.val = new OperatorNode("<=");}
	| KW_GT		{ $$.val = new OperatorNode(">");}
	| KW_GE		{ $$.val = new OperatorNode(">=");}
	| KW_IN		{ $$.val = new OperatorNode("in");}
	| KW_IS		{ $$.val = new OperatorNode("is");}
	| KW_AS		{ $$.val = new OperatorNode("as");}
	;

literal
	: CONST_INT		{ $$.val = new IntegerLiteralNode($$1.val);}
	| CONST_BOOL	{ $$.val = new BoolLiteralNode($$1.val);}
	| CONST_REAL	{ $$.val = new RealLiteralNode($$1.val);}
	| CONST_NIL		{ $$.val = new NilLiteralNode($$1.val);}
	| string_const	{ $$.val = new StringLiteralNode($$1.val);}
	;

discrete
	: CONST_INT
	| CONST_CHAR
	| CONST_BOOL
	;

string_const
	: CONST_STR
	| CONST_CHAR 
	| string_const CONST_STR
	| string_const CONST_CHAR
	;

id	: IDENTIFIER
	;

idlist
	: id
	| idlist COMMA id
	;

qualid
	: id
	| qualid KW_DOT id
	;
	
exprlist
	: expr
	| exprlist COMMA expr
	;

exprlistopt
	:
	| exprlist
	;





	// ========================================================================
	// Sets and Enums literals
	// ========================================================================

	/* examples:
		SmallNums : Set of 0..55;		// Set of the first 56 set members
		SmallNums := [3..12,23,30..32];	// Set only some of the members on
		
		type TDay = (Mon=1, Tue, Wed, Thu, Fri, Sat, Sun);	// Enumeration values
		TWeekDays = Mon..Fri;	// Enumeration subranges
		TWeekend  = Sat..Sun;

		[red, green, MyColor]
		[1, 5, 10..K mod 12, 23]
		['A'..'Z', 'a'..'z', Chr(Digit + 48)]
	*/

rangetype
	: sign rangestart KW_RANGE expr
	| rangestart KW_RANGE expr
	;

rangestart
	: discrete
	| qualid
	| id LPAREN casttype RPAREN
	| id LPAREN literal RPAREN
	;

enumtype
	: LPAREN enumtypeellist RPAREN
	;

enumtypeellist
	: enumtypeel
	| enumtypeellist COMMA enumtypeel
	;

enumtypeel
	: id 
	| id KW_EQ expr
	;

setconstructor
	: LBRAC	RBRAC
	| LBRAC setlist	RBRAC
	;

setlist
	: setelem
	| setlist COMMA setelem
	;
	
setelem
	: expr
	| expr KW_RANGE expr
	;




	
	// ========================================================================
	// Constants
	// ========================================================================

	/*
	// Resource strings, windows-only

	rscstringsec
		: TYPE_RSCSTR  constrscdecllist
		;

	constrscdecllist
		: constrscdecl
		| constrscdecllist constrscdecl
		;

	constrscdecl
		: id KW_EQ literal SEMICOL
		;	
	
	// --------------------------------
	*/
	
constsec
	: KW_CONST constdecl
	| constsec constdecl
	;

constdecl
	: id KW_EQ constexpr /*port_opt*/ SEMICOL			// true const
	| id COLON vartype KW_EQ constexpr /*port_opt*/ SEMICOL		// typed const
	;

constexpr
	: expr
	| arrayconst
	| recordconst
	;

	// 1 or more exprs
arrayconst
	: LPAREN constexpr COMMA constexprlist RPAREN
	;

constexprlist
	: constexpr
	| constexprlist COMMA constexpr
	;

recordconst
	: LPAREN fieldconstlist RPAREN
	| LPAREN fieldconstlist SEMICOL RPAREN
	;

fieldconstlist
	: fieldconst
	| fieldconstlist SEMICOL fieldconst
	;

fieldconst
	: id COLON constexpr 
	;





	// ========================================================================
	// Composite Types
	// ========================================================================
	
	// Records and objects are treated as classes
	
classtype
	: class_keyword heritage class_struct_opt KW_END
	| class_keyword heritage		// forward decl
	;

class_keyword
	: KW_CLASS
	| KW_OBJECT
	| KW_RECORD
	;

heritage
	:
	| LPAREN idlist RPAREN		// inheritance from class and interf(s) 
	;

class_struct_opt
	: fieldlist complist scopeseclist
	;

scopeseclist
	: 
	| scopeseclist scopesec
	;

scopesec
	: scope_decl fieldlist complist
	;

scope_decl
	: KW_PUBLISHED
	| KW_PUBLIC
	| KW_PROTECTED
	| KW_PRIVATE
	;

fieldlist
	: 
	| fieldlist objfield
	;
		
complist
	:
	| complist class_comp
	;

objfield
	: idlist COLON type SEMICOL
	;
	
class_comp
	: staticclassopt procproto
	| property
	;

interftype
	: KW_INTERF heritage classmethodlistopt classproplistopt KW_END
	;

classmethodlistopt
	: methodlist
	|
	;

methodlist
	: procproto
	| methodlist procproto
	;


	
	
	
	// ========================================================================
	// Properties
	// ========================================================================
	
	
classproplistopt
	: classproplist
	|
	;

classproplist
	: property
	| classproplist property
	;

property
	: KW_PROPERTY id SEMICOL
	| KW_PROPERTY id propinterfopt COLON funcrettype indexspecopt propspecifiers SEMICOL defaultdiropt
	;

defaultdiropt
	:
	| id SEMICOL	// id == DEFAULT
	;

propinterfopt
	:
	| LBRAC idlisttypeidlist RBRAC
	;
	
idlisttypeidlist
	: idlisttypeid
	| idlisttypeidlist SEMICOL idlisttypeid
	;

idlisttypeid
	: idlist COLON funcparamtype
	| KW_CONST idlist COLON funcparamtype
	;

indexspecopt
	:
	| KW_INDEX expr
	;


	// Ugly, but the only way...
	// 1st-id: specifier - default, implements, read, write, stored
	// 2nd-id: argument
propspecifiers
	:
	| id id 
	| id id id id
	| id id id id id id
	| id id id id id id id id
	| id id id id id id id id id id 
	;


/*	// Properties directive: emitted as ids since they are not real keywords

	propspecifiers
		: indexspecopt readacessoropt writeacessoropt storedspecopt defaultspecopt implementsspecopt
		;

	storedspecopt
		:
		| KW_STORED id
		;

	defaultspecopt
		:
		| KW_DEFAULT literal
	//	| KW_NODEFAULT	not supported by now
		;

	implementsspecopt
		:
		| KW_IMPLEMENTS id
		;

	readacessoropt
		:
		| KW_READ	id
		;

	writeacessoropt
		:
		| KW_WRITE	id
		;
	*/

	
	
	// ========================================================================
	// Types
	// ========================================================================

typedecl
	: id KW_EQ typeopt vartype /*port_opt*/ SEMICOL
	| id KW_EQ typeopt proceduretype /*port_opt*/ SEMICOL funcdirectopt 
	;

typeopt
	:
	| KW_TYPE
	;

type
	: vartype
	| proceduretype
	;

vartype
	: simpletype
	| enumtype
	| rangetype
	| varianttype
	| refpointertype
	// metaclasse
	| classreftype
	// object definition
	| KW_PACKED  packedtype
	| packedtype
	;

packedtype
	: structype
	| restrictedtype
	;

classreftype
	: KW_CLASS KW_OF scalartype
	;

simpletype
	: scalartype
	| realtype
	| stringtype
	| TYPE_PTR
	;

ordinaltype
	: enumtype
	| rangetype
	| scalartype
	;

scalartype
	: inttype
	| chartype
	| qualid		// user-defined type
	;

realtype
	: TYPE_REAL48
	| TYPE_FLOAT
	| TYPE_DOUBLE
	| TYPE_EXTENDED
	| TYPE_CURR
	| TYPE_COMP
	;

inttype
	: TYPE_BYTE
	| TYPE_BOOL
	| TYPE_INT
	| TYPE_SHORTINT
	| TYPE_SMALLINT
	| TYPE_LONGINT
	| TYPE_INT64
	| TYPE_UINT64
	| TYPE_WORD
	| TYPE_LONGWORD
	| TYPE_CARDINAL
	;

chartype
	: TYPE_CHAR
	| TYPE_WIDECHAR
	;

stringtype
	: TYPE_STR		// dynamic size
	| TYPE_PCHAR
	| TYPE_STR LBRAC expr RBRAC
	| TYPE_SHORTSTR
	| TYPE_WIDESTR
	;

varianttype
	: TYPE_VAR
	| TYPE_OLEVAR
	;

structype
	: arraytype
	| settype
	| filetype
	;
	
restrictedtype
	: classtype
	| interftype
	;

arraysizelist
	: rangetype
	| arraysizelist COMMA rangetype
	;

arraytype
	: TYPE_ARRAY LBRAC arraytypedef RBRAC KW_OF type /*port_opt*/
	| TYPE_ARRAY KW_OF type /*port_opt*/
	;

arraytypedef
	: arraysizelist
	| inttype
	| chartype
	| qualid
	;

settype
	: TYPE_SET KW_OF ordinaltype /*port_opt*/
	;

filetype
	: TYPE_FILE KW_OF type /*port_opt*/
	| TYPE_FILE
	;

refpointertype
	: KW_DEREF type /*port_opt*/
	;

funcparamtype
	: simpletype
	| TYPE_ARRAY KW_OF simpletype /*port_opt*/
	;

funcrettype
	: simpletype
	;
	
	// simpletype w/o user-defined types
casttype
	: inttype
	| chartype
	| realtype
	| stringtype
	| TYPE_PTR
	;


%%

	}	// close parser class, opened in prolog
	
	
	namespace yydebug
	{
		// Internal for Debug prints
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
	
	namespace yyParser
	{
		internal class InputRejected : yyException 
		{
			public InputRejected (string message = "Input invalid - Parsing terminated by REJECT action",
									int lineno = -1) 
				: base ("Line " + ((lineno >= 0)? lineno+"" : "unknown") + ": " + message)  { }
		}
	}

// already defined in template
//	} // close outermost namespace
