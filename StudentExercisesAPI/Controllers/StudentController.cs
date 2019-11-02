using System;
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
    public class StudentController : ControllerBase
    {
        private readonly IConfiguration _config;

        public StudentController(IConfiguration config)
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
                    cmd.CommandText = @"SELECT s.Id, s.StuFirstName, s.StuLastName, s.StuSlackHandle, s.CohortId, c.CohortName,
                                        se.ExerciseId, e.ExerciseName, e. ExerciseLang
                                        FROM Student s INNER JOIN Cohort c ON s.CohortID = c.Id
                                        LEFT JOIN StudentExercises se on se.StudentId = s.Id
                                        LEFT JOIN Exercise e on se.ExerciseId = e.Id";
                    SqlDataReader reader = cmd.ExecuteReader();
                    Dictionary<int, Student> students = new Dictionary<int, Student>();

                    while (reader.Read())
                    {
                        int studentId = reader.GetInt32(reader.GetOrdinal("Id"));
                        if (!students.ContainsKey(studentId))
                        {
                            Student student = new Student()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                StuFirstName = reader.GetString(reader.GetOrdinal("StuFirstName")),
                                StuLastName = reader.GetString(reader.GetOrdinal("StuLastName")),
                                StuSlackHandle = reader.GetString(reader.GetOrdinal("StuSlackHandle")),
                                CohortId = reader.GetInt32(reader.GetOrdinal("CohortId")),
                                Cohort = new Cohort()
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("CohortId")),
                                    CohortName = reader.GetString(reader.GetOrdinal("CohortName"))
                                }
                            };

                            students.Add(studentId, student);
                        }
                        Student fromDictionary = students[studentId];

                        if (!reader.IsDBNull(reader.GetOrdinal("ExerciseId")))
                        {
                            Exercise anExercise = new Exercise()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("ExerciseId")),
                                ExerciseName = reader.GetString(reader.GetOrdinal("ExerciseName")),
                                ExerciseLang = reader.GetString(reader.GetOrdinal("ExerciseLang"))
                            };
                            fromDictionary.exercises.Add(anExercise);
                        }
                    }
                        reader.Close();

                        return Ok(students.Values);
                    

                }
            }
        }

        [HttpGet("{id}", Name = "GetStudent")]
        public async Task<IActionResult> Get([FromRoute] int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT
                            Id, ExerciseName, ExerciseLang
                        FROM Exercise
                        WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = cmd.ExecuteReader();

                    Exercise exercise = null;

                    if (reader.Read())
                    {
                        exercise = new Exercise
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            ExerciseName = reader.GetString(reader.GetOrdinal("ExerciseName")),
                            ExerciseLang = reader.GetString(reader.GetOrdinal("ExerciseLang"))
                        };
                    }
                    reader.Close();

                    return Ok(exercise);
                }
            }
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Exercise exercise)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"INSERT INTO Exercise (ExerciseName, ExerciseLang)
                                        OUTPUT INSERTED.Id
                                        VALUES (@exerciseName, @exerciseLang)";
                    cmd.Parameters.Add(new SqlParameter("@exerciseName", exercise.ExerciseName));
                    cmd.Parameters.Add(new SqlParameter("@exerciseLang", exercise.ExerciseLang));

                    int newId = (int)cmd.ExecuteScalar();
                    exercise.Id = newId;
                    return CreatedAtRoute("GetExercise", new { id = newId }, exercise);
                }
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put([FromRoute] int id, [FromBody] Exercise exercise)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"UPDATE Exercise
                                            SET ExerciseName = @exerciseName,
                                                ExerciseLang = @exerciseLang
                                            WHERE Id = @id";
                        cmd.Parameters.Add(new SqlParameter("@exerciseName", exercise.ExerciseName));
                        cmd.Parameters.Add(new SqlParameter("@exerciseLang", exercise.ExerciseLang));
                        cmd.Parameters.Add(new SqlParameter("@id", id));

                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            return new StatusCodeResult(StatusCodes.Status204NoContent);
                        }
                        throw new Exception("No rows affected");
                    }
                }
            }
            catch (Exception)
            {
                if (!ExerciseExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"DELETE FROM Exercise WHERE Id = @id";
                        cmd.Parameters.Add(new SqlParameter("@id", id));

                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            return new StatusCodeResult(StatusCodes.Status204NoContent);
                        }
                        throw new Exception("No rows affected");
                    }
                }
            }
            catch (Exception)
            {
                if (!ExerciseExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        private bool ExerciseExists(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT Id, ExerciseName, ExerciseLang
                        FROM Exercise
                        WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    SqlDataReader reader = cmd.ExecuteReader();
                    return reader.Read();
                }
            }
        }
    }
}