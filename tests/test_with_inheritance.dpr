Program test;

Type
  Creature = Class;

  TalkingBehavior = Interface
    Procedure Talk();
  End;

  EatingBehavior = Interface
    Procedure Eat(Food:String);
  End;

  House = Class
    Inhabitants:Array Of Creature;

    Procedure Add(P:Creature);
  End;

  Creature = Class(TInterfacedObject, EatingBehavior, TalkingBehavior)
    Procedure Talk(); Virtual; Abstract;
    Procedure Eat(Food:String); Virtual; Abstract;
	constructor Create();
  End;

  Dog = Class(Creature)
    Procedure Talk; Override;
    Procedure Eat(Food:String); Override;

  End;

{ Dog }
Procedure Dog.Eat(Food: String);
Begin
  WriteLn('lol, eating a ',Food);
End;

Procedure Dog.Talk;
Begin
  WriteLn('miau!');
  Eat('comida');
End;

Var
  P:Creature;
 
{ House }
procedure House.Add(P: Creature);
begin
    SetLength(Inhabitants, Succ(Length(Inhabitants)));
    Inhabitants[Pred(Length(Inhabitants))] := P;
end;

Begin
  P := Dog.Create;
End.