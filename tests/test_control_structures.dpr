program b;

{$APPTYPE CONSOLE}
Var
  I:Integer;

Begin
  For I:=0 To 5 Do
    writeln(I);

  For I:=5 DownTo 0 Do
    writeln(I);

  I:=0;
  Repeat
    writeln(I);
    Inc(I);
  Until (I>5);

  I:=0;
  While (I<5) Do
  Begin
    writeln(I);
    Inc(I);
  End;

End.
