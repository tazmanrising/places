using Calibrus.SparkPortal.DataAccess.Infrastructure;
using System;
using System.Collections.Generic;

namespace Calibrus.SparkPortal.ViewModel
{
    public class RateIndexViewModel
    {
        public RateIndexViewModel(int? id)
        {
            List<Program> programs = id.HasValue ? Business.AppLogic.GetPrograms(false, id.Value) : Business.AppLogic.GetPrograms(false);
            Rates = new List<RateItem>();
            programs.ForEach(v =>
                        Rates.Add(new RateItem()
                        {
                            Id = v.ProgramId,
                            ProgramCode = v.ProgramCode,
                            ProgramName = v.ProgramName,
                            EffectiveStartDate = v.EffectiveStartDate,
                            EffectiveEndDate = v.EffectiveEndDate,
                            Rate = v.Rate
                        }
                    )
            );
        }

        public List<RateItem> Rates { get; set; }

        public class RateItem
        {
            public int Id { get; set; }

            public string ProgramCode { get; set; }

            public string ProgramName { get; set; }

            public DateTime EffectiveStartDate { get; set; }

            public DateTime EffectiveEndDate { get; set; }

            public decimal Rate { get; set; }

            public decimal Msf { get; set; }

            public decimal Etf { get; set; }
        }
    
    }
}