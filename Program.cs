using System;

namespace Lineup
{
    class Program {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello");
            Game game = new Game(6, 7);
            game.drawGrid();
        }
    }
}
