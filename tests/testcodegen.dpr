Program testcodegenints;

Type 
	i = Integer;

	function fff(aa: i; bb: i):i;
	Var
	  A:Integer;
	  C:^Integer;

	Begin
	  A := aa; //*2 / bb;
	  C := @A;
	  Result := C^ + A;
	end;


	
	procedure ppp(aa: i; bb: i);
	Var
	  A:Integer;
	  C:^Integer;

	Begin
	  A := aa; //*2 / bb;
	  C := @A;
	end;
Var

	ttt : i;
    a,b,c,aa:Integer;
    par :^Integer;
	Ret : Integer;
	ii : Integer;
	
Label lblrep;

begin
	ttt := 6;
	a := 1;
	b := ttt;
	aa := 50;

	if (a < b)
	then
		begin
			c := a;
			b := c * 3+a;
		end
	else
		begin
			a := b;
			c := a / 3 + ttt;
		end ;
		
	for ii := 34 downto 22 do
		begin
			a := ret shr 1 + (b shl 1);
			b := ret / (b+1) + a;
			if (a mod b = 1) then 
				goto lblrep;
		end;

	repeat
		begin
			aa := aa - 1 ;
			if (aa mod 3 = 1) 
			then 
				aa := aa + b
			else
				aa := aa - b
		end;
	until aa < 70;
	
	
	while ( aa < 100) do
		begin
			aa := aa + 1 ;
			if (a mod b*2 = 12) then 
				break;
			if (a / b = 7) then 
				continue;
		end;

	ttt := fff(1,ttt);
	
lblrep:
	ppp(200/a*11,ttt);
	
	A := a*2 + b;

	par := @A;
	
	c := par^;
	Ret := C shr 1;
End.
