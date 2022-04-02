using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace LCDSArtGallery.Models
{
    public class Artist
    {
        [Key]
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public int ContactNumber { get; set; }
        public string Address { get; set; }
        public string Overview { get; set; }

        public string Biography { get; set; }
        /*public int productId { get; set; }
        [ForeignKey("productID")]*/

    }
}
