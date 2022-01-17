using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MyPlushBuddy.Api.Models;
using MyPlushBuddy.Api.Services;
using System;
using System.Linq;

namespace MyPlushBuddy.Api.Controllers
{
    [ApiController]
    [Route("api/enquiries")]
    public class EnquiryController : ControllerBase
    {
        private ILogger<EnquiryController> logger;
        private IMailService mailService;

        public EnquiryController(ILogger<EnquiryController> logger, IMailService mailService)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.mailService = mailService ?? throw new ArgumentNullException(nameof(mailService));
        }

        [HttpGet]
        public IActionResult GetEnquiries()
        {
            var iteracion = 2;

            logger.LogDebug($"MyPlushByddy Debug {iteracion}");
            logger.LogInformation($"MyPlushByddy Information {iteracion}");
            logger.LogWarning($"MyPlushByddy Warning {iteracion}");
            logger.LogError($"MyPlushByddy Error {iteracion}");
            logger.LogCritical($"MyPlushByddy Critical {iteracion}");

            try
            {
                throw new NotImplementedException();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
            }

            return Ok(EnquiryDataSource.Current.Enquiries);
        }

        [HttpGet("{EnquiryMail}", Name = "GetEnquiry")]
        public IActionResult GetEnquiry(string EnquiryMail)
        {
            var enquiryResult = EnquiryDataSource.Current.Enquiries.FirstOrDefault(e => e.EnquiryMail == EnquiryMail);

            if (enquiryResult == null)
            {
                return NotFound();
            }

            return Ok(enquiryResult);
        }

        [HttpPost]
        public IActionResult SaveEnquiry([FromBody] EnquiryModel enquiry)
        {
            EnquiryDataSource.Current.Enquiries.Add(enquiry);

            mailService.Send("New Enquiry mail received",
                $"Enquiry email from {enquiry.FullName} ({enquiry.EnquiryMail}) received");

            return CreatedAtRoute(
                "GetEnquiry",
                new { EnquiryMail  = enquiry.EnquiryMail},
                enquiry);
        }
    }
}
