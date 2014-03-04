Program testwithbethods;

Type
  Vector = Record
    X,Y:Integer;
    Function Length:Single;
  End;

Function Vector.Length:Single;
Begin
  X := 3 + 4 + 5;
  Y := 3 + Sqrt(5);
  Result := Sqrt(Sqr(X)+Sqr(Y));
End;

Begin
End.
