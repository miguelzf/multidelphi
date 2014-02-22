Program test3;

Uses Unit1;

Type
  Dog = Class(Animal)
    Constructor Create;
    Procedure Eat(Food:Integer); Override;
  End;

Var
  A:Animal;

{ Dog }
Constructor Dog.Create;
Begin
  Legs := 4;
End;

Procedure Dog.Eat(Food:Integer);
Begin
  Inherited;
  WriteLn('Dog is eating!');
End;

Var
  I, X:Integer;
  Y:^Integer;
Begin
  A := Dog.Create();
  I := 0;
  X := Integer(5);
  Y := @X;
  For I:=1 To X Do
  Begin
    A.Eat();
    Inc(I);
    Y^ := 0;
  End;
  X := Succ(5);
  Repeat
    A.Eat();
    Dec(X);
  Until (X<=0);
  //A.Destroy;
End.
