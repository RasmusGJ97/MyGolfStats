using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyGolfStatsApi.DTOs;
using MyGolfStatsApi.Services;

namespace MyGolfStatsApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseController : ControllerBase
    {
        private ICourseService _courseService;

        public CourseController(ICourseService courseService)
        {
            _courseService = courseService;
        }

        [HttpPut("updateCourse")]
        [Authorize(Roles = "User,Admin")]
        public async Task<IActionResult> UpdateCourse([FromBody] CourseDTO course)
        {
            if (course == null)
            {
                return BadRequest("Invalid course data.");
            }

            var updatedCourse = await _courseService.UpdateCourse(course);

            if (updatedCourse == null)
            {
                return NotFound($"Course with ID {course.Id} was not found.");
            }

            return Ok(new
            {
                message = "Course updated successfully.",
                course = updatedCourse
            });
        }

        [HttpPost("addCourse")]
        [Authorize(Roles = "User,Admin")]
        public async Task<IActionResult> AddCourse([FromBody] AddCourseDTO course)
        {
            if (course == null)
            {
                return BadRequest("Invalid course data.");
            }

            var addedCourse = await _courseService.AddCourse(course);

            if (addedCourse == null)
            {
                return NotFound($"Course could not be added");
            }

            return Ok(new
            {
                message = "Course added successfully.",
                course = addedCourse
            });
        }

        [HttpGet("allCourses")]
        [Authorize(Roles = "User,Admin")]
        public async Task<IActionResult> GetAllCourses()
        {
            var listOfCourses = await _courseService.GetAllCourses();

            if (listOfCourses == null)
            {
                return NotFound($"Courses was not found.");
            }

            return Ok(new
            {
                message = "Courses found successfully.",
                courses = listOfCourses
            });
        }

        [HttpDelete("admin/deleteCourse/{courseId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteCourse(int courseId)
        {
            var removed = await _courseService.DeleteCourse(courseId);

            if (removed == false)
            {
                return NotFound($"Course was not found.");
            }

            return Ok(new
            {
                message = "Course removed successfully.",
                courseRemoved = removed
            });
        }
    }
}
