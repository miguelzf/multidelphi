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
			throw new yyParser.InputRejected(lexer.yylineno(), msg);
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


%type<bool>  staticclassopt ofobjectopt
%type<ProgramNode> program
%type<LibraryNode> library
%type<UnitNode> unit
%type<PackageNode> package
%type<UsesNode> requiresclause containsclause usesclauseopt useidlist
%type<IdentifierNode> id useid externarg qualid
%type<UnitImplementationNode> implementsec
%type<UnitInterfaceNode> interfsec
%type<DeclarationListNode> interfdecllist maindecllist
%type<UnitInitialization> initsec
%type<BlockWithDeclarationsNode> main_block
%type<DeclarationNode> interfdecl maindeclsec funcdeclsec basicdeclsec typesec labeldeclsec labelidlist  varsec thrvarsec vardecllist vardecl constdecl typedecl
%type<LabelNode> labelid
%type<ExportItem> exportsec	 exportsitemlist exportsitem
%type<ProcedureDefinitionNode> procdefinition
%type<ProcedureHeaderNode> procdefproto procproto proceduretype
%type<ProcedureBodyNode> proc_define func_block
%type<TypeNode> funcrettype simpletype funcretopt funcparamtype paramtypeopt paramtypespec
%type<FunctionClass> procbasickind
%type<ParameterNodeList> formalparams formalparamslist
%type<ParamterNode> formalparm
%type<VarParameterQualifier> paramqualif
%type<Expression> paraminitopt expr rangetype  rangestart constexpr
%type<ProcedureDirectiveList> funcdirectopt funcdirectopt_nonterm funcdir_strict_opt funcdirective_list funcdir_strict_list func_nondef_list									
%type<ProcedureDirective> funcdirective funcdir_strict funcdir_nondef funcqualif
%type<CallConventionNode> funccallconv
%type<StatementBlock> block stmtlist
%type<Statement> stmt nonlbl_stmt inheritstmts assign goto_stmt ifstmt casestmt else_case repeatstmt whilestmt forstmt with_stmt tryexceptstmt tryfinallystmt raisestmt assemblerstmt asmcode
%type<CaseSelectorList> caseselectorlist
%type<CaseSelector> caseselector
%type<CaseLabelList> caselabellist
%type<CaseLabel> caselabel
%type<ExceptionBlockNode> exceptionblock
%type<OnListNode> onlist
%type<OnStatement> ondef
%type<VarDeclarationOption> vardeclopt
%type<ProcedureCallNode> proccall
%type<LValue> lvalue
%type<OperatorNode> sign mulop addop relop
%type<LiteralNode> literal discrete string_const
%type<IdentifierListNode> idlist heritage
%type<ExpressionListNode> exprlist exprlistopt
%type<EmumList> enumtype enumtypeellist
%type<FieldInit> enumtypeel fieldconst
%type<SetList> setconstructor setlist
%type<SetElement> setelem
%type<Literal> rscstringsec constrscdecllist constrscdecl
%type<ConstDeclarationList> constsec
%type<ExpressionListNode> arrayconst constexprlist
%type<FieldInitList>  recordconst fieldconstlist
%type<ClassDefinition> classtype
%type<ClassType> class_keyword
%type<ClassStruct> class_struct_opt scopesec
%type<ClassContentList> scopeseclist complist classmethodlistopt methodlist classproplistopt classproplist
%type<ClassContent> class_comp
%type<Scope> scope_decl
%type<ClassFieldList> fieldlist
%type<VarDeclarationNode> objfield
%type<InterfaceDefinition> interftype
%type<ClassProperty> property
%type<bool> typeopt
%type<TypeNode> type vartype packedtype classreftype simpletype ordinaltype
%type<TypeNode> scalartype realtype inttype chartype stringtype varianttype funcrettype
%type<TypeNode> structype restrictedtype arraytype settype filetype refpointertype funcparamtype 

/*!!missing:
declseclist
procdeclnondef
defaultdiropt
propinterfopt
idlisttypeidlist
idlisttypeid
indexspecopt
propspecifiers
arraysizelist
arraytypedef
casttype
*/
		

	
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
	: program	{ $$ = $1; }
	| package	{ $$ = $1; }
	| library	{ $$ = $1; }
	| unit		{ $$ = $1; }
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
	: KW_PROGRAM id SEMICOL	usesclauseopt main_block	{ $$ = new ProgramNode($2, $4, $5); }
	| 						usesclauseopt main_block	{ $$ = new ProgramNode("untitled", $1, $2); }
	;

library
	: KW_LIBRARY id SEMICOL usesclauseopt main_block	{ $$ = new LibraryNode($2, $4, $5); }
	;

unit
	: KW_UNIT id /*port_opt*/ SEMICOL interfsec implementsec initsec	{ $$ = new UnitNode($2, $4, $5, $6); }
	;

package
	: id id SEMICOL requiresclause containsclause KW_END	{ $$ = new PackageNode($2, $4, $5); }
	;

requiresclause
	: id idlist SEMICOL	{ $$ = new UsesNode($1, $2);}
	;

containsclause
	: id idlist SEMICOL	{ $$ = new UsesNode($1, $2);}
	;

usesclauseopt
	:							{ $$ = null; }
	| KW_USES useidlist SEMICOL	{ $$ = $2; }
	;

useidlist
	: useid						{ $$ = new UsesNode($1, null);}
	| useidlist COMMA useid		{ $$ = new UsesNode($1, $3);}
	;
	
useid
	: id						{ $$ = new IdentifierNode($1);}
	| id KW_IN string_const		{ $$ = new IdentifierNodeWithLocation($1, $3);}
	;

implementsec
	: KW_IMPL usesclauseopt	maindecllist	{ $$ = new UnitImplementationNode($2, $3);}
	| KW_IMPL usesclauseopt					{ $$ = new UnitImplementationNode($2, null);}
	;

interfsec
	: KW_INTERF usesclauseopt interfdecllist	{ $$ = new UnitInterfaceNode($2, $3);}
	;

interfdecllist
	:								{ $$ = null;}
	| interfdecllist interfdecl		{ $$ = new DeclarationListNode($2, $1);}
	;

initsec
	: KW_INIT stmtlist KW_END							{ $$ = new UnitInitialization($2, null);}
	| KW_FINALIZ stmtlist KW_END						{ $$ = new UnitInitialization(null, $2);}
	| KW_INIT stmtlist KW_FINALIZ stmtlist KW_END		{ $$ = new UnitInitialization($2, $4);}
	| block												{ $$ = new UnitInitialization($1, null);}
	| KW_END											{ $$ = null;}
	;

main_block
	: maindecllist	block	{ $$ = new BlockWithDeclarationsNode($1, $2);}
	|				block	{ $$ = new BlockWithDeclarationsNode(null, $1);}
	;
	
maindecllist
	: maindeclsec				{ $$ = new DeclarationListNode($1, null);}
	| maindecllist maindeclsec	{ $$ = new DeclarationListNode($2, $1);}
	;

declseclist
	: funcdeclsec				{ $$ = new UnfinishedNode($1);}
	| declseclist funcdeclsec	{ $$ = new UnfinishedNode($2);}
	;

	

	// ========================================================================
	// Declaration sections
	// ========================================================================

interfdecl
	: basicdeclsec					{ $$ = $1;}
	| staticclassopt procproto		{ $2.isStatic = $1; $$ = $2;}
	| thrvarsec						{ $$ = $1;}
//	| rscstringsec				
	;

maindeclsec
	: basicdeclsec					{ $$ = $1;}
	| thrvarsec						{ $$ = $1;}
	| exportsec						{ $$ = $1;}
	| staticclassopt procdeclnondef	{ $2.isStatic = $1; $$ = $2;}
	| staticclassopt procdefinition	{ $2.isStatic = $1; $$ = $2;}
	| labeldeclsec					{ $$ = $1;}
	;

funcdeclsec
	: basicdeclsec			{ $$ = $1;}
	| labeldeclsec			{ $$ = $1;}
	| procdefinition		{ $$ = $1;}
	| procdeclnondef		{ $$ = $1;}
	;

basicdeclsec
	: constsec		{ $$ = $1;}
	| typesec		{ $$ = $1;}
	| varsec		{ $$ = $1;}
	;

typesec
	: KW_TYPE typedecl		{ $$ = $2; }
	| typesec typedecl		{ $$ = new TypeDeclarationListNode($2, $1); }
	;

	
	// labels
	
labeldeclsec
	: KW_LABEL labelidlist SEMICOL	{$$ = $2; }
	;
	
labelidlist 
	: labelid						{ $$ = new LabelDeclarationNode($1, null); }
	| labelidlist COMMA labelid		{ $$ = new LabelDeclarationNode($3, $1); }
	;

labelid
	: CONST_INT /* must be decimal integer in the range 0..9999 */	{ $$ = new NumberLabelNode($1); }
	| id															{ $$ = new StringLabelNode($1); }
	;

	// Exports
		
exportsec	
	: KW_EXPORTS exportsitemlist		{ $$ = $1; }
	;

exportsitemlist
	: exportsitem							{ $$ = $1; }
	| exportsitemlist COMMA exportsitem		{ $$ = $1; }
	;

exportsitem
	: id							{ $$ = new ExportItem($1, null, null); }
	| id KW_NAME  string_const		{ $$ = new ExportItem($1, $3, null); }
	| id KW_INDEX expr				{ $$ = new ExportItem($1, null, $3); }
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
	: procdefproto proc_define SEMICOL	{ $$ = new ProcedureDefinitionNode($1, $2); } 
	;

	// proc proto for definitions or external/forward decls
procdefproto
	: procbasickind qualid formalparams funcretopt SEMICOL funcdir_strict_opt	{ $$ = new ProcedureHeaderNode($1, $2, $3, $4, null, $6); }
	;

	// check that funcrecopt is null for every kind except FUNCTION
procproto
	: procbasickind qualid formalparams funcretopt SEMICOL funcdirectopt  /*port_opt*/	{ $$ = new ProcedureHeaderNode($1, $2, $3, $4, null, $6); }
	;

funcretopt
	:						{ $$ = null;}
	| COLON funcrettype		{ $$ = $2;}
	;

staticclassopt
	:							{ $$ = false;}
	| KW_CLASS					{ $$ = true;}
	;

procbasickind
	: KW_FUNCTION				{ $$ = FunctionClass.Function;}
	| KW_PROCEDURE				{ $$ = FunctionClass.Procedure;}
	| KW_CONSTRUCTOR			{ $$ = FunctionClass.Constructor;}
	| KW_DESTRUCTOR				{ $$ = FunctionClass.Destructor;}
	;

proceduretype
	: KW_PROCEDURE formalparams ofobjectopt		{ $$ = new ProcedureHeaderNode(FunctionKind.Procedure, null, $2, null, $3, null); }
	| KW_FUNCTION  formalparams COLON funcrettype ofobjectopt { $$ = new ProcedureHeaderNode(FunctionKind.Function, null, $2, $4, $5, null); }
	;

ofobjectopt
	:						{ return false; }
	| KW_OF KW_OBJECT		{ return true; }
	;

	// Function blocks and parameters

proc_define
	: func_block			{ $$ = $1; }
	| assemblerstmt			{ $$ = new AssemblerProcedureBodyNode($1); }
	;


func_block
	: declseclist block			{ $$ = new ProcedureBodyNode($1, $2); }
	| 			  block			{ $$ = new ProcedureBodyNode(null, $2); }
	;

formalparams
	:										{ $$ = null; }
	| LPAREN RPAREN							{ $$ = null; }
	| LPAREN formalparamslist RPAREN		{ $$ = $2; }
	;

formalparamslist
	: formalparm							{ $$ = new ParameterNodeList($1, null); }
	| formalparamslist SEMICOL formalparm	{ $$ = new ParameterNodeList($3, $1); }
	;

formalparm
	: paramqualif	idlist paramtypeopt					{ $$ = new ParameterNode($1, $2, $3, null); } 
	| 				idlist paramtypespec paraminitopt	{ $$ = new ParameterNode(null, $1, $2, $3); }
	| KW_CONST		idlist paramtypeopt paraminitopt	{ $$ = new ParameterNode(new ConstParameterQualifier(), $1, $2, $3); }
	;

paramqualif
	: KW_VAR		{ $$ = new VarParameterQualifier();}
	| KW_OUT		{ $$ = new OutParameterQualifier();}
	;

paramtypeopt
	:					{ $$ = null; }
	| paramtypespec		{ $$ = $1; }
	;

paramtypespec
	: COLON funcparamtype	{ $$ = $2; }
	;

paraminitopt
	:											{ $$ = null; }
	| KW_EQ expr	// evaluates to constant	{ $$ = $2; }
	;

	
	// Function directives
	
funcdirectopt
	:										{ $$ = null; }
	| funcdirective_list SEMICOL			{ $$ = $1; }
	;

funcdirectopt_nonterm
	:							{ $$ = null; }
	| funcdirective_list		{ $$ = $1; }
	;

funcdir_strict_opt
	:							{ $$ = null; }
	| funcdir_strict_list		{ $$ = $1; }
	;

funcdirective_list
	: funcdirective									{ $$ = new ProcedureDirectiveList($1, null); }
	| funcdirective_list SEMICOL funcdirective		{ $$ = new ProcedureDirectiveList($3, $1); }
	;

funcdir_strict_list
	: funcdir_strict SEMICOL						{ $$ = new ProcedureDirectiveList($1, null); }
	| funcdir_strict_list funcdir_strict SEMICOL	{ $$ = new ProcedureDirectiveList($2, $1); }
	;

func_nondef_list									
	: funcdir_nondef SEMICOL						{ $$ = new ProcedureDirectiveList($1, null); }
	| func_nondef_list funcdir_nondef SEMICOL		{ $$ = new ProcedureDirectiveList($2, $1); }
	;

funcdirective
	: funcdir_strict	{ $$ = $1; }
	| funcdir_nondef	{ $$ = $1; }
	;

funcdir_strict
	: funcqualif		{ $$ = $1; }
	| funccallconv		{ $$ = $1; }
	;

funcdir_nondef
	: KW_EXTERNAL string_const externarg		{ $$ = new ExternalProcedureDirective(new IdentifierNode($2), $3); }
	| KW_EXTERNAL qualid externarg				{ $$ = new ExternalProcedureDirective($2, $3); }
	| KW_EXTERNAL								{ $$ = new ExternalProcedureDirective(null, null); }
	| KW_FORWARD								{ $$ = new ProcedureDirective(ProcedureDirectiveEnum.Forward); }
	;

externarg
	:					{$$ = null; }
	| id string_const 	{$$ = new IdentifierNode($2);}	// id == NAME		
	| id qualid		 	{$$ = $2;}  // id == NAME		
	;

funcqualif
	: KW_ABSOLUTE			{ $$ = new ProcedureDirective(ProcedureDirectiveEnum.Absolute); }
	| KW_ABSTRACT			{ $$ = new ProcedureDirective(ProcedureDirectiveEnum.Abstract); }
	| KW_ASSEMBLER			{ $$ = new ProcedureDirective(ProcedureDirectiveEnum.Assembler); }
	| KW_DYNAMIC			{ $$ = new ProcedureDirective(ProcedureDirectiveEnum.Dynamic); }
	| KW_EXPORT				{ $$ = new ProcedureDirective(ProcedureDirectiveEnum.Export); }
	| KW_INLINE				{ $$ = new ProcedureDirective(ProcedureDirectiveEnum.Inline); }
	| KW_OVERRIDE			{ $$ = new ProcedureDirective(ProcedureDirectiveEnum.Override); }
	| KW_OVERLOAD			{ $$ = new ProcedureDirective(ProcedureDirectiveEnum.Overload); }
	| KW_REINTRODUCE		{ $$ = new ProcedureDirective(ProcedureDirectiveEnum.Reintroduce); }
	| KW_VIRTUAL			{ $$ = new ProcedureDirective(ProcedureDirectiveEnum.Virtual); }
	| KW_VARARGS			{ $$ = new ProcedureDirective(ProcedureDirectiveEnum.VarArgs); }
	;

funccallconv
	: KW_PASCAL			{ $$ = new CallConventionNode(CallConvention.Pascal); }
	| KW_SAFECALL		{ $$ = new CallConventionNode(CallConvention.SafeCall); }
	| KW_STDCALL		{ $$ = new CallConventionNode(CallConvention.StdCall); }
	| KW_CDECL			{ $$ = new CallConventionNode(CallConvention.CDecl); }
	| KW_REGISTER		{ $$ = new CallConventionNode(CallConvention.Register); }
	;

	
	// ========================================================================
	// Statements
	// ========================================================================
	
block
	: KW_BEGIN stmtlist KW_END		{ $$ = $1; }
	;

stmtlist
	: stmt SEMICOL stmtlist			{ $$ = new StatementBlock($3, $1); }
	| stmt							{ $$ = new StatementBlock($1, null); }
	;

stmt
	: nonlbl_stmt					{ $$ = $1; }
	| labelid COLON nonlbl_stmt		{ $3.SetLabel($1); $$ = $1; }
	;

nonlbl_stmt
	:						{ $$ = null;}
	| inheritstmts			{ $$ = $1; }
	| goto_stmt				{ $$ = $1; }
	| block					{ $$ = $1; }
	| ifstmt				{ $$ = $1; }
	| casestmt				{ $$ = $1; }
	| repeatstmt			{ $$ = $1; }
	| whilestmt				{ $$ = $1; }
	| forstmt				{ $$ = $1; }
	| with_stmt				{ $$ = $1; }
	| tryexceptstmt			{ $$ = $1; } // purely Delphi stuff!
	| tryfinallystmt		{ $$ = $1; }
	| raisestmt				{ $$ = $1; }
	| assemblerstmt			{ $$ = $1; }
	;

inheritstmts
	: KW_INHERITED				{ $$ = new InheritedStatement(null); }
	| KW_INHERITED assign		{ $$ = new InheritedStatement($2); }
	| KW_INHERITED proccall		{ $$ = new InheritedStatement($2); }
	| proccall					{ $$ = $1; }
	| assign					{ $$ = $1; }
	;

assign
	: lvalue KW_ASSIGN expr					{ $$ = new AssignementStatement($1, $3, false); }
	| lvalue KW_ASSIGN KW_INHERITED expr	{ $$ = new AssignementStatement($1, $3, true); }
	;


goto_stmt
	: KW_GOTO labelid	{ $$ = new GotoStatement($2); }
	;

ifstmt
	: KW_IF expr KW_THEN nonlbl_stmt KW_ELSE nonlbl_stmt		{ $$ = new IfStatement($2, $4, $6); }
	| KW_IF expr KW_THEN nonlbl_stmt							{ $$ = new IfStatement($2, $4, null); }
	;

casestmt
	: KW_CASE expr KW_OF caseselectorlist else_case KW_END	{ $$ = new CaseStatement($2, $4, $5); }
	;

else_case
	:								{ $$ = null;}	
	| KW_ELSE nonlbl_stmt			{ $$ = $2; }
	| KW_ELSE nonlbl_stmt SEMICOL	{ $$ = $2; }
	;

caseselectorlist
	: caseselector								{ $$ = new CaseSelectorList($1, null); }
	| caseselectorlist SEMICOL caseselector		{ $$ = new CaseSelectorList($3, $1); }
	;

caseselector
	:										{ $$ = null; }
	| caselabellist COLON nonlbl_stmt		{ $$ = new CaseSelector($1, $3); }
	;

caselabellist
	: caselabel							{ $$ = new CaseLabelList($1, null); }
	| caselabellist COMMA caselabel		{ $$ = new CaseLabelList($3, $1); }
	;

	// all exprs must be evaluate to const
caselabel
	: expr								{ $$ = new CaseLabel($1, null); }
	| expr KW_RANGE expr				{ $$ = new CaseLabel($1, $2); }
	;

repeatstmt
	: KW_REPEAT stmtlist KW_UNTIL expr	{ $$ = new RepeatStatement($2, $4); }
	;

whilestmt
	: KW_WHILE expr KW_DO nonlbl_stmt	{ $$ = new WhileStatement($2, $4); }
	;

forstmt
	: KW_FOR id KW_ASSIGN expr KW_TO	 expr KW_DO nonlbl_stmt	{ $$ = new ForStatement($2, $4, $6, $8, 1); }
	| KW_FOR id KW_ASSIGN expr KW_DOWNTO expr KW_DO nonlbl_stmt { $$ = new ForStatement($2, $4, $6, $8, -1); }
	;

	// expr must yield a ref to a record, object, class, interface or class type
with_stmt
	: KW_WITH exprlist KW_DO nonlbl_stmt		{ $$ = new WithStatement($2, $4); }
	;

tryexceptstmt
	: KW_TRY stmtlist KW_EXCEPT exceptionblock KW_END		{ $$ = new TryExceptStatement($2, $4); }
	;

exceptionblock
	: onlist KW_ELSE stmtlist						{ $$ = new ExceptionBlockNode($1, $3); }
	| onlist										{ $$ = new ExceptionBlockNode($1, null); }
	| stmtlist										{ $$ = $1; }
	;

onlist
	: ondef											{ $$ = new OnListNode($1, null); }
	| onlist ondef									{ $$ = new OnListNode($2, $1); }
	;

ondef
	: KW_ON id COLON id KW_DO nonlbl_stmt SEMICOL	{ $$ = new OnStatement($2, $4, $6); }
	| KW_ON 		 id KW_DO nonlbl_stmt SEMICOL	{ $$ = new OnStatement(null, $2, $4); }
	;

tryfinallystmt
	: KW_TRY  stmtlist KW_FINALLY stmtlist KW_END		{ $$ = new TryFinallyStatement($2, $4); }
	;

raisestmt
	: KW_RAISE							{ $$ = new RaiseStatement(null, null); }
	| KW_RAISE lvalue					{ $$ = new RaiseStatement($2, null); }
	| KW_RAISE			KW_AT expr		{ $$ = new RaiseStatement(null, $2); }
	| KW_RAISE lvalue	KW_AT expr		{ $$ = new RaiseStatement($2, $4); }
	;

assemblerstmt
	: KW_ASM asmcode KW_END		{ $$ = $2; }
	;

asmcode
	: ASM_OP					{ $$ = new AssemblerListNode($1, null); }
	| asmcode ASM_OP			{ $$ = new AssemblerListNode($2, $1); }
	;





	// ========================================================================
	// Variables and Expressions
	// ========================================================================

varsec
	: KW_VAR vardecllist		{ $$ = $1; }
	;
	
thrvarsec
	: KW_THRVAR vardecllist		{ $$ = $1; }
	;

vardecllist
	: vardecl					{ $$ = new VarDeclarationListNode($1, null); }
	| vardecllist vardecl		{ $$ = new VarDeclarationListNode($2, $1); }
	;

	/* VarDecl
		On Windows -> idlist ':' Type [(ABSOLUTE (id | ConstExpr))	| '=' ConstExpr] [portability]
		On Linux   -> idlist ':' Type [ ABSOLUTE (Ident)			| '=' ConstExpr] [portability]
	*/

vardecl
	: idlist COLON vartype vardeclopt SEMICOL				{ $$ = new VarDeclarationNode($1, $3, $4); }
	| idlist COLON proceduretype SEMICOL funcdirectopt		{ $$ = new ProcedurePointerDeclarationNode($1, $3, $5, null); }
	| idlist COLON proceduretype SEMICOL funcdirectopt_nonterm KW_EQ CONST_NIL SEMICOL	{ $$ = new ProcedurePointerDeclarationNode($1, $3, $5, null); }
	| idlist COLON proceduretype SEMICOL funcdirectopt_nonterm KW_EQ id SEMICOL	{ $$ = new ProcedurePointerDeclarationNode($1, $3, $5, $7); }
	;

vardeclopt
	: /*port_opt*/
	| KW_ABSOLUTE id  /*port_opt*/				{ $$ = new VariableAbsoluteNode($2); }
	| KW_EQ constexpr /*portabilityonapt*/		{ $$ = new VariableInitNode($2); }
	;

	// func call, type cast or identifier //  TODO TODO TODO TODO
proccall
	: id									{ $$ = new ProcedureCallNode($1, null); }
	| lvalue KW_DOT id						{ $$ = new FieldAcess($1, $3); } // field access
	| lvalue LPAREN exprlistopt RPAREN		{ $$ = new ProcedureCallNode($1, $3); } // pointer deref
	| lvalue LPAREN casttype RPAREN			{ $$ = new ProcedureCallNode($1, $3); }// for funcs like High, Low, Sizeof etc
	;

lvalue
	: proccall								{ $$ = $1; }	// proc_call or cast
//	| KW_INHERITED proccall		// TODO
	| expr KW_DEREF 						{ $$ = new PointerDereferenceNode($1); } // pointer deref
	| lvalue LBRAC exprlist RBRAC			{ $$ = new ArrayAccessNode($1, $3); }	// array access
	| string_const LBRAC exprlist RBRAC		{ $$ = new ArrayAccessNode($1, $3); } // string access
	| casttype LPAREN exprlistopt RPAREN	{ $$ = new TypeCastNode($1, $3); } // cast with pre-defined type

//	| LPAREN lvalue RPAREN		// TODO
	;

expr
	: literal							{ $$ = $1; }
	| lvalue							{ $$ = new LValueNode($1); }
	| setconstructor					{ $$ = $1; }
	| KW_ADDR expr						{ $$ = new AddressNode($2); }
	| KW_NOT expr						{ $$ = new NegationNode($2); }
	| sign	 expr %prec UNARY			{ $$ = new UnaryOperationNode($2, $1); }
	| LPAREN expr RPAREN				{ $$ = $1; }
	| expr relop expr %prec KW_EQ		{ $$ = new BinaryOperationNode($1, $3, $2); }
	| expr addop expr %prec KW_SUB		{ $$ = new BinaryOperationNode($1, $3, $2); }
	| expr mulop expr %prec KW_MUL		{ $$ = new BinaryOperationNode($1, $3, $2); }
	;

sign
	: KW_SUB		{ $$ = new OperatorNode("-");}
	| KW_SUM		{ $$ = new OperatorNode("+");}
	;
mulop
	: KW_MUL		{ $$ = new OperatorNode("*");}
	| KW_DIV		{ $$ = new OperatorNode("/");}
	| KW_QUOT		{ $$ = new OperatorNode("div");}
	| KW_MOD		{ $$ = new OperatorNode("mod");}
	| KW_SHR		{ $$ = new OperatorNode("shr");}
	| KW_SHL		{ $$ = new OperatorNode("shl");}
	| KW_AND		{ $$ = new OperatorNode("and");}
	;
addop
	: KW_SUB	{ $$ = new OperatorNode("-");}
	| KW_SUM	{ $$ = new OperatorNode("+");}
	| KW_OR		{ $$ = new OperatorNode("or");}
	| KW_XOR	{ $$ = new OperatorNode("xor");}
	;
relop
	: KW_EQ		{ $$ = new OperatorNode("=");}
	| KW_DIFF	{ $$ = new OperatorNode("<>");}
	| KW_LT		{ $$ = new OperatorNode("<");}
	| KW_LE		{ $$ = new OperatorNode("<=");}
	| KW_GT		{ $$ = new OperatorNode(">");}
	| KW_GE		{ $$ = new OperatorNode(">=");}
	| KW_IN		{ $$ = new OperatorNode("in");}
	| KW_IS		{ $$ = new OperatorNode("is");}
	| KW_AS		{ $$ = new OperatorNode("as");}
	;

literal
	: CONST_INT		{ $$ = new IntegerLiteralNode($1);}
	| CONST_BOOL	{ $$ = new BoolLiteralNode($1);}
	| CONST_REAL	{ $$ = new RealLiteralNode($1);}
	| CONST_NIL		{ $$ = new NilLiteralNode();}
	| string_const	{ $$ = new StringLiteralNode($1);}
	;

discrete
	: CONST_INT		{ $$ = new IntegerLiteralNode($1);}
	| CONST_CHAR	{ $$ = new CharLiteralNode($1);}
	| CONST_BOOL	{ $$ = new BoolLiteralNode($1);}
	;

string_const
	: CONST_STR					{ $$ = $1; }
	| CONST_CHAR				{ $$ = $1; }
	| string_const CONST_STR	{ $$ = $1 + $2; }
	| string_const CONST_CHAR	{ $$ = $1 + $2; }
	;

id	: IDENTIFIER	{ $$ = new IdentifierNode($1); }
	;

idlist
	: id				{ $$ = new IdentifierListNode($1, null); }
	| idlist COMMA id	{ $$ = new IdentifierListNode($3, $1); }
	;

qualid
	: id				{ $$ = new IdentifierListNode($1, null); }
	| qualid KW_DOT id	{ $$ = new UnfinishedNode($1); }
	;
	
exprlist
	: expr					{ $$ = new ExpressionListNode($1, null); }
	| exprlist COMMA expr	{ $$ = new ExpressionListNode($3, $1); }
	;

exprlistopt
	:						{ $$ = null; }
	| exprlist				{ $$ = $1; }
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
	: sign rangestart KW_RANGE expr		{ $$ = new SetElement($2, $4); }	
	| rangestart KW_RANGE expr			{ $$ = new SetElement($1, $3); }
	;

rangestart
	: discrete						{ $$ = $1; }
	| qualid						{ $$ = $1; }
	| id LPAREN casttype RPAREN		{ $$ = ProcedureCall($1, $3); }
	| id LPAREN literal RPAREN		{ $$ = ProcedureCall($1, $3); }
	;

enumtype
	: LPAREN enumtypeellist RPAREN		{ $$ = $2; }
	;

enumtypeellist
	: enumtypeel							{ $$ = new EnumList($1, null); }
	| enumtypeellist COMMA enumtypeel		{ $$ = new EnumList($3, $1); }
	;

enumtypeel
	: id					{ $$ = new FieldInit($1, null); }
	| id KW_EQ expr			{ $$ = new FieldInit($1, $3); }
	;

setconstructor
	: LBRAC	RBRAC				{ $$ = null;}
	| LBRAC setlist	RBRAC		{ $$ = $2; }
	;

setlist
	: setelem					{ $$ = new SetList($1, null); }
	| setlist COMMA setelem		{ $$ = new SetList($3, $1); }
	;
	
setelem
	: expr					{ $$ = new SetElement($1, null); }
	| expr KW_RANGE expr	{ $$ = new SetElement($1, $3); }
	;




	
	// ========================================================================
	// Constants
	// ========================================================================

	/*
	// Resource strings, windows-only

	rscstringsec
		: TYPE_RSCSTR  constrscdecllist		{ $$ = $1; }
		;

	constrscdecllist
		: constrscdecl							{$$ = new ConstDeclarationList($1, null); }
		| constrscdecllist constrscdecl			{$$ = new ConstDeclarationList($2, $1); }
		;

	constrscdecl
		: id KW_EQ literal SEMICOL		{$$ = new ConstDeclarationNode($1, null, $3); }
		;	
	
	// --------------------------------
	*/
	
constsec
	: KW_CONST constdecl	{ $$ = new ConstDeclarationList($2, null); }
	| constsec constdecl	{ $$ = new ConstDeclarationList($2, $1); }
	;

constdecl
	: id KW_EQ constexpr /*port_opt*/ SEMICOL	{ $$ = new ConstDeclarationNode($1, null, $3);}			// true const
	| id COLON vartype KW_EQ constexpr /*port_opt*/ SEMICOL	{ $$ = new ConstDeclarationNode($1, $3, $5);}	// typed const
	;

constexpr
	: expr				{ $$ = $1; }
	| arrayconst		{ $$ = $1; }
	| recordconst		{ $$ = $1; }
	;

	// 1 or more exprs
arrayconst
	: LPAREN constexpr COMMA constexprlist RPAREN	{ $$ = new ExpressionListNode($2, $4); }
	;

constexprlist
	: constexpr							{ $$ = new ExpressionListNode($1, null); }
	| constexprlist COMMA constexpr		{ $$ = new ExpressionListNode($3, $1); }
	;

recordconst
	: LPAREN fieldconstlist RPAREN				{$$ = $2; }
	| LPAREN fieldconstlist SEMICOL RPAREN		{$$ = $2; }
	;

fieldconstlist
	: fieldconst							{ $$ = FieldInitList($1, null);}
	| fieldconstlist SEMICOL fieldconst		{ $$ = FieldInitList($3, $1);}
	;

fieldconst
	: id COLON constexpr		{ $$ = new FieldInit($1, $3); }
	;





	// ========================================================================
	// Composite Types
	// ========================================================================
	
	// Records and objects are treated as classes
	
classtype
	: class_keyword heritage class_struct_opt KW_END		{ $$ = new ClassDefinition($1, $2, $3); }
	| class_keyword heritage								{ $$ = new ClassDefinition($1, $2, null); } // forward decl			
	;

class_keyword
	: KW_CLASS		{ $$ = ClassType.Class; }
	| KW_OBJECT		{ $$ = ClassType.Object; }
	| KW_RECORD		{ $$ = ClassType.Record; }
	;

heritage
	:
	| LPAREN idlist RPAREN	{ $$ = $2; }		// inheritance from class and interf(s)			
	;

class_struct_opt
	: fieldlist complist scopeseclist	{ $$ = new ClassStruct(Scope.Public, $2, $3);  }
	;

scopeseclist
	:							{ $$ = null; }
	| scopeseclist scopesec		{ $$ = new ClassContentList($2, $1);  }
	;

scopesec
	: scope_decl fieldlist complist	{ $$ = new ClassStruct($1, $2, $3);  }
	;

scope_decl
	: KW_PUBLISHED			{ $$ = Scope.Published; }
	| KW_PUBLIC				{ $$ = Scope.Public; }
	| KW_PROTECTED			{ $$ = Scope.Protected; }
	| KW_PRIVATE			{ $$ = Scope.Private; }
	;

fieldlist
	:						{ $$ = null;}
	| fieldlist objfield	{ $$ = new ClassFieldList($2, $1); }
	;
		
complist
	:							{ $$ = null; }
	| complist class_comp		{ $$ = new ClassContentList($2, $1); }
	;

objfield
	: idlist COLON type SEMICOL	{ $$ = new VarDeclarationNode($1, $3, null); }
	;
	
class_comp
	: staticclassopt procproto	{ $2.isStatic = $1; $$ = $2; }
	| property					{ $$ = $1; }
	;

interftype
	: KW_INTERF heritage classmethodlistopt classproplistopt KW_END	 { $$ = new InterfaceDefinition($2, $3, $4); }
	;

classmethodlistopt
	: methodlist		{ $$ = $1; }
	|					{ $$ = null; }
	;

methodlist
	: procproto					{ $$ = new ClassContentList(new ClassMethod($1), null); }
	| methodlist procproto		{ $$ = new ClassContentList(new ClassMethod($2), $1); }
	;


	
	
	
	// ========================================================================
	// Properties
	// ========================================================================
	
	
classproplistopt
	: classproplist		{ $$ = $1; }
	|					{ $$ = null; }
	;

classproplist
	: property					{ $$ = new ClassContentList($1, null); }
	| classproplist property	{ $$ = new ClassContentList($2, $1); }
	;

property
	: KW_PROPERTY id SEMICOL		{ $$ = null; }
	| KW_PROPERTY id propinterfopt COLON funcrettype indexspecopt propspecifiers SEMICOL defaultdiropt { $$ = new ClassProperty($2, $3, $4, $6); }
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
		: indexspecopt readacessoropt writeacessoropt storedspecopt defaultspecopt implementsspecopt	{ $$ = new PropertySpecifierNode($2, $3); }
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
	: id KW_EQ typeopt vartype /*port_opt*/ SEMICOL							{ $$ = new TypeDeclarationNode($1, $4); }
	| id KW_EQ typeopt proceduretype /*port_opt*/ SEMICOL funcdirectopt		{ $$ = new ProcedureTypeDeclarationNode($1, $4, $6); }
	;

typeopt
	:								{ $$ = true; }
	| KW_TYPE						{ $$ = true; }
	;

type
	: vartype						{ $$ = $1; }
	| proceduretype					{ $$ = $1; }
	;

vartype
	: simpletype					{ $$ = $1; }
	| enumtype						{ $$ = $1; }
	| rangetype						{ $$ = $1; }
	| varianttype					{ $$ = $1; }
	| refpointertype				{ $$ = $1; }
	// metaclasse
	| classreftype					{ $$ = $1; }
	// object definition		
	| KW_PACKED  packedtype			{ $$ = $1; }
	| packedtype					{ $$ = $1; }
	;

packedtype
	: structype						{ $$ = new UnfinishedNode($1); }
	| restrictedtype				{ $$ = new UnfinishedNode($1); }
	;

classreftype
	: KW_CLASS KW_OF scalartype		{ $$ = new UnfinishedNode($1); }
	;

simpletype
	: scalartype				{ $$ = $1; }
	| realtype					{ $$ = $1; }
	| stringtype				{ $$ = $1; }
	| TYPE_PTR					{ $$ = new PointerType(); }
	;

ordinaltype
	: enumtype					{ $$ = $1; }
	| rangetype					{ $$ = $1; }
	| scalartype				{ $$ = $1; }
	;

scalartype
	: inttype								{ $$ = $1; }
	| chartype								{ $$ = $1; }
	| qualid		// user-defined type	{ $$ = $1; }
	;

realtype
	: TYPE_REAL48		{ $$ = new DoubleType(); }
	| TYPE_FLOAT		{ $$ = new FloatType(); }
	| TYPE_DOUBLE		{ $$ = new DoubleType(); }
	| TYPE_EXTENDED		{ $$ = new ExtendedType(); }
	| TYPE_CURR			{ $$ = new CurrencyType(); }
	;

inttype
	: TYPE_BYTE				{ $$ = new UnsignedInt8Type(); }
	| TYPE_BOOL				{ $$ = new BoolType(); }
	| TYPE_INT				{ $$ = new SignedInt32Type(); }
	| TYPE_SHORTINT			{ $$ = new SignedInt8Type(); }
	| TYPE_SMALLINT			{ $$ = new SignedInt16Type(); }
	| TYPE_LONGINT			{ $$ = new SignedInt32Type(); }
	| TYPE_INT64			{ $$ = new SignedInt64Type(); }
	| TYPE_UINT64			{ $$ = new UnsignedInt64Type(); }
	| TYPE_WORD				{ $$ = new UnsignedInt16Type(); }
	| TYPE_LONGWORD			{ $$ = new UnsignedInt32Type(); }
	| TYPE_CARDINAL			{ $$ = new UnsignedInt32Type(); }
	| TYPE_COMP				{ $$ = new SignedInt64Type(); }
	;

chartype
	: TYPE_CHAR						{ $$ = new CharType(); }
	| TYPE_WIDECHAR					{ $$ = new CharType(); }
	;

stringtype
	: TYPE_STR		// dynamic size	{ $$ = new StringType(null); }
	| TYPE_PCHAR					{ $$ = new StringType(null); }
	| TYPE_STR LBRAC expr RBRAC		{ $$ = new StringType($3); }
	| TYPE_SHORTSTR					{ $$ = new StringType(null); }
	| TYPE_WIDESTR					{ $$ = new StringType(null); }
	;

varianttype
	: TYPE_VAR			{ $$ = new UnfinishedNode($1); }
	| TYPE_OLEVAR		{ $$ = new UnfinishedNode($1); }
	;

structype
	: arraytype			{ $$ = new UnfinishedNode($1); }
	| settype			{ $$ = new UnfinishedNode($1); }
	| filetype			{ $$ = new UnfinishedNode($1); }
	;
	
restrictedtype
	: classtype								{ $$ = $1; }
	| interftype							{ $$ = $1; }
	;

arraysizelist
	: rangetype								{ $$ = $1; }
	| arraysizelist COMMA rangetype			{ $$ = new UnfinishedNode($1); }
	;

arraytype
	: TYPE_ARRAY LBRAC arraytypedef RBRAC KW_OF type /*port_opt*/	{ $$ = new UnfinishedNode($1); }
	| TYPE_ARRAY KW_OF type /*port_opt*/	{ $$ = new UnfinishedNode($1); }
	;

arraytypedef
	: arraysizelist		{ $$ = $1; }
	| inttype			{ $$ = $1; }
	| chartype			{ $$ = $1; }
	| qualid			{ $$ = $1; }
	;

settype
	: TYPE_SET KW_OF ordinaltype /*port_opt*/		{ $$ = new UnfinishedNode($1); }
	;

filetype
	: TYPE_FILE KW_OF type /*port_opt*/		{ $$ = new FileType($3); }
	| TYPE_FILE								{ $$ = new FileType(null); }
	;

refpointertype
	: KW_DEREF type /*port_opt*/				{ $$ = new UnfinishedNode($1); }
	;

funcparamtype
	: simpletype								{ $$ = new UnfinishedNode($1); }
	| TYPE_ARRAY KW_OF simpletype /*port_opt*/	{ $$ = new UnfinishedNode($1); }
	;

funcrettype
	: simpletype		{ $$ = $1; }
	;
	
	// simpletype w/o user-defined types
casttype
	: inttype			{ $$ = new UnfinishedNode($1); }
	| chartype			{ $$ = new UnfinishedNode($1); }
	| realtype			{ $$ = new UnfinishedNode($1); }
	| stringtype		{ $$ = new UnfinishedNode($1); }
	| TYPE_PTR			{ $$ = new UnfinishedNode($1); }
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
			public InputRejected (int lineno, string message = "Input invalid - Parsing terminated by REJECT action") 
				: base ("Line " + lineno + ": " + message)  { }
			
			public InputRejected (string message = "Input invalid - Parsing terminated by REJECT action") 
				: base (message)  { }
		}
	}

// already defined in template
//	} // close outermost namespace
