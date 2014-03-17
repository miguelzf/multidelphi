Program test1;

Var
  A,B:String;
  A1,B1:String;
  A2,B2:String;
  A3,B3:String;

Const
  X = 'test';
  X1 = 'test1';
  X2 = 'test2';
  X3 = 'test3';

Function Str1():String;
Begin
  Result := X + Str1;
  
  str1 := Result - Str1;
End;

Var
  C:String;
  C1:String;
  C2:String;
  C3:String;

Begin
  A := 'A';
  B := Str1();
  C := A+B;
End.
