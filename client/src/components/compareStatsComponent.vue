<template>
  <div class="statistik-box p-3 text-white rounded">
    <div class="statistik-container">
      <div class="container py-4">
        <div class="row justify-content-center g-4" v-if="selectedSubFilter != 'compHcp'">
          <div class="col-12 col-md-6 col-lg-4" v-for="(card, index) in statCards" :key="index">
            <div class="card golf-dark shadow rounded text-stat h-100 d-flex flex-column text-light text-center">
              <div class="card-body d-flex flex-column justify-content-center">
                <h5 class="card-title mb-3">{{ card.title }}</h5>              
                <p class="mb-3 text-light">
                  {{ formatComparisonTitle(selectedFilter, selectedSubFilter) }}
                </p>
                <div class="d-flex justify-content-between align-items-center mx-auto">
                  <span class="card-text display-6">{{ card.value }}</span>
                  <span class="mx-4" v-html="getComparisonIcon(card.value, card.comp, card.betterDirection)"></span>
                  <span class="card-text display-6">{{ card.comp }}</span>
                </div>
              </div>
            </div>
          </div>
        </div>
        <div class="row justify-content-center g-4" v-if="selectedSubFilter === 'compHcp'">
          <div class="col-12 col-md-6 col-lg-4" v-for="(card, index) in statCardsHcp" :key="index">
            <div class="card golf-dark shadow rounded text-stat h-100 d-flex flex-column text-light text-center">
              <div class="card-body d-flex flex-column justify-content-center">
                <h5 class="card-title mb-3">{{ card.title }}</h5>              
                <p class="mb-3 text-light">
                  {{ formatComparisonTitle(selectedFilter, selectedSubFilter) }}
                </p>
                <div class="d-flex justify-content-between align-items-center mx-auto">
                  <span class="card-text display-6">{{ card.value }}</span>
                  <span class="mx-4" v-html="getComparisonIcon(card.value, card.comp, card.betterDirection)"></span>
                  <span class="card-text display-6">{{ card.comp }}</span>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>


<script>
export default {
  name: 'StatistikCompare',
  props: {
    statistics: {
      type: Array,
      required: true
    },
    statisticsToCompareWith: {
      type: Array,
      required: true
    },
    selectedFilter: String,
    selectedSubFilter: String,
  },
  data() {
  return {
    filterDisplayMap: {
      all: 'Totalt',
      thisYear: 'I år',
      last10: 'Senaste 10',
      last5: 'Senaste 5',
      sameHcp: 'Liknande HCP',
      compTotal: 'Totalt',
      compYear: 'I år',
      compLast10: 'Senaste 10',
      compLast5: 'Senaste 5',
      compHcp: 'Liknande HCP',
    }
  };
},
  computed: {
    allStats() {
      return this.statistics.flatMap(round => round.statistics || []);
    },
    allCompareStats() {
      if(this.selectedSubFilter != 'compHcp'){
        return this.statisticsToCompareWith.flatMap(round => round.statistics || []);
      }
    },
    statCards() {
      return [
        { title: 'Snitt Pliktslag:', value: this.averagePenaltyStrokes(this.allStats), comp: this.averagePenaltyStrokes(this.allCompareStats), betterDirection: 'lower'  },
        { title: 'Puttar per hål:', value: this.avgPuttsPerHole(this.allStats), comp: this.avgPuttsPerHole(this.allCompareStats), betterDirection: 'lower'  },
        { title: 'FiR %:', value: this.firSeries(this.allStats) + '%', comp: this.firSeries(this.allCompareStats) + '%', betterDirection: 'higher'  },
        { title: 'GiR %:', value: this.girSeries(this.allStats) + '%', comp: this.girSeries(this.allCompareStats) + '%', betterDirection: 'higher'  },
        { title: 'Up & Down %:', value: this.upDownSeries(this.allStats) + '%', comp: this.upDownSeries(this.allCompareStats) + '%', betterDirection: 'higher'  },
        { title: 'Sand Save %:', value: this.sandSeries(this.allStats) + '%', comp: this.sandSeries(this.allCompareStats) + '%', betterDirection: 'higher'  },
      ]
    },
    statCardsHcp(){
      return [
        { title: 'Puttar per hål:', value: this.avgPuttsPerHole(this.allStats), comp: this.compareStatsHcp.hcpPutts, betterDirection: 'lower'  },
        { title: 'FiR %:', value: this.firSeries(this.allStats) + '%', comp: this.compareStatsHcp.hcpFiR + '%', betterDirection: 'higher'  },
        { title: 'GiR %:', value: this.girSeries(this.allStats) + '%', comp: this.compareStatsHcp.hcpGiR + '%', betterDirection: 'higher'  },
      ]
    },
    compareStatsHcp() {
      const result = {};
      this.statisticsToCompareWith.forEach(item => {
        const key = Object.keys(item)[0];
        result[key] = item[key];
      });
      return result;
    }
  },
  methods: {
    formatComparisonTitle(filter, subFilter) {
      const prettyFilter = this.filterDisplayMap[filter] || filter;
      const prettySubFilter = this.filterDisplayMap[subFilter] || subFilter;

      if (!prettyFilter && !prettySubFilter) return '';
      if (prettyFilter && prettySubFilter) return `${prettyFilter} vs ${prettySubFilter}`;
      if (prettyFilter) return `${prettyFilter}`;
      return `${prettySubFilter}`;
    },
    getComparisonIcon(value, comp, betterDirection = 'higher') {
      if (value == null || comp == null) {
        return '<i class="fas fa-question text-secondary"></i>';
      }

      const parseValue = val => {
        if (typeof val === 'string' && val.endsWith('%')) {
          return parseFloat(val.replace('%', ''));
        }
        return isNaN(val) ? val : Number(val);
      };

      const parsedValue = parseValue(value);
      const parsedComp = parseValue(comp);

      if (typeof parsedValue !== 'number' || typeof parsedComp !== 'number') {
        return '<i class="fas fa-question text-secondary"></i>';
      }

      if (parsedValue === parsedComp) {
        return '<i class="fas fa-minus text-secondary"></i>';
      }

      const isBetter = betterDirection === 'higher'
        ? parsedValue > parsedComp
        : parsedValue < parsedComp;

      return isBetter
        ? '<i class="fas fa-arrow-up text-success tilted-arrow-green"></i>'
        : '<i class="fas fa-arrow-down text-danger tilted-arrow-red"></i>';
    },

    firSeries(allStats) {
      const relevant = allStats.filter(s => ['hit', 'miss-left', 'miss-right', 'miss-short', 'miss-long'].includes(s.fiR));
      const total = relevant.length;
      const hit = relevant.filter(s => s.fiR === 'hit').length;

      return total > 0 ? Math.round((hit / total) * 100) : 0;
    },

    girSeries(allStats) {
      const relevant = allStats.filter(s => ['hit', 'miss-left', 'miss-right', 'miss-short', 'miss-long'].includes(s.giR));
      const total = relevant.length;
      const hit = relevant.filter(s => s.giR === 'hit').length;

      return total > 0 ? Math.round((hit / total) * 100) : 0;
    },

    upDownSeries(allStats) {
      const total = allStats.filter(s => s.upAndDown !== null).length;
      const hit = allStats.filter(s => s.upAndDown === true).length;

      return total > 0 ? Math.round((hit / total) * 100) : 0;
    },

    sandSeries(allStats) {
      const total = allStats.filter(s => s.sandSave !== null).length;
      const hit = allStats.filter(s => s.sandSave === true).length;

      return total > 0 ? Math.round((hit / total) * 100) : 0;
    },

    averagePenaltyStrokes(allStats) {
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

    avgPuttsPerHole(allStats) {
      const totalPutts = allStats.reduce((sum, s) => sum + (s.putts || 0), 0)
      const totalHoles = allStats.length
      return totalHoles > 0 ? (totalPutts / totalHoles).toFixed(2) : 0
    },

    penaltyCauseData(allStats) {
      const causeCount = {};

      allStats.forEach(stat => {
        if (Array.isArray(stat.penaltyCause)) {
          stat.penaltyCause.forEach(cause => {
            const name = cause.cause || 'Okänd orsak';

            if (!causeCount[name]) {
              causeCount[name] = 0;
            }

            causeCount[name] += (typeof stat.penaltyStrokes === 'number' ? stat.penaltyStrokes : 0);
          });
        }
      });

      return causeCount;
    },

    clubPenaltyData(allStats) {
      const clubCount = {};
      const nameMap = this.clubNameMap();

      allStats.forEach(stat => {
        if (Array.isArray(stat.penaltyCause)) {
          stat.penaltyCause.forEach(cause => {
            const rawClub = cause.club || 'unknown';
            const readableClub = nameMap[rawClub] || rawClub;

            if (!clubCount[readableClub]) {
              clubCount[readableClub] = 0;
            }

            clubCount[readableClub] += (typeof stat.penaltyStrokes === 'number' ? stat.penaltyStrokes : 0);
          });
        }
      });

      return clubCount;
    },
  }
};
</script>

<style>
.tilted-arrow-green {
  display: inline-block;
  transform: rotate(45deg);
}
.tilted-arrow-red {
  display: inline-block;
  transform: rotate(-45deg);
}
</style>
