using Framework.Managers;
using ModdingAPI;

namespace BlasphemousRandomizer
{
    public class Logger
    {
        public enum LogType { Standard, Error, Data };

        private string lastScene;
        private bool firstLog;

        public Logger(string initialMessage)
        {
            firstLog = true;
            lastScene = "";

            if (initialMessage != null && initialMessage != "")
                Log(initialMessage, LogType.Standard);
        }

        public void Log(string text, LogType type)
        {
            // Log data output to file
            if (type == LogType.Data)
            {
                Main.Randomizer.FileUtil.saveTextFile("data.txt", text);
                return;
            }

            // Log message onto output file
            string output = "";

            // Add space to beginning of each game session
            if (firstLog)
            {
                output += "\n\n";
                firstLog = false;
            }

            // Check if in new scene
            string currentScene = Core.LevelManager.currentLevel != null ? Core.LevelManager.currentLevel.LevelName : "Initialization";
            if (lastScene != currentScene)
            {
                output += currentScene + "\n";
                lastScene = currentScene;
            }

            // Indent & write to file
            output += type == LogType.Error ? "  * " : "\t";
            Main.Randomizer.FileUtil.appendLog(output + text + "\n");
        }
    }
}
