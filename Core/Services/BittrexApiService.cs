using System;
using Microsoft.Extensions.Logging;

namespace Core.Services
{
    public interface IBittrexService
    {
        void DoMinuteTasks();
    }

    public class BittrexService : IBittrexService
    {
        private readonly ILogger<BittrexService> _logger;

        public BittrexService(ILogger<BittrexService> logger)
        {
            _logger = logger;
        }

        public void Initialize()
        {
            _logger.LogDebug("Initialized BittrexService");
        }

        public void DoMinuteTasks()
        {
            throw new NotImplementedException();
        }
    }
}
