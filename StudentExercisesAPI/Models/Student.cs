﻿using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace StudentExercisesAPI.Models
{
    public class Student
    {
        public int Id { get; set; }
        public string StuFirstName { get; set; }
        public string StuLastName { get; set; }
        [StringLength(12, MinimumLength = 3)]
        public string StuSlackHandle { get; set; }
        public int CohortId { get; set; }
        public Cohort Cohort { get; set; }
        public int InstructorId { get; set; }
        public List<Student> studentlist { get; set; } = new List<Student>();
        public List<Exercise> exercises { get; set; } = new List<Exercise>();
    }
}
