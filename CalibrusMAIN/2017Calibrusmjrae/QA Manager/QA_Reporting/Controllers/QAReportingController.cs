using QA_Reporting.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Data;
using QA_Reporting.Utilities;
using QA_Reporting.CustomAttributes;

namespace QA_Reporting.Controllers
{
    [CustomApiAccessAuthorize]
    public class QAReportingController : ApiController
    {

        private QAContext _db = new QAContext();

        [HttpGet]
        [Route("GetQAByDate/date/{date}")]
        public IHttpActionResult GetQAFromDate(string date)
        {

            var qaLists = new List<QAList>();

            //incoming date format   string dateIn = "2-15-2017";

            DateTime dateParsed = DateTime.Parse(date);


            try
            {
                var query = _db.QALists
                    .Where(z => z.CreatedDateTime >= dateParsed)
                    .AsEnumerable()
                    .Select(z => new QAList()
                    {
                        Agent = z.Agent.Trim(),
                        ClientName = z.ClientName,
                        Disposition = z.Disposition,
                        Calldate = z.Calldate,
                        Comment = z.Comment,
                        CreatedDateTime = z.CreatedDateTime,
                        CallLength = z.CallLength,
                        IdentityColumnId = z.IdentityColumnId,
                        Number = z.Number,
                        InboundCall = z.InboundCall,
                        OutboundCall = z.OutboundCall,
                        Status = z.Status,
                        QAListId = z.QAListId

                    }).ToList();

                return Ok(query);

            }
            catch (Exception ex)
            {

                throw;
            }
            

        }

        [HttpGet]
        [Route("GetQAByDateTime/date/{date}/time/{time}")]
        public IHttpActionResult GetQAFromDateTime(string date, string time)
        {
            var qaLists = new List<QAList>();

            string timeExt = ":00";
            string combinedTime = date + ' ' + time + timeExt;

            try
            {

                var param = new SqlParameter("@datetime", combinedTime);
                var result = _db.Database.SqlQuery<QAList>("dbo.GetQAListByDateTime @datetime", param).ToList();

                          

                return Ok(result);

            }
            catch (Exception ex)
            {

                throw;
            }
        }


        [HttpGet]
        [Route("GetQAByDateTimeOLDWAY/date/{date}/time/{time}")]
        public IHttpActionResult GetQAFromDateTimeOLDWAY(string date, string time)
        {

          
            string xdate = "2-15-2017";
            string xtime = "11";
            string timeExt = ":00";
            string combinedTime = date + ' ' + time + timeExt;

            string sprocName = "GetQAListByDateTime";

            var qaLists = new List<QAList>();


            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["qaList"]))
            {

                

                try
                {
                    using (SqlDataAdapter da = new SqlDataAdapter())
                    {
                        da.SelectCommand = new SqlCommand(sprocName, conn);
                        da.SelectCommand.CommandType = CommandType.StoredProcedure;
                        da.SelectCommand.Parameters.Add("@datetime", SqlDbType.DateTime).Value = combinedTime;
                        DataSet ds = new DataSet();
                        da.Fill(ds, "qaList");

                        DataTable dt = ds.Tables["qaList"];

                        foreach(DataRow row in dt.Rows)
                        {

                            var qaList = new QAList
                            {
                                Agent = CommonUtilities.ConvertFromDbVal<string>(row["Agent"]).Trim(),
                                Calldate = CommonUtilities.ConvertFromDbVal<DateTime>(row["Calldate"]),
                                CallLength = CommonUtilities.ConvertFromDbVal<int>(row["CallLength"]),
                                ClientName = CommonUtilities.ConvertFromDbVal<string>(row["ClientName"]),
                                InboundCall = CommonUtilities.ConvertFromDbVal<string>(row["InboundCall"]),
                                OutboundCall = CommonUtilities.ConvertFromDbVal<string>(row["OutboundCall"]),
                                CreatedDateTime = CommonUtilities.ConvertFromDbVal<DateTime>(row["CreatedDateTime"])

                              
                            };

                            qaLists.Add(qaList);
                        }


                        if (qaLists.Any() == false)
                        {
                            return NotFound();
                        }

                        


                    }
                }
                catch(SqlException ex)
                {

                }
                catch (Exception e)
                {

                    throw;
                }
               

            }


            return (Ok(qaLists));




        }


        [HttpGet]
        [Route("GetCalls")]
        public IHttpActionResult GetAllCalls()
        {

            var qaLists = new List<QAList>();

            try
            {
                var query = (from c in _db.QALists
                             orderby c.Calldate
                             select c);

                foreach(var q in query)
                {
                    var qaList = new QAList
                    {
                        Agent = q.Agent.Trim(),
                        Calldate = q.Calldate,
                        ClientName = q.ClientName,
                        CreatedDateTime = q.CreatedDateTime,
                        InboundCall = q.InboundCall,
                        OutboundCall = q.OutboundCall,
                        CallLength = q.CallLength,
                        Disposition = q.Disposition,
                        Supervisor = q.Supervisor,
                        Location = q.Location

                    };

                    qaLists.Add(qaList);

                }

                if(query.Any() == false)
                {
                    return NotFound();
                }

                return (Ok(qaLists));
                
            }
            catch (Exception exception)
            {

                throw;
            }





        }



    }
}
