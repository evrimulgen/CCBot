using Microsoft.Extensions.Logging;

namespace CCBotDesktop.Presenters
{
    public interface IMainPresenter
    {

    }

    public class MainPresenter : IMainPresenter
    {
        private ILogger<MainPresenter> _logger;

        public MainPresenter(ILogger<MainPresenter> logger)
        {
            _logger = logger;
        }
    }
}
