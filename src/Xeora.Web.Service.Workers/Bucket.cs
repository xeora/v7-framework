using System;

namespace Xeora.Web.Service.Workers
{
    public class Bucket
    {
        private readonly Func<ActionContainer, bool> _AddHandler;
        private readonly Action _CompletedHandler;
        private readonly Action _ReportRequestHandler;

        internal Bucket(string trackingId, Func<ActionContainer, bool> addHandler, Action completedHandler, Action reportRequestHandler)
        {
            this.TrackingId = trackingId;
            this._AddHandler = addHandler;
            this._CompletedHandler = completedHandler;
            this._ReportRequestHandler = reportRequestHandler;
        }

        public string TrackingId { get; }

        public Bulk New() =>
            new Bulk(this._AddHandler);
        
        public void Completed() =>
            this._CompletedHandler.Invoke();

        internal void PrintBucketDetails() =>
            this._ReportRequestHandler.Invoke();
    }
}
