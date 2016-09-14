using System;
using System.ComponentModel.DataAnnotations;

namespace System.Data.DB2.EntityFramework.Migration.Test.Model01
{
    class Entity
    {
        public int Id { get; set; }
        [MaxLength(50)]
        public string Description { get; set; }

        public RelatedEntity RelatedEntity { get; set; }
    }


    class RelatedEntity
    {
        public int Id { get; set; }

        [MaxLength(50)]
        public string Description { get; set; }
    }


}
