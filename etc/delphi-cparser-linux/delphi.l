%option case-insensitive yylineno
%option outfile="delphi.lex.c"

%{

#include <stdlib.h>
#include "delphi.tab.h"
void yyerror(char *);

int insideObj = 0;

int linenum = 1;	// custom, adjusted for includes

	// executed for every match, before the rule's action
#ifdef YYDEBUG 
#if YYDEBUG
#define YY_USER_ACTION	printf("[line %d] READ: %s | 1st char: %c %d |\n",  \
								yylinenum, yytext, yytext[0], yytext[1]);
#endif
#endif

%}

	/* Delphi is case insensitive. Use solely lower case. */

%x XCOMMENT1 XCOMMENT2 ASMCODESEC

%%

	/* file type */
library		return KW_LIBRARY;
unit		return KW_UNIT;
program		return KW_PROGRAM;

	/* dependencies */
uses		return KW_USES;
exports		return KW_EXPORTS;

	/* units keywords */
interface	return KW_INTERF;
implementation	return KW_IMPL;
finalization	return KW_FINALIZ;
initialization	return KW_INIT;

	/* objects */
object		return KW_OBJECT;
record		return KW_RECORD;
class		return KW_CLASS;
packed		return KW_PACKED;		// packed struct
of			return KW_OF;			// decl of types

	/* functions */
function	return KW_FUNCTION;
procedure	return KW_PROCEDURE;
property	return KW_PROPERTY;		
inherited	return KW_INHERITED;	// call of base construcotr/methods

	/* section headers */
const		return KW_CONST;
var			return KW_VAR;
out			return KW_OUT;			// modifier of func params
threadvar	return KW_THRVAR;
type		return KW_TYPE;
constructor	return KW_CONSTRUCTOR;
destructor	return KW_DESTRUCTOR;

	/* blocks */
begin		return KW_BEGIN;
end			return KW_END;
with		return KW_WITH;		// with x do ..
do			return KW_DO;

	/* control flow: loops */
for			return KW_FOR;
to			return KW_TO;
downto		return KW_DOWNTO;
repeat		return KW_REPEAT;
until		return KW_UNTIL;
while		return KW_WHILE;

	/* control flow: others */
if			return KW_IF;
then		return KW_THEN;
else		return KW_ELSE;
case		return KW_CASE;		/* switch */
goto		return KW_GOTO;
label		return KW_LABEL;

	/* control flow: exceptions */
raise		return KW_RAISE;
at			return KW_AT;	// ex: raise excpt at address
try			return KW_TRY;
except		return KW_EXCEPT;
finally		return KW_FINALLY;
on			return KW_ON;


	/*****************************************************************
	 * Directives:
	 * Should be treated as identifiers instead of reserved keywords
	 *****************************************************************
	 */

	/*	properties keywords 
	default		return KW_DEFAULT;
	index		return KW_INDEX;
	implements	return KW_IMPLEMENTS;
	name		return KW_NAME;
	nodefault	return KW_NODEFAULT;
	read		return KW_READ;
	stored		return KW_STORED;
	write		return KW_WRITE;
	*/

	/* scope qualifiers */
protected	return KW_PROTECTED;
public		return KW_PUBLIC;
published	return KW_PUBLISHED;
private		return KW_PRIVATE;

	/* function call types */
cdecl		return KW_CDECL;
pascal		return KW_PASCAL;
mwpascal		return KW_PASCAL;
register	return KW_REGISTER;
safecall	return KW_SAFECALL;
stdcall		return KW_STDCALL;

	/* function qualifiers */
absolute	return KW_ABSOLUTE;
abstract	return KW_ABSTRACT;
assembler	return KW_ASSEMBLER;
dynamic		return KW_DYNAMIC;
export		return KW_EXPORT;
external	return KW_EXTERNAL;
forward		return KW_FORWARD;
inline		return KW_INLINE;
override	return KW_OVERRIDE;
overload	return KW_OVERLOAD;
reintroduce	return KW_REINTRODUCE;
virtual		return KW_VIRTUAL;
varargs		return KW_VARARGS;

	/* file warnings */
platform	return KW_PLATFORM;
deprecated	return KW_DEPRECATED;

	/* packages 
	package		return KW_PACKAGE;
	requires	return KW_REQUIRES;
	contains	return KW_CONTAINS;
	*/

	/* Embarcadero Delphi directives - ignored
	delayed experimental final
	helper operator reference
	sealed static strict unsafe
	*/

	/*		// windows/forms/COM specific - ignored

	winapi		return KW_WINAPI;
	message		return KW_MESSAGE;
	dispinterface	return KW_DISPINTERF;
	dispid		return KW_DISPID;
	automated	return KW_AUTOMATED;	// visibilidade
		// properties modifiers for dispinterface
	writeonly	return KW_WRITEONLY;
	readonly	return KW_READONLY;
		// DOS deprecated
	far			return KW_FAR;
	near		return KW_NEAR;
	resident	return KW_RESIDENT;
	*/


\{$[^}]+\}	;	/* ignore compiler funcdirectives */


	/* types: integers */
byte		return TYPE_BYTE;
shortint	return TYPE_SHORTINT;
word		return TYPE_WORD;
smallint	return TYPE_SMALLINT;
cardinal	return TYPE_CARDINAL;
integer		return TYPE_INT;
longword	return TYPE_LONGWORD;
longint		return TYPE_LONGINT;
int64		return TYPE_INT64;
uint64		return TYPE_UINT64;

	/* types: floats */
single		return TYPE_FLOAT;
real48		return TYPE_REAL48;
extended	return TYPE_EXTENDED;
double		return TYPE_DOUBLE;
real		return TYPE_DOUBLE;

	/* types: chars */
char		return TYPE_CHAR;
pchar		return TYPE_PCHAR;
ansichar	return TYPE_PCHAR;
widechar	return TYPE_WIDECHAR;
string		return TYPE_STR;
shortstring	return TYPE_SHORTSTR;
widestring	return TYPE_WIDESTR;
ansistring	return TYPE_STR;
resourcestring	return TYPE_RSCSTR;
 
	/* types others */
boolean		return TYPE_BOOL;
olevariant	return TYPE_OLEVAR;
variant		return TYPE_VAR;
comp		return TYPE_COMP;
currency	return TYPE_CURR;
array		return TYPE_ARRAY;
pointer		return TYPE_PTR;
file		return TYPE_FILE;
set			return TYPE_SET;
	

	/* ASM x86 */

asm						{	BEGIN(ASMCODESEC);
							return KW_ASM;
						}

<ASMCODESEC>[ \t\r]+	;
<ASMCODESEC>end;		{	// HACK!!! temporary. must change
							unput(';');
							BEGIN(INITIAL);
							return KW_END;
						}
<ASMCODESEC>\n			linenum++;
<ASMCODESEC>.			return ASM_OP;	// ASM unknown char



	/* literals: numeric */
[0-9]+			return CONST_INT;
\$[0-9a-z]+		return CONST_INT;		// convert hex=>int here
nil				return CONST_NIL;
[0-9]+\.[0-9]+	return CONST_REAL;
[0-9]+e[+-]?[0-9]+	return CONST_REAL;
[0-9]+\.[0-9]+e[+-]?[0-9]+	return CONST_REAL;
	
	/* literals: alpha */
\'.\'			return CONST_CHAR;
\#[0-9]+		return CONST_CHAR;
\#\$[0-9a-f]+	return CONST_CHAR;
''				return CONST_STR;
'[^']*'			return CONST_STR;

true			return CONST_BOOL;
false			return CONST_BOOL;


	/* separators */
":"			return COLON;
","			return COMMA;
";"			return SEMICOL;
"["			return LBRAC;
"("			return LPAREN;
"]"			return RBRAC;
")"			return RPAREN;


	/* expr: general */
":="		return KW_ASSIGN;
".."		return KW_RANGE;
as			return KW_AS;	// cast

	/* expr: unary pointer access */
"."			return KW_DOT;
"@"			return KW_ADDR;
"^"			return KW_DEREF;

	/* expr: numeric operators */
"-"			return KW_SUB;
"+"			return KW_SUM;
"/"			return KW_DIV;
"*"			return KW_MUL;
div			return KW_QUOT;
mod			return KW_MOD;

	/* expr: logical operators */
and			return KW_AND;
or			return KW_OR;
xor			return KW_XOR;
shl			return KW_SHL;
shr			return KW_SHR;
not			return KW_NOT;

	/* expr: comparison operators */
"<"			return KW_LT;
">"			return KW_GT;
">="		return KW_GE;
"<="		return KW_LE;
"="			return KW_EQ;
"<>"		return KW_DIFF;
"is"		return KW_IS;	// type comp
"in"		return KW_IN;	// is in set


[_a-z][_a-z0-9]*	return IDENTIFIER;


"//".*		;	// single line comment

				/* multi-line comments */
\{				BEGIN(XCOMMENT1);
<XCOMMENT1>\}	BEGIN(INITIAL);
<XCOMMENT1>.	;
<XCOMMENT1>\n	linenum++;

\(\*			BEGIN(XCOMMENT2);
<XCOMMENT2>\*\)	BEGIN(INITIAL);
<XCOMMENT2>.	;
<XCOMMENT2>\n	linenum++;



[ \r\t]+        ;       /* ignore whitespace */

\n				linenum++;

.               {	char buf[100];
					sprintf(buf, "Unknown character: %c, ascii %d", yytext[0], yytext[0]);
					yyerror(buf);
				}
				


%%

int yywrap(void) {
    return 1;
}



