using Microsoft.EntityFrameworkCore;
using MyGolfStatsApi.Db.AppDbContext;
using MyGolfStatsApi.Db.Models;
using MyGolfStatsApi.DTOs;
using System.Reflection;

namespace MyGolfStatsApi.Services
{
    public class CourseService : ICourseService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<CourseService> _log;

        public CourseService(AppDbContext context, ILogger<CourseService> log)
        {
            _context = context;
            _log = log;
        }
        public async Task<Course> AddCourse(AddCourseDTO course)
        {
            try
            {
                int coursePar = 0;
                var newCourse = new Course
                {
                    ClubName = course.ClubName,
                    CourseRating = course.CourseRating,
                    Tees = course.Tees,
                    Holes = new List<Hole>()
                };
                foreach (var hole in course.Holes)
                {
                    var newHole = new Hole
                    {
                        HoleNumber = hole.HoleNumber,
                        Par = hole.Par,
                        Course = newCourse
                    };
                    coursePar += hole.Par;
                    newCourse.Holes.Add(newHole);
                }
                newCourse.Par = coursePar;

                _context.Courses.Add(newCourse);
                await _context.SaveChangesAsync();


                return newCourse;
            }
            catch (Exception ex)
            {
                _log.LogError(ex, $"Message: {ex.Message}. Method: {MethodBase.GetCurrentMethod()?.Name ?? ""}.");
                throw new Exception("An error occurred while trying to add a course.", ex);
            }
        }

        public async Task<bool> DeleteCourse(int courseId)
        {
            try
            {
                var existingCourse = await _context.Courses.FirstOrDefaultAsync(c => c.Id == courseId);
                if (existingCourse == null)
                {
                    throw new Exception($"Course with ID {courseId} not found.");
                }

                _context.Courses.Remove(existingCourse);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _log.LogError(ex, $"Message: {ex.Message}. Method: {MethodBase.GetCurrentMethod()?.Name ?? ""}.");
                throw new Exception("An error occurred while trying to delete a course.", ex);
            }
        }

        public async Task<List<Course>> GetAllCourses()
        {
            try
            {
                return await _context.Courses.Include(c => c.Holes).ToListAsync();
            }
            catch (Exception ex)
            {
                _log.LogError(ex, $"Message: {ex.Message}. Method: {MethodBase.GetCurrentMethod().Name ?? ""}.");
                throw new Exception("An error occurred while trying to find courses.", ex);
            }
        }

        public async Task<Course> UpdateCourse(CourseDTO course)
        {
            try
            {
                var existingCourse = await _context.Courses
                    .Include(c => c.Holes)
                    .FirstOrDefaultAsync(c => c.Id == course.Id);

                if (existingCourse == null)
                    throw new Exception($"Course with ID {course.Id} not found.");

                if (course.Holes.Count != existingCourse.Holes.Count)
                {
                    throw new InvalidOperationException($"Det inkommande antalet hål ({course.Holes.Count}) är färre än antalet hål i databasen ({existingCourse.Holes.Count}).");
                }

                existingCourse.ClubName = course.ClubName;
                existingCourse.CourseRating = course.CourseRating;
                existingCourse.Tees = course.Tees;

                var coursePar = 0;

                foreach (var holeDto in course.Holes)
                {
                    var existingHole = existingCourse.Holes
                        .FirstOrDefault(h => h.HoleNumber == holeDto.HoleNumber);

                    if (existingHole != null)
                    {
                        existingHole.Par = holeDto.Par;
                    }

                    coursePar += holeDto.Par;
                }

                existingCourse.Par = coursePar;

                await _context.SaveChangesAsync();
                return existingCourse;
            }
            catch (Exception ex)
            {
                _log.LogError(ex, $"Message: {ex.Message}. Method: {MethodBase.GetCurrentMethod()?.Name ?? ""}.");
                throw new Exception("An error occurred while trying to update a course.", ex);
            }
        }

    }
}
