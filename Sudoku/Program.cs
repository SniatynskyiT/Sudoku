using System;

namespace Sudoku
{
    class Program
    {
        static void Main(string[] args)
        {
            var grid1 = "003020600900305001001806400008102900700000008006708200002609500800203009005010300";
            var grid2 = ".....6....59.....82....8....45........3........6..3.54...325..6..................";
            var grid3 = "4.....8.5.3..........7......2.....6.....8.4......1.......6.3.7.5..2.....1.4......";
            var grid4 = "800000000003600000070090200050007000000045700000100030001000068008500010090000400";

            SudokuHandler.Display(SudokuHandler.GetGridValues(grid3));

            Console.WriteLine();
            Console.WriteLine("Solution!");

            SudokuHandler.Display(SudokuHandler.Solve(grid3));

            Console.ReadKey();
        }
    }
}
