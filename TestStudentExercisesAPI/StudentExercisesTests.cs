using StudentExercisesAPI.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using TestStudentExercisesAPI;

namespace StudentExercisesAPI
{
    public class StudentExercisesTests
    {
        [Fact]
        public async Task Test_Create_Student()
        {
            /*
                Generate a new instance of an HttpClient that you can
                use to generate HTTP requests to your API controllers.
                The `using` keyword will automatically dispose of this
                instance of HttpClient once your code is done executing.
            */
            using (var client = new APIClientProvider().Client)
            {
                /*
                    ARRANGE
                */

                // Construct a new student object to be sent to the API
                Student freddy = new Student
                {
                    StuFirstName = "Freddy",
                    StuLastName= "Fullerton",
                    StuSlackHandle = "@fullhousefreddy",
                    CohortId = 1,
                    InstructorId = 2
                };

                // Serialize the C# object into a JSON string
                var freddyAsJSON = JsonConvert.SerializeObject(freddy);


                /*
                    ACT
                */

                // Use the client to send the request and store the response
                var response = await client.PostAsync(
                    "/api/Student",
                    new StringContent(freddyAsJSON, Encoding.UTF8, "application/json")
                );

                // Store the JSON body of the response
                string responseBody = await response.Content.ReadAsStringAsync();

                // Deserialize the JSON into an instance of 
                var newFreddy = JsonConvert.DeserializeObject<Student>(responseBody);


                /*
                    ASSERT
                */

                Assert.Equal(HttpStatusCode.Created, response.StatusCode);
                Assert.Equal("Freddy", newFreddy.StuFirstName);
                Assert.Equal("Fullerton", newFreddy.StuLastName);
                Assert.Equal("@fullhousefreddy", newFreddy.StuSlackHandle);
                Assert.Equal(1, newFreddy.CohortId);
                Assert.Equal(2, newFreddy.InstructorId);
            }
        }

        [Fact]
        public async Task Test_Get_All_Students()
        {

            using (var client = new APIClientProvider().Client)
            {
                /*
                    ARRANGE
                */


                /*
                    ACT
                */
                var response = await client.GetAsync("/api/Student");


                string responseBody = await response.Content.ReadAsStringAsync();
                var studentList = JsonConvert.DeserializeObject<List<Student>>(responseBody);

                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(studentList.Count > 0);
            }
        }

        [Fact]
        public async Task Test_Modify_Student()
        {
            // New last name to change to and test
            string newLastName = "Bookerton";

            using (var client = new APIClientProvider().Client)
            {
                /*
                    PUT section
                */
                Student modifiedFreddy = new Student
                {
                    StuFirstName = "Freddy",
                    StuLastName = newLastName,
                    StuSlackHandle = "@fullhousefreddy",
                    CohortId = 1,
                    InstructorId = 2
                };
                var modifiedFreddyAsJSON = JsonConvert.SerializeObject(modifiedFreddy);

                var response = await client.PutAsync(
                    "/api/Student/1",
                    new StringContent(modifiedFreddyAsJSON, Encoding.UTF8, "application/json")
                );
                string responseBody = await response.Content.ReadAsStringAsync();

                Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);


                /*
                    GET section
                    Verify that the PUT operation was successful
                */
                var getFreddy = await client.GetAsync("/api/Student/1");
                getFreddy.EnsureSuccessStatusCode();

                string getFreddyBody = await getFreddy.Content.ReadAsStringAsync();
                Student newFreddy = JsonConvert.DeserializeObject<Student>(getFreddyBody);

                Assert.Equal(HttpStatusCode.OK, getFreddy.StatusCode);
                Assert.Equal(newLastName, newFreddy.StuLastName);
            }
        }
    }
}
