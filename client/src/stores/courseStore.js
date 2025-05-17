import { defineStore } from 'pinia';
import { getAllCourses } from '../api/MyGolfStatsApi';

export const useCourseStore = defineStore('course', {
  state: () => ({
    courses: [],
    selectedCourse: null
  }),

  actions: {
    async fetchCourses() {
      const data = await getAllCourses();
      this.courses = data.map(course => ({
        ...course,
        holeCount: Array.isArray(course.holes) ? course.holes.length : 0
      }));
    },

    setSelectedCourse(course) {
      this.selectedCourse = course;
    },

    getCourseById(id) {
      const course = this.courses.find(c => c.id === parseInt(id));
      return course
    }
  }
});
