using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Cw3.DAL;
using Cw3.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authorization;

namespace Cw3.Controllers
{
    [ApiController]
    [Route("api/enrollments")]
    public class EnrollmentsController : ControllerBase
    {
        private readonly IDbService _dbService;

        public EnrollmentsController(IDbService dbService)
        {
            _dbService = dbService;
        }

        [HttpPost]
        [Authorize(Roles = "employee")]
        public IActionResult Enroll(Student student)
        {
            if(String.IsNullOrEmpty(student.IndexNumber) || String.IsNullOrEmpty(student.FirstName) || String.IsNullOrEmpty(student.LastName) || String.IsNullOrEmpty(student.BirthDate) || String.IsNullOrEmpty(student.Studies))
            {
                return BadRequest();
            }

            Study st = _dbService.GetStudyByName(student.Studies);

            if (st == null)
            {
                return BadRequest();
            }

            Enrollment en = _dbService.SetFirstEnrollment(st, student);

            if (en != null)
            {
                return new ObjectResult(en) { StatusCode = StatusCodes.Status201Created };
            } else
            {
                return StatusCode(500);
            }

        }

        [HttpPost]
        [Authorize(Roles = "employee")]
        [Route("promotions")]
        public IActionResult Promote(String study, int semester)
        {

            Enrollment en = _dbService.GetEnrollment(study, semester);

            if (en != null)
            {
                return NotFound();
            }
            else
            {
                en = _dbService.Promote(study, semester);
                return new ObjectResult(en) { StatusCode = StatusCodes.Status201Created };
            }

        }
    }
}