program testnestede;
{$APPTYPE CONSOLE}

function a(x,y:integer):Integer;
var
  sum:integer;

  function b(count:integer):Integer; register;
  var
    total:integer;

    function c(n:integer):integer; overload;
    begin
      result := n * 2;
    end;

  begin
    total := sum + c(count);
  end;
begin
  sum := x;
  result := b(y);
end;

begin
  writeln(a(2,3));
end.