using System;
using System.Collections.Generic;

namespace Cw3.Models
{
    public partial class Student
    {
        public string IndexNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime BirthDate { get; set; }
        public int IdEnrollment { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }

        public virtual Enrollment IdEnrollmentNavigation { get; set; }
    }
}
