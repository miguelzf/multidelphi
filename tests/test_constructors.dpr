program testconsrt;

type
  animal = class
    legs:integer;
    constructor create(legs:integer);
    procedure eat; virtual; abstract;
  end;

  dog = class(animal)
    constructor create();
    procedure eat; override;
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

procedure dog.eat;
begin
  writeln('lol');
end;

begin
   a := dog.create();
end.
