Program test1;

Var
  A,B:String;

Const
  X = 'test';

Function Str():String;
Begin
  Result := X;
End;


Function QuaternionSlerp(A,B:Quaternion; Const T:Single):Quaternion;
Var
  Theta, Sine, Beta, Alpha:Single;
  Cosine:Single;
Begin
  Cosine := a.x*b.x + a.y*b.y + a.z*b.z + a.w*b.w;
  Cosine := Abs(Cosine);

  If ((1-cosine)>Epsilon) Then
  Begin
    Theta := ArcCos(cosine);
  	Sine := Sin(theta);

  	Beta := Sin((1-t)*theta) / sine;
  	Alpha := Sin(t*theta) / sine;
  End
 Else
  Begin
    Beta := (1.0 - T);
    Alpha := T;
  End;

  Result.X := A.X * Beta + B.X * Alpha;
  Result.Y := A.Y * Beta + B.Y * Alpha;
  Result.Z := A.Z * Beta + B.Z * Alpha;
  Result.W := A.W * Beta + B.W * Alpha;
End;


Var
  C:String;

Begin
  A := 'A';

  If ((1-cosine)>Epsilon) Then
  Begin
    Theta := ArcCos(cosine);
  	Sine := Sin(theta);

  	Beta := Sin((1-t)*theta) / sine;
  	Alpha := Sin(t*theta) / sine;
  End
 Else
  Begin
    Beta := (1.0 - T);
    Alpha := T;
  End;

  B := Str();
  C := (A.a+B.a);


End.
