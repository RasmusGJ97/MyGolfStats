<template>
  <div class="background-image" v-if="userStore.user">
    <div class="invisible-div-home text-light pb-4 rounded">
      <div class="container py-4">
        <div class="row g-4">
          <div class="col-lg-3" v-for="(card, index) in yearlyStatCards" :key="index">
            <div class="card text-bg-light shadow rounded text-stat h-100 d-flex flex-column text-center">
              <div class="card-body d-flex flex-column justify-content-center">
                <h5 class="card-title">{{ card.title }}</h5>
                <p class="card-text display-6">{{ card.value }}</p>
              </div>
            </div>
          </div>
          <!-- FiR Donut -->
          <div class="col-lg-4">
            <div class="card text-bg-light shadow rounded donut-stat">
              <div class="card-body text-center">
                <apexchart 
                  type="donut" 
                  width="100%" 
                  :options="donutOptions('FiR')" 
                  :series="firSeries">
                </apexchart>
              </div>
            </div>
          </div>

          <!-- GiR Donut -->
          <div class="col-lg-4">
            <div class="card text-bg-light shadow rounded donut-stat">
              <div class="card-body text-center">
                <apexchart 
                  type="donut" 
                  width="100%" 
                  :options="donutOptions('GiR')" 
                  :series="girSeries">
                </apexchart>
              </div>
            </div>
          </div>

          <!-- Up&Down Donut -->
          <div class="col-lg-4">
            <div class="card text-bg-light shadow rounded donut-stat">
              <div class="card-body text-center">
                <apexchart 
                  type="donut" 
                  width="100%" 
                  :options="donutOptions('Up & Down')" 
                  :series="upDownSeries">
                </apexchart>
              </div>
            </div>
          </div>

          <!-- Slag de senaste 10 rundorna -->
          <div class="col-lg-6"> 
            <div class="card text-bg-light shadow rounded bar-stat">
              <div class="card-body">
                <apexchart 
                  type="bar" 
                  height="300"
                  :options="roundChartOptions"
                  :series="roundSeries">
                </apexchart>
              </div>
            </div>
          </div>

          <!-- Mest spelade banor -->
          <div class="col-lg-6">
            <div class="card text-bg-light shadow rounded bar-stat">
              <div class="card-body">
                <apexchart 
                  type="bar" 
                  height="300"
                  :options="courseChartOptions"
                  :series="courseSeries">
                </apexchart>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import ApexChart from 'vue3-apexcharts'
import { useUserStore } from '../stores/userStore'
import { useCourseStore } from '../stores/courseStore'

export default {
  components: {
    apexchart: ApexChart
  },
  data() {
    return {
      userStore: useUserStore(),
      courseStore: useCourseStore(),
      palette: ['#38761d', '#cc0000', '#cc0000', '#cc0000', '#cc0000'],

      mostPlayedCourseData: [],
      mostPlayedCourseNames: [],
    }
  },
  computed: {
    yearlyStatCards() {
      return [
        {
          title: 'Birdies: I år / Totalt:',
          value: `${this.birdiesThisYear} / ${this.birdieCount}`
        },
        {
          title: 'Puttar per hål i år:',
          value: this.avgPuttsPerHole
        },
        {
          title: 'Antal rundor i år:',
          value: this.roundsThisYear
        },
        {
          title: 'Bortslagna bollar i år:',
          value: this.lostBallsThisYear
        }
      ];
    },
    birdieCount() {
      return this.userStore.user.rounds.reduce((total, round) => {
        const birdies = round.statistics?.filter(s => s.birdie)?.length || 0
        return total + birdies
      }, 0)
    },
    birdiesThisYear() {
      const currentYear = new Date().getFullYear()
      return this.userStore.user.rounds.reduce((total, round) => {
        const roundYear = new Date(round.date).getFullYear()
        if (roundYear !== currentYear) return total
        const birdies = round.statistics?.filter(s => s.birdie)?.length || 0
        return total + birdies
      }, 0)
    },
    roundsThisYear() {
      const currentYear = new Date().getFullYear()
      return this.userStore.user.rounds.filter(r => new Date(r.date).getFullYear() === currentYear).length
    },
    firSeries() {
      const allStats = this.userStore?.user?.rounds?.flatMap(r => r.statistics || []) || [];
      const total = allStats.filter(s => ['hit', 'miss-left', 'miss-right', 'miss-short', 'miss-long'].includes(s.fiR)).length;
      const hit = allStats.filter(s => s.fiR === 'hit').length;
      const missLeft = allStats.filter(s => s.fiR === 'miss-left').length;
      const missRight = allStats.filter(s => s.fiR === 'miss-right').length;
      const missShort = allStats.filter(s => s.fiR === 'miss-short').length;
      const missLong = allStats.filter(s => s.fiR === 'miss-long').length;

      return total > 0
        ? [hit, missLeft, missRight, missShort, missLong]
        : [0, 0, 0, 0, 0];
    },
    girSeries() {
      const allStats = this.userStore?.user?.rounds?.flatMap(r => r.statistics || []) || [];
      const total = allStats.filter(s => ['hit', 'miss-left', 'miss-right', 'miss-short', 'miss-long'].includes(s.giR)).length;
      const hit = allStats.filter(s => s.giR === 'hit').length;
      const missLeft = allStats.filter(s => s.giR === 'miss-left').length;
      const missRight = allStats.filter(s => s.giR === 'miss-right').length;
      const missShort = allStats.filter(s => s.giR === 'miss-short').length;
      const missLong = allStats.filter(s => s.giR === 'miss-long').length;

      return total > 0
        ? [hit, missLeft, missRight, missShort, missLong]
        : [0, 0, 0, 0, 0];
    },
    upDownSeries() {
      const allStats = this.userStore.user.rounds.flatMap(r => r.statistics || [])
      const hit = allStats.filter(s => s.upAndDown === true).length
      const miss = allStats.filter(s => s.upAndDown === false).length
      return [hit, miss]
    },
    avgPuttsPerHole() {
      const allStats = this.userStore.user.rounds.flatMap(r => r.statistics || [])
      const totalPutts = allStats.reduce((sum, s) => sum + (s.putts || 0), 0)
      const totalHoles = allStats.length
      return totalHoles > 0 ? (totalPutts / totalHoles).toFixed(2) : 0
    },
    lostBallsThisYear() {
      const currentYear = new Date().getFullYear()
      return this.userStore.user.rounds
        .filter(r => new Date(r.date).getFullYear() === currentYear)
        .flatMap(r => r.statistics || [])
        .reduce((sum, s) => sum + (s.lostBalls || 0), 0)
    },
    last10Rounds() {
      return this.userStore.user.rounds
        .sort((a, b) => new Date(b.date) - new Date(a.date))
        .slice(0, 10)
        .map(r => r.bruttoScore)
        .reverse()
    },
    roundSeries() {
      return [
        {
          name: 'Antal slag',
          data: this.last10Rounds
        }
      ]
    },
    roundChartOptions() {
      return {
        chart: {
          id: 'slag',
          foreColor: '#ffffff'
        },
        xaxis: {
          categories: ['R1','R2','R3','R4','R5','R6','R7','R8','R9','R10'],
          labels: {
            style: {
              colors: '#ffffff'
            }
          }
        },
        yaxis: {
          labels: {
            style: {
              colors: '#ffffff'
            }
          }
        },
        title: {
          text: 'Antal slag de senaste 10 rundorna',
          align: 'center',
          style: {
            color: '#ffffff'
          }
        },
        legend: {
          labels: {
            colors: ['#ffffff']
          }
        },
        tooltip: {
          theme: 'dark'
        }
      }
    },
    mostPlayedCourses() {
      const courseCounts = {}

      this.userStore.user.rounds.forEach(round => {
        const firstHole = round.statistics?.[0]?.hole
        const courseId = firstHole?.courseId
        if (!courseId) return
        
        const course = this.courseStore.getCourseById(courseId)
        const courseName = course?.clubName || 'Okänd bana'
        courseCounts[courseName] = (courseCounts[courseName] || 0) + 1
      })

      const sorted = Object.entries(courseCounts).sort((a, b) => b[1] - a[1]).slice(0, 5)

      return {
        mostPlayedCourseNames: sorted.map(([name]) => name),
        mostPlayedCourseData: sorted.map(([, count]) => count)
      }
    },
    courseSeries() {
      return [
        {
          name: 'Antal rundor',
          data: this.mostPlayedCourses.mostPlayedCourseData
        }
      ]
    },
    courseChartOptions() {
      return {
        chart: {
          id: 'mest-spelade-banor',
          foreColor: '#ffffff'
        },
        xaxis: {
          categories: this.mostPlayedCourses.mostPlayedCourseNames,
          labels: {
            style: {
              colors: '#ffffff'
            }
          }
        },
        yaxis: {
          labels: {
            style: {
              colors: '#ffffff'
            }
          }
        },
        title: {
          text: 'Mest spelade banor',
          align: 'center',
          style: {
            color: '#ffffff'
          }
        },
        legend: {
          labels: {
            colors: ['#ffffff']
          }
        },
        tooltip: {
          theme: 'dark'
        },
      }
    }
  },
  methods: {
    donutOptions(label) {
      const labelsMap = {
        'FiR': ['Träff', 'Miss vänster', 'Miss höger', 'Miss kort', 'Miss lång'],
        'GiR': ['Träff', 'Miss vänster', 'Miss höger', 'Miss kort', 'Miss lång'],
        'Up & Down': [`${label} - Ja`, `${label} - Nej`]
      };

      return {
        labels: labelsMap[label] || [],
        title: {
          text: `${label} % i år`,
          align: 'center',
          style: {
            color: '#ffffff',
            fontSize: '16px'
          }
        },
        legend: {
          position: 'bottom',
          horizontalAlign: 'center',
          fontSize: '14px',
          labels: {
            colors: ['#ffffff', '#ffffff', '#ffffff', '#ffffff', '#ffffff']
          }
        },
        dataLabels: {
          style: {
            colors: ['#ffffff']
          }
        },
        tooltip: {
          theme: 'dark'
        },
        colors: this.palette,
        stroke: {
          show: false
        }
      }
    }
  }
}
</script>

<style scoped>
.card {
  background-color: rgba(0, 0, 0, 0.80) !important;
  border-radius: 12px;
  box-shadow: 0 2px 10px rgba(0,0,0,0.05);
  color: white !important;
}
.dashboard {
  min-height: 100vh;
}
</style>
