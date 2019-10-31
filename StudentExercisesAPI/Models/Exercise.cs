using System;
using System.Collections.Generic;
using System.Text;

namespace StudentExercisesAPI.Models
{
    public class Exercise
    {
        public int Id { get; set; }
        public string ExerciseName { get; set; }
        public string ExerciseLang { get; set; }
        public int StudentId { get; set; }
        public int InstructorId { get; set; }
        public List<Exercise> Exercises { get; set; } = new List<Exercise>();
    }
}
