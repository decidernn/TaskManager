//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Task_Manager
{
    using System;
    using System.Collections.Generic;
    
    public partial class Members
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Members()
        {
            this.MemberSubtask = new HashSet<MemberSubtask>();
        }
    
        public int Id { get; set; }
        public int IdRole { get; set; }
        public int IdTeam { get; set; }
        public int IdUser { get; set; }
    
        public virtual Roles Roles { get; set; }
        public virtual Teams Teams { get; set; }
        public virtual User User { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MemberSubtask> MemberSubtask { get; set; }
    }
}