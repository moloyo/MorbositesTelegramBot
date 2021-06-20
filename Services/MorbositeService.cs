using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MorbositesBotApi.Models;
using System;
using System.Collections.Generic;
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

        public async Task UpdateLasstMessageForUserAsync(User user)
        {
            using var context = new MorbisitesContext(_configuration);

            var morbosite = await context.Morbosites.SingleOrDefaultAsync(u => u.UserId == user.Id);

            if (morbosite == default)
                await AddMorbositeAsync(user);
            else
                morbosite.LastMessageOn = DateTime.UtcNow;
                await context.SaveChangesAsync();
        }

        public async Task<Morbosite> AddMorbositeAsync(User user)
        {
            using var context = new MorbisitesContext(_configuration);

            var morbosite = new Morbosite()
            {
                UserId = user.Id,
                Username = user.Username,
                JoinedOn = DateTime.UtcNow
            };

            await context.Morbosites.AddAsync(morbosite);
            await context.SaveChangesAsync();

            return morbosite;
        }

        public async Task DeleteMorbositeAsync(User user)
        {
            using var context = new MorbisitesContext(_configuration);

            var morbosite = await context.Morbosites.SingleAsync(u => u.UserId == user.Id);

            context.Morbosites.Remove(morbosite);
            await context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Morbosite>> GetMorbositesAsync()
        {
            using var context = new MorbisitesContext(_configuration);

            return await context.Morbosites.ToListAsync();
        }
    }
}
