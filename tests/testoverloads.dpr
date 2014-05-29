program testoverloads;
{$APPTYPE CONSOLE}

function c(n:integer):integer; overload;
begin
  result := n * 2;
end;

function c(n:single):integer; overload;
begin
  result := trunc(n * 2);
end;

var
  sum:integer = 34;

var
	total:integer;

begin
	total := sum + c(4);
end.
