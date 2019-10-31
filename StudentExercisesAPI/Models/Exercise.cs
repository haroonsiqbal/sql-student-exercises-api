using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace StudentExercisesAPI.Models
{
    public class Exercise
    {
        public int Id { get; set; }
        [Required]
        public string ExerciseName { get; set; }
        [Required]
        public string ExerciseLang { get; set; }
        public int StudentId { get; set; }
        public int InstructorId { get; set; }
        public List<Exercise> Exercises { get; set; } = new List<Exercise>();
    }
}
