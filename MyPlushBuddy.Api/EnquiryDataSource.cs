using MyPlushBuddy.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyPlushBuddy.Api
{
    public class EnquiryDataSource
    {
        public static EnquiryDataSource Current { get; } = new EnquiryDataSource();
        public List<EnquiryModel> Enquiries { get; set; }

        public EnquiryDataSource()
        {
            Enquiries = new List<EnquiryModel>()
            {
                new EnquiryModel()
                {
                    FullName = "John Doe",
                    Description = "Test Enquiry",
                    EnquiryMail = "johndoe@gmail.com",
                    Subject = "Test Subject"
                }
            };
        }
    }
}
