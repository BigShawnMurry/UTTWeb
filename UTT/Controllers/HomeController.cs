using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using UTT.Models;

namespace UTT.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Default1/
       
        
        public ActionResult Index(string traveler, bool? found)
        {
            if (found.HasValue)
            {
                if (!found.Value)
                    ViewBag.message = "No Unused Tickets Found For This Email Address.";
                return View();
            }

            if (!string.IsNullOrEmpty(traveler))
            {
                traveler = traveler.ToUpper();
                return RedirectToAction("DisplayUTT", new { email = traveler });
            }
            
            return View();
        }
        //shows the initial UTT results set on the webpage
        public ActionResult DisplayUTT(string email)
        {
            string em = email;
            var u = new UTTModel();
            u.TravelerName = getTravelerName(em);
            if (u.TravelerName == "")
            { 
            return RedirectToAction("Index", new { traveler = em, found = false });
            } 
            List<UTTModel> utt = new List<UTTModel>();

            if (u.TravelerName != null )
            {

                SqlDataReader rd = null;
                string con = ConfigurationManager.ConnectionStrings["UTTConnectionString"].ConnectionString;
                SqlConnection sqlcon = new SqlConnection(con);
                sqlcon.Open();
                SqlCommand com = new SqlCommand("getUTTData", sqlcon);
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.AddWithValue("@CEmail", em);
                rd = com.ExecuteReader();
               
                while (rd.Read())
                {
                    var ut = new UTTModel();
                    ut.ExpirationDate = (DateTime)rd["dtExpiryDate"];
                    ut.Airline = rd["szAirline"].ToString();
                    ut.Value = rd["szTotalValue"].ToString();
                    ut.TicketNumber = rd["szTicketNumber"].ToString();
                    ut.TravelerName = u.TravelerName;
                    utt.Add(ut);
                }
            
          
            
            
            }
            //if (more.HasValue)
            //{
            //    if (more.Value)
            //    {
            //        foreach (var key in form.AllKeys)
            //        {
            //            return RedirectToAction("DisplayMore", new { ticketnum = key });
            //        }
            //    }
            //}

           
            return View(utt);
        }
        public ActionResult DisplayMore(string ticketnum )
        {
            //if(more.HasValue)
            
            List<ShowMore> more= new List<ShowMore>();
            SqlDataReader rd = null;
            string con = ConfigurationManager.ConnectionStrings["UTTConnectionString"].ConnectionString;
            SqlConnection sqlcon = new SqlConnection(con);
            sqlcon.Open();
            SqlCommand com = new SqlCommand("sp_UTTGetMore", sqlcon);
            com.CommandType = CommandType.StoredProcedure;
            com.Parameters.AddWithValue("@ticknum", ticketnum);
            rd = com.ExecuteReader();

            while (rd.Read()) 
            {
                var moinf = new ShowMore();
                moinf.airfare = (Decimal)rd["nAirFare"];
                moinf.Airline = rd["szAirline"].ToString();
                moinf.CityPairs = rd["szCityPair"].ToString();
                moinf.TravelerName = rd["ClientName"].ToString();
                moinf.BAR = rd["BAR"].ToString();
                moinf.PAR = rd["PAR"].ToString();
                moinf.CurrencyCode = rd["szCurrencyCode"].ToString();
                moinf.CustAccNum = rd["szCustomerNumber"].ToString();
                moinf.ExpirationDate =(DateTime) rd["dtExpiryDate"];
                moinf.FareBasisCode = rd["szFareBasisCode"].ToString();
                moinf.InvoiceNumber = rd["szInvoiceNumber"].ToString();
                moinf.IssueDate = (DateTime)rd["dtIssued"];
                moinf.OpenSegmentStatus = rd["szOpenSegments"].ToString();
                moinf.PCC = rd["szPCC"].ToString();
                moinf.RecordLocator = rd["RecordLocator"].ToString();
                moinf.refundableStat = rd["blnRefundable"].ToString();
                moinf.SeatClass = rd["szSeatClass"].ToString();
                moinf.TicketNumber = ticketnum;
                moinf.ticketStatus = rd["szStatus"].ToString();
                moinf.TotalAirFairValue = (Decimal)rd["nTotalAirFare"];
                moinf.TravelerName = rd["ClientName"].ToString();
                more.Add(moinf);
            }
            return View(more);
        }
        //public void SetStored(string email)
        //{
        //    string con = ConfigurationManager.ConnectionStrings["UTTConnectionString"].ConnectionString;
        //    SqlConnection sqlcon = new SqlConnection(con);
        //    SqlCommand com = new SqlCommand();
        //    com.Connection = sqlcon;
        //    com.CommandType = CommandType.StoredProcedure;
        //    if (setStoredPro == "GetUTTClientName")
        //    {
        //        com.Parameters.AddWithValue("@CEmail", email);
        //    }
        //    if (setStoredPro == "getUTTData")
        //    {
        //        com.Parameters.AddWithValue("@CEmail", email); 
        //    }
        //}
        public string getTravelerName(string email)
        {
            string traveler="";
            SqlDataReader rd = null;
            string con = ConfigurationManager.ConnectionStrings["UTTConnectionString"].ConnectionString;
                SqlConnection sqlcon = new SqlConnection(con);
                sqlcon.Open();
                SqlCommand com = new SqlCommand("GetUTTClientName",sqlcon);
               
               com.CommandType = CommandType.StoredProcedure;
               com.Parameters.AddWithValue("@CEmail", email);
               rd = com.ExecuteReader();
               while (rd.Read())
               { traveler = rd["ClientName"].ToString(); }
               sqlcon.Close();
            return traveler;
        }
    }
}
