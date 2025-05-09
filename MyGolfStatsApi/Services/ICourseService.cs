using MyGolfStatsApi.Db.Models;
using MyGolfStatsApi.DTOs;

namespace MyGolfStatsApi.Services
{
    public interface ICourseService
    {
        Task<List<Course>> GetAllCourses();
        Task<Course> AddCourse(AddCourseDTO course);
        Task<Course> UpdateCourse(CourseDTO course);
        Task<bool> DeleteCourse(int courseId);
    }
}
