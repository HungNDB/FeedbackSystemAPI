using FeedbackSystemAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;

namespace FeedbackSystemAPI.Services
{
    public interface IMailService
    {
        Task SendEmailAsync(string toMail, string bodyMess);
    }
}
