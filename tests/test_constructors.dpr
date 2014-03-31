program testconsrt;

type
  animal = class
    legs:integer;
    constructor create(legs:integer);
  end;

  dog = class(animal)
    constructor create();
  end;

var
  a:animal;

{ animal }
constructor animal.create(legs: integer);
begin
  self.legs := legs;
end;

{ dog }

constructor dog.create;
begin
  inherited create(4);
end;

begin
  a := dog.create();
end.
