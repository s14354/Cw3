using Cw3.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Cw3.DAL
{
    public class MockDbService : IDbService
    {
        private static IEnumerable<Student> _students;

        static MockDbService()
        {
            //_students = new List<Student>
            //{
            //    new Student{IdStudent = 1, FirstName = "aaa", LastName = "ddd"},
            //    new Student{IdStudent = 2, FirstName = "bbb", LastName = "eee"},
            //    new Student{IdStudent = 3, FirstName = "ccc", LastName = "fff"}
            //};
        }

        public IEnumerable<Student> GetStudents()
        {
            List<Student> students = new List<Student>();
            using (var con = new SqlConnection("Data Source=db-mssql;Initial Catalog=s14354;Integrated Security=True"))
            using (var com = new SqlCommand())
            {
                com.Connection = con;
                com.CommandText = "select * from student s left join enrollment e on e.IdEnrollment = s.IdEnrollment left join studies t on t.IdStudy = e.IdStudy";
                con.Open();
                var dr = com.ExecuteReader();
                while(dr.Read())
                {
                    var st = new Student();
                    st.FirstName = dr["FirstName"].ToString();
                    st.LastName = dr["LastName"].ToString();
                    st.BirthDate = dr["BirthDate"].ToString();
                    st.StudyName = dr["name"].ToString();
                    st.Semester = (int)dr["Semester"];
                    students.Add(st);
                }
            }
            return students;
        }
    }
}
