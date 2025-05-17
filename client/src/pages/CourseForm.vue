<template>
  <div class="background-image">
    <div class="invisible-div golf-dark text-light pb-2 rounded">
      <div class="container mt-4 text-light">
        <h2>{{ isEdit ? 'Redigera golfbana' : 'Lägg till golfbana' }}</h2>
    
        <form @submit.prevent="onSubmit">
          <div class="mb-3">
            <label class="form-label">Golfklubbens namn:</label>
            <input v-model="course.clubName" class="form-control" required />
          </div>
    
          <div class="mb-3">
            <label class="form-label">Course Rating:</label>
            <input v-model.number="course.courseRating" class="form-control" type="number" min="0" step="0.1" required />
          </div>
    
          <div class="mb-3">
            <label class="form-label">Tees: (kommaseparerat)</label>
            <input v-model="teeInput" @blur="updateTees" class="form-control" placeholder="ex: Gul, Röd, Vit" />
          </div>
    
          <div class="mb-3">
            <label class="form-label">Hål: (lägg till minst ett)</label>
            <div class="row">
              <div v-for="(hole, index) in course.holes" :key="index" class="col-lg-12 col-xl-6 col-xxl-4 mb-3 d-flex justify-content-center align-items-center gap-2 flex-wrap">
                <div class="text-center">
                  <label class="form-label mb-1">Hål: </label>
                  <input v-model.number="hole.holeNumber" class="form-control form-control-sm text-center" type="number" placeholder="hålnummer" />
                </div>

                <div class="text-center">
                  <label class="form-label mb-1">Par: </label>
                  <input v-model.number="hole.par" class="form-control form-control-sm text-center" type="number" placeholder="par" />
                </div>

                <div class="mt-auto">
                  <button type="button" class="btn btn-outline-danger btn-sm mt-2" @click="removeHole(index)">
                    <i class="fas fa-trash"></i>
                  </button>
                </div>
              </div>
              <div class="d-flex justify-content-between mt-2">
                <button type="button" class="btn btn-outline-light d-flex justify-content-center align-items-center" @click="addHole">
                  <i class="fas fa-plus small me-2"></i>
                  <span>Lägg till hål</span>
                </button>
                <div>
                  <router-link to="/courses" class="btn btn-outline-secondary button-secondary">Avbryt</router-link>
                  <button type="submit" class="btn btn-outline-primary ms-3 me-2">Spara</button>
                </div>
              </div>
            </div>
          </div>
        </form>
      </div>
    </div>
  </div>
</template>

<script>
import { useCourseStore } from '../stores/courseStore';
import { addCourse, updateCourse } from '../api/MyGolfStatsApi.js'

export default {
  name: "CourseForm",
  data() {
    return {
      course: {
        id: 0,
        clubName: "",
        courseRating: 0,
        tees: [],
        holes: []
      },
      teeInput: "",
      isEdit: false
    };
  },
  async created() {
    const courseStore = useCourseStore();
    const id = this.$route.params.id;

    if (id && id !== "new") {
      this.isEdit = true;

      const existing = courseStore.getCourseById(id) || courseStore.selectedCourse;

      if (existing) {
        this.course = {
          ...existing,
          holes: Array.isArray(existing.holes) ? existing.holes : []
        };
      } else {
        await courseStore.fetchCourses(); // fallback
        const found = courseStore.getCourseById(id);
        if (found) this.course = { ...found };
      }
    }

    // Uppdatera teeInput från course-data
    this.teeInput = this.course.tees?.join(", ") || "";
  },
  methods: {
    updateTees() {
    this.course.tees = this.teeInput
      .split(",")
      .map(t => t.trim())
      .filter(t => t);
    },
    addHole() {
      const nextNumber = this.course.holes.length > 0
        ? Math.max(...this.course.holes.map(h => h.holeNumber || 0)) + 1
        : 1;

      this.course.holes.push({ holeNumber: nextNumber, par: null });
    },
    removeHole(index) {
      this.course.holes.splice(index, 1);
    },
    async onSubmit() {
      try {
        if (this.isEdit) {
          // Uppdatera befintlig
          await updateCourse(this.course);
          alert("Golfbanan uppdaterad!");
        } else {
          // Lägg till ny
          await addCourse(this.course);
          alert("Ny golfbana tillagd!");
        }
        this.$router.push("/courses");
      } catch (err) {
        console.error("Fel vid spara:", err);
        alert("Kunde inte spara golfbanan.");
      }
    }
  }
};
</script>
