// Handle label table for simple PVM assembler
// P.D. Terry, Rhodes University, 2015

using Library;
using System.Collections.Generic;
using System.Text;

namespace Assem {

  class LabelEntry {

    public string name;
    public Label label;
    public List<int> refs = null; 

    public LabelEntry(string name, Label label, int lineNumber) {
      this.name  = name;
      this.label = label;
      this.refs = new List<int>(); 
      this.refs.Add(lineNumber); 
    } // LabelEntry constructor 

    public void AddReference (int lineNumber) {
        this.refs.Add(lineNumber); 
    } // AddReference 

  } // end LabelEntry

// -------------------------------------------------------------------------------------

  class LabelTable {

    private static List<LabelEntry> list = new List<LabelEntry>();

    public static void Insert(LabelEntry entry) {
    // Inserts entry into label table
      list.Add(entry);
    } // insert

    public static LabelEntry Find(string name) {
    // Searches table for label entry matching name.  If found then returns entry.
    // If not found, returns null
      int i = 0;
      while (i < list.Count && !name.Equals(list[i].name)) i++;
      if (i >= list.Count) return null; else return list[i];
    } // find

    public static void CheckLabels() {
    // Checks that all labels have been defined (no forward references outstanding)
      for (int i = 0; i < list.Count; i++) {
        if (!list[i].label.IsDefined())
          Parser.SemError("undefined label - " + list[i].name);
      }
    } // CheckLabels
    
    public static void printLabelTable() {
        IO.WriteLine("Labels: \n");
        foreach (LabelEntry lab  in list) {
            string def;
            if (lab.label.IsDefined() == true) def = "defined";
            else def = "undefined";
            StringBuilder addresses = new StringBuilder (); 
            if (lab.refs.Count > 0) {
            foreach (int s in lab.refs) addresses.Append(s + "\t"); 
            string toPrint = string.Format("{0}\t({1})\t{2}\t", lab.name, def, addresses.ToString() );
            IO.WriteLine(toPrint);
            } else IO.WriteLine ("No Labels Found"); 
        }
    }

    public static void ListReferences(OutFile output) {
    // Cross reference list of all labels used on output file

    } // ListReferences

  } // end LabelTable

// -------------------------------------------------------------------------------------

  class VariableEntry {

    public string name;
    public int offset;
    public List<int> lineNumbers = null; 

    public VariableEntry(string name, int offset, int lineNumber) {
      this.name    = name;
      this.offset  = offset;
      this.lineNumbers = new List<int>(); 
      this.lineNumbers.Add(lineNumber); 
    }     
 
    public void AddLineNumber(int lineNumber) {
        this.lineNumbers.Add(lineNumber); 
    } // AddOffset
  
  } // end VariableEntry

  class VarTable {

    private static List<VariableEntry> list = new List<VariableEntry>();
    private static int varOffSet = 0;

    public static int FindOffSet(string varName) {
    // Searches table for variable entry matching name.  If found then returns the known offset.
    // If not found, makes an entry and updates the master offset
      foreach (VariableEntry v in list) {
        if (v.name == varName){ 
            v.AddLineNumber(CodeGen.GetCodeLength()); 
            return v.offset; 
        }
      } 
      list.Add(new VariableEntry(varName, varOffSet, CodeGen.GetCodeLength()));
      varOffSet++;
      return varOffSet-1 ;    
    } // FindOffset

    public static void ListReferences(OutFile output) {
    // Cross reference list of all variables on output file

    } // ListReferences

    public static void printVarTable() 
    {
        IO.WriteLine("\nVariables: \n");
        
        // Check if any Variables were declared. 
        if (list.Count == 0) { IO.WriteLine("No Variables Found"); return;}

        foreach (VariableEntry va in list){
            StringBuilder offsetAddresses = new StringBuilder(); 
            foreach (int i in va.lineNumbers) offsetAddresses.Append(i + "\t");
            string toPrint = string.Format("{0}\t- offset {1}\t {2}", va.name, va.offset, offsetAddresses);
            IO.WriteLine(toPrint);
        }
   }


 

  } // end VarTable

} // end namespace
