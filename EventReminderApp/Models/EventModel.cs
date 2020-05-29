using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EventReminderApp.Models
{
    public class EventModel
    {
        public int EventId { get; set; }
        public int UserId { get; set; }
        public string EventName { get; set; }
        public string Description { get; set; }        
        public DateTime StartDate { get; set; }       
        public DateTime EndDate { get; set; }       
    }
}