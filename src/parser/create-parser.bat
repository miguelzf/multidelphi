
csflex -v --nobak --csharp --skel CSFlex-CS-skeleton-nested.cs  DelphiLex.l

jay.exe -tvc < Jay-CS-skeleton.cs DelphiGrammar.y > DelphiParser.cs

::call msbuild.bat /verbosity:quiet DelphiParser.csproj

::@if %errorlevel% neq 0 goto :eof

::for /r ..\terra_engine_src\ %%a in ( *.pas ) do @parser-delphi.exe  %%a
::parser-delphi.exe  TERRA_AL.pas

