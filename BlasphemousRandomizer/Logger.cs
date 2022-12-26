using Framework.Managers;

namespace BlasphemousRandomizer
{
    public class Logger
    {
        public enum LogType { Standard, Error, Data };

        private string lastScene;
        private bool debugMode;
        private bool firstLog;

        public Logger(string initialMessage)
        {
            firstLog = true;
            lastScene = "";

            debugMode = FileUtil.read("debug.txt", false, out string text) && text == "true";
            if (initialMessage != null && initialMessage != "")
                Log(initialMessage, LogType.Standard);
        }

        public void Log(string text, LogType type)
        {
            if (!debugMode) return;

            // Log data output to file
            if (type == LogType.Data)
            {
                FileUtil.writeFull("data.txt", text);
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
            FileUtil.writeLine("log.txt", output + text + "\n");
        }
    }
}
