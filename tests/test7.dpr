Program test7;

{$APPTYPE CONSOLE}

Type
  Manager = Class
    X:Integer;
    Y:Integer;
    Constructor Create;
    Class Function Instance:Manager;
    Procedure Write(S:String);
  End;

Var
  _Instance:Manager;

{ Manager }
Constructor Manager.Create;
Begin
End;

class function Manager.Instance: Manager;
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
