/*
  Compilers Prac 5 
  Task 2
  Groupe G Jefrrey Skeen, Luke des Tombe, Tom Makkink
  2018/08/16
*/

  // This is a skeleton program for developing a parser for C declarations
  // P.D. Terry, Rhodes University, 2015

  using Library;
  using System;
  using System.Text;

  class Token {
    public int kind;
    public string val;

    public Token(int kind, string val) {
      this.kind = kind;
      this.val = val;
    } // constructor

  } // Token
  

  class Declarations {

    // +++++++++++++++++++++++++ File Handling and Error handlers ++++++++++++++++++++

    static InFile input;
    static OutFile output; 
    static bool errors = false; 

    static string NewFileName(string oldFileName, string ext) {
    // Creates new file name by changing extension of oldFileName to ext
      int i = oldFileName.LastIndexOf('.');
      if (i < 0) return oldFileName + ext; else return oldFileName.Substring(0, i) + ext;
    } // NewFileName

    static void ReportError(string errorMessage) {
    // Displays errorMessage on standard output and on reflected output
      errors = true; 
      Console.WriteLine(errorMessage);
      output.WriteLine(errorMessage);
    } // ReportError

    static void Abort(string errorMessage) {
    // Abandons parsing after issuing error message
      ReportError(errorMessage);
      output.Close();
      System.Environment.Exit(1);
    } // Abort

    // +++++++++++++++++++++++  token kinds enumeration +++++++++++++++++++++++++

    const int
      noSym        =  0,
      EOFSym       =  1,
      numSym       =  2,
      identSym     =  3,
      commaSym     =  4,
      semiSym      =  5,
      intSym       =  6,
      voidSym      =  7,
      boolSym      =  8,
      charSym      =  9,
      starSym      =  10,  
      lParenSym    =  11,
      rParenSym    =  12,
      lBracketSym  =  13,
      rBracketSym  =  14,
      doubleSym    =  15,
      divideSym    =  16;
      // and others like this

    // +++++++++++++++++++++++++++++ Character Handler ++++++++++++++++++++++++++

    const char EOF = '\0';
    static bool atEndOfFile = false;

    // Declaring ch as a global variable is done for expediency - global variables
    // are not always a good thing

    static char ch;    // look ahead character for scanner

    static void GetChar() {
    // Obtains next character ch from input, or CHR(0) if EOF reached
    // Reflect ch to output
      if (atEndOfFile) ch = EOF;
      else {
        ch = input.ReadChar();
        atEndOfFile = ch == EOF;
        if (!atEndOfFile) output.Write(ch);
      }
    } // GetChar

    // +++++++++++++++++++++++++++++++ Scanner ++++++++++++++++++++++++++++++++++

    // Declaring sym as a global variable is done for expediency - global variables
    // are not always a good thing

    static Token sym;

    static void GetSym() {
    // Scans for next sym from input
      while (ch > EOF && ch <= ' ') GetChar();
      StringBuilder symLex = new StringBuilder();
      int symKind = noSym;

      if(Char.IsLetter(ch) || ch == '_'){
        do{
          symLex.Append(ch);
          GetChar();
        }while(Char.IsLetterOrDigit(ch) || ch == '_');
        switch(symLex.ToString()){
          case "int":
            symKind = intSym;
          break;
          case "char":
            symKind = charSym;
          break;
          case "bool":
            symKind = boolSym;
          break;
          case "void":
            symKind = voidSym;
          break;
          default:
            symKind = identSym;
          break;
        }

      }// gets ident token
      else if(Char.IsDigit(ch)){
        do{
          symLex.Append(ch);
          GetChar();
        }while(Char.IsDigit(ch));
        symKind = numSym;

      }// gets number token
      else{
        symLex.Append(ch);

        switch(ch){
            case EOF:
              symLex = new StringBuilder("EOF");
              symKind = EOFSym;
            break;
            case '(':
              symKind = lParenSym;
              GetChar();
            break;
            case ')':
              symKind = rParenSym;
              GetChar();
            break;
            case ',':
              symKind = commaSym;
              GetChar();
            break;
            case ';':
              symKind = semiSym;
              GetChar();
            break;
            case '[':
              symKind = lBracketSym;
              GetChar();
            break;
            case ']':
              symKind = rBracketSym;
              GetChar();
            break;
            case '*':
              symKind = starSym;
              GetChar();
              if( ch == '/'){
                symLex.Append(ch);
                symKind = noSym;
                GetChar();
              }
            break;
            case '/':
              GetChar();
              if (ch == '/') {
                do {
                  GetChar();
                } while (ch != '\n' && ch != EOF);
                if (ch == EOF) {
                    symLex = new StringBuilder("EOF");
                    symKind = EOFSym;
                }
               else{
                GetSym();                 
                return;
               }
              } else if (ch == '*'){
                GetChar();
                bool isEndOfComment = false; 
                do 
                {
                  if (ch == '*') {
                    GetChar();
                    if (ch == '/') isEndOfComment = true;                     
                  } 
                  GetChar();
                } while (!isEndOfComment && ch != EOF);
                if (ch == EOF) {
                    symLex = new StringBuilder("EOF");
                    symKind = EOFSym;
                }
               else{
                GetSym();                 
                return;
               }
              } else{
                symKind = divideSym;
              }
            break;
            default:
              symKind = noSym;
              GetChar();
            break;
        }// switch
      }//else

      // over to you!

      sym = new Token(symKind, symLex.ToString());
    } // GetSym

   //*  ++++ Commented out for the moment

    // +++++++++++++++++++++++++++++++ Parser +++++++++++++++++++++++++++++++++++
	
	// FIRST SETS OF ALL PRODUCTION RULES 
  static IntSet 
    FirstCdecls 	  	= new IntSet(boolSym, charSym, voidSym, intSym, EOFSym),
    FirstDecList      = new IntSet(boolSym, charSym, voidSym, intSym),
    FirstType         = FirstDecList,
    FirstOneDecl      = new IntSet(starSym, identSym, lParenSym),
    FirstDirect       = new IntSet(identSym, lParenSym),
    FirstSuffix   		= new IntSet(lBracketSym, lParenSym),
    FirstParams       = new IntSet(lParenSym),
    FirstOneParam     = FirstType,
    FirstArray   	  	= new IntSet(lBracketSym);

	
	// FOLLOW SETS OF ALL PRODUCTION RULES 	
  static IntSet 
    FollowDecList     = FirstDecList.Union(new IntSet(EOFSym)),
    FollowType        = FirstOneDecl,
    FollowOneDecl     = new IntSet (commaSym, semiSym, starSym, rParenSym),   // check this one 
    FollowOneParam    = new IntSet (commaSym, rParenSym),
    FollowArray       = FirstArray;
 

  static void Test(IntSet allowed, IntSet beacons, string errorMessage) {
    if (allowed.Contains(sym.kind)) return; 
    IntSet stopSet = allowed.Union(beacons);
    while (!stopSet.Contains(sym.kind)) GetSym(); 
  } // Test 
	
  static void Accept(int wantedSym, string errorMessage) {
  // Checks that lookahead token is wantedSym
    if (sym.kind == wantedSym) GetSym(); else ReportError(errorMessage);
  } // Accept

  static void Accept(IntSet allowedSet, string errorMessage) {
  // Checks that lookahead token is in allowedSet
    if (allowedSet.Contains(sym.kind)) GetSym(); else ReportError(errorMessage);
  } // Accept

  static void CDecls(IntSet followers) {
    // Cdecls = { DecList } EOF .
    if (FirstCdecls.Contains(sym.kind))
    {
      while (FirstDecList.Contains(sym.kind)) DecList(followers.Union(FollowDecList));
      Accept(EOFSym, "EOF expected");
    }
    else ReportError("Error occured in Production Rule: CDecls");
  } // CDecls 

  static void DecList(IntSet followers){
    // DecList = Type OneDecl { "," OneDecl } ";" .
    Test(FirstDecList, followers, "Invalid start to DecList");
    
    if (FirstDecList.Contains(sym.kind)) {
      Type(followers.Union(FollowType));
      OneDecl(followers.Union(FollowOneDecl));
      
      while(sym.kind == commaSym){
        Accept(commaSym, ", expected");
        OneDecl(followers.Union(FollowOneDecl));
      }

      Accept(semiSym, "; expected");
      Test(followers, new IntSet (), "Invalid start after Declist");
    }  
    else ReportError("Invalid Type in Production Rule: DecList.");
  }

  static void Type(IntSet followers){
    //Type = "int" | "void" | "bool" | "char" .
    Accept(FirstType, "Invalid Type");
  }

  static void OneDecl(IntSet followers){
    // OneDecl = "*" OneDecl | Direct .
    if (FirstOneDecl.Contains(sym.kind)){
      if (sym.kind == starSym)
      { 
        GetSym();                           //Accept(starSym, "* expected");
        OneDecl(followers.Union(FollowOneDecl));
      }	
      else Direct(followers);
    }
    else ReportError("Error in Production rule: OneDecl ");
  }

  static void Direct(IntSet followers){
    // Direct = ( ident | "(" OneDecl ")" ) [ Suffix ] .
    Test(FirstDirect, followers, "Invalid start to Direct");
    if (FirstDirect.Contains(sym.kind)) 
    {
      if (sym.kind == identSym) GetSym();    // Accept(identSym, "identifier expected");
      else {
        Accept(lParenSym, "( expected");
        OneDecl(followers.Union(FollowOneDecl));
        Accept(rParenSym, ") expected");
      }
      if (sym.kind == lBracketSym || sym.kind == lParenSym) Suffix(followers);
      Test(followers, new IntSet (), "Invalid sym after Direct");
    } 
    else ReportError("Error in Production Rule: Direct"); 
  } // Direct

  static void Suffix(IntSet followers){
    // Suffix = Array { Array } | Params .
    if (FirstSuffix.Contains(sym.kind))
    {
      if (sym.kind == lBracketSym)
      {
        do {
          Array(followers.Union(FollowArray));
        } while (sym.kind == lBracketSym);
      }
      else Params(followers);
    }
      else ReportError("Error in Production Rule: Suffix"); 
  } // suffix

  static void Params(IntSet followers){
    //Params = "(" [ OneParam { "," OneParam } ] ")" .
    if (FirstParams.Contains(sym.kind))
    {
      Accept(lParenSym,"( expected.");
      if (sym.kind == intSym || sym.kind == voidSym || sym.kind == boolSym || sym.kind == charSym){
      OneParam(followers.Union(FollowOneParam));
      while (sym.kind == commaSym){
        GetSym();                         // Accept(commaSym, ", expected");
        OneParam(followers.Union(FollowOneParam));
        }
      }
      Accept(rParenSym, ") expected 2");
    }
    else ReportError("Error in Production Rule: Params"); 
  } // Params
	
	
  static void OneParam(IntSet followers){
    //OneParam = Type [ OneDecl ] .
    if (FirstOneParam.Contains(sym.kind))
    {
        Type(followers);
        if(starSym == sym.kind || sym.kind == identSym || sym.kind == lParenSym)
        {
          OneDecl(followers.Union(FollowOneDecl));
        }
    }
      else ReportError("Error in Production Rule: OneParam"); 
  }// OneParam  
	
	
  static void Array(IntSet followers)
  {
    //Array = "[" [ number ] "]" .
    if (FirstArray.Contains(sym.kind))
    {
      Accept(lBracketSym,"[ expected");
      if (numSym == sym.kind){
        Accept(numSym, "number expected");
      }
      Accept(rBracketSym, "] expected");
    }
    else ReportError("Error in Production Rule: Array"); 
  }
  //++++++ */

    // +++++++++++++++++++++ Main driver function +++++++++++++++++++++++++++++++

    public static void Main(string[] args) {
      // Open input and output files from command line arguments
      if (args.Length == 0) {
        Console.WriteLine("Usage: Declarations FileName");
        System.Environment.Exit(1);
      }
      input = new InFile(args[0]);
      output = new OutFile(NewFileName(args[0], ".out"));

      GetChar();                                  // Lookahead character

  //  To test the scanner we can use a loop like the following:
/*
      do {
        GetSym();                                 // Lookahead symbol
        OutFile.StdOut.Write(sym.kind, 3);
        OutFile.StdOut.WriteLine(" " + sym.val);  // See what we got
      } while (sym.kind != EOFSym);
*/
  //*  After the scanner is debugged we shall substitute this code:

      GetSym();                                   // Lookahead symbol
      CDecls(new IntSet());                                   // Start to parse from the goal symbol
      // if we get back here everything must have been satisfactory
      if (!errors) Console.WriteLine("Parsed correctly");

  //*/
      output.Close();
    } // Main

  } // Declarations

