
all: clean flex byacc gcc 

clean:
	rm -f *.tab.c lex.yy.c *.tab.h parser-delphi

	# build targets

flex:
	flex delphi.l

byacc:
	byacc -dtv -b delphi delphi.y 

bison:
	bison -Wall -b delphi -dtv delphi.y

gcc:
	gmcs PreProcessor-v3.cs -optimize
	gcc -O2 -Wall -g delphi.tab.c delphi.lex.c -o parser-delphi
# 	-DYYDEBUG=1


