using MorbositesBotApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace MorbositesBotApi.Services
{
    public interface IMorbositeService
    {
        Task UpdateLasstMessageForUserAsync(long chatId, User user);
        Task<Morbosite> AddMorbositeAsync(long chatId, User user);
        Task DeleteMorbositeAsync(long chatId, User user);
        Task<IEnumerable<Morbosite>> GetMorbositesAsync(long chatId);
        Task<IEnumerable<Morbosite>> GetInactiveMorbositesAsync(long chatId);
    }
}