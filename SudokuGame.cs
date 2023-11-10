using System;
using System.Diagnostics;
using System.Threading;

class Sudoku
{
    static int[,] board = new int[9, 9]; // Represents the current state of the Sudoku grid.
    static int[,] initialBoard = new int[9, 9]; // Stores the initial puzzle configuration.
    static int numbersToSolve = 9; // Set the number of cells the user needs to fill.
    static Stopwatch stopwatch = new Stopwatch(); // Used to measure time taken to solve the puzzle.

    static void Main(string[] args)
    {
        InitializeBoard(); // Initialize the Sudoku puzzle.
        PrintBoard(); // Display the initial puzzle.
        StartTimer(); // Start the timer to track solving time.

        while (numbersToSolve > 0) // Main game loop - continue until all cells are filled.
        {
            Console.Write("Enter row (1-9) and column (1-9) to input a number (e.g., '5 2'): ");
            string input = Console.ReadLine();

            if (TryParseInput(input, out int row, out int col))
            {
                if (initialBoard[row - 1, col - 1] == 0) // Check if the cell is not pre-filled.
                {
                    Console.Write("Enter the number (1-9): ");
                    if (int.TryParse(Console.ReadLine(), out int num) && num >= 1 && num <= 9)
                    {
                        if (IsSafeToPlace(row - 1, col - 1, num)) // Check if it's safe to place the number.
                        {
                            board[row - 1, col - 1] = num; // Place the number in the cell.
                            PrintBoard(); // Display the updated puzzle.
                            numbersToSolve--; // Decrement the count of cells to fill.

                            if (IsSolved()) // Check if the puzzle is solved.
                            {
                                StopTimer(); // Stop the timer.
                                Console.WriteLine("Congratulations! You solved the puzzle.");
                                break; // Exit the game loop.
                            }
                        }
                        else
                        {
                            Console.WriteLine("Invalid move. Try again.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid number. Please enter a number between 1 and 9.");
                    }
                }
                else
                {
                    Console.WriteLine("This cell is pre-filled. Please select another cell.");
                }
            }
            else
            {
                Console.WriteLine("Invalid input. Please enter row and column (e.g., '53 2').");
            }
        }
    }

    static void InitializeBoard()
    {
        // Set your initial Sudoku puzzle here with more pre-filled numbers.
        initialBoard = new int[,]
        {
            {5, 3, 4, 6, 7, 8, 9, 1, 2},
            {6, 7, 2, 1, 9, 5, 3, 0, 8},
            {1, 0, 8, 3, 4, 2, 5, 6, 7},
            {8, 5, 9, 7, 6, 1, 4, 2, 3},
            {4, 2, 6, 0, 5, 3, 7, 9, 1},
            {7, 1, 3, 9, 2, 4, 8, 0, 6},
            {9, 0, 1, 5, 3, 7, 2, 8, 4},
            {2, 8, 7, 4, 1, 9, 0, 3, 5},
            {3, 4, 5, 2, 8, 6, 1, 7, 9}
        };

        // Copy the initial puzzle to the board.
        Array.Copy(initialBoard, board, initialBoard.Length);
    }

   // This function parses the user's input to extract the row and column for placing a number.
// It checks if the input is in the correct format (e.g., '5 2') and if the row and column are within valid Sudoku coordinates (1-9).
static bool TryParseInput(string input, out int row, out int col)
{
    row = 0;
    col = 0;
    string[] parts = input.Split(' '); // Split the input by space.

    if (parts.Length == 2 && int.TryParse(parts[0], out row) && int.TryParse(parts[1], out col))
    {
        return row >= 1 && row <= 9 && col >= 1 && col <= 9; // Check if row and col are within bounds.
    }

    return false; // Return false if the input is invalid.
}

// This function prints the current state of the Sudoku board to the console.
// It adds visual separators to make the board resemble a Sudoku grid.
static void PrintBoard()
{
    Console.Clear(); // Clear the console.

    Console.WriteLine("Sudoku Board:");
    for (int i = 0; i < 9; i++)
    {
        if (i % 3 == 0 && i != 0)
        {
            Console.WriteLine("-----------"); // Horizontal separator for 3x3 subgrids.
        }

        for (int j = 0; j < 9; j++)
        {
            if (j % 3 == 0 && j != 0)
            {
                Console.Write("|"); // Vertical separator for columns.
            }
            Console.Write(board[i, j] == 0 ? " " : board[i, j].ToString()); // Print numbers or spaces for empty cells.
        }
        Console.WriteLine();
    }
}

// The following functions (IsSafeToPlace, IsSafeInRow, IsSafeInColumn, and IsSafeInBox) 
// are used to validate whether it's safe to place a number in a specific cell on the Sudoku board.

// This function combines checks for the number's safety in the row, column, and 3x3 box.
static bool IsSafeToPlace(int row, int col, int num)
{
    return IsSafeInRow(row, num) && IsSafeInColumn(col, num) && IsSafeInBox(row - row % 3, col - col % 3, num);
}

// This function checks if the number already exists in the specified row.
static bool IsSafeInRow(int row, int num)
{
    for (int i = 0; i < 9; i++)
    {
        if (board[row, i] == num)
        {
            return false; // The number is not safe in this row.
        }
    }
    return true; // The number is safe in this row.
}

// This function checks if the number already exists in the specified column.
static bool IsSafeInColumn(int col, int num)
{
    for (int i = 0; i < 9; i++)
    {
        if (board[i, col] == num)
        {
            return false; // The number is not safe in this column.
        }
    }
    return true; // The number is safe in this column.
}

// This function checks if the number already exists in the 3x3 box containing the cell.
static bool IsSafeInBox(int boxStartRow, int boxStartCol, int num)
{
    for (int i = 0; i < 3; i++)
    {
        for (int j = 0; j < 3; j++)
        {
            if (board[i + boxStartRow, j + boxStartCol] == num)
            {
                return false; // The number is not safe in this box.
            }
        }
    }
    return true; // The number is safe in this box.
}

// This function checks if the Sudoku puzzle is solved by looking for any empty cells (0 values).
static bool IsSolved()
{
    for (int row = 0; row < 9; row++)
    {
        for (int col = 0; col < 9; col++)
        {
            if (board[row, col] == 0)
            {
                return false; // There are still empty cells, so the puzzle is not solved.
            }
        }
    }
    return true; // All cells are filled, indicating that the puzzle is solved.
}

// This function starts the timer to measure the time taken to solve the Sudoku puzzle.
static void StartTimer()
{
    stopwatch.Start();
}

// This function stops the timer, calculates the elapsed time, and displays it in a human-readable format.
static void StopTimer()
{
    stopwatch.Stop();
    TimeSpan elapsedTime = stopwatch.Elapsed;
    Console.WriteLine($"Time taken: {elapsedTime.Hours:D2}:{elapsedTime.Minutes:D2}:{elapsedTime.Seconds:D2}");
}
}