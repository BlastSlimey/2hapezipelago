
using ILogger = Core.Logging.ILogger;

namespace _2hapezipelago
{
    public class APMod : IMod
    {
        public ILogger Logger;
        public ConnectionHandler? ConHandler;
        public ResearchHandler? ResHandler;
        public SaveDataHandler? SaveHandler;

        public APMod(ILogger logger)
        {
            logger.Info?.Log("2hapezipelago initialized, I hope you're not BK anymore...");
            Logger = logger;
            ConHandler = new ConnectionHandler(this);
            ResHandler = new ResearchHandler(this);
            SaveHandler = new SaveDataHandler(this);
        }

        public void Dispose()
        {
            ConHandler?.Dispose();
            ResHandler?.Dispose();
            SaveHandler?.Dispose();
            ConHandler = null;
            SaveHandler = null;
            ResHandler = null;
        }
    }
}
