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
    public class CohortController : ControllerBase
    {
        private readonly IConfiguration _config;

        public CohortController(IConfiguration config)
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
                    cmd.CommandText = @"SELECT c.Id, c.CohortName, s.StuFirstName, s.StuLastName, i.InstFirstName, i.InstLastName
                                        FROM Cohort c 
                                        INNER JOIN Student s ON s.CohortId = c.Id
                                        LEFT JOIN Instructor i ON i.InstCohort = c.Id";
                    SqlDataReader reader = cmd.ExecuteReader();
                    Dictionary<int, Cohort> cohorts = new Dictionary<int, Cohort>();

                    while (reader.Read())
                    {
                        int cohortId = reader.GetInt32(reader.GetOrdinal("Id"));
                        if (!cohorts.ContainsKey(cohortId))
                        {
                            Cohort cohort = new Cohort()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                CohortName = reader.GetString(reader.GetOrdinal("CohortName")),
                                Student = new Student()
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                    StuFirstName = reader.GetString(reader.GetOrdinal("StuFirstName")),
                                    StuLastName = reader.GetString(reader.GetOrdinal("StuLastName"))
                                },
                                Instructor = new Instructor()
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                    InstFirstName = reader.GetString(reader.GetOrdinal("InstFirstName")),
                                    InstLastName = reader.GetString(reader.GetOrdinal("InstLastName"))
                                }
                            };

                            cohorts.Add(cohortId, cohort);
                        }
                    }
                    reader.Close();

                    return Ok(cohorts.Values);


                }
            }
        }
    }
}