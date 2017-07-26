using System;

namespace ClearviewDailySalesBatchXLS
{
    internal class Record : IDisposable
    {
        //Class which holds the data we are going to report on
        public DateTime? CallDateTime;
        public string LdcCode;
        public string UtilityTypeName;
        public string PremiseTypeName;
        public string AccountNumber;
        public string Btn;
        public string AuthorizationFirstName;
        public string AuthorizationLastName;
        public string Relation;
        public string AccountFirstName;
        public string AccountLastName;
        public string CustomerNameKey;
        public string ServiceAddress;
        public string ServiceCity;
        public string ServiceState;
        public string ServiceZip;
        public string BillingAddress;
        public string BillingCity;
        public string BillingState;
        public string BillingZip;
        public string AgentId;
        public string ProgramCode;
        public string MainId;
        public string Email;
        public string MarketerCode;
        public string ParticipatingConsent;
        public string Dnis;
        public string Spanish;

        public Record(DateTime? callDateTime,
                        string ldcCode,
                         string utilityTypeName,
                         string premiseTypeName,
                         string accountNumber,
                         string btn,
                         string authorizationFirstName,
                         string authorizationLastName,
                         string relation,
                         string accountFirstName,
                         string accountLastName,
                         string customerNameKey,
                         string serviceAddress,
                         string serviceCity,
                         string serviceState,
                         string serviceZip,
                         string billingAddress,
                         string billingCity,
                         string billingState,
                         string billingZip,
                         string agentId,                        
                         string programCode,
                         string mainId,
                         string email,
                         string marketerCode,
                        string participatingConsent,
                        string dnis,
                        string spanish)
        {
            CallDateTime = callDateTime;
            LdcCode = ldcCode;
            UtilityTypeName = utilityTypeName;
            PremiseTypeName = premiseTypeName;
            AccountNumber = accountNumber;
            Btn = btn;
            AuthorizationFirstName = authorizationFirstName;
            AuthorizationLastName = authorizationLastName;
            Relation = relation;
            AccountFirstName = accountFirstName;
            AccountLastName = accountLastName;
            CustomerNameKey = customerNameKey;
            ServiceAddress = serviceAddress;
            ServiceCity = serviceCity;
            ServiceState = serviceState;
            ServiceZip = serviceZip;
            BillingAddress = billingAddress;
            BillingCity = billingCity;
            BillingState = billingState;
            BillingZip = billingZip;
            AgentId = agentId;            
            ProgramCode = programCode;
            MainId = mainId;
            Email = email;
            MarketerCode = marketerCode;
            ParticipatingConsent = participatingConsent;
            Dnis = dnis;
            Spanish = spanish;
        }

        private bool disposed = false;

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
        ~Record()
        {
            // Simply call Dispose(false).
            Dispose(false);
        }
    }
}