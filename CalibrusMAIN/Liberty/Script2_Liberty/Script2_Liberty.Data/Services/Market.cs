using Script2_Liberty.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Script2_Liberty.Data.Services
{
    public class Market
    {
        public List<ContractTerm> TestMarketUtilities(int? MarketProductId)
        {
            //var ContractTerms = new List<ContractTerm>();

            //using (LibertyDbContext context = new LibertyDbContext())
            //{
            //    ContractTerms = context.ContractTerms
            //                       .Join(context.ProductContractLinks,
            //                          ct => ct.ContractTermId,
            //                          pcl => pcl.ContractTermId,
            //                          (ct, pcl) => new { ct = ct, pcl = pcl })
            //                          .Where(pclAndct => pclAndct.ct.ContractTermId == pclAndct.pcl.ContractTermId)

            //       .AsEnumerable()
            //       .Select(t => new ContractTerm()
            //       {
            //           ContractTermId = t.ct.ContractTermId,
            //           MonthlyTerm = t.ct.MonthlyTerm
            //       }).ToList();
            //}

            List<ContractTerm> ContractTerms = new List<ContractTerm>();

            using (LibertyDbContext context = new LibertyDbContext())
            {
                ContractTerms = (from ct in context.ContractTerms
                                 join pcl in context.ProductContractLinks on ct.ContractTermId equals pcl.ContractTermId
                                 where pcl.MarketProductId == MarketProductId
                                 select ct).OrderBy(x => x.ContractTermId).ToList();

            }
            return ContractTerms;
          

        }
    }
}
