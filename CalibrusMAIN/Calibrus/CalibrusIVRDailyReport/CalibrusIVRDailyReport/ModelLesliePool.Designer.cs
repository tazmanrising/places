﻿//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.ComponentModel;
using System.Data.EntityClient;
using System.Data.Objects;
using System.Data.Objects.DataClasses;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Serialization;

[assembly: EdmSchemaAttribute()]
namespace CalibrusIVRDailyReport
{
    #region Contexts
    
    /// <summary>
    /// No Metadata Documentation available.
    /// </summary>
    public partial class LesliesPoolEntities : ObjectContext
    {
        #region Constructors
    
        /// <summary>
        /// Initializes a new LesliesPoolEntities object using the connection string found in the 'LesliesPoolEntities' section of the application configuration file.
        /// </summary>
        public LesliesPoolEntities() : base("name=LesliesPoolEntities", "LesliesPoolEntities")
        {
            this.ContextOptions.LazyLoadingEnabled = true;
            OnContextCreated();
        }
    
        /// <summary>
        /// Initialize a new LesliesPoolEntities object.
        /// </summary>
        public LesliesPoolEntities(string connectionString) : base(connectionString, "LesliesPoolEntities")
        {
            this.ContextOptions.LazyLoadingEnabled = true;
            OnContextCreated();
        }
    
        /// <summary>
        /// Initialize a new LesliesPoolEntities object.
        /// </summary>
        public LesliesPoolEntities(EntityConnection connection) : base(connection, "LesliesPoolEntities")
        {
            this.ContextOptions.LazyLoadingEnabled = true;
            OnContextCreated();
        }
    
        #endregion
    
        #region Partial Methods
    
        partial void OnContextCreated();
    
        #endregion
    
        #region ObjectSet Properties
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        public ObjectSet<tblMainLesliePool> tblMains
        {
            get
            {
                if ((_tblMains == null))
                {
                    _tblMains = base.CreateObjectSet<tblMainLesliePool>("tblMains");
                }
                return _tblMains;
            }
        }
        private ObjectSet<tblMainLesliePool> _tblMains;

        #endregion

        #region AddTo Methods
    
        /// <summary>
        /// Deprecated Method for adding a new object to the tblMains EntitySet. Consider using the .Add method of the associated ObjectSet&lt;T&gt; property instead.
        /// </summary>
        public void AddTotblMains(tblMainLesliePool tblMain)
        {
            base.AddObject("tblMains", tblMain);
        }

        #endregion

    }

    #endregion

    #region Entities
    
    /// <summary>
    /// No Metadata Documentation available.
    /// </summary>
    [EdmEntityTypeAttribute(NamespaceName="LesliesPoolModel", Name="tblMain")]
    [Serializable()]
    [DataContractAttribute(IsReference=true)]
    public partial class tblMainLesliePool : EntityObject
    {
        #region Factory Method
    
        /// <summary>
        /// Create a new tblMain object.
        /// </summary>
        /// <param name="mainId">Initial value of the MainId property.</param>
        public static tblMainLesliePool CreatetblMain(global::System.Int32 mainId)
        {
            tblMainLesliePool tblMain = new tblMainLesliePool();
            tblMain.MainId = mainId;
            return tblMain;
        }

        #endregion

        #region Primitive Properties
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=true, IsNullable=false)]
        [DataMemberAttribute()]
        public global::System.Int32 MainId
        {
            get
            {
                return _MainId;
            }
            set
            {
                if (_MainId != value)
                {
                    OnMainIdChanging(value);
                    ReportPropertyChanging("MainId");
                    _MainId = StructuralObject.SetValidValue(value);
                    ReportPropertyChanged("MainId");
                    OnMainIdChanged();
                }
            }
        }
        private global::System.Int32 _MainId;
        partial void OnMainIdChanging(global::System.Int32 value);
        partial void OnMainIdChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=true)]
        [DataMemberAttribute()]
        public Nullable<global::System.DateTime> CallDateTime
        {
            get
            {
                return _CallDateTime;
            }
            set
            {
                OnCallDateTimeChanging(value);
                ReportPropertyChanging("CallDateTime");
                _CallDateTime = StructuralObject.SetValidValue(value);
                ReportPropertyChanged("CallDateTime");
                OnCallDateTimeChanged();
            }
        }
        private Nullable<global::System.DateTime> _CallDateTime;
        partial void OnCallDateTimeChanging(Nullable<global::System.DateTime> value);
        partial void OnCallDateTimeChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=true)]
        [DataMemberAttribute()]
        public global::System.String Dnis
        {
            get
            {
                return _Dnis;
            }
            set
            {
                OnDnisChanging(value);
                ReportPropertyChanging("Dnis");
                _Dnis = StructuralObject.SetValidValue(value, true);
                ReportPropertyChanged("Dnis");
                OnDnisChanged();
            }
        }
        private global::System.String _Dnis;
        partial void OnDnisChanging(global::System.String value);
        partial void OnDnisChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=true)]
        [DataMemberAttribute()]
        public Nullable<global::System.Int32> CallLength
        {
            get
            {
                return _CallLength;
            }
            set
            {
                OnCallLengthChanging(value);
                ReportPropertyChanging("CallLength");
                _CallLength = StructuralObject.SetValidValue(value);
                ReportPropertyChanged("CallLength");
                OnCallLengthChanged();
            }
        }
        private Nullable<global::System.Int32> _CallLength;
        partial void OnCallLengthChanging(Nullable<global::System.Int32> value);
        partial void OnCallLengthChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=true)]
        [DataMemberAttribute()]
        public global::System.String PbxCallId
        {
            get
            {
                return _PbxCallId;
            }
            set
            {
                OnPbxCallIdChanging(value);
                ReportPropertyChanging("PbxCallId");
                _PbxCallId = StructuralObject.SetValidValue(value, true);
                ReportPropertyChanged("PbxCallId");
                OnPbxCallIdChanged();
            }
        }
        private global::System.String _PbxCallId;
        partial void OnPbxCallIdChanging(global::System.String value);
        partial void OnPbxCallIdChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=true)]
        [DataMemberAttribute()]
        public global::System.String Ani
        {
            get
            {
                return _Ani;
            }
            set
            {
                OnAniChanging(value);
                ReportPropertyChanging("Ani");
                _Ani = StructuralObject.SetValidValue(value, true);
                ReportPropertyChanged("Ani");
                OnAniChanged();
            }
        }
        private global::System.String _Ani;
        partial void OnAniChanging(global::System.String value);
        partial void OnAniChanged();

        #endregion

    
    }

    #endregion

    
}