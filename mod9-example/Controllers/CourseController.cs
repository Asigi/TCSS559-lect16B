using System;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Net;
using System.Net.Http;
using MySql.Data.MySqlClient;

namespace mod9_example.Controllers
{
    public class CourseController : ControllerBase
    {
        [ApiExplorerSettings(IgnoreApi = true)]
        [NonAction]
        public DataSet executeSQL(string sqlStatement)
        {
            string connStr = "server=localhost; port=3306; user=root; password=M5hDgCMFaRngRu; database=mod9-courses; ";
            MySqlConnection conn = new MySqlConnection(connStr);
            MySqlDataAdapter sqlAdapter = new MySqlDataAdapter(sqlStatement, conn);
            DataSet myResultSet = new DataSet();
            sqlAdapter.Fill(myResultSet, "courses");
            return myResultSet;
        }


        //Step 1: connect to the database and execute a sql for retreving all of the existing courses. Then reutrn them with 200 OK.
        [HttpGet]
        [Route("courses/")]
        public IActionResult getCourses()
        {
            DataSet uwCourses = executeSQL("SELECT * FROM courses");
            return Ok(uwCourses);
        }



        //Step 2: return course info by its id.
        [HttpGet]
        [Route("courses/{id:int}")]
        public IActionResult getCourse(int id)
        {
            DataSet uwCourses = executeSQL("SELECT * FROM courses WHERE courseId =" + id.ToString() + ";" );
            int countRecords = uwCourses.Tables[0].Rows.Count;
            if (countRecords == 0)
            {
                return NotFound();
            }
            return Ok(uwCourses);
        }

        //Step 3: we would like to retrieve the title of a specific course
        [HttpGet]
        [Route("courses/{id:int}/title")]
        public IActionResult getCourseName(int id)
        {
            DataSet uwCourses = executeSQL("SELECT courseTitle FROM courses WHERE courseId =" + id.ToString() + ";" );
            int countRecords = uwCourses.Tables[0].Rows.Count;
            if (countRecords == 0)
            {
                return BadRequest();
            }
            return Ok(uwCourses);
        }

        //Step 4: I would like to insert (add) a new course into the database
        [HttpPost]
        [Route("courses/")]
        public IActionResult addCourse([FromBody] Course course)
        {
            try 
            {
                string sqlStatement = "INSERT INTO courses VALUES ( " + course.courseId + "," +  
                (char)39 + course.courseCode + (char)39 + "," +
                (char)39 + course.courseTitle + (char)39 + "," +
                (char)39 + course.deptName + (char)39 + "," +
                (char)39 + course.instructorName + (char)39 + ");" ;

                executeSQL(sqlStatement);
                HttpContext.Response.Headers.Add("Status", "Record for " + course.courseTitle + " has been added");
                HttpContext.Response.StatusCode = 200;
                //return Ok("The record has been successfully added.");
                return Content("The record has been successfully added.", "application/json");

            } 
            catch
            {
                HttpContext.Response.Headers.Add("Status", "Record for " + course.courseTitle + " has failed");
                HttpContext.Response.StatusCode = 400;
                return Content("the record has been not successfuly added.", "application/json");
            }
        }


        // complex type to be used in put, because [FromBody] expects it.
        public class myPutModel {
            public string title {get; set;}
        }
        //update an existing record for the coureseTitle
        [HttpPut]
        [Route("courses/{id:int}/title")]
        public IActionResult setCourseTitle(int id, [FromBody] myPutModel putModel )
        {
            try 
            {
                string sqlStatement = "UPDATE courses SET courseTitle = " + (char)39 + putModel.title + (char)39 + "where courseID = " + id;

                executeSQL(sqlStatement);
                HttpContext.Response.Headers.Add("Status", "Record for " + putModel.title + " has been updated");
                HttpContext.Response.StatusCode = 200;
                //return Ok("The record has been successfully added.");
                return Content("The record has been successfully updated!", "application/json");

            } 
            catch
            {
                HttpContext.Response.Headers.Add("Status", "Record for " + putModel.title + " has failed");
                HttpContext.Response.StatusCode = 400;
                return Content("the record has been not successfuly updated.", "application/json");
            }
        }


        //Step 6: we would like to delete an existing record from the database
        [HttpDelete]
        [Route("/courses/{id:int}")]
        public IActionResult deleteCourse(int id)
        {
            try{
                string sqlStatement = "DELETE FROM courses WHERE courseId = " + id;
                executeSQL(sqlStatement);
                HttpContext.Response.Headers.Add("Status", "Record for " + id + " has been deleted");
                HttpContext.Response.StatusCode = 200;
                return Content("The record has been successfully deleted!", "application/json");

            } 
            catch
            {
                HttpContext.Response.Headers.Add("Status", "Record for " + id + " has failed");
                HttpContext.Response.StatusCode = 400;
                return Content("the record has been not successfuly deleted.", "application/json");
            }
            
        }


    }
}