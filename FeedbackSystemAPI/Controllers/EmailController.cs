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
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FeedbackSystemAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        private readonly IMailService mailService;
        private readonly FeedbacSystemkDBContext _context;
        public EmailController(IMailService mailService, FeedbacSystemkDBContext context)
        {
            _context = context;
            this.mailService = mailService;
        }

        

        [AllowAnonymous]
        [HttpPost("Send")]
        public async Task<IActionResult> Send([FromForm] string toMail, string idFb)
        {
            var fb = await _context.Feedbacks.Include(d => d.Device).ThenInclude(d => d.Location)
                .Where(d => d.FeedbackId == idFb).FirstAsync();



            string bodyMess = "Thiết bị: "+ fb.Device.Name +" Tại : " +
                fb.Device.Location.LocatitonName + " có vấn đề, hư hỏng, không sử dụng được và cần bạn đến xem sét và sửa chữa" ;

            try
            {
                await mailService.SendEmailAsync(toMail, bodyMess);
                return Ok();
            }
            catch (Exception ex)
            {

                throw ex;
            }


        }
    }
}
