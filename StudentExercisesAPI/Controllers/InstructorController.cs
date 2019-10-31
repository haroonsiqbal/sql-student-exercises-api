﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;
using StudentExercisesAPI.Models;
using Microsoft.AspNetCore.Http;

namespace StudentExercisesAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InstructorController : ControllerBase
    {
        private readonly IConfiguration _config;

        public InstructorController(IConfiguration config)
        {
            _config = config;
        }

        public SqlConnection Connection
        {
            get
            {
                return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            }
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT i.Id, i.InstFirstName, i.InstLastName, i.InstSlackHandle, i.InstCohort, c.CohortName
                                        FROM Instructor i 
                                        INNER JOIN Cohort c ON i.InstCohort = c.Id";
                    SqlDataReader reader = cmd.ExecuteReader();
                    Dictionary<int, Instructor> instructors = new Dictionary<int, Instructor>();

                    while (reader.Read())
                    {
                        int instructorId = reader.GetInt32(reader.GetOrdinal("Id"));
                        if (!instructors.ContainsKey(instructorId))
                        {
                            Instructor instructor = new Instructor()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                InstFirstName = reader.GetString(reader.GetOrdinal("InstFirstName")),
                                InstLastName = reader.GetString(reader.GetOrdinal("InstLastName")),
                                InstSlackHandle = reader.GetString(reader.GetOrdinal("InstSlackHandle")),
                                InstCohort = reader.GetInt32(reader.GetOrdinal("InstCohort")),
                                Cohort = new Cohort()
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("InstCohort")),
                                    CohortName = reader.GetString(reader.GetOrdinal("CohortName"))
                                }
                            };

                            instructors.Add(instructorId, instructor);
                        }
                    }
                    reader.Close();

                    return Ok(instructors.Values);


                }
            }
        }
    }
}