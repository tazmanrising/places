using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FrontierModel;

/// <summary>
/// Summary description for Data
/// </summary>
public class Data
{
    public static IList<tblMain> GetResult()
    {
        IList<tblMain> bindResult = (IList<tblMain>)HttpContext.Current.Cache["result"];

        if (bindResult == null)
        {
            using (FrontierEntities data = new FrontierEntities())
            {
                IQueryable<tblMain> result = data.tblMains.Include("tblTns").AsQueryable();

                if (SessionVars.RecordLocator != 0)
                {
                    result = result.Where(s => s.MainId == SessionVars.RecordLocator);
                }
                if (!String.IsNullOrEmpty(SessionVars.PhoneNumber))
                {
                    result = result.Where(s => s.tblTns.Where(t => t.Tn == SessionVars.PhoneNumber).Count() > 0);
                }
                if (!String.IsNullOrEmpty(SessionVars.TpvAgentId))
                {
                    result = result.Where(s => s.TpvAgentId == SessionVars.TpvAgentId);
                }
                if (!String.IsNullOrEmpty(SessionVars.BillingName))
                {
                    result = result.Where(s => s.DecisionMaker == SessionVars.BillingName);
                }
                if (!String.IsNullOrEmpty(SessionVars.CompanyName))
                {
                    result = result.Where(s => s.CompanyName == SessionVars.CompanyName);
                }
                if (!String.IsNullOrEmpty(SessionVars.Disposition))
                {
                    if (SessionVars.Disposition == "All Failures")
                    {
                        result = result.Where(s => s.Verified == "0");
                    }
                    else
                    {
                        result = result.Where(s => s.Concern == SessionVars.Disposition);
                    }
                }
                if (SessionVars.StartDate != null && SessionVars.EndDate != null)
                {
                    result = result.Where(p => ((p.DateTime.Value >= SessionVars.StartDate.Value && p.DateTime.Value < SessionVars.EndDate.Value)));
                }
                bindResult = result.ToList();
                HttpContext.Current.Cache["result"] = bindResult;
            }
        }
        return bindResult;
    }


    public static IList<tblE911LoadFile> GetE911Result()
    {
        IList<tblE911LoadFile> bindResult = (IList<tblE911LoadFile>)HttpContext.Current.Cache["result"];

        if (bindResult == null)
        {
            using (FrontierEntities data = new FrontierEntities())
            {
                IQueryable<tblE911LoadFile> result = data.tblE911LoadFile.Include("tblE911Main").AsQueryable();
                //IQueryable<tblE911LoadFile> result = data.tblE911LoadFile.AsQueryable();
                if (!String.IsNullOrEmpty(SessionVarsE911.SubscriberId))
                {
                    result = result.Where(s => s.SubscriberId == SessionVarsE911.SubscriberId);
                }
                if (!String.IsNullOrEmpty(SessionVarsE911.PhoneNumber))
                {
                    result = result.Where(s => s.TN == SessionVarsE911.PhoneNumber);
                }
                if (!String.IsNullOrEmpty(SessionVarsE911.Name))
                {
                    result = result.Where(s => s.Name == SessionVarsE911.Name);
                }
                if (!String.IsNullOrEmpty(SessionVarsE911.Disposition))
                {
                    result = result.Where(s => s.tblE911Main.Where(t => t.Disposition == SessionVarsE911.Disposition).Count() > 0);
                }
                if (SessionVarsE911.StartDate != null && SessionVarsE911.EndDate != null)
                {
                    //result = result.Where(s => s.tblE911Main.Where(t => t.CallDateTime.Value >= SessionVarsE911.StartDate.Value && t.CallDateTime.Value < SessionVarsE911.EndDate.Value));
                    result = result.Where(p => ((p.LoadDateTime.Value >= SessionVars.StartDate.Value && p.LoadDateTime.Value < SessionVars.EndDate.Value)));
                }

                bindResult = result.ToList();
                HttpContext.Current.Cache["result"] = bindResult;
            }

        }

        return bindResult;
    }
    
    public static IList<tblSNETMain> GetSNETResult()
    {
        IList<tblSNETMain> bindResult = (IList<tblSNETMain>)HttpContext.Current.Cache["result"];

        if (bindResult == null)
        {

            using (FrontierEntities data = new FrontierEntities())
            {
                IQueryable<tblSNETMain> result = data.tblSNETMains.Include("tblSNETTns").AsQueryable();

                if (SessionVars.RecordLocator != 0)
                {
                    result = result.Where(s => s.SNETMainId == SessionVars.RecordLocator);
                }
                if (!String.IsNullOrEmpty(SessionVars.PhoneNumber))
                {
                    result = result.Where(s => s.tblSNETTns.Where(t => t.Tn == SessionVars.PhoneNumber).Count() > 0);
                }
                if (!String.IsNullOrEmpty(SessionVars.TpvAgentId))
                {
                    result = result.Where(s => s.TpvAgentId == SessionVars.TpvAgentId);
                }
                if (!String.IsNullOrEmpty(SessionVars.SalesAgentId))
                {
                    result = result.Where(s => s.SalesAgentId == SessionVars.SalesAgentId);
                }
                if (!String.IsNullOrEmpty(SessionVars.Disposition))
                {
                    result = result.Where(s => s.Concern == SessionVars.Disposition);
                }
                if (SessionVars.StartDate != null && SessionVars.EndDate != null)
                {
                    result = result.Where(p => ((p.DateTime.Value >= SessionVars.StartDate.Value && p.DateTime.Value < SessionVars.EndDate.Value)));
                }

                bindResult = result.ToList();
                HttpContext.Current.Cache["result"] = bindResult;
            }

        }

        return bindResult;
    }

}