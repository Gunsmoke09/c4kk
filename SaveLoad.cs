using System;
using System.IO;
using System.Text.Json;

namespace LineUp
{
    public static class SaveLoad
    {
        public static bool TrySave(string path, GameState gs)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(path)) return false;
                //check if the path is valid
                var json = JsonSerializer.Serialize(gs);
                File.WriteAllText(path, json);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool TryLoad(string path, out GameState? gs)
        {
            gs = null;
            try
            {
                if (string.IsNullOrWhiteSpace(path) || !File.Exists(path)) return false;
                var json = File.ReadAllText(path);
                gs = JsonSerializer.Deserialize<GameState>(json);
                return gs != null;
            }
            catch
            {
                gs = null;
                return false;
            }
        }
    }
}
