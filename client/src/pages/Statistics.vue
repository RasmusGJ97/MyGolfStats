<template>
  <div class="background-image">
    <div class="filter-div golf-dark m-3 p-4 rounded text-light">
      <div class="mb-2">
        <label class="d-block">Visa statistik för:</label>
        <select v-model="selectedFilter" class="form-select w-100 bg-dark text-light border-secondary">
          <option value="all">Totalt</option>
          <option value="year">I år</option>
          <option value="last10">Senaste 10 rundorna</option>
          <option value="last5">Senaste 5 rundorna</option>
        </select>
      </div>
      
      <div class="mb-2">
        <label class="d-block">Filtrera på:</label>
        <select v-model="selectedSubFilter" class="form-select w-100 bg-dark text-light border-secondary">
          <option value="all">Allt</option>
          <option value="course">Specifik bana</option>
          <option value="hole">Specifikt hål</option>
          <option value="hcp">Jämfört med samma HCP</option>
        </select>
      </div>

      <div class="mb-2" v-if="selectedSubFilter != 'all' && selectedSubFilter != 'hcp'">
        <label class="d-block">Välj bana:</label>
          <select v-model="selectedCourseFilter" class="form-select w-100 bg-dark text-light border-secondary">
            <option value="all">Alla</option>
            <option v-for="course in availableCourses" :key="course.id" :value="course.id">
              {{ course.clubName }}
            </option>
          </select>
      </div>

      <div class="mb-2" v-if="selectedSubFilter == 'hole' && selectedCourseFilter != 'all'">
        <label class="d-block">Välj hål:</label>
          <select v-model="selectedHoleFilter" class="form-select w-100 bg-dark text-light border-secondary">
            <option value="" disabled>-- Välj hål --</option>
            <option v-for="hole in availableHoles" :key="hole" :value="hole">
              Hål {{ hole.holeNumber }} - Par {{ hole.par }}
            </option>
          </select>
      </div>
    </div>
    <div class="invisible-div-statistics text-light pb-4 rounded">
      <div>
        <StatisticsComponent v-if="selectedSubFilter != 'hcp'" 
          :statistics="filteredRounds"
          :selectedFilter="selectedFilter"
          :selectedSubFilter="selectedSubFilter"
          :selectedCourseFilter="selectedCourseFilter"
          :selectedHoleFilter="selectedHoleFilter" />
      </div>
    </div>
  </div>
</template>

<script>
import StatisticsComponent from '../components/StatisticsComponent.vue'
import { useUserStore } from '../stores/userStore'
import { useCourseStore } from '../stores/courseStore'

export default {
  components: {
    StatisticsComponent
  },
  data() {
    return {
      selectedFilter: 'all',
      selectedSubFilter: 'all',
      selectedCourseFilter: 'all',
      selectedHoleFilter: 'all',
      loading: true
    }
  },
  computed: {
    userStore() {
      return useUserStore()
    },
    courseStore() {
      return useCourseStore()
    },
    rounds() {
      return this.userStore.user?.rounds || []
    },
    currentUser() {
      return this.userStore.currentUser || null
    },
    filteredRounds() {
      if (!this.rounds || this.rounds.length === 0) return [];

      let result = [...this.rounds];

      // Huvudfilter
      switch (this.selectedFilter) {
        case 'year':
          result = result.filter(r => new Date(r.date).getFullYear() === new Date().getFullYear());
          break;
            case 'last10':
              result = result
                .filter(r => {
                  return this.selectedCourseFilter === 'all' ||r.statistics?.some(s => s.hole?.courseId === Number(this.selectedCourseFilter));
                })
                .slice()
                .sort((a, b) => new Date(b.date) - new Date(a.date))
                .slice(0, 10);
              break;

            case 'last5':
              result = result
                .filter(r => {
                  return this.selectedCourseFilter === 'all' ||r.statistics?.some(s => s.hole?.courseId === Number(this.selectedCourseFilter));
                })
                .slice()
                .sort((a, b) => new Date(b.date) - new Date(a.date))
                .slice(0, 5);
              break;
      }

      // Underfilter
      switch (this.selectedSubFilter) {
        case 'hcp':
          result = result.filter(r => r.hcp === this.currentUser?.hcp);
          break;

        case 'course':
          if (this.selectedCourseFilter !== 'all') {
            const selectedCourseId = Number(this.selectedCourseFilter);
            result = result.filter(r =>
              r.statistics?.some(s => s.hole?.courseId === selectedCourseId)
            );
          }
          break;

        case 'hole':
          const selectedCourseId = Number(this.selectedCourseFilter);
          const selectedHoleNumber = Number(this.selectedHoleFilter.holeNumber);

          let allStats = [];

          result.forEach(r => {
            if (r.statistics) {
              const matchedStats = r.statistics.filter(s =>
                s.hole?.courseId === selectedCourseId &&
                s.hole?.holeNumber === selectedHoleNumber
              );
              allStats.push(...matchedStats);
            }
          });
          return allStats;
      }      
      return result;
    },

    availableCourses() {
      return this.courseStore.courses || []
    },
    availableHoles() {
      const selectedCourse = this.courseStore.courses.find(c => c.id === this.selectedCourseFilter);
      return selectedCourse ? selectedCourse.holes : [];
    }
  },
}
</script>
