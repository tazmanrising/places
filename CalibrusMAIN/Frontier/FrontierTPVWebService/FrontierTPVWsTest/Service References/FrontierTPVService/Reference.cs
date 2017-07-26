﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18408
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace FrontierTPVWsTest.FrontierTPVService {
    using System.Data;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(Namespace="http://ws.calibrus.com/FrontierTPVWebService", ConfigurationName="FrontierTPVService.FrontierTPVWSSoap")]
    public interface FrontierTPVWSSoap {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://ws.calibrus.com/FrontierTPVWebService/SubmitOrder", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        int SubmitOrder(FrontierTPVWsTest.FrontierTPVService.Account account);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://ws.calibrus.com/FrontierTPVWebService/RetrieveDataTelephoneNumber", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        System.Data.DataSet RetrieveDataTelephoneNumber(string PhoneNumber);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://ws.calibrus.com/FrontierTPVWebService/RetrieveDataRecordLocator", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        System.Data.DataSet RetrieveDataRecordLocator(string RecordLocator);
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.18408")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://ws.calibrus.com/FrontierTPVWebService")]
    public partial class Account : object, System.ComponentModel.INotifyPropertyChanged {
        
        private string salesAgentIdField;
        
        private string stateField;
        
        private string custFirstNameField;
        
        private string custLastNameField;
        
        private string decisionMakerField;
        
        private string companyNameField;
        
        private string productField;
        
        private bool businessField;
        
        private PhoneNumber[] phoneNumbersField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=0)]
        public string SalesAgentId {
            get {
                return this.salesAgentIdField;
            }
            set {
                this.salesAgentIdField = value;
                this.RaisePropertyChanged("SalesAgentId");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=1)]
        public string State {
            get {
                return this.stateField;
            }
            set {
                this.stateField = value;
                this.RaisePropertyChanged("State");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=2)]
        public string CustFirstName {
            get {
                return this.custFirstNameField;
            }
            set {
                this.custFirstNameField = value;
                this.RaisePropertyChanged("CustFirstName");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=3)]
        public string CustLastName {
            get {
                return this.custLastNameField;
            }
            set {
                this.custLastNameField = value;
                this.RaisePropertyChanged("CustLastName");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=4)]
        public string DecisionMaker {
            get {
                return this.decisionMakerField;
            }
            set {
                this.decisionMakerField = value;
                this.RaisePropertyChanged("DecisionMaker");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=5)]
        public string CompanyName {
            get {
                return this.companyNameField;
            }
            set {
                this.companyNameField = value;
                this.RaisePropertyChanged("CompanyName");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=6)]
        public string Product {
            get {
                return this.productField;
            }
            set {
                this.productField = value;
                this.RaisePropertyChanged("Product");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=7)]
        public bool Business {
            get {
                return this.businessField;
            }
            set {
                this.businessField = value;
                this.RaisePropertyChanged("Business");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlArrayAttribute(Order=8)]
        public PhoneNumber[] PhoneNumbers {
            get {
                return this.phoneNumbersField;
            }
            set {
                this.phoneNumbersField = value;
                this.RaisePropertyChanged("PhoneNumbers");
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.18408")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://ws.calibrus.com/FrontierTPVWebService")]
    public partial class PhoneNumber : object, System.ComponentModel.INotifyPropertyChanged {
        
        private string tnField;
        
        private bool pLOCChangeField;
        
        private bool pLOCFreezeField;
        
        private bool iLPIntraField;
        
        private bool iLPIntraFreezeField;
        
        private bool pICInterField;
        
        private bool pICInterFreezeField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=0)]
        public string Tn {
            get {
                return this.tnField;
            }
            set {
                this.tnField = value;
                this.RaisePropertyChanged("Tn");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=1)]
        public bool PLOCChange {
            get {
                return this.pLOCChangeField;
            }
            set {
                this.pLOCChangeField = value;
                this.RaisePropertyChanged("PLOCChange");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=2)]
        public bool PLOCFreeze {
            get {
                return this.pLOCFreezeField;
            }
            set {
                this.pLOCFreezeField = value;
                this.RaisePropertyChanged("PLOCFreeze");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=3)]
        public bool ILPIntra {
            get {
                return this.iLPIntraField;
            }
            set {
                this.iLPIntraField = value;
                this.RaisePropertyChanged("ILPIntra");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=4)]
        public bool ILPIntraFreeze {
            get {
                return this.iLPIntraFreezeField;
            }
            set {
                this.iLPIntraFreezeField = value;
                this.RaisePropertyChanged("ILPIntraFreeze");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=5)]
        public bool PICInter {
            get {
                return this.pICInterField;
            }
            set {
                this.pICInterField = value;
                this.RaisePropertyChanged("PICInter");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=6)]
        public bool PICInterFreeze {
            get {
                return this.pICInterFreezeField;
            }
            set {
                this.pICInterFreezeField = value;
                this.RaisePropertyChanged("PICInterFreeze");
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface FrontierTPVWSSoapChannel : FrontierTPVWsTest.FrontierTPVService.FrontierTPVWSSoap, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class FrontierTPVWSSoapClient : System.ServiceModel.ClientBase<FrontierTPVWsTest.FrontierTPVService.FrontierTPVWSSoap>, FrontierTPVWsTest.FrontierTPVService.FrontierTPVWSSoap {
        
        public FrontierTPVWSSoapClient() {
        }
        
        public FrontierTPVWSSoapClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public FrontierTPVWSSoapClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public FrontierTPVWSSoapClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public FrontierTPVWSSoapClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public int SubmitOrder(FrontierTPVWsTest.FrontierTPVService.Account account) {
            return base.Channel.SubmitOrder(account);
        }
        
        public System.Data.DataSet RetrieveDataTelephoneNumber(string PhoneNumber) {
            return base.Channel.RetrieveDataTelephoneNumber(PhoneNumber);
        }
        
        public System.Data.DataSet RetrieveDataRecordLocator(string RecordLocator) {
            return base.Channel.RetrieveDataRecordLocator(RecordLocator);
        }
    }
}