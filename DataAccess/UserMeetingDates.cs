//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DataAccess
{
    #pragma warning disable 1573
    using System;
    using System.Collections.Generic;
    
    public partial class UserMeetingDates
    {
        public int Id { get; set; }
        public System.DateTime Date { get; set; }
        public int MeetingId { get; set; }
        public bool IsAvaliable { get; set; }
        public Nullable<int> UserProfileId { get; set; }
        public Nullable<int> CachedUserNamesId { get; set; }
    
        public virtual Meeting Meeting { get; set; }
    }
}
