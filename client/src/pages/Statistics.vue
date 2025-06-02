<template>
  <div class="background-image">
    <div class="statistics-div d-flex flex-wrap justify-content-center align-items-center gap-2 gap-md-3 gap-lg-4">
      <div class="filter-div golf-dark p-4 rounded text-light">
        <div class="mb-2">
          <label class="d-block">Visa statistik för:</label>
          <select v-model="selectedFilter" class="form-select w-100 bg-dark text-light border-secondary">
            <option value="all">Totalt</option>
            <option value="year">I år</option>
            <option value="last10">Senaste 10 rundorna</option>
            <option value="last5">Senaste 5 rundorna</option>
            <option value="round">Specifik runda</option>
          </select>
        </div>
        
        <div class="mb-2" v-if="selectedFilter != 'round'">
          <label class="d-block">Filtrera på:</label>
          <select v-model="selectedSubFilter" class="form-select w-100 bg-dark text-light border-secondary">
            <option value="all">Allt</option>
            <option value="course">Specifik bana</option>
            <option value="hole">Specifikt hål</option>
            <option value="compHcp">Jämfört med samma HCP</option>
            <option value="compTotal">Jämfört med totalt</option>
            <option value="compYear">Jämfört med I år</option>
            <option value="compLast10">Jämfört med senaste 10 rundorna</option>
            <option value="compLast5">Jämfört med senaste 5 rundorna</option>
          </select>
        </div>
        
        <div class="mb-2" v-if="selectedFilter === 'round'">
          <label>Välj rond:</label>
          <select v-model="selectedRoundId" class="form-select w-100 bg-dark text-light border-secondary">
            <option disabled value="">-- Välj rond --</option>
            <option v-for="round in sortedRounds" :key="round.id" :value="round.id">
              {{ new Date(round.date).toLocaleDateString() }} - {{ getCourseNameFromRound(round) }}
            </option>
          </select>
        </div>
  
        <div class="mb-2" v-if="selectedSubFilter != 'all' && !selectedSubFilter.startsWith('comp')">
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
        <div class="mb-3">
          <StatisticsComponent v-if="!selectedSubFilter.startsWith('comp')"
            :statistics="filteredRounds"
            :selectedFilter="selectedFilter"
            :selectedSubFilter="selectedSubFilter"
            :selectedCourseFilter="selectedCourseFilter"
            :selectedHoleFilter="selectedHoleFilter" />
  
          <CompareStatsComponent v-if="selectedSubFilter.startsWith('comp')"
            :statistics="filteredRounds"
            :statisticsToCompareWith="compareFilter"
            :selectedFilter="selectedFilter"
            :selectedSubFilter="selectedSubFilter" />
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import StatisticsComponent from '../components/StatisticsComponent.vue'
import CompareStatsComponent from '../components/compareStatsComponent.vue'
import { useUserStore } from '../stores/userStore'
import { useCourseStore } from '../stores/courseStore'

export default {
  components: {
    StatisticsComponent,
    CompareStatsComponent
  },
  data() {
    return {
      selectedFilter: 'all',
      selectedSubFilter: 'all',
      selectedCourseFilter: 'all',
      selectedHoleFilter: 'all',
      selectedRoundId: null,
      loading: true,
      hcpGroup: {
        scratch: [{hcpFiR: '62'}, {hcpGiR: '57'}, {hcpPutts: '1.75'}],
        five: [{hcpFiR: '55'}, {hcpGiR: '48'}, {hcpPutts: '1.81'}],
        ten: [{hcpFiR: '51'}, {hcpGiR: '37'}, {hcpPutts: '1.87'}],
        fifteen: [{hcpFiR: '48'}, {hcpGiR: '28'}, {hcpPutts: '1.93'}],
        twenty: [{hcpFiR: '45'}, {hcpGiR: '21'}, {hcpPutts: '1.99'}],
        twentyfive: [{hcpFiR: '44'}, {hcpGiR: '16'}, {hcpPutts: '2.04'}],
        more: [{hcpFiR: '43'}, {hcpGiR: '12'}, {hcpPutts: '2.14'}]
      }
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
      return this.userStore.user || null
    },
    sortedRounds() {
      return [...this.rounds]
        .slice()
        .sort((a, b) => new Date(b.date) - new Date(a.date));
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

        case 'round':
          if (this.selectedRoundId) {
            result = result.filter(r => r.id === this.selectedRoundId);
          } else {
            result = [];
          }
          break;
      }

      // Underfilter
      switch (this.selectedSubFilter) {
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

    compareFilter(){
      if (!this.rounds || this.rounds.length === 0) return [];

      let result = [...this.rounds];

      switch (this.selectedSubFilter) {
        case 'compHcp':
          const hcp = this.currentUser.hcp
          console.log(hcp)
          if(hcp <= 1){
            result = this.hcpGroup.scratch
          }
          else if(hcp <= 5){
            result = this.hcpGroup.five
          }
          else if(hcp <= 10){
            result = this.hcpGroup.ten
          }
          else if(hcp <= 15){
            result = this.hcpGroup.fifteen
          }
          else if(hcp <= 20){
            result = this.hcpGroup.twenty
          }
          else if(hcp <= 25){
            result = this.hcpGroup.twentyfive
          }
          else{
            result = this.hcpGroup.more;
          }
        console.log(result)
          break;

        case 'compYear':
          result = result.filter(r => new Date(r.date).getFullYear() === new Date().getFullYear());
          break;
          
        case 'compLast10':
          result = result
            .filter(r => {
              return this.selectedCourseFilter === 'all' ||r.statistics?.some(s => s.hole?.courseId === Number(this.selectedCourseFilter));
            })
            .slice()
            .sort((a, b) => new Date(b.date) - new Date(a.date))
            .slice(0, 10);
          break;

        case 'compLast5':
          result = result
            .filter(r => {
              return this.selectedCourseFilter === 'all' ||r.statistics?.some(s => s.hole?.courseId === Number(this.selectedCourseFilter));
            })
            .slice()
            .sort((a, b) => new Date(b.date) - new Date(a.date))
            .slice(0, 5);
          break;
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
  methods: {
    getCourseNameFromRound(round) {
      const courseId = round?.statistics?.[0]?.hole?.courseId;
      if (!courseId) return 'Okänd bana';
      
      const course = this.courseStore.courses.find(c => c.id === courseId);
      return course?.clubName || 'Okänd bana';
    }
  }
}
</script>
