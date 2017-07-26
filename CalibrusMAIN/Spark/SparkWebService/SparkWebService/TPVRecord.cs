using System;

namespace SparkWebService
{
    public class TPVRecord : IDisposable
    {
        public int CalibrusRecordLocator { get; set; }

        public DateTime? CallDateTime { get; set; }

        public DateTime? WebDateTime { get; set; }

        public string Verified { get; set; }

        public string Concern { get; set; }

        public string ConcernCode { get; set; }

        public string TpvAgentName { get; set; }

        public string TpvAgentId { get; set; }

        public string AgentID { get; set; }

        public string AgentFirstName { get; set; }

        public string AgentLastName { get; set; }

        public string Email { get; set; }

        public string AuthorizationFirstName { get; set; }

        public string AuthorizationLastName { get; set; }

        public string AccountFirstName { get; set; }

        public string AccountLastName { get; set; }

        public string Relation { get; set; }

        public string Btn { get; set; }

        public string AccountNumber { get; set; }

        public string CustomerNameKey { get; set; }

        public string ServiceAddress { get; set; }

        public string ServiceCity { get; set; }

        public string ServiceCounty { get; set; }

        public string ServiceState { get; set; }

        public string ServiceZip { get; set; }

        public string BillingAddress { get; set; }

        public string BillingCity { get; set; }

        public string BillingCounty { get; set; }

        public string BillingState { get; set; }

        public string BillingZip { get; set; }

        public string ProgramCode { get; set; }

        public string ProgramName { get; set; }

        public decimal? MSF { get; set; }

        public decimal? ETF { get; set; }

        public decimal? Rate { get; set; }

        public decimal? Term { get; set; }

        public string UtilityType { get; set; }

        public string PremiseType { get; set; }

        public string State { get; set; }

        public string LdcCode { get; set; }

        public string AccountNumberType { get; set; }

        public string BillingFirstName { get; set; }

        public string BillingLastName { get; set; }

        private bool disposed = false;

        public TPVRecord() { }
        public TPVRecord(int calibrusRecordLocator,
                        DateTime? callDateTime,
                        DateTime? webDateTime,
                        string verified,
                        string concern,
                        string concernCode,
                        string tpvAgentName,
                        string tpvAgentId,
                        string agentID,
                        string agentFirstName,
                        string agentLastName,
                        string email,
                        string authorizationFirstName,
                        string authorizationLastName,
                        string accountFirstName,
                        string accountLastName,
                        string relation,
                        string btn,
                        string accountNumber,
                        string customerNameKey,
                        string serviceAddress,
                        string serviceCity,
                        string serviceCounty,
                        string serviceState,
                        string serviceZip,
                        string billingAddress,
                        string billingCity,
                        string billingCounty,
                        string billingState,
                        string billingZip,
                        string programCode,
                        string programName,
                        decimal? msf,
                        decimal? etf,
                        decimal? rate,
                        decimal? term,
                        string utilityType,
                        string premiseType,
                        string state,
                        string ldcCode,
                        string accountNumberType,
                        string billingFirstName,
                        string billingLastName)
        {
            CalibrusRecordLocator = calibrusRecordLocator;
            CallDateTime = callDateTime;
            WebDateTime = webDateTime;
            Verified = verified;
            Concern = concern;
            ConcernCode = concernCode;
            TpvAgentName = tpvAgentName;
            TpvAgentId = tpvAgentId;
            AgentID = agentID;
            AgentFirstName = agentFirstName;
            AgentLastName = AgentLastName;
            Email = email;
            AuthorizationFirstName = authorizationFirstName;
            AuthorizationLastName = authorizationLastName;
            AccountFirstName = accountFirstName;
            AccountLastName = accountLastName;
            Relation = relation;
            Btn = btn;
            AccountNumber = accountNumber;
            CustomerNameKey = customerNameKey;
            ServiceAddress = serviceAddress;
            ServiceCity = serviceCity;
            ServiceCounty = serviceCounty;
            ServiceState = serviceState;
            ServiceZip = serviceZip;
            BillingAddress = billingAddress;
            BillingCity = billingCity;
            BillingCounty = billingCounty;
            BillingState = billingState;
            BillingZip = billingZip;
            ProgramCode = programCode;
            ProgramName = programName;
            MSF = msf;
            ETF = etf;
            Rate = rate;
            Term = term;
            UtilityType = utilityType;
            PremiseType = premiseType;
            State = state;
            LdcCode = ldcCode;
            AccountNumberType = accountNumberType;
            BillingFirstName = billingFirstName;
            BillingLastName = billingLastName;
        }

        //Implement IDisposable.
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    // Free other state (managed objects).
                }
                // Free your own state (unmanaged objects).
                // Set large fields to null.
                disposed = true;
            }
        }

        // Use C# destructor syntax for finalization code.
        ~TPVRecord()
        {
            // Simply call Dispose(false).
            Dispose(false);
        }
    }
}