using Cw3.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cw3.DAL
{
    public interface IDbService
    {
        IEnumerable<Student> GetStudents(String orderBy);
        Student GetStudent(int id);
        Study GetStudyByName(string name);
        Enrollment SetFirstEnrollment(Study st, Student stu);
        Enrollment GetEnrollment(string name, int semester);
        Enrollment Promote(string study, int semester);
        bool CheckIndex(string index);
    }
}
