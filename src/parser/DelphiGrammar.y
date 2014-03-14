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
		
		
		// Internal helpers
		
		string lastObjectName = null;	// keeps track of current class/object/interface being parsed
		
		void ListAdd(ref NodeList headlst, NodeList bodylst, Node elem)
		{
			bodylst.Add(elem);
			headlst = bodylst;
		}
		
		BinaryExpression CreateBinaryExpression(Expression e1, int token, Expression e2)
		{
			switch(token)
			{
				case Token.KW_MUL	:	return new Product    (e1, e2);
				case Token.KW_DIV	:	return new Division   (e1, e2);
				case Token.KW_QUOT	:	return new Quotient   (e1, e2);
				case Token.KW_MOD	:	return new Modulus    (e1, e2);
				case Token.KW_SHR	:	return new ShiftRight (e1, e2);
				case Token.KW_SHL	:	return new ShiftLeft  (e1, e2);
				case Token.KW_AND	:	return new LogicalAnd (e1, e2);
				case Token.KW_SUB	:	return new Subtraction(e1, e2);
				case Token.KW_SUM	:	return new Addition   (e1, e2);
				case Token.KW_OR 	:	return new LogicalOr  (e1, e2);
				case Token.KW_XOR	:	return new LogicalXor (e1, e2);
				case Token.KW_EQ	:	return new Equal      (e1, e2);
				case Token.KW_NE	:	return new NotEqual   (e1, e2);
				case Token.KW_LT	:	return new LessThan   (e1, e2);
				case Token.KW_LE	:	return new LessOrEqual(e1, e2);
				case Token.KW_GT	:	return new GreaterThan(e1, e2);
				case Token.KW_GE	:	return new GreaterOrEqual(e1, e2);
				default: throw ParserException("Invalid Binary Operation token"); 	// should never happen
			}
		}

%}


	// ==============================================================
	// Rules declarations
	// ==============================================================

%start goal
	// file type

%type<string> id labelid
%type<bool>  ofobjectopt
%type<GoalNode> goal file
%type<ProgramNode> program
%type<LibraryNode> library
%type<UnitNode> unit
%type<PackageNode> package
%type<UsesNode> requiresclause containsclause usesclauseopt useidlst
%type<Identifier> useid externarg qualifname
%type<ImplementationSection> implsec
%type<InterfaceSection> interfsec

%type<NodeList> interfdecllst maindecllst declseclst formalparams formalparamslst constsec
%type<NodeList> funcdirectopt funcdir_noterm_opt  stmtlst metdirectopt 

%type<CallableDirectives> metdirectlst funcdirectlst routinedecldirs
%type<NodeList> caseselectorlst caselabellst onlst recfieldlst propfield
%type<NodeList> scopeseclst complst classmethodlstopt methodlst classproplstopt classproplst fieldlst 
%type<NodeList> propspecifiers constinitexprlst recvarlst rscstringlst propfieldlst
%type<NodeList> idlst heritage exprlst exprlstopt 
%type<NodeList> setelemlst arrayconst recordconst fieldconstlst arrayszlst arraytypedef 

%type<Section> initsec finalsec
%type<ProgramBody> main_block
%type<Declaration> interfdecl maindeclsec funcdeclsec basicdeclsec typesec labeldeclsec labelidlst  varsec thrvarsec vardecllst vardecl constdecl typedecl methoddecl varfuncptr
%type<ExportItem> exportsec	 expitemlst expitem
%type<RoutineDefinition> routinedef methodroutinedef routinedefunqualif
%type<RoutineDeclaration> proceduraltype proctypefield routineproto methodproto routinedeclext routinedecl
%type<RoutineBody> funcdefine funcblock
%type<TypeNode> funcrettype scalartype funcparamtype paramtypeopt paramtypespec  funcret
%type<ParamterNode> formalparm
%type<Expression> paraminitopt expr rangetype  rangestart constexpr functypeinit
%type<int> funcdirective  funcqualif funcdeprecated directinterfopt metdirective metqualif importdirforced importdiropt
%type<CallConventionNode> routinecallconv
%type<BlockStatement> block
%type<Statement> stmt nonlbl_stmt assign goto_stmt ifstmt casestmt else_case repeatstmt whilestmt forstmt withstmt tryexceptstmt tryfinallystmt raisestmt assemblerstmt asmcode
%type<CaseSelector> caseselector
%type<CaseLabel> caselabel
%type<ExceptionBlock> exceptionblock
%type<OnStatement> ondef
%type<VarDeclarationOption> vardeclopt
%type<RoutineCall> routinecall
%type<LvalueExpression> lvalue
%type<Literal> literal discrete stringconst
%type<NodeList> enumtype enumelemlst
%type<FieldInit> enumelem fieldconst
%type<SetElement> setelem
 
%type<ClassDefinition> classtype
%type<ClassType> classkeyword
%type<ClassBody> scopesec
%type<ClassContent> classcomp
%type<Scope> scope_decl
%type<VarDeclaration> objfield
%type<InterfaceDefinition> interftype
%type<ClassProperty> property
%type<bool> typeopt 
%type<TypeNode> vartype metaclasstype scalartype ordinaltype casttype
%type<TypeNode> packstructtype packcomptype compositetype
%type<TypeNode> integraltype realtype inttype chartype stringtype varianttype funcrettype 
%type<TypeNode> arraytype settype filetype pointertype funcparamtype  structuredtype

%type<PropertySpecifier> propinterfopt defaultdiropt indexopt storedopt defaultopt implopt readopt writeopt
%type<Expression> unaryexpr constinitexpr inheritedexpr inheritedcall basicliteral rangestart functypeinit set
%type<RecordNode> recordtype recordtypebasic

%type<Node> recvariant  recfield  recvar guid rscstring classbody rscstringsec 
%type<int> sign addop mulop relop
%type<int> KW_EQ KW_GT KW_LT KW_LE KW_GE KW_NE KW_IN KW_IS KW_SUM KW_SUB KW_OR KW_XOR KW_MUL KW_DIV KW_QUOT KW_MOD KW_SHL KW_SHR KW_AS KW_AND

	
	
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
%nonassoc KW_RANGE COMMA COLON SCOL KW_ASSIGN
	// relational/comparative
%left KW_EQ KW_GT KW_LT KW_LE KW_GE KW_NE KW_IN KW_IS
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

scolopt
	:
	| SCOL
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
	: KW_PROGRAM id SCOL	usesclauseopt main_block	{ $$ = new ProgramNode($2, $4, $5); }
	| 						usesclauseopt main_block	{ $$ = new ProgramNode("untitled", $1, $2); }
	;

library
	: KW_LIBRARY id SCOL usesclauseopt main_block	{ $$ = new LibraryNode($2, $4, $5); }
	;

package
	: id id SCOL requiresclause containsclause KW_END	{ $$ = new PackageNode($2, $4, $5); }
	;

requiresclause
	: id idlst SCOL	{ $$ = new UsesNode($1, $2);}	// check that id == "Requires"
	;

containsclause
	: id idlst SCOL	{ $$ = new UsesNode($1, $2);}	// check that id == "Contains"
	;

usesclauseopt
	:							{ $$ = new EmptyNode(); }
	| KW_USES useidlst SCOL		{ $$ = $2; }
	;

useidlst
	: useid						{ $$ = new NodeList($1); }
	| useidlst COMMA useid		{ ListAdd($$, $1, $3); }
	;
	
useid
	: id						{ $$ = new UsesNode($1);}
	| id KW_IN stringconst		{ $$ = new UsesNode($1, $3);}
	;

unit
	: KW_UNIT id SCOL interfsec implsec initsec finalsec KW_END  { $$ = new UnitNode($2, $4, $5, $6); }
	;

implsec
	: KW_IMPL usesclauseopt	maindecllst		{ $$ = new ImplementationSection($2, $3);}
	| KW_IMPL usesclauseopt					{ $$ = new ImplementationSection($2, null);}
	;

interfsec
	: KW_INTERF usesclauseopt interfdecllst	{ $$ = new InterfaceSection($2, $3);}
	;

interfdecllst
	:											{ $$ = new NodeList();}
	| interfdecllst interfdecl					{ ListAdd($$, $1, $2); }
	;

initsec
	: KW_INIT  stmtlst							{ $$ = new InitializationSection($2);}
	| KW_BEGIN stmtlst							{ $$ = new InitializationSection($2);}
	;
	
finalsec
	: KW_FINALIZ stmtlst 						{ $$ = new FinalizationSection($2);}
	;
	
main_block
	: maindecllst block			{ $$ = new ProgramBody($1, $2);}
	|			  block			{ $$ = new ProgramBody(null, $1);}
	;
	
maindecllst
	: maindeclsec				{ $$ = new NodeList($1);}
	| maindecllst maindeclsec	{ ListAdd($$, $1, $2); }
	;

declseclst
	: funcdeclsec				{ $$ = new NodeList($1);}
	| declseclst funcdeclsec	{ ListAdd($$, $1, $2); }
	;

	

	// ========================================================================
	// Declaration sections
	// ========================================================================

interfdecl
	: basicdeclsec			{ $$ = $1;}
	| routinedecl			{ $$ = $1;}
	| thrvarsec				{ $$ = $1;}
	| rscstringsec			{ $$ = $1;}
	;

maindeclsec
	: basicdeclsec			{ $$ = $1;}
	| thrvarsec				{ $$ = $1;}
	| exportsec				{ $$ = $1;}
	| routinedeclext		{ $$ = $1;}
	| routinedef			{ $$ = $1;}
	| labeldeclsec			{ $$ = $1;}
	;

funcdeclsec
	: basicdeclsec			{ $$ = $1;}
	| labeldeclsec			{ $$ = $1;}
	| routinedefunqualif	{ $$ = $1;}
	;

basicdeclsec
	: constsec				{ $$ = $1;}
	| typesec				{ $$ = $1;}
	| varsec				{ $$ = $1;}
	;

typesec
	: KW_TYPE typedecl		{ $$ = new NodeList($2); }
	| typesec typedecl		{ ListAdd($$, $1, $2); }
	;

	
	// labels
	
labeldeclsec
	: KW_LABEL labelidlst SCOL		{$$ = new LabelDeclaration($2); }
	;
	
labelidlst 
	: labelid						{ $$ = new List($1); }
	| labelidlst COMMA labelid		{ $$ = $1; $1.Add($3); }
	;

labelid
	: CONST_INT 					{ 	/* decimal int 0..9999 */
										if (yyVal < 0 || yyVal > 9999)
											yyerror("Label number must be between 0 and 9999")
										$$ = ""+yyVal;
									}
	| id							{ $$ = $1; }
	;

	// Exports

exportsec	
	: KW_EXPORTS expitemlst			{ $$ = $2; }
	;

expitemlst
	: expitem						{ $$ = $1; }
	| expitemlst COMMA expitem		{ $$ = $1; }
	;

expitem
	: id							{ $$ = new ExportItem($1, null, null); }
	| id KW_NAME  stringconst		{ $$ = new ExportItem($1, $3, null); }
	| id KW_INDEX expr				{ $$ = new ExportItem($1, null, $3); }
	;
	



	// ========================================================================
	// Functions
	// ========================================================================

	// Prototypes/signatures
	// proc proto for definitions or external/forward decls
	// check that funcrecopt is null for every kind except FUNCTION

routinedef
	: routinedefunqualif								{ $$ = $1; }
	| methodroutinedef	funcdefine SCOL 				{ $$ = $1; }
	;
	
methodroutinedef
	: methodproto SCOL metdirectopt 					{ $$ = $1; $1.Directives = $3; }
	| KW_CLASS methodproto SCOL metdirectopt 			{ $$ = $2; $2.Directives = $4; $2.isStatic = true; }
	| KW_CONSTRUCTOR id KW_DOT id formalparams SCOL		{ $$ = new ConstructorDeclaration($2, $4, $5); }
	| KW_DESTRUCTOR  id KW_DOT id formalparams SCOL		{ $$ = new DestructorDeclaration ($2, $4, $5); }
	;
	
methodproto
	: KW_FUNCTION  id KW_DOT id formalparams funcret 	{ $$ = new MethodDeclaration($2, $4, $5, $6); }
	| KW_PROCEDURE id KW_DOT id formalparams 			{ $$ = new MethodDeclaration($2, $4, $5); }
	;
	
	// global routine definition
routinedefunqualif
	: routineproto funcdirectopt funcdefine SCOL		{  $$.Directives = $2; $$ = new RoutineDefinition($1, $3); }
	;
	
	// routine decl for interface sections
routinedecl
	: routineproto importdiropt			{ $$ = $1; $1.Directives = $2; }
	;

	// routine decl for implementation sections, needs an external/forward
routinedeclext
	: routineproto importdirforced		{ $$ = $1; $1.Directives = $2; }
	;

methoddecl
	: KW_FUNCTION    id formalparams funcret SCOL metdirectopt  { $$ = new MethodDeclaration(lastObjectName, $2, $3, $4, $6); }
	| KW_PROCEDURE   id formalparams         SCOL metdirectopt  { $$ = new MethodDeclaration(lastObjectName, $2, $3, null, $5); }
	| KW_CONSTRUCTOR id formalparams SCOL						{ $$ = new DestructorDeclaration(lastObjectName, $2, $3); }
	| KW_DESTRUCTOR  id formalparams SCOL						{ $$ = new DestructorDeclaration(lastObjectName, $2, $3); }
	;

routineproto
	: KW_FUNCTION  id formalparams funcret	SCOL	{ $$ = new RoutineDeclaration($2, $3, $4); }
	| KW_PROCEDURE id formalparams 			SCOL	{ $$ = new RoutineDeclaration($2, $3); }
	;	
	

proceduraltype			
	: KW_PROCEDURE formalparams 						{ $$ = new ProceduralType($2); } 
	| KW_FUNCTION  formalparams funcret					{ $$ = new ProceduralType($2, $3); } 
	| KW_PROCEDURE formalparams KW_OF KW_OBJECT			{ $$ = new MethodType($2); } 
	| KW_FUNCTION  formalparams funcret KW_OF KW_OBJECT	{ $$ = new MethodType($2, $3); } 
	;

proctypefield
	: proceduraltype					{ $$ = $1; }
	| proceduraltype routinecallconv		{ $$ = $1; $1.Directives.Callconv = $2; }
	;

funcret
	: COLON funcrettype					{ $$ = $2;}
	;



	// Function blocks and parameters

funcdefine
	: declseclst funcblock				{ $$ = new RoutineBody($1, $2); }
	| 			 funcblock				{ $$ = new RoutineBody(null, $1); }
	;

funcblock
	: block								{ $$ = $1; }
	| assemblerstmt						{ $$ = $1; }
	;

formalparams
	:									{ $$ = new ParameterList(); }
	| LPAREN RPAREN						{ $$ = new ParameterList(); }
	| LPAREN formalparamslst RPAREN		{ $$ = $2; }
	;

formalparamslst
	: formalparm						{ $$ = new ParameterList($1); }
	| formalparamslst SCOL formalparm	{ ListAdd($$, $1, $3); }
	;

formalparm
	: KW_VAR	idlst paramtypeopt					{ $$ = new VarParamDeclaration($2, $3); } 
	| KW_OUT	idlst paramtypeopt					{ $$ = new OutParamDeclaration($2, $3); } 
	| 			idlst paramtypespec paraminitopt	{ $$ = new ParamDeclaration($1, $2, $3); }
	| KW_CONST	idlst paramtypeopt  paraminitopt	{ $$ = new ConstParamDeclaration($2, $3, $4); }
	;

paramtypeopt
	:									{ $$ = null; }
	| paramtypespec						{ $$ = $1; }
	;

paramtypespec
	: COLON funcparamtype				{ $$ = $2; }
	;

paraminitopt
	:									{ $$ = null; }
	| KW_EQ constexpr					{ $$ = $2; }
	;

functypeinit
	: KW_EQ id 							{ $$ = new ConstExpression(new Identifier($2)); }
	| KW_EQ CONST_NIL					{ $$ = new PointerLteral(yyVal); }
	;

	
	// Function directives

importdiropt
	: funcdirectopt						{ $$ = $1; }
	| importdirforced					{ $$ = $1; }
	;

importdirforced
	: funcdirectopt KW_EXTERNAL externarg  funcdirectopt	{ $$ = $1; $1.Add($4); $1.Importdir = ImportDirective.External; $1.External = $3}
	| funcdirectopt KW_FORWARD  funcdirectopt				{ $$ = $1; $1.Add($3); $1.Importdir = ImportDirective.Forward; }
	;

externarg
	: constexpr KW_NAME constexpr		{ $$ = new ExternalDirective($1, $3); }
	| constexpr							{ $$ = new ExternalDirective($1); }
	|            						{ $$ = null; }
	;

funcdir_noterm_opt
	:									{ $$ = null	; }
	| funcdirectlst						{ $$ = $1	; }
	;

funcdirectopt
	:									{ $$ = null	; }
	| funcdirectlst SCOL				{ $$ = $1	; }
	;

metdirectopt
	:									{ $$ = null	; }
	| metdirectlst SCOL					{ $$ = $1	; }
	;

funcdirectlst
	: funcdirective						{ $$ = new RoutineDirectives(); }
	| funcdirectlst SCOL funcdirective	{ $1.AddDirective($3); $$ = $1; }
	;

metdirectlst
	: metdirective						{ $$ = new MethodDirectives(); }
	| metdirectlst SCOL metdirective	{ $1.AddDirective($3); $$ = $1; }
	;

metdirective
	: funcdirective			{ $$ = $1; }
	| metqualif				{ $$ = $1; }
	;
	
funcdirective
	: funcqualif			{ $$ = $1; }
	| routinecallconv			{ $$ = $1; }
	| funcdeprecated		{ $$ = $1; }
	;

funcdeprecated
	: KW_FAR				{ $$ = RoutineDirectiveDeprecated.Far; }
	| KW_NEAR				{ $$ = RoutineDirectiveDeprecated.Near; }
	| KW_RESIDENT			{ $$ = RoutineDirectiveDeprecated.Resident; }
	;

metqualif
	: KW_ABSTRACT			{ $$ = MethodDirective.Abstract; }
	| KW_DYNAMIC			{ $$ = MethodDirective.Dynamic; }
	| KW_OVERRIDE			{ $$ = MethodDirective.Override; }
	| KW_VIRTUAL			{ $$ = MethodDirective.Virtual; }
	| KW_REINTRODUCE		{ $$ = MethodDirective.Reintroduce; }
	;

funcqualif
	: KW_ASSEMBLER			{ $$ = GeneralDirective.Assembler; }
	| KW_EXPORT				{ $$ = GeneralDirective.Export; }
	| KW_INLINE				{ $$ = GeneralDirective.Inline; }
	| KW_OVERLOAD			{ $$ = GeneralDirective.Overload; }
	| KW_VARARGS			{ $$ = GeneralDirective.VarArgs; }
	;
	
routinecallconv
	: KW_PASCAL				{ $$ = CallConvention.Pascal; }
	| KW_SAFECALL			{ $$ = CallConvention.SafeCall; }
	| KW_STDCALL			{ $$ = CallConvention.StdCall; }
	| KW_CDECL				{ $$ = CallConvention.CDecl; }
	| KW_REGISTER			{ $$ = CallConvention.Register; }
	;
	

	
	// ========================================================================
	// Statements
	// ========================================================================

block
	: KW_BEGIN stmtlst KW_END		{ $$ = $2; }
	;

stmtlst
	: stmt							{ $$ = new StatementList($1); }
	| stmt SCOL stmtlst			{ ListAdd($$, $3, $1); }
	;

stmt
	: nonlbl_stmt					{ $$ = $1; }
	| labelid COLON nonlbl_stmt		{ $$ = new LabelStatement($1, $3); }
	;

nonlbl_stmt
	:						{ $$ = new EmptyStatement(); }
	| inheritedexpr			{ $$ = new ExpressionStatement($1); }
	| routinecall			{ $$ = new ExpressionStatement($1); }
	| assign				{ $$ = $1; }
	| goto_stmt				{ $$ = $1; }
	| block					{ $$ = $1; }
	| ifstmt				{ $$ = $1; }
	| casestmt				{ $$ = $1; }
	| repeatstmt			{ $$ = $1; }
	| whilestmt				{ $$ = $1; }
	| forstmt				{ $$ = $1; }
	| withstmt				{ $$ = $1; }
	| tryexceptstmt			{ $$ = $1; }
	| tryfinallystmt		{ $$ = $1; }
	| raisestmt				{ $$ = $1; }
	| assemblerstmt			{ $$ = $1; }
	| KW_BREAK				{ $$ = new BreakStatement(); }
	| KW_CONTINUE			{ $$ = new ContinueStatement(); }
	;

assign		// TODO reuse this rule for initializations
	: lvalue KW_ASSIGN expr					{ $$ = new Assignement($1, $3); }
	;

goto_stmt
	: KW_GOTO labelid	{ $$ = new GotoStatement($2); }
	;

ifstmt
	: KW_IF expr KW_THEN nonlbl_stmt KW_ELSE nonlbl_stmt		{ $$ = new IfStatement($2, $4, $6); }
	| KW_IF expr KW_THEN nonlbl_stmt							{ $$ = new IfStatement($2, $4, null); }
	;

casestmt
	: KW_CASE expr KW_OF caseselectorlst else_case KW_END	{ $$ = new CaseStatement($2, $4, $5); }
	;

else_case
	:								{ $$ = null;}	
	| KW_ELSE nonlbl_stmt			{ $$ = $2; }
	| KW_ELSE nonlbl_stmt SCOL	{ $$ = $2; }
	;

caseselectorlst
	: caseselector								{ $$ = new NodeList($1); }
	| caseselectorlst SCOL caseselector		{ ListAdd($$, $1, $3); }
	;

caseselector
	:										{ $$ = null; }
	| caselabellst COLON nonlbl_stmt		{ $$ = new CaseSelector($1, $3); }
	;
	
caselabellst
	: caselabel							{ $$ = new NodeList($1); }
	| caselabellst COMMA caselabel		{ ListAdd($$, $1, $3); }
	;

	// the labels must be constant
caselabel
	: setelem							{ $$ = new CaseLabel($1); }
	;

repeatstmt
	: KW_REPEAT stmtlst KW_UNTIL expr	{ $$ = new RepeatLoop($2, $4); }
	;

whilestmt
	: KW_WHILE expr KW_DO nonlbl_stmt	{ $$ = new WhileLoop($2, $4); }
	;

forstmt
	: KW_FOR id KW_ASSIGN expr KW_TO	 expr KW_DO nonlbl_stmt	{ $$ = new ForLoop($2, $4, $6, $8, 1); }
	| KW_FOR id KW_ASSIGN expr KW_DOWNTO expr KW_DO nonlbl_stmt { $$ = new ForLoop($2, $4, $6, $8, -1); }
	;

	// expr must yield a ref to a record, object, class, interface or class type
withstmt
	: KW_WITH exprlst KW_DO nonlbl_stmt		{ $$ = new WithStatement($2, $4); }
	;

tryexceptstmt
	: KW_TRY stmtlst KW_EXCEPT exceptionblock KW_END	{ $$ = new TryExceptStatement($2, $4); }
	;

exceptionblock
	: onlst KW_ELSE stmtlst						{ $$ = new ExceptionBlock($1, $3); }
	| onlst										{ $$ = new ExceptionBlock($1, null); }
	| stmtlst									{ $$ = $1; }
	;

onlst
	: ondef											{ $$ = new StatementList($1); }
	| onlst ondef									{ ListAdd($$, $1, $2); }
	;

ondef
	: KW_ON id COLON id KW_DO nonlbl_stmt SCOL	{ $$ = new OnStatement($2, $4, $6); }
	| KW_ON 		 id KW_DO nonlbl_stmt SCOL	{ $$ = new OnStatement(null, $2, $4); }
	;

tryfinallystmt
	: KW_TRY  stmtlst KW_FINALLY stmtlst KW_END	{ $$ = new TryFinallyStatement($2, $4); }
	;

raisestmt
	: KW_RAISE							{ $$ = new RaiseStatement(null, null); }
	| KW_RAISE lvalue					{ $$ = new RaiseStatement($2, null); }
	| KW_RAISE			KW_AT expr		{ $$ = new RaiseStatement(null, $3); }
	| KW_RAISE lvalue	KW_AT expr		{ $$ = new RaiseStatement($2, $4); }
	;

assemblerstmt
	: KW_ASM asmcode KW_END		{ $$ = new AssemblerBlock($2); }
	;

asmcode
	: 							{ $$ = null; }
	| asmcode ASM_OP			{ $$ = null; }
	;





	// ========================================================================
	// Variables
	// ========================================================================

varsec
	: KW_VAR vardecllst		{ $$ = $2; }
	;
	
thrvarsec
	: KW_THRVAR vardecllst		{ $$ = $2; }
	;

vardecllst
	: vardecl					{ $$ = new NodeList($1); }
	| vardecllst vardecl		{ ListAdd($$, $1, $2); }
	;

vardecl
	: idlst COLON vartype vardeclopt SCOL				{ $$ = new VarDeclaration($1, $3, $4); }
	| varfuncptr funcdirectopt							{ $$ = $1; $1.Directives.Add($2); }
	| varfuncptr funcdir_noterm_opt functypeinit SCOL	{ $$ = $1; $1.Directives.Add($2); $1.init = $3; }
	;

varfuncptr
	: idlst COLON proceduraltype SCOL		{ $$ = new VarDeclaration($1, $3); }
	;

vardeclopt
	: 										{ $$ = null; }
	| KW_ABSOLUTE id  						{ $$ = new VariableAbsoluteNode($2); }
	| KW_EQ constinitexpr 					{ $$ = new VariableInitNode($2); }
	;

	// Resourcestrings fom windows
	
rscstringsec
	: TYPE_RSCSTR rscstringlst				{ $$ = $2; }
	;
	
rscstringlst
	: rscstring								{ $$ = new NodeList($1); }
	| rscstringlst rscstring				{ ListAdd($$, $1, $2); }
	;
	
rscstring
	:  id KW_EQ stringconst SCOL			{ $$ = new ConstDeclaration($1, null, $3); }
	;


	
	// ========================================================================
	// Expressions
	// ========================================================================

inheritedexpr
	: KW_INHERITED inheritedcall			{ $$ = new InheritedCall($2); }
	;
	
inheritedcall
	: 										{ $$ = null; }
	| id									{ $$ = new RoutineCall($1	); }
	| id LPAREN exprlstopt RPAREN			{ $$ = new RoutineCall($1,$3); }
	;
	
	// routine call to be called as statement
routinecall
	: id									{ $$ = new RoutineCall($1); }
	| lvalue LPAREN exprlstopt RPAREN		{ $$ = new RoutineCall($1, $3); }
	| lvalue KW_DOT id						{ $$ = new FieldAcess($1, $3); }
	;
	
lvalue	// lvalue
	: id									{ $$ = new Identifier($1); }
	| lvalue LPAREN exprlstopt RPAREN		{ $$ = new RoutineCall($1, $3); }
	| lvalue LPAREN casttype RPAREN			{ $$ = new RoutineCall($1, $3); }
	| lvalue KW_DOT id						{ $$ = new FieldAcess($1, $3); }
	| lvalue KW_DEREF						{ $$ = new PointerDereference($1); }
	| lvalue LBRAC exprlst RBRAC			{ $$ = new ArrayAccess($1, $3); }
	| stringconst LBRAC expr RBRAC			{ $$ = new ArrayAccess($1, $3); }
	| LPAREN expr RPAREN					{ $$ = $2; }
	;

unaryexpr
	: literal								{ $$ = $1; }
	| lvalue								{ $$ = $1; }
	| set									{ $$ = $1; }
	| casttype LPAREN exprlstopt RPAREN		{ $$ = new TypeCast($1, $3); }
	| KW_ADDR unaryexpr						{ $$ = new AddressLvalue($2); }
	| KW_NOT unaryexpr						{ $$ = new LogicalNot($2); }
	| KW_SUM unaryexpr 						{ $$ = new UnaryPlus($1); }
	| KW_SUB unaryexpr 						{ $$ = new UnaryMinus($1); }
	| inheritedexpr							{ $$ = $1; }
	;

expr
	: unaryexpr								{ $$ = $1; }
	| expr KW_AS casttype					{ $$ = new TypeCast($1, $2); }
	| expr KW_IS casttype					{ $$ = new TypeIs($1,$2); }
	| expr KW_IN expr						{ $$ = new SetIn($1, $2); }
	| expr relop expr %prec KW_EQ			{ $$ = CreateBinaryExpression($1, $2, $3); }
	| expr addop expr %prec KW_SUB			{ $$ = CreateBinaryExpression($1, $2, $3); }
	| expr mulop expr %prec KW_MUL			{ $$ = CreateBinaryExpression($1, $2, $3); }
	;

sign
	: KW_SUB		{ $$ = $1; }
	| KW_SUM		{ $$ = $1; }
	;
mulop
	: KW_MUL		{ $$ = $1; }
	| KW_DIV		{ $$ = $1; }
	| KW_QUOT		{ $$ = $1; }
	| KW_MOD		{ $$ = $1; }
	| KW_SHR		{ $$ = $1; }
	| KW_SHL		{ $$ = $1; }
	| KW_AND		{ $$ = $1; }
	;
addop
	: KW_SUB		{ $$ = $1; }
	| KW_SUM		{ $$ = $1; }
	| KW_OR			{ $$ = $1; }
	| KW_XOR		{ $$ = $1; }
	;
relop
	: KW_EQ			{ $$ = $1; }
	| KW_NE			{ $$ = $1; }
	| KW_LT			{ $$ = $1; }
	| KW_LE			{ $$ = $1; }
	| KW_GT			{ $$ = $1; }
	| KW_GE			{ $$ = $1; }
	;


literal
	: basicliteral	{ $$ = $1; }
	| stringconst	{ $$ = new StringLiteral($1);}
	;

basicliteral
	: CONST_INT		{ $$ = new IntLiteral(yyVal);}
	| CONST_BOOL	{ $$ = new BoolLiteral(yyVal);}
	| CONST_REAL	{ $$ = new RealLiteral(yyVal);}
	| CONST_NIL		{ $$ = new PointerLiteral();}
	;

discrete
	: CONST_INT		{ $$ = new IntLiteral(yyVal);}
	| CONST_CHAR	{ $$ = new StringLiteral(""+yyVal);}
	| CONST_BOOL	{ $$ = new BoolLiteral(yyVal);}
	;

stringconst
	: CONST_STR					{ $$ = yyVal; }
	| CONST_CHAR				{ $$ = ""+yyVal; }
	| stringconst CONST_STR		{ $$ = $1 + ""+yyVal; }
	| stringconst CONST_CHAR	{ $$ = $1 + ""+yyVal; }
	;

id	: IDENTIFIER				{ $$ = yyVal; /* string */ }
	;

idlst
	: id						{ $$ = new List($1); }
	| idlst COMMA id			{ $1.Add($3); $$ = $1; }
	;

exprlst
	: expr						{ $$ = new NodeList($1); }
	| exprlst COMMA expr		{ ListAdd($$, $1, $3); }
	;

exprlstopt
	:							{ $$ = new NodeList(); }
	| exprlst					{ $$ = $1; }
	;


	
	// ========================================================================
	// Sets and Enums literals
	// ========================================================================

rangetype			// must be const
	: rangestart KW_RANGE expr		{ $$ = new SetRange($1, new ConstExpression($3)); }
	;
	
	// best effort to support constant exprs. TODO improve
rangestart
	: discrete						{ $$ = $1; }
//	| lvalue						{ $$ = null; /* TODO */ }
	| sign expr						{ $$ = $1; }
	;

enumtype
	: LPAREN enumelemlst RPAREN		{ $$ = new EnumDeclaration($2); }
	;

enumelemlst
	: enumelem						{ $$ = new NodeList($1); }
	| enumelemlst COMMA enumelem	{ ListAdd($$, $1, $3); }
	;

enumelem
	: id							{ $$ = new EnumInitializer($1); }
	| id KW_EQ expr					{ $$ = new EnumInitializer($1, $3); }
	;

set
	: LBRAC	RBRAC					{ $$ = new Set();}
	| LBRAC setelemlst	RBRAC		{ $$ = new Set($2); }
	;

setelemlst
	: setelem						{ $$ = new NodeList($1); }
	| setelemlst COMMA setelem		{ ListAdd($$, $1, $3); }
	;
	
setelem
	: expr							{ $$ = $1; }
	| expr KW_RANGE expr			{ $$ = new SetRange($1, $3); }
	;



	// ========================================================================
	// Constants
	// ========================================================================

constsec
	: KW_CONST constdecl	{ $$ = new NodeList($2); }
	| constsec constdecl	{ ListAdd($$, $1, $2); }
	;

constdecl
	: id KW_EQ constinitexpr  SCOL				{ $$ = new ConstDeclaration($1, null, $3); }	// true const
	| id COLON vartype KW_EQ constinitexpr SCOL	{ $$ = new ConstDeclaration($1, $3, $5); }		// typed const
	| id COLON proceduraltype funcdir_noterm_opt 
							functypeinit SCOL	{ $$ = new VarDeclaration($1, $3); $$.Directives.Add($4); $$.init = $5; }
	;
	
constinitexpr
	: constexpr				{ $$ = $1; }
	| arrayconst			{ $$ = $1; }
	| recordconst			{ $$ = $1; }
	;

constexpr
	: expr					{ $$ = new ConstExpression($1); }
	;
	

	// 1 or more exprs
arrayconst
	: LPAREN constexpr COMMA constinitexprlst RPAREN	{ $$ = new ArrayConst($4.InsertAt(0, constexpr)); }
	;

constinitexprlst
	: constexpr								{ $$ = new ExpressionList($1); }
	| constinitexprlst COMMA constexpr		{ ListAdd($$, $1, $3); }
	;

recordconst
	: LPAREN fieldconstlst scolopt RPAREN	{$$ = new RecordConst($2); }
	;

fieldconstlst
	: fieldconst							{ $$ = new FieldInitList($1); }
	| fieldconstlst SCOL fieldconst			{ ListAdd($$, $1, $3); }
	;

fieldconst
	: id COLON constinitexpr				{ $$ = new FieldInit($1, $3); }
	;



	// ========================================================================
	// Records
	// ========================================================================
	
	// Only supports 'simple' structs, without class-like components

recordtypebasic
	: KW_RECORD KW_END						{ $$ = null; /* TODO */ }
	| KW_RECORD fieldlst scolopt KW_END		{ $$ = null; /* TODO */ }
	;
	
recordtype
	: recordtypebasic									{ $$ = null; /* TODO */ }
	| KW_RECORD fieldlst SCOL recvariant scolopt KW_END	{ $$ = null; /* TODO */ }
	| KW_RECORD recvariant scolopt KW_END				{ $$ = null; /* TODO */ }
	;
	
recvariant
	: KW_CASE id COLON ordinaltype KW_OF recfieldlst	{ $$ = null; /* TODO */ }
	| KW_CASE ordinaltype KW_OF recfieldlst				{ $$ = null; /* TODO */ }
	;

recfieldlst
	: recfield %prec LOWESTPREC	{ $$ = null; /* TODO */ }
	| recfield SCOL				{ $$ = null; /* TODO */ }
	| recfield SCOL recfieldlst	{ $$ = null; /* TODO */ }
	;

recfield
	: constexpr COLON LPAREN recvarlst scolopt RPAREN	{ $$ = null; /* TODO */ }
	;
	
recvarlst
	: recvar					{ $$ = null; /* TODO */ }
	| recvarlst SCOL recvar		{ $$ = null; /* TODO */ }
	;

recvar
	: objfield					{ $$ = null; /* TODO */ }
	| recvariant				{ $$ = null; /* TODO */ }
	;
	

	
	
	// ========================================================================
	// Classes
	// ========================================================================

	// Objects are treated as classes
classtype
	: classkeyword heritage classbody KW_END	{ $$ = new ClassDefinition($1, $2, $3); }
	| classkeyword heritage						{ $$ = new ClassDefinition($1, $2, null); } // forward decl			
	;

classkeyword
	: KW_CLASS					{ $$ = ClassType.Class; }
	| KW_OBJECT					{ $$ = ClassType.Object; }
	;

heritage
	:							{ $$ = new List(); }
	| LPAREN idlst RPAREN		{ $$ = $2; }		// inheritance from class and interf(s)			
	;

classbody
	: fieldlst SCOL	complst scopeseclst		{ $$ = new ClassBody(Scope.Public, $3, $4);  }
	|				complst scopeseclst		{ $$ = new ClassBody(Scope.Public, $1, $2);  }
	;

scopeseclst
	:							{ $$ = new NodeList(); }
	| scopeseclst scopesec		{ ListAdd($$, $1, $2); }
	;

scopesec
	: scope_decl fieldlst SCOL complst	{ $$ = new ClassBody($1, $2, $4);  }
	| scope_decl			   complst	{ $$ = new ClassBody($1, null, $2);  }
	;
	
scope_decl
	: KW_PUBLISHED				{ $$ = Scope.Published; }
	| KW_PUBLIC					{ $$ = Scope.Public; }
	| KW_PROTECTED				{ $$ = Scope.Protected; }
	| KW_PRIVATE				{ $$ = Scope.Private; }
	;

fieldlst
	: objfield					{ $$ = new NodeList($1); }
	| fieldlst SCOL objfield	{ ListAdd($$, $1, $3); }
	;
	
complst
	:							{ $$ = new NodeList(); }
	| complst classcomp			{ ListAdd($$, $1, $2); }
	;

objfield
	: idlst COLON vartype		{ $$ = new FieldDeclaration($1, $3); }
	| idlst COLON proctypefield	{ $$ = new FieldDeclaration($1, $3);  }
//	| idlst COLON proceduraltype funcdir_noterm_opt	{ $$ = null; }
	;
	
classcomp
	: methoddecl				{ $$ = $1; }
	| KW_CLASS methoddecl		{ $2.isStatic = true; $$ = $2; }
	| property					{ $$ = $1; }
	;

interftype
	: KW_INTERF heritage guid classmethodlstopt classproplstopt KW_END	{ $$ = new InterfaceDefinition($2, $4, $5); }
	| KW_INTERF heritage classmethodlstopt classproplstopt KW_END		{ $$ = new InterfaceDefinition($2, $3, $4); }
	| KW_INTERF heritage %prec LOWESTPREC								{ $$ = null; /* TODO */ }
	;

guid
	: LBRAC stringconst RBRAC	{ /* ignored */ }
	| LBRAC lvalue RBRAC		{ /* ignored */ }
	;

classmethodlstopt
	: methodlst					{ $$ = $1; }
	|							{ $$ = null; }
	;

methodlst
	: methoddecl				{ $$ = new NodeList(new ClassMethod($1)); }
	| methodlst methoddecl		{ ListAdd($$, $1, $2); }
	;

	
	
	// ========================================================================
	// Properties
	// ========================================================================
	
classproplstopt
	: classproplst				{ $$ = $1; }
	|							{ $$ = null; }
	;

classproplst
	: property					{ $$ = new NodeList($1); }
	| classproplst property		{ ListAdd($$, $1, $2); }
	;

property
	: KW_PROPERTY id SCOL		{ $$ = null; }
	| KW_PROPERTY id propinterfopt COLON funcrettype propspecifiers SCOL defaultdiropt { $$ = new ClassProperty($2, $5, $6, $3, $8); }
	;

defaultdiropt
	:							{ $$ = null;}
	| id SCOL					{ $$ = new PropertyDefault($1); }
	;

propinterfopt
	:							{ $$ = null; }
	| LBRAC propfieldlst RBRAC	{ $$ = null; /* TODO */ }
	;
	
propfieldlst
	: propfield							{ $$ = null; /* TODO */ }
	| propfieldlst SCOL propfield		{ $$ = null; /* TODO */ }
	;

propfield
	: idlst COLON funcparamtype				{ $$ = null; /* TODO */ }
	| KW_CONST idlst COLON funcparamtype	{ $$ = null; /* TODO */ }
	;


	// Properties directive: emitted as keywords caught from within a lexical scope

propspecifiers
	: indexopt readopt writeopt storedopt defaultopt implopt	{ $$ = new PropertySpecifiers($1, $2, $3, $4, $5, $6); }
	;

indexopt
	:							{ $$ = null; }
	| KW_INDEX CONST_INT		{ $$ = new PropertyIndex(yyVal);  }
	;

storedopt
	:							{ $$ = null;  }
	| KW_STORED id				{ $$ = new PropertyStored($2); }
	;

defaultopt
	:							{ $$ = null;  }
	| KW_DEFAULT literal		{ $$ = new PropertyIndex($2); }
	| KW_NODEFAULT				{ $$ = null;  }
	;

implopt
	:							{ $$ = null;  }
	| KW_IMPLEMENTS id			{ $$ = new PropertyImplements($2); }
	;

readopt
	:							{ $$ = null;  }
	| KW_READ	id				{ $$ = new PropertyReadNode($2); }
	;

writeopt
	:							{ $$ = null; }
	| KW_WRITE	id				{ $$ = new PropertyWriteNode($2); }
	;


	
	
	// ========================================================================
	// Types
	// ========================================================================

typedecl
	: id KW_EQ typeopt vartype  SCOL						{ $$ = new TypeDeclarationNode($1, $4); }
	| id KW_EQ typeopt proceduraltype SCOL funcdirectopt	{ $$ = new TypeDeclarationNode($1, $4, $6); }
	| id KW_EQ typeopt packcomptype SCOL					{ lastObjectName = id; $$ = $4; }
	;

typeopt
	:							{ $$ = false; }
	| KW_TYPE					{ $$ = true ; }
	;

vartype
	: scalartype				{ $$ = $1; }
	| enumtype					{ $$ = $1; }
	| rangetype					{ $$ = $1; }
	// metaclasse
	| metaclasstype				{ $$ = $1; }
	| packstructtype			{ $$ = $1; }
	;

packcomptype
	: KW_PACKED compositetype	{ $$ = $2; $2.IsPacked = true; }
	| compositetype				{ $$ = $1; }
	;

compositetype
	: classtype					{ $$ = $1; }
	| interftype				{ $$ = $1; }
	;

metaclasstype
	: KW_CLASS KW_OF id			{ $$ = new ClassType($3); }
	;

packstructtype
	: structuredtype			{ $$ = $1; }
	| KW_PACKED structuredtype	{ $$ = $2; $2.IsPacked = true; }
	;

structuredtype
	: arraytype					{ $$ = $1; }
	| settype					{ $$ = $1; }
	| filetype					{ $$ = $1; }
	| recordtype				{ $$ = $1; }
	;
	
arrayszlst
	: rangetype					{ $$ = new NodeList($1); }
	| arrayszlst COMMA rangetype{ ListAdd($$, $1, $3); }
	;

arraytype
	: TYPE_ARRAY LBRAC arraytypedef RBRAC KW_OF vartype 	{ $$ = new ArrayType($6, $3); }
	| TYPE_ARRAY KW_OF vartype 	{ $$ = new ArrayType($3); }
	;

arraytypedef
	: arrayszlst				{ $$ = $1; }
	| inttype					{ $$ = $1; }
	| chartype					{ $$ = $1; }
	| id						{ $$ = $1; }
	;

settype
	: TYPE_SET KW_OF ordinaltype{ $$ = new SetType($3); }
	;

filetype
	: TYPE_FILE KW_OF vartype 	{ $$ = new FileType($3); }
	| TYPE_FILE					{ $$ = new FileType(null); }
	;


scalartype
	: integraltype				{ $$ = $1; }
	| realtype					{ $$ = $1; }
	| stringtype				{ $$ = $1; }
	| varianttype				{ $$ = $1; }
	| pointertype				{ $$ = $1; }
	;

ordinaltype
	: rangetype					{ $$ = $1; }
	| enumtype					{ $$ = $1; }
	| integraltype				{ $$ = $1; }
	;

integraltype
	: inttype					{ $$ = $1; }
	| chartype					{ $$ = $1; }
	| TYPE_BOOL					{ $$ = new BoolType(); }
	| id						{ $$ = $1; }
	;

realtype
	: TYPE_REAL48				{ $$ = new DoubleType(); }
	| TYPE_FLOAT				{ $$ = new FloatType (); }
	| TYPE_DOUBLE				{ $$ = new DoubleType(); }
	| TYPE_EXTENDED				{ $$ = new ExtendedType(); }
	| TYPE_CURR					{ $$ = new CurrencyType(); }
	;

inttype
	: TYPE_BYTE					{ $$ = new UnsignedInt8Type(); }
	| TYPE_INT					{ $$ = new SignedInt32Type(); }
	| TYPE_SHORTINT				{ $$ = new SignedInt8Type ();  }
	| TYPE_SMALLINT				{ $$ = new SignedInt16Type(); }
	| TYPE_LONGINT				{ $$ = new SignedInt32Type(); }
	| TYPE_INT64				{ $$ = new SignedInt64Type(); }
	| TYPE_UINT64				{ $$ = new UnsignedInt64Type(); }
	| TYPE_WORD					{ $$ = new UnsignedInt16Type(); }
	| TYPE_LONGWORD				{ $$ = new UnsignedInt32Type(); }
	| TYPE_CARDINAL				{ $$ = new UnsignedInt32Type(); }
	| TYPE_COMP					{ $$ = new SignedInt64Type	(); }
	;

chartype
	: TYPE_CHAR					{ $$ = new CharType(); }
	| TYPE_WIDECHAR				{ $$ = new CharType(); }
	;

stringtype
	: TYPE_STR /*dynamic size*/	{ $$ = new StringType(null); }
	| TYPE_PCHAR				{ $$ = new StringType(null); }
	| TYPE_STR LBRAC expr RBRAC	{ $$ = new StringType($3); }
	| TYPE_SHORTSTR				{ $$ = new StringType(null); }
	| TYPE_WIDESTR				{ $$ = new StringType(null); }
	;

varianttype
	: TYPE_VAR					{ $$ = new VariantType(); }
	| TYPE_OLEVAR				{ $$ = new VariantType(); }
	;

pointertype
	: KW_DEREF scalartype 		{ $$ = new PointerType($2); }
	| TYPE_PTR					{ $$ = new PointerType(); }
	;

funcparamtype
	: scalartype				{ $$ = $1; }
	| TYPE_ARRAY KW_OF scalartype	{ $$ = new ArrayType(null, $3); }
	;

funcrettype
	: scalartype				{ $$ = $1; }
	;
	
	// scalartype w/o user-defined types
casttype
	: inttype					{ $$ = $1; }
	| chartype					{ $$ = $1; }
	| realtype					{ $$ = $1; }
	| stringtype				{ $$ = $1; }
	| pointertype				{ $$ = $1; }
	;


%%

	}	// close parser class, opened in prolog	

// already defined in template
//} // close outermost namespace
