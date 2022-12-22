using System.ComponentModel.DataAnnotations;
using System;

namespace FYPBackEnd.Data.Entities
{
    public class BaseEntity
    {
        public Guid id { get; set; }
        public DateTime? CreationDate { get; set; }
        public DateTime? LastModifiedDate { get; set; }
         
         
    }
}
