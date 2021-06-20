using System.Threading.Tasks;

namespace MorbositesBotApi.Services
{
    public interface IBotService
    {
        Task StartBotAsync();

        Task StopBotAsync();
    }
}