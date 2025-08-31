using System.Runtime.InteropServices;
using System.Text.Json;

namespace Lineup
{
    class FileManager()
    {
        public static bool SaveGame(Game game, string path)
        {
            File.WriteAllText(path, game.toJSON());
            Console.WriteLine(game.toJSON());
            return false;
        }

        public static Game? LoadGame(string path)
        {
            string jsonStringRead = File.ReadAllText(path);
            var jsonData = JsonSerializer.Deserialize<object>(jsonStringRead);
            if (jsonData != null)
            {
                return Game.LoadFromJSON(jsonData);
            }

            return null;
        }
    }
}