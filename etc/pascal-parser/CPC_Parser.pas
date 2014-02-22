Unit CPC_Parser;

Interface
Uses TERRA_Utils, TERRA_FileManager, TERRA_OS, TERRA_IO, TERRA_FileIO, TERRA_FileUtils;

Const
  MaxIndices = 1024;

  functionProcedure = 0;
  functionFunction  = 1;
  functionConstructor = 2;
  functionDestructor = 3;

  visiblityPublic = 0;
  visiblityPrivate = 1;
  visiblityProtected = 2;

Type
  ModuleFunction = Class;
  VariableAcessor = Class;

  FunctionSignature = Class;

  VariableType = Record
    Primitive:String;
    BaseOffset:Integer;
    Count:Integer;
    IsPointer:Boolean;
    IsDynamicArray:Boolean;
    Module:String;
    Signature:FunctionSignature;
  End;

  Expression = Class;

  Variable = Class
    Name:String;
    Owner:Variable;
    VarType:VariableType;
    InitialValue:Expression;
    Parent:ModuleFunction;
    Childs:Array Of Variable;
    ChildCount:Integer;

    Function Nest:Integer;
    Procedure Emit; Virtual; Abstract;

    Function GetVariable(S:String; Ac:VariableAcessor):Variable;
    Function GetPath():String;
  End;

  Argument = Class(Variable)
    IsVar:Boolean;
    IsConst:Boolean;
  End;

  VariableBlock = Record
    Variables:Array Of Variable;
    VariableCount:Integer;
  End;

  FunctionSignature = Class
    Name:String;
    Owner:String;
    Genre:Integer;
    ResultType:VariableType;
    CallConvention:String;
    ExternalLocation:String;
    ExternalName:String;
    IsStatic:Boolean;
    IsVirtual:Boolean;
    IsAbstract:Boolean;
    IsOverride:Boolean;
    IsOverloaded:Boolean;
    Arguments:Array Of Argument;
    ArgumentCount:Integer;

    Procedure Emit(HasBody:Boolean); Virtual; Abstract;
  End;

  Constant = Class
    Name:String;
    Value:Expression;
    Init:String;
    VarType:VariableType;
    Parent:ModuleFunction;

    Function Nest:Integer;
    Procedure Emit; Virtual; Abstract;
  End;

  ConstantBlock = Record
    Constants:Array Of Constant;
    ConstantCount:Integer;
  End;

  VarType = Class
    Name:String;
    TypeName:String;
    Parent:ModuleFunction;
    IsPointer:Boolean;
    BaseOffset:Integer;
    Count:Integer;

    Function Nest:Integer;
    Procedure Emit; Virtual; Abstract;
  End;

  RecordType = Class(VarType)
    Variables:VariableBlock;
  End;

  EnumEntry = Record
    Name:String;
    Value:Integer;
  End;

  EnumType = Class(VarType)
    Values:Array Of EnumEntry;
    Count:Integer;
  End;

  FunctionType = Class(VarType)
    Signature:FunctionSignature;
  End;

  MethodList = Record
    Methods:Array Of FunctionSignature;
    MethodCount:Integer;
  End;

  ClassProperty = Class
    Name:String;
    Read:String;
    Write:String;
    Owner:String;
    PropType:VariableType;
    Procedure Emit; Virtual; Abstract;
  End;

  ClassSignature = Class(VarType)
    Ancestor:ClassSignature;

    PrivateVariables:VariableBlock;
    ProtectedVariables:VariableBlock;
    PublicVariables:VariableBlock;

    PrivateMethods:MethodList;
    ProtectedMethods:MethodList;
    PublicMethods:MethodList;

    Properties:Array Of ClassProperty;
    PropertyCount:Integer;

    Function GetVariable(S:String; Ac:VariableAcessor):Variable;
    Function GetMethod(S:String):FunctionSignature;
    Function GetProperty(S:String):ClassProperty;
  End;

  TypeBlock = Record
    Types:Array Of VarType;
    TypeCount:Integer;
  End;

  Statement = Class
    Parent:ModuleFunction;

    Function Nest:Integer;
    Procedure Emit; Virtual; Abstract;
    Function Reduce():Statement; Virtual; Abstract;
  End;

  StatementBlock = Class(Statement)
    Statements:Array Of Statement;
    StatementCount:Integer;

    Procedure AddStatement(St:Statement);
    Function Reduce():Statement; Override;
  End;

  Expression = Class(Statement)
    Procedure Emit; Virtual; Abstract;
    Function Reduce():Expression; Virtual; Abstract;
  End;

  ConstantExpression = Class(Expression)
    Value:String;
    Function Reduce():Expression; Override;
  End;

  CastExpression = Class(Expression)
    TypeName:String;
    Body:Expression;
    V:Variable;
    Acessor:VariableAcessor;
    Function Reduce():Expression; Override;
  End;

  VariableAcessor = Class
    Element:String;
    Index:Expression;
    Acessor:VariableAcessor;
    Parent:VariableAcessor;
    Dereferenced:Boolean;

    IsFunctionCall:Boolean;
    Arguments:Array Of Expression;
    ArgumentCount:Integer;

    Function GetPathBack:String;
    Function GetPathFront(First:Boolean=True):String;
    Procedure Emit; Virtual; Abstract;
    Procedure PostProcess(); Virtual;
  End;

  FunctionCall = Class(Statement)
    Acessor:VariableAcessor;
    Function Reduce():Statement; Override;
  End;


  VariableExpression = Class(Expression)
    V:Variable;
    Acessor:VariableAcessor;
    Function Reduce():Expression; Override;
  End;

  CallExpression = Class(Expression)
    Name:String;
    Obj:String;
    Acessor:VariableAcessor;
    Function Reduce():Expression; Override;
  End;

  UnaryExpression = Class(Expression)
    Op:String;
    A:Expression;
    Function Reduce():Expression; Override;
  End;

  BinaryExpression = Class(Expression)
    Op:String;
    A,B:Expression;
    Function Reduce():Expression; Override;
  End;

  Assignment = Class(Statement)
    V:Variable;
    Acessor:VariableAcessor;
    Cast:CastExpression;

    Result:Expression;
    Function Reduce():Statement; Override;
  End;

  IfStatement = Class(Statement)
    Condition:Expression;
    WhenTrue:Statement;
    WhenFalse:Statement;
    Function Reduce():Statement; Override;
  End;

  CaseEntry = Record
    Constants:Array Of String;
    ConstantCount:Integer;
    Result:Statement;
  End;

  CaseStatement = Class(Statement)
    Condition:Expression;
    Entries:Array Of CaseEntry;
    EntryCount:Integer;
    WhenFalse:Statement;
    Function Reduce():Statement; Override;
  End;

  IncrementStatement = Class(Statement)
    V:Variable;
    Acessor:VariableAcessor;
    Ammount:Expression;
    Negative:Boolean;
    Function Reduce():Statement; Override;
  End;

  WhileStatement = Class(Statement)
    Condition:Expression;
    Body:Statement;
    Function Reduce():Statement; Override;
  End;

  RepeatStatement = Class(Statement)
    Condition:Expression;
    Body:Statement;
    Function Reduce():Statement; Override;
  End;

  ForStatement = Class(Statement)
    V:Variable;
    Acessor:VariableAcessor;
    StartValue:Expression;
    EndValue:Expression;
    Body:Statement;
    Inverted:Boolean;
    Function Reduce():Statement; Override;
  End;

  WithStatement = Class(Statement)
    V:Variable;
    Acessor:VariableAcessor;
    Body:Statement;
    Parent:WithStatement;

    Function GetVariable(S:String; Ac:VariableAcessor):Variable;

    Function Reduce():Statement; Override;
  End;

  ModuleSection = Class;

  ModuleFunction = Class
    Signature:FunctionSignature;
    Nest:Integer;

    Statements:Statement;

    Variables:VariableBlock;
    Constants:ConstantBlock;
    Types:TypeBlock;

    Functions:Array Of ModuleFunction;
    FunctionCount:Integer;

    Parent:ModuleFunction;
    Section:ModuleSection;

    Procedure Emit; Virtual; Abstract;

    Function GetVariable(S:String; Ac:VariableAcessor):Variable;
    Function GetConstant(S:String):Constant;
    Function GetProperty(S:String):ClassProperty;
    Function GetType(S:String):VarType;

    Function EvaluateConstant(S: String): String;

    Procedure PostProcess(); Virtual;

    Function IsFunction(S:String):Boolean;
    Function IsConstant(S:String):Boolean;
    Function IsType(S:String):Boolean;
  End;

  Module = Class;

  ModuleSection = Class
    Parent:Module;
    Units:Array Of String;
    UnitCount:Integer;

    Variables:VariableBlock;
    Constants:ConstantBlock;
    Types:TypeBlock;

    IsImplementation:Boolean;

    Functions:Array Of ModuleFunction;
    FunctionCount:Integer;

    Body:Statement;
    Init:Statement;
    Final:Statement;

    Procedure Emit; Virtual; Abstract;

    Function GetVariable(S:String; Ac:VariableAcessor):Variable;
    Function GetConstant(S:String):Constant;
    Function GetType(S:String):VarType;

    Function IsFunction(S:String):Boolean;
    Function IsConstant(S:String):Boolean;
    Function IsType(S:String):Boolean;

    Function EvaluateConstant(S: String): String;

    Procedure CompileUses();

    Procedure PostProcess(); Virtual;
  End;

  ModuleBuilder = Class
    Function CreateIf:IFStatement; Virtual; Abstract;
    Function CreateWhile:WhileStatement; Virtual; Abstract;
    Function CreateRepeat:RepeatStatement; Virtual; Abstract;
    Function CreateFor:ForStatement; Virtual; Abstract;
    Function CreateWith:WithStatement; Virtual; Abstract;
    Function CreateCase:CaseStatement; Virtual; Abstract;
    Function CreateIncrement:IncrementStatement; Virtual; Abstract;
    Function CreateBlock:StatementBlock; Virtual; Abstract;
    Function CreateAcessor:VariableAcessor; Virtual; Abstract;
    Function CreateVariable:Variable; Virtual; Abstract;
    Function CreateArgument:Argument; Virtual; Abstract;
    Function CreateConstant():Constant; Virtual; Abstract;
    Function CreateAssignment:Assignment; Virtual; Abstract;
    Function CreateConstantExpression:Expression; Virtual; Abstract;
    Function CreateVariableExpression:Expression; Virtual; Abstract;
    Function CreateCallExpression:Expression; Virtual; Abstract;
    Function CreateUnaryOpExpression:Expression; Virtual; Abstract;
    Function CreateBinaryOpExpression:Expression; Virtual; Abstract;
    Function CreateCast:Expression; Virtual; Abstract;
    Function CreateType:VarType; Virtual; Abstract;
    Function CreateRecord:RecordType; Virtual; Abstract;
    Function CreateEnum:EnumType; Virtual; Abstract;
    Function CreateFunctionDefinition:FunctionType; Virtual; Abstract;
    Function CreateClass:ClassSignature; Virtual; Abstract;
    Function CreateProperty:ClassProperty; Virtual; Abstract;
    Function CreateFunctionCall():FunctionCall; Virtual; Abstract;
    Function CreateFunction:ModuleFunction; Virtual; Abstract;
    Function CreateFunctionSignature:FunctionSignature; Virtual; Abstract;
    Function CreateSection:ModuleSection; Virtual; Abstract;
    Procedure Compile(M:Module); Virtual; Abstract;
  End;

  Module = Class
    Name:String;
    InterfaceSection:ModuleSection;
    ImplementationSection:ModuleSection;
    Builder:ModuleBuilder;
    IsUnit:Boolean;
    Txt:String;

    CurrentToken:String;
    CurrentSection:ModuleSection;
    CurrentFunction:ModuleFunction;
    CurrentVisiblity:Integer;
    LastIndex:Integer;
    LastToken:String;
    CurrentLine:Integer;
    LastIndices:Array[0..Pred(MaxIndices)] Of Integer;
    Src:Stream;

    SymbolTable:Array Of String;
    SymbolCount:Integer;

    Defines:Array Of String;
    DefineCount:Integer;

    CurrentWith:WithStatement;

    Procedure CompileInterface(SrcFile:String; Builder:ModuleBuilder);
    Procedure CompileImplementation();
    Procedure Compile(IsInterface:Boolean);

    Procedure RevertToken(Src:Stream; LastIndex:Integer);
    Function Equals(S:String):Boolean;
    Function NextChar(Src:Stream; HaltOnEnd:Boolean):Char;
    Function InternalToken(Src:Stream; Var LastIndex:Integer; ReturnLineFeeds:Boolean=False; HaltOnEnd:Boolean=True):String;
    Function GetToken(Src:Stream; Var LastIndex:Integer; ReturnLineFeeds:Boolean=False; HaltOnEnd:Boolean=True):String;
    Procedure Expect(Token:String);
    Function IsNewSection():Boolean;
    Function LookAhead(Src:Stream; StopToken:String):String;

    Procedure ExpandVariable(V:Variable; Depth:Integer);

    Procedure LoadUsesClause(Section:ModuleSection);
    Procedure LoadVariables(Var Variables:VariableBlock);
    Procedure LoadConstants(Var Constants:ConstantBlock);
    Procedure LoadTypes(Var Types:TypeBlock);
    Function LoadFunction(Section:ModuleSection; FunctionGenre:Integer; IsStatic:Boolean):ModuleFunction;
    Function LoadExpression(Exp:String):Expression;
    Function LoadSingleExpression(Src:MemoryStream; Var LastIndex:Integer):Expression;
    Function LoadType():VariableType;
    Function LoadStatement():Statement;
    Function LoadProperty():ClassProperty;
    Function LoadAcessor(Parent:VariableAcessor; Src:Stream; ErrorOnFail:Boolean):VariableAcessor;
    Function LoadSignature(FunctionGenre:Integer; IsStatic:Boolean; Nameless:Boolean):FunctionSignature;
    Function FindClass(Name:String; ValidateError:Boolean):ClassSignature; Overload;
    Function FindClass(Section:ModuleSection; Name:String):ClassSignature; Overload;

    //Function CopyAcessor(Ac, Last:VariableAcessor):VariableAcessor;

    Function GetCurrentLine():Integer;

    Procedure AddIdentifier(S:String);
    Function GetIdentifier(S:String):String;

    Function GetVariable(S:String; Ac:VariableAcessor):Variable;
    Function GetType(S:String):VarType;
    Function EvaluateConstant(S:String):String;

    //Function ProcessAcessor(Ac:VariableAcessor; V:Variable):VariableAcessor;

    Function IsFunction(S:String):Boolean;
    Function IsConstant(S:String):Boolean;
    Function IsType(S:String):Boolean;

    Function SearchFile(S:String):String;

    procedure AddDefine(S: String);
    function IsDefined(S: String): Boolean;
    procedure RemoveDefine(S: String);

    Procedure PostProcess();
  End;

  ModuleList = Class
    ProjectName:String;
    Modules:Array Of Module;
    ModuleCount:Integer;
    Paths:String;
    PathCount:Integer;

    Function GetType(S:String):VarType;
    Function GetVariable(S:String; Ac:VariableAcessor):Variable;
  End;

Var
  Project:ModuleList;
  CurrentModule:Module;

Procedure CompileProject(MainSrc:String; Builder:ModuleBuilder);
Procedure Output(S:String; Nest:Integer; NewLine:Boolean);
Procedure OutputInclude(S:String);
Procedure Error(S:String);

Function IsNumber(S:String):Boolean;
Function IsString(S:String):Boolean;

Procedure SetOutputFile(S:String); Overload;
Procedure SetOutputFile(S:FileStream); Overload;
Function GetOutputFile():FileStream;

Function GetModule(Name:String):Module;

Procedure AddPath(Path:String);

Implementation

Type
  Buffer = Class
    Dest:FileStream;
    Includes:Array Of String;
    IncludeCount:Integer;
  End;

Var
  CurrentBuffer:Buffer;
  Buffers:Array Of Buffer;
  BufferCount:Integer;
  LastLookAhead:String;

Procedure AddPath(Path:String);
Begin
  FileManager.Instance.AddPath(Path);
End;

Function GetModule(Name:String):Module;
Var
  I:Integer;
Begin
  Name := LowStr(Name);
  For I:=0 To Pred(Project.ModuleCount) Do
  If (Project.Modules[I]<>Nil) And (LowStr(Project.Modules[I].Name)=Name) Then
  Begin
    Result := Project.Modules[I];
    Exit;
  End;

  Result := Nil;
End;

Procedure SetOutputFile(S:FileStream);
Var
  I:Integer;
Begin
  For I:=0 To Pred(BufferCount) Do
  If (Buffers[I].Dest = S) Then
  Begin
    CurrentBuffer := Buffers[I];
    Exit;
  End;
End;

Procedure SetOutputFile(S:String);
Var
  I:Integer;
Begin
  For I:=0 To Pred(BufferCount) Do
  If (GetFileName(Buffers[I].Dest.Name, False)=S) Then
  Begin
    CurrentBuffer := Buffers[I];
    Exit;
  End;

  S := 'output\'+S;
  Inc(BufferCount);
  SetLength(Buffers, BufferCount);
  CurrentBuffer := Buffer.Create;
  Buffers[Pred(BufferCount)] := CurrentBuffer;
  CurrentBuffer.Dest := FileStream.Create(S);
End;

Function GetOutputFile():FileStream;
Begin
  Result := CurrentBuffer.Dest;
End;

Var
  OutputBuffer:String;

Procedure Output(S:String; Nest:Integer; NewLine:Boolean);
Var
  I:Integer;
Begin
  For I:=1 To Nest Do
    S := #9 + S;

  If (CurrentBuffer.Dest=Nil) Then
  Begin
    Write(S);
    If NewLine Then
      WriteLn;
  End Else
  Begin
    OutputBuffer := OutputBuffer + S;
    If NewLine Then
    Begin
      CurrentBuffer.Dest.WriteLine(OutputBuffer);
      OutputBuffer := '';
    End;
  End;
End;

Procedure OutputInclude(S:String);
Var
  I:Integer;
  SS:String;
Begin
  SS := LowStr(S);
  For I:=0 To Pred(CurrentBuffer.IncludeCount) Do
  If (CurrentBuffer.Includes[I] = SS) Then
  Begin
    Exit;
  End;

  Inc(CurrentBuffer.IncludeCount);
  SetLength(CurrentBuffer.Includes, CurrentBuffer.IncludeCount);
  CurrentBuffer.Includes[Pred(CurrentBuffer.IncludeCount)] := SS;
  Output(S, 0, True);
End;

Procedure Module.RevertToken(Src:Stream; LastIndex:Integer);
Begin
  Src.Seek(LastIndex);
End;

Procedure Error(S:String);
Var
  Line:Integer;
  Dest:Stream;
Begin
  If CurrentModule=Nil Then
    WriteLn(S)
  Else
  Begin
    Line := CurrentModule.GetCurrentLine();
    WriteLn(CurrentModule.Name+': Error, line ',Line,': '+S);
    LastLookAhead := TrimLeft(TrimRight(LastLookAhead));
    If LastLookAhead<>'' Then
      WriteLn('Last lookahead: '+LastLookAhead);

    Dest := FileStream.Create('temp.pas');
    Dest.WriteLine(CurrentModule.Txt);
    Dest.Destroy;
  End;
  
  ReadLn;
  Halt;
End;

{Function Module.CopyAcessor(Ac, Last:VariableAcessor):VariableAcessor;
Begin
  If (Ac=Nil) Then
  Begin
    Result := Nil;
    Exit;
  End;
  Result := CurrentModule.Builder.CreateAcessor();
  Result.Element := Ac.Element;
  Result.Index := Ac.Index;
  If (Ac.Acessor = Nil) Then
    Result.Acessor := Last
  Else
    Result.Acessor := CopyAcessor(Ac.Acessor, Last);

  If Assigned(Result.Acessor) Then
    Result.Acessor.Parent := Result;
End;}

Function Module.Equals(S:String):Boolean;
Begin
  Result := UpStr(CurrentToken)=UpStr(S);
End;

Function Module.NextChar(Src:Stream; HaltOnEnd:Boolean):Char;
Begin
  If (Src.EOF) And (HaltOnEnd) Then
  Begin
    Error('Unexpected end of file.');
  End;

  Src.Read(@Result, 1);
End;

Function Module.GetToken(Src:Stream; Var LastIndex:Integer; ReturnLineFeeds,HaltOnEnd:Boolean):String;
Begin
  LastToken := CurrentToken;
  Result := InternalToken(Src, LastIndex, ReturnLineFeeds, HaltOnEnd);
//  If (LastToken<>CurrentToken) Then  WriteLn(Result);
End;

Function Module.InternalToken(Src:Stream; Var LastIndex:Integer; ReturnLineFeeds,HaltOnEnd:Boolean):String;
Var
  C, C2:Char;
  IsNum:Boolean;
  I:Integer;
Begin
  Result := '';
  For I:=Pred(MaxIndices) DownTo 1 Do
    LastIndices[I] := LastIndices[I-1];
  LastIndex := Src.Position;
  LastIndices[0] := LastIndex;
  //WriteLn(CurrentToken);
  Repeat
    If (Src.EOF) Then
    Begin
      Result := '';
      CurrentToken := Result;
      Exit;
    End;

    C := NextChar(Src,HaltOnEnd);
    If (C=#13) And (ReturnLineFeeds) Then
    Begin
      Result := crLf;
      CurrentToken := Result;
      Exit;
    End;

    If C='(' Then
    Begin
      C2 := NextChar(Src,HaltOnEnd);
      If (C2='*') Then
      Begin
        Repeat
          C2 := NextChar(Src,HaltOnEnd);
          If (C2='*') Then
          Begin
            C2 := NextChar(Src,HaltOnEnd);
            If (C2=')') Then
              Break
            Else
              Src.Skip(-1);
          End;
        Until False;
        C := #0;
        Continue;
      End Else
        Src.Skip(-1);
    End;

    If C='{' Then
    Begin
      C := NextChar(Src,HaltOnEnd);
      If (C='$') Then
      Begin
        Result := C;
        Repeat
          C := NextChar(Src,HaltOnEnd);
          If C='}' Then
            Break
          Else
            Result := Result + C;
        Until False;
        CurrentToken := Result;
        Exit;
      End;

      Src.Skip(-1);
      Repeat
        C := NextChar(Src,HaltOnEnd);
      Until C='}';
      C := #0;
    End;

    If C='/' Then
    Begin
      C2 := NextChar(Src,HaltOnEnd);
      If (C2='/') Then
      Begin
        Repeat
          C2 := NextChar(Src,HaltOnEnd);
        Until C2=#13;
        C := #0;
      End Else
      Begin
        Result := C;
        CurrentToken := Result;
        Src.Skip(-1);
        Exit;
      End;
    End;

    If C='''' Then
    Begin
      Result := C;
      Repeat
        C := NextChar(Src,HaltOnEnd);
        Result := Result+C;
        If (C='''') Then
        Begin
          If (Src.EOF) Then
          Begin
            CurrentToken := Result;
            Exit;
          End;

          C2 := NextChar(Src,HaltOnEnd);
          If (C2='''') Then
          Begin
            Result := Result+'''';
            C := #0;
          End Else
          Begin
            Src.Skip(-1);
          End;
        End;

      Until C='''';
      CurrentToken := Result;
      Exit;
    End;

  Until (C>#32);

  Case C Of
  '.',',',';','(',')','[',']','^','@','=','+','-','/','*':
    Begin
      Result := C;
      CurrentToken := Result;
      Exit;
    End;

  ':','>','<':
    Begin
      Src.Read(@C2, 1);
      If (C2='=') Or (C2='>') Then
      Begin
        Result := C+C2;
        CurrentToken := Result;
        Exit;
      End Else
      Begin
        Result := C;
        CurrentToken := Result;
        Src.Skip(-1);
        Exit;
      End;
    End;
  End;

  IsNum := False;
  Repeat
    If (C>='A') And (C<='Z') Then
      Result := Result + C
    Else
    If (C>='a') And (C<='z') Then
      Result := Result + C
    Else
    If (C='$') Then
      Result := Result + C
    Else
    If (C>='0') And (C<='9') Then
    Begin
      If Result = '' Then
        IsNum := True;
      Result := Result + C;
    End Else
    If (C='_') Or (C='#') Then
      Result := Result + C
    Else
    If (C='.') Then
    Begin
      If (Src.EOF) Then
      Begin
        CurrentToken := Result;
        Exit;
      End;

      C2 := NextChar(Src,HaltOnEnd);
      If (C2='.') Then
      Begin
        If (Result='.') Then
        Begin
          Result := '..';
          CurrentToken := Result;
          Exit;
        End Else
        Begin
          Src.Skip(-2);
          CurrentToken := Result;
          Exit;
        End;
      End Else
      If (IsNum) Then
      Begin
        Result := Result + C+C2;
      End Else
      Begin
        Src.Skip(-2);
        CurrentToken := Result;
        Exit;
      End;
    End Else
    Begin
      If (Result='') Then
        Error('Unexpected symbol: '+C);
      Src.Skip(-1);
      //Result := LowStr(Result);
      CurrentToken := Result;
      If GetCurrentLine()=15 Then
        IntToString(2);
      Exit;
    End;

    If (Src.EOF) Then
    Begin
      CurrentToken := Result;
      Exit;
    End;

    C := NextChar(Src,HaltOnEnd);
  Until (False);
End;

Procedure Module.Expect(Token:String);
Var
  S:String;
Begin
  S := GetToken(Src, LastIndex);
  If Not Equals(Token) Then
    Error('Expected '+Token+' -> Got '+S);
End;

Procedure CompileProject(MainSrc:String; Builder:ModuleBuilder);
Var
  I:Integer;
Begin
  If Assigned(Project) Then
    Project.Destroy;

  Project := ModuleList.Create;
  Project.ModuleCount := 1;
  SetLength(Project.Modules, 1);
  Project.Modules[0] := Module.Create();
  Project.Modules[0].CompileInterface(MainSrc, Builder);

  For I:=0 To Pred(Project.ModuleCount) Do
  Begin
    CurrentModule := Project.Modules[I];
    Project.Modules[I].CompileImplementation();
  End;

  For I:=0 To Pred(Project.ModuleCount) Do
  Begin
    CurrentModule := Project.Modules[I];
    Builder.Compile(CurrentModule);
  End;

  For I:=0 To Pred(BufferCount) Do
  Begin
    If (Buffers[I].Dest.Size<=0) Then
      Buffers[I].Dest.Delete();
    Buffers[I].Dest.Destroy;
    Buffers[I].Destroy;
  End;
End;

Procedure Module.CompileImplementation();
Begin
  Self.Compile(False);

  Self.PostProcess();

  Src.Destroy;
  Src :=Nil;
End;

Procedure Module.CompileInterface(SrcFile:String; Builder:ModuleBuilder);
Type
  DefineEntry = Record
    Name:String;
    Ignore:Boolean;
  End;

Var
  S, S2,S3:String;
  I,J,K,N,Count:Integer;
  Passing:Boolean;
  Dest:Stream;
  IsDirective:Boolean;
  Src2:MemoryStream;
  Defines:Array[0..1024] Of DefineEntry;
Begin
  Self.Name := GetFileName(SrcFile, False);
  CurrentModule := Self;

  If Not FileStream.Exists(SrcFile) Then
    Error('File not found: '+SrcFile);

  Self.Builder := Builder;
  Src := MemoryStream.Create(SrcFile);
  SetLength(Txt, Src.Size);
  Move(MemoryStream(Src).Buffer^, Txt[1], Src.Size);

  I := Pos('$I ', Txt);
  While I>0 Do
  Begin
    S := Copy(Txt, I +3, MaxInt);
    I := Pos('}', S);
    S := Copy(S, 1, Pred(I));
    S2 := SearchFile(S);
    If S2='' Then
      Error('Cannot find include file: '+S);

    Src2 := MemoryStream.Create(S2);
    S2 := '';
    While Not Src2.EOF Do
    Begin
      Src2.ReadLine(S3);
      S2 := S2+ ' '+S3;
    End;
    Src2.Destroy;

    ReplaceText('{$I '+S+'}', S2,Txt);
    I := Pos('$I ', Txt);
  End;
  Src.Destroy;
  Src := MemoryStream.Create(Length(TXT));
  Move(Txt[1], MemoryStream(Src).Buffer^, Length(TXT));

  Count := 0;
  N := 0;
  Repeat
    If (Src.EOF) Then
      Break;

    Defines[0].Ignore := False;
    N := Src.Position;
    S2 := GetToken(Src, LastIndex, True, False);
    If (S2='') Then
      Break;

      //WriteLn('module: '+CurrentModule.Name);
    If (S2[1]='$') And (Pos('log',CurrentModule.Name)>0) Then
      IntToString(2);

    //  writeln(S2);
    If (S2[1]='$') Then
    Begin
      S2 := LowStr(S2);
      S3 := GetNextWord(S2, ' ');
      IsDirective := False;
      If (S3='$define') Then
      Begin
        IsDirective := True;
        If (Not Defines[Count].Ignore) Then
          AddDefine(S2);
      End Else
      If (S3='$ifdef') Then
      Begin
        IsDirective := True;
        Inc(Count);
        Passing := True;
        Defines[Count].Name := S2;
        Defines[Count].Ignore := (Not IsDefined(S2));
      End Else
      If (S3='$ifndef') Then
      Begin
        IsDirective := True;
        Inc(Count);
        Passing := True;
        Defines[Count].Name := S2;
        Defines[Count].Ignore := (IsDefined(S2));
      End Else
      If (S3='$else') Then
      Begin
        IsDirective := True;
        Defines[Count].Ignore := Not Defines[Count].Ignore;
      End Else
      If (S3='$endif') Then
      Begin
        IsDirective := True;
        If (Count>0) Then
          Dec(Count);

        If (Count<=0) Then
          Passing := False;
      End Else
      If (S3='$i-') Or (S3='$r-') Or (Pos('$overflow',S3)>0) Then
      Begin
        IsDirective := True;
      End Else
      Begin
        StringToInt(S3);
      End;

      If (IsDirective) Or (Defines[Count].Ignore) Then
      Begin
        For J:=N+1 to Src.Position Do
          Txt[J] := ' ';
      End;
    End Else
    If (Defines[Count].Ignore) Then
    Begin
      For J:=N+1 to Src.Position Do
      If (Txt[J]<>#13) And (Txt[J]<>#10) Then
        Txt[J] := ' ';
    End;
  Until False;

  Src.Destroy ;

  Src := MemoryStream.Create(Length(Txt), @Txt[1]);
  Src.Seek(0);

  S := GetToken(Src, LastIndex);

  If Equals('unit') Then
    Self.IsUnit := True
  Else
  If Equals('program') Then
    Self.IsUnit := False
  Else
    Error('"program" expected');

  Self.Name := GetToken(Src, LastIndex);
  Expect(';');

  InterfaceSection := Builder.CreateSection;
  InterfaceSection.Parent := Self;
  InterfaceSection.IsImplementation := False;
  ImplementationSection := Builder.CreateSection;
  ImplementationSection.Parent := Self;
  ImplementationSection.IsImplementation := True;

  CurrentSection := ImplementationSection;
  Self.Compile(True);
End;

Procedure Module.Compile(IsInterface:Boolean);
Var
  IsStatic:Boolean;
  S:String;
Begin
      If (Name='TERRA_Image') Then
        IntToString(2);

  Repeat
    S := GetToken(Src, LastIndex);

    If S='' Then
      Break;

    S := LowStr(S);
    WritelN(Self.Name,':', Self.GetCurrentLine(),' ', S);

    If Equals('begin') Or Equals('initialization') Then
    Begin
      IntToString(2);
      Break;
    End;

    If Equals('end') Then
      Break;

    If Equals('uses') Then
    Begin
      LoadUsesClause(CurrentSection);
      CurrentSection.CompileUses();
      Continue;
    End;

    If Equals('interface') Then
    Begin
      CurrentSection := InterfaceSection;
      Continue;
    End;

    If Equals('implementation') Then
    Begin
      If (IsInterface) Then
      Begin
        RevertToken(Src, LastIndex);
        Exit;
      End;

      CurrentSection := ImplementationSection;
      Continue;
    End;

    If Equals('var') Then
    Begin
      LoadVariables(CurrentSection.Variables);
      Continue;
    End;

    If Equals('const') Then
    Begin
      If (Pos('Renderer',CurrentModule.Name)>0) Then
        IntToString(2);

      LoadConstants(CurrentSection.Constants);
      Continue;
    End;

    If Equals('type') Then
    Begin
      LoadTypes(CurrentSection.Types);
      Continue;
    End;

    If Equals('class') Then
    Begin
      S := GetToken(Src, LastIndex);
      IsStatic := True;
    End Else
      IsStatic := False;

    If Equals('procedure') Then
    Begin
      LoadFunction(CurrentSection, functionProcedure, IsStatic);
      Continue;
    End;

    If Equals('function') Then
    Begin
      LoadFunction(CurrentSection, functionFunction, IsStatic);
      Continue;
    End;

    If Equals('constructor') Then
    Begin
      LoadFunction(CurrentSection, functionConstructor, False);
      Continue;
    End;

    If Equals('destructor') Then
    Begin
      LoadFunction(CurrentSection, functionDestructor, False);
      Continue;
    End;

    If Equals('function') Then
    Begin
      LoadFunction(CurrentSection, functionDestructor, False);
      Continue;
    End;

    Error('Unexpected token: '+S);
  Until False;

  RevertToken(Src, LastIndex);
  If (Equals('begin')) Then
    CurrentSection.Body := LoadStatement()
  Else
  If (Equals('initialization')) Then
    CurrentSection.Init := LoadStatement();

  If (Not Src.EOF) Then
  Begin
    S := GetToken(Src, LastIndex);
    If Equals('finalization') Then
    Begin
      If Pos('Tween',CurrentModule.Name)>0 Then
        IntToString(2);

      RevertToken(Src, LastIndex);
      CurrentSection.Final := LoadStatement();
    End;
  End;
End;

Function IsNumber(S:String):Boolean;
Var
  I:Integer;
Begin
  Result := (S[1]>='0') And (S[1]<='9');
End;

Function IsString(S:String):Boolean;
Begin
  Result := (S[1]='''');
End;

Function Module.LoadSingleExpression(Src:MemoryStream; Var LastIndex:Integer):Expression;
Var
  S,S2,S3, Obj:String;
  A:Expression;
  Ac:VariableAcessor;
  C:Constant;
  I, Count, Temp, Temp2, Temp3:Integer;
  V:Variable;
  P:ClassProperty;
  T:VarType;
Begin
  Obj := '';
  Result := Nil;
  If Src.EOF Then
    Exit;

  Temp3 := Src.Position;
  S := GetToken(Src, LastIndex);
//  WriteLn(S);
  If S='' Then
    Exit;

  If S='Hex' Then
    IntToString(2);

  If (Equals('(')) Then
  Begin
    Count := 0;
    S := '';
    Repeat
      S2 := GetToken(Src, LastIndex);
      If (Equals(')')) Then
      Begin
        If (Count=0) Then
          Break
        Else
          Dec(Count);
      End Else
      If (Equals('(')) Then
        Inc(Count);

      S := S + S2+ ' ';
    Until False;

    Result := LoadExpression(S);
    Exit;
  End;

  If (Equals('-')) Or (Equals('+')) Or (Equals('not')) Or (Equals('@')) Then
  Begin
    If (Equals('not')) Then
      IntToString(2);

    Result := Builder.CreateUnaryOpExpression();
    UnaryExpression(Result).Op := S;
    UnaryExpression(Result).A := LoadSingleExpression(Src, LastIndex);
    Exit;
  End;

  T := Project.GetType(S);
  If Assigned(T) Then
  Begin
    Result := Builder.CreateCallExpression();
    CallExpression(Result).Name := LowStr(S);
    CallExpression(Result).Obj := Obj;
    RevertToken(Src, Temp3);
    CallExpression(Result).Acessor := LoadAcessor(Nil, Src, True);
    Exit;
  End;

  V := CurrentModule.GetVariable(S, Nil);
  If (Not Assigned(V)) And (Assigned(CurrentFunction)) Then
    V := CurrentFunction.GetVariable(S, Nil);
  If (Not Assigned(V)) Then
    V := Project.GetVariable(S, Nil);

  If Assigned(V) Then
  Begin
    Obj := S;
    RevertToken(Src, LastIndex);
    Temp := Src.Position;
    S2 := LookAhead(Src, ';');
    Result := Builder.CreateVariableExpression();
    VariableExpression(Result).V := V;
    VariableExpression(Result).Acessor := LoadAcessor(Nil, Src, True);
    //VariableExpression(Result).Acessor := ProcessAcessor(VariableExpression(Result).Acessor, V);
    GetToken(Src, LastIndex);
    RevertToken(Src, LastIndex);
    If Equals('(') Then
    Begin
      Src.Seek(Temp);
      S := GetToken(Src, LastIndex);
      GetToken(Src, LastIndex);
      If (Equals('.')) Then
        S := GetToken(Src, LastIndex)
      Else
        RevertToken(Src, LastIndex);
      Ac := VariableExpression(Result).Acessor;
    End Else
      Exit;
  End;

  C := CurrentModule.CurrentSection.GetConstant(S);
  If (Not Assigned(C)) And (Assigned(CurrentFunction)) Then
    C := CurrentFunction.GetConstant(S);
  If Assigned(C) Then
  Begin
    Obj := S;
    Temp := Src.Position;
    S2 := LookAhead(Src, ';');
    If (Pos('[', S2)>0) Then
    Begin
      RevertToken(Src, LastIndex);
      Result := Builder.CreateVariableExpression();
      VariableExpression(Result).V := Nil;
      VariableExpression(Result).Acessor := LoadAcessor(Nil, Src, True);
    //VariableExpression(Result).Acessor := ProcessAcessor(VariableExpression(Result).Acessor, V);
      Exit;
    End;
  End;

  P := Nil;
  If (Assigned(CurrentFunction)) Then
    P := CurrentFunction.GetProperty(S);
  If (S='Camera') Or (S='ActiveViewport') Then
    IntToString(2);

  If Assigned(P) Then
  Begin
    Temp := Src.Position;
    S2 := LookAhead(Src, ';');
    //If (Pos('[', S2)>0) Then
    Begin
      RevertToken(Src, LastIndex);
      Result := Builder.CreateVariableExpression();
      VariableExpression(Result).V := Nil;
      VariableExpression(Result).Acessor := LoadAcessor(Nil, Src, True);
    //VariableExpression(Result).Acessor := ProcessAcessor(VariableExpression(Result).Acessor, V);
      Exit;
    End;
  End;

  If (Not Src.EOF) Then
  Begin
    Temp := LastIndex;
    S2 := GetToken(Src, LastIndex);
    If (Equals('.')) Then
    Begin
      Obj := S;
      Temp2 := LastIndex;
      S := GetToken(Src, LastIndex);
      If (Not Src.EOF) Then
        GetToken(Src, LastIndex);
      RevertToken(Src, LastIndex);
      If (Equals('(')) Then
      Begin
        CurrentModule.AddIdentifier(S);
        //RevertToken(Src, Temp2);
      End Else
        Obj := '';
    End Else
      RevertToken(Src, LastIndex);

    S3 := LookAhead(Src, ';');
    If (Pos('(',S3)>0) Then
    Begin
      Result := Builder.CreateCallExpression();
      CurrentModule.AddIdentifier(S);
      CurrentModule.AddIdentifier(Obj);
      CallExpression(Result).Name := LowStr(S);
      CallExpression(Result).Obj := Obj;
      RevertToken(Src, Temp3);
      CallExpression(Result).Acessor := LoadAcessor(Nil, Src, True);
      Exit;
    End Else
    Begin
      RevertToken(Src, LastIndex);
      LastIndex := Temp;
    End;
  End;

  V := CurrentModule.GetVariable(S, Nil);

  If Assigned(V) Then
  Begin
    RevertToken(Src, LastIndex);
    Result := Builder.CreateVariableExpression();
    VariableExpression(Result).V := V;
    VariableExpression(Result).Acessor := LoadAcessor(Nil, Src, True);
    //VariableExpression(Result).Acessor := ProcessAcessor(VariableExpression(Result).Acessor, V);
    Exit;
  End;

  Result := Builder.CreateConstantExpression();
  CurrentModule.AddIdentifier(S);
  ConstantExpression(Result).Value := S;
End;

Function Module.LoadExpression(Exp: String): Expression;
Var
  S, S2:String;
  Src:MemoryStream;
  Count:Integer;
  A,B:Expression;
  Op:String;
  LastIndex:Integer;
Begin
  Exp := TrimRight(TrimLeft(Exp));
  If (Exp='') Then
  Begin
    Result := Nil;
    Exit;
  End;

  //WriteLn('Expression: '+Exp);
  If (Pos('DevMode',Exp)>0) Then
    IntToString(2);


  Src := MemoryStream.Create(Length(Exp), @Exp[1]);
  Src.Seek(0);
  LastIndex := 0;
  A := LoadSingleExpression(Src, LastIndex);
  If Not Src.EOF Then
  Begin
    Op := LowStr(GetToken(Src, LastIndex));
    If (Op='=') Or (Op='+') Or (Op='-') Or (Op='*') Or (Op='/') Or (Op='div') Or (Op='or') Or (Op='and') Or (Op='xor')
    Or (Op='>') Or (Op='<') Or (Op='<=') Or (Op='>=') Or (Op='<>') Or (Op='mod') Or (Op='shl') Or (Op='shr')
    Or (Op='is') Then
    Begin
      B := LoadSingleExpression(Src, LastIndex);
      Result := Builder.CreateBinaryOpExpression();
      BinaryExpression(Result).A := A;
      BinaryExpression(Result).B := B;
      BinaryExpression(Result).Op := Op;
    End Else
      Error('Unexpected operator: '+Op);
  End Else
    Result := A;
  //Result.Value := Exp;
  Src.Destroy;
End;

Function Module.LoadFunction(Section: ModuleSection; FunctionGenre:Integer; IsStatic:Boolean):ModuleFunction;
Var
  S, S2:String;
  Func, Parent, Temp:ModuleFunction;
Begin
  Parent := CurrentFunction;
  CurrentFunction := Builder.CreateFunction;
  CurrentFunction.Parent := Parent;
  CurrentFunction.Section := Section;
  If Assigned(Parent) Then
    CurrentFunction.Nest := Parent.Nest + 1
  Else
    CurrentFunction.Nest := 0;
  Func := CurrentFunction;
  CurrentFunction.Signature := LoadSignature(FunctionGenre, IsStatic, False);
  WriteLn(CurrentFunction.Signature.Name);

  If (CurrentFunction.Signature.Name='getkeyname') Then
    IntToString(2);

  If (CurrentSection.IsImplementation) Then
  Begin
    Inc(Section.FunctionCount);
    SetLength(Section.Functions, Section.FunctionCount);
    Section.Functions[Pred(Section.FunctionCount)] := CurrentFunction;
    Result := CurrentFunction;
  End;

  Repeat
    S := GetToken(Src, LastIndex);
    If (Equals('procedure')) And (Section.IsImplementation) Then
    Begin
      Temp := LoadFunction(Section, functionProcedure, False);
      Inc(CurrentFunction.FunctionCount);
      SetLength(CurrentFunction.Functions, CurrentFunction.FunctionCount);
      CurrentFunction.Functions[Pred(CurrentFunction.FunctionCount)] := Temp;
    End Else
    If (Equals('function')) And (Section.IsImplementation) Then
    Begin
      Temp := LoadFunction(Section, functionFunction, False);
      Inc(CurrentFunction.FunctionCount);
      SetLength(CurrentFunction.Functions, CurrentFunction.FunctionCount);
      CurrentFunction.Functions[Pred(CurrentFunction.FunctionCount)] := Temp;
    End Else
    If (Equals('var'))  And (Section.IsImplementation) Then
    Begin
      Self.LoadVariables(CurrentFunction.Variables);
    End Else
    If (Equals('const'))  And (Section.IsImplementation) Then
    Begin
      Self.LoadConstants(CurrentFunction.Constants);
    End Else
    If (Equals('begin'))  And (Section.IsImplementation) Then
    Begin
      RevertToken(Src, LastIndex);
      CurrentFunction.Statements := LoadStatement();
      Exit;
    End Else
    Begin
      RevertToken(Src, LastIndex);
      Break;
    End;
  Until False;

  CurrentFunction := Parent;
End;

Function Module.LoadStatement: Statement;
Var
  CastName:String;
  S, S2:String;
  St,St2:Statement;
  V:Variable;
  I, N, Temp, Temp2:Integer;
  Ac,Ac2:VariableAcessor;
  T:VarType;
Begin
  If (GetCurrentLine>=390) And (CurrentModule.Name='TERRA_Classes') Then
    IntToString(2);

  Result := Nil;
  S := GetToken(Src, LastIndex);
  If (Equals('begin')) Or (Equals('initialization')) Or (Equals('finalization')) Then
  Begin
    Result := Builder.CreateBlock;
    Repeat
      S := GetToken(Src, LastIndex);
      If Equals('finalization') Then
      Begin
        IntToString(2);
        RevertToken(Src, LastIndex);
        Break;
      End;

      If Equals('end') Then
      Begin
        IntToString(2);
        Break;
      End;
      RevertToken(Src, LastIndex);

      St := LoadStatement();
      StatementBlock(Result).AddStatement(St);
    Until False;

    S := GetToken(Src, LastIndex);
    If Equals('finalization') Then
    Begin
      RevertToken(Src, LastIndex);
      Exit;
    End Else
    If Equals('else') Then
    Begin
      IntToString(2);
    End Else
    If (S<>';') And (S<>'.') Then
    Begin
      RevertToken(Src, LastIndex);
      Expect(';');
    End;
  End Else
  If Equals('case') Then
  Begin
    S := '';
    Repeat
      S2 := GetToken(Src, LastIndex);
      If (Equals('of')) Then
        Break;

      S := S + S2;
    Until False;

    Result := Builder.CreateCase();
    CaseStatement(Result).Condition := LoadExpression(S);
    CaseStatement(Result).EntryCount := 0;
    CaseStatement(Result).WhenFalse := Nil;
    N := 0;
    Repeat
      S2 := GetToken(Src, LastIndex);
      If (Equals('end')) Then
      Begin
        IntToString(2);
        Break;
      End;

      If (Equals('else')) Then
      Begin
        CaseStatement(Result).WhenFalse := LoadStatement();
        Continue;
      End;

      If (Equals(',')) Then
        Continue;

      If (Equals(':')) Then
      Begin
        CaseStatement(Result).Entries[N].Result := LoadStatement();
        Inc(N);
        Continue;
      End;

      If (N>=Length(CaseStatement(Result).Entries)) Then
      Begin
        Inc(CaseStatement(Result).EntryCount);
        SetLength(CaseStatement(Result).Entries, CaseStatement(Result).EntryCount);
      End;

      Inc(CaseStatement(Result).Entries[N].ConstantCount);
      SetLength(CaseStatement(Result).Entries[N].Constants, CaseStatement(Result).Entries[N].ConstantCount);
      CaseStatement(Result).Entries[N].Constants[Pred(CaseStatement(Result).Entries[N].ConstantCount)] := S2;

    Until False;
    Expect(';');
  End Else
  If Equals('if') Then
  Begin
    S := '';
    Repeat
      S2 := GetToken(Src, LastIndex);
      If (Equals('then')) Then
        Break;
      S := S + S2+ ' ';
    Until False;

    If (GetCurrentLine()>=274) And (Pos('Options',S)>0) Then
      IntToString(2);

    Result := Builder.CreateIF();
    IfStatement(Result).Condition := LoadExpression(S);

    IfStatement(Result).WhenTrue := LoadStatement();
    S2 := GetToken(Src, LastIndex);
    If (Equals('else')) Then
    Begin
      IfStatement(Result).WhenFalse := LoadStatement();
    End Else
    Begin
      IfStatement(Result).WhenFalse := Nil;
      RevertToken(Src, LastIndex);
    End;

  End Else
  If Equals('while') Then
  Begin
    Result := Builder.CreateWhile();
    S := '';
    Repeat
      S2 := GetToken(Src, LastIndex);
      If (Equals('do')) Then
        Break;
      S := S + S2+ ' ';
    Until False;
    WhileStatement(Result).Condition := LoadExpression(S);
    WhileStatement(Result).Body := LoadStatement();
  End Else
  If Equals('repeat') Then
  Begin
    Result := Builder.CreateRepeat();
    RepeatStatement(Result).Body := Builder.CreateBlock;
    Repeat
      S := GetToken(Src, LastIndex);
      If Equals('until') Then
      Begin
        IntToString(2);
        Break;
      End;
      RevertToken(Src, LastIndex);

      St := LoadStatement();
      StatementBlock(RepeatStatement(Result).Body).AddStatement(St);
    Until False;

    S := '';
    Repeat
      S2 := GetToken(Src, LastIndex);
      If (Equals(';')) Then
        Break;
      S := S + S2+ ' ';
    Until False;
    RepeatStatement(Result).Condition := LoadExpression(S);
  End Else
  If Equals('for') Then
  Begin
    Result := Builder.CreateFor();
    S := GetToken(Src, LastIndex);
    V := CurrentModule.GetVariable(S, Nil);
    If (V=Nil) And (Assigned(CurrentFunction)) Then
      V := CurrentFunction.GetVariable(S, Nil);

    If Assigned(V) Then
    Begin
      RevertToken(Src, LastIndex);
      ForStatement(Result).V := V;
      ForStatement(Result).Acessor := LoadAcessor(Nil, Src, True);
      //ForStatement(Result).Acessor := ProcessAcessor(ForStatement(Result).Acessor, V);
    End Else
      Error('Could not find acessor '+S);

    Expect(':=');

    S := '';
    Repeat
      S2 := GetToken(Src, LastIndex);
      If (Equals('to')) Then
      Begin
        ForStatement(Result).Inverted := False;
        Break;
      End;

      If (Equals('downto')) Then
      Begin
        ForStatement(Result).Inverted := True;
        Break;
      End;

      S := S + S2+ ' ';
    Until False;

    ForStatement(Result).StartValue := LoadExpression(S);

    S := '';
    Repeat
      S2 := GetToken(Src, LastIndex);
      If (Equals('do')) Then
        Break;

      S := S + S2+ ' ';
    Until False;

    ForStatement(Result).EndValue := LoadExpression(S);
    ForStatement(Result).Body := LoadStatement();
  End Else
  If (Equals('with')) Then
  Begin
    Result := Builder.CreateWith();
    WithStatement(Result).Parent := CurrentModule.CurrentWith;

    S := GetToken(Src, LastIndex);
    V := CurrentModule.GetVariable(S, Nil);
    RevertToken(Src, LastIndex);

    If (Not Assigned(V)) And (Assigned(CurrentFunction)) Then
      V := CurrentFunction.GetVariable(S, Nil);

    If Not Assigned(V) Then
      Error('Unknown variable '+S);

    WithStatement(Result).V := V;
    WithStatement(Result).Acessor := LoadAcessor(Nil, Src, True);
    //WithStatement(Result).Acessor := ProcessAcessor(WithStatement(Result).Acessor, V);

    Expect('do');

    CurrentModule.CurrentWith := WithStatement(Result);
    WithStatement(Result).Body := LoadStatement();
    CurrentModule.CurrentWith := WithStatement(Result).Parent;
  End Else
  Begin
    Temp := LastIndex;
    V := Nil;

    If Equals('Log') Then
      IntToString(2);

    If Equals('exit') Then
    Begin
      Result := Builder.CreateFunctionCall();
      FunctionCall(Result).Acessor := Builder.CreateAcessor();
      VariableAcessor(FunctionCall(Result).Acessor).Element := 'exit';
      VariableAcessor(FunctionCall(Result).Acessor).IsFunctionCall := True;
      Expect(';');
      Exit;
    End;

    If (Equals('inherited')) Then
    Begin
      Result := Builder.CreateFunctionCall();
      FunctionCall(Result).Acessor := Builder.CreateAcessor();
      VariableAcessor(FunctionCall(Result).Acessor).Element := 'inherited';
      VariableAcessor(FunctionCall(Result).Acessor).IsFunctionCall := True;
      FunctionCall(Result).Parent := CurrentFunction;
      Repeat
        S2 := GetToken(Src, LastIndex);
      Until (S2=';');
      Exit;
    End;

    If (GetcurrentLine()>=183) And (S='_Filters') Then
      StringToInt(S);

    If (V=Nil) Then
      V := CurrentFunction.GetVariable(S, Nil);
    If V=Nil Then
      V := CurrentModule.GetVariable(S, Nil);

    S2 := LowStr(LookAhead(Src, ';'));
    I := Pos('else', S2);
    If I>0 Then
    Begin
      S2 := Copy(S2, 1, Pred(I));
    End;

    {If (GetCurrentLine>=435) And (Pos('Utils',CurrentModule.Name)>0) Then
      IntToString(2);}

    If Pos(':=', S2)>0 Then
    Begin
      RevertToken(Src, LastIndex);
      Result := Builder.CreateAssignment();
      Assignment(Result).V := V;
      Assignment(Result).Acessor := LoadAcessor(Nil, Src, False);
      //Assignment(Result).Acessor := ProcessAcessor(Assignment(Result).Acessor, V);

      If (CurrentModule.Name = 'TERRA_Classes') Then
        IntToString(2);

      Expect(':=');
      S := '';
      Repeat
        S2 := GetToken(Src, LastIndex);
        If (Equals(';')) Or (Equals('else')) Then
        Begin
          Break;
        End;

        S := S + S2+' ';
      Until False;
      Assignment(Result).Result := LoadExpression(S);

      If (S2<>';') And (LowStr(S2)<>'else') Then
        Error('Unterminated statement.');
    End Else
    Begin
      //WriteLn(S2);
      RevertToken(Src, LastIndex);

      Result := Builder.CreateFunctionCall();
      FunctionCall(Result).Acessor := LoadAcessor(Nil, Src, False);
      S2 := GetToken(Src, LastIndex);
      If (S2<>';') And (LowStr(S2)<>'else') Then
        Error('Unterminated statement.');
        //FunctionCall(Result).Acessor := ProcessAcessor(FunctionCall(Result).Acessor, V);
    End;
  End;

  If Result = Nil Then
    Error('Unknown Statement type: '+S)
  Else
    Result.Parent := CurrentFunction;
End;

Function Module.LoadType: VariableType;
Var
  S, S2, S3:String;
  Min, Max:Integer;
  A,B:Expression;
  IsDynamic:Boolean;
  Temp:Integer;
Begin
  Result.Module := '';
  Result.Signature := Nil;
  Result.BaseOffset := 0;
  Result.Count := 1;
  S := GetToken(Src, LastIndex);
  S3 := GetToken(Src, Temp);
  If Equals('.') Then
  Begin
    Result.Module := S;
    S := GetToken(Src, LastIndex);
  End Else
    RevertToken(Src, Temp);

  CurrentToken := S;

  If Equals('procedure') Then
  Begin
    Result.Signature := LoadSignature(functionProcedure, False, True);
    Result.Primitive := 'procedure';
    RevertToken(Src, LastIndex);
  End Else
  If Equals('function') Then
  Begin
    Result.Signature := LoadSignature(functionFunction, False, True);
    Result.Primitive := 'function';
    RevertToken(Src, LastIndex);
  End Else
  If Equals('array') Then
  Begin
    S := GetToken(Src, LastIndex);
    If (S='[') Then
    Begin
      IsDynamic := False;
      S3 := '';
      Repeat
        S2 := GetToken(Src, LastIndex);
        If (S2='.') Then
          Break;
        S3 := S3 + S2 + ' ';
      Until False;
      A := LoadExpression(S3);
      Expect('.');

      S3 := '';
      Repeat
        S2 := GetToken(Src, LastIndex);
        If (S2=']') Then
          Break;
        S3 := S3 + S2 + ' ';
      Until False;
      B := LoadExpression(S3);

      A := A.Reduce();
      B := B.Reduce();

      If (A Is ConstantExpression) Then
      Begin
        Min := StringToInt(ConstantExpression(A).Value);
      End Else
        Error('Invalid expression!');

      If (B Is ConstantExpression) Then
      Begin
        Max := StringToInt(ConstantExpression(B).Value);
      End Else
        Error('Invalid expression!');
      Expect('of');
    End Else
    If (Equals('of')) Then
      IsDynamic := True
    Else
      Error('Array declaration error!');

    S := GetToken(Src, LastIndex);
    CurrentModule.AddIdentifier(S);
    Result.Primitive := LowStr(S);
    Result.BaseOffset := Min;
    Result.IsDynamicArray := IsDynamic;
    Result.Count := Succ(Max-Min);
  End Else
  Begin
    Self.AddIdentifier(S);
    Result.Primitive := LowStr(S);
    Result.BaseOffset := 0;
    Result.Count := 1;
  End;
End;

Procedure Module.LoadUsesClause(Section:ModuleSection);
Var
  S:String;
  Temp:Module;
Begin
  Repeat
    S := GetToken(Src, LastIndex);
    If S=';' Then
      Exit;

    If (S=',') Then
      Continue;

    Inc(Section.UnitCount);
    SetLength(Section.Units, Section.UnitCount);
    Section.Units[Pred(Section.UnitCount)] := S;
  Until False;
End;

Function Module.IsNewSection():Boolean;
Begin
  Result := (Equals('const')) Or (Equals('type')) Or (Equals('begin')) Or (Equals('procedure')) Or
    (Equals('function')) Or (Equals('end')) Or (Equals('var')) Or (Equals('constructor')) Or (Equals('destructor'))
    Or (Equals('implementation')) Or (Equals('interface')) Or (Equals('class')) Or (Equals('property'))
    Or (Equals('public')) Or (Equals('private')) Or (Equals('protected'));
End;

Procedure Module.LoadVariables(Var Variables:VariableBlock);
Var
  S, S2,S3, InitValue, VarList:String;
  VarType:VariableType;
  IsPointer:Boolean;
  Count:Integer;
  Exp:Expression;
Begin
  Repeat
	  VarList := '';
	  Repeat
		S := GetToken(Src, LastIndex);

		If Equals(':') Then
		  Break;

		If Equals(',') Then
		  Continue;

		VarList := VarList + S+',';
	  Until False;

    GetToken(Src, LastIndex);
    If Equals('^') Then
	  Begin
      IsPointer := True;
    End Else
    Begin
  		RevertToken(Src, LastIndex);
      IsPointer := False;
    End;

    VarType := LoadType();

	  GetToken(Src, LastIndex);
	  If Equals('=') Then
	  Begin
  		InitValue := GetToken(Src, LastIndex);
	  End Else
		RevertToken(Src, LastIndex);

	  S2 := GetToken(Src, LastIndex);
    If (S2='[') Then
    Begin
      S2 := '';
      Repeat
        S3 := GetToken(Src, LastIndex);
        If S3=']' Then
          Break;
        S2 := S2 + S3;
      Until False;

      If (VarType.Primitive='string') Then
      Begin
        Exp := LoadExpression(S2);
        If (Exp Is ConstantExpression) Then
        Begin
          Count := StringToInt(ConstantExpression(Exp).Value);
        End Else
          Error('Invalid expression!');

        VarType.BaseOffset := 1;
        VarType.Count := Count;
      End Else
        Error('Invalid array type, expected string.');

      Expect(';');
    End Else
    Begin
      RevertToken(Src, LastIndex);
      If (VarType.Primitive<>'function') And (VarType.Primitive<>'procedure') Then
        Expect(';');
    End;

    VarType.IsPointer := IsPointer;

	  While (VarList<>'') Do
	  Begin
  		S := GetNextWord(VarList, ',');
      If (S='list') Or (S='List') Then
        IntToString(2);

  		Inc(Variables.VariableCount);
  		SetLength(Variables.Variables, Variables.VariableCount);
      CurrentModule.AddIdentifier(S);
  		Variables.Variables[Pred(Variables.VariableCount)] := Builder.CreateVariable();
      Variables.Variables[Pred(Variables.VariableCount)].Parent := CurrentFunction;
  		Variables.Variables[Pred(Variables.VariableCount)].Name := LowStr(S);
	  	Variables.Variables[Pred(Variables.VariableCount)].VarType := VarType;
  		Variables.Variables[Pred(Variables.VariableCount)].InitialValue := LoadExpression(InitValue);
      Variables.Variables[Pred(Variables.VariableCount)].Owner := Nil;
      ExpandVariable(Variables.Variables[Pred(Variables.VariableCount)], 0);
	  End;
    InitValue := '';

    S := GetToken(Src, LastIndex);
    RevertToken(Src, LastIndex);
    If IsNewSection() Then
      Break;
  Until False;
End;

Procedure ReduceAndReplace(Var Exp:Expression); Overload;
Var
  Temp:Expression;
Begin
  If (Exp = Nil) Then
    Exit;

  Temp := Exp;
  Exp := Exp.Reduce();
  If Temp<>Exp Then
    Temp.Destroy;
End;

Procedure ReduceAndReplace(Var St:Statement); Overload;
Var
  Temp:Statement;
Begin
  If (St = Nil) Then
    Exit;

  Temp := St;
  St := St.Reduce();
  If Temp<>St Then
    Temp.Destroy;
End;

{ StatementBlock }
Procedure StatementBlock.AddStatement(St:Statement);
Begin
  Inc(Self.StatementCount);
  SetLength(Self.Statements, Self.StatementCount);
  Self.Statements[Pred(Self.StatementCount)] := St;
End;

Function StatementBlock.Reduce:Statement;
Var
  I:Integer;
Begin
  For I:=0 To Pred(Self.StatementCount) Do
    ReduceAndReplace(Self.Statements[I]);

  Result := Self;
End;

{ ModuleFunction }
Function ModuleFunction.GetVariable(S: String; Ac:VariableAcessor):Variable;
Var
  I:Integer;
  C:ClassSignature;
  A,B:String;
Begin
  If (Self = Nil) Then
  Begin
    Result := CurrentModule.CurrentSection.GetVariable(S, Ac);
    Exit;
  End;

  S := LowStr(S);
  For I:=0 To Pred(Self.Variables.VariableCount) Do
  Begin
    A := Self.Variables.Variables[I].GetPath();
    If (Ac=Nil) Then
      B := A
    Else
      B := Ac.GetPathBack() + Ac.GetPathFront();

    If (Self.Variables.Variables[I].Name = S) And (A=B) Then
    Begin
      Result := Self.Variables.Variables[I];
      Exit;
    End Else
    Begin
      Result := Self.Variables.Variables[I].GetVariable(S, Ac);
      If Assigned(Result) Then
        Exit;
    End;
  End;

  If (Signature=Nil) Then
  Begin
    Result := Nil;
    Exit;
  End;

  For I:=0 To Pred(Signature.ArgumentCount) Do
  If (Signature.Arguments[I].Name = S) Then
  Begin
    Result := Signature.Arguments[I];
    Exit;
  End Else
  Begin
    Result := Signature.Arguments[I].GetVariable(S, Ac);
    If Assigned(Result) Then
      Exit;
  End;

  If Signature.Owner<>'' Then
  Begin
    C := CurrentModule.FindClass(Signature.Owner, True);
    Result := C.GetVariable(S, Nil);
    If Assigned(Result) Then
      Exit;
  End;

  If Assigned(Parent) Then
  Begin
    Result := Parent.GetVariable(S, Ac);
    Exit;
  End;

  Result := Nil;
End;

Function Module.LoadAcessor(Parent:VariableAcessor; Src:Stream; ErrorOnFail:Boolean):VariableAcessor;
Var
  S, S2,VarName:String;
  Exp:Expression;
  Count, I, J, Temp:Integer;
  InStr:Boolean;
  C:ClassSignature;
Begin
  VarName := GetToken(Src, LastIndex);
  //WriteLn('Acessor: '+VarName);

  CurrentModule.AddIdentifier(VarName);
  Result := Builder.CreateAcessor();
  Result.Element := VarName;
  Result.Acessor := Nil;
  Result.Parent := Parent;
  Result.Dereferenced := False;
  If Equals('instance') Then
    Result.IsFunctionCall := True;

  {Result.V := Nil;
  If Assigned(CurrentWith) Then
    Result.V := CurrentWith.GetVariable(VarName, Nil);

  If (Result.V=Nil) And (Assigned(CurrentFunction)) Then
    Result.V := CurrentFunction.GetVariable(VarName, Result);

  If (Result.V=Nil) Then
    Result.V := Self.GetVariable(VarName, Result);}

  If Src.EOF Then
    Exit;

  S := GetToken(Src, LastIndex);
  If Equals('[') Then
  Begin
    S := '';
    Repeat
      S2 := GetToken(Src, LastIndex);
      If Equals(']') Then
        Break;

      S := S + S2+ ' ';
    Until False;

    Exp := LoadExpression(S);
    Result.Index := Exp;
    {If (Result.V.VarType.BaseOffset>0) Then
    Begin
      Result.Index := Builder.CreateBinaryOpExpression();
      BinaryExpression(Result.Index).Op := '-';
      BinaryExpression(Result.Index).A := Exp;
      BinaryExpression(Result.Index).B := Builder.CreateConstantExpression();
      ConstantExpression(BinaryExpression(Result.Index).B).Value := IntToString(Result.V.VarType.BaseOffset);
    End;}
  End Else
    RevertToken(Src, LastIndex);

  If Src.EOF Then
    Exit;

  S := GetToken(Src, LastIndex);
  If (Equals('(')) Then
  Begin
    Result.IsFunctionCall := True;
    Count := 0;
    S := '';
    Repeat
      S2 := GetToken(Src, LastIndex);
      If (Equals(')')) Then
      Begin
        If (Count=0) Then
        Begin
          Break;
        End Else
          Dec(Count);
      End Else
      If (Equals('(')) Then
        Inc(Count);

      S := S + S2+ ' ';
    Until False;

    Count := 0;
    While S<>'' Do
    Begin
      InStr := False;
      I:=1;
      //WriteLn(s);
      While (I<=Length(S)) Do
      If (I>=Length(S)) Then
      Begin
        S2 := S;
        S := '';
        Break;
      End Else
      If (S[I]=',') Then
      Begin
        If (Not InStr) And (Count=0) Then
        Begin
          S2 := Copy(S, 1, Pred(I));
          S := Copy(S, Succ(I), MaxInt);
          Break;
        End Else
          Inc(I);
      End Else
      If (S[I]='''') Then
      Begin
        InStr := Not InStr;
        Inc(I);
      End Else
      If (S[I]='(') Then
      Begin
        If (Not InStr) Then
          Inc(Count);
        Inc(I);
      End Else
      If (S[I]=')') Then
      Begin
        If (Not InStr) Then
          Dec(Count);
        Inc(I);
      End Else
        Inc(I);

      Inc(Result.ArgumentCount);
      SetLength(Result.Arguments, Result.ArgumentCount);
//      CurrentModule.AddIdentifier(S2);
      Result.Arguments[Pred(Result.ArgumentCount)] := LoadExpression(S2);
    End;
  End Else
  Begin
    StringToInt(S);
    RevertToken(Src, LastIndex);
  End;

  If Src.EOF Then
    Exit;

  S := GetToken(Src, LastIndex);
  If Equals('.') Then
  Begin
    Result.Acessor := LoadAcessor(Result, Src, ErrorOnFail);
  End Else
  Begin
    StringToInt(S);
    RevertToken(Src, LastIndex);
  End;

  {If (Result.V=Nil) And (ErrorOnFail) Then
    Error('Could not find variable: '+VarName);}

  S := GetToken(Src, LastIndex);
  If Equals('^') Then
  Begin
    Result.Dereferenced := True;
  End Else
  Begin
    RevertToken(Src, LastIndex);
  End;
End;

Function ModuleFunction.IsConstant(S: String): Boolean;
Var
  I:Integer;
Begin
  For I:=0 To Pred(Self.Constants.ConstantCount) Do
  If LowStr(Self.Constants.Constants[I].Name) = LowStr(S) Then
  Begin
    Result := True;
    Exit;
  End;
  Result := False;
End;

Function ModuleFunction.IsType(S: String): Boolean;
Var
  I:Integer;
Begin
  S := LowStr(S);
  If (S='integer') Or (S='cardinal') Or (S='byte') Or (S='char') Or (S='boolean')
  Or (S='shortint') Or (S='word') Or (S='smallint') Or (S='string') Then
  Begin
    Result := True;
    Exit;
  End;

  For I:=0 To Pred(Self.Types.TypeCount) Do
  If LowStr(Self.Types.Types[I].Name) = S Then
  Begin
    Result := True;
    Exit;
  End;
  Result := False;
End;

Function ModuleFunction.IsFunction(S: String): Boolean;
Var
  I:Integer;
Begin
  Result := LowStr(Signature.Name) = LowStr(S);
End;

Procedure ModuleFunction.PostProcess;
Begin
  If Assigned(Self.Statements) Then
    ReduceAndReplace(Self.Statements);
End;

Function ModuleFunction.GetType(S: String): VarType;
Var
  I:Integer;
Begin
  S := LowStr(S);
  For I:=0 To Pred(Types.TypeCount) Do
  If (LowStr(Types.Types[I].Name)=S) Then
  Begin
    Result := Types.Types[I];
    If Assigned(Result) Then
      Exit;
  End;

  Result := Nil;
End;

Function ModuleFunction.EvaluateConstant(S: String): String;
Var
  I:Integer;
Begin
  S := LowStr(S);
  For I:=0 To Pred(Self.Constants.ConstantCount) Do
  If (LowStr(Self.Constants.Constants[I].Name) = S) Then
  Begin
    Self.Constants.Constants[I].Value.Reduce();
    If (Self.Constants.Constants[I].Value Is ConstantExpression) Then
    Begin
      Result := ConstantExpression(Self.Constants.Constants[I].Value).Value;
      Exit;
    End;
  End;

  Result := '';
End;

Function ModuleFunction.GetConstant(S: String): Constant;
Var
  I:Integer;
Begin
  S := LowStr(S);
  For I:=0 To Pred(Self.Constants.ConstantCount) Do
  If (LowStr(Self.Constants.Constants[I].Name) = S) Then
  Begin
    Result := Self.Constants.Constants[I];
    Exit;
  End;

  Result := Nil;
End;

Function ModuleFunction.GetProperty(S: String):ClassProperty;
Var
  I:Integer;
  C:ClassSignature;
Begin
  Result := Nil;
  If (Signature = Nil) Or (Self.Signature.Owner='') Then
    Exit;

  C := CurrentModule.FindClass(Signature.Owner, True);
  If Assigned(C) Then
    Result := C.GetProperty(S);
End;

{ Statement }
Function Statement.Nest: Integer;
Begin
  If Assigned(Parent) Then
    Result := Parent.Nest + 1
  Else
    Result := 0;
End;

{ Constant }
Function Constant.Nest: Integer;
Begin
  If Assigned(Parent) Then
    Result := Parent.Nest + 1
  Else
    Result := 0;
End;

{ Variable }
Function Variable.GetVariable(S: String; Ac:VariableAcessor): Variable;
Var
  I:Integer;
  A,B:String;
Begin
  S := LowStr(S);
  A := Self.GetPath();
  If (Ac=Nil) Then
    B := A
  Else
    B := Ac.GetPathBack() + Ac.GetPathFront();

  If (LowStr(Self.Name) = S) And (A=B) Then
  Begin
    Result := Self;
    Exit;
  End;

  For I:=0 To Pred(ChildCount) Do
  Begin
    Result := Childs[I].GetVariable(S, Ac);
    If Assigned(Result) Then
      Exit;
  End;

  Result := Nil;
End;

Function Variable.Nest: Integer;
Begin
  If Assigned(Parent) Then
    Result := Parent.Nest + 1
  Else
    Result := 0;
End;

{ VarType }
Function VarType.Nest: Integer;
Begin
  If Assigned(Parent) Then
    Result := Parent.Nest + 1
  Else
    Result := 0;
End;

{ ModuleSection }
function ModuleSection.EvaluateConstant(S: String): String;
Var
  I:Integer;
Begin
  S := LowStr(S);
  For I:=0 To Pred(Self.Constants.ConstantCount) Do
  If (LowStr(Self.Constants.Constants[I].Name) = S) Then
  Begin
    Self.Constants.Constants[I].Value.Reduce();
    If (Self.Constants.Constants[I].Value Is ConstantExpression) Then
    Begin
      Result := ConstantExpression(Self.Constants.Constants[I].Value).Value;
      Exit;
    End;
  End;

  For I:=0 To Pred(FunctionCount) Do
  Begin
    Result := Functions[I].EvaluateConstant(S);
    If (Result<>'') Then
      Exit;
  End;

  Result := '';
End;

Function ModuleSection.GetConstant(S: String): Constant;
Var
  I:Integer;
Begin
  S := LowStr(S);
  For I:=0 To Pred(Self.Constants.ConstantCount) Do
  If (LowStr(Self.Constants.Constants[I].Name) = S) Then
  Begin
    Result := Self.Constants.Constants[I];
    Exit;
  End;

  Result := Nil;
End;

Function ModuleSection.GetType(S: String): VarType;
Var
  I:Integer;
Begin
  S := LowStr(S);
  For I:=0 To Pred(Types.TypeCount) Do
  Begin
    //WriteLn(Types.Types[I].Name);
    If (LowStr(Types.Types[I].Name)=S) Then
    Begin
      Result := Types.Types[I];
      If Assigned(Result) Then
        Exit;
    End;
  End;

  For I:=0 To Pred(FunctionCount) Do
  Begin
    Result := Functions[I].GetType(S);
    If Assigned(Result) Then
      Exit;
  End;

  Result := Nil;
End;

Function ModuleSection.GetVariable(S: String; Ac:VariableAcessor): Variable;
Var
  I:Integer;
  A,B:String;
Begin
  S := LowStr(S);
  For I:=0 To Pred(Self.Variables.VariableCount) Do
  Begin
    A := Self.Variables.Variables[I].GetPath();
    If (Ac=Nil) Then
      B := A
    Else
      B := Ac.GetPathBack() + Ac.GetPathFront();
    If (Self.Variables.Variables[I].Name = S) And (A=B) Then
    Begin
      Result := Self.Variables.Variables[I];
      Exit;
    End Else
    Begin
      Result := Self.Variables.Variables[I].GetVariable(S, Ac);
      If Assigned(Result) Then
        Exit;
    End;
  End;

  Result := Nil;
End;

Procedure Module.LoadConstants(var Constants: ConstantBlock);
Var
  S, S2, S3:String;
  VarType:VariableType;
  Count:Integer;
  A,B:Expression;
  Min,Max:Integer;
Begin
  Repeat
    VarType.Primitive := '';
		S := GetToken(Src, LastIndex);
//    WriteLn('const: '+S);

    If S='FullscreenQuad' Then
      IntToString(2);

    S2 := GetToken(Src, LastIndex);
    If (S2=':') Then
    Begin
      VarType.BaseOffset := 0;
      VarType.Count := 1;
      VarType.IsPointer := False;
      VarType.IsDynamicArray := False;
      VarType.Primitive := GetToken(Src, LastIndex);
      S3 := GetToken(Src, LastIndex);
      If (S3='[') Then
      Begin
        S3 := '';
        Repeat
          S2 := GetToken(Src, LastIndex);
          If (S2='.') Then
            Break;
          S3 := S3 + S2 + ' ';
        Until False;
        A := LoadExpression(S3);
        Expect('.');

        S3 := '';
        Repeat
          S2 := GetToken(Src, LastIndex);
          If (S2=']') Then
            Break;
          S3 := S3 + S2 + ' ';
        Until False;
        B := LoadExpression(S3);

        A := A.Reduce();
        B := B.Reduce();

        If (A Is ConstantExpression) Then
        Begin
          Min := StringToInt(ConstantExpression(A).Value);
        End Else
          Error('Invalid expression!');

        If (B Is ConstantExpression) Then
        Begin
          Max := StringToInt(ConstantExpression(B).Value);
        End Else
          Error('Invalid expression!');

        VarType.BaseOffset := Min;
        VarType.Count := Succ(Max-Min);

        Expect('of');
        VarType.Primitive := GetToken(Src, LastIndex);
        Expect('=');
      End Else
      If (S3<>'=') Then
        Error('Expected =, got '+S3);

      S2 := ':';
    End Else
    If (S2<>'=') Then
      Error('Expected =, got '+S2);

    Inc(Constants.ConstantCount);
    SetLength(Constants.Constants, Constants.ConstantCount);
    Constants.Constants[Pred(Constants.ConstantCount)] := Builder.CreateConstant();
    Constants.Constants[Pred(Constants.ConstantCount)].Name := S;
    Constants.Constants[Pred(Constants.ConstantCount)].Parent := CurrentFunction;
    Constants.Constants[Pred(Constants.ConstantCount)].VarType := VarType;

    If (S2=':') Then
    Begin
      Count := 0;
      S := '';

      S2 := GetToken(Src, LastIndex);
      If Equals('(') Then
      Begin
        RevertToken(Src, LastIndex);
        Repeat
	  	    S2 := GetToken(Src, LastIndex);
          If (Equals(')')) Then
          Begin
            Dec(Count);
            If (Count=0) Then
            Begin
              S := S + S2 + ' ';
              Break;
            End;
          End Else
          If (Equals('(')) Then
          Begin
            Inc(Count);
          End;
          S := S + S2 + ' ';
        Until False;
        Constants.Constants[Pred(Constants.ConstantCount)].Value := Builder.CreateConstantExpression();
        ConstantExpression(Constants.Constants[Pred(Constants.ConstantCount)].Value).Value := S;
      End Else
      Begin
        Constants.Constants[Pred(Constants.ConstantCount)].Value := Builder.CreateConstantExpression();
        ConstantExpression(Constants.Constants[Pred(Constants.ConstantCount)].Value).Value := S2;
      End;
      Expect(';');
    End Else
    Begin
      S := '';
      Repeat
		    S2 := GetToken(Src, LastIndex);
        If (Equals(';')) Then
          Break;
        S := S + S2 + ' ';
      Until False;
    End;

    If Constants.Constants[Pred(Constants.ConstantCount)].Value = Nil Then
    Begin
      If (Pos(':',S)>0) Then
      Begin
        Constants.Constants[Pred(Constants.ConstantCount)].Value := Nil;
        Constants.Constants[Pred(Constants.ConstantCount)].Init := S;
      End Else
      Begin
        Constants.Constants[Pred(Constants.ConstantCount)].Value := LoadExpression(S);
        Constants.Constants[Pred(Constants.ConstantCount)].Init := '';
      End;
    End;
    
    S := GetToken(Src, LastIndex);
    RevertToken(Src, LastIndex);
    If IsNewSection() Then
      Break;
  Until False;
End;

Procedure Module.LoadTypes(Var Types: TypeBlock);
Var
  S, S2,S3:String;
  ML:^MethodList;
  C:ClassSignature;
  IsStatic:Boolean;
  A,B:Expression;
  N:Integer;
Begin
  Repeat
		S := GetToken(Src, LastIndex);
    WriteLn('Type: '+S);

  If Equals('texturerendertarget') Then
    IntToString(2);

    Expect('=');
		S2 := GetToken(Src, LastIndex);

    If (Equals('packed')) Then
  		S2 := GetToken(Src, LastIndex);

    If Equals('(') Then
    Begin
      Inc(Types.TypeCount);
      SetLength(Types.Types, Types.TypeCount);
      CurrentModule.AddIdentifier(S);
      Types.Types[Pred(Types.TypeCount)] := Builder.CreateEnum();
      Types.Types[Pred(Types.TypeCount)].Name := LowStr(S);
      Types.Types[Pred(Types.TypeCount)].Parent := CurrentFunction;
      Types.Types[Pred(Types.TypeCount)].TypeName := 'procedure';
      EnumType(Types.Types[Pred(Types.TypeCount)]).Count := 0;
      Repeat
        S2 := GetToken(Src, LastIndex);
        If Equals(')') Then
          Break;

        Inc(EnumType(Types.Types[Pred(Types.TypeCount)]).Count);
        N := EnumType(Types.Types[Pred(Types.TypeCount)]).Count;
        SetLength(EnumType(Types.Types[Pred(Types.TypeCount)]).Values, N);
        EnumType(Types.Types[Pred(Types.TypeCount)]).Values[Pred(N)].Name := S2;
        S2 := GetToken(Src, LastIndex);
        If Equals('=') Then
        Begin
          S2 := GetToken(Src, LastIndex);
          EnumType(Types.Types[Pred(Types.TypeCount)]).Values[Pred(N)].Value := StringToInt(S2);
        End Else
        Begin
          RevertToken(Src, LastIndex);
          EnumType(Types.Types[Pred(Types.TypeCount)]).Values[Pred(N)].Value:= Pred(N);
        End;

      Until False;
      Expect(';');
    End Else
    If (Equals('procedure')) Then
    Begin
      Inc(Types.TypeCount);
      SetLength(Types.Types, Types.TypeCount);
      CurrentModule.AddIdentifier(S);
      Types.Types[Pred(Types.TypeCount)] := Builder.CreateFunctionDefinition();
      Types.Types[Pred(Types.TypeCount)].Name := LowStr(S);
      Types.Types[Pred(Types.TypeCount)].Parent := CurrentFunction;
      Types.Types[Pred(Types.TypeCount)].TypeName := 'procedure';
      FunctionType(Types.Types[Pred(Types.TypeCount)]).Signature := LoadSignature(functionProcedure, False, True);
    End Else
    If (Equals('function')) Then
    Begin
      Inc(Types.TypeCount);
      SetLength(Types.Types, Types.TypeCount);
      CurrentModule.AddIdentifier(S);
      Types.Types[Pred(Types.TypeCount)] := Builder.CreateFunctionDefinition();
      Types.Types[Pred(Types.TypeCount)].Name := LowStr(S);
      Types.Types[Pred(Types.TypeCount)].Parent := CurrentFunction;
      Types.Types[Pred(Types.TypeCount)].TypeName := 'function';
      FunctionType(Types.Types[Pred(Types.TypeCount)]).Signature := LoadSignature(functionFunction, False, True);
    End Else
    If (Equals('record')) Then
    Begin
      Inc(Types.TypeCount);
      SetLength(Types.Types, Types.TypeCount);
      CurrentModule.AddIdentifier(S);
      Types.Types[Pred(Types.TypeCount)] := Builder.CreateRecord();
      Types.Types[Pred(Types.TypeCount)].Name := LowStr(S);
      Types.Types[Pred(Types.TypeCount)].Parent := CurrentFunction;
      Types.Types[Pred(Types.TypeCount)].TypeName := 'record';
      LoadVariables(RecordType(Types.Types[Pred(Types.TypeCount)]).Variables);
      Expect('end');
      Expect(';');
    End Else
    If (Equals('class')) Or (Equals('object')) Then
    Begin
      If (Pos('TERRA_UI',CurrentModule.Name)>0) Then
        IntToString(2);

		  GetToken(Src, LastIndex);
      If (Equals(';')) Then
      Begin
        IntToString(2);
      End Else
      If (Equals('of')) Then
      Begin
        S2 := GetToken(Src, LastIndex);
        Expect(';');
      End Else
      Begin
        Inc(Types.TypeCount);
        SetLength(Types.Types, Types.TypeCount);
        CurrentModule.AddIdentifier(S);
        RevertToken(Src, LastIndex);
        C := Builder.CreateClass();
        Types.Types[Pred(Types.TypeCount)] := C;
        Types.Types[Pred(Types.TypeCount)].Name := LowStr(S);
        Types.Types[Pred(Types.TypeCount)].Parent := CurrentFunction;
        Types.Types[Pred(Types.TypeCount)].TypeName := 'class';
        CurrentVisiblity := visiblityPublic;
        C.Ancestor := Nil;
        S := GetToken(Src, LastIndex);
        If (Equals('(')) Then
        Begin
          S := GetToken(Src, LastIndex);
          C.Ancestor := FindClass(S, True);
          Expect(')');
        End Else
          RevertToken(Src, LastIndex);

        If (C.Ancestor = Nil) Then
        Begin
          Inc(C.ProtectedVariables.VariableCount);
          SetLength(C.ProtectedVariables.Variables, C.ProtectedVariables.VariableCount);
          C.ProtectedVariables.Variables[Pred(C.ProtectedVariables.VariableCount)] := Builder.CreateVariable();
          C.ProtectedVariables.Variables[Pred(C.ProtectedVariables.VariableCount)].Name := 'self';
          C.ProtectedVariables.Variables[Pred(C.ProtectedVariables.VariableCount)].VarType.Primitive := C.Name;
          C.ProtectedVariables.Variables[Pred(C.ProtectedVariables.VariableCount)].InitialValue := Nil;
          C.ProtectedVariables.Variables[Pred(C.ProtectedVariables.VariableCount)].Owner := Nil;
          C.ProtectedVariables.Variables[Pred(C.ProtectedVariables.VariableCount)].Parent := CurrentFunction;
          ExpandVariable(C.ProtectedVariables.Variables[Pred(C.ProtectedVariables.VariableCount)], 0);

          Inc(C.ProtectedVariables.VariableCount);
          SetLength(C.ProtectedVariables.Variables, C.ProtectedVariables.VariableCount);
          C.ProtectedVariables.Variables[Pred(C.ProtectedVariables.VariableCount)] := Builder.CreateVariable();
          C.ProtectedVariables.Variables[Pred(C.ProtectedVariables.VariableCount)].Name := 'classname';
          C.ProtectedVariables.Variables[Pred(C.ProtectedVariables.VariableCount)].VarType.Primitive := 'string';
          C.ProtectedVariables.Variables[Pred(C.ProtectedVariables.VariableCount)].InitialValue := Nil;
          C.ProtectedVariables.Variables[Pred(C.ProtectedVariables.VariableCount)].Owner := Nil;
          C.ProtectedVariables.Variables[Pred(C.ProtectedVariables.VariableCount)].Parent := CurrentFunction;
          ExpandVariable(C.ProtectedVariables.Variables[Pred(C.ProtectedVariables.VariableCount)], 0);
        End;

        Repeat
          S := GetToken(Src, LastIndex);
          //WriteLn('token: '+S);

          Case CurrentVisiblity Of
          visiblityPublic:
            ML := @C.PublicMethods;
          visiblityPrivate:
            ML := @C.PrivateMethods;
          visiblityProtected:
            ML := @C.ProtectedMethods;
          Else
            ML := Nil;
          End;

          If (Equals('end')) Then
            Break;

          If (Equals('class')) Then
          Begin
            IsStatic := True;
            S := GetToken(Src, LastIndex);
          End Else
            IsStatic := False;

          If (Equals('public')) Then
          Begin
            CurrentVisiblity := visiblityPublic;
          End Else
          If (Equals('private')) Then
          Begin
            CurrentVisiblity := visiblityPrivate;
          End Else
          If (Equals('protected')) Then
          Begin
            CurrentVisiblity := visiblityProtected;
          End Else
          If (Equals('property')) Then
          Begin
            Inc(C.PropertyCount);
            SetLength(C.Properties, C.PropertyCount);
            C.Properties[Pred(C.PropertyCount)] := LoadProperty();
            C.Properties[Pred(C.PropertyCount)].Owner := C.Name;
          End Else
          If (Equals('procedure')) Then
          Begin
            Inc(ML.MethodCount);
            SetLength(ML.Methods, ML.MethodCount);
            ML.Methods[Pred(ML.MethodCount)] := LoadSignature(functionProcedure, IsStatic, False);
            ML.Methods[Pred(ML.MethodCount)].Owner := C.Name;
          End Else
          If (Equals('function')) Then
          Begin
            Inc(ML.MethodCount);
            SetLength(ML.Methods, ML.MethodCount);
            ML.Methods[Pred(ML.MethodCount)] := LoadSignature(functionFunction, IsStatic, False);
            ML.Methods[Pred(ML.MethodCount)].Owner := C.Name;
          End Else
          If (Equals('constructor')) Then
          Begin
            Inc(ML.MethodCount);
            SetLength(ML.Methods, ML.MethodCount);
            ML.Methods[Pred(ML.MethodCount)] := LoadSignature(functionConstructor, False, False);
            ML.Methods[Pred(ML.MethodCount)].Owner := C.Name;
          End Else
          If (Equals('destructor')) Then
          Begin
            Inc(ML.MethodCount);
            SetLength(ML.Methods, ML.MethodCount);
            ML.Methods[Pred(ML.MethodCount)] := LoadSignature(functionDestructor, False, False);
            ML.Methods[Pred(ML.MethodCount)].Owner := C.Name;
          End Else
          Begin
            RevertToken(Src, LastIndex);

            Case CurrentVisiblity Of
            visiblityPublic:
              LoadVariables(C.PublicVariables);
            visiblityPrivate:
              LoadVariables(C.PrivateVariables);
            visiblityProtected:
              LoadVariables(C.ProtectedVariables);
            End;
          End;
        Until False;
        Expect(';');

        If (C.Ancestor=Nil) And (C.GetMethod('destroy')=Nil) Then
        Begin
          Inc(C.PublicMethods.MethodCount);
          SetLength(C.PublicMethods.Methods, C.PublicMethods.MethodCount);
          C.PublicMethods.Methods[Pred(C.PublicMethods.MethodCount)] := Builder.CreateFunctionSignature();
          C.PublicMethods.Methods[Pred(C.PublicMethods.MethodCount)].Name := 'destroy';
          C.PublicMethods.Methods[Pred(C.PublicMethods.MethodCount)].Genre := functionDestructor;
          C.PublicMethods.Methods[Pred(C.PublicMethods.MethodCount)].ArgumentCount := 0;
          C.PublicMethods.Methods[Pred(C.PublicMethods.MethodCount)].Owner := C.Name;

          Inc(Self.CurrentSection.FunctionCount);
          SetLength(Self.CurrentSection.Functions, Self.CurrentSection.FunctionCount);
          Self.CurrentSection.Functions[Pred(Self.CurrentSection.FunctionCount)] := Builder.CreateFunction();
          Self.CurrentSection.Functions[Pred(Self.CurrentSection.FunctionCount)].Signature:= C.PublicMethods.Methods[Pred(C.PublicMethods.MethodCount)];
          Self.CurrentSection.Functions[Pred(Self.CurrentSection.FunctionCount)].Section := Self.CurrentSection;
          Self.CurrentSection.Functions[Pred(Self.CurrentSection.FunctionCount)].Statements := Builder.CreateBlock();
        End;
      End;
    End Else
    Begin
      Inc(Types.TypeCount);
      SetLength(Types.Types, Types.TypeCount);   
      Types.Types[Pred(Types.TypeCount)] := Builder.CreateType();
      If S2='^' Then
      Begin
        Types.Types[Pred(Types.TypeCount)].IsPointer := True;
        S2 := GetToken(Src, LastIndex);
      End Else
      Begin
        Types.Types[Pred(Types.TypeCount)].IsPointer := False;
      End;

      If (LowStr(S2)='array') Then
      Begin
        Expect('[');
        S3 := '';
        Repeat
          S2 := GetToken(Src, LastIndex);
          If (S2='.') Then
            Break;
          S3 := S3 + S2 + ' ';
        Until False;
        A := LoadExpression(S3);
        Expect('.');

        S3 := '';
        Repeat
          S2 := GetToken(Src, LastIndex);
          If (S2=']') Then
            Break;
          S3 := S3 + S2 + ' ';
        Until False;
        B := LoadExpression(S3);

        Expect('of');
        S2 := GetToken(Src, LastIndex);
      End;

      CurrentModule.AddIdentifier(S);
      Types.Types[Pred(Types.TypeCount)].Name := LowStr(S);
      Types.Types[Pred(Types.TypeCount)].Parent := CurrentFunction;
      VarType(Types.Types[Pred(Types.TypeCount)]).TypeName := LowStr(S2);
      Expect(';');
    End;

    S := GetToken(Src, LastIndex);
    RevertToken(Src, LastIndex);

    If IsNewSection() Then
      Break;
  Until False;
End;

Function Module.LoadSignature(FunctionGenre:Integer; IsStatic:Boolean; Nameless:Boolean): FunctionSignature;
Var
  S, S2, S3:String;
  V:Variable;
  VarType:VariableType;
  IsVar,IsConst:Boolean;
  InitValue:String;
Begin
  Result := Builder.CreateFunctionSignature();
  If Not Nameless Then
  Begin
    S := GetToken(Src, LastIndex);
    CurrentModule.AddIdentifier(S);
  End;

  If LowStr(S)='oniap_cancel' Then
    IntToString(2);

  Result.Name := LowStr(S);
  Result.Genre := FunctionGenre;
  Result.IsStatic := IsStatic;
  GetToken(Src, LastIndex);
  If (Equals('.')) Then
  Begin
    Result.Owner := Result.Name;
    Result.Name := GetToken(Src, LastIndex);
  End Else
    RevertToken(Src, LastIndex);

  GetToken(Src, LastIndex);
  If Equals('(') Then
  Begin
    Repeat
      S := GetToken(Src, LastIndex);
      If Equals(')') Then
        Break;

      If Equals(';') Then
        Continue;

      IsVar := False;
      IsConst := False;
      If (Equals('var')) Then
      Begin
        S := GetToken(Src, LastIndex);
        IsVar := True;
      End Else
      If (Equals('const')) Then
      Begin
        S := GetToken(Src, LastIndex);
        IsConst := True;
      End;

      Repeat
        S2 := GetToken(Src, LastIndex);
        If Equals(':') Then
          Break;
        S := S+ S2+',';
      Until False;
      If Pos('Target',S)>0 Then
        IntToString(2);

      VarType := LoadType();
      GetToken(Src, LastIndex);
      If (Equals('=')) Then
      Begin
        InitValue := '';
        Repeat
          S3 := GetToken(Src, LastIndex);
          If (Equals(';')) Or (Equals(')')) Then
          Begin
            RevertToken(Src, LastIndex);
            Break;
          End Else
            InitValue := InitValue + S3 + ' ';
        Until False;
      End Else
      Begin
        InitValue := '';
        RevertToken(Src, LastIndex);
      End;

      While S<>'' Do
      Begin
        S2 := GetNextWord(S, ',');
        Inc(Result.ArgumentCount);
        SetLength(Result.Arguments, Result.ArgumentCount);
        CurrentModule.AddIdentifier(S2);
        Result.Arguments[Pred(Result.ArgumentCount)] := Builder.CreateArgument;
        Result.Arguments[Pred(Result.ArgumentCount)].Name := LowStr(S2);
        Result.Arguments[Pred(Result.ArgumentCount)].VarType := VarType;
        Result.Arguments[Pred(Result.ArgumentCount)].IsVar := IsVar;
        Result.Arguments[Pred(Result.ArgumentCount)].IsConst := IsConst;
        Result.Arguments[Pred(Result.ArgumentCount)].InitialValue := LoadExpression(InitValue);
        ExpandVariable(Result.Arguments[Pred(Result.ArgumentCount)], 0);
      End;
    Until False;

  End Else
    RevertToken(Src, LastIndex);

  If FunctionGenre = functionFunction Then
  Begin
    Expect(':');
    Result.ResultType := LoadType();

    If Assigned(CurrentFunction) Then
    Begin
      Inc(CurrentFunction.Variables.VariableCount);
      SetLength(CurrentFunction.Variables.Variables, CurrentFunction.Variables.VariableCount);
      V := Builder.CreateVariable;
      V.Parent := CurrentFunction;
      V.Name := 'result';
      V.Owner := Nil;
      V.VarType := Result.ResultType;
      ExpandVariable(V, 0);
      CurrentFunction.Variables.Variables[Pred(CurrentFunction.Variables.VariableCount)] := V;
    End;
  End Else
    Result.ResultType.Primitive := '';

  Expect(';');

  S := GetToken(Src, LastIndex);
  If (Equals('stdcall')) Or (Equals('cdecl')) Or (Equals('register')) Then
  Begin
    Result.CallConvention := S;
    Expect(';');
  End Else
  Begin
    Result.CallConvention := '';
    RevertToken(Src, LastIndex);
  End;

  S := GetToken(Src, LastIndex);
  If (Equals('external')) Then
  Begin
    Result.ExternalLocation := GetToken(Src, LastIndex);
    S := GetToken(Src, LastIndex);
    If (S='name') Then
    Begin
      Result.ExternalName := GetToken(Src, LastIndex);
    End Else
    Begin
      Result.ExternalName := Result.Name;
      RevertToken(Src, LastIndex);
    End;
    Expect(';');
  End Else
  Begin
    RevertToken(Src, LastIndex);
  End;

  S := GetToken(Src, LastIndex);
  If (Equals('overload')) Then
  Begin
    Result.IsOverloaded := True;
    Expect(';');
  End Else
  Begin
    Result.IsOverloaded := False;
    RevertToken(Src, LastIndex);
  End;
  
  S := GetToken(Src, LastIndex);
  If (Equals('reintroduce')) Then
  Begin
    Expect(';');
  End Else
  Begin
    RevertToken(Src, LastIndex);
  End;

  S := GetToken(Src, LastIndex);
  If (Equals('virtual')) Then
  Begin
    Result.IsVirtual := True;
    Expect(';');
  End Else
  Begin
    Result.IsVirtual := False;
    RevertToken(Src, LastIndex);
  End;

  S := GetToken(Src, LastIndex);
  If (Equals('abstract')) Then
  Begin
    Result.IsAbstract := True;
    Expect(';');
  End Else
  Begin
    Result.IsAbstract := False;
    RevertToken(Src, LastIndex);
  End;

  S := GetToken(Src, LastIndex);
  If (Equals('reintroduce')) Then
  Begin
    Expect(';');
  End Else
  Begin
    RevertToken(Src, LastIndex);
  End;

  S := GetToken(Src, LastIndex);
  If (Equals('override')) Then
  Begin
    Result.IsOverride := True;
    Expect(';');
  End Else
  Begin
    Result.IsOverride := False;
    RevertToken(Src, LastIndex);
  End;
End;

Function Module.FindClass(Name: String; ValidateError:Boolean):ClassSignature;
Var
  I:Integer;
Begin
  Result := FindClass(Self.InterfaceSection, Name);
  If Result = Nil Then
    Result := FindClass(Self.ImplementationSection, Name);
  If Assigned(Result) Then
    Exit;

  For I:=0 To Pred(Project.ModuleCount) Do
  If Assigned(Project.Modules[I]) Then
  Begin
    Result := FindClass(Project.Modules[I].InterfaceSection, Name);
    If Result = Nil Then
      Result := FindClass(Project.Modules[I].ImplementationSection, Name);
    If Assigned(Result) Then
      Exit;
  End;

  If (Result = Nil) And (ValidateError) Then
    Error('Class not found: '+Name);
End;

Function Module.FindClass(Section: ModuleSection; Name: String):ClassSignature;
Var
  I:Integer;
Begin
  Name := LowStr(Name);
  For I:=0 To Pred(Section.Types.TypeCount) Do
  If (Section.Types.Types[I].TypeName='class') And (Section.Types.Types[I].Name = Name) Then
  Begin
    Result := ClassSignature(Section.Types.Types[I]);
    Exit;
  End;

  Result := Nil;
End;

Function ModuleSection.IsConstant(S: String): Boolean;
Var
  I:Integer;
Begin
  For I:=0 To Pred(Self.FunctionCount) Do
  Begin
    Result := Self.Functions[I].IsConstant(S);
    If Result Then
      Exit;
  End;

  For I:=0 To Pred(Self.Constants.ConstantCount) Do
  If LowStr(Self.Constants.Constants[I].Name) = LowStr(S) Then
  Begin
    Result := True;
    Exit;
  End;

  Result := False;
End;

Function ModuleSection.IsFunction(S: String): Boolean;
Var
  I:Integer;
Begin
  For I:=0 To Pred(Self.FunctionCount) Do
  Begin
    Result := Self.Functions[I].IsFunction(S);
    If Result Then
      Exit;
  End;

  Result := False;
End;

Function ModuleSection.IsType(S: String): Boolean;
Var
  I:Integer;
Begin
  For I:=0 To Pred(Self.FunctionCount) Do
  Begin
    Result := Self.Functions[I].IsType(S);
    If Result Then
      Exit;
  End;

  Result := False;
End;

Procedure ModuleSection.PostProcess;
Var
  I:Integer;
Begin
  For I:=0 To Pred(Self.FunctionCount) Do
    Self.Functions[I].PostProcess();

  ReduceAndReplace(Body);
End;

{ ClassSignature }
Function ClassSignature.GetMethod(S: String): FunctionSignature;
Var
  I:Integer;
Begin
  S := LowStr(S);
  For I:=0 To Pred(PrivateMethods.MethodCount) Do
  If (LowStr(PrivateMethods.Methods[I].Name) = S) Then
  Begin
    Result := PrivateMethods.Methods[I];
    Exit;
  End;

  For I:=0 To Pred(ProtectedMethods.MethodCount) Do
  If (LowStr(ProtectedMethods.Methods[I].Name) = S) Then
  Begin
    Result := ProtectedMethods.Methods[I];
    Exit;
  End;

  For I:=0 To Pred(ProtectedMethods.MethodCount) Do
  If (LowStr(ProtectedMethods.Methods[I].Name) = S) Then
  Begin
    Result := ProtectedMethods.Methods[I];
    Exit;
  End;

  Result := Nil;
End;

Function ClassSignature.GetProperty(S: String): ClassProperty;
Var
  I:Integer;
Begin
  S := LowStr(S);
  For I:=0 To Pred(PropertyCount) Do
  If (LowStr(Properties[I].Name)=S) Then
  Begin
    Result := Properties[I];
    Exit;
  End;

  If Assigned(Self.Ancestor) Then
  Begin
    Result := Self.Ancestor.GetProperty(S);
    If Assigned(Result) Then
      Exit;
  End;

  Result := Nil;
End;

function ClassSignature.GetVariable(S: String; Ac:VariableAcessor): Variable;
Var
  I:Integer;
  A,B:String;
Begin
  For I:=0 To Pred(PrivateVariables.VariableCount) Do
  Begin
    A := PrivateVariables.Variables[I].GetPath();
    If (Ac=Nil) Then
      B := A
    Else
      B := Ac.GetPathBack() + Ac.GetPathFront();

    If (PrivateVariables.Variables[I].Name = S) And (A=B) Then
    Begin
      Result := PrivateVariables.Variables[I];
      Exit;
    End Else
    Begin
      Result := PrivateVariables.Variables[I].GetVariable(S, Ac);
      If Assigned(Result) Then
        Exit;
    End;
  End;

  For I:=0 To Pred(ProtectedVariables.VariableCount) Do
  Begin
    A := ProtectedVariables.Variables[I].GetPath();
    If (Ac=Nil) Then
      B := A
    Else
      B := Ac.GetPathBack() + Ac.GetPathFront();

    If (ProtectedVariables.Variables[I].Name = S) And (A=B) Then
    Begin
      Result := ProtectedVariables.Variables[I];
      Exit;
    End Else
    Begin
      Result := ProtectedVariables.Variables[I].GetVariable(S, Ac);
      If Assigned(Result) Then
        Exit;
    End;
  End;

  For I:=0 To Pred(PublicVariables.VariableCount) Do
  Begin
    A := PublicVariables.Variables[I].GetPath();
    If (Ac=Nil) Then
      B := A
    Else
      B := Ac.GetPathBack() + Ac.GetPathFront();

    If (PublicVariables.Variables[I].Name = S) And (A=B) Then
    Begin
      Result := PublicVariables.Variables[I];
      Exit;
    End Else
    Begin
      Result := PublicVariables.Variables[I].GetVariable(S, Ac);
      If Assigned(Result) Then
        Exit;
    End;
  End;

  If Assigned(Self.Ancestor) Then
  Begin
    Result := Self.Ancestor.GetVariable(S, Ac);
    If Assigned(Result) Then
      Exit;
  End;

  Result := Nil;
End;

Procedure Module.PostProcess;
Begin
  If Assigned(InterfaceSection) Then
    InterfaceSection.PostProcess();
  If Assigned(ImplementationSection) Then
    ImplementationSection.PostProcess();
End;

{ CaseStatement }
Function CaseStatement.Reduce:Statement;
Var
  I:Integer;
Begin
  For I:=0 To Pred(Self.EntryCount) Do
  Begin
    ReduceAndReplace(Self.Entries[I].Result);
  End;

  ReduceAndReplace(Self.WhenFalse);
  Result := Self;
End;

{ ConstantExpression }
Function ConstantExpression.Reduce;
Var
  C:Constant;
Begin
  C := Nil;
  If (Self.Value = 'MAX_SAMPLE_COUNT') Then
    IntToString(2);

  If Assigned(CurrentModule.InterfaceSection) Then
    C := CurrentModule.InterfaceSection.GetConstant(Self.Value);
  If (C=Nil) And (Assigned(CurrentModule.ImplementationSection)) Then
    C := CurrentModule.ImplementationSection.GetConstant(Self.Value);
  If (C=Nil) And (Assigned(CurrentModule.CurrentFunction)) Then
    C := CurrentModule.CurrentFunction.GetConstant(Self.Value);

  If Assigned(C) Then
  Begin
    WriteLn('Reducing ',C.Name);
    If (C.Name='terraHeader') Then
      IntToString(2);

    C.Value := C.Value.Reduce;
    If C.Value Is ConstantExpression Then
      Self.Value := ConstantExpression(C.Value).Value;
  End;

  Result := Self;
End;

Function Variable.GetPath: String;
Begin
  Result := LowStr(Self.Name);
  If Assigned(Owner) Then
    Result := Owner.GetPath + '.' + Result;
End;

{ VariableAcessor }
Function VariableAcessor.GetPathFront(First:Boolean=True): String;
Begin
  If (Not First) Then
    Result := LowStr(Self.Element)
  Else
    Result := '';

  If Assigned(Self.Acessor) Then
    Result := Result + '.'+Self.Acessor.GetPathFront(False);
End;

Function VariableAcessor.GetPathBack: String;
Begin
  Result := LowStr(Self.Element);
  If Assigned(Self.Parent) Then
    Result := Parent.GetPathBack + '.' + Result;
End;

Procedure VariableAcessor.PostProcess;
Begin
  ReduceAndReplace(Self.Index);

  If Assigned(Self.Acessor) Then
    Self.Acessor.PostProcess();
End;

{ VariableExpression }
Function VariableExpression.Reduce():Expression;
Begin
  If Assigned(Self.Acessor) Then
    Self.Acessor.PostProcess();

  Result := Self;
End;

{ CallExpression }
Function CallExpression.Reduce:Expression;
Var
  I:Integer;
Begin
  If Assigned(Self.Acessor) Then
    Self.Acessor.PostProcess();

  If (LowStr(Self.Name) = 'succ') Then
  Begin
    Result := CurrentModule.Builder.CreateBinaryOpExpression();
    BinaryExpression(Result).Op := '+';
    BinaryExpression(Result).A := Self.Acessor.Arguments[0];
    BinaryExpression(Result).B := CurrentModule.Builder.CreateConstantExpression();
    ConstantExpression(BinaryExpression(Result).B).Value := '1';
    Result := BinaryExpression(Result).Reduce();
    Exit;
  End;

  If (LowStr(Self.Name) = 'pred') Then
  Begin
    Result := CurrentModule.Builder.CreateBinaryOpExpression();
    BinaryExpression(Result).Op := '-';
    BinaryExpression(Result).A := Self.Acessor.Arguments[0];
    BinaryExpression(Result).B := CurrentModule.Builder.CreateConstantExpression();
    ConstantExpression(BinaryExpression(Result).B).Value := '1';
    Result := BinaryExpression(Result).Reduce();
    Exit;
  End;


  If (CurrentModule.IsType(Self.Name)) Then
  Begin
    Result := CurrentModule.Builder.CreateCast();
    CastExpression(Result).TypeName := Self.Name;
    CastExpression(Result).Body := Self.Acessor.Arguments[0];
    Exit;
  End;

  Result := Self;
End;

Function IsCompareOperator(S:String):Boolean;
Begin
  Result := (S='>') Or (S='<') Or (S='>=') Or (S='<=');    
End;

Function InvertOp(S:String):String;
Begin
  If (S='>') Then
    Result := '<='
  Else
  If (S='<') Then
    Result := '>='
  Else
  If (S='>=') Then
    Result := '<'
  Else
  If (S='<=') Then
    Result := '>'
  Else
    Result := S;
End;

{ UnaryExpression }
Function UnaryExpression.Reduce():Expression;
Begin
  ReduceAndReplace(A);

  If (Self.Op = 'not') Then
  Begin
    If (Self.A Is BinaryExpression) And (IsCompareOperator(BinaryExpression(A).Op)) Then
    Begin
      BinaryExpression(A).Op := InvertOp(BinaryExpression(A).Op);
      Result := A;
      Exit;
    End;
  End;

  Result := Self;
End;

{ BinaryExpression }
Function BinaryExpression.Reduce:Expression;
Var
  S:String;
  X,Y:Integer;
  AA,BB:String;
Begin
  ReduceAndReplace(A);
  ReduceAndReplace(B);

  If (A Is ConstantExpression) And (B Is ConstantExpression) And (Op<>'') Then
  Begin
    AA := CurrentModule.EvaluateConstant(ConstantExpression(A).Value);
    BB := CurrentModule.EvaluateConstant(ConstantExpression(B).Value);
    If (AA<>'') And (BB<>'') Then
    Begin
      X := StringToInt(AA);
      Y := StringToInt(BB);
      Case Op[1] Of
      '+':  S := IntToString(X+Y);
      '-':  S := IntToString(X-Y);
      '*':  S := IntToString(X*Y);
      '/':  S := IntToString(X Div Y);
      'd':  S := IntToString(X Div Y);
      'm':  S := IntToString(X Mod Y);
      Else
        S := '';
      End;

      If S<>'' Then
      Begin
        Result := CurrentModule.Builder.CreateConstantExpression();
        ConstantExpression(Result).Value := S;
        Exit;
      End;
    End;
  End;

  Result := Self;
End;

{ FunctionCall }
Function FunctionCall.Reduce:Statement;
Var
  I:Integer;
Begin
  If Assigned(Self.Acessor) Then
    Self.Acessor.PostProcess();

  {For I:=0 To Pred(Self.ArgumentCount) Do
    ReduceAndReplace(Self.Arguments[I]);

  If (LowStr(Self.Name)='inc') Or (LowStr(Self.Name)='dec') Then
  Begin
    Result := CurrentModule.Builder.CreateIncrement();
    If (Arguments[0] Is VariableExpression) Then
    Begin
      IncrementStatement(Result).V := VariableExpression(Arguments[0]).V;
      IncrementStatement(Result).Acessor := VariableExpression(Arguments[0]).Acessor;
      IncrementStatement(Result).Negative := (Self.Name='dec');
      If (Self.ArgumentCount>1) Then
        IncrementStatement(Result).Ammount := Arguments[1]
      Else
      Begin
        IncrementStatement(Result).Ammount := CurrentModule.Builder.CreateConstantExpression();
        ConstantExpression(IncrementStatement(Result).Ammount).Value := '1';
      End;
    End Else
      Error('Variable expected!');
    Exit;
  End;}

  Result := Self;
End;

{ Assignment }
Function Assignment.Reduce:Statement;
Begin
  If Assigned(Self.Acessor) Then
    Self.Acessor.PostProcess();

  ReduceAndReplace(Self.Result);
  Result := Self;
End;

{ IfStatement }
Function IfStatement.Reduce:Statement;
Begin
  ReduceAndReplace(Condition);
  ReduceAndReplace(WhenTrue);
  ReduceAndReplace(WhenFalse);
  Result := Self;
End;

{ WhileStatement }
Function WhileStatement.Reduce():Statement;
Begin
  ReduceAndReplace(Condition);
  ReduceAndReplace(Body);
  Result := Self;
End;

{ RepeatStatement }
Function RepeatStatement.Reduce:Statement;
Var
  P:Expression;
Begin
  ReduceAndReplace(Condition);

  P := CurrentModule.Builder.CreateUnaryOpExpression();
  UnaryExpression(P).Op := 'not';
  UnaryExpression(P).A := Condition;
  Self.Condition := P;
  ReduceAndReplace(Condition);

  ReduceAndReplace(Body);
  Result := Self;
End;

{ ForStatement }
Function ForStatement.Reduce:Statement;
Begin
  If Assigned(Self.Acessor) Then
    Self.Acessor.PostProcess();

  ReduceAndReplace(StartValue);
  ReduceAndReplace(EndValue);
  ReduceAndReplace(Body);
  Result := Self;
End;

{ WithStatement }
Function WithStatement.GetVariable(S: String; Ac:VariableAcessor): Variable;
Begin
  Result := V.GetVariable(S, Ac);
  If Assigned(Result) Then
    Exit;

  If Assigned(Parent) Then
  Begin
    Result := Parent.GetVariable(S, Ac);
    If Assigned(Result) Then
      Exit;
  End;

  Result := Nil;
End;

Function WithStatement.Reduce:Statement;
Begin
  Self.Acessor.PostProcess();
  ReduceAndReplace(Body);
  Result := Self;
End;

{ IncrementStatement }
Function IncrementStatement.Reduce: Statement;
Begin
  Acessor.PostProcess();
  Result := Self;
End;

Function Module.SearchFile(S: String): String;
Begin
  Result := FileManager.Instance.SearchResourceFile(S);
End;

Procedure Module.AddIdentifier(S: String);
Var
  SS:String;
  I:Integer;
Begin
  SS := S;
  S := LowStr(S);
  For I:= 0 To Pred(SymbolCount) Do
  If (LowStr(SymbolTable[I])= S) Then
    Exit;

  Inc(SymbolCount);
  SetLength(SymbolTable, SymbolCount);
  SymbolTable[Pred(SymbolCount)] := SS;
End;

Function Module.GetIdentifier(S: String): String;
Var
  SS:String;
  I:Integer;
Begin
  SS := S;
  S := LowStr(S);
  For I:= 0 To Pred(SymbolCount) Do
  If (LowStr(SymbolTable[I])= S) Then
  Begin
    Result := SymbolTable[I];
    Exit;
  End;

  Result := SS;
End;

Function Module.IsFunction(S: String): Boolean;
Begin
  If Assigned(InterfaceSection) Then
  Begin
    Result := InterfaceSection.IsFunction(S);
    If Result Then
      Exit;
  End;

  If Assigned(ImplementationSection) Then
  Begin
    Result := ImplementationSection.IsFunction(S);
    If Result Then
      Exit;
  End;
End;

Function Module.IsConstant(S: String): Boolean;
Begin
  Result := False;
  If S='' Then
    Exit;

  If (IsNumber(S)) Or (IsString(S)) Or (LowStr(S)='nil') Then
  Begin
    Result := True;
    Exit;
  End;

  If Assigned(InterfaceSection) Then
  Begin
    Result := InterfaceSection.IsConstant(S);
    If Result Then
      Exit;
  End;

  If Assigned(ImplementationSection) Then
  Begin
    Result := ImplementationSection.IsConstant(S);
    If Result Then
      Exit;
  End;
End;

Function Module.IsType(S: String): Boolean;
Begin
  S := LowStr(S);

  If Assigned(InterfaceSection) Then
  Begin
    Result := InterfaceSection.IsType(S);
    If Result Then
      Exit;
  End;

  If Assigned(ImplementationSection) Then
  Begin
    Result := ImplementationSection.IsType(S);
    If Result Then
      Exit;
  End;
End;

{ CastExpression }
Function CastExpression.Reduce: Expression;
Begin
  Result := Self;
End;

Function Module.GetVariable(S: String; Ac:VariableAcessor): Variable;
Begin
  If (Ac=Nil) And (CurrentWith<>Nil) Then
  Begin
    Result := GetVariable(S, CurrentWith.Acessor);
    If Assigned(Result) Then
      Exit;
  End;

  If Assigned(InterfaceSection) Then
  Begin
    Result := InterfaceSection.GetVariable(S, Ac);
    If Assigned(Result) Then
      Exit;
  End;

  If Assigned(ImplementationSection) Then
  Begin
    Result := ImplementationSection.GetVariable(S, Ac);
    If Assigned(Result) Then
      Exit;
  End;

  Result := Nil;
End;

Procedure Module.ExpandVariable(V: Variable; Depth:Integer);
Var
  T:VarType;
  I:Integer;
  IsPointer:Boolean;
Begin
  If (Depth>3) Then
    Exit;

  If (Pos('Package', CurrentModule.Name)>0) Then
    IntToString(2);

  T := Self.GetType(V.VarType.Primitive);
  If (T=Nil) Then
    Exit;

  If (T.IsPointer) Then
  Begin
    IsPointer := True;
    StringToInt(T.TypeName);
    T := Self.GetType(T.TypeName);
  End Else
    IsPointer := False;

  If Assigned(T) Then
  Begin
    If (T Is RecordType) Then
    Begin
      V.ChildCount := 0;
      For I:=0 To Pred(RecordType(T).Variables.VariableCount) Do
      Begin
        Inc(V.ChildCount);
        SetLength(V.Childs, V.ChildCount);
        V.Childs[Pred(V.ChildCount)] := Builder.CreateVariable();
        V.Childs[Pred(V.ChildCount)].VarType := RecordType(T).Variables.Variables[I].VarType;
        V.Childs[Pred(V.ChildCount)].Name := RecordType(T).Variables.Variables[I].Name;
        V.Childs[Pred(V.ChildCount)].Parent := RecordType(T).Variables.Variables[I].Parent;
        V.Childs[Pred(V.ChildCount)].Owner := V;
        If (Not IsPointer) Then
          ExpandVariable(V.Childs[Pred(V.ChildCount)], Depth+1);
      End;
    End Else
    If (T Is ClassSignature) Then
    Begin
      V.ChildCount := 0;
      For I:=0 To Pred(ClassSignature(T).PrivateVariables.VariableCount) Do
      Begin
        Inc(V.ChildCount);
        SetLength(V.Childs, V.ChildCount);
        V.Childs[Pred(V.ChildCount)] := Builder.CreateVariable();
        V.Childs[Pred(V.ChildCount)].VarType := ClassSignature(T).PrivateVariables.Variables[I].VarType;
        V.Childs[Pred(V.ChildCount)].Name := ClassSignature(T).PrivateVariables.Variables[I].Name;
        V.Childs[Pred(V.ChildCount)].Parent := ClassSignature(T).PrivateVariables.Variables[I].Parent;
        V.Childs[Pred(V.ChildCount)].Owner := V;
        If (Not IsPointer) And (LowStr(V.Childs[Pred(V.ChildCount)].Name)<>'self') Then
          ExpandVariable(V.Childs[Pred(V.ChildCount)], Depth+1);
      End;
      For I:=0 To Pred(ClassSignature(T).ProtectedVariables.VariableCount) Do
      Begin
        Inc(V.ChildCount);
        SetLength(V.Childs, V.ChildCount);
        V.Childs[Pred(V.ChildCount)] := Builder.CreateVariable();
        V.Childs[Pred(V.ChildCount)].VarType := ClassSignature(T).ProtectedVariables.Variables[I].VarType;
        V.Childs[Pred(V.ChildCount)].Name := ClassSignature(T).ProtectedVariables.Variables[I].Name;
        V.Childs[Pred(V.ChildCount)].Parent := ClassSignature(T).ProtectedVariables.Variables[I].Parent;
        V.Childs[Pred(V.ChildCount)].Owner := V;
        If (Not IsPointer) And (LowStr(V.Childs[Pred(V.ChildCount)].Name)<>'self') Then
          ExpandVariable(V.Childs[Pred(V.ChildCount)], Depth+1);
      End;
      For I:=0 To Pred(ClassSignature(T).PublicVariables.VariableCount) Do
      Begin
        Inc(V.ChildCount);
        SetLength(V.Childs, V.ChildCount);
        V.Childs[Pred(V.ChildCount)] := Builder.CreateVariable();
        V.Childs[Pred(V.ChildCount)].VarType := ClassSignature(T).PublicVariables.Variables[I].VarType;
        V.Childs[Pred(V.ChildCount)].Name := ClassSignature(T).PublicVariables.Variables[I].Name;
        V.Childs[Pred(V.ChildCount)].Parent := ClassSignature(T).PublicVariables.Variables[I].Parent;
        V.Childs[Pred(V.ChildCount)].Owner := V;
        If (Not IsPointer) And (LowStr(V.Childs[Pred(V.ChildCount)].Name)<>'self') Then
          ExpandVariable(V.Childs[Pred(V.ChildCount)], Depth+1);
      End;
    End;
  End;
End;

Function Module.GetType(S: String): VarType;
Var
  I:Integer;
Begin
  If Assigned(InterfaceSection) Then
  Begin
    Result := InterfaceSection.GetType(S);
    If Assigned(Result) Then
      Exit;
  End;

  If Assigned(ImplementationSection) Then
  Begin
    Result := ImplementationSection.GetType(S);
    If Assigned(Result) Then
      Exit;
  End;

  Result := Nil;
End;

{Function Module.ProcessAcessor(Ac: VariableAcessor; V: Variable): VariableAcessor;
Var
  V2:Variable;
  Temp:VariableAcessor;
  A,B:String;
Begin
  Temp := Ac;
  V2 := V;
  Result := Ac;
  While (True) Do
  Begin
    A := Result.GetPathBack+Result.GetPathFront;
    B := V2.GetPath;
    If (Length(B)<Length(A)) Then
      Break;

    If (A=B) Then
      Break;
    V := V.Owner;
    If (V=Nil) Then
      Break;

    Ac := Temp;
    Result := Builder.CreateAcessor();
    Result.V := V;
    Result.Element := V.Name;
    Result.Index := Nil;
    Result.Acessor := Ac;
    Result.Dereferenced := False;
    Ac.Parent := Result;
    Temp := Result;
  End;
End;}

Procedure Module.AddDefine(S: String);
Var
  I:Integer;
Begin
  S := lowStr(S);
  For I:=0 To Pred(DefineCount) Do
  If (Defines[I] = S) Then
    Exit;

  Inc(DefineCount);
  SetLength(Defines, DefineCount);
  Defines[Pred(DefineCount)] := S;
End;

Function Module.IsDefined(S: String): Boolean;
Var
  I:Integer;
Begin
  S := lowStr(S);
  If (S='max_match_is_258') Then
    IntToString(2);
    
  For I:=0 To Pred(DefineCount) Do
  If (Defines[I] = S) Then
  Begin
    Result := True;
    Exit;
  End;

  Result := False;
End;

Procedure Module.RemoveDefine(S: String);
Var
  I:Integer;
Begin
  S := lowStr(S);
  I := 0;
  While (I<DefineCount) Do
  If (Defines[I]=S) Then
  Begin
    Defines[I] := Defines[Pred(DefineCount)];
  End Else
    Inc(I);
End;

Function Module.GetCurrentLine: Integer;
Var
  Temp,Count:Integer;
  C:Char;
Begin
  If (Self=Nil) Or (Src=Nil) Then
  Begin
    IntToString(2);
    Result := 0;
    Exit;
  End;

  Count := Src.Position;
  Src.Seek(0);
  Temp := Count;
  Result := 1;
  While (Count>0) Do
  Begin
    Src.Read(@C, 1);
    If C=#13  Then
      Inc(Result);
    Dec(Count);
  End;
  If (Result=392) Then
    IntToString(2);
  Src.Seek(Temp);
End;

Function Module.EvaluateConstant(S: String): String;
Var
  I:Integer;
Begin
  If (IsNumber(S)) Then
  Begin
    Result := S;
    Exit;
  End;

  If Assigned(InterfaceSection) Then
  Begin
    Result := InterfaceSection.EvaluateConstant(S);
    If Result<>'' Then
      Exit;
  End;

  If Assigned(ImplementationSection) Then
  Begin
    Result := ImplementationSection.EvaluateConstant(S);
    If Result<>'' Then
      Exit;
  End;

  Result := '';
  //Error('Cannot evaluate constant: '+S);
End;

Function Module.LookAhead(Src:Stream; StopToken: String): String;
Var
  I,N:Integer;
  S:String;
  Temp:Integer;
  Inside:Boolean;
Begin
  N := Src.Position;
  Temp := LastIndex;
  Result := '';
  Repeat
    If (Src.EOF) Then
    Begin
      Src.Seek(N);
      Exit;
    End;

    S := GetToken(Src, Temp);
    If (S=StopToken) Then
    Begin
      Src.Seek(N);
      If Pos('INCLUDE',REsult)>0 Then
      IntToString(2);

      Inside := False;
      For I:=1 To Length(Result) Do
      If (Result[I]='''') Then
        Inside := Not Inside
      Else
      If (Inside) Then
        Result[I] := ' ';

      Exit;
    End;

    Result := Result + S + ' ';
  Until False;

  LastLookAhead := Result;
End;

Function Module.LoadProperty: ClassProperty;
Var
  S:String;
Begin
  Result := Builder.CreateProperty();
  Result.Name := GetToken(Src, LastIndex);
  Expect(':');
  Result.PropType := LoadType();
  Repeat
    S := GetToken(Src, LastIndex);
    If Equals(';') Then
      Break;

    If Equals('read') Then
    Begin
      Result.Read := GetToken(Src, LastIndex);
    End Else
    If Equals('write') Then
    Begin
      Result.Write := GetToken(Src, LastIndex);
    End Else
      Error('Property declaration error!');
  Until False;
End;

{ ModuleList }
Function ModuleList.GetType(S: String): VarType;
Var
  I:Integer;
Begin
  For I:=0 To Pred(ModuleCount) Do
  Begin
    If (S='UI') And (Pos('UI',Modules[I].Name)>0) Then
      IntToString(2);
    Result := Modules[I].GetType(S);
    If Assigned(Result) Then
      Exit;
  End;

  Result := Nil;
End;

Procedure ModuleSection.CompileUses;
Var
  S:String;
  I:Integer;
  Temp:Module;
Begin
  For I:=0 To Pred(Self.UnitCount) Do
  Begin
    S := Units[I];

    Temp := GetModule(S);
    If Assigned(Temp) Then
      Continue;

    S := Parent.SearchFile(S+'.pas');
    If (S='') Then
    Begin
      S := LowStr(Units[I]);
      If (S<>'windows') And (S<>'sysutils') And (S<>'messages') And (S<>'mmsystem')
      And (S<>'math') And (S<>'nb30') Then
        Error('Cannot find unit: '+Units[I])
      Else
        S := '';
    End;

    If S<>'' Then
    Begin
      Temp := CurrentModule;
      Inc(Project.ModuleCount);
      SetLength(Project.Modules, Project.ModuleCount);
      Project.Modules[Pred(Project.ModuleCount)] := Module.Create;
      Try
        Project.Modules[Pred(Project.ModuleCount)].CompileInterface(S, Parent.Builder);
      Except
        ReadLn;
      End;
      CurrentModule := Self.Parent;
    End;
  End;
End;

Function ModuleList.GetVariable(S: String; Ac: VariableAcessor): Variable;
Var
  I:Integer;
Begin
  For I:=0 To Pred(Self.ModuleCount) Do
  If (Self.Modules[I]<>Nil) Then
  Begin
    Result := Modules[I].InterfaceSection.GetVariable(S, Ac);
    If Assigned(Result) Then
      Exit;
  End;

  Result := Nil;
End;

End.
