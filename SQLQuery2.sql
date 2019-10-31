SELECT c.Id, c.CohortName, s.StuFirstName, s.StuLastName, i.InstFirstName, i.InstLastName
                                        FROM Cohort c 
                                        INNER JOIN Student s ON s.CohortId = c.Id
                                        LEFT JOIN Instructor i ON i.InstCohort = c.Id