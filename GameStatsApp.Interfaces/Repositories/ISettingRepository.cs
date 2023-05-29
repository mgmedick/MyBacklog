using System;
using System.Collections.Generic;
using GameStatsApp.Model.Data;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace GameStatsApp.Interfaces.Repositories
{
    public interface ISettingRepository
    {
        Setting GetSetting(string name);
        IEnumerable<Setting> GetSettings(Expression<Func<Setting, bool>> predicate = null);
        void UpdateSetting(Setting setting);
    }
}





