Program testarrays;

type
  point = record
    x:Integer;
    y:Integer;
  end;

var
  v:array[0..10] of point;
  m:array[0..5, 0..5] of point;
  i:integer;

function lol(x:Integer):integer;
begin
  result := x+ 2;
end;


function makepoint(x,y:Integer):point;
begin
  result.x := x;
  result.y := y;
end;

begin
  for I:=0 to 5 do
    v[i] := makepoint(1,1);

  m[0,2].x := lol(2);
end.