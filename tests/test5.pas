Program test5;

{$DEFINE BOO}
{$DEFINE FOO}

Var
{$IFDEF BOO}
  X:Integer = 1;
  {$IFDEF FOO}
  Y:Integer = 2;
  {$ENDIF}
{$ELSE}
  X:Integer = 5;
{$ENDIF}

Begin
  Inc(X);
End.
