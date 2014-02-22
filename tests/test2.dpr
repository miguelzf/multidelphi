Program test2;

Type
  Value = Integer;
  Point = Record
    X,Y:Value;
    Name:String;
  End;

Var
  A:Point;

Function MakePoint(X,Y:Integer):Point;
Begin
  Result.X := X;
  Result.Y := Y;
  Result.Name := 'point';
End;

Var
  B:Point;

Begin
  A := MakePoint(10, 10);
  B := MakePoint(20, 20);
End.
