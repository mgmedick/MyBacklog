using System;
using GameStatsApp.Model;
using GameStatsApp.Model.Data;
using GameStatsApp.Model.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GameStatsApp.Interfaces.Services
{
    public interface IEmailService
    {
        Task SendEmailTemplate(string emailTo, string subject, string templateName, object parameters);
        Task SendEmail(string emailTo, string subject, string htmlBody);
    }
}
