Program test7;

Type
  Manager = Class
    X:Integer;
    Y:Integer;
    Constructor Create;
    Function Instance:Manager;
    Procedure Writet(S:String);
  End;

Var
  _Instance:Manager;

{ Manager }
Constructor Manager.Create;
Begin
End;

function Manager.Instance: Manager;
begin
  If Not assigned(_Instance) Then
    _Instance := Manager.Create();
  Result := _Instance;
end;

procedure Manager.Write(S: String);
begin
  WriteLn(S);
end;

Begin
  Manager.Instance().Write('test');
End.

