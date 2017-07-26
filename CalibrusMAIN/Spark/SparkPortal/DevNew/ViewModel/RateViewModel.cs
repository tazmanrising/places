using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Calibrus.SparkPortal.DataAccess.Infrastructure;

namespace Calibrus.SparkPortal.ViewModel
{
    public class RateViewModel : IValidatableObject
    {
        public RateViewModel()
        {
            SetDropdowns();

        }

        public RateViewModel(int rateId)
        {
            Program program = Business.AppLogic.GetProgram(rateId);

            Id = program.ProgramId;
            ProgramCode = program.ProgramCode;
            ProgramName = program.ProgramName;
            ProgramDescription = program.ProgramDescription;
            DefaultPricingPlanDescription = program.DefaultPricingPlanDescription;
            EffectiveStartDate = program.EffectiveStartDate.Date;
            EffectiveEndDate = program.EffectiveEndDate.Date;
            Msf = program.Msf;
            Etf = program.Etf;
            Rate = program.Rate;
            PromotionalCode = program.PromotionalCode;
            UnitOfMeasureId = program.UnitOfMeasureId;
            Term = program.Term;
            UtilityTypeId = program.UtilityTypeId;
            PremiseTypeId = program.PremiseTypeId;
            State = program.State;
            UtilityId = program.UtilityId;
            AccountNumberTypeId = program.AccountNumberTypeId;
            AccountNumberLength = program.AccountNumberLength;
            AccountNumberFixedLength = program.AccountNumberFixedLength.GetValueOrDefault();
			MeterNumber = program.MeterNumber.GetValueOrDefault();
            MeterNumberLength = program.MeterNumberLength;
            RescindBy = program.RescindBy;
            Hefpa = program.Hefpa.GetValueOrDefault();
            Vendor = program.Vendor;
            SalesChannel = program.SalesChannel;
	        RateVerbiage = program.RateVerbiage;
	        CancellationVerbiage = program.CancellationVerbiage;
	        RateVerbiageSpanish = program.RateVerbiageSpanish;
	        CancellationVerbiageSpanish = program.CancellationVerbiageSpanish;
            BrandId = program.BrandId;
            UpdatedBy = program.UpdatedBy;
            UpdatedDateTime = program.UpdatedDateTime;
            ServiceReference = program.ServiceReference;
            CreditCheck = program.CreditCheck;

	        SelectedVendors = program.ProgramVendors.Select(x => x.VendorId).ToList();
            SelectedSalesChannels = program.SalesChannelPrograms.Select(x => x.SalesChannelId.GetValueOrDefault()).ToList();
           
            SetDropdowns();
        }

        public RateViewModel(RateViewModel model)
        {
        }

        public void SetDropdowns()
        {
            UnitsOfMeasure = GetActiveUnitOfMeasure();
            UtilityTypes = GetActiveUtilityTypes();
            PremiseTypes = GetActivePremiseTypes();
            States = GetStates();
            AccountNumberTypes = GetAccountNumberTypes();
	        Vendors = GetVendors();
	        Utilities = GetActiveUtilities();
            Brands = GetBrands();
            SalesChannels = GetSalesChannels();
        }

        public int? Id { get; set; }

        public string UpdatedBy { get; set; }

        public DateTime? UpdatedDateTime { get; set; }

        [Required]
        [DisplayName("Program Code")]
        public string ProgramCode { get; set; }

        [Required]
        [DisplayName("Program Name")]
        public string ProgramName { get; set; }

        [DisplayName("Program Description")]
        public string ProgramDescription { get; set; }

        [DisplayName("Default Pricing Plan Description")]
        public string DefaultPricingPlanDescription { get; set; }

        [Required]
        [DisplayName("Effective Start Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime? EffectiveStartDate { get; set; }

        [Required]
        [DisplayName("Effective End Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime? EffectiveEndDate { get; set; }

        [DisplayName("MSF")]
        public decimal? Msf { get; set; }

        [DisplayName("ETF")]
        public decimal? Etf { get; set; }

        [Required]
        [DisplayName("Rate")]
        public decimal? Rate { get; set; }

        [DisplayName("Promotional Code")]
        public string PromotionalCode { get; set; }

        [Required]
        [DisplayName("Unit of Measure")]
        public int? UnitOfMeasureId { get; set; }

        public List<UnitOfMeasure> UnitsOfMeasure { get; private set; }

        [DisplayName("Term (months)")]
        public int? Term { get; set; }

        [Required]
        [DisplayName("Utility Type")]
        public int? UtilityTypeId { get; set; }

        public List<UtilityType> UtilityTypes { get; private set; }

        [Required]
        [DisplayName("Premise Type")]
        public int? PremiseTypeId { get; set; }

        public List<PremiseType> PremiseTypes { get; private set; }

        [Required]
        [DisplayName("State")]
        public string State { get; set; }

        public List<State> States { get; private set; }
		
		[Required]
        [DisplayName("Utility")]
        public int UtilityId { get; set; }

		public List<Utility> Utilities { get; private set; }
			
		[Required]
        [DisplayName("Account Number Type")]
        public int AccountNumberTypeId { get; set; }

        public List<AccountNumberType> AccountNumberTypes { get; private set; }

		[Required(ErrorMessage="A Vendor is required.")]
		[DisplayName("Selected Vendors")]
		public List<int> SelectedVendors { get; set; }

        [Required(ErrorMessage = "A Sales Channel is required.")]
        [DisplayName("Selected Sales Channels")]
        public List<int> SelectedSalesChannels { get; set; }

        public List<Vendor> Vendors { get; private set; }

        public List<SalesChannel> SalesChannels { get; private set; }

        public List<Brand> Brands { get; private set; }

        [Required]
        [DisplayName("Account Number Length")]
        public int? AccountNumberLength { get; set; }

        [DisplayName("Account Number Fixed Length")]
        public bool AccountNumberFixedLength { get; set; }

        [DisplayName("Meter Number")]
        public bool MeterNumber { get; set; }

        [DisplayName("Meter Number Length")]
        public int? MeterNumberLength { get; set; }

        [DisplayName("Service Reference Number")]
        public bool ServiceReference { get; set; }

        [DisplayName("Rescind By (days)")]
        public int? RescindBy { get; set; }

        [DisplayName("HEFPA")]
        public bool Hefpa { get; set; }

        [Required]
        [DisplayName("Brand")]
        public int BrandId { get; set; }

        [DisplayName("Rate Verbiage")]
		public string RateVerbiage { get; set; }

		[DisplayName("Cancellation Verbiage")]
		public string CancellationVerbiage { get; set; }

		[DisplayName("Rate Verbiage (Spanish)")]
		public string RateVerbiageSpanish { get; set; }

		[DisplayName("Cancellation Verbiage (Spanish)")]
		public string CancellationVerbiageSpanish { get; set; }

        [DisplayName("Vendor")]
        public string Vendor { get; set; }

        [DisplayName("Sales Channel")]
        public string SalesChannel { get; set; }

        [DisplayName("Credit Check")]
        public bool CreditCheck { get; set; }

        private List<UnitOfMeasure> GetActiveUnitOfMeasure()
        {
            return Business.AppLogic.GetActiveUnitOfMeasures();
        }

        private List<UtilityType> GetActiveUtilityTypes()
        {
            return Business.AppLogic.GetActiveUtilityTypes();
        }

		private List<Utility> GetActiveUtilities()
		{
			return Business.AppLogic.GetActiveUtilities();
		}

        private List<PremiseType> GetActivePremiseTypes()
        {
            return Business.AppLogic.GetActivePremiseTypes();
        }

        private List<State> GetStates()
        {
            return Business.AppLogic.GetStates();
        }

        private List<AccountNumberType> GetAccountNumberTypes()
        {
            return Business.AppLogic.GetActiveAccountNumberTypes();
        }

		private List<Vendor> GetVendors()
		{
			return Business.AppLogic.GetVendors(true);
		}

        private List<SalesChannel> GetSalesChannels()
        {
            return Business.AppLogic.GetActiveSalesChannels();
        }

        private List<Brand> GetBrands()
        {
            return Business.AppLogic.GetBrands(true);
        }

        public void SaveViewModel()
        {
            //TODO: make sure loggedInUser is a member to the vendor the user id being created/edited in

			Program submittedProgram = new Program
	            {
		            ProgramCode = this.ProgramCode,
		            ProgramName = this.ProgramName,
                    ProgramDescription = this.ProgramDescription,
                    DefaultPricingPlanDescription = this.DefaultPricingPlanDescription,
		            EffectiveStartDate = this.EffectiveStartDate.Value,
		            EffectiveEndDate = this.EffectiveEndDate.Value,
		            Msf = this.Msf,
		            Etf = this.Etf,
		            Rate = this.Rate.GetValueOrDefault(0),
		            PromotionalCode = this.PromotionalCode,
		            UnitOfMeasureId = this.UnitOfMeasureId.Value,
		            Term = this.Term,
		            UtilityTypeId = this.UtilityTypeId,
		            PremiseTypeId = this.PremiseTypeId,
		            State = this.State,
		            UtilityId = this.UtilityId,
		            AccountNumberTypeId = this.AccountNumberTypeId,
		            AccountNumberLength = this.AccountNumberLength.Value,
		            AccountNumberFixedLength = this.AccountNumberFixedLength,
		            MeterNumber = this.MeterNumber,
                    MeterNumberLength = this.MeterNumberLength,
		            RescindBy = this.RescindBy,
		            Hefpa = this.Hefpa,
		            Vendor = this.Vendor,
		            SalesChannel = this.SalesChannel,
					RateVerbiage = this.RateVerbiage,
					CancellationVerbiage = this.CancellationVerbiage,
					RateVerbiageSpanish = this.RateVerbiageSpanish,
					CancellationVerbiageSpanish = this.CancellationVerbiageSpanish,
                    BrandId = this.BrandId,
                    UpdatedBy = this.UpdatedBy,
                    UpdatedDateTime = DateTime.Now,
                    ServiceReference = this.ServiceReference,
                    CreditCheck = this.CreditCheck
	            };			
			 

            if (Id.HasValue && Id > 0) //edit
            {
	            submittedProgram.ProgramId = this.Id.Value;
				foreach (int vendorId in this.SelectedVendors)
				{
					submittedProgram.ProgramVendors.Add(new ProgramVendor {ProgramId = submittedProgram.ProgramId, VendorId = vendorId, CreatedBy = UpdatedBy, CreatedDateTime = DateTime.Now });
				}
                foreach( int SalesChannelId in this.SelectedSalesChannels)
                {
                    submittedProgram.SalesChannelPrograms.Add(new SalesChannelProgram { ProgramId = submittedProgram.ProgramId, SalesChannelId = SalesChannelId });
                }
				Id = Business.AppLogic.UpdateRate(submittedProgram);
            }
            else //new
            {

                foreach (int SalesChannelId in this.SelectedSalesChannels)
                {
                    submittedProgram.SalesChannelPrograms.Add(new SalesChannelProgram { ProgramId = submittedProgram.ProgramId, SalesChannelId = SalesChannelId });
                }
                foreach (int vendorId in this.SelectedVendors)
				{
					submittedProgram.ProgramVendors.Add(new ProgramVendor {VendorId = vendorId, CreatedBy = UpdatedBy, CreatedDateTime = DateTime.Now });
				}
				Id = Business.AppLogic.CreateRate(submittedProgram);
            }
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Business.AppLogic.ProgramCodeExists(Id.GetValueOrDefault(0), ProgramCode, EffectiveStartDate.Value, EffectiveEndDate.Value))
            {
                yield return new ValidationResult("There is already an active program with this code.", new[] { "ProgramCode" });
            }

            if (MeterNumber == true && !MeterNumberLength.HasValue)
            {
                yield return new ValidationResult("Meter Number Length is Required.", new[] { "MeterNumberLength" });
            }

            if (MeterNumberLength.HasValue && MeterNumberLength.GetValueOrDefault(0) < 5)
            {
                yield return new ValidationResult("Meter Number Length must be at least 5.", new[] { "MeterNumberLength" });
            }
        }
    }
}