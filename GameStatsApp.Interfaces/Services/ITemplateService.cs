using System;
using GameStatsApp.Model;
using GameStatsApp.Model.Data;
using GameStatsApp.Model.ViewModels;
using System.Collections.Generic;

namespace GameStatsApp.Interfaces.Services
{
    public interface ITemplateService
    {
        string RenderTemplate(string templateName, object model);
    }
}
