using System;
using GameStatsApp.Model;
using GameStatsApp.Model.Data;
using GameStatsApp.Model.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GameStatsApp.Interfaces.Services
{
    public interface ISettingService
    {       
        Setting GetSetting(string name);
        void UpdateSetting(string name, string value);
        void UpdateSetting(string name, DateTime value);
        void UpdateSetting(string name, int value);
        void UpdateSetting(Setting setting); 
    }
}
