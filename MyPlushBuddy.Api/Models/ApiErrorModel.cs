﻿namespace MyPlushBuddy.Api.Models
{
    public class ApiErrorModel
    {
        public string Id { get; set; }
        public short Status { get; set; }
        public string Code { get; set; }
        public string Links { get; set; }
        public string Title { get; set; }
        public string Detail { get; set; }
    }
}
