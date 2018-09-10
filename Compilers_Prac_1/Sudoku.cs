// Sudoku 
// Compilers Practical 1 Tom, Luke, Jeff 
using Library;
using System;
using System.Collections.Generic;


class Sudoko
{
    // Prints out the standard board, which is updated when users input a value
    public static void writeBoard(List<List<int>> board)
    {
        IO.Write("  \t 0   1   2   3   4   5   6   7   8\n");
        IO.Write("-----------------------------------------------\n");
        for (int r = 0; r < 9; r++)
        {
            IO.Write(r + ":\t");
            for (int c = 0; c < 9; c++)
            {
                if(board[r][c] == 0){
                    IO.Write("... ");
                }
                else{
                    IO.Write(" "+board[r][c] + "  ");
                }

            }
            IO.Write("\n");
        }
    }

    // Determines whether the game is over or not
    public static bool isGameOver(List<List<int>> board)
    {
        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                if (board[i][j] == 0) return false;
            }
        }
        return true;
    }

    // Get move from user
    public static string [] getInputFromUser()
    {
        IO.WriteLine("Please enter a row, column and value. e.g. 0 2 5\n");
        // Problem with the readLine 
        string s = IO.ReadLine();
        return s.Split(' ');
    }

    // Check that the user input is valid
    public static bool validateInput(string [] userInput)
    {
        // row columns value 
        if (userInput.Length == 3)
        {
            // Check that the row and column values are valid 
            if (Convert.ToInt32(userInput[0]) >= 0 && Convert.ToInt32(userInput[0]) < 9 && Convert.ToInt32(userInput[1]) >= 0 && Convert.ToInt32(userInput[1]) < 9)
            {
                // Check that the value is valid
                if (Convert.ToInt32(userInput[2]) > 0 && Convert.ToInt32(userInput[2]) < 10)
                {
                    return true;
                }
            }
        }
        return false;
    }

    // Converts string list to int list
    public static List<int> convertToIntList(string [] userInput)
    {
        // We know it is a 3 element input, as we have checked it
        List<int> userMove = new List<int> { };
        for (int i = 0; i < 3; i++)
        {
            userMove.Add(Convert.ToInt32(userInput[i]));
        }
        return userMove;
    }

    // Create a standard board 
    public static List<List<int>> createBoard(InFile data)
    {
        // We know it is a 3 element input, as we have checked it
        List<List<int>> tempBoard = new List<List<int>> {};
        for (int i =0; i < 9; i++)
        {
            List<int> te = new List<int> {};
            for (int j =0; j < 9;j++)
            {
                int t = data.ReadInt();
                te.Add(t);

            }
            tempBoard.Add(te);
        }
        return tempBoard;
    }

    // Compares each IntSet and applies a Xor
    public static IntSet filter (List<IntSet> row, IntSet cell)
    {
        for(int i = 0; i < 9; i++)
        {
            if (row[i].Members()==1)
            {
                cell = cell.Xor(row[i]);
            }
        }
        return cell;
    }    
    
    // Checks what values are available in the rows 
    public static List<List<IntSet>> FilterRows (List<List<IntSet>> board)
    {
        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                if (board[i][j].Members() != 1)
                {
                    board[i][j] = filter(board[i], board[i][j]);
                }
            }
        }
        return board;
    }

    // Checks what values are available in the columns 
    public static List<List<IntSet>> FilterCols (List<List<IntSet>> board)
    {
        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                List<IntSet> col = new List<IntSet> {};
                for (int k = 0; k < 9; k++)
                {
                    col.Add(board[k][i]);
                }
                if (board[i][j].Members() != 1)
                {
                    board[j][i] = filter(col, board[j][i]);
                }      
            }
        }
        return board;
    }

    // Places all squares into a list of rows - meant for filtering squares, but not complete
    public static List<List<IntSet>> FilterSquares (List<List<IntSet>> board)
    {
        List<List<IntSet>> squareList = new List<List<IntSet>> {
            new List<IntSet> {board[0][0], board[0][1], board[0][2], board[1][0], board[1][1], board[1][2], board[2][0], board[2][1], board[2][2]},
            new List<IntSet> {board[0][3], board[0][4], board[0][5], board[1][3], board[1][4], board[1][5], board[2][3], board[2][4], board[2][5]},
            new List<IntSet> {board[0][6], board[0][7], board[0][8], board[1][6], board[1][7], board[1][8], board[2][6], board[2][7], board[2][8]},
            new List<IntSet> {board[3][0], board[3][1], board[3][2], board[4][0], board[4][1], board[4][2], board[5][0], board[5][1], board[5][2]},
            new List<IntSet> {board[3][3], board[3][4], board[3][5], board[4][3], board[4][4], board[4][5], board[5][3], board[5][4], board[5][5]},
            new List<IntSet> {board[3][6], board[3][7], board[3][8], board[4][6], board[4][7], board[4][8], board[5][6], board[5][7], board[5][8]},
            new List<IntSet> {board[6][0], board[6][1], board[6][2], board[7][0], board[7][1], board[7][2], board[8][0], board[8][1], board[8][2]},
            new List<IntSet> {board[6][3], board[6][4], board[6][5], board[7][3], board[7][4], board[7][5], board[8][3], board[8][4], board[8][5]},
            new List<IntSet> {board[6][6], board[6][7], board[6][8], board[7][6], board[7][7], board[7][8], board[8][6], board[8][7], board[8][8]}
        };

        return squareList;
    }

    // Creates a board with IntSets in each element
    public static List<List<IntSet>> createPredicativeBoard(List<List<int>> board)
    {
        List<List<IntSet>> predictionBoard = new List<List<IntSet>> {};
        IntSet temp = new IntSet (1, 2, 3, 4, 5, 6, 7, 8, 9);
        for (int i = 0; i < 9; i++)
        {
            List<IntSet> te = new List<IntSet> {};
            for (int j = 0; j < 9; j++)
            {
                    
                if (board[i][j] != 0)
                {
                    te.Add(new IntSet (board[i][j]));
                }
                else 
                {
                    te.Add(temp);
                }
            }
            predictionBoard.Add(te);
        }

        return predictionBoard;
    }

    // Writes out the predictive board
    public static void writePredictionBoard(List<List<IntSet>> board)
    {
        for (int i = 0; i < 9; i++)
        {
            IO.WriteLine("");
            for (int j = 0; j < 9; j++)
            {
                IO.Write(board[i][j] + " ");
            }
        }
    }


    public static void Main(string[] args)
    {
        Screen.ClrScr();

        InFile data = new InFile(args[0]);
        if (data.OpenError())
        {
            Console.WriteLine("cannot open " + args[0]);
            System.Environment.Exit(1);
        }

        List<List<int>> Board = createBoard(data);

        List<List<IntSet>> predictionBoard = createPredicativeBoard(Board);
        predictionBoard = FilterRows(predictionBoard);
        predictionBoard = FilterCols(predictionBoard);

        IO.Write('\n');
        IO.WriteLine("Please may you about to begin a new Game of Sudoku.");
        //writeBoard();
        IO.WriteLine("Please may you you either type in (S)tart or (Q)uite");
        char T= IO.ReadChar();
        if (T=='S' || T == 's')
        {
            writeBoard(Board);
            string trash = IO.ReadLine();
            while (!isGameOver(Board))
            {
                // Get input from user 
                string [] userInput = getInputFromUser();
                while (!validateInput(userInput))
                {
                    IO.WriteLine("Invalid Input.");
                    userInput = getInputFromUser();
                }
                // List of valid input 
                List<int> userMove = convertToIntList(userInput);
                Board[userMove[0]][userMove[1]] = userMove[2];
                writeBoard(Board);
            
            }
        }
        if(T=='q' || T == 'Q') { Screen.ClrScr(); System.Environment.Exit(1);}
    }

}
