using FeedbackSystemAPI.Models;
using FeedbackSystemAPI.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using MailKit.Net.Smtp;
using MailKit.Security;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FeedbackSystemAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        private readonly IMailService mailService;
        public EmailController(IMailService mailService)
        {
            this.mailService = mailService;
        }
        [AllowAnonymous]
        [HttpPost("Send")]
        public async Task<IActionResult> Send([FromForm] MailRequest request)
        {


            try
            {
                await mailService.SendEmailAsync(request);
                return Ok();
            }
            catch (Exception ex)
            {

                throw ex;
            }


        }
    }
}
