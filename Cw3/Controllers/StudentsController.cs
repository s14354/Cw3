using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Cw3.DAL;
using Cw3.Models;
using Cw3.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace Cw3.Controllers
{
    [ApiController]
    [Route("api/students")]
    public class StudentsController : ControllerBase
    {
        private readonly IDbService _dbService;
        public IConfiguration Configuration { get; set; }

        public StudentsController(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public StudentsController(IDbService dbService)
        {
            _dbService = dbService;
        }

        [HttpGet("{orderBy}")]
        public IActionResult GetStudents(string orderBy)
        {
            return Ok(_dbService.GetStudents(orderBy));
        }

        [HttpGet("{id}")]
        public IActionResult GetStudent(int id)
        {
            return Ok(_dbService.GetStudent(id));
        }

        [HttpPost]
        public IActionResult CreateStudent(Student student)
        {
            student.IndexNumber = $"s{new Random().Next(1, 20000)}";
            return Ok(student);
        }

        [HttpPut("{id}")]
        public IActionResult PutStudent(int id)
        {
            return Ok("Aktualizacja zakończona");
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteStudent(int id)
        {
            return Ok("Usuwanie zakończone");
        }

        [HttpPost]
        public IActionResult Login(LoginRequestDTO request)
        {
            LoginResponseDTO response = _dbService.GetRole(request);
            if (!String.IsNullOrEmpty(response.role)) {

                var claims = new[]
                {
                new Claim(ClaimTypes.NameIdentifier, response.id.ToString()),
                new Claim(ClaimTypes.Name, response.name),
                new Claim(ClaimTypes.Role, response.role)
            };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["SecretKey"]));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken
                (
                    issuer: "Gakko",
                    audience: "Students",
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(10),
                    signingCredentials: creds
                );

                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    refreshToken = Guid.NewGuid()
                });
            } else
            {
                return BadRequest();
            }
        }

        [HttpPost("refresh/{token}")]
        public IActionResult RefreshToken(string refToken)
        {
            LoginResponseDTO response = _dbService.GetRole(refToken);
            if (!String.IsNullOrEmpty(response.role))
            {

                var claims = new[]
                {
                new Claim(ClaimTypes.NameIdentifier, response.id.ToString()),
                new Claim(ClaimTypes.Name, response.name),
                new Claim(ClaimTypes.Role, response.role)
            };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["SecretKey"]));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken
                (
                    issuer: "Gakko",
                    audience: "Students",
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(10),
                    signingCredentials: creds
                );

                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    refreshToken = Guid.NewGuid()
                });
            }
            else
            {
                return BadRequest();
            }
        }

    }
}