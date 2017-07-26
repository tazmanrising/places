using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace QA_Report_Service
{
    public class QA_Builder
    {


        public void GetAllCalls()
        {
            
            //using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings[Environment.MachineName + "ESO"]))
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["qaList"]))
            {

               
                try
                {
                    
                    conn.Open();
                                        
                    //TODO  change to stringbuilder

                    string query = "select * from QA_Reporting.dbo.QADatabases WHERE ServerName = 'TMPSQL2' AND Status = 1";
                    //string query = "select * from QA_Reporting.dbo.QADatabases WHERE DatabaseName in ('CenturyTel','Clearview','DirectEnergy')";
                    var sqlCmd = new SqlCommand(query, conn);
                    var da = new SqlDataAdapter { SelectCommand = sqlCmd };
                    var dt = new DataTable();
                    da.Fill(dt);

                    List<QAFields> qaFields = new List<QAFields>();

                    foreach(DataRow row in dt.Rows)
                    {
                        qaFields.Add(new QAFields()
                        {   Id = (int)row["QADatabasesId"],
                            ServerName = row["ServerName"].ToString(),
                            DatabaseSchemaTable = row["DatabaseSchemaTable"].ToString(),
                            ColumnIdName = row["ColumnIdName"].ToString(),
                            Description = row["Description"].ToString(),
                            DatabaseName = row["DatabaseName"].ToString(),
                            ClientName = row["ClientName"].ToString(),
                            Agent = row["Agent"].ToString(),
                            Supervisor = row["Supervisor"].ToString(),
                            Location = row["Location"].ToString(),
                            Number = row["Number"].ToString(),
                            Disposition = row["Disposition"].ToString(),
                            CallDate = row["CallDate"].ToString(),
                            CallLength = row["CallLength"].ToString(),
                            InboundCall = row["InboundCall"].ToString(),
                            OutboundCall = row["OutboundCall"].ToString(),
                        });
                            
                    }
                    

                    //##   Loop over all the databases to insert into 
                    for (var i = 0; i <= qaFields.Count() - 1; i++)
                    {
                        query = "";
                        query = "select";
                        query += " min(IdentityColumnId) as Start";
                        query += ",max(IdentityColumnId) as [end]";
                        query += ",max(IdentityColumnId) - min(IdentityColumnId) as diff";
                        query += ",(max(IdentityColumnId) - min(IdentityColumnId)) * .03 as qa";
                        query += " from ";
                        query += qaFields[i].DatabaseSchemaTable;
                        query += " where ";
                        // #################   USE THIS ##########################
                        query += " CallDate between DATEADD(hh, -1, GetDATE()) and Getdate()";

                        //query += " CallDate between '2/9/2017 6:00' and '2/9/2017 19:00'";


                        sqlCmd = new SqlCommand(query, conn);

                        da = new SqlDataAdapter { SelectCommand = sqlCmd };
                        var dt2 = new DataTable();
                        da.Fill(dt2);
                        List<QAData> qaData = new List<QAData>();
                        int? result = dt2.Rows[0].Field<int?>("diff");

                        if (dt2.Rows[0]["diff"] == DBNull.Value || result == 0)
                        {
                            continue;
                        }

                        foreach (DataRow row in dt2.Rows)
                        {
                            qaData.Add(new QAData() { StartId = (int)row["Start"], EndId = (int)row["end"], Difference = (int)row["diff"], QAPercentAmount = Convert.ToDouble(row["qa"]) });
                        }

                        Random rnd = new Random();
                        int start = qaData[0].StartId;
                        int end = qaData[0].EndId;
                        double percent = qaData[0].QAPercentAmount;
                        var t = Math.Ceiling(percent);
                        string idVals = "";
                        for (int ctr = 1; ctr <= t; ctr++)
                        {                        
                            idVals += rnd.Next(start, end) + ",";
                        }

                        idVals = idVals.TrimEnd(',');
                        List<string> uniques = idVals.Split(',').Distinct().ToList();
                        string newStr = string.Join(",", uniques);
                        
                        query = "";
                        query += " INSERT INTO [QA_Reporting].[dbo].[QAList]";
                        query += " ([ClientName]";
                        query += " ,[Agent]";
                        query += " ,[Number]";
                        query += " ,[Disposition]";
                        query += " ,[Calldate]";
                        query += " ,[CallLength]";
                        query += " ,[InboundCall]";
                        query += " ,[OutboundCall]";
                        query += " ,[IdentityColumnId])";
                        query += " SELECT ";
                        query += "ClientName";
                        query += ",Agent";
                        query += ",Number";
                        query += ",Disposition";
                        query += ",CallDate";
                        query += ",CallLength";
                        query += ", InboundCall";
                        query += ",OutboundCall";
                        query += ",IdentityColumnId";
                        query += " FROM " + qaFields[i].DatabaseSchemaTable;
                        query += " WHERE IdentityColumnId in(" + newStr + ")";
                        query += " AND Agent IS NOT NULL";
                        using (SqlCommand command = new SqlCommand(query, conn))
                        {
                            command.ExecuteNonQuery();
                        }


                    }



                    conn.Close();
                    conn.Dispose();

                }
                catch (Exception ex)
                {

                    throw;
                }



            }


        }

         

    }

}