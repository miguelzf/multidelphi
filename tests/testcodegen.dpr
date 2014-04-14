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
    a,b,c:Integer;
    par :^Integer;
	Ret : Integer;

begin
	ttt := 6;

	a := 1;
	b := ttt;
	
	ttt := fff(1,ttt);
	
	ppp(200/a*11,ttt);
	
	A := a*2 + b;

	par := @A;
	
	c := par^;
	Ret := C shr 1;
End.

def test(x)
  printd(x) :
  x = 4 :
  printd(x);
  
def f(x y z) x = 11 : y = 35 : z = x / y ;

def binary : 1 (x y) y;

def fib(x)
  if (x < 3) then
    1
  else
    fib(x-1)+fib(x-2);

def fibi(x)
  var a = 1, b = 1, c in
  (for i = 3, i < x in
     c = a + b :
     a = b :
     b = c) :
  b;
