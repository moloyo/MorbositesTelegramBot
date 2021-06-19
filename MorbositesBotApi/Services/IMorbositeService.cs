using MorbositesBotApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace MorbositesBotApi.Services
{
    public interface IMorbositeService
    {
        Task UpdateLasstMessageForUserAsync(User user);
        Task<Morbosite> AddMorbositeAsync(User user);
        Task DeleteMorbositeAsync(User user);
        Task<IEnumerable<Morbosite>> GetMorbositesAsync();
    }
}