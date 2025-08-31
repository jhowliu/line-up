using System;

namespace Lineup
{
    class Program {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, Welcome to Lineup. Please select options, input 0,1:");
            Console.WriteLine("0: Load Game\n1: New Game");
            Console.Write(">> ");
            int option = Convert.ToInt32(Console.ReadLine());
            Game game = new Game();
            Console.WriteLine(option);
            while (option != 1 || option != 2)
            {
                if (option == 1)
                {
                    Console.WriteLine("Now, Please input the columns and rows.");
                    Console.Write("Input row number (>=6) >> ");
                    int row = Convert.ToInt32(Console.ReadLine());
                    while (row < Constant.DefaultRowSize)
                    {
                        Console.Write("Please input row number again (>=6) >> ");
                        row = Convert.ToInt32(Console.ReadLine());
                    }

                    Console.Write("Input column number (>=7) >> ");
                    int col = Convert.ToInt32(Console.ReadLine());
                    while (col < Constant.DefaultColumnSize)
                    {
                        Console.Write("Please input column number again (>=7) >> ");
                        col = Convert.ToInt32(Console.ReadLine());
                    }
                    game = new Game(row, col);
                    break;
                }
                else
                {
                    Console.Write("Please input 0 or 1. >> ");
                    option = Convert.ToInt32(Console.ReadLine());
                    continue;
                }
            }

            game.drawGrid();
        }
    }
}
