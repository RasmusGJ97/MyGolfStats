<template>
  <div class="background-image">
    <div class="invisible-div golf-dark text-light pb-4 rounded">
      <div class="container mt-4">
        <div class="d-flex justify-content-between align-items-center mb-3">
          <h2 class="text-light m-0">Golfbanor</h2>
          <button class="btn btn-outline-success" @click="onAddCourse">
            <i class="fas fa-plus me-2"></i>Lägg till
          </button>
        </div>

        <div v-if="loading" class="text-center my-5">
          <div class="spinner-border text-light" role="status">
            <span class="visually-hidden">Laddar...</span>
          </div>
        </div>

        <DataGrid
          v-else-if="courses.length"
          :items="courses"
          :columns="columns"
          :pageSize="10"
          @row-clicked="onRowClicked"
          @delete-item="onDeleteClicked"
        />

        <div v-else class="text-light">Inga golfbanor kunde visas.</div>
      </div>
    </div>
  </div>
</template>

<script>
import DataGrid from '../components/DataGrid.vue'
import { useCourseStore } from '../stores/courseStore'
import { deleteCourse } from '../api/MyGolfStatsApi';

export default {
  components: { DataGrid },
  data() {
    return {
      courses: [],
      columns: [
        { label: 'Golfklubb - Bana', key: 'clubName' },
        { label: 'Antal Hål', key: 'holeCount' },
        { label: 'Par', key: 'par' },
        { label: 'Course Rating', key: 'courseRating' },
        { label: 'Tee', key: 'tees' },
      ],
      loading: true
    };
  },
  async mounted() {
    await this.loadCourses()
  },
  methods: {
    async loadCourses(){
      this.loading = true;
      const courseStore = useCourseStore();
      await courseStore.fetchCourses();
      this.courses = courseStore.courses;
      this.loading = false;
    },
    onAddCourse() {
      this.$router.push(`/course/new`);
    },
    onRowClicked(course) {
      const courseStore = useCourseStore();
      courseStore.setSelectedCourse(course);
      this.$router.push(`/course/${course.id}`);
    },
    async onDeleteClicked(course) {
      const confirmed = window.confirm(`Är du säker på att du vill ta bort denna banan?`);
      
      if (!confirmed) return;
      await deleteCourse(course.id)
      await this.loadCourses()
    }
  }
};
</script>