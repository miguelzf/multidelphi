%{
using System;
using System.Linq;
using System.Collections;
using System.IO;
using System.Text;
using crosspascal.ast;
using crosspascal.ast.nodes;
using crosspascal.semantics;

namespace crosspascal.parser
{

	// Open main Parser class
	public partial class DelphiParser
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
			
		string lastObjName = null;	// keeps track of current class/object/interface being parsed
		
%}


	// =========================================================================
	// Rules declarations
	// =========================================================================

%start goal
%type<string> id qualifid idtypeopt labelid stringconst stringorchar stringnonnull conststr expid programid
%type<ArrayList> labelidlst idlst

	// sections and declarations
%type<TranslationUnit> goal file program library unit package
%type<UnitItem> containsitem usesitem expitem 
%type<NodeList> usesopt useslst requires requireslst containslst contains expitemlst
%type<DeclarationList> interfdecllst maindecllst declseclst interfdecl maindeclsec funcdeclsec basicdeclsec
%type<DeclarationList> thrvarsec rscstringsec exportsec labeldeclsec constsec
%type<DeclarationList> typesec varsec rscstringlst vardecl objfield
%type<ImplementationSection> implsec
%type<InterfaceSection> interfsec
%type<BlockStatement> initsec finalsec
%type<ConstDeclaration> constdecl rscstring
%type<TypeDeclaration> typedecl
%type<RoutineSection> funcdefine

	// functions
%type<CallableDeclaration> routinedeclmain
%type<RoutineDeclaration> routineproto routinedeclinterf
%type<MethodDeclaration> methoddecl classmetdecl interfmetdecl 
%type<MethodDefinition> methoddef metdefproto
%type<RoutineDefinition> routinedef
%type<RoutineDirectives> funcdirectlst funcdir_noterm_opt funcdiropt importdirforced 
%type<MethodDirectives> metdirectopt metdirectlst smetdirs smetdirslst
%type<int> funcdirective  funcqualif funcdeprecated metdirective metqualif routinecallconv smetqualif 
%type<MethodKind> kwmetspec
%type<DeclarationList> formalparamslst formalparm
%type<ParametersSection> formalparams
%type<ExternalDirective> externarg
%type<string> kwfunction kwprocedure

	// statements
%type<BlockStatement> block funcblock assemblerstmt	blockstmt
%type<ExceptionBlock> exceptionblock
%type<Statement> stmt nonlbl_stmt assign goto_stmt ifstmt casestmt elsecase caseselector 
%type<Statement> tryexceptstmt tryfinallystmt raisestmt ondef repeatstmt whilestmt forstmt withstmt 
%type<StatementList> caseselectorlst onlst stmtlist asmcode

	// expresssions
%type<Literal> literal basicliteral discrete strorcharlit
%type<StringLiteral> stringlit
%type<int> mulop addop relop 
%type<int> KW_EQ KW_GT KW_LT KW_LE KW_GE KW_NE KW_IN KW_IS KW_SUM KW_SUB KW_OR KW_XOR
%type<int> KW_MUL KW_DIV KW_QUOT KW_MOD KW_SHL KW_SHR KW_AS KW_AND
%type<ulong> constint constnil
%type<bool>   constbool
%type<double> constreal
%type<char>   constchar
%type<LvalueExpression> lvalue lvalstmt
%type<Identifier> identifier
%type<Expression> unaryexpr expr rangestart set setelem constexpr constinit paraminitopt
%type<ExpressionList> caselabellst exprlst constexprlst exprlstopt setelemlst arrayexprlst callparams
%type<TypeList> arrayszlst
%type<FieldInitList> fieldconstlst 
%type<FieldInit> fieldconst
%type<EnumValueList> enumelemlst
%type<EnumValue> enumelem
%type<ConstExpression> functypeinit arrayconst recordconst

	// Composites
%type<DeclarationList> recvariant recfield recvarfield recfieldlst propfield recvarlst 
%type<DeclarationList> ccomplstopt fieldlst cfieldlst classcomplst interfcomplst arrayprops
%type<ArrayList> heritage 
%type<Declaration> classcomp property interfcomp
%type<InterfaceType> packinterftype interftype
%type<ClassType> classtype packclasstype
%type<UnaryExpression> guid
%type<Scope> scope
%type<bool> staticopt defaultdiropt
%type<ObjectSection> classbody interfbody class1stsec scopeseclst
%type<String> implopt readopt writeopt
%type<IntLiteral> indexopt 
%type<Literal> defaultopt 
%type<ConstExpression> storeopt
%type<PropertySpecifiers> spropspecsnormal spropspecsoverride spropspecsarray

	// Types
%type<VariableType> paramtypeopt paramtypespec funcparamtype	// scalar or array
%type<VariableType> enumtype rangetype ordinaltype fixedtype
%type<TypeNode> vartype funcrettype funcret
%type<StructuredType> structuredtype packstructtype recordtype  arraytype settype filetype
%type<ProceduralType> proceduraltype proceduralsign
	
	
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
	//mainblocks
%token KW_BEGIN KW_END KW_WITH KW_DO
	// control flow loops
%token KW_FOR KW_TO KW_DOWNTO KW_REPEAT KW_UNTIL KW_WHILE
	// control flow others
%token KW_IF KW_THEN KW_ELSE KW_CASE KW_GOTO KW_LABEL KW_BREAK KW_CONTINUE
	// control flow exceptions
%token KW_RAISE KW_AT KW_TRY KW_EXCEPT KW_FINALLY KW_ON
	// function qualifiers
%token KW_ABSOLUTE KW_ABSTRACT KW_ASSEMBLER KW_DYNAMIC KW_EXPORT KW_EXTERNAL KW_FORWARD KW_INLINE KW_OVERRIDE KW_OVERLOAD KW_REINTRODUCE KW_VIRTUAL KW_VARARGS
	// function call conventions
%token KW_PASCAL KW_SAFECALL KW_STDCALL KW_CDECL KW_REGISTER
	// types
%token TYPE_WIDESTR TYPE_STR TYPE_RSCSTR TYPE_SHORTSTR	TYPE_ARRAY TYPE_FILE TYPE_PTR TYPE_SET
	// properties keywords
%token KW_NAME KW_READ KW_WRITE KW_INDEX KW_STORED KW_DEFAULT KW_NODEFAULT KW_IMPLEMENTS
	// pseudo, hints, windows-specific, deprecated, obscure, etc
%token ASM_OP WINDOWS_GUID  KW_FAR KW_NEAR KW_RESIDENT 



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

%nonassoc LBRAC RBRAC LPAR RPAR

%nonassoc MAXPREC

	//		Highest precedence ^
	// =========================================================================

%%


	// =========================================================================
	//
	// 						YACC/JAY Rules
	//
	// =========================================================================
	
	
goal: file KW_DOT		{ $$ = $1; ACCEPT(); }
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
	

	
	// =========================================================================
	// Top-level Sections
	// =========================================================================

program
	: programid	usesopt	maindecllst block	{ $$ = new ProgramNode($1, $2,   $3, $4); }
	| programid	usesopt             block	{ $$ = new ProgramNode($1, $2, null, $3); }
	;
	
programid
	: 							{ $$ = null; }
	| KW_PROGRAM id SCOL		{ $$ = $2;   }
	; 
	
library
	: KW_LIBRARY id SCOL usesopt maindecllst block		{ $$ = new LibraryNode($2, $4, $5  , $6); }
	| KW_LIBRARY id SCOL usesopt             block		{ $$ = new LibraryNode($2, $4, null, $5); }
	;
	
package
	: KW_PACKAGE id SCOL requires contains KW_END	{ $$ = new PackageNode($2, $4, $5); }
	;

requires
	:									{ $$ = new NodeList(); }
	| KW_REQUIRES requireslst SCOL		{ $$ = $2; }
	;
	
requireslst
	: id								{ $$ = new NodeList(new RequiresItem($1)); }
	| requireslst COMMA id				{ $$ = ListAdd<NodeList,Node>($1, new RequiresItem($3)); }
	;

contains
	: KW_CONTAINS containslst SCOL		{ $$ = $2; }
	;

containslst
	: containsitem						{ $$ = new NodeList($1); }
	| containslst COMMA containsitem	{ $$ = ListAdd<NodeList,Node>($1, $3); }
	;

containsitem
	: id						{ $$ = new ContainsItem($1); }
	| id KW_IN stringnonnull	{ $$ = new ContainsItem($1, $3); }
	;

usesopt
	:							{ $$ = new NodeList(); }
	| KW_USES useslst SCOL		{ $$ = $2; }
	;

useslst
	: usesitem					{ $$ = new NodeList($1); }
	| useslst COMMA usesitem	{ $$ = ListAdd<NodeList,Node>($1, $3); }
	;
	
usesitem
	: id						{ $$ = new UsesItem($1); }
	// TODO
	| id KW_IN stringnonnull	{ $$ = new UsesItem($1, $3);}
	;

unit
	: KW_UNIT id SCOL interfsec implsec initsec finalsec KW_END { $$ = new UnitNode($2, $4, $5, $6, $7); }
	| KW_UNIT id SCOL interfsec implsec initsec  KW_END			{ $$ = new UnitNode($2, $4, $5, $6, null); }
	| KW_UNIT id SCOL interfsec implsec finalsec KW_END			{ $$ = new UnitNode($2, $4, $5, null, $6); }
	| KW_UNIT id SCOL interfsec implsec          KW_END 		{ $$ = new UnitNode($2, $4, $5); }
	;

implsec
	: KW_IMPL usesopt	maindecllst		{ $$ = new ImplementationSection($2, $3);}
	| KW_IMPL usesopt					{ $$ = new ImplementationSection($2, null);}
	;

interfsec
	: KW_INTERF usesopt interfdecllst	{ $$ = new InterfaceSection($2, $3); }
	;

interfdecllst
	:							{ $$ = new DeclarationList();}
	| interfdecllst interfdecl	{ $$ = ListAddRange<DeclarationList,Declaration>($1, $2); }
	;

initsec
	: KW_INIT  blockstmt		{ $$ = $2;}
	| KW_BEGIN blockstmt		{ $$ = $2;}
	;
	
finalsec
	: KW_FINALIZ blockstmt 		{ $$ = $2;}
	;
		
maindecllst
	: maindeclsec				{ $$ = $1; }
	| maindecllst maindeclsec	{ $$ = ListAddRange<DeclarationList,Declaration>($1, $2); }
	;

declseclst
	: funcdeclsec				{ $$ = $1;}
	| declseclst funcdeclsec	{ $$ = ListAddRange<DeclarationList,Declaration>($1, $2); }
	;

	


	// =========================================================================
	// Declaration sections
	// =========================================================================

interfdecl
	: basicdeclsec			{ $$ = $1;}
	| routinedeclinterf		{ $$ = new DeclarationList($1);}
	| thrvarsec				{ $$ = $1;}
	| rscstringsec			{ $$ = $1;}
	;

maindeclsec
	: basicdeclsec			{ $$ = $1;}
	| thrvarsec				{ $$ = $1;}
	| exportsec				{ $$ = $1;}
	| routinedeclmain		{ $$ = new DeclarationList($1);}
	| labeldeclsec			{ $$ = $1;}
	;

funcdeclsec
	: basicdeclsec			{ $$ = $1;}
	| labeldeclsec			{ $$ = $1;}
	| routinedef			{ $$ = new DeclarationList($1);}
	;

basicdeclsec
	: constsec				{ $$ = $1;}
	| typesec				{ $$ = $1;}
	| varsec				{ $$ = $1;}
	;

typesec
	: KW_TYPE typedecl		{ $$ = new DeclarationList($2); }
	| typesec typedecl		{ $$ = ListAdd<DeclarationList,Declaration>($1, $2); }
	;


	// Variables

varsec
	: KW_VAR vardecl		{ $$ = $2; }
	| varsec vardecl		{ $$ = ListAdd<DeclarationList,Declaration>($1, $2); }
	;
	
thrvarsec
	: KW_THRVAR vardecl		{ $$ = $2;	foreach (VarDeclaration d in $2) d.isThrVar = true; }
	| thrvarsec vardecl		{ $$ = ListAdd<DeclarationList,Declaration>($1, $2); 
									foreach (VarDeclaration d in $2) d.isThrVar = true; }
	;

vardecl
	: idlst COLON vartype SCOL							{ $$ = CreateDecls($1, new CreateDecl((s) => new VarDeclaration(s, $3))); }
	| idlst COLON vartype KW_EQ constinit SCOL			{ $$ = CreateDecls($1, new CreateDecl((s) => new VarDeclaration(s, $3, $5))); }
	| idlst COLON vartype KW_ABSOLUTE id SCOL			{ $$ = CreateDecls($1, new CreateDecl((s) => new VarDeclaration(s, $3, $5))); }
	| idlst COLON proceduraltype SCOL funcdiropt		{ $$ = CreateDecls($1, new CreateDecl((s) => new VarDeclaration(s, $3)));
															$3.Directives = $5; }
	| idlst COLON proceduraltype SCOL
			funcdir_noterm_opt functypeinit SCOL		{ $$ = CreateDecls($1, new CreateDecl((s) => new VarDeclaration(s, $3, $6)));
															$3.Directives = $5; }
	;


	// Resourcestrings fom windows
	
rscstringsec
	: TYPE_RSCSTR rscstringlst		{ $$ = $2; }
	;
	
rscstringlst
	: rscstring						{ $$ = new DeclarationList($1); }
	| rscstringlst rscstring		{ $$ = ListAdd<DeclarationList,Declaration>($1, $2); }
	;
	
rscstring
	:  id KW_EQ stringlit SCOL		{ $$ = new ConstDeclaration($1, $3); }
	;

	
	// labels
	
labeldeclsec
	: KW_LABEL labelidlst SCOL		{ $$ = CreateDecls($2, new CreateDecl((s) => new LabelDeclaration(s))); }
	;
	
labelidlst 
	: labelid						{ $$ = new ArrayList(); ($$ as ArrayList).Add($1); }
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
	| expitemlst COMMA expitem		{ $$ = ListAdd<NodeList,Node>($1, $3); }
	;

expitem
	: expid formalparams 						{ $$ = new ExportItem($1, $2); }
	| expid formalparams KW_NAME  stringnonnull	{ $$ = new ExportItem($1, $2, $4); }
	| expid formalparams KW_INDEX constint		{ $$ = new ExportItem($1, $2, (int) $4); }
	;
	
expid
	: id				{ $$ = $1; }
	| id KW_DOT id		{ $$ = $1+"."+$3; }
	;



	
	// =========================================================================
	// Context-opening keywords
	// =========================================================================
	
	

kwclass
	: KW_CLASS			{ }
	| KW_OBJECT			{ }
	;



	// =========================================================================
	// Functions
	// =========================================================================

	// Prototypes/signatures
	// proc proto for definitions or external/forward decls
		
	// global routine decl or finition
routinedeclmain											// create defintion from type created in declaration rule
	: routinedef										{ $$ = $1; }
	| routineproto importdirforced						{ $$ = $1; $1.Directives = $2; }
	| methoddef funcdefine SCOL							{ $$ = $1; $1.body = $2; }
	;

routinedef
	: routineproto funcdiropt funcdefine SCOL			{ $$ = new RoutineDefinition($1.name, $1.Type, $2, $3); }
	;

methoddecl
	: kwfunction  formalparams funcret SCOL 			{ $$ = new MethodDeclaration(lastObjName, $1, $2, $3); }
	| kwprocedure formalparams         SCOL 			{ $$ = new MethodDeclaration(lastObjName, $1, $2, null); }
	;
	
routineproto
	: kwfunction  formalparams funcret	SCOL			{ $$ = new RoutineDeclaration($1, $2, $3); }
	| kwprocedure formalparams 			SCOL			{ $$ = new RoutineDeclaration($1, $2); }
	;	

methoddef
	:			metdefproto SCOL metdirectopt 			{ $$ = $1; $1.Directives = $3; }
	| KW_CLASS	metdefproto SCOL metdirectopt 			{ $$ = $2; $2.Directives = $4; $2.isStatic = true; }
	| kwmetspec id KW_DOT id formalparams SCOL smetdirs	{ $$ = new MethodDefinition($2, $4, $5, null, $7, $1);}
	;

metdefproto
	: kwfunction  KW_DOT id formalparams funcret 		{ $$ = new MethodDefinition($1, $3, $4, $5); }
	| kwprocedure KW_DOT id formalparams 				{ $$ = new MethodDefinition($1, $3, $4); }
	;
	
	// routine decl for interface sections
routinedeclinterf
	: routineproto 										{ $$ = $1; }
	;

classmetdecl
	: methoddecl metdirectopt							{ $$ = $1; $1.Directives = $2; }
	| kwmetspec id formalparams SCOL smetdirs			{ $$ = new MethodDeclaration(lastObjName, $2, $3, null, $5, $1);
															Console.WriteLine("CONSTRUCT METDECL: " + lastObjName); }
	;

interfmetdecl
	: methoddecl 										{ $$ = $1; }
	;

kwfunction
	: KW_FUNCTION  id	{ $$ = $2; }
	;
kwprocedure
	: KW_PROCEDURE id	{ $$ = $2; }
	;
kwmetspec
	: KW_CONSTRUCTOR 	{ $$ = MethodKind.Constructor; }
	| KW_DESTRUCTOR  	{ $$ = MethodKind.Destructor; }
	;
	
proceduraltype
	: proceduralsign									{ $$ = $1; }
	;
	
proceduralsign
	: KW_PROCEDURE formalparams 						{ $$ = new ProceduralType($2); }
	| KW_FUNCTION  formalparams funcret					{ $$ = new ProceduralType($2, $3); } 
	| KW_PROCEDURE formalparams KW_OF KW_OBJECT			{ $$ = new MethodType($2); } 
	| KW_FUNCTION  formalparams funcret KW_OF KW_OBJECT	{ $$ = new MethodType($2, $3); } 
	;

funcret
	: COLON funcrettype									{ $$ = $2;}
	;


	// Function blocks and parameters

funcdefine
	: declseclst funcblock				{ $$ = new RoutineSection($1, $2); }
	| 			 funcblock				{ $$ = new RoutineSection(null, $1); }
	;

funcblock
	: block								{ $$ = $1; }
	| assemblerstmt						{ $$ = $1; }
	;

formalparams
	:									{ $$ = new ParametersSection(); }
	| LPAR RPAR							{ $$ = new ParametersSection(); }
	| LPAR formalparamslst RPAR			{ $$ = new ParametersSection($2); }
	;

formalparamslst
	: formalparm						{ $$ = $1; }
	| formalparamslst SCOL formalparm	{ $$ = ListAdd<DeclarationList,Declaration>($1, $3); }
	;


formalparm
	: KW_VAR	idlst paramtypeopt					{ $$ = CreateDecls($2, new CreateDecl((s) => new VarParamDeclaration(s, $3))); } 
	| KW_OUT	idlst paramtypeopt					{ $$ = CreateDecls($2, new CreateDecl((s) => new OutParamDeclaration(s, $3))); } 
	| 			idlst paramtypespec paraminitopt	{ $$ = CreateDecls($1, new CreateDecl((s) => new ParamDeclaration(s, $2, $3))); }
	| KW_CONST	idlst paramtypeopt  paraminitopt	{ $$ = CreateDecls($2, new CreateDecl((s) => new ConstParamDeclaration(s, $3, $4))); }
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
	: KW_EQ id							{ $$ = new ConstIdentifier($2); }
	| KW_EQ constnil					{ $$ = new PointerLiteral($2); }
	;

	
	// Function directives

importdirforced
	: funcdiropt KW_EXTERNAL externarg SCOL funcdiropt	{ $$ = JoinImportDirectives($1, $5, $3); }
	| funcdiropt KW_FORWARD SCOL funcdiropt				{ $$ = JoinImportDirectives($1, $4, ImportDirective.Forward); }
	;

externarg
	: constexpr KW_NAME constexpr		{ $$ = new ExternalDirective($1, $3); }
	| constexpr							{ $$ = new ExternalDirective($1); }
	|            						{ $$ = null; }
	;

funcdir_noterm_opt
	:									{ $$ = null ; }
	| funcdirectlst						{ $$ = $1	; }
	;

funcdiropt
	:									{ $$ = null ; }
	| funcdirectlst SCOL				{ $$ = $1	; }
	;

metdirectopt
	:									{ $$ = null ; }
	| metdirectlst SCOL					{ $$ = $1	; }
	;

smetdirs
	:									{ $$ = null ; }
	| smetdirslst SCOL					{ $$ = $1	; }
	;

funcdirectlst
	: funcdirective						{ $$ = new RoutineDirectives($1); }
	| funcdirectlst SCOL funcdirective	{ $$ = $1; $1.Add($3); }
	;

smetdirslst
	: smetqualif						{ $$ = new MethodDirectives($1); }
	| smetdirslst SCOL smetqualif		{ $$ = $1; $1.Add($3); }
	;

metdirectlst
	: metdirective						{ $$ = new MethodDirectives($1); }
	| metdirectlst SCOL metdirective	{ $$ = $1; $1.Add($3); }
	;

smetqualif
	: KW_OVERLOAD			{ $$ = GeneralDirective.Overload; }
	| metqualif				{ $$ = $1; }
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
	

	
	// =========================================================================
	// Statements
	// =========================================================================

block
	: KW_BEGIN blockstmt KW_END		{ $$ = $2; }
	;

	// encapsulate the StatementList in a Statement itself
blockstmt
	: stmtlist				{ $$ = new BlockStatement(new StatementList($1.Reverse())); }
	;
	
	// Warning! the stmts are added in reverse order. The list must be reversed in the end.
	// Faster than doing List.Insertions, O(n) since List is build on an array
stmtlist
	: stmt					{ $$ = new StatementList($1); }
	| stmt SCOL stmtlist	{ $$ = ListAdd<StatementList,Statement>($3, $1); }
	;

stmt
	: nonlbl_stmt					{ $$ = $1; }
	| labelid COLON nonlbl_stmt		{ $$ = new LabelStatement($1, $3); }
	;

nonlbl_stmt
	:						{ $$ = new EmptyStatement(); }
	| lvalstmt				{ $$ = new ExpressionStatement($1); }
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

assign
	: lvalue KW_ASSIGN expr					{ $$ = new Assignment($1, $3); }
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
	| caseselectorlst SCOL caseselector		{ $$ = ListAdd<StatementList,Statement>($1, $3); }
	;

caseselector
	:										{ $$ = null; }
	| caselabellst COLON nonlbl_stmt		{ $$ = new CaseSelector($1, $3); }
	;
	
caselabellst
	: setelem							{ $$ = new ExpressionList($1); }
	| caselabellst COMMA setelem		{ $$ = ListAdd<ExpressionList,Expression>($1, $3); }
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
	| onlst ondef								{ $$ = ListAdd<StatementList,Statement>($1, $2); }
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




	
	// =========================================================================
	// Expressions
	// =========================================================================

identifier
	: id 									{ $$ = new Identifier($1); }
	;
	
lvalstmt
	: identifier							{ $$ = new UnresolvedId($1); }
	| lvalue LPAR exprlstopt RPAR			{ $$ = new UnresolvedCall($1, $3); }
	| TYPE_PTR LPAR expr RPAR				{ $$ = new StaticCast(PointerType.Single, $3); }
	| lvalue KW_DOT id						{ $$ = new ObjectAccess($1, $3); }
	| KW_INHERITED							{ $$ = new InheritedCall(null); }
	| KW_INHERITED id callparams			{ $$ = new InheritedCall($2, $3); }
	;
	
lvalue	// lvalue
	: identifier							{ $$ = new UnresolvedId($1); }
	| lvalue LPAR exprlstopt RPAR			{ $$ = new UnresolvedCall($1, $3); }
	| TYPE_PTR LPAR expr RPAR				{ $$ = new StaticCast(PointerType.Single, $3); }
	| lvalue KW_DOT id						{ $$ = new ObjectAccess($1, $3); }
	| lvalue KW_DEREF						{ $$ = new PointerDereference($1); }
	| lvalue LBRAC exprlst RBRAC			{ $$ = new ArrayAccess($1, $3); }
	| LPAR expr RPAR						{ $$ = new ExprAsLvalue($2); }
	;

unaryexpr
	: literal								{ $$ = $1; }
	| lvalue								{ $$ = $1; }
	| set									{ $$ = $1; }
	| KW_ADDR unaryexpr						{ $$ = new AddressLvalue($2); }
	| KW_NOT unaryexpr						{ $$ = new LogicalNot($2); }
	| KW_SUM unaryexpr 						{ $$ = new UnaryPlus($2); }
	| KW_SUB unaryexpr 						{ $$ = new UnaryMinus($2); }
	| KW_INHERITED							{ $$ = new InheritedCall(null); }
	| KW_INHERITED id callparams			{ $$ = new InheritedCall($2, $3); }
	| stringnonnull LBRAC expr RBRAC		{ $$ = new ArrayAccess(new ArrayConst($1), new ExpressionList($3)); }
	;

callparams
	:										{ $$ = new ExpressionList(); }
	| LPAR exprlstopt RPAR					{ $$ = $2; }
	;

expr
	: unaryexpr								{ $$ = $1; }
	| expr KW_AS qualifid					{ $$ = new RuntimeCast($1, new ClassRefType($3)); }
	| expr KW_IS qualifid					{ $$ = new TypeIs($1, new ClassRefType($3)); }
	| expr KW_IN expr						{ $$ = new SetIn($1, $3); }
	| expr relop expr %prec KW_EQ			{ $$ = CreateBinaryExpression($1, $2, $3); }
	| expr addop expr %prec KW_SUB			{ $$ = CreateBinaryExpression($1, $2, $3); }
	| expr mulop expr %prec KW_MUL			{ $$ = CreateBinaryExpression($1, $2, $3); }
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
	| exprlst COMMA expr		{ $$ = ListAdd<ExpressionList,Expression>($1, $3); }
	;

exprlstopt
	:							{ $$ = new ExpressionList(); }
	| exprlst					{ $$ = $1; }
	;
	

	

	// =========================================================================
	// Literals
	// =========================================================================

literal
	: basicliteral	{ $$ = $1; }
	| strorcharlit	{ $$ = $1; }
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

strorcharlit
	: stringorchar	{ $$ = (($1.Length==1)? (Literal) new CharLiteral($1[0]) : (Literal) new StringLiteral($1)); }
	;

stringlit
	: stringconst	{ $$ = new StringLiteral($1); }
	;

stringorchar
	: conststr					{ $$ = $1; }
	| constchar					{ $$ = ""+$1; }
	| stringorchar conststr		{ $$ = $1 + $2; }
	| stringorchar constchar	{ $$ = $1 + $2; }
	;
	
stringconst
	: stringorchar				{ $$ = $1; if ($1.Length == 1) yyerror("Expected string, found char"); }
	;

stringnonnull
	: stringconst				{ $$ = $1; if ($1.Length == 0) yyerror("Invalid empty string"); }
	;
	
idlst
	: id						{ $$ = new ArrayList(); ($$ as ArrayList).Add($1); }
	| idlst COMMA id			{ $$ = $1; $1.Add($3); }
	;

	// Cast the output value from the Scanner
	
id	: IDENTIFIER	{ $$ = (string) yyVal; }
	;
constnil
	: CONST_NIL		{ $$ = (ulong) 0; }
	; 
constint
	: CONST_INT		{ $$ = (ulong) yyVal;}
	;
constchar
	: CONST_CHAR	{ $$ = (char) (ulong) yyVal;}	// nasty unboxing
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

	


	// =========================================================================
	// Sets and Enums
	// =========================================================================

rangetype			// must be const
	: rangestart KW_RANGE expr		{ $$ = new RangeType($1, $3); $1.EnforceConst = $3.EnforceConst = true; }
	;
	
	// best effort to support constant exprs. TODO improve
rangestart
	: discrete						{ $$ = $1; }
//	| lvalue						{ $$ = null; /* TODO */ }
	| KW_SUM unaryexpr 				{ $$ = new UnaryPlus($2); }
	| KW_SUB unaryexpr 				{ $$ = new UnaryMinus($2); }
	;

enumtype
	: LPAR enumelemlst RPAR			{ $$ = new EnumType($2); }
	;

enumelemlst
	: enumelem						{ $$ = new EnumValueList($1); }
	| enumelemlst COMMA enumelem	{ $$ = ListAdd<EnumValueList,EnumValue>($1, $3); }
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
	| setelemlst COMMA setelem		{ $$ = ListAdd<ExpressionList,Expression>($1, $3); }
	;
	
setelem
	: constexpr						{ $$ = $1; }
	| constexpr KW_RANGE constexpr	{ $$ = new SetRange(new RangeType($1, $3)); }
	;



	// =========================================================================
	// Constants
	// =========================================================================

constsec
	: KW_CONST constdecl	{ $$ = new DeclarationList($2); }
	| constsec constdecl	{ $$ = ListAdd<DeclarationList,Declaration>($1, $2); }
	;

constdecl
	: id KW_EQ constexpr SCOL					{ $$ = new ConstDeclaration($1, $3); }	// true const
	| id COLON vartype KW_EQ constinit SCOL		{ $$ = new ConstDeclaration($1, $5, $3); }		// typed const
	| id COLON proceduraltype 
		funcdir_noterm_opt functypeinit SCOL	{ $$ = new ConstDeclaration($1, $5, $3); $3.Directives = $4; }
	;
	
constinit
	: constexpr				{ $$ = $1; }
	| arrayconst			{ $$ = $1; }
	| recordconst			{ $$ = $1; }
	;

constexpr
	: expr					{ $$ = $1; $1.EnforceConst = true; }
	;

constexprlst
	: constexpr						{ $$ = new ExpressionList($1); }
	| constexprlst COMMA constexpr	{ $$ = ListAdd<ExpressionList,Expression>($1, $3); }
	;

	// 1 or more exprs
arrayconst
	: LPAR constinit COMMA arrayexprlst RPAR	{ $$ = new ArrayConst($4); $4.InsertAt(0, $2); }
	;

arrayexprlst
	: constinit								{ $$ = new ExpressionList($1); }
	| arrayexprlst COMMA constinit			{ $$ = ListAdd<ExpressionList,Expression>($1, $3); }
	;

recordconst
	: LPAR fieldconstlst scolopt RPAR		{ $$ = new RecordConst($2); }
	;

fieldconstlst
	: fieldconst							{ $$ = new FieldInitList($1);}
	| fieldconstlst SCOL fieldconst			{ $$ = ListAdd<FieldInitList,Expression>($1, $3); }
	;

fieldconst
	: id COLON constinit					{ $$ = new FieldInit($1, $3); }
	;



	// =========================================================================
	// Records
	// =========================================================================
	
	// Records in Delphi7 only have fields and variants, no methods or properties

recordtype
	: KW_RECORD  KW_END									{ $$ = new RecordType(new DeclarationList()); }
	| KW_RECORD  fieldlst scolopt KW_END					{ $$ = new RecordType($2); }
	| KW_RECORD  fieldlst SCOL recvariant scolopt KW_END	{ $$ = new RecordType($2); }
	| KW_RECORD  recvariant scolopt KW_END				{ $$ = new RecordType($2); }
	;
	
recvariant
	: KW_CASE id COLON ordinaltype KW_OF recfieldlst	{ $$ = new DeclarationList(new VariantDeclaration($2, $4, $6)); }
	| KW_CASE          ordinaltype KW_OF recfieldlst	{ $$ = new DeclarationList(new VariantDeclaration(null, $2, $4)); }
	;

recfieldlst
	: recfield %prec LOWESTPREC							{ $$ = $1; }
	| recfield SCOL										{ $$ = $1; }
	| recfield SCOL recfieldlst							{ $$ = $1; $1.AddStart($1); }
	;

recfield
	: constexprlst COLON LPAR recvarlst scolopt RPAR	{ $$ = new DeclarationList($1.Select(x => 
																new VarEntryDeclaration(x as Expression, $4))); }
	;
	
recvarlst
	: recvarfield					{ $$ = $1; }
	| recvarlst SCOL recvarfield	{ $$ = ListAdd<DeclarationList,Declaration>($1, $3); }
	;

recvarfield
	: objfield					{ $$ = $1; }
	| recvariant				{ $$ = $1; }
	;
	

	
	
	// =========================================================================
	// Classes
	// =========================================================================

	// Objects are treated as classes
classtype
	: kwclass heritage classbody KW_END	{ $$ = new ClassType($2, $3); }
	| kwclass heritage					{ $$ = new ClassType($2); } // forward decl
	;

heritage
	:									{ $$ = new ArrayList(); }
	| LPAR idlst RPAR					{ $$ = $2; }		// inheritance from class and interf(s)			
	;

classbody
	: class1stsec scopeseclst			{ $$ = $1; $1.Add($2); }
	;

class1stsec
	: cfieldlst ccomplstopt 			{ $$ = new ObjectSection($1, $2); }
	|			ccomplstopt 			{ $$ = new ObjectSection(null, $1); }
	;
	
scopeseclst
	:											{ $$ = new ObjectSection(); }
	| scopeseclst scope ccomplstopt				{ $$ = $1; $1.AddDecls($3, $2); } 
	| scopeseclst scope cfieldlst ccomplstopt	{ $$ = $1; $1.AddDecls($4, $2); $1.AddFields($3, $2); } 
	;
	
scope
	: KW_PUBLISHED				{ $$ = Scope.Published; }
	| KW_PUBLIC					{ $$ = Scope.Public; }
	| KW_PROTECTED				{ $$ = Scope.Protected; }
	| KW_PRIVATE				{ $$ = Scope.Private; }
	;
	
cfieldlst
	: fieldlst SCOL					{ $$ = $1; }
	| KW_CLASS KW_VAR fieldlst SCOL { $$ = $3; foreach (Declaration d in $3) ((FieldDeclaration)d).isStatic = true; }
	;
	
fieldlst
	: objfield						{ $$ = $1; }
	| fieldlst SCOL objfield		{ $$ = ListAdd<DeclarationList,Declaration>($1, $3); }
	;
	
objfield
	: idlst COLON vartype			{ $$ = CreateDecls($1, new CreateDecl((s) => new FieldDeclaration(s, $3))); }
	| idlst COLON proceduraltype 	{ $$ = CreateDecls($1, new CreateDecl((s) => new FieldDeclaration(s, $3))); }
	| idlst COLON proceduraltype SCOL funcdirectlst	
									{ $$ = CreateDecls($1, new CreateDecl((s) => new FieldDeclaration(s, $3))); 
										$3.Directives = $5; }
	;
	
ccomplstopt
	:							{ $$ = new DeclarationList(); }
	| classcomplst				{ $$ = $1; }
	;
	
classcomplst
	: classcomp					{ $$ = new DeclarationList($1); }
	| classcomplst classcomp	{ $$ = ListAdd<DeclarationList,Declaration>($1, $2); }
	;
	
classcomp
	: staticopt classmetdecl	{ $$ = $2; $2.isStatic = $1; }
	| property					{ $$ = $1; }
	;

staticopt
	: 							{ $$ = false; }
	| KW_CLASS					{ $$ = true	; }
	;

interftype
	: KW_INTERF heritage guid interfbody KW_END	{ $$ = new InterfaceType($2, $4, $3); }
	| KW_INTERF heritage 						{ $$ = new InterfaceType($2); }
	;

interfbody
	: interfcomplst				{ $$ = new ObjectSection(null, $1, Scope.Public); }
	;
	
interfcomplst
	: interfcomp				{ $$ = new DeclarationList($1); }
	| interfcomplst interfcomp	{ $$ = ListAdd<DeclarationList,Declaration>($1, $2); }
	;
	
interfcomp
	: interfmetdecl				{ $$ = $1; }
	| property					{ $$ = $1; }
	;
	
guid
	:							{ $$ = null; }
	| LBRAC stringlit RBRAC		{ $$ = $2; }
	| LBRAC lvalue RBRAC		{ $$ = $2; }
	;

	
	
	
	// =========================================================================
	// Properties
	// =========================================================================
	
property
	// Normal case
	: KW_PROPERTY id COLON funcrettype spropspecsnormal SCOL	{ $$ = new PropertyDeclaration($2, $4, $5); }
	// Array properties
	| KW_PROPERTY id arrayprops COLON funcrettype spropspecsarray SCOL defaultdiropt { $$ = new ArrayProperty($2, $5, $3, $6, $8); }
	// Property overrides
	| KW_PROPERTY id spropspecsoverride SCOL 					{ $$ = new PropertyDeclaration($2, null, $3); }
	// Class properties: not supported in Delphi 7
	;

defaultdiropt
	:									{ $$ = false;}
	| id								{ $$ = CheckDirectiveId("default", $1); }
	;

arrayprops
	: LBRAC propfield RBRAC				{ $$ = $2; }
	;

propfield
	:			idlst COLON vartype		{ $$ = CreateDecls($1, new CreateDecl((s) => new VarParamDeclaration(s, $3))); }
	| KW_CONST  idlst COLON vartype		{ $$ = CreateDecls($2, new CreateDecl((s) => new ConstParamDeclaration(s, $4))); }
	;


	// Properties Specifiers
	
	// must have at least 1 read or write
spropspecsnormal
	: indexopt readopt writeopt storeopt defaultopt		{ $$ = new PropertySpecifiers($1, $2, $3, $4, $5); }
	;

	// must have at least 1 read or write
spropspecsarray
	: readopt writeopt 									{ $$ = new PropertySpecifiers($1, $2); }
	;

spropspecsoverride
	: indexopt readopt writeopt storeopt defaultopt implopt	{ $$ = new PropertySpecifiers($1, $2, $3, $4, $5, $6); }
	;
	
indexopt
	:							{ $$ = null; }
	| KW_INDEX constint			{ $$ = new IntLiteral($2);  }
	;

readopt
	:							{ $$ = null; }
	| KW_READ	id				{ $$ = $2; }
	;

writeopt
	:							{ $$ = null; }
	| KW_WRITE	id				{ $$ = $2; }
	;

storeopt
	:							{ $$ = new BoolLiteral(true);  }
	| KW_STORED id				{ $$ = new ConstIdentifier($2); ($$ as ConstIdentifier).Type = BoolType.Single; }
	| KW_STORED constbool		{ $$ = new BoolLiteral($2);  }
	;
	
defaultopt
	:							{ $$ = new IntLiteral(Int32.MaxValue);  }
	| KW_NODEFAULT				{ $$ = new IntLiteral(Int32.MaxValue);  }
	| KW_DEFAULT constexpr		{ $$ = $2; }
	;
	
implopt
	:							{ $$ = null;  }
	| KW_IMPLEMENTS id			{ $$ = $2; }
	;


	
	
	// =========================================================================
	// Types
	// =========================================================================

typedecl
	: idtypeopt vartype  SCOL					{ $$ = new TypeDeclaration($1, $2); }
	| idtypeopt proceduraltype SCOL funcdiropt	{ $$ = new TypeDeclaration($1, $2); $2.Directives = $4; }
	| idtypeopt packclasstype SCOL				{ $$ = new ClassDeclaration($1,$2); $2.Name = $1; }
	| idtypeopt packinterftype SCOL				{ $$ = new InterfaceDeclaration($1, $2); $2.Name = $1; }
	;

idtypeopt
	: id KW_EQ 	typeopt			{ $$ = lastObjName = $1; }
	;

typeopt		// ignored for now
	:
	| KW_TYPE
	;

vartype
 	: qualifid					{ $$ = new UnresolvedType($1); }
	| TYPE_STR /*dynamic size*/	{ $$ = StringType.Single; }
	| TYPE_STR LBRAC constexpr RBRAC { $$ = new FixedStringType($3); }
	| KW_DEREF vartype 			{ $$ = new PointerType($2); }
	| TYPE_PTR					{ $$ = PointerType.Single; }
	| KW_CLASS KW_OF qualifid	{ $$ = new MetaclassType(new ClassRefType($3)); }
	| packstructtype			{ $$ = $1; }
	| rangetype					{ $$ = $1; }
	| enumtype					{ $$ = $1; }
	;

ordinaltype
	: rangetype					{ $$ = $1; }
	| qualifid					{ $$ = new UnresolvedOrdinalType($1); }
	;

funcparamtype
	: qualifid							{ $$ = new UnresolvedVariableType($1); }
	| TYPE_ARRAY KW_OF funcparamtype	{ $$ = new ArrayType($3); }
	| TYPE_STR /*dynamic size*/			{ $$ = StringType.Single; }
	| TYPE_STR LBRAC constexpr RBRAC	{ $$ = new FixedStringType($3); }
	| TYPE_PTR							{ $$ = PointerType.Single; }
	;

funcrettype
	: qualifid					{ $$ = new UnresolvedType($1); }
	| TYPE_PTR					{ $$ = PointerType.Single; }
	| TYPE_STR /*dynamic size*/	{ $$ = StringType.Single; }
	;
	
	
	// Reference types
	
packclasstype
	: classtype					{ $$ = $1; }
	| KW_PACKED classtype		{ $$ = $2; $2.IsPacked = true; }
	;

packinterftype
	: interftype				{ $$ = $1; }
	| KW_PACKED interftype		{ $$ = $2; $2.IsPacked = true; }
	;

	
	// Structured Types
	
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
	: rangetype					{ $$ = new TypeList($1); }
	| arrayszlst COMMA rangetype{ $$ = ListAdd<TypeList,TypeNode>($1, $3); }
	;

arraytype
	: TYPE_ARRAY LBRAC arrayszlst RBRAC KW_OF vartype 	{ $$ = new ArrayType($6, $3); }
	| TYPE_ARRAY LBRAC qualifid	  RBRAC KW_OF vartype	{ $$ = new ArrayType($6, new UnresolvedIntegralType($3)); }
	| TYPE_ARRAY KW_OF vartype 	{ $$ = new ArrayType($3); }
	;

settype
	: TYPE_SET KW_OF ordinaltype{ $$ = new SetType($3);}
	;

filetype
	: TYPE_FILE KW_OF fixedtype	{ $$ = new FileType($3); }
	| TYPE_FILE					{ $$ = new FileType(); }
	;

fixedtype
	: qualifid							{ $$ = new UnresolvedVariableType($1); }
	| TYPE_STR LBRAC constexpr RBRAC	{ $$ = new FixedStringType($3); }
	| packstructtype					{ $$ = $1; }
	| rangetype							{ $$ = $1; }
	| proceduraltype SCOL funcdiropt	{ $$ = $1; $1.Directives = $3; }
	;
	
qualifid	// qualified
	: id							{ $$ = $1; }
	| id KW_DOT id					{ $$ = $1 + "." + $3; }
	;
	
%%

	}	// close parser class, opened in prolog	

// already defined in template
//} // close outermost namespace
