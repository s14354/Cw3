using Cw3.Models;
using Cw3.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cw3.DAL
{
    public interface IDbService
    {
        IEnumerable<Student> GetStudents();
        Student GetStudent(int id);
        Studies GetStudyByName(string name);
        Enrollment SetFirstEnrollment(int study, Student stu);
        Enrollment GetEnrollment(string name, int semester);
        void Promote(string study, int semester);
        bool CheckIndex(string index);
        LoginResponseDTO GetRole(LoginRequestDTO request);
        LoginResponseDTO GetRole(string refToken);
        bool UpdateStudent(string fname, string index);
        bool DeleteStudent(string index);
    }
}
