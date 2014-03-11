%{

using System;
using System.Collections;
using System.IO;
using System.Text;
using System.Diagnostics;
using crosspascal.ast;
using crosspascal.ast.nodes;

namespace crosspascal.parser
{

	// Open main Parser class
	public class DelphiParser
	{

		public static int DebugLevel  =  0;

		// Emulate YACC

		void ACCEPT()
		{	// make scanner emit EOF, ends scanning and parsing
			lexer.Accept();
			yyState = yyFinal;
		}

		void REJECT(string msg = "")
		{	
			throw new InputRejected(lexer.yylineno(), msg);
		}
			
		// Internal helper functions
		
		string GetErrorMessage(ParserException e)
		{
			StackTrace st = new StackTrace(e, true);
			StackFrame frame = st.GetFrame(st.FrameCount-1); 
			return "[ERROR] " + e.Message + " in " + Path.GetFileName(frame.GetFileName())
					+ ": line " + frame.GetFileLineNumber();
		}

		
		//	Encoding.Default);	// typically Single-Bye char set
		// TODO change charset to unicode, use %unicode in flex
		public static readonly Encoding DefaultEncoding = Encoding.GetEncoding("iso-8859-1");

		DelphiScanner lexer;		
		
		// Entry point and public interface
		
		internal DelphiParser(ParserDebug dgb)
		{
			if (dgb != null) {
				this.debug = (ParserDebug) dgb;
				DebugLevel = 1;
			}
			
			eof_token = DelphiScanner.YYEOF;
			
		}

		internal DelphiParser()
		{
			this.debug = new Func<ParserDebug>(
					() => {	switch(DelphiParser.DebugLevel)
						{	case 1: return new DebugPrintFinal();
							case 2: return new DebugPrintAll();
							default: return null;
						}
					})();
			eof_token = DelphiScanner.YYEOF;
		}
		
		// wrapper for yyparse
		internal Object Parse(TextReader tr, ParserDebug dgb = null)
		{
			if (dgb != null) {
				this.debug = (ParserDebug) dgb;
				DebugLevel = 1;
			}
			
			lexer = new DelphiScanner(tr);
			
			try {
				Object ret = yyparse(lexer);
				return ret;
			} 
			catch (ParserException yye) {
				ErrorOutput.WriteLine(yye.Message);
				// only clean way to signal error. null is the default yyVal
				throw yye; // new InputRejected(GetErrorMessage(yye));
			}
		}

%}


	// ==============================================================
	// Rules declarations
	// ==============================================================

%start goal
	// file type

%type<bool>  staticclassopt ofobjectopt
%type<GoalNode> goal file
%type<ProgramNode> program
%type<LibraryNode> library
%type<UnitNode> unit
%type<PackageNode> package
%type<UsesNode> requiresclause containsclause usesclauseopt useidlist
%type<IdentifierNode> id useid externarg qualid
%type<UnitImplementationNode> implementsec
%type<UnitInterfaceNode> interfsec
%type<DeclarationListNode> interfdecllist maindecllist declseclist
%type<UnitInitialization> initsec
%type<BlockWithDeclarationsNode> main_block
%type<DeclarationNode> interfdecl maindeclsec funcdeclsec basicdeclsec typesec labeldeclsec labelidlist  varsec thrvarsec vardecllist vardecl constdecl typedecl
%type<LabelNode> labelid
%type<ExportItem> exportsec	 exportsitemlist exportsitem
%type<ProcedureDefinitionNode> procdefinition procdeclnondef 
%type<ProcedureHeaderNode> procdefproto procdeclinterf proceduresign procsignfield
%type<ProcedureBodyNode> proc_define func_block
%type<TypeNode> funcrettype simpletype funcretopt funcparamtype paramtypeopt paramtypespec
%type<FunctionClass> prockind
%type<ParameterNodeList> formalparams formalparamslist
%type<ParamterNode> formalparm
%type<VarParameterQualifier> paramqualif
%type<Expression> paraminitopt expr rangetype  rangestart constexpr functypeinit
%type<ProcedureDirectiveList> funcdirectopt funcdir_noterm_opt funcdirectlist funcqualinterflist									
%type<ProcedureDirective> funcdirective funcqualinterf funcqualif funcdeprecated funcqualinterfopt
%type<CallConventionNode> funccallconv
%type<StatementBlock> block stmtlist
%type<Statement> stmt nonlbl_stmt assign goto_stmt ifstmt casestmt else_case repeatstmt whilestmt forstmt with_stmt tryexceptstmt tryfinallystmt raisestmt assemblerstmt asmcode
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
%type<ConstDeclarationList> constsec
%type<ExpressionListNode> arrayconst
%type<FieldInitList>  recordconst fieldconstlist
%type<ClassDefinition> classtype
%type<ClassType> class_keyword
%type<ClassStruct> scopesec
%type<ClassContentList> scopeseclist complist classmethodlistopt methodlist classproplistopt classproplist
%type<ClassContent> class_comp
%type<Scope> scope_decl
%type<ClassFieldList> fieldlist
%type<VarDeclarationNode> objfield
%type<InterfaceDefinition> interftype
%type<ClassProperty> property
%type<bool> typeopt 
%type<TypeNode> vartype classreftype simpletype ordinaltype casttype
%type<TypeNode> packstructtype packcomptype compositetype
%type<TypeNode> scalartype realtype inttype chartype stringtype varianttype funcrettype 
%type<TypeNode> arraytype settype filetype refpointertype funcparamtype  structuredtype
%type<ArraySizeList>  arraysizelist
%type<ArraySizeList> arraytypedef

%type<ListNode> propspecifiers constinitexprlist variantlist rscstringlist idlisttypeidlist

%type<PropertySpecifier> propinterfopt defaultdiropt indexspecopt storedspecopt defaultspecopt implementsspecopt readacessoropt writeacessoropt
%type<Expression> unaryexpr constinitexpr inheritexpr basicliteral simpleconst rangestart functypeinit
%type<RecordNode> recordtype recordtypebasic

%type<Node> variant_struct varfieldlist varfield  variantvar guid rscstring class_struct rscstringsec idlisttypeid


	
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

	// pseudo, hints, windows-specific, deprecated, obscure, etc
%token ASM_OP WINDOWS_GUID KW_BREAK KW_CONTINUE KW_FAR KW_NEAR KW_RESIDENT 


	// ==============================================================
	// Precedence and associativity
	// ==============================================================

	// lowest precedence |
	//					 v
%nonassoc LOWESTPREC 

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

%%

	// ==============================================================
	// YACC Rules
	// ==============================================================
	
goal: file KW_DOT		{	$$ = $1; YYACCEPT();	}
	;

file
	: program	{ $$ = $1; }
	| package	{ $$ = $1; }
	| library	{ $$ = $1; }
	| unit		{ $$ = $1; }
	;

semicolopt
	:
	| SEMICOL
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
	: KW_UNIT id  SEMICOL interfsec implementsec initsec	{ $$ = new UnitNode($2, $4, $5, $6); }
	;

package
	: id id SEMICOL requiresclause containsclause KW_END	{ $$ = new PackageNode($2, $4, $5); }
	;

requiresclause
	: id idlist SEMICOL	{ $$ = new UsesNode($1, $2);}	// check that id == "Requires"
	;

containsclause
	: id idlist SEMICOL	{ $$ = new UsesNode($1, $2);}	// check that id == "Contains"
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
	: funcdeclsec				{ $$ = new DeclarationListNode($1, null);}
	| declseclist funcdeclsec	{ $$ = new DeclarationListNode($2, $1);}
	;

	

	// ========================================================================
	// Declaration sections
	// ========================================================================

interfdecl
	: basicdeclsec					{ $$ = $1; }
	| staticclassopt procdeclinterf		{ $2.isStatic = $1; $$ = $2;}
	| thrvarsec						{ $$ = $1; }
	| rscstringsec					{ $$ = $1; }
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
	: constsec				{ $$ = $1;}
	| typesec				{ $$ = $1;}
	| varsec				{ $$ = $1;}
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
	: CONST_INT /* decimal int 0..9999 */	{ $$ = new NumberLabelNode(yyLex.value()); }
	| id									{ $$ = new StringLabelNode($1); }
	;

	// Exports
		
exportsec	
	: KW_EXPORTS exportsitemlist		{ $$ = $2; }
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
	: procdefproto funcqualinterflist funcdirectopt		{ $$ = new ProcedureDefinitionNode($1, null); } 
	;

procdefinition
	: procdefproto proc_define SEMICOL	{ $$ = new ProcedureDefinitionNode($1, $2); } 
	;

procdeclinterf
	: procdefproto funcqualinterfopt	{ $$ = null; /* TODO */ }
	;

	// proc proto for definitions or external/forward decls
	// check that funcrecopt is null for every kind except FUNCTION
procdefproto
	: prockind qualid formalparams funcretopt SEMICOL funcdirectopt		{ $$ = new ProcedureHeaderNode($1, $2, $3, $4, null, $6); }
	;

funcqualinterfopt
	: 									{ $$ = null; /* TODO */ }
	| funcqualinterflist funcdirectopt	{ $$ = null; /* TODO */ }
	;

funcretopt
	:						{ $$ = null;}
	| COLON funcrettype		{ $$ = $2;}
	;

staticclassopt
	:							{ $$ = false;}
	| KW_CLASS					{ $$ = true;}
	;

prockind
	: KW_FUNCTION				{ $$ = FunctionClass.Function;}
	| KW_PROCEDURE				{ $$ = FunctionClass.Procedure;}
	| KW_CONSTRUCTOR			{ $$ = FunctionClass.Constructor;}
	| KW_DESTRUCTOR				{ $$ = FunctionClass.Destructor;}
	;

proceduresign
	: KW_PROCEDURE formalparams ofobjectopt		{ $$ = new ProcedureHeaderNode(FunctionKind.Procedure, null, $2, null, $3, null); }
	| KW_FUNCTION  formalparams COLON funcrettype ofobjectopt { $$ = new ProcedureHeaderNode(FunctionKind.Function, null, $2, $4, $5, null); }
	;

procsignfield
	: proceduresign					{ $$ = null; /* TODO */ }
	| proceduresign funccallconv	{ $$ = null; /* TODO */ }
	;

ofobjectopt
	:						{ return false; }
	| KW_OF KW_OBJECT		{ return true; }
	;


	// Function blocks and parameters

proc_define
	: declseclist func_block				{ $$ = new ProcedureBodyNode($1, $2); }
	| 			  func_block				{ $$ = new ProcedureBodyNode(null, $1); }
	;

func_block
	: block									{ $$ = $1; }
	| assemblerstmt							{ $$ = $1; }
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
	| KW_CONST		idlist paramtypeopt paraminitopt	{ $$ = new ParameterNode(new ConstParameterQualifier(), $2, $3, $4); }
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
	:									{ $$ = null; }
	| KW_EQ constexpr					{ $$ = $2; }
	;

	
	// Function directives
	
functypeinit
	: KW_EQ id 							{ $$ = null; /* TODO */ }
	| KW_EQ CONST_NIL					{ $$ = null; /* TODO */ }
	;

funcdir_noterm_opt
	:									{ $$ = null; /* TODO */ }
	| funcdirectlist					{ $$ = null; /* TODO */ }
	;

funcdirectopt
	:									{ $$ = null; }
	| funcdirectlist SEMICOL			{ $$ = $1; }
	;

funcdirectlist
	: funcdirective									{ $$ = new ProcedureDirectiveList($1, null); }
	| funcdirectlist SEMICOL funcdirective			{ $$ = new ProcedureDirectiveList($3, $1); }
	;

funcqualinterflist									
	: funcqualinterf SEMICOL						{ $$ = new ProcedureDirectiveList($1, null); }
	| funcqualinterflist funcqualinterf SEMICOL		{ $$ = new ProcedureDirectiveList($2, $1); }
	;

funcdirective
	: funcqualif		{ $$ = $1; }
	| funccallconv		{ $$ = $1; }
	| funcdeprecated	{ $$ = $1; }
	;

funcdeprecated
	: KW_FAR			{ $$ = new ProcedureDirective(ProcedureDirectiveEnum.Far); }
	| KW_NEAR			{ $$ = new ProcedureDirective(ProcedureDirectiveEnum.Near); }
	| KW_RESIDENT		{ $$ = new ProcedureDirective(ProcedureDirectiveEnum.Resident); }
	;

funcqualinterf
	: KW_EXTERNAL string_const externarg	{ $$ = new ExternalProcedureDirective(new IdentifierNode($2), $3); }
	| KW_EXTERNAL qualid externarg			{ $$ = new ExternalProcedureDirective($2, $3); }
	| KW_EXTERNAL							{ $$ = new ExternalProcedureDirective(null, null); }
	| KW_FORWARD							{ $$ = new ProcedureDirective(ProcedureDirectiveEnum.Forward); }
	;

externarg
	:						{$$ = null; }
	| KW_NAME string_const 	{$$ = new IdentifierNode($2);}	// id == NAME		
	| KW_NAME qualid	 	{$$ = $2;}  // id == NAME		
	;

funcqualif
	: KW_ABSTRACT			{ $$ = new ProcedureDirective(ProcedureDirectiveEnum.Abstract); }
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
	: KW_BEGIN stmtlist KW_END		{ $$ = $2; }
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
	:						{ $$ = null; /* TODO */ }
	// procedure call, no params 
	| KW_INHERITED			{ $$ = null; /* TODO */ }
	| inheritexpr			{ $$ = null; /* TODO */ }
	| assign				{ $$ = null; /* TODO */ }
	| proccall				{ $$ = null; /* TODO */ }
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
	| KW_BREAK				{ $$ = new BreakStatement(); }
	| KW_CONTINUE			{ $$ = new ContinueStatement(); }
	;

assign
	: lvalue KW_ASSIGN expr					{ $$ = new AssignementStatement($1, $3, false); }
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

caselabel
	: constexpr							{ $$ = new CaseLabel($1, null); }
	| constexpr KW_RANGE constexpr		{ $$ = new CaseLabel($1, $3); }
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
	: KW_TRY stmtlist KW_EXCEPT exceptionblock KW_END	{ $$ = new TryExceptStatement($2, $4); }
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
	: KW_TRY  stmtlist KW_FINALLY stmtlist KW_END	{ $$ = new TryFinallyStatement($2, $4); }
	;

raisestmt
	: KW_RAISE							{ $$ = new RaiseStatement(null, null); }
	| KW_RAISE lvalue					{ $$ = new RaiseStatement($2, null); }
	| KW_RAISE			KW_AT expr		{ $$ = new RaiseStatement(null, $3); }
	| KW_RAISE lvalue	KW_AT expr		{ $$ = new RaiseStatement($2, $4); }
	;

assemblerstmt
	: KW_ASM asmcode KW_END		{ $$ = new AssemblerProcedureBodyNode($2); }
	;

asmcode
	: 							{ $$ = null; }
	| asmcode ASM_OP			{ $$ = null; }
	;





	// ========================================================================
	// Variables
	// ========================================================================

varsec
	: KW_VAR vardecllist		{ $$ = $2; }
	;
	
thrvarsec
	: KW_THRVAR vardecllist		{ $$ = $2; }
	;

vardecllist
	: vardecl					{ $$ = new VarDeclarationListNode($1, null); }
	| vardecllist vardecl		{ $$ = new VarDeclarationListNode($2, $1); }
	;

vardecl
	: idlist COLON vartype vardeclopt SEMICOL				{ $$ = new VarDeclarationNode($1, $3, $4); }
	| idlist COLON proceduresign SEMICOL funcdirectopt		{ $$ = new ProcedurePointerDeclarationNode($1, $3, $5, null); }
	| idlist COLON proceduresign SEMICOL funcdir_noterm_opt functypeinit SEMICOL	{ $$ = new ProcedurePointerDeclarationNode($1, $3, $5, $6); }
	;

vardeclopt
	: 										{ $$ = null; /* TODO */ }
	| KW_ABSOLUTE id  						{ $$ = new VariableAbsoluteNode($2); }
	| KW_EQ constinitexpr 					{ $$ = new VariableInitNode($2); }
	;

	// Resourcestrings fom windows
	
rscstringsec
	: TYPE_RSCSTR rscstringlist				{ $$ = $2; }
	;
	
rscstringlist
	: rscstring								{ $$ = new ConstDeclarationList($1, null); }
	| rscstringlist rscstring				{ $$ = new ConstDeclarationList($2, $1); }
	;
	
rscstring
	:  id KW_EQ string_const SEMICOL		{ $$ = new ConstDeclarationNode($1, null, $3); }
	;


	
	// ========================================================================
	// Expressions
	// ========================================================================

inheritexpr
	: KW_INHERITED id								{ $$ = null; /* TODO */ }
	| KW_INHERITED id LPAREN exprlistopt RPAREN		{ $$ = null; /* TODO */ }
	;
 
	// func call to be called as statement
proccall
	: id									{ $$ = new ProcedureCallNode($1, null); }
	| lvalue LPAREN exprlistopt RPAREN		{ $$ = new ProcedureCallNode($1, $3); } // pointer deref
	| lvalue KW_DOT id						{ $$ = new FieldAcess($1, $3); } // field access
	;
	
lvalue	// lvalue
	: id									{ $$ = $1; }
	| lvalue LPAREN exprlistopt RPAREN		{ $$ = null; /* TODO */ }
	| lvalue LPAREN casttype RPAREN			{ $$ = null; /* TODO */ }
	| casttype LPAREN exprlistopt RPAREN	{ $$ = new TypeCastNode($1, $3); } // cast with pre-defined type
	| lvalue KW_DOT id						{ $$ = null; /* TODO */ }
	| lvalue KW_DEREF						{ $$ = new PointerDereferenceNode($1); } // pointer deref
	| lvalue LBRAC exprlist RBRAC			{ $$ = new ArrayAccessNode($1, $3); }	// array access
	| string_const LBRAC expr RBRAC			{ $$ = new ArrayAccessNode($1, $3); } // string access
	| LPAREN expr RPAREN					{ $$ = null; /* TODO */ }
	;

unaryexpr
	: literal								{ $$ = $1; }
	| lvalue								{ $$ = new LValueNode($1); }
	| setconstructor						{ $$ = $1; }
	| KW_ADDR unaryexpr						{ $$ = new AddressNode($2); }
	| KW_NOT unaryexpr						{ $$ = new NegationNode($2); }
	| sign	 unaryexpr 						{ $$ = new UnaryOperationNode($2, $1); }
	| inheritexpr
	;

expr
	: unaryexpr								{ $$ = $1; }
	| expr relop expr %prec KW_EQ			{ $$ = new BinaryOperationNode($1, $3, $2); }
	| expr addop expr %prec KW_SUB			{ $$ = new BinaryOperationNode($1, $3, $2); }
	| expr mulop expr %prec KW_MUL			{ $$ = new BinaryOperationNode($1, $3, $2); }
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
	: basicliteral	{ $$ = $1; }
	| string_const	{ $$ = new StringLiteralNode($1);}
	;

basicliteral
	: CONST_INT		{ $$ = new IntLiteral(yyVal);}
	| CONST_BOOL	{ $$ = new BoolLiteral(yyVal);}
	| CONST_REAL	{ $$ = new RealLiteral(yyVal);}
	| CONST_NIL		{ $$ = new PointerLiteral();}
	;

discrete
	: CONST_INT		{ $$ = new IntLiteral(yyVal);}
	| CONST_CHAR	{ $$ = new CharLiteralNode(yyVal);}
	| CONST_BOOL	{ $$ = new BoolLiteral(yyVal);}
	;

string_const
	: CONST_STR					{ $$ = yyVal; }
	| CONST_CHAR				{ $$ = ""+yyVal; }
	| string_const CONST_STR	{ $$ = $1 + ""+yyVal; }
	| string_const CONST_CHAR	{ $$ = $1 + ""+yyVal; }
	;

id	: IDENTIFIER	{ $$ = new IdentifierNode(yyVal); }
	;

idlist
	: id				{ $$ = new IdentifierListNode($1, null); }
	| idlist COMMA id	{ $$ = new IdentifierListNode($3, $1); }
	;

qualid
	: id				{ $$ = $1; }
	| qualid KW_DOT id	{ $$ = new IdentifierNodeWithField($3.value, $1.value); }
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

rangetype			// must be const
	: rangestart KW_RANGE expr		{ $$ = new SetElement($1, $3); }
	;
	
	// best effort to support constant exprs. TODO improve
rangestart
	: simpleconst					{ $$ = $1; }
	| sign expr						{ $$ = $1; }
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

constsec
	: KW_CONST constdecl	{ $$ = new ConstDeclarationList($2, null); }
	| constsec constdecl	{ $$ = new ConstDeclarationList($2, $1); }
	;

constdecl
	: id KW_EQ constinitexpr  SEMICOL				{ $$ = new ConstDeclarationNode($1, null, $3); }			// true const
	| id COLON vartype KW_EQ constinitexpr SEMICOL	{ $$ = new ConstDeclarationNode($1, $3, $5); }	// typed const
	| id COLON proceduresign funcdir_noterm_opt functypeinit SEMICOL		{ $$ = null; /* TODO */ }
	;

constinitexpr
	: constexpr				{ $$ = $1; }
	| arrayconst			{ $$ = $1; }
	| recordconst			{ $$ = $1; }
	;

constexpr
	: expr					{ $$ = null; /* TODO */ }
	;
	

	// 1 or more exprs
arrayconst
	: LPAREN constexpr COMMA constinitexprlist RPAREN	{ $$ = new ExpressionListNode($2, $4); }
	;

constinitexprlist
	: constexpr								{ $$ = new ExpressionListNode($1, null); }
	| constinitexprlist COMMA constexpr		{ $$ = new ExpressionListNode($3, $1); }
	;

recordconst
	: LPAREN fieldconstlist RPAREN			{$$ = $2; }
	| LPAREN fieldconstlist SEMICOL RPAREN	{$$ = $2; }
	;

fieldconstlist
	: fieldconst							{ $$ = FieldInitList($1, null);}
	| fieldconstlist SEMICOL fieldconst		{ $$ = FieldInitList($3, $1);}
	;

fieldconst
	: id COLON constinitexpr				{ $$ = new FieldInit($1, $3); }
	;

simpleconst
	: discrete						{ $$ = null; /* TODO */ }
	| qualid						{ $$ = null; /* TODO */ }
	| id LPAREN casttype RPAREN		{ $$ = null; /* TODO */ }
	| id LPAREN literal RPAREN		{ $$ = null; /* TODO */ }
	;



	// ========================================================================
	// Records
	// ========================================================================
	
	// Only supports 'simple' structs, without class-like components

recordtypebasic
	: KW_RECORD KW_END						{ $$ = null; /* TODO */ }
	| KW_RECORD fieldlist semicolopt KW_END		{ $$ = null; /* TODO */ }
	;
	
recordtype
	: recordtypebasic		{ $$ = null; /* TODO */ }
	| KW_RECORD fieldlist SEMICOL variant_struct semicolopt KW_END		{ $$ = null; /* TODO */ }
	| KW_RECORD variant_struct semicolopt KW_END		{ $$ = null; /* TODO */ }
	;
	
variant_struct
	: KW_CASE id COLON ordinaltype KW_OF varfieldlist		{ $$ = null; /* TODO */ }
	| KW_CASE ordinaltype KW_OF varfieldlist		{ $$ = null; /* TODO */ }
	;

varfieldlist
	: varfield %prec LOWESTPREC		{ $$ = null; /* TODO */ }
	| varfield SEMICOL		{ $$ = null; /* TODO */ }
	| varfield SEMICOL varfieldlist		{ $$ = null; /* TODO */ }
	;

varfield
	: simpleconst COLON LPAREN variantlist semicolopt RPAREN		{ $$ = null; /* TODO */ }
	;
	
variantlist
	: variantvar		{ $$ = null; /* TODO */ }
	| variantlist SEMICOL variantvar		{ $$ = null; /* TODO */ }
	;

variantvar
	: objfield		{ $$ = null; /* TODO */ }
	| variant_struct		{ $$ = null; /* TODO */ }
	;
	

	
	
	// ========================================================================
	// Classes
	// ========================================================================

	// Objects are treated as classes
classtype
	: class_keyword heritage class_struct KW_END	{ $$ = new ClassDefinition($1, $2, $3); }
	| class_keyword heritage						{ $$ = new ClassDefinition($1, $2, null); } // forward decl			
	;

class_keyword
	: KW_CLASS		{ $$ = ClassType.Class; }
	| KW_OBJECT		{ $$ = ClassType.Object; }
	;

heritage
	:						{ $$ = null; /* TODO */ }
	| LPAREN idlist RPAREN	{ $$ = $2; }		// inheritance from class and interf(s)			
	;

class_struct
	: fieldlist SEMICOL complist scopeseclist		{ $$ = new ClassStruct(Scope.Public, $3, $4);  }
	|					complist scopeseclist		{ $$ = new ClassStruct(Scope.Public, $1, $2);  }
	;

scopeseclist
	:							{ $$ = null; }
	| scopeseclist scopesec		{ $$ = new ClassContentList($2, $1);  }
	;

scopesec
	: scope_decl fieldlist SEMICOL complist			{ $$ = new ClassStruct($1, $2, $4);  }
	| scope_decl				   complist			{ $$ = new ClassStruct($1, null, $2);  }
	;
	
scope_decl
	: KW_PUBLISHED			{ $$ = Scope.Published; }
	| KW_PUBLIC				{ $$ = Scope.Public; }
	| KW_PROTECTED			{ $$ = Scope.Protected; }
	| KW_PRIVATE			{ $$ = Scope.Private; }
	;

fieldlist
	: objfield					{ $$ = $1; }
	| fieldlist SEMICOL objfield { $$ = new ClassFieldList($3, $1); }
	;
	
complist
	:							{ $$ = null; }
	| complist class_comp		{ $$ = new ClassContentList($2, $1); }
	;

objfield
	: idlist COLON vartype		{ $$ = new VarDeclarationNode($1, $3, null); }
	| idlist COLON procsignfield	{ $$ = null; /* TODO */ }
//	| idlist COLON proceduresign funcdir_noterm_opt
	;
	
class_comp
	: staticclassopt procdefproto	{ $2.isStatic = $1; $$ = $2; }
	| property						{ $$ = $1; }
	;

interftype
	: KW_INTERF heritage guid classmethodlistopt classproplistopt KW_END	{ $$ = new InterfaceDefinition($2, $4, $5); }
	| KW_INTERF heritage classmethodlistopt classproplistopt KW_END			{ $$ = new InterfaceDefinition($2, $3, $4); }
	| KW_INTERF heritage %prec LOWESTPREC									{ $$ = null; /* TODO */ }
	;

guid
	: LBRAC string_const RBRAC		{ $$ = null; /* TODO */ }
	| LBRAC qualid RBRAC			{ $$ = null; /* TODO */ }
	;

classmethodlistopt
	: methodlist		{ $$ = $1; }
	|					{ $$ = null; }
	;

methodlist
	: procdefproto					{ $$ = new ClassContentList(new ClassMethod($1), null); }
	| methodlist procdefproto		{ $$ = new ClassContentList(new ClassMethod($2), $1); }
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
	| KW_PROPERTY id propinterfopt COLON funcrettype propspecifiers SEMICOL defaultdiropt { $$ = new ClassProperty($2, $5, $6, $3, $8); }
	;

defaultdiropt
	:				{ $$ = null; /* TODO */ }
	| id SEMICOL	{ $$ = new PropertyDefault($1); }	// id == DEFAULT
	;

propinterfopt
	:									{ $$ = null; }
	| LBRAC idlisttypeidlist RBRAC		{ $$ = null; /* TODO */ }
	;
	
idlisttypeidlist
	: idlisttypeid								{ $$ = null; /* TODO */ }
	| idlisttypeidlist SEMICOL idlisttypeid		{ $$ = null; /* TODO */ }
	;

idlisttypeid
	: idlist COLON funcparamtype		{ $$ = null; /* TODO */ }
	| KW_CONST idlist COLON funcparamtype		{ $$ = null; /* TODO */ }
	;


	// Properties directive: emitted as keywords caught from within a lexical scope

propspecifiers
	: indexspecopt readacessoropt writeacessoropt storedspecopt defaultspecopt implementsspecopt		{ $$ = null; /* TODO */ }
	;

indexspecopt
	:							{ $$ = null; /* TODO */ }
	| KW_INDEX CONST_INT		{ $$ = null; /* TODO */ }
	;

storedspecopt
	:							{ $$ = null; /* TODO */ }
	| KW_STORED id				{ $$ = null; /* TODO */ }
	;

defaultspecopt
	:							{ $$ = null; /* TODO */ }
	| KW_DEFAULT literal		{ $$ = null; /* TODO */ }
	| KW_NODEFAULT				{ $$ = null; /* TODO */ }
	;

implementsspecopt
	:							{ $$ = null; /* TODO */ }
	| KW_IMPLEMENTS id			{ $$ = null; /* TODO */ }
	;

readacessoropt
	:							{ $$ = null; /* TODO */ }
	| KW_READ	id				{ $$ = null; /* TODO */ }
	;

writeacessoropt
	:							{ $$ = null; /* TODO */ }
	| KW_WRITE	id				{ $$ = null; /* TODO */ }
	;

	
	
	// ========================================================================
	// Types
	// ========================================================================

typedecl
	: id KW_EQ typeopt vartype  SEMICOL							{ $$ = new TypeDeclarationNode($1, $4); }
	| id KW_EQ typeopt proceduresign  SEMICOL funcdirectopt		{ $$ = new proceduresignDeclarationNode($1, $4, $6); }
	| id KW_EQ typeopt packcomptype SEMICOL						{ $$ = null; /* TODO */ }
	;

typeopt
	:								{ $$ = true; }
	| KW_TYPE						{ $$ = true; }
	;

vartype
	: simpletype					{ $$ = $1; }
	| enumtype						{ $$ = $1; }
	| rangetype						{ $$ = $1; }
	| refpointertype				{ $$ = $1; }
	// metaclasse
	| classreftype					{ $$ = $1; }
	| packstructtype				{ $$ = $1; }
	;

packcomptype
	: KW_PACKED compositetype		{ $$ = null; /* TODO */ }
	| compositetype		{ $$ = null; /* TODO */ }
	;

compositetype
	: classtype								{ $$ = $1; }
	| interftype							{ $$ = $1; }
	;

classreftype
	: KW_CLASS KW_OF scalartype		{ $$ = new ClassType($3); }
	;

simpletype
	: scalartype					{ $$ = $1; }
	| realtype						{ $$ = $1; }
	| stringtype					{ $$ = $1; }
	| varianttype					{ $$ = $1; }
	| TYPE_PTR						{ $$ = new PointerType(null); }
	;

ordinaltype
	: enumtype					{ $$ = $1; }
	| rangetype					{ $$ = $1; }
	| scalartype				{ $$ = $1; }
	;

scalartype
	: inttype								{ $$ = $1; }
	| chartype								{ $$ = $1; }
	| qualid								{ $$ = $1; }
	;

realtype
	: TYPE_REAL48			{ $$ = new DoubleType(); }
	| TYPE_FLOAT			{ $$ = new FloatType(); }
	| TYPE_DOUBLE			{ $$ = new DoubleType(); }
	| TYPE_EXTENDED			{ $$ = new ExtendedType(); }
	| TYPE_CURR				{ $$ = new CurrencyType(); }
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
	: TYPE_STR	/* dynamic size	*/	{ $$ = new StringType(null); }
	| TYPE_PCHAR					{ $$ = new StringType(null); }
	| TYPE_STR LBRAC expr RBRAC		{ $$ = new StringType($3); }
	| TYPE_SHORTSTR					{ $$ = new StringType(null); }
	| TYPE_WIDESTR					{ $$ = new StringType(null); }
	;

varianttype
	: TYPE_VAR			{ $$ = new VariantType(); }
	| TYPE_OLEVAR		{ $$ = new VariantType(); }
	;

packstructtype
	: structuredtype		{ $$ = null; /* TODO */ }
	| KW_PACKED structuredtype		{ $$ = null; /* TODO */ }
	;

structuredtype
	: arraytype			{ $$ = $1; }
	| settype			{ $$ = $1; }
	| filetype			{ $$ = $1; }
	| recordtype		{ $$ = $1; }
	;
	
arraysizelist
	: rangetype								{ $$ = new ArraySizeList($1, null); }
	| arraysizelist COMMA rangetype			{ $$ = new ArraySizeList($3, $1); }
	;

arraytype
	: TYPE_ARRAY LBRAC arraytypedef RBRAC KW_OF vartype 	{ $$ = new ArrayType($3, $6); }
	| TYPE_ARRAY KW_OF vartype 	{ $$ = new ArrayType(null, $3); }
	;

arraytypedef
	: arraysizelist		{ $$ = $1; }
	| inttype			{ $$ = new ArrayTypeList($1); }
	| chartype			{ $$ = new ArrayTypeList($1); }
	| qualid			{ $$ = new ArrayTypeList($1); }
	;

settype
	: TYPE_SET KW_OF ordinaltype 	{ $$ = new SetType($3); }
	;

filetype
	: TYPE_FILE KW_OF vartype 		{ $$ = new FileType($3); }
	| TYPE_FILE						{ $$ = new FileType(null); }
	;

refpointertype
	: KW_DEREF vartype 				{ $$ = new PointerType($2); }
	;

funcparamtype
	: simpletype					{ $$ = $1; }
	| TYPE_ARRAY KW_OF simpletype	{ $$ = new ArrayType(null, $3); }
	;

funcrettype
	: simpletype		{ $$ = $1; }
	;
	
	// simpletype w/o user-defined types
casttype
	: inttype			{ $$ = $1; }
	| chartype			{ $$ = $1; }
	| realtype			{ $$ = $1; }
	| stringtype		{ $$ = $1; }
	| TYPE_PTR			{ $$ = new PointerType(); }
	;


%%

	}	// close parser class, opened in prolog	

// already defined in template
//} // close outermost namespace
