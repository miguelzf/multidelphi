Program test6;

Type
  Point = Record
    X:Integer;
    Y:Integer;
  End;

Var
  A:Array[3..5] Of Point;
Begin
  A[3].X := 20;
End.
