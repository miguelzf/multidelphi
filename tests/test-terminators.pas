Program test_terminators;

Var
	A,B:String;

Const
	X = 'test';
	C = 'String';

Function Str():String;
Begin
	Result := X;

	Begin
		A := 'A';
		B := Str();
		C := A+B
	End;
	Begin
		;
		;;;
		C := A+B
	End;
	Begin
		A := 'A';
		B := Str();
		C := A+B;;;
	End;
	Repeat
		A := 'A';
		B := Str();
		C := A+B;
	until B = A;
	Repeat
		A := 'A';
		B := Str();
		C := A+B
	until B = A;
	Repeat
		A := 'A';;
		B := Str();;
		C := A+B;;;
	until B = A;
	Repeat
	until B = A;
	Repeat
	;;;;
	until B = A;

End;

Var
	C:String;

Begin
	A := 'A';
	B := Str();
	C := A+B;
End.


