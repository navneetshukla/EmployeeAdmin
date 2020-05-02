using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace userDetailsMVCWebApp.Models
{
    public class Employee
    {
        [Key]
        public int EmployeeId { get; set; }
        [Column(TypeName ="nvarchar(250)")]
        [Required(ErrorMessage ="This field is required")]
        [Display(Name = "Employee Name")]
        public string  EmployeeName { get; set; }

        [Column(TypeName = "nvarchar(250)")]
        [Required(ErrorMessage = "This field is required")]
        [Display(Name = "Employee Email ID")]
        public string EmailId { get; set; }

        [Display(Name = "D.O.B")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime DOB { get; set; }

        [Column(TypeName = "varchar(10)")]
        public string Gender { get; set; }

        [Column(TypeName = "varchar(20)")]
        public string MarritalStatus { get; set; }

        [Display(Name = "Date of Anniversary")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Anniversary { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        [Required(ErrorMessage = "Please upload an image first")]
        public string fileName { get; set; }
    }
}
