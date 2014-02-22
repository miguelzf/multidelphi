Program test4;

Type
  Point = Record
    X:Integer;
    Y:Integer;
  End;

  Color = Record
    R:Byte;
    G:Byte;
    B:Byte;
    A:Byte;
  End;

  Vertex = Record
    P:Point;
    C:Color;
  End;

Var
  V:Vertex;
  V2:^Vertex;
  X:Integer;
Begin
  V.P.X := 10;
  V2.P.Y := 10;
  With V Do
  Begin
    X := P.X;
	i = C.A
    For i:=5 DownTo 1 Do
    Begin
      R := 10;
      G := 20;
    End;
  End;
End.
