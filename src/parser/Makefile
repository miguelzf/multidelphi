# Use make -B to rebuild all targets regardless

DEPSDIR=..\..\deps
#add -l to disable #line emissions
JAYFLAGS=-tvc
CSFLEXFLAGS=-q --nobak --skel Flex-skeleton-nested.cs
#-debug
FLEXDEBUG=
DEBUGFLAG=$(shell grep "int +DefaultDebugLevel += +2"  ../core/Compiler.cs)

JAYEXEC=$(DEPSDIR)\cs-jay\jay.exe
LEXEXEC=$(DEPSDIR)\cs-jflex\csflex.exe

DEPS=DelphiPreprocessor.cs DelphiScanner.cs DelphiParser.cs


all: setvars $(DEPS)

setvars:
ifneq ($(DEBUGFLAG), )
FLEXDEBUG=-debug
endif

DelphiPreprocessor.cs: DelphiPreprocessor.l ..\core\Compiler.cs
	$(LEXEXEC) $(CSFLEXFLAGS) --partial $<

DelphiScanner.cs: DelphiLex.l ..\core\compiler.cs
	$(LEXEXEC) $(FLEXDEBUG) $(CSFLEXFLAGS)  $<

DelphiParser.cs: DelphiGrammar.y
	$(JAYEXEC) $(JAYFLAGS) < Jay-CS-skeleton.cs $< > $@

