//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Crud_Operation.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class UserPermission
    {
        public byte PermissionId { get; set; }
        public int UserId { get; set; }
        public byte RoleId { get; set; }
    
        public virtual UserRoleType UserRoleType { get; set; }
        public virtual User User { get; set; }
    }
}