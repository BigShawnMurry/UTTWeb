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
       
        
        public ActionResult Index(string traveler, bool? found,string UTTSelect,string airline,string custid)
        {
            if (found.HasValue)
            {
                if (!found.Value)
                    ViewBag.message = "No Unused Tickets Found For This Email Address or Customer ID.";
                return View();
            }

            if (!string.IsNullOrEmpty(traveler) && string.IsNullOrEmpty(custid))
            {
                if(!string.IsNullOrEmpty(airline))
                {
                    traveler=traveler.ToUpper();
                    custid = "";
                    return RedirectToAction("DisplayUTT", new {email=traveler,UTTsel=UTTSelect,carrier=airline,cust=custid});
                }
                else{
                    airline = "";
                    custid = "";
                traveler = traveler.ToUpper();
                return RedirectToAction("DisplayUTT", new { email = traveler,UTTsel=UTTSelect,carrier=airline, cust=custid });
            }}
            if(!string.IsNullOrEmpty(custid) && string.IsNullOrEmpty(traveler))
            {
                if (!string.IsNullOrEmpty(airline))
                {
                    traveler = "";
                    return RedirectToAction("DisplayUTT", new { UTTsel = UTTSelect, carrier = airline, cust = custid,email=traveler });
                }
                else 
                {
                    traveler = "";
                    airline = "";
                    return RedirectToAction("DisplayUTT", new { UTTsel = UTTSelect,  cust = custid, carrier=airline,email=traveler });
                }
            }
            if (string.IsNullOrEmpty(custid) && string.IsNullOrEmpty(traveler))
            {
                ViewBag.Message = "Please Enter Either a Traveler Email Address or a Customer ID";
                return View();
            }
            if(!string.IsNullOrEmpty(traveler) && !string.IsNullOrEmpty(custid))
            {
                ViewBag.Message="Please Only Enter Either a Traveler Email Address or a Customer ID, But Not Both";
                return View();
            }
            
            return View();
        }
        //shows the initial UTT results set on the webpage
        public ActionResult DisplayUTT(string email,string UTTsel, string carrier, string cust)
        {
            DateTime exp;
            string em = email;
            var u = new UTTModel();
            string custom = cust;
            string air=carrier;
            if (em != null && custom==null)
            {
                u.TravelerName = getTravelerName(em);
                if (u.TravelerName =="")
                { return RedirectToAction("Index", new { traveler=em, custid=custom,found=false}); }
                String customerid = getCustID(em);
                u.ClientName = getClientName(customerid);
            }
            else if(em==null && custom!=null) { 
                u.ClientName = getClientName(custom);
                if(u.ClientName=="")
                {
                    return RedirectToAction("Index", new { traveler = em, custid = custom, found = false }); 
                }
                u.TravelerName = "";
            }
            if (em == null && custom==null)
            { 
            return RedirectToAction("Index", new { traveler = em,custid=custom, found = false });
            } 
            List<UTTModel> utt = new List<UTTModel>();

           
                if (UTTsel == "All") { 
                SqlDataReader rd = null;
                string con = ConfigurationManager.ConnectionStrings["UTTConnectionString"].ConnectionString;
                SqlConnection sqlcon = new SqlConnection(con);
                sqlcon.Open();
                SqlCommand com = new SqlCommand("sp_getUTTData", sqlcon);
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.AddWithValue("@szEmail", em);
                com.Parameters.AddWithValue("@szCustNo", custom);
                com.Parameters.AddWithValue("@szAirline", carrier);
                rd = com.ExecuteReader();
               
                while (rd.Read())
                {
                    var ut = new UTTModel();
                    exp = (DateTime)rd["dtExpiryDate"];
                    ut.ExpirationDate = exp.ToShortDateString();
                    ut.Airline = rd["szAirline"].ToString();
                    ut.Value = rd["szTotalValue"].ToString();
                    ut.TicketNumber = rd["szTicketNumber"].ToString();
                    ut.ClientName = u.ClientName;
                    
                        ut.TravelerName = u.TravelerName;
                        if (ut.TravelerName == "")
                        {
                            string fname = rd["szFirstName"].ToString();
                            string lname = rd["szLastName"].ToString();
                            string name = fname + " " + lname;
                            ut.TravelerName = name;
                        }
                  
                    utt.Add(ut);
                }}
                if (UTTsel == "Used")
                {
                    SqlDataReader rd = null;
                    string con = ConfigurationManager.ConnectionStrings["UTTConnectionString"].ConnectionString;
                    SqlConnection sqlcon = new SqlConnection(con);
                    sqlcon.Open();
                    SqlCommand com = new SqlCommand("sp_UTTUSED", sqlcon);
                    com.CommandType = CommandType.StoredProcedure;
                    com.Parameters.AddWithValue("@szEmail", em);
                    com.Parameters.AddWithValue("@szCustNo", custom);
                    com.Parameters.AddWithValue("@szAirline", carrier);
                    rd = com.ExecuteReader();

                    while (rd.Read())
                    {
                        var ut = new UTTModel();
                        exp = (DateTime)rd["dtExpiryDate"];
                        ut.ExpirationDate = exp.ToShortDateString();
                        ut.Airline = rd["szAirline"].ToString();
                        ut.Value = rd["szTotalValue"].ToString();
                        ut.TicketNumber = rd["szTicketNumber"].ToString();



                        
                            ut.TravelerName = u.TravelerName;
                            if (ut.TravelerName == "")
                        {
                            string fname = rd["szFirstName"].ToString();
                            string lname = rd["szLastName"].ToString();
                            string name = fname + " " + lname;
                            ut.TravelerName = name;
                        }
                            ut.ClientName = u.ClientName;
                  
                        
                        utt.Add(ut);
                    }
                }
                if(UTTsel == "OpenPartial")
                {
                    SqlDataReader rd = null;
                    string con = ConfigurationManager.ConnectionStrings["UTTConnectionString"].ConnectionString;
                    SqlConnection sqlcon = new SqlConnection(con);
                    sqlcon.Open();
                    SqlCommand com = new SqlCommand("sp_UTTOpen", sqlcon);
                    com.CommandType = CommandType.StoredProcedure;
                    com.Parameters.AddWithValue("@szEmail", em);
                    com.Parameters.AddWithValue("@szCustNo", custom);
                    com.Parameters.AddWithValue("@szAirline", carrier);
                    rd = com.ExecuteReader();
                   
                    while (rd.Read())
                    {
                        var ut = new UTTModel();
                       exp = (DateTime)rd["dtExpiryDate"];
                       ut.ExpirationDate = exp.ToShortDateString();
                        ut.Airline = rd["szAirline"].ToString();
                        ut.Value = rd["szTotalValue"].ToString();
                        ut.TicketNumber = rd["szTicketNumber"].ToString();


                        ut.ClientName = u.ClientName;
                      
                            ut.TravelerName = u.TravelerName;
                            if (ut.TravelerName == "")
                            {
                                string fname = rd["szFirstName"].ToString();
                                string lname = rd["szLastName"].ToString();
                                string name = fname + " " + lname;
                                ut.TravelerName = name;
                            }
                  
                        
                      
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
            DateTime issue;
            DateTime exp;
            while (rd.Read()) 
            {
                var moinf = new ShowMore();
                
                moinf.Airline = rd["szAirline"].ToString();
                moinf.CityPairs = rd["szCityPair"].ToString();
                moinf.TravelerName = rd["ClientName"].ToString();
                moinf.BAR = rd["BAR"].ToString();
                moinf.PAR = rd["PAR"].ToString();
                
                moinf.CustAccNum = rd["szCustomerNumber"].ToString();
                exp =(DateTime)rd["dtExpiryDate"];
                moinf.ExpirationDate = exp.ToShortDateString();
                moinf.FareBasisCode = rd["szFareBasisCode"].ToString();
                moinf.DepartmentCode = rd["szDepartmentCode"].ToString();
                issue = (DateTime)rd["dtIssued"];
                moinf.IssueDate = issue.ToShortDateString();
             
                moinf.PCC = rd["szPCC"].ToString();
                moinf.RecordLocator = rd["RecordLocator"].ToString();
                moinf.refundableStat = rd["blnRefundable"].ToString();
                if(moinf.refundableStat.Equals("true"))
                { moinf.refundableStat = "Refundable"; }
                else { moinf.refundableStat = "Nonrefundable"; }
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
        private string getCustID(string email)
        {
            string id = "";
            SqlDataReader rd = null;
            string con = ConfigurationManager.ConnectionStrings["UTTConnectionString"].ConnectionString;
            SqlConnection sqlcon = new SqlConnection(con);
            sqlcon.Open();
            SqlCommand com = new SqlCommand("sp_GetUTTCustID", sqlcon);

            com.CommandType = CommandType.StoredProcedure;
            com.Parameters.AddWithValue("@CEmail", email);
            rd = com.ExecuteReader();
            while (rd.Read())
            { id = rd["szCustomerNumber"].ToString(); }
            sqlcon.Close();

            return id;
        }
        public string getClientName(string cusid)
        {
            string cust = "";
            SqlDataReader rd = null;
            string con = ConfigurationManager.ConnectionStrings["UTTConnectionString"].ConnectionString;
            SqlConnection sqlcon = new SqlConnection(con);
            sqlcon.Open();
            SqlCommand com = new SqlCommand("sp_GetUTTCustName", sqlcon);
            com.CommandType = CommandType.StoredProcedure;
            com.Parameters.AddWithValue("@CustNo", cusid);
            rd = com.ExecuteReader();
            while (rd.Read())
            { cust = rd["CompName"].ToString(); }
            sqlcon.Close();
            return cust;
        }
    }
}
