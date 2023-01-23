
// This API is created using Entity Framework code first approach

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CRUD1.Model
{
    public class users
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [StringLength(150)]
        public string Name { get; set; }

        public int Age { get; set; }

        [StringLength(150)]
        public string Gender { get; set; }

        [StringLength(250)]
        public string Email { get; set; }

        public int IsMarried { get; set; }

        public string Country { get; set; }

        public DateTime Djoin { get; set; }

        [StringLength(500)]
        public string Details { get; set; }

        public string ImgPath { get; set; }
    }
}
