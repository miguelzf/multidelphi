%{

using System;
using System.Collections;
using System.IO;
using System.Text;
using System.Diagnostics;
using crosspascal.ast;
using crosspascal.ast.nodes;
using crosspascal.semantics;

namespace crosspascal.parser
{

	// Open main Parser class
	public class DelphiParser
	{

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

		
		//Encoding.Default;	// typically Single-Bye char set
		// TODO change charset to unicode, use %unicode in flex
		public static readonly Encoding DefaultEncoding = Encoding.GetEncoding("iso-8859-1");

		DelphiScanner lexer;
		
		public static int DebugLevel;

		// to be fetched in the AST Declaration node
		public static DeclarationRegistry DeclRegistry;

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
		
		
		//
		// Entry point and public interface
		//
		
		public DelphiParser(ParserDebug dgb)
		{
			if (dgb != null) {
				this.debug = (ParserDebug) dgb;
			}
			
			eof_token = DelphiScanner.YYEOF;
		}

		public DelphiParser(int dgbLevel)
			: this(new Func<ParserDebug>(
					() => {	switch(dgbLevel)
						{	case 1: return new DebugPrintFinal();
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
			if (dgb != null) {
				this.debug = (ParserDebug) dgb;
				DebugLevel = 1;
			}

			DeclRegistry = new DeclarationRegistry();
			DeclRegistry.LoadRuntimeNames();
			
			lexer = new DelphiScanner(tr);
			Object parserRet;
			try {
				parserRet = yyparse(lexer);
			} 
			catch (Exception yye) {
				ErrorOutput.WriteLine(yye.Message + " in line " + lexer.yylineno());
				ErrorOutput.WriteLine(yye.StackTrace);
				// only clean way to signal error. null is the default yyVal
				throw yye; // new InputRejected(GetErrorMessage(yye));
			}
			
			if (!parserRet.GetType().IsSubclassOf(typeof(CompilationUnit)))
				throw new ParserException();
			
			return (CompilationUnit) parserRet;
		}
		
		
		
		// Internal helpers
		
		string lastObjectName = null;	// keeps track of current class/object/interface being parsed
		
		ListNode<Node>  ListAdd(ListNode<Node> bodylst, Node elem)
		{
			bodylst.Add(elem);
			return bodylst;
		}

		ListNode<Node> ListAdd(ListNode<Node> bodylst, IListNode<Node> elems)
		{
			bodylst.Add(elems);
			return bodylst;
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
				default: throw new ParserException("Invalid Binary Operation token: " + token);
			}
		}
		
		bool CheckDirectiveId(String expected, String idtoken)
		{
			if (String.Compare(expected, idtoken, true) != 0)
			{	yyerror("Invalid directive '" + idtoken + "', expected: " + expected);
				return false;
			}
			return true;
		}


%}


	// =========================================================================
	// Rules declarations
	// =========================================================================

%start goal
%type<string> id labelid exportid stringconst conststr
//%type<int> kwfunction kwprocedure kwconstructor kwdestructor kwrecord kwclass kwinterface
%type<ArrayList> labelidlst idlst

	// sections and declarations
%type<CompilationUnit> goal file program library unit package
%type<UnitItem> containsitem usesitem expitem 
%type<NodeList> usesopt useslst requires requireslst containslst contains expitemlst
%type<DeclarationList> interfdecllst maindecllst declseclst interfdecl maindeclsec funcdeclsec basicdeclsec
%type<DeclarationList> thrvarsec rscstringsec exportsec labeldeclsec constsec typesec varsec rscstringlst
%type<ImplementationSection> implsec
%type<InterfaceSection> interfsec
%type<InitializationSection> initsec
%type<FinalizationSection> finalsec   
%type<ProgramBody> main_block 
%type<VarDeclaration> vardecl 
%type<FieldDeclaration> objfield
%type<ConstDeclaration> constdecl rscstring
%type<TypeDeclaration> typedecl

	// functions
%type<RoutineDefinition> routinedef methodroutinedef routinedefunqualif
%type<RoutineDeclaration> routineproto routinedeclext routinedecl 
%type<MethodDeclaration> methodproto methoddecl methoddeclin
%type<RoutineDirectives>  funcdirectlst funcdir_noterm_opt funcdirectopt importdiropt importdirforced 
%type<MethodDirectives> metdirectopt metdirectlst
%type<int> funcdirective  funcqualif funcdeprecated metdirective metqualif routinecallconv
%type<RoutineBody> funcdefine
%type<ParameterList> formalparams formalparamslst
%type<ParameterDeclaration> formalparm
%type<ExternalDirective>  externarg

	// statements
	
%type<BlockStatement> block funcblock assemblerstmt	blockstmt
%type<ExceptionBlock> exceptionblock
%type<Statement> stmt nonlbl_stmt assign goto_stmt ifstmt casestmt elsecase repeatstmt whilestmt forstmt withstmt caseselector ondef
%type<Statement> tryexceptstmt tryfinallystmt raisestmt 
%type<StatementList> caseselectorlst onlst stmtlist asmcode

	// expresssions
%type<Literal> literal basicliteral discrete stringlit
%type<int> sign mulop addop relop 
%type<int> KW_EQ KW_GT KW_LT KW_LE KW_GE KW_NE KW_IN KW_IS KW_SUM KW_SUB KW_OR KW_XOR KW_MUL KW_DIV KW_QUOT KW_MOD KW_SHL KW_SHR KW_AS KW_AND
%type<ulong> constint constnil
%type<bool>   constbool
%type<double> constreal
%type<char>   constchar
%type<LvalueExpression> routinecall lvalue 
%type<RoutineCall> inheritedcall
%type<Identifier> identifier
%type<Expression> unaryexpr expr rangestart set inheritedexpr
%type<ExpressionList> caselabellst exprlst exprlstopt setelemlst constinitexprlst 
%type<TypeList> arrayszlst
%type<FieldInitList> fieldconstlst 
%type<EnumValueList> enumelemlst
%type<EnumValue> enumelem
%type<ConstExpression> paraminitopt constexpr functypeinit arrayconst recordconst fieldconst constinitexpr setelem

	// objects etc - TODO
%type<Node> recvariant  recfield  recvar guid  recfieldlst propfield
%type<DeclarationList> scopeseclst complst classmethodlstopt methodlst classproplstopt classproplst fieldlst 
%type<NodeList> recvarlst propfieldlst
%type<ArrayList> heritage 
%type<Declaration> scopesec classbody classcomp classtype property interftype
%type<Scope> scope_decl
%type<PropertySpecifier> propinterfopt defaultdiropt indexopt storedopt defaultopt implopt readopt writeopt
%type<PropertySpecifiers> propspecifiers

	// Types
%type<ScalarType> scalartype pointertype casttype pointedtype
	// realtype inttype chartype stringtype varianttype scalartype  
%type<ScalarType> funcrettype funcret paramtypeopt paramtypespec 
%type<StructuredType> structuredtype
%type<VariableType> funcparamtype arraytype settype filetype packstructtype
%type<VariableType> vartype enumtype rangetype metaclasstype 
	// %type<IntegralType> arraytypedef integraltype
%type<CompositeDeclaration> packcomptype compositetype
%type<RecordType> recordtype recordtypebasic
%type<TypeNode> ordinaltype
%type<ProceduralType> proceduraltype proctypefield proceduralsign


	
	
	// =========================================================================
	// Tokens declarations
	// =========================================================================

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


	// =========================================================================
	// Precedence and associativity
	// =========================================================================

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
	// =========================================================================

%%


	// =========================================================================
	//
	// 						YACC/JAY Rules
	//
	// =========================================================================
	
	
goal: file KW_DOT		{	$$ = $1; ACCEPT();	}
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
	

	// Context-opening keywords
	
kwfunction
	: KW_FUNCTION		{ DeclRegistry.EnterContext(); }
	;
kwprocedure
	: KW_PROCEDURE		{ DeclRegistry.EnterContext(); }
	;
kwconstructor
	: KW_CONSTRUCTOR	{ DeclRegistry.EnterContext(); }
	;
kwdestructor
	: KW_DESTRUCTOR		{ DeclRegistry.EnterContext(); }
	;
kwrecord
	: KW_RECORD			{ DeclRegistry.EnterContext(); }
	;
kwclass
	: KW_CLASS			{ DeclRegistry.EnterContext(); }
	| KW_OBJECT			{ DeclRegistry.EnterContext(); }
	;
kwinterf
	: KW_INTERF			{ DeclRegistry.EnterContext(); }
	;


	
	// ===================================================================================
	// Top-level Sections
	// ===================================================================================

program
	: KW_PROGRAM id SCOL	usesopt main_block	{ $$ = new ProgramNode($2, $4, $5); }
	| 						usesopt main_block	{ $$ = new ProgramNode("untitled", $1, $2); }
	;

library
	: KW_LIBRARY id SCOL usesopt main_block		{ $$ = new LibraryNode($2, $4, $5); }
	;
	
package
	: id id SCOL requires contains KW_END	{ $$ = (CheckDirectiveId("package", $1)? new PackageNode($2, $4, $5) : null); }
	;

requires
	:									{ $$ = new NodeList(); }
	| KW_REQUIRES requireslst SCOL		{ $$ = $2; }
	;
	
requireslst
	: id								{ $$ = new NodeList(new RequiresItem($1)); }
	| requireslst COMMA id				{ $$ = $1; $1.Add(new RequiresItem($3)); }
	;

contains
	: KW_CONTAINS containslst SCOL		{ $$ = $2; }
	;

containslst
	: containsitem						{ $$ = new NodeList($1); }
	| containslst COMMA containsitem	{ $$ = $1; $1.Add($3); }
	;

containsitem
	: id						{ $$ = new ContainsItem($1); }
	| id KW_IN stringconst		{ $$ = new ContainsItem($1, $3); }
	;

usesopt
	:							{ $$ = new NodeList(); }
	| KW_USES useslst SCOL		{ $$ = $2; }
	;

useslst
	: usesitem					{ $$ = new NodeList($1); }
	| useslst COMMA usesitem	{ $$ = $1; $1.Add($3); }
	;
	
usesitem
	: id						{ $$ = new UsesItem($1);}
	| id KW_IN stringconst		{ $$ = new UsesItem($1, $3);}
	;

unit
	: KW_UNIT id SCOL interfsec implsec initsec finalsec KW_END  { $$ = new UnitNode($2, $4, $5, $6); }
	;

implsec
	: KW_IMPL usesopt	maindecllst		{ $$ = new ImplementationSection($2, $3);}
	| KW_IMPL usesopt					{ $$ = new ImplementationSection($2, null);}
	;

interfsec
	: kwinterf usesopt interfdecllst	{ $$ = new InterfaceSection($2, $3);}
	;

interfdecllst
	:							{ $$ = new DeclarationList();}
	| interfdecllst interfdecl	{ $$ = $1; $1.Add($2); }
	;

initsec
	: KW_INIT  blockstmt		{ $$ = new InitializationSection($2);}
	| KW_BEGIN blockstmt		{ $$ = new InitializationSection($2);}
	;
	
finalsec
	: KW_FINALIZ blockstmt 		{ $$ = new FinalizationSection($2);}
	;
	
main_block
	: maindecllst block			{ $$ = new ProgramBody($1, $2);}
	|			  block			{ $$ = new ProgramBody(null, $1);}
	;
	
maindecllst
	: maindeclsec				{ $$ = $1; }
	| maindecllst maindeclsec	{ $$ = $1; $1.Add($2); }
	;

declseclst
	: funcdeclsec				{ $$ = $1;}
	| declseclst funcdeclsec	{ $$ = $1; $1.Add($2); }
	;

	


	// ===================================================================================
	// Declaration sections
	// ===================================================================================

interfdecl
	: basicdeclsec			{ $$ = $1;}
	| routinedecl			{ $$ = new DeclarationList($1);}
	| thrvarsec				{ $$ = $1;}
	| rscstringsec			{ $$ = $1;}
	;

maindeclsec
	: basicdeclsec			{ $$ = $1;}
	| thrvarsec				{ $$ = $1;}
	| exportsec				{ $$ = $1;}
	| routinedeclext		{ $$ = new DeclarationList($1);}
	| routinedef			{ $$ = new DeclarationList($1);}
	| labeldeclsec			{ $$ = $1;}
	;

funcdeclsec
	: basicdeclsec			{ $$ = $1;}
	| labeldeclsec			{ $$ = $1;}
	| routinedefunqualif	{ $$ = new DeclarationList($1);}
	;

basicdeclsec
	: constsec				{ $$ = $1;}
	| typesec				{ $$ = $1;}
	| varsec				{ $$ = $1;}
	;

typesec
	: KW_TYPE typedecl		{ $$ = new DeclarationList($2); }
	| typesec typedecl		{ $$ = $1; $1.Add($2); }
	;


	// Variables

varsec
	: KW_VAR vardecl		{ $$ = new DeclarationList($2); }
	| varsec vardecl		{ $$ = $1; $1.Add($2); }
	;
	
thrvarsec
	: KW_THRVAR vardecl		{ $$ = new DeclarationList($2); $2.isThrVar = true; }
	| thrvarsec vardecl		{ $$ = $1; $1.Add($2);   $2.isThrVar = true; }
	;

vardecl
	: idlst COLON vartype SCOL							{ $$ = new VarDeclaration($1, $3); }
	| idlst COLON vartype KW_EQ constinitexpr SCOL		{ $$ = new VarDeclaration($1, $3, $5); }
	| idlst COLON vartype KW_ABSOLUTE id SCOL			{ $$ = new VarDeclaration($1, $3, $5); }
	| idlst COLON proceduraltype SCOL funcdirectopt		{ $$ = new VarDeclaration($1, $3); $3.Directives = $5; }
	| idlst COLON proceduraltype SCOL
			funcdir_noterm_opt functypeinit SCOL		{ $$ = new VarDeclaration($1, $3, $6); $3.Directives = $5; }
	;


	// Resourcestrings fom windows
	
rscstringsec
	: TYPE_RSCSTR rscstringlst		{ $$ = $2; }
	;
	
rscstringlst
	: rscstring						{ $$ = new DeclarationList($1); }
	| rscstringlst rscstring		{ $$ = $1; $1.Add($2); }
	;
	
rscstring
	:  id KW_EQ stringconst SCOL	{ $$ = new ConstDeclaration($1, new StringLiteral($3)); }
	;

	
	// labels
	
labeldeclsec
	: KW_LABEL labelidlst SCOL		{$$ = new DeclarationList(new LabelDeclaration($2)); }
	;
	
labelidlst 
	: labelid						{ var ar = new ArrayList(); ar.Add($1); $$ = ar; }
	| labelidlst COMMA labelid		{ $$ = $1; $1.Add($3); }
	;

labelid
	: constint 						{ 	/* decimal int 0..9999 */
										if ($1 < 0 || $1 > 9999)
											yyerror("Label number must be between 0 and 9999");
										$$ = ""+$1;
									}
	| id							{ $$ = $1; }
	;


	// Exports

exportsec	
	: KW_EXPORTS expitemlst			{ $$ = $2; }
	;

expitemlst
	: expitem						{ $$ = new NodeList($1); }
	| expitemlst COMMA expitem		{ $$ = $1; $1.Add($3); }
	;

expitem
	: exportid						{ $$ = new ExportItem($1); }
	| exportid KW_NAME stringconst	{ $$ = new ExportItem($1, $3); }
	| exportid KW_INDEX constint	{ $$ = new ExportItem($1, (int) $3); }
	;
	
exportid	// TODO formalparams
	: id formalparams 				{ $$ = $1; }
	;



	// ===================================================================================
	// Functions
	// ===================================================================================

	// Prototypes/signatures
	// proc proto for definitions or external/forward decls
	// check that funcrecopt is null for every kind except FUNCTION

routinedef
	: routinedefunqualif								{ $$ = $1; }
	| methodroutinedef	funcdefine SCOL 				{ $$ = $1; DeclRegistry.LeaveContext(); }
	;
	
methodroutinedef
	: methodproto SCOL metdirectopt 					{ $$ = $1; $1.Directives = $3; }
	| kwclass methodproto SCOL metdirectopt 			{ $$ = $2; $2.Directives = $4; $2.isStatic = true; }
	| kwconstructor id KW_DOT id formalparams SCOL		{ $$ = new ConstructorDeclaration($2, $4, $5); }
	| kwdestructor  id KW_DOT id formalparams SCOL		{ $$ = new DestructorDeclaration ($2, $4, $5); }
	;
	
methodproto
	: kwfunction  id KW_DOT id formalparams funcret 	{ $$ = new MethodDeclaration($2, $4, $5, $6); }
	| kwprocedure id KW_DOT id formalparams 			{ $$ = new MethodDeclaration($2, $4, $5); }
	;
	
	// global routine definition
routinedefunqualif
	: routineproto funcdirectopt funcdefine SCOL		{  $1.Directives = $2; $$ = new RoutineDefinition($1, $3); DeclRegistry.LeaveContext(); }
	;
	
	// routine decl for interface sections
routinedecl
	: routineproto importdiropt			{ $$ = $1; $1.Directives = $2; DeclRegistry.LeaveContext(); }
	;

	// routine decl for implementation sections, needs an external/forward
routinedeclext
	: routineproto importdirforced		{ $$ = $1; $1.Directives = $2; DeclRegistry.LeaveContext(); }
	;

methoddecl
	: methoddeclin						{ $$ = $1; DeclRegistry.LeaveContext(); }
	;
	
methoddeclin
	: kwfunction    id formalparams funcret SCOL metdirectopt	{ $$ = new MethodDeclaration(lastObjectName, $2, $3, $4, $6); }
	| kwprocedure   id formalparams         SCOL metdirectopt	{ $$ = new MethodDeclaration(lastObjectName, $2, $3, null, $5); }
	| kwconstructor id formalparams SCOL						{ $$ = new DestructorDeclaration(lastObjectName, $2, $3); }
	| kwdestructor  id formalparams SCOL						{ $$ = new DestructorDeclaration(lastObjectName, $2, $3); }
	;

routineproto
	: kwfunction  id formalparams funcret	SCOL	{ $$ = new RoutineDeclaration($2, $3, $4); }
	| kwprocedure id formalparams 			SCOL	{ $$ = new RoutineDeclaration($2, $3); }
	;	
	

proceduraltype
	: proceduralsign						{ $$ = $1; DeclRegistry.LeaveContext(); }
	;
	
proceduralsign
	: kwprocedure formalparams 							{ $$ = new ProceduralType($2); } 
	| kwfunction  formalparams funcret					{ $$ = new ProceduralType($2, $3); } 
	| kwprocedure formalparams KW_OF KW_OBJECT			{ $$ = new MethodType($2); } 
	| kwfunction  formalparams funcret KW_OF KW_OBJECT	{ $$ = new MethodType($2, $3); } 
	;

proctypefield
	: proceduraltype					{ $$ = $1; }
	| proceduraltype routinecallconv	{ $$ = $1; $1.Directives.Callconv = (CallConvention) $2; }
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
	| formalparamslst SCOL formalparm	{ $$ = $1; $1.Add($3); }
	;

formalparm
	: KW_VAR	idlst paramtypeopt					{ $$ = new VarParameterDeclaration($2, $3); } 
	| KW_OUT	idlst paramtypeopt					{ $$ = new OutParameterDeclaration($2, $3); } 
	| 			idlst paramtypespec paraminitopt	{ $$ = new ParameterDeclaration($1, $2, $3); }
	| KW_CONST	idlst paramtypeopt  paraminitopt	{ $$ = new ConstParameterDeclaration($2, $3, $4); }
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
	: KW_EQ identifier					{ $$ = new ConstExpression($2); }
	| KW_EQ constnil					{ $$ = new ConstExpression(new PointerLiteral($2)); }
	;

	
	// Function directives

importdiropt
	: funcdirectopt						{ $$ = $1; }
	| importdirforced					{ $$ = $1; }
	;

importdirforced
	: funcdirectopt KW_EXTERNAL externarg  funcdirectopt	{ $$ = $1; $1.Add($4); $1.Importdir = ImportDirective.External; $1.External = $3; }
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
	| funcdirectlst SCOL funcdirective	{ $1.Add($3); $$ = $1; }
	;

metdirectlst
	: metdirective						{ $$ = new MethodDirectives(); }
	| metdirectlst SCOL metdirective	{ $1.Add($3); $$ = $1; }
	;

metdirective
	: funcdirective			{ $$ = $1; }
	| metqualif				{ $$ = $1; }
	;
	
funcdirective
	: funcqualif			{ $$ = $1; }
	| routinecallconv		{ $$ = $1; }
	| funcdeprecated		{ $$ = $1; }
	;

funcdeprecated
	: KW_FAR				{ $$ = GeneralDirective.Far; }
	| KW_NEAR				{ $$ = GeneralDirective.Near; }
	| KW_RESIDENT			{ $$ = GeneralDirective.Resident; }
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
	

	
	// ===================================================================================
	// Statements
	// ===================================================================================

block
	: KW_BEGIN blockstmt KW_END		{ $$ = $2; }
	;

	// encapsulate the StatementList in a Statement itself
blockstmt
	: stmtlist				{ $$ = new BlockStatement($1); }
	;
	
stmtlist
	: stmt					{ $$ = new StatementList($1); }
	| stmt SCOL stmtlist	{ $$ = $3; $3.Add($1); }
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
	: KW_CASE expr KW_OF caseselectorlst elsecase KW_END	{ $$ = new CaseStatement($2, $4, $5); }
	;

elsecase
	:								{ $$ = null;}	
	| KW_ELSE nonlbl_stmt			{ $$ = $2; }
	| KW_ELSE nonlbl_stmt SCOL		{ $$ = $2; }
	;

caseselectorlst
	: caseselector							{ $$ = new StatementList($1); }
	| caseselectorlst SCOL caseselector		{ $$ = $1; $1.Add($3); }
	;

caseselector
	:										{ $$ = null; }
	| caselabellst COLON nonlbl_stmt		{ $$ = new CaseSelector($1, $3); }
	;
	
caselabellst
	: setelem							{ $$ = new ExpressionList($1); }
	| caselabellst COMMA setelem		{ $$ = $1; $1.Add($3); }
	;

repeatstmt
	: KW_REPEAT blockstmt KW_UNTIL expr	{ $$ = new RepeatLoop($2, $4); }
	;

whilestmt
	: KW_WHILE expr KW_DO nonlbl_stmt	{ $$ = new WhileLoop($2, $4); }
	;

forstmt
	: KW_FOR identifier KW_ASSIGN expr KW_TO	 expr KW_DO nonlbl_stmt	{ $$ = new ForLoop($2, $4, $6, $8, 1); }
	| KW_FOR identifier KW_ASSIGN expr KW_DOWNTO expr KW_DO nonlbl_stmt { $$ = new ForLoop($2, $4, $6, $8, -1); }
	;

	// expr must yield a ref to a record, object, class, interface or class type
withstmt
	: KW_WITH exprlst KW_DO nonlbl_stmt		{ $$ = new WithStatement($2, $4); }
	;

tryexceptstmt
	: KW_TRY blockstmt KW_EXCEPT exceptionblock KW_END	{ $$ = new TryExceptStatement($2, $4); }
	;

exceptionblock
	: onlst KW_ELSE blockstmt					{ $$ = new ExceptionBlock($1, $3); }
	| onlst										{ $$ = new ExceptionBlock($1); }
	| blockstmt									{ $$ = new ExceptionBlock(null, $1); }
	;

onlst
	: ondef										{ $$ = new StatementList($1); }
	| onlst ondef								{ $$ = $1; $1.Add($2); }
	;

ondef
	: KW_ON id COLON id KW_DO nonlbl_stmt SCOL	{ $$ = new OnStatement($2, $4, $6); }
	| KW_ON 		 id KW_DO nonlbl_stmt SCOL	{ $$ = new OnStatement(null, $2, $4); }
	;

tryfinallystmt
	: KW_TRY blockstmt KW_FINALLY blockstmt KW_END	{ $$ = new TryFinallyStatement($2, $4); }
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




	
	// ===================================================================================
	// Expressions
	// ===================================================================================

inheritedexpr
	: KW_INHERITED inheritedcall			{ $$ = new InheritedCall($2); }
	;
	
inheritedcall
	: 										{ $$ = null; }
	| identifier							{ $$ = new RoutineCall($1); }
	| identifier LPAREN exprlstopt RPAREN	{ $$ = new RoutineCall($1,$3); }
	;
	
identifier
	: id 									{ $$ = new Identifier($1); }
	;
	
	// routine call to be used as a statement
routinecall
	: identifier							{ $$ = new RoutineCall($1); }
	| lvalue LPAREN exprlstopt RPAREN		{ $$ = new RoutineCall($1, $3); }
	| lvalue KW_DOT id						{ $$ = new FieldAcess($1, $3); }
	;
	
lvalue	// lvalue
	: identifier							{ $$ = ResolveId($1); }
	| lvalue LPAREN exprlstopt RPAREN		{ $$ = new RoutineCall($1, $3); }
	| lvalue KW_DOT id						{ $$ = new FieldAcess($1, $3); }
	| lvalue KW_DEREF						{ $$ = new PointerDereference($1); }
	| lvalue LBRAC exprlst RBRAC			{ $$ = new ArrayAccess($1, $3); }
	| stringconst LBRAC expr RBRAC			{ if ($1.Length < 2)
												yyerror("Cannot access character or empty string as an array");
											 else
												$$ = new ArrayAccess(new ArrayConst($1), new ExpressionList($3));
											}
	| LPAREN expr RPAREN					{ $$ = $2; }
	;

//	Log.Instance().Write(logWarning,'AL',Proc+' not avaliable.');

unaryexpr
	: literal								{ $$ = $1; }
	| lvalue								{ $$ = $1; }
	| set									{ $$ = $1; }
	| KW_ADDR unaryexpr						{ $$ = new AddressLvalue($2); }
	| KW_NOT unaryexpr						{ $$ = new LogicalNot($2); }
	| KW_SUM unaryexpr 						{ $$ = new UnaryPlus($2); }
	| KW_SUB unaryexpr 						{ $$ = new UnaryMinus($2); }
	| inheritedexpr							{ $$ = $1; }
	;

expr
	: unaryexpr								{ $$ = $1; }
	| expr KW_AS casttype					{ $$ = new TypeCast($3, $1); }
	| expr KW_IS casttype					{ $$ = new TypeIs($1, $3); }
	| expr KW_IN expr						{ $$ = new SetIn($1, $3); }
	| expr relop expr %prec KW_EQ			{ $$ = CreateBinaryExpression($1, $2, $3); }
	| expr addop expr %prec KW_SUB			{ $$ = CreateBinaryExpression($1, $2, $3); }
	| expr mulop expr %prec KW_MUL			{ $$ = CreateBinaryExpression($1, $2, $3); }
	;

sign
	: KW_SUB		{ $$ = currentToken; }
	| KW_SUM		{ $$ = currentToken; }
	;
mulop
	: KW_MUL		{ $$ = currentToken; }
	| KW_DIV		{ $$ = currentToken; }
	| KW_QUOT		{ $$ = currentToken; }
	| KW_MOD		{ $$ = currentToken; }
	| KW_SHR		{ $$ = currentToken; }
	| KW_SHL		{ $$ = currentToken; }
	| KW_AND		{ $$ = currentToken; }
	;
addop
	: KW_SUB		{ $$ = currentToken; }
	| KW_SUM		{ $$ = currentToken; }
	| KW_OR			{ $$ = currentToken; }
	| KW_XOR		{ $$ = currentToken; }
	;
relop
	: KW_EQ			{ $$ = currentToken; }
	| KW_NE			{ $$ = currentToken; }
	| KW_LT			{ $$ = currentToken; }
	| KW_LE			{ $$ = currentToken; }
	| KW_GT			{ $$ = currentToken; }
	| KW_GE			{ $$ = currentToken; }
	;

exprlst
	: expr						{ $$ = new ExpressionList($1); }
	| exprlst COMMA expr		{ $$ = $1; $1.Add($3); }
	;

exprlstopt
	:							{ $$ = new ExpressionList(); }
	| exprlst					{ $$ = $1; }
	;
	


	// ===================================================================================
	// Literals
	// ===================================================================================

literal
	: basicliteral	{ $$ = $1; }
	| stringlit		{ $$ = $1; }
	;

basicliteral
	: constint		{ $$ = new IntLiteral($1);}
	| constbool		{ $$ = new BoolLiteral($1);}
	| constreal		{ $$ = new RealLiteral($1);}
	| constnil		{ $$ = new PointerLiteral($1);}
	;

discrete
	: constint		{ $$ = new IntLiteral($1);}
	| constchar		{ $$ = new CharLiteral($1);}
	| constbool		{ $$ = new BoolLiteral($1);}
	;

stringlit
	: stringconst	{ if ($1.Length==1)	$$ = new CharLiteral($1[0]);
					  else				$$ = new StringLiteral($1);
					}
	;

stringconst
	: conststr					{ $$ = $1; }
	| constchar					{ $$ = ""+$1; }
	| stringconst conststr		{ $$ = $1 + $2; }
	| stringconst constchar		{ $$ = $1 + $2; }
	;

idlst
	: id						{ var ar = new ArrayList(); ar.Add($1); $$ = ar; }
	| idlst COMMA id			{ $1.Add($3); $$ = $1; }
	;

	// Cast the output value from the Scanner
id	: IDENTIFIER	{ $$ = (string) yyVal; }
	;
constnil
	: CONST_NIL		{ $$ = 0; }
	; 
constint
	: CONST_INT		{ $$ = (ulong) yyVal;}
	;
constchar
	: CONST_CHAR	{ $$ = (char) yyVal;}
	;
constreal
	: CONST_REAL	{ $$ = (double) yyVal;}
	;
constbool
	: CONST_BOOL	{ $$ = (bool) yyVal;}
	;
conststr
	:  CONST_STR	{ $$ = (string) yyVal; }
	;

	


	// ===================================================================================
	// Sets and Enums
	// ===================================================================================

rangetype			// must be const
	: rangestart KW_RANGE expr		{ $3.EnforceConst = true; $$ = new RangeType($1, new ConstExpression($3)); }
	;
	
	// best effort to support constant exprs. TODO improve
rangestart
	: discrete						{ $$ = $1; }
//	| lvalue						{ $$ = null; /* TODO */ }
	| sign expr						{ $$ = $1; }
	;

enumtype
	: LPAREN enumelemlst RPAREN		{ $$ = new EnumType($2); }
	;

enumelemlst
	: enumelem						{ $$ = new EnumValueList($1); }
	| enumelemlst COMMA enumelem	{ $$ = $1; $1.Add($3); }
	;

enumelem
	: id							{ $$ = new EnumValue($1); }
	| id KW_EQ expr					{ $$ = new EnumValue($1, $3); }
	;

set
	: LBRAC	RBRAC					{ $$ = new Set();}
	| LBRAC setelemlst	RBRAC		{ $$ = new Set($2); }
	;

setelemlst
	: setelem						{ $$ = new ExpressionList($1); }
	| setelemlst COMMA setelem		{ $$ = $1; $1.Add($3); }
	;
	
setelem
	: constexpr							{ $$ = $1; }
	| constexpr KW_RANGE constexpr		{ $$ = new SetRange(new RangeType($1, $3)); }
	;



	// ===================================================================================
	// Constants
	// ===================================================================================

constsec
	: KW_CONST constdecl	{ $$ = new DeclarationList($2); }
	| constsec constdecl	{ $$ = $1; $1.Add($2); }
	;

constdecl
	: id KW_EQ constinitexpr  SCOL				{ $$ = new ConstDeclaration($1, $3); }	// true const
	| id COLON vartype KW_EQ constinitexpr SCOL	{ $$ = new ConstDeclaration($1, $5, $3); }		// typed const
	| id COLON proceduraltype funcdir_noterm_opt
							functypeinit SCOL	{ $$ = new ConstDeclaration($1, $5, $3); $3.Directives = $4; }
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
	: LPAREN constexpr COMMA constinitexprlst RPAREN	{ $4.InsertAt(0, $2); $$ = new ArrayConst($4); }
	;

constinitexprlst
	: constexpr								{ $$ = new ExpressionList($1); }
	| constinitexprlst COMMA constexpr		{ $$ = $1; $1.Add($3); }
	;

recordconst
	: LPAREN fieldconstlst scolopt RPAREN	{$$ = new RecordConst($2); }
	;

fieldconstlst
	: fieldconst							{ var ll = new FieldInitList(); ll.Add($1); $$ = ll;}
	| fieldconstlst SCOL fieldconst			{ $$ = $1; $1.Add($3); }
	;

fieldconst
	: id COLON constinitexpr				{ $$ = new FieldInit($1, $3); }
	;



	// ===================================================================================
	// Records
	// ===================================================================================
	
	// Only supports 'simple' structs, without class-like components

recordtypebasic
	: kwrecord  KW_END						{ $$ = null; /* TODO */ }
	| kwrecord  fieldlst scolopt KW_END		{ $$ = null; /* TODO */ }
	;
	
recordtype
	: recordtypebasic									{ $$ = null; /* TODO */ }
	| kwrecord  fieldlst SCOL recvariant scolopt KW_END	{ $$ = null; /* TODO */ }
	| kwrecord  recvariant scolopt KW_END				{ $$ = null; /* TODO */ }
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
	

	
	
	// ===================================================================================
	// Classes
	// ===================================================================================

	// Objects are treated as classes
classtype
	: kwclass heritage classbody KW_END	{ $$ = null; } // new ClassDefinition(0, $2, $3);  }
	| kwclass heritage					{ $$ = null; } // new ClassDefinition(0, $2, null);} // forward decl
	;

heritage
	:							{ $$ = new ArrayList(); }
	| LPAREN idlst RPAREN		{ $$ = $2; }		// inheritance from class and interf(s)			
	;

classbody
	: fieldlst SCOL	complst scopeseclst		{ $$ = null; } // new ClassBody(Scope.Public, $3, $4);  }
	|				complst scopeseclst		{ $$ = null; } // new ClassBody(Scope.Public, $1, $2);  }
	;

scopeseclst
	:							{ $$ = new DeclarationList(); }
	| scopeseclst scopesec		{ $$ = $1; $1.Add($2); }
	;

scopesec
	: scope_decl fieldlst SCOL complst	{ $$ = null; } // new ClassBody($1, $2, $4);  }
	| scope_decl			   complst	{ $$ = null; } // new ClassBody($1, null, $2);  }
	;
	
scope_decl
	: KW_PUBLISHED				{ $$ = Scope.Published; }
	| KW_PUBLIC					{ $$ = Scope.Public; }
	| KW_PROTECTED				{ $$ = Scope.Protected; }
	| KW_PRIVATE				{ $$ = Scope.Private; }
	;

fieldlst
	: objfield					{ $$ = new DeclarationList($1); }
	| fieldlst SCOL objfield	{ $$ = $1; $1.Add($3); }
	;
	
complst
	:							{ $$ = new DeclarationList(); }
	| complst classcomp			{ $$ = $1; $1.Add($2); }
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
	: kwinterf heritage guid classmethodlstopt classproplstopt KW_END	{ $$ = null; } // new InterfaceDefinition($2, $4, $5); }
	| kwinterf heritage classmethodlstopt classproplstopt KW_END		{ $$ = null; } // new InterfaceDefinition($2, $3, $4); }
	| kwinterf heritage %prec LOWESTPREC								{ $$ = null; /* TODO */ }
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
	: methoddecl				{ $$ = new DeclarationList($1); }
	| methodlst methoddecl		{ $$ = $1; $1.Add($2); }
	;

	
	
	// ===================================================================================
	// Properties
	// ===================================================================================
	
classproplstopt
	: classproplst				{ $$ = $1; }
	|							{ $$ = null; }
	;

classproplst
	: property					{ $$ = new NodeList($1); }
	| classproplst property		{ $$ = $1; $1.Add($2); }
	;

property
	: KW_PROPERTY id SCOL		{ $$ = null; }
	| KW_PROPERTY id propinterfopt COLON funcrettype propspecifiers SCOL defaultdiropt { $$ = null; } // new ClassProperty($2, $5, $6, $3, $8); }
	;

defaultdiropt
	:							{ $$ = null;}
	| id SCOL					{ $$ = null; CheckDirectiveId("default", $1); /* TODO */ }	// qualifier default != specifier default
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
	: indexopt readopt writeopt storedopt defaultopt implopt	{ $$ = null; } // new PropertySpecifiers($1, $2, $3, $4, $5, $6); }
	;

indexopt
	:							{ $$ = null; }
	| KW_INDEX constint			{ $$ = new PropertyIndex((uint) $2);  }
	;

storedopt
	:							{ $$ = null;  }
	| KW_STORED id				{ $$ = new PropertyStored($2); }
	;

defaultopt
	:							{ $$ = null;  }
	| KW_DEFAULT literal		{ $$ = new PropertyDefault($2); }
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


	
	
	// ===================================================================================
	// Types
	// ===================================================================================

typedecl
	: id KW_EQ typeopt vartype  SCOL						{ $$ = new TypeDeclaration($1, $4); }
	| id KW_EQ typeopt proceduraltype SCOL funcdirectopt	{ $$ = new TypeDeclaration($1, $4); $4.Directives = $6; }
	| id KW_EQ typeopt packcomptype SCOL					{ lastObjectName = $1; $$ = $4; }
	;

typeopt		// ignored for now
	:	
	| KW_TYPE
	;

vartype
	: scalartype				{ $$ = $1; }
	| enumtype					{ $$ = $1; }
	| rangetype					{ $$ = $1; }
	| metaclasstype				{ $$ = $1; }
	| packstructtype			{ $$ = $1; }
	;

packcomptype
	: KW_PACKED compositetype	{ $$ = $2; $2.IsPacked = true; }
	| compositetype				{ $$ = $1; }
	;

compositetype
	: classtype					{ $$ = $1; DeclRegistry.LeaveContext(); }
	| interftype				{ $$ = $1; DeclRegistry.LeaveContext(); }
	;

metaclasstype
	: KW_CLASS KW_OF id			{ $$ = new MetaclassType(DeclRegistry.FetchType($3)); }
	;

packstructtype
	: structuredtype			{ $$ = $1; }
	| KW_PACKED structuredtype	{ $$ = $2; $2.IsPacked = true; }
	;

structuredtype
	: arraytype					{ $$ = $1; }
	| settype					{ $$ = $1; }
	| filetype					{ $$ = $1; }
	| recordtype				{ $$ = $1; DeclRegistry.LeaveContext();}
	;
	
arrayszlst
	: rangetype					{ $$ = new TypeList($1); }
	| arrayszlst COMMA rangetype{ $$ = $1; $1.Add($3); }
	;

arraytype
	: TYPE_ARRAY LBRAC arrayszlst   RBRAC KW_OF vartype 	{ $$ = new ArrayType($6, $3); }
	| TYPE_ARRAY LBRAC id 			RBRAC KW_OF vartype		{ $$ = new ArrayType($6, DeclRegistry.FetchTypeIntegral($3)); }
	| TYPE_ARRAY KW_OF vartype 	{ $$ = new ArrayType($3); }
	;

settype
	: TYPE_SET KW_OF ordinaltype { /* TODO!! $$ = new SetType($3);*/ }
	;

filetype
	: TYPE_FILE KW_OF vartype 	{ $$ = new FileType($3); }
	| TYPE_FILE					{ $$ = new FileType(); }
	;

scalartype
	: id						{ $$ = DeclRegistry.FetchTypeScalar($1); }
	| TYPE_STR /*dynamic size*/	{ $$ = StringType.Single; }
	| TYPE_STR LBRAC constexpr RBRAC { $$ = new FixedStringType($3); }
	| pointertype				{ $$ = $1; }
	;
	
ordinaltype
	: rangetype					{ $$ = $1; }
	| enumtype					{ $$ = $1; }
	| id						{ $$ = DeclRegistry.FetchTypeIntegral($1); }
	;
	
casttype
	: id						{ $$ = DeclRegistry.FetchTypeScalar($1); }
	;

pointertype
	: KW_DEREF pointedtype 		{ $$ = new PointerType($2); }
	| TYPE_PTR					{ $$ = PointerType.Single; }
	;

	// Scalartype that allows for forward type declarations
pointedtype
	: id						{ $$ = new ScalarTypeForward($1); }
	| TYPE_STR /*dynamic size*/	{ $$ = StringType.Single; }
	| pointertype				{ $$ = $1; }
	;

funcparamtype
	: scalartype				{ $$ = $1; }
	| TYPE_ARRAY KW_OF scalartype	{ $$ = new ArrayType($3); }
	;

funcrettype
	: scalartype				{ $$ = $1; }
	;


%%

	}	// close parser class, opened in prolog	

// already defined in template
//} // close outermost namespace
