program testclasses;

uses unit1;

type
  dog = class(animal)
    Procedure Eat(Food:Integer); Override;
  end;

var
  d:animal;

begin
  d := dfd.create;
  d.Eat(1);
  d.destroy;
end.
