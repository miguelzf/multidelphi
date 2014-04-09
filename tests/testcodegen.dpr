Program testcodegenints;

Type 
	i = Integer;
{
	function fff(aa: i; bb: i):i;
	Var
	  A:Integer;
	  C:^Integer;

	Begin
	  A := aa*2 / bb;
	  C := @A;
	  Result := C^ + A;
	end;
}

Var
	ttt : i;
    A,aa,bb:Integer;
//    C:^Integer;

begin
	ttt := 6;

	aa := 3;
	bb := ttt;
//	ttt := fff(3,ttt);
	
	A := aa*2 + bb;
//	C := @A;
//	A := C^ + A;
End.
