Program testwithbethods;

Type
  Vector = Record
    X,Y:Integer;
    Function Length:Single;
  End;

Function Vector.Length:Single;
Begin
  Result := Sqrt(Sqr(X)+Sqr(Y));
End;

Begin
End.
