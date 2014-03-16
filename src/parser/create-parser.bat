@echo off

@set debuflag=
@set jayemitlines=	
::-l

@grep -q  "int \+DefaultDebugLevel \+= \+2"  ..\core\Compiler.cs

@if %errorlevel% neq 0 goto :skip
set debuflag=--debug

:skip

..\..\deps\cs-jflex\csflex -q --nobak --partial --skel Flex-skeleton-nested.cs  DelphiPreprocessor.l

..\..\deps\cs-jflex\csflex -q %debuflag% --nobak --skel Flex-skeleton-nested.cs  DelphiLex.l

..\..\deps\cs-jay\jay.exe %jayemitlines% -tvc < Jay-CS-skeleton.cs DelphiGrammar.y > DelphiParser.cs


