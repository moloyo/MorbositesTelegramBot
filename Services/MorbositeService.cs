using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MorbositesBotApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace MorbositesBotApi.Services
{
    public class MorbositeService : IMorbositeService
    {
        private readonly IConfiguration _configuration;

        public MorbositeService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task UpdateLasstMessageForUserAsync(long chatId, User user)
        {
            using var context = new MorbisitesContext(_configuration);

            var morbosite = await context.Morbosites.SingleOrDefaultAsync(u => u.UserId == user.Id && u.ChatId == chatId);

            if (morbosite == default)
                await AddMorbositeAsync(chatId, user);
            else
                morbosite.LastMessageOn = DateTime.UtcNow;
                await context.SaveChangesAsync();
        }

        public async Task<Morbosite> AddMorbositeAsync(long chatId, User user)
        {
            using var context = new MorbisitesContext(_configuration);

            var morbosite = new Morbosite()
            {
                UserId = user.Id,
                Username = user.Username ?? user.FirstName,
                JoinedOn = DateTime.UtcNow,
                ChatId = chatId
            };

            await context.Morbosites.AddAsync(morbosite);
            await context.SaveChangesAsync();

            return morbosite;
        }

        public async Task DeleteMorbositeAsync(long chatId, User user)
        {
            using var context = new MorbisitesContext(_configuration);

            var morbosite = await context.Morbosites.SingleAsync(u => u.UserId == user.Id && u.ChatId == chatId);

            context.Morbosites.Remove(morbosite);
            await context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Morbosite>> GetMorbositesAsync(long chatId)
        {
            using var context = new MorbisitesContext(_configuration);

            return await context.Morbosites
                .Where(m => m.ChatId == chatId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Morbosite>> GetInactiveMorbositesAsync(long chatId)
        {
            var morbosites = await GetMorbositesAsync(chatId);

            return morbosites.Where(u => (DateTime.UtcNow - (u.LastMessageOn ?? u.JoinedOn)).Days > 14);            
        }
    }
}
