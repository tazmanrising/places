using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConstellationDTDAlertSend
{
    class AlertRecord : IDisposable
    {
        public int? Id { get; set; }
        public DateTime? AlertDateTime { get; set; }
        public int? AlertTypeId { get; set; }
        public int? EnrollmentId { get; set; }
        public int? MainId { get; set; }
        public int? UserId { get; set; }
        public string AlertType { get; set; }
        public string Template { get; set; }
        public string Subject { get; set; }
        public string ToList { get; set; }
        public string CCList { get; set; }

        public AlertRecord(int? id, DateTime? alertDateTime, int? alertTypeId, int? enrollmentId, int? mainId, int? userId, string alertType, string template, string subject, string toList, string ccList)
        {
            Id = id;
            AlertDateTime = alertDateTime;
            AlertTypeId = alertTypeId;
            EnrollmentId = enrollmentId;
            MainId = mainId;
            UserId = userId;
            AlertType = alertType;
            Template = template;
            Subject = subject;
            ToList = toList;
            CCList = ccList;
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
        ~AlertRecord()
        {
            // Simply call Dispose(false).
            Dispose(false);
        }
    }
}
