using System;
using GameStatsApp.Model;
using GameStatsApp.Model.Data;
using GameStatsApp.Model.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GameStatsApp.Interfaces.Services
{
    public interface ICacheService
    {
        void RefreshCache();
        IEnumerable<GameView> GetGameViews(bool refresh = false);
    }
}
