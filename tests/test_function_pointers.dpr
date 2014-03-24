program testfp;

{$APPTYPE CONSOLE}
var
  fp:function(x:Single):Single;
  p, p2:procedure;

function bla(x:single):single;
begin
  result := cos(x);
end;

procedure lol;
begin
  writeln('lol');
end;

begin
  fp := @bla;
  fp(2);
  p := lol;
  p2 := @lol;
  p;
end.