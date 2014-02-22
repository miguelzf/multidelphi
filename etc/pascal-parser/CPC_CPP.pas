Unit CPC_CPP;

Interface
Uses TERRA_Utils, TERRA_IO, TERRA_FileIO, CPC_Parser;

Type
  CppVariable = Class(Variable)
    Procedure Emit; Override;
  End;

  CppArgument = Class(Argument)
    Procedure Emit; Override;
  End;

  CppConstant = Class(Constant)
    Procedure Emit; Override;
  End;

  CppType = Class(VarType)
    Procedure Emit; Override;
  End;

  CppFunctionType = Class(FunctionType)
    Procedure Emit; Override;
  End;

  CppEnumType = Class(EnumType)
    Procedure Emit; Override;
  End;

  CppRecordType = Class(RecordType)
    Procedure Emit; Override;
  End;

  CppClassProperty = Class(ClassProperty)
    Procedure Emit; Override;
  End;

  CppClassType = Class(ClassSignature)
    Procedure Emit; Override;
  End;

  CppStatementBlock = Class(StatementBlock)
    Procedure Emit; Override;
  End;

  CppConstantExpression = Class(ConstantExpression)
    Procedure Emit; Override;
  End;

  CppVariableExpression = Class(VariableExpression)
    Procedure Emit; Override;
  End;

  CppCastExpression = Class(CastExpression)
    Procedure Emit; Override;
  End;

  CppCallExpression = Class(CallExpression)
    Procedure Emit; Override;
  End;

  CppUnaryExpression = Class(UnaryExpression)
    Procedure Emit; Override;
  End;

  CppBinaryExpression = Class(BinaryExpression)
    Procedure Emit; Override;
  End;

  CppFunctionCall = Class(FunctionCall)
    Procedure Emit; Override;
  End;

  CppVariableAcessor = Class(VariableAcessor)
    Procedure Emit; Override;
  End;

  CppAssignment = Class(Assignment)
    Procedure Emit; Override;
  End;

  CppIfStatement = Class(IfStatement)
    Procedure Emit; Override;
  End;

  CppIncrementStatement = Class(IncrementStatement)
    Procedure Emit; Override;
  End;

  CppCaseStatement = Class(CaseStatement)
    Procedure Emit; Override;
  End;

  CppWhileStatement = Class(WhileStatement)
    Procedure Emit; Override;
  End;

  CppRepeatStatement = Class(RepeatStatement)
    Procedure Emit; Override;
  End;

  CppForStatement = Class(ForStatement)
    Procedure Emit; Override;
  End;

  CppWithStatement = Class(WithStatement)
    Procedure Emit; Override;
  End;

  CppFunctionSignature = Class(FunctionSignature)
    Procedure Emit(HasBody:Boolean); Override;
  End;

  CppModuleFunction = Class(ModuleFunction)
    Procedure Emit; Override;
  End;

  CppModuleSection = Class(ModuleSection)
    Procedure Emit; Override;
    Procedure PostProcess; Override;
  End;

  CppBuilder = Class(ModuleBuilder)
    Function CreateIF:IFStatement; Override;
    Function CreateWhile:WhileStatement; Override;
    Function CreateRepeat:RepeatStatement; Override;
    Function CreateFor:ForStatement; Override;
    Function CreateWith:WithStatement; Override;
    Function CreateCase:CaseStatement; Override;
    Function CreateIncrement:IncrementStatement; Override;
    Function CreateBlock:StatementBlock; Override;
    Function CreateAcessor:VariableAcessor; Override;
    Function CreateFunctionCall:FunctionCall; Override;
    Function CreateVariable:Variable; Override;
    Function CreateArgument:Argument; Override;
    Function CreateConstant():Constant; Override;
    Function CreateAssignment:Assignment; Override;
    Function CreateConstantExpression:Expression; Override;
    Function CreateVariableExpression:Expression; Override;
    Function CreateCallExpression:Expression; Override;
    Function CreateUnaryOpExpression:Expression; Override;
    Function CreateBinaryOpExpression:Expression; Override;
    Function CreateCast:Expression; Override;
    Function CreateType:VarType; Override;
    Function CreateRecord:RecordType; Override;
    Function CreateEnum:EnumType; Override;
    Function CreateClass:ClassSignature; Override;
    Function CreateProperty:ClassProperty; Override;
    Function CreateFunction:ModuleFunction; Override;
    Function CreateFunctionSignature:FunctionSignature; Override;
    Function CreateSection:ModuleSection; Override;
    Function CreateFunctionDefinition:FunctionType; Override;

    Procedure Compile(M:Module); Override;
    Procedure CompileSection(Section:ModuleSection);
  End;


Implementation

Function ResolveCppVariable(S:String):String;
Begin
  S := LowStr(S);
  If (S='self') Then
    Result := 'this'
  Else
    Result := S;
End;

Function ResolveCppType(S:String):String;
Begin
  S := LowStr(S);
  If (S='') Then
    Result := 'void'
  Else
  If (S='string') Then
    Result := 'std::string'
  Else
  If (S='integer') Then
    Result := 'int'
  Else
  If (S='cardinal') Then
    Result := 'unsigned int'
  Else
  If (S='boolean') Then
    Result := 'bool'
  Else
  If (S='byte') Then
    Result := 'unsigned char'
  Else
  If (S='shortint') Then
    Result := 'char'
  Else
    Result := CurrentModule.GetIdentifier(S);
End;

Function ResolveCppOperator(S:String):String;
Begin
  S := LowStr(S);
  If (S='@') Then
    Result := '&'
  Else
  If (S='shl') Then
    Result := '<<'
  Else
  If (S='shr') Then
    Result := '>>'
  Else
  If (S='^') Then
    Result := '*'
  Else
  If (S='=') Then
    Result := '=='
  Else
  If (S='or') Then
    Result := '||'
  Else
  If (S='and') Then
    Result := '&&'
  Else
  If (S='not') Then
    Result := '!'
  Else
  If (S='div') Then
    Result := '/'
  Else
  If (S='mod') Then
    Result := '%'
  Else
    Result := S;
End;

Function ResolveCppConstant(S:String):String;
Var
  SS:String;
Begin
  SS := LowStr(S);
  If (SS='nil') Then
    Result := 'NULL'
  Else
    Result := S;
End;

{ CppBuilder }
Function CppBuilder.CreateAcessor: VariableAcessor;
Begin
  Result := CppVariableAcessor.Create;
End;

Function CppBuilder.CreateArgument: Argument;
Begin
  Result := CppArgument.Create;
End;

Function CppBuilder.CreateAssignment: Assignment;
Begin
  Result := CppAssignment.Create;
End;

Function CppBuilder.CreateBinaryOpExpression: Expression;
Begin
  Result := CppBinaryExpression.Create;
End;

Function CppBuilder.CreateBlock: StatementBlock;
Begin
  Result := CppStatementBlock.Create;
End;

Function CppBuilder.CreateCallExpression: Expression;
Begin
  Result := CppCallExpression.Create;
End;

Function CppBuilder.CreateCase: CaseStatement;
Begin
  Result := CppCaseStatement.Create;
End;

Function CppBuilder.CreateConstantExpression: Expression;
Begin
  Result := CppConstantExpression.Create;
End;

Function CppBuilder.CreateFunction: ModuleFunction;
Begin
  Result := CppModuleFunction.Create;
End;

Function CppBuilder.CreateFunctionCall: FunctionCall;
Begin
  Result := CppFunctionCall.Create;
End;

Function CppBuilder.CreateIF: IFStatement;
Begin
  Result := CppIFStatement.Create;
End;

Function CppBuilder.CreateSection: ModuleSection;
Begin
  Result := CppModuleSection.Create;
End;

function CppBuilder.CreateUnaryOpExpression: Expression;
Begin
  Result := CppUnaryExpression.Create;
End;

Function CppBuilder.CreateVariable: Variable;
Begin
  Result := CppVariable.Create;
End;

Function CppBuilder.CreateVariableExpression: Expression;
Begin
  Result := CppVariableExpression.Create;
End;

Function CppBuilder.CreateWhile: WhileStatement;
Begin
  Result := CppWhileStatement.Create;
End;

Function CppBuilder.CreateWith: WithStatement;
Begin
  Result := CppWithStatement.Create;
End;

Function CppBuilder.CreateConstant: Constant;
Begin
  Result := CppConstant.Create;
End;

Function CppBuilder.CreateRecord: RecordType;
Begin
  Result := CppRecordType.Create;
End;

Function CppBuilder.CreateType: VarType;
Begin
  Result := CppType.Create;
End;

Function CppBuilder.CreateClass: ClassSignature;
Begin
  Result := CppClassType.Create;
End;

Function CppBuilder.CreateFunctionSignature: FunctionSignature;
Begin
  Result := CppFunctionSignature.Create;
End;

Function CppBuilder.CreateFor: ForStatement;
Begin
  Result := CppForStatement.Create;
End;

Function CppBuilder.CreateRepeat: RepeatStatement;
Begin
  Result := CppRepeatStatement.Create;
End;

Function CppBuilder.CreateIncrement: IncrementStatement;
Begin
  Result := CppIncrementStatement.Create;
End;

Function CppBuilder.CreateCast: Expression;
Begin
  Result := CppCastExpression.Create;
End;

Function CppBuilder.CreateFunctionDefinition: FunctionType;
Begin
  Result := CppFunctionType.Create();
End;

Function CppBuilder.CreateProperty: ClassProperty;
Begin
  Result := CppClassProperty.Create();
End;

Function CppBuilder.CreateEnum: EnumType;
Begin
  Result := CppEnumType.Create();
End;

{ CppVariable }
Procedure CppVariable.Emit;
Var
  S:String;
  C:ClassSignature;
Begin
  C := CurrentModule.FindClass(Self.VarType.Primitive, False);
  S := '';
  If Assigned(C) Then
  Begin
    S := '*';
  End;

  S := S + ResolveCppType(Self.VarType.Primitive) + ' ';
  If Self.VarType.IsPointer Then
    S := S +'*';
  S := S + ResolveCppVariable(CurrentModule.GetIdentifier(Self.Name));
  If (Self.VarType.Count>1) Then
  Begin
    S := S + '['+IntToString(Self.VarType.Count)+']';
  End;

  Output(S, Self.Nest, False);

  If (Self.InitialValue<>Nil) Then
  Begin
    Output(' = ',0, False);
    Self.InitialValue.Emit();
  End;

  Output(';',0, True);
End;

{ CppArgument }

procedure CppArgument.Emit;
begin
  Output(ResolveCppType(Self.VarType.Primitive) +' '+ Self.Name, 0, False);
end;

{ CppStatementBlock }

procedure CppStatementBlock.Emit;
Var
  I:Integer;
begin
  Output('{', Self.Nest, True);
  For I:=0 To Pred(Self.StatementCount) Do
    Self.Statements[I].Emit();
  Output('}', Self.Nest, True);
end;

{ CppFunctionCall }
Procedure CppFunctionCall.Emit;
Var
  S:String;
  I:Integer;
  C:ClassSignature;
Begin
  Output('',Self.Nest, False);
  If (LowStr(Self.Acessor.Element) = 'inherited') Then
  Begin
    If (Self.Parent=Nil) Then
    Begin
      Error('Inherited without parent');
    End;

    C := CurrentModule.FindClass(Self.Parent.Signature.Owner, False);
    If (Assigned(C)) And (C.Ancestor<>Nil) Then
    Begin
      Output(C.Ancestor.Name+'::'+CurrentModule.GetIdentifier(Self.Parent.Signature.Name), Self.Nest, True);
      Exit;
    End;

    Error('Cannot call inherited here!');
  End;

  Self.Acessor.Emit();
  Output(';',0, True);
End;

{ CppVariableAcessor }
Procedure CppVariableAcessor.Emit;
Var
  I:Integer;
Begin
  If (Self.Parent<>Nil) Then
  Begin
    {If (Self.V.Owner.VarType.IsPointer) Then
      Output('->', 0, False)
    Else}
      Output('.', 0, False);
  End;

  If (Self.Dereferenced) Then
    Output('(*',0, False);

  Output(CurrentModule.GetIdentifier(ResolveCppVariable(Self.Element)), 0, False);
  If Assigned(Self.Index) Then
  Begin
    Output('[',0, False);
    Self.Index.Emit();
    Output(']',0, False);
  End;

  If (Self.IsFunctionCall) Then
  Begin
    Output('(',0, False);
    For I:=0 To Pred(Self.ArgumentCount) Do
      Self.Arguments[I].Emit();
    Output(')',0, False);
  End;

  If (Self.Dereferenced) Then
    Output(')',0, False);

  If (Assigned(Self.Acessor)) Then
    Self.Acessor.Emit();
End;

{ CppAssignment }
Procedure CppAssignment.Emit;
Begin
  Output('', Self.Nest, False);
  Self.Acessor.Emit();
  Output(' = ', 0, False);
  Self.Result.Emit();
  Output(';', 0, True);
End;

{ CppIfStatement }

procedure CppIfStatement.Emit;
begin
  Output('if (', Self.Nest, False);
  Self.Condition.Emit();
  Output(')', 0, True);
  Self.WhenTrue.Emit();
  If (Assigned(Self.WhenFalse)) Then
  Begin
    Output('else', Self.Nest, True);
    Self.WhenFalse.Emit();
  End;
end;

{ CppCaseStatement }

procedure CppCaseStatement.Emit;
begin
  inherited;

end;

{ CppForStatement }
Procedure CppForStatement.Emit;
Begin
  Output('for (', Self.Nest, False);
  Self.Acessor.Emit();
  Output(' = ', 0, False);
  Self.StartValue.Emit();
  Output('; ', 0, False);
  Self.Acessor.Emit();
  If (Inverted) Then
    Output('>=', 0, False)
  Else
    Output('<=', 0, False);
  Self.EndValue.Emit();
  Output('; ', 0, False);
  Self.Acessor.Emit();
  If (Inverted) Then
    Output('--', 0, False)
  Else
    Output('++', 0, False);
  Output(')', 0, True);
  Self.Body.Emit();
End;

{ CppRepeatStatement }
Procedure CppRepeatStatement.Emit;
Begin
  Output('do ', Self.Nest, False);
  Self.Body.Emit();
  Output(' while (', 0, False);
  Self.Condition.Emit();
  Output(');', 0, True);
End;

{ CppWhileStatement }
Procedure CppWhileStatement.Emit;
Begin
  Output('while (', Self.Nest, False);
  Self.Condition.Emit();
  Output(')', 0, True);
  Self.Body.Emit();
End;

{ CppWithStatement }
Procedure CppWithStatement.Emit;
Begin
  Self.Body.Emit();
End;

{ CppModuleFunction }
Procedure CppModuleFunction.Emit;
Var
  S:String;
  I:Integer;
  Temp:FileStream;
Begin
  Temp := GetOutputFile();
  If Self.Signature.Owner<>'' Then
  Begin
    SetOutputFile(Self.Signature.Owner+'.cpp');
    OutputInclude('#include "'+Self.Signature.Owner+'.h"');
  End;

  For I:=0 To Pred(Constants.ConstantCount) Do
  Begin
    Constants.Constants[I].Emit();
  End;

  For I:=0 To Pred(Types.TypeCount) Do
  Begin
    Types.Types[I].Emit();
  End;

  Self.Signature.Emit(True);
  Output('{', 0, True);

  For I:=0 To Pred(Variables.VariableCount) Do
  Begin
    Variables.Variables[I].Emit();
  End;

  If Assigned(Self.Statements) Then
    Self.Statements.Emit();
  If Self.Signature.ResultType.Primitive<>'' Then
  Begin
    Output('return result;', Self.Nest, True);
  End;
  Output('}', Self.Nest, True);
  Output('', 0, True);

  SetOutputFile(Temp);
End;

{ CppModuleSection }

Procedure CppModuleSection.Emit;
Var
  M:Module;
  I:Integer;
Begin
  For I:=0 To Pred(Self.UnitCount) Do
  Begin
    M:= GetModule(Self.Units[I]);
    If Assigned(M) Then
      Continue;

    OutputInclude('#include "'+Self.Units[I]+'.h"');
  End;
End;

Procedure CppModuleSection.PostProcess;
Begin
  If Assigned(Init) Then
  Begin
    Inc(Self.FunctionCount);
    SetLength(Self.Functions, Self.FunctionCount);
    Self.Functions[Pred(Self.FunctionCount)] := CurrentModule.Builder.CreateFunction();
    Self.Functions[Pred(Self.FunctionCount)].Signature := CurrentModule.Builder.CreateFunctionSignature();
    Self.Functions[Pred(Self.FunctionCount)].Signature.Name := Self.Parent.Name+'_init';
    Self.Functions[Pred(Self.FunctionCount)].Signature.Owner := '';
    Self.Functions[Pred(Self.FunctionCount)].Signature.Genre := functionProcedure;
    Self.Functions[Pred(Self.FunctionCount)].Signature.ArgumentCount := 0;
    Self.Functions[Pred(Self.FunctionCount)].Statements := Self.Init;
  End;

  If Assigned(Final) Then
  Begin
    Inc(Self.FunctionCount);
    SetLength(Self.Functions, Self.FunctionCount);
    Self.Functions[Pred(Self.FunctionCount)] := CurrentModule.Builder.CreateFunction();
    Self.Functions[Pred(Self.FunctionCount)].Signature := CurrentModule.Builder.CreateFunctionSignature();
    Self.Functions[Pred(Self.FunctionCount)].Signature.Name := Self.Parent.Name+'_final';
    Self.Functions[Pred(Self.FunctionCount)].Signature.Owner := '';
    Self.Functions[Pred(Self.FunctionCount)].Signature.Genre := functionProcedure;
    Self.Functions[Pred(Self.FunctionCount)].Signature.ArgumentCount := 0;
    Self.Functions[Pred(Self.FunctionCount)].Statements := Self.Final;
  End;

  Inherited;
End;

{ CppConstantExpression }
Procedure CppConstantExpression.Emit;
Var
  S:String;
Begin
  S := ResolveCppConstant(CurrentModule.GetIdentifier(Self.Value));
  If IsString(S) Then
  Begin
    S[1] := '"';
    S[Length(S)] := '"';
  End;
  Output(S, 0, False);
End;

{ CppVariableExpression }

procedure CppVariableExpression.Emit;
begin
  Self.Acessor.Emit();
end;

{ CppCallExpression }
procedure CppCallExpression.Emit;
Begin
  Self.Acessor.Emit();
End;

{ CppUnaryExpression }

procedure CppUnaryExpression.Emit;
begin
  Output(ResolveCppOperator(Op), 0, False);
//  Output('(', 0, False);
  A.Emit();
//  Output(')', 0, False);
end;

{ CppBinaryExpression }
procedure CppBinaryExpression.Emit;
begin
  If (A=Nil) Or (B=Nil) Then
  IntToString(2);
  Output('(', 0, False);
  A.Emit();
  Output(' '+ResolveCppOperator(Op)+' ', 0, False);
  B.Emit();
  Output(')', 0, False);
end;

{ CppFunctionSignature }
Procedure CppFunctionSignature.Emit(HasBody:Boolean);
Var
  S:string;
  I:Integer;
Begin
  S := '';
  If (Self.IsVirtual) And (Not HasBody) Then
    S := S + 'virtual ';

  If (Self.Genre <> functionConstructor) And (Self.Genre <> functionDestructor) Then
    S := S + ResolveCppType(ResultType.Primitive)+' ';

  If (Self.Owner<>'') And (HasBody) Then
    S := S +Self.Owner+'::';

  If (Self.Genre = functionConstructor) Then
  Begin
      S := S+Self.Owner;
  End Else
  If (Self.Genre = functionDestructor) Then
  Begin
      S := S+'~'+Self.Owner;
  End Else
    S := S +CurrentModule.GetIdentifier(Name);

  S := S +'(';
  Output(S, 0, False);

  For I:=0 To Pred(Self.ArgumentCount) Do
  Begin
    Arguments[I].Emit();
    If (I<Pred(Self.ArgumentCount)) Then
      Output(', ',0, False);
  End;

  S := ')';
  If (Not HasBody) Then
  Begin
    If Self.IsAbstract Then
      S := S + ' = 0';
    S := S +';';
  End;
  Output(S, 0, True);
End;

{ CppConstant }
Procedure CppConstant.Emit;
Begin
  Output('#define '+CurrentModule.GetIdentifier(Self.Name)+' ',  Self.Nest, False);
  Self.Value.Emit();
  Output('', 0, True);
End;

{ CppRecordType }
Procedure CppRecordType.Emit;
Var
  I:Integer;
Begin
  Output('typedef struct {', Self.Nest, True);
  For I:=0 To Pred(Variables.VariableCount) Do
  Begin
    Output(#9, 0, False);
    Variables.Variables[I].Emit();
  End;
  Output('} '+CurrentModule.GetIdentifier(Self.Name)+';', 0, True);
End;

{ CppClassType }
Procedure CppClassType.Emit;
Var
  I:Integer;
  Temp:FileStream;
Begin
  Temp := GetOutputFile();
  SetOutputFile(LowStr(Self.Name)+'.h');

  Output('#ifndef '+UpStr(Self.Name)+'_H',0, True);
  Output('#define '+UpStr(Self.Name)+'_H',0, True);

  If Assigned(Self.Ancestor) Then
  Begin
    OutputInclude('#include "'+LowStr(Self.Ancestor.Name)+'.h"');
  End;

  Output('class '+CurrentModule.GetIdentifier(Self.Name), Self.Nest, False);
  If (Self.Ancestor<>Nil) Then
  Begin
    Output(': public '+CurrentModule.GetIdentifier(Self.Ancestor.Name), 0, False);
  End;
  Output(' {', 0, True);

  If (Self.PrivateVariables.VariableCount>0) Or (Self.PrivateMethods.MethodCount>0) Then
  Begin
    Output('private:',Self.Nest, True);
    For I:=0 To Pred(Self.PrivateVariables.VariableCount) Do
      Self.PrivateVariables.Variables[I].Emit();

    For I:=0 To Pred(Self.PrivateMethods.MethodCount) Do
      Self.PrivateMethods.Methods[I].Emit(False);
  End;

  If (Self.ProtectedVariables.VariableCount>1) Or (Self.ProtectedMethods.MethodCount>0) Then
  Begin
    Output('protected:',Self.Nest, True);
    For I:=0 To Pred(Self.ProtectedVariables.VariableCount) Do
    If (LowStr(Self.ProtectedVariables.Variables[I].Name)<>'self') Then
      Self.ProtectedVariables.Variables[I].Emit();

    For I:=0 To Pred(Self.ProtectedMethods.MethodCount) Do
      Self.ProtectedMethods.Methods[I].Emit(False);
  End;

  If (Self.PublicVariables.VariableCount>0) Or (Self.PublicMethods.MethodCount>0) Then
  Begin
    Output('public:',Self.Nest, True);
    For I:=0 To Pred(Self.PublicVariables.VariableCount) Do
      Self.PublicVariables.Variables[I].Emit();

    For I:=0 To Pred(Self.PublicMethods.MethodCount) Do
      Self.PublicMethods.Methods[I].Emit(False);
  End;

  Output('};', Self.Nest, True);
  Output('#endif',0, True);

  SetOutputFile(Temp);
End;

{ CppType }
Procedure CppType.Emit;
Begin
  Output('typedef ', Self.Nest, False);
  Output(ResolveCppType(Self.TypeName), 0, False);
  Output(' '+CurrentModule.GetIdentifier(Self.Name)+';', 0, True);
End;

{ CppIncrementStatement }
Procedure CppIncrementStatement.Emit;
Begin
  Output('', Self.Nest, False);
  Self.Acessor.Emit();
  If (Negative) Then
    Output(' -= ', 0, False)
  Else
    Output(' += ', 0, False);
  Ammount.Emit();

  Output(';', 0, True);
End;

{ CppEnumType }
Procedure CppEnumType.Emit;
Begin
End;

{ CppClassProperty }
Procedure CppClassProperty.Emit;
Begin
  inherited;
End;

{ CppCastExpression }
Procedure CppCastExpression.Emit;
Begin
  Output('(('+ResolveCppType(Self.TypeName)+')', 0, False);
  Self.Body.Emit();
  Output(')', 0, False);
End;

{ CppFunctionType }
Procedure CppFunctionType.Emit;
Begin
End;

Procedure CppBuilder.CompileSection(Section: ModuleSection);
Var
  I, J:Integer;
  M:Module;
Begin
  If (Section=Nil) Then
    Exit;

  Section.Emit();

  If (Section.Constants.ConstantCount=0) And (Section.Variables.VariableCount=0) And (Section.FunctionCount=0)
  And (Section.Body=Nil) Then
    Exit;

{  For I:=0 To Pred(Project.ModuleCount) Do
  If (Project.Modules[I]<>CurrentModule) Then
    Begin
      OutputInclude('#include "'+Project.Modules[I].Name+'.h"');
    End;}

  For I:=0 To Pred(Section.UnitCount) Do
  Begin
    M := GetModule(Section.Units[I]);
    If Not Assigned(M) Then
      Continue;

    For J:=0 To Pred(M.InterfaceSection.Types.TypeCount) Do
    If (M.InterfaceSection.Types.Types[J].TypeName='class') Then
    Begin
      OutputInclude('#include "'+LowStr(M.InterfaceSection.Types.Types[J].Name)+'.h"');
    End;

  End;

  For I:=0 To Pred(Section.Constants.ConstantCount) Do
  Begin
    Section.Constants.Constants[I].Emit();
  End;

  For I:=0 To Pred(Section.Types.TypeCount) Do
  Begin
    Section.Types.Types[I].Emit();
    If (Section.Types.Types[I].TypeName='class') Then
      OutputInclude('#include "'+LowStr(Section.Types.Types[I].Name)+'.h"');
  End;

  For I:=0 To Pred(Section.Variables.VariableCount) Do
  Begin
    Section.Variables.Variables[I].Emit();
  End;

  For I:=0 To Pred(Section.FunctionCount) Do
    Section.Functions[I].Emit();

  If Assigned(Section.Body) Then
  Begin
    Output('int main()', 0, True);
    Output('{', 0, True);

    For I:=0 To Pred(Project.ModuleCount) Do
    If (Project.Modules[I]<>CurrentModule) And (Project.Modules[I].ImplementationSection<>Nil)
    And (Project.Modules[I].ImplementationSection.Init<>Nil)Then
    Begin
      Output(Project.Modules[I].Name+'_init();',0,True);
    End;

    Section.Body.Emit();
    Output('return 0;', 0, True);
    Output('}', 0, True);
  End;
End;

Procedure CppBuilder.Compile(M: Module);
Begin
  //Output('using namespace std;', 0, True);
  SetOutputFile(LowStr(M.Name)+'.h');
  CompileSection(M.InterfaceSection);

  SetOutputFile(LowStr(M.Name)+'.cpp');
  CompileSection(M.ImplementationSection);
End;

End.
