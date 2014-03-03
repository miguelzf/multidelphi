

csflex -v --csharp    DelphiLex.l

jay.exe -tvc < Jay-CS-skeleton.cs DelphiGrammar.y > DelphiParser.cs

::msbuild /verbosity:quiet DelphiParser.csproj

::parser-delphi.exe  <  TERRA_AL.pas

