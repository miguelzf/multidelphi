@echo off

@set debuflag=

@grep -q  "int \+DebugLevel \+= \+2"  DelphiGrammar.y

@if %errorlevel% neq 0 goto :skip
set debuflag=--debug

:skip

csflex -q --nobak --partial --skel Flex-skeleton-nested.cs  DelphiPreprocessor.l

csflex -q %debuflag% --nobak --skel Flex-skeleton-nested.cs  DelphiLex.l

jay.exe -tvc < Jay-CS-skeleton.cs DelphiGrammar.y > DelphiParser.cs

call msbuild.bat /verbosity:quiet DelphiParser.csproj

@if %errorlevel% neq 0 goto :eof

::for /r ..\terra_engine_src\ %%a in ( *.pas ) do @parser-delphi.exe  %%a
parser-delphi.exe  TERRA_AL.pas

