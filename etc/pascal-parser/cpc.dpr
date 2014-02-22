Program cpc;
{$APPTYPE CONSOLE}

Uses CPC_Parser, CPC_CPP;
Begin
  AddPath('C:\compilers\terra_engine_src\Core');
  AddPath('C:\compilers\terra_engine_src\Graphics');
  AddPath('C:\compilers\terra_engine_src\Utils');
  AddPath('C:\compilers\terra_engine_src\Utils\FileIO');
  AddPath('C:\compilers\terra_engine_src\Utils\Math');
  AddPath('C:\compilers\terra_engine_src\OS\Windows');
  AddPath('C:\compilers\terra_engine_src\UI');
  AddPath('C:\compilers\terra_engine_src\Image');
  
  //CompileProject('test4.pas', CPPBuilder.Create);
  //CompileProject('test3.pas', CPPBuilder.Create);
  //CompileProject('test8.pas', CPPBuilder.Create);
  CompileProject('C:\compilers\terra_engine_src\Demos\basicSprites\sprite_simple.dpr', CPPBuilder.Create);
  
  //CompileProject('C:\compilers\terra_engine_src\Core\TERRA_Log.pas', CPPBuilder.Create);
  //CompileProject('lesson3a.pas', CPPBuilder.Create);
  
  WriteLn('Compiled sucessfully!');
  ReadLn;
End.