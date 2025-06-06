<template>
  <div class="statistik-box p-3 text-white rounded">
    <div class="statistik-container">
      <div class="container py-4">
        <div class="row justify-content-center g-4">
          <div class="col-12 col-sm-6 col-lg-4 col-xl-3" v-for="(card, index) in statCards" :key="index">
            <div class="card text-bg-light shadow rounded text-stat h-100 d-flex flex-column text-center">
              <div class="card-body d-flex flex-column justify-content-center">
                <h5 class="card-title">{{ card.title }}</h5>
                <p class="card-text display-6">{{ card.value }}</p>
              </div>
            </div>
          </div>
        </div>
      </div>
      <div class="chart-row">
        <apexchart class="golf-dark p-3 text-white rounded"
          type="donut"
          :series="firSeries"
          :options="donutOptions('FiR')"
          width="300"
        />
        <apexchart class="golf-dark p-3 text-white rounded"
          type="donut"
          :series="girSeries"
          :options="donutOptions('GiR')"
          width="300"
        />
        <apexchart class="golf-dark p-3 text-white rounded"
          type="donut"
          :series="upDownSeries"
          :options="donutOptions('Up & Down')"
          width="300"
        />
        <apexchart class="golf-dark p-3 text-white rounded"
          type="donut"
          :series="sandSeries"
          :options="donutOptions('Sand Save')"
          width="300"
        />
        <apexchart class="golf-dark p-3 text-white rounded"
          type="donut"
          :options="penaltyDonutChart.options"
          :series="penaltyDonutChart.series"
          width="300"
        />
        <apexchart class="golf-dark p-3 text-white rounded"
          type="donut"
          :options="clubDonut.options"
          :series="clubDonut.series"
          width="300"
        />
      </div>
      <div class="chart-row" style="max-height: 400px; overflow-y: auto; margin-top: 2rem;">
        <apexchart class="golf-dark p-3 text-white rounded"
          v-if="selectedSubFilter === 'all'"
          width="500"
          type="bar"
          :options="courseBarChart.options"
          :series="courseBarChart.series"
        />
      </div>
    </div>
  </div>
</template>

<script>
import ApexChart from 'vue3-apexcharts'
import { useCourseStore } from '../stores/courseStore'

export default {
  name: 'StatistikVisning',
  components: {
    apexchart: ApexChart
  },
  props: {
    statistics: {
      type: Array,
      required: true
    },
    selectedFilter: String,
    selectedSubFilter: String,
    selectedCourseFilter: [String, Number],
    selectedHoleFilter: [String, Object]
  },
  data() {
    return {
      palette: ['#38761d', '#cc0000', '#cc0000', '#cc0000', '#cc0000'],
      palette2: [
        '#1f77b4', // blå
        '#ff7f0e', // orange
        '#2ca02c', // grön
        '#d62728', // röd
        '#9467bd', // lila
        '#8c564b', // brun
        '#e377c2', // rosa
        '#7f7f7f', // grå
        '#bcbd22', // gulgrön
        '#17becf', // turkos
        '#aec7e8', // ljusblå
        '#ffbb78', // ljusorange
        '#98df8a', // ljusgrön
        '#ff9896', // ljusröd
        '#c5b0d5', // ljuslila
        '#c49c94', // ljusbrun
        '#f7b6d2', // ljusrosa
        '#c7c7c7', // ljusgrå
        '#dbdb8d', // ljus gulgrön
        '#9edae5'  // ljus turkos
      ]
    }
  },
  computed: {
    statCards() {
      return [
        { title: 'Eagles:', value: this.eagleCount },
        { title: 'Birdies:', value: this.birdieCount },
        { title: 'Puttar per hål:', value: this.avgPuttsPerHole },
        { title: 'Antal rundor:', value: this.statistics.length },
        { title: 'Bortslagna bollar:', value: this.lostBalls },
        { title: 'Snitt Score:', value: this.averageScore },
        { title: 'Bästa Score:', value: this.bestScore },
        { title: 'Pliktslag:', value: this.totalPenaltyStrokes },
        { title: 'Snitt Pliktslag:', value: this.averagePenaltyStrokes },
      ]
    },
    normalizedStatistics() {
      if (
        this.statistics.length &&
        this.statistics[0]?.statistics &&
        Array.isArray(this.statistics[0].statistics)
      ) {
        return this.statistics.flatMap(r => r.statistics)
      }
      return this.statistics
    },

    averageScore() {
      if (!this.statistics || this.statistics.length === 0) return 0;

      if (this.selectedSubFilter !== 'hole') {
        const fullRounds = this.statistics.filter(round =>
          Array.isArray(round.statistics) && round.statistics.length ===18
        );
        if (fullRounds.length === 0) return 0;
        
        const totalScore = fullRounds.reduce((sum, round) => {
          const score = typeof round.bruttoScore === 'number' ? round.bruttoScore : 0;
          return sum + score;
        }, 0);
        
        return (totalScore / fullRounds.length).toFixed(1);
      }
      
      const validHoleStats = this.statistics.filter(stat => typeof stat.strokes === 'number');
      
      if (validHoleStats.length === 0) return 0;

      const totalHoleScore = validHoleStats.reduce((sum, stat) => sum + stat.strokes, 0);
      return (totalHoleScore / validHoleStats.length).toFixed(1);
    },

    bestScore() {
      if (!this.statistics || this.statistics.length === 0) return 0;

      if (this.selectedSubFilter !== 'hole') {
        const fullRounds = this.statistics.filter(round =>
          Array.isArray(round.statistics) && round.statistics.length === 18 &&
          typeof round.bruttoScore === 'number'
        );

        if (fullRounds.length === 0) return 0;

        const best = fullRounds.reduce((min, round) => {
          return round.bruttoScore < min ? round.bruttoScore : min;
        }, Infinity);

        return best === Infinity ? 0 : best;
      }

      const validHoleStats = this.statistics.filter(stat =>
        typeof stat.strokes === 'number'
      );

      if (validHoleStats.length === 0) return 0;

      const bestHole = validHoleStats.reduce((min, stat) => {
        return stat.strokes < min ? stat.strokes : min;
      }, Infinity);

      return bestHole === Infinity ? 0 : bestHole;
    },

    averagePenaltyStrokes() {
      const allStats = this.normalizedStatistics;
      if (!allStats || allStats.length === 0) return 0;

      let relevantStats = allStats;

      if (this.selectedSubFilter === 'hole' && this.selectedHoleFilter?.holeNumber) {
        relevantStats = allStats.filter(stat => stat.hole?.holeNumber === this.selectedHoleFilter.holeNumber);
      }

      if (relevantStats.length === 0) return 0;

      const totalPenalty = relevantStats.reduce((sum, stat) => {
        return sum + (typeof stat.penaltyStrokes === 'number' ? stat.penaltyStrokes : 0);
      }, 0);

      if (this.selectedSubFilter === 'hole' && this.selectedHoleFilter?.holeNumber) {
        return (totalPenalty / relevantStats.length).toFixed(1);
      }

      const totalHoles = relevantStats.length;
      const roundsEquivalent = totalHoles / 18;
      return (totalPenalty / roundsEquivalent).toFixed(1);
    },

    firSeries() {
      const allStats = this.normalizedStatistics;
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
      const allStats = this.normalizedStatistics;
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
      const total = this.normalizedStatistics.filter(s => s.upAndDown !== null).length
      const hit = this.normalizedStatistics.filter(s => s.upAndDown === true).length
      const percent = total > 0 ? Math.round((hit / total) * 100) : 0
      return [percent, 100 - percent]
    },

    sandSeries() {
      const total = this.normalizedStatistics.filter(s => s.sandSave !== null).length
      const hit = this.normalizedStatistics.filter(s => s.sandSave === true).length
      const percent = total > 0 ? Math.round((hit / total) * 100) : 0
      return [percent, 100 - percent]
    },

    avgPuttsPerHole() {
      const validStats = this.normalizedStatistics.filter(s => s.putts != null);
      const totalPutts = validStats.reduce((sum, s) => sum + s.putts, 0);
      const totalHoles = validStats.length;
      return totalHoles > 0 ? (totalPutts / totalHoles).toFixed(2) : 0;
    },

    birdieCount() {
      return this.normalizedStatistics.filter(s => s.birdie).length
    },

    totalPenaltyStrokes() {
      return this.normalizedStatistics.reduce((sum, s) => {
        return sum + (typeof s.penaltyStrokes === 'number' ? s.penaltyStrokes : 0);
      }, 0);
    },

    eagleCount() {
      return this.normalizedStatistics.filter(s => s.eagle).length
    },

    lostBalls() {
      return this.normalizedStatistics.reduce((sum, s) => sum + (s.lostBalls || 0), 0)
    },

    penaltyCauseData() {
      const allStats = this.normalizedStatistics;
      const causeCount = {};

      allStats.forEach(stat => {
        if (Array.isArray(stat.penaltyCause)) {
          stat.penaltyCause.forEach(cause => {
            const name = cause.cause || 'Okänd orsak';
            const strokes = typeof cause.penaltyStrokes === 'number' ? cause.penaltyStrokes : 0;

            if (!causeCount[name]) {
              causeCount[name] = 0;
            }

            causeCount[name] += strokes;
          });
        }
      });

      return causeCount;
    },

    clubPenaltyData() {
      const allStats = this.normalizedStatistics;
      const clubCount = {};
      const nameMap = this.clubNameMap();

      allStats.forEach(stat => {
        if (Array.isArray(stat.penaltyCause)) {
          stat.penaltyCause.forEach(cause => {
            const rawClub = cause.club || 'unknown';
            const readableClub = nameMap[rawClub] || rawClub;
            const strokes = typeof cause.penaltyStrokes === 'number' ? cause.penaltyStrokes : 0;

            if (!clubCount[readableClub]) {
              clubCount[readableClub] = 0;
            }

            clubCount[readableClub] += strokes;
          });
        }
      });

      return clubCount;
    },

    courseChartData() {
      const data = this.coursePlayCount();
      if (!data) return null;

      const courseStore = useCourseStore();

      const dataWithNames = Object.entries(data).map(([courseId, count]) => {
        const courseName = courseStore.getCourseById(courseId)?.clubName || `Okänd bana (${courseId})`;
        return { name: courseName, count };
      });

      const sorted = dataWithNames.sort((a, b) => b.count - a.count);

      return {
        labels: sorted.map(x => x.name),
        datasets: [
          {
            label: 'Rounds Played',
            backgroundColor: '#4B9CD3',
            data: sorted.map(x => x.count)
          }
        ]
      };
    },

    penaltyDonutChart() {
      return {
        series: Object.values(this.penaltyCauseData),
        options: this.getPenaltyDonutOptions()
      };
    },

    clubDonut() {
      return {
        series: Object.values(this.clubPenaltyData),
        options: this.getPenaltyClubDonutOptions()
      };
    },
    courseBarChart() {
      const chartData = this.courseChartData;
      if (!chartData) return { series: [], options: {} };

      return {
        series: chartData.datasets.map(ds => ({
          name: ds.label,
          data: ds.data
        })),
        options: this.courseChartOptions(chartData.labels)
      };
    }
  },
  methods: {
    donutOptions(label) {
      const labelsMap = {
        'FiR': ['Träff', 'Miss vänster', 'Miss höger', 'Miss kort', 'Miss lång'],
        'GiR': ['Träff', 'Miss vänster', 'Miss höger', 'Miss kort', 'Miss lång'],
        'Up & Down': [`${label} - Ja`, `${label} - Nej`],
        'Sand Save': [`${label} - Ja`, `${label} - Nej`]
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
    },
    getPenaltyDonutOptions() {
      const causeData = this.penaltyCauseData;
      return {
        series: Object.values(causeData),
        labels: Object.keys(causeData),
        title: {
          text: `Plikt - Orsak`,
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
            colors: ['#ffffff', '#ffffff']
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
        colors: this.palette2,
        stroke: {
          show: false
        },
        chart: {
          foreColor: '#ffffff'
        }
      }
    },
    getPenaltyClubDonutOptions() {
      const causeData = this.clubPenaltyData;
      return {
        series: Object.values(causeData),
        labels: Object.keys(causeData),
        title: {
          text: `Plikt - Klubba`,
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
            colors: ['#ffffff', '#ffffff']
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
        colors: this.palette2,
        stroke: {
          show: false
        },
        chart: {
          foreColor: '#ffffff'
        }
      }
    },
    courseChartOptions(labels) {
      return {
        chart: {
          type: 'bar',
          foreColor: '#ffffff',
          toolbar: {
            show: false
          }
        },
        title: {
          text: 'Antal rundor per bana',
          style: {
            color: '#ffffff'
          }
        },
        plotOptions: {
          bar: {
            horizontal: true,
            distributed: true
          }
        },
        xaxis: {
          categories: labels,
          labels: {
            style: {
              colors: '#ffffff'
            }
          }
        },
        dataLabels: {
          enabled: false
        },
        legend: {
          labels: {
            colors: '#ffffff'
          }
        },
        tooltip: {
          theme: 'dark'
        },
        colors: this.palette2
      };
    },

    coursePlayCount() {
      if (this.selectedSubFilter !== 'all') return [];

      const courseCounts = {};
      const seenRounds = new Set();

      this.normalizedStatistics.forEach(stat => {
        const roundId = stat.roundId;
        const hole = stat.hole;

        if (!seenRounds.has(roundId) && hole?.holeNumber === 1) {
          seenRounds.add(roundId);
          const courseId = hole.courseId;

          if (!courseCounts[courseId]) {
            courseCounts[courseId] = 0;
          }
          courseCounts[courseId]++;
        }
      });

      return courseCounts;
    },
    clubNameMap() {
      return {
        driver: 'Driver',
        twoW: '2 Wood',
        threeW: '3 Wood',
        fourW: '4 Wood',
        fiveW: '5 Wood',
        sixW: '6 Wood',
        sevenW: '7 Wood',
        twoHy: '2 Hybrid',
        threeHy: '3 Hybrid',
        fourHy: '4 Hybrid',
        fiveHy: '5 Hybrid',
        oneI: '1 Iron',
        twoI: '2 Iron',
        threeI: '3 Iron',
        fourI: '4 Iron',
        fiveI: '5 Iron',
        sixI: '6 Iron',
        sevenI: '7 Iron',
        eightI: '8 Iron',
        nineI: '9 Iron',
        pWedge: 'Pitching Wedge',
        aWedge: 'Approach Wedge',
        gWedge: 'Gap Wedge',
        sWedge: 'Sand Wedge',
        lWedge: 'Lob Wedge',
        unknown: 'Okänd klubba'
      };
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
.statistik-container {
  display: flex;
  flex-direction: column;
  align-items: center;
}
.chart-row {
  display: flex;
  gap: 2rem;
  flex-wrap: wrap;
  justify-content: center;
}
</style>