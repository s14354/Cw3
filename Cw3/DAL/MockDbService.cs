using Cw3.Models;
using Cw3.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Text;
using Microsoft.EntityFrameworkCore;

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

            var db = new s14354Context();
            var students = db.Student.ToList();
            return students;
        }

        public bool UpdateStudent(string fname, string index)
        {
            var db = new s14354Context();

            var s = new Student
            {
                IndexNumber = index,
                FirstName = fname
            };
            db.Attach(s);

            db.Entry(s).Property("FirstName").IsModified = true;
            db.SaveChanges();
            return true;
        }

        public bool DeleteStudent(string index)
        {
            var db = new s14354Context();
            var s = new Student
            {
                IndexNumber = index
            };
            db.Attach(s);
            db.Remove(s);
            return true;
        }

        public Student GetStudent(int ID)
        {
            using (var con = new SqlConnection("Data Source=db-mssql;Initial Catalog=s14354;Integrated Security=True"))
            using (var com = new SqlCommand())
            {
                com.Connection = con;
                com.CommandText = "select * from student where IdStudent = @id";
                com.Parameters.AddWithValue("id", ID);
                con.Open();
                var dr = com.ExecuteReader();
                while (dr.Read())
                {
                    var st = new Student();
                    st.FirstName = dr["FirstName"].ToString();
                    st.LastName = dr["LastName"].ToString();
                    st.BirthDate = Convert.ToDateTime(dr["BirthDate"].ToString());
                    return st;
                }
                return null;
            }
        }

        public Studies GetStudyByName(string name)
        {
            using (var con = new SqlConnection("Data Source=db-mssql;Initial Catalog=s14354;Integrated Security=True"))
            using (var com = new SqlCommand())
            {
                com.Connection = con;
                com.CommandText = "select * from Studies where Name = @name";
                com.Parameters.AddWithValue("name", name);
                con.Open();
                var dr = com.ExecuteReader();
                while (dr.Read())
                {
                    var st = new Studies();
                    st.IdStudy = (int)dr["IdStudy"];
                    st.Name = dr["Name"].ToString();
                    return st;
                }
                return null;
            }
        }

        public Enrollment GetFirstEnrollment(Studies st)
        {
            using (var con = new SqlConnection("Data Source=db-mssql;Initial Catalog=s14354;Integrated Security=True"))
            using (var com = new SqlCommand())
            {
                com.Connection = con;
                com.CommandText = "select * from Enrollment where IdStudy = @IdStudy and Semester = 1";
                com.Parameters.AddWithValue("IdStudy", st.IdStudy);
                con.Open();
                var dr = com.ExecuteReader();
                while (dr.Read())
                {
                    var en = new Enrollment();
                    en.IdEnrollment = (int)dr["IdEnrollment"];
                    en.Semester = (int)dr["Semester"];
                    en.IdStudy = (int)dr["IdStudy"];
                    en.StartDate = Convert.ToDateTime(dr["StartDate"].ToString());
                    return en;
                }
                return null;
            }
        }

        public Enrollment SetFirstEnrollment(int st, Student stu)
        {

            var db = new s14354Context();

            var en = db.Enrollment.Where(en => en.Semester == 1 && en.IdStudy == st).First();

            stu.IdEnrollment = en.IdEnrollment;
            db.Attach(stu);
            db.Entry(stu).State = EntityState.Added;
            db.SaveChanges();

            return en;
        }

        public Enrollment GetEnrollment(String name, int sem)
        {
            using (var con = new SqlConnection("Data Source=db-mssql;Initial Catalog=s14354;Integrated Security=True"))
            using (var com = new SqlCommand())
            {
                com.Connection = con;
                com.CommandText = "select e.* from Enrollment e left join Studies s on e.IdStudy = s.IdStudy where Name = @name and Semester = @semester";
                com.Parameters.AddWithValue("Name", name);
                com.Parameters.AddWithValue("semester", sem);
                con.Open();
                var dr = com.ExecuteReader();
                while (dr.Read())
                {
                    var en = new Enrollment();
                    en.IdEnrollment = (int)dr["IdEnrollment"];
                    en.Semester = (int)dr["Semester"];
                    en.IdStudy = (int)dr["IdStudy"];
                    en.StartDate = Convert.ToDateTime(dr["StartDate"].ToString());
                    return en;
                }
                return null;
            }
        }

        public void Promote(String study, int sem)
        {

            var db = new s14354Context();

            var students = db.Database.ExecuteSqlRaw("execute spPromote @study,@sem", study, sem);

            using (var con = new SqlConnection("Data Source=db-mssql;Initial Catalog=s14354;Integrated Security=True"))
            using (var com = new SqlCommand("spPromote", con));

        }

        public bool CheckIndex(String index)
        {
            using (var con = new SqlConnection("Data Source=db-mssql;Initial Catalog=s14354;Integrated Security=True"))
            using (var com = new SqlCommand())
            {
                com.Connection = con;
                com.CommandText = "select IndexNumber from Student where IndexNumber = @index";
                com.Parameters.AddWithValue("index", index);
                con.Open();
                var dr = com.ExecuteReader();
                while (dr.Read())
                {
                    var en = new Enrollment();
                    en.IdEnrollment = (int)dr["IdEnrollment"];
                    en.Semester = (int)dr["Semester"];
                    en.IdStudy = (int)dr["IdStudy"];
                    if (String.IsNullOrEmpty(dr["IndexNumber"].ToString())){
                        return false;
                    }
                    return true;
                }
                return false;
            }
        }

        public LoginResponseDTO GetRole(LoginRequestDTO request)
        {
            using (var con = new SqlConnection("Data Source=db-mssql;Initial Catalog=s14354;Integrated Security=True"))
            using (var com = new SqlCommand())
            {
                com.Connection = con;
                //hash
                request.Haslo = Hash(request.Haslo, GetSalt(request.Login));

                com.CommandText = "select IndexNumber, LastName, Role from Student where IndexNumber = @index and Password = @password";
                com.Parameters.AddWithValue("index", request.Login);
                com.Parameters.AddWithValue("password", request.Haslo);
                con.Open();
                var dr = com.ExecuteReader();
                while (dr.Read())
                {
                    var response = new LoginResponseDTO();
                    response.id = dr["IndexNumber"].ToString();
                    response.name = dr["LastName"].ToString();
                    response.role = dr["Role"].ToString();
                    return response;
                }
                return null;
            }
        }

        public LoginResponseDTO GetRole(string refToken)
        {
            using (var con = new SqlConnection("Data Source=db-mssql;Initial Catalog=s14354;Integrated Security=True"))
            using (var com = new SqlCommand())
            {
                com.Connection = con;
                com.CommandText = "select IndexNumber, LastName, Role from Student where RefToken = @refToken";
                com.Parameters.AddWithValue("RefToken", refToken);
                con.Open();
                var dr = com.ExecuteReader();
                while (dr.Read())
                {
                    var response = new LoginResponseDTO();
                    response.id = dr["IndexNumber"].ToString();
                    response.name = dr["LastName"].ToString();
                    response.role = dr["Role"].ToString();
                    return response;
                }
                return null;
            }
        }

        //public static string Salt()
        //{
        //    byte[] randomBytes = new byte[128 / 8];
        //    using(var generator = RandomNumberGenerator.Create())
        //    {
        //        generator.GetBytes(randomBytes);
        //        return Convert.ToBase64String(randomBytes);
        //    }
        //}

        private string GetSalt(string index)
        {
            using (var con = new SqlConnection("Data Source=db-mssql;Initial Catalog=s14354;Integrated Security=True"))
            using (var com = new SqlCommand())
            {
                com.Connection = con;
                com.CommandText = "select Salt from Student where IndexNumber = @index";
                com.Parameters.AddWithValue("index", index);
                con.Open();
                var dr = com.ExecuteReader();
                while (dr.Read())
                {
                    return dr["IndexNumber"].ToString();
                }
                return null;
            }
        }

        public static string Hash(string value, string salt)
        {
            var valueBytes = KeyDerivation.Pbkdf2(
                password: value,
                salt: Encoding.UTF8.GetBytes(salt),
                prf: KeyDerivationPrf.HMACSHA512,
                iterationCount: 10000,
                numBytesRequested: 256 / 8);
            return Convert.ToBase64String(valueBytes);
        }

    }
}
