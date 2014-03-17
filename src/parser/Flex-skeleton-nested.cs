	//********************************************************************************************
	// CSFlex nested templated extended with some missing Flex features
	//		Changed to C# only, removed java 
	//********************************************************************************************

	// Pre-action default user-defined general action. Empty by default;
	Action PRE_ACTION_DEFAULT = () => { };

	// Control Debug messages
	public int yyLexDebugLevel = 0;

	/** This character denotes the end of file */
	public const int YYEOF = 0;
  
	// Avoid repeated string creations in yytext. Must reset text to null before each rule's action.
	string _text;
	string text {
		get { 
			if (_text == null)
				_text = yytext();
			return _text;
		}
		set { _text = value; }
	}
	
	
	// **********************************************
	// Make CSFlex compatible with FLex:
	
	const int INITIAL = YYINITIAL;
	public Object yylval;	// value of last token
	
	// Finish scanning
	public int Accept()
	{
		if (!zzEOFDone) {
			zzEOFDone = true;
			yyclose();
		}
		return YYEOF;
	}


	// **********************************************
	// Message helpers
		
	void yyerror(String msg = "Unknown lexical error")
	{
	//	throw new ScannerException(yyline, msg);
		Console.Error.WriteLine("[ERROR Flex] " + msg + " in line " + yyline);
	}

	void yyprint(String msg)
	{
		Console.Out.WriteLine("[Line " + yyline + "] " + msg);
	}
		
	void yydebug(String msg)
	{
		if (this.yyLexDebugLevel >= 1)
			Console.Out.WriteLine("[Line " + yyline + "] " + msg);
	}

	void yydebugall(String msg)
	{
		if (this.yyLexDebugLevel >= 2)
			Console.Out.WriteLine("[Line " + yyline + "] " + msg);
	}


	// **********************************************
	// Implementation of yacc's state stack

	Stack<int> stateStack = new Stack<int>();
	
	void yypushstate(int state)
	{
		yydebugall("PUSH STATE: from " + stateName(stateStack.Peek()) + " to " +  stateName(state) );
		stateStack.Push(state);
		yybegin(state);
	}

	void yypopstate()
	{
		int state = stateStack.Pop();
		yydebugall("POP STATE: from " + stateName(state)
				+ " to " +  stateName(stateStack.Peek()));
		yybegin(stateStack.Peek());
	}

	void switchstate(int state)
	{
		stateStack.Pop();
		stateStack.Push(state);
	}

	int yylaststate()
	{
		if (stateStack.Count > 1)
			return stateStack.ElementAt<int>(stateStack.Count-2);
		else
			return -1;
	}
	
	string[] stateNames;
	string stateName(int state)
	{
		if (state < stateNames.Count())
			return stateNames[state];
		else
			return "UnknownState";
	}

	// **********************************************
	
	
  /** initial size of the lookahead buffer */
--- private static final int ZZ_BUFFERSIZE = ...;

  /** lexical states */
---  lexical states, charmap

  /* error codes */
  private const int ZZ_UNKNOWN_ERROR = 0;
  private const int ZZ_NO_MATCH = 1;
  private const int ZZ_PUSHBACK_2BIG = 2;

  /* error messages for the codes above */
  private static readonly String[] ZZ_ERROR_MSG = new string[] {
    "Unkown internal scanner error",
    "Error: could not match input",
    "Error: pushback value was too large"
  };

--- isFinal list
  /** the input device */
  private System.IO.TextReader zzReader;

  /** the current state of the DFA */
  private int zzState;

  /** the current lexical state */
  private int zzLexicalState = YYINITIAL;

  /** this buffer contains the current text to be matched and is
      the source of the yytext() string */
  private char[] zzBuffer = new char[ZZ_BUFFERSIZE];

  /** the textposition at the last accepting state */
  private int zzMarkedPos;

  /** the textposition at the last state to be included in yytext */
  private int zzPushbackPos;

  /** the current text position in the buffer */
  private int zzCurrentPos;

  /** startRead marks the beginning of the yytext() string in the buffer */
  private int zzStartRead;

  /** endRead marks the last character in the buffer, that has been read
      from input */
  private int zzEndRead;

  /** number of newlines encountered up to the start of the matched text */
  private int yyline;

  /** the number of characters up to the start of the matched text */
  private int yychar;

  /**
   * the number of characters from the last newline up to the start of the 
   * matched text
   */
  private int yycolumn;

  /** 
   * zzAtBOL == true <=> the scanner is currently at the beginning of a line
   */
  private bool zzAtBOL = true;

  /** zzAtEOF == true <=> the scanner is at the EOF */
  private bool zzAtEOF;


  /** the stack of open (nested) input streams to read from */
  private System.Collections.Stack zzStreams = new System.Collections.Stack();

  /**
   * inner class used to store info for nested
   * input streams
   */
  private sealed class ZzFlexStreamInfo {
    public System.IO.TextReader zzReader;
    public int zzEndRead;
    public int zzStartRead;
    public int zzCurrentPos;
    public int zzMarkedPos;
    public int zzPushbackPos;
    public int yyline;
    public int yycolumn;
    public char [] zzBuffer;
    public bool zzAtEOF;

    /** sets all values stored in this class */
    public ZzFlexStreamInfo(System.IO.TextReader zzReader, int zzEndRead, int zzStartRead,
                  int zzCurrentPos, int zzMarkedPos, int zzPushbackPos,
                  char [] zzBuffer, bool zzAtEOF, int yyline, int yycolumn) {
      this.zzReader      = zzReader;
      this.zzEndRead     = zzEndRead;
      this.zzStartRead   = zzStartRead;
      this.zzCurrentPos  = zzCurrentPos;
      this.zzMarkedPos   = zzMarkedPos;
      this.zzPushbackPos = zzPushbackPos;
      this.zzBuffer      = zzBuffer;
      this.zzAtEOF       = zzAtEOF;
      this.yyline         = yyline;
      this.yycolumn       = yycolumn;
    }
  }

--- user class code

  /**
   * Creates a new scanner
   * There is also a System.IO.Stream version of this constructor.
   *
   * @param   in  the System.IO.TextReader to read input from.
   */
--- constructor declaration

  /**
   * Refills the input buffer.
   *
   * @return      <code>false</code>, iff there was new input.
   * 
   * @exception   System.IO.IOException  if any I/O-Error occurs
   */
  private bool zzRefill() {

    /* first: make room (if you can) */
    if (zzStartRead > 0) {
      Array.Copy(zzBuffer, zzStartRead,
                 zzBuffer, 0,
                 zzEndRead-zzStartRead);

      /* translate stored positions */
      zzEndRead-= zzStartRead;
      zzCurrentPos-= zzStartRead;
      zzMarkedPos-= zzStartRead;
      zzPushbackPos-= zzStartRead;
      zzStartRead = 0;
    }

    /* is the buffer big enough? */
    if (zzCurrentPos >= zzBuffer.Length) {
      /* if not: blow it up */
      char[] newBuffer = new char[zzCurrentPos*2];
      Array.Copy(zzBuffer, 0, newBuffer, 0, zzBuffer.Length);
      zzBuffer = newBuffer;
    }

    /* finally: fill the buffer with new input */
    int numRead = zzReader.Read(zzBuffer, zzEndRead,
                                            zzBuffer.Length-zzEndRead);

    if (numRead <= 0) {
      return true;
    }
    else {
      zzEndRead+= numRead;
      return false;
    }
  }

    
  /**
   * Closes the input stream.
   */
  public void yyclose() {
    zzAtEOF = true;            /* indicate end of file */
    zzEndRead = zzStartRead;  /* invalidate buffer    */

    if (zzReader != null)
      zzReader.Close();
	if (zzAtBOL) { }
  }


  /**
   * Stores the current input stream on a stack, and
   * reads from a new stream. Lexical state, line,
   * char, and column counting remain untouched.
   *
   * The current input stream can be restored with
   * yypopstream (usually in an <<EOF>> action).
   *
   * @param reader the new input stream to read from
   *
   * @see #yypopStream()
   */
  public void yypushStream(TextReader reader) {
    zzStreams.Push(
      new ZzFlexStreamInfo(zzReader, zzEndRead, zzStartRead, zzCurrentPos,
                        zzMarkedPos, zzPushbackPos, zzBuffer, zzAtEOF,
                        yyline, yycolumn)
    );
    zzAtEOF  = false;
    zzBuffer = new char[ZZ_BUFFERSIZE];
    zzReader = reader;
    zzEndRead = zzStartRead = 0;
    zzCurrentPos = zzMarkedPos = zzPushbackPos = 0;
    yyline = yycolumn = 1;
  }
    

  /**
   * Closes the current input stream and continues to
   * read from the one on top of the stream stack. 
   *
   * @throws System.InvalidOperationException
   *         if there is no further stream to read from.
   *
   * @throws System.IO.IOException
   *         if there was an error in closing the stream.
   *
   * @see #yypushStream(TextReader)
   */
  public void yypopStream() {
    zzReader.Close();
    ZzFlexStreamInfo s = (ZzFlexStreamInfo) zzStreams.Pop();
    zzBuffer      = s.zzBuffer;
    zzReader      = s.zzReader;
    zzEndRead     = s.zzEndRead;
    zzStartRead   = s.zzStartRead;
    zzCurrentPos  = s.zzCurrentPos;
    zzMarkedPos   = s.zzMarkedPos ;
    zzPushbackPos = s.zzPushbackPos;
    zzAtEOF       = s.zzAtEOF;
    yyline         = s.yyline;
    yycolumn       = s.yycolumn;
  }


  /**
   * Returns true iff there are still streams left 
   * to read from on the stream stack.
   */
  public bool yymoreStreams() {
    return zzStreams.Count != 0;
  }


  /**
   * Resets the scanner to read from a new input stream.
   * Does not close the old reader.
   *
   * All internal variables are reset, the old input stream 
   * <b>cannot</b> be reused (internal buffer is discarded and lost).
   * Lexical state is set to <tt>ZZ_INITIAL</tt>.
   *
   * @param reader   the new input stream 
   *
   * @see #yypushStream(System.IO.TextReader)
   * @see #yypopStream()
   */
  public void yyreset(System.IO.TextReader reader) {
    zzReader = reader;
    zzAtBOL  = true;
    zzAtEOF  = false;
    zzEndRead = zzStartRead = 0;
    zzCurrentPos = zzMarkedPos = zzPushbackPos = 0;
    yyline = yychar = yycolumn = 1;
    zzLexicalState = YYINITIAL;
  }


  /**
   * Returns the current lexical state.
   */
  public int yystate() {
    return zzLexicalState;
  }


  /**
   * Enters a new lexical state
   *
   * @param newState the new lexical state
   */
  public void yybegin(int newState) {
    zzLexicalState = newState;
  }


  /**
   * Returns the text matched by the current regular expression.
   */
  public String yytext() {
    return new String( zzBuffer, zzStartRead, zzMarkedPos-zzStartRead );
  }


  /**
   * Returns the character at position <tt>pos</tt> from the 
   * matched text. 
   * 
   * It is equivalent to yytext().charAt(pos), but faster
   *
   * @param pos the position of the character to fetch. 
   *            A value from 0 to yylength()-1.
   *
   * @return the character at position pos
   */
  public char yycharat(int pos) {
    return zzBuffer[zzStartRead+pos];
  }


  /**
   * Returns the length of the matched text region.
   */
  public int yylength() {
    return zzMarkedPos-zzStartRead;
  }


  /**
   * Reports an error that occured while scanning.
   *
   * In a wellformed scanner (no or only correct usage of 
   * yypushback(int) and a match-all fallback rule) this method 
   * will only be called with things that "Can't Possibly Happen".
   * If this method is called, something is seriously wrong
   * (e.g. a JFlex/CSFlex bug producing a faulty scanner etc.).
   *
   * Usual syntax/scanner level error handling should be done
   * in error fallback rules.
   *
   * @param   errorCode  the code of the errormessage to display
   */
--- zzScanError declaration
    String message;
    try {
      message = ZZ_ERROR_MSG[errorCode];
    }
    catch (IndexOutOfRangeException) {
      message = ZZ_ERROR_MSG[ZZ_UNKNOWN_ERROR];
    }

--- throws clause
  } 


  /**
   * Pushes the specified amount of characters back into the input stream.
   *
   * They will be read again by then next call of the scanning method
   *
   * @param number  the number of characters to be read again.
   *                This number must not be greater than yylength()!
   */
--- yypushback decl (contains zzScanError exception)
    if ( number > yylength() )
      zzScanError(ZZ_PUSHBACK_2BIG);

    zzMarkedPos -= number;
  }


--- zzDoEOF
  /**
   * Resumes scanning until the next regular expression is matched,
   * the end of input is encountered or an I/O-Error occurs.
   *
   * @return      the next token
   * @exception   System.IO.IOException  if any I/O-Error occurs
   */
--- yylex declaration
    int zzInput = 0;
    int zzAction;

--- local declarations

    while (true) {
      // cached fields:
      int zzCurrentPosL;
      int zzMarkedPosL = zzMarkedPos;
      int zzEndReadL = zzEndRead;
      char [] zzBufferL = zzBuffer;
      char [] zzCMapL = ZZ_CMAP;

--- start admin (line, char, col count)
      zzAction = -1;

      zzCurrentPosL = zzCurrentPos = zzStartRead = zzMarkedPosL;
  
--- start admin (lexstate etc)

        while (true) {
          goto zzForAction_skip;
        zzForAction: break;
        zzForAction_skip:
          if (!ZZ_SPURIOUS_WARNINGS_SUCK) goto zzForAction;
    
--- next input, line, col, char count, next transition, isFinal action
            zzAction = zzState;
            zzMarkedPosL = zzCurrentPosL;
--- line count update
          }

        }

      // store back cached position
      zzMarkedPos = zzMarkedPosL;
  	  PRE_ACTION_DEFAULT();
	  
--- char count update

--- actions
        default: 
          if (zzInput == YYEOF && zzStartRead == zzCurrentPos) {
            zzAtEOF = true;
--- eofvalue
          } 
          else {
--- no match
          }
          break;
      }
    }
  }

--- main

}
