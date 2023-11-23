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

        public void RefreshCache()
        {
            GetGameViews(true);
        }

        public IEnumerable<GameView> GetGameViews(bool refresh = false)
        {
            IEnumerable<GameView> games = null;
            if (!_cache.TryGetValue<IEnumerable<GameView>>("games", out games) || refresh)
            {
                games = _gameRepo.GetGameViews();
                foreach(var game in games)
                {
                    game.SantizedName = game.Name.SanatizeGameName();
                    game.SantizedNameNoSpace = game.SantizedName.Replace(" ", string.Empty);
                }

                _cache.Set("games", games);
            }
            
            return games;
        }
    }
}
