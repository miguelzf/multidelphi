Program test1;

Var
  A,B:String;

Const
  X = 'test';

Function Str1():String;
Begin
  Result := X;
End;

Var
  C:String;

Begin
  A := 'A';
  B := Str1();
  C := A+B;
End.
