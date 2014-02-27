using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UTT.Models
{
    public class ShowMore
    {
        
        public string TravelerName { get; set; }



        public Decimal TotalAirFairValue { get; set; }
        public string TicketNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CustAccNum { get; set; }
        public string DepartmentCode { get; set; }
        public string PCC { get; set; }
        public string Airline { get; set; }
        public string refundableStat { get; set; }
        public string RecordLocator { get; set; }
        public string ticketStatus { get; set; }
       
        public Decimal airfare { get; set; }
        public string SeatClass { get; set; }
        public string CityPairs { get; set; }
        public string PAR { get; set; }
        public string BAR { get; set; }
        public string FareBasisCode { get; set; }
     
        public DateTime IssueDate { get; set; }
        public DateTime ExpirationDate { get; set; }
     
    }
}