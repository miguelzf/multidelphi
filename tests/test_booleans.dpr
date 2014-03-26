Program testbools;

var
  a,b,c,d:boolean;
  i,j:Integer;

begin
  I := 2;
  J := 5;
  a := true;
  b := (I>J);
  c := a and b;
  d := not ((a or b) xor (c));
end.