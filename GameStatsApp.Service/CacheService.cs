using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Caching.Memory;
using System.Net.Mail;
using System.Net;
using Serilog;
using GameStatsApp.Interfaces.Services;
using GameStatsApp.Interfaces.Repositories;
using System.Threading.Tasks;
using GameStatsApp.Model.Data;
using GameStatsApp.Model;
using System.Linq.Expressions;
using GameStatsApp.Common.Extensions;

namespace GameStatsApp.Service
{
    public class CacheService : ICacheService
    {
        public IMemoryCache _cache { get; set; }
        public IGameRepository _gameRepo { get; set; }

        public CacheService(IMemoryCache cache, IGameRepository gameRepo)
        {
            _cache = cache;
            _gameRepo = gameRepo;
        }

        public IEnumerable<Game> GetGames()
        {
            IEnumerable<Game> games = null;
            if (!_cache.TryGetValue<IEnumerable<Game>>("games", out games))
            {
                games = _gameRepo.GetGames();
                foreach(var game in games)
                {
                    game.SantizedName = game.Name.SanatizeGameName();
                    game.SantizedNameNoSpace = game.SantizedName.Replace(" ", string.Empty);
                }

                var expireDate = DateTime.UtcNow.Date.AddDays(1).AddHours(2).AddMinutes(30) - DateTime.UtcNow;
                _cache.Set("games", games, expireDate);
            }

            return games;
        }
    }
}
