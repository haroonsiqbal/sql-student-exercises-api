using System;
using System.Collections.Generic;
using System.Text;

namespace StudentExercisesAPI.Models
{
    public class Instructor
    {
        public int Id { get; set; }
        public string InstFirstName { get; set; }
        public string InstLastName { get; set; }
        public string InstSlackHandle { get; set; }
        public int InstCohort { get; set; }
        public Cohort Cohort { get; set; }
        public string InstSpeciality { get; set; }
        public List<Instructor> instructorlist { get; set; } = new List<Instructor>();
    }
}
