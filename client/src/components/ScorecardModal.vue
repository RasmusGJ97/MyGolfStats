<template>
    <div
      v-if="modelValue"
      class="scorecardModal-backdrop"
      @click.self="close()">
      <div class="scorecardModal-dialog">
        <header class="scorecardModal-header">
          <h3>Scorekort</h3>
        </header>

        <section v-if="courseStore.selectedCourse" class="scorecardModal-body">
          <div class="scorecardHeader scorecardBoarder">
            <div class="scorecardCourse d-flex">
              <h5 class="col"> {{ this.courseStore.selectedCourse.clubName }}</h5>
              <div>Datum: {{ round.date }}</div>
            </div>
            <div class="scorecardScores m-3 d-flex">
              <div v-if="round.bruttoScore-this.courseStore.selectedCourse.par > 0" class="col">Brutto Score: {{ round.bruttoScore }} (+{{ round.bruttoScore-this.courseStore.selectedCourse.par }})</div>
              <div v-else-if="round.bruttoScore-this.courseStore.selectedCourse.par < 0" class="col">Brutto Score: {{ round.bruttoScore }} ({{ round.bruttoScore-this.courseStore.selectedCourse.par }})</div>
              <div v-else-if="round.bruttoScore-this.courseStore.selectedCourse.par == 0" class="col">Brutto Score: {{ round.bruttoScore }} (par)</div>
              <div class="col">Netto Score: {{ round.nettoScore }}</div>
              <div class="col">Erhållna slag: {{ round.bruttoScore-round.nettoScore }}</div>
              <div class="col">Tee: {{ round.tee }}</div>
            </div>
          </div>
          <div class="scorecardBody d-flex">
            <div class="scorecardTitles">
              <div class="scorecardRowHeight p-2 scorecardBoarder">Hål</div>
              <div class="scorecardRowHeight p-2 scorecardBoarder">Par</div>
              <div class="scorecardRowHeight p-2 scorecardBoarder">Score</div>
              <div class="scorecardRowHeight p-2 scorecardBoarder">Puttar</div>
              <div class="scorecardRowHeight p-2 scorecardBoarder">GiR</div>
              <div class="scorecardRowHeight p-2 scorecardBoarder">FiR</div>
            </div>

            <div class="scorecardRows w-100" style="height: 30px;">
              <div class="scorecardRowHole scorecardRowHeight p-2 scorecardBoarder d-flex">
                <div v-for="n in 9" :key="'hole-' + n" class="col text-center">
                  {{ n }}
                </div>
                <div class="col text-center fw-bold">UT</div>
                <div v-for="n in 9" :key="'hole-' + (n + 9)" class="col text-center">
                  {{ n + 9 }}
                </div>
                <div class="col text-center fw-bold">IN</div>
                <div class="col text-center fw-bold">Tot</div>
              </div>

              <div class="scorecardRowPar scorecardRowHeight p-2 scorecardBoarder d-flex">
                <div v-for="n in 9" :key="'par-' + n" class="col text-center">
                  {{ getStatByHole(n)?.hole.par ?? '' }}
                </div>
                <div class="col text-center fw-bold">
                  {{ sumStats('par', 1, 9) }}
                </div>
                <div v-for="n in 9" :key="'par-' + (n + 9)" class="col text-center">
                  {{ getStatByHole((n + 9))?.hole.par ?? '' }}
                </div>
                <div class="col text-center fw-bold">
                  {{ sumStats('par', 10, 18) }}
                </div>
                <div class="col text-center fw-bold">
                  {{ sumStats('par', 1, 18) }}
                </div>
              </div>
              <div class="scorecardRowScore scorecardRowHeight p-2 scorecardBoarder d-flex">
                <div v-for="n in 9" :key="'score-' + n" class="col text-center">
                  <div :class="[getScoreClass(n), 'score-circle']">
                    <div :class="[getScoreClass2(n), 'score-circle2']">
                      {{ getStatByHole(n)?.strokes ?? '' }}
                    </div>
                  </div>
                </div>
                <div class="col text-center fw-bold">
                  {{ sumStats('strokes', 1, 9) }}
                </div>
                <div v-for="n in 9" :key="'score-' + (n + 9)" class="col text-center">
                  <div :class="[getScoreClass((n + 9)), 'score-circle']">
                    <div :class="[getScoreClass2((n + 9)), 'score-circle2']">
                      {{ getStatByHole((n + 9))?.strokes ?? '' }}
                    </div>
                  </div>
                </div>
                <div class="col text-center fw-bold">
                  {{ sumStats('strokes', 10, 18) }}
                </div>
                <div class="col text-center fw-bold">
                  {{ sumStats('strokes', 1, 18) }}
                </div>
              </div>
              <div class="scorecardRowPutts scorecardRowHeight p-2 scorecardBoarder d-flex">
                <div v-for="n in 9" :key="'putts-' + n" class="col text-center">
                  {{ getStatByHole(n)?.putts ?? '' }}
                </div>
                <div class="col text-center fw-bold">
                  {{ sumStats('putts', 1, 9) }}
                </div>
                <div v-for="n in 9" :key="'putts-' + (n + 9)" class="col text-center">
                  {{ getStatByHole((n + 9))?.putts ?? '' }}
                </div>
                <div class="col text-center fw-bold">
                  {{ sumStats('putts', 10, 18) }}
                </div>
                <div class="col text-center fw-bold">
                  {{ sumStats('putts', 1, 18) }}
                </div>
              </div>
              <div class="scorecardRowGiR scorecardRowHeight p-2 scorecardBoarder d-flex">
                <div v-for="n in 9" :key="'gir-' + n" class="col text-center">
                  <i :class="getDirectionalIcon(getStatByHole(n)?.giR)" />
                </div>
                <div class="col text-center fw-bold">
                  {{ countHitsInRange('giR', 1, 9) }}
                </div>
                <div v-for="n in 9" :key="'gir-' + (n + 9)" class="col text-center">
                  <i :class="getDirectionalIcon(getStatByHole((n + 9))?.giR)" />
                </div>
                <div class="col text-center fw-bold">
                  {{ countHitsInRange('giR', 10, 18) }}
                </div>
                <div class="col text-center fw-bold">
                  {{ countHitsInRange('giR', 1, 18) }}
                </div>
              </div>

              <div class="scorecardRowFiR scorecardRowHeight p-2 scorecardBoarder d-flex">
                <div v-for="n in 9" :key="'fir-' + n" class="col text-center">
                  <i :class="getDirectionalIcon(getStatByHole(n)?.fiR)" />
                </div>
                <div class="col text-center fw-bold">
                  {{ countHitsInRange('fiR', 1, 9) }}
                </div>
                <div v-for="n in 9" :key="'fir-' + (n + 9)" class="col text-center">
                  <i :class="getDirectionalIcon(getStatByHole((n + 9))?.fiR)" />
                </div>
                <div class="col text-center fw-bold">
                  {{ countHitsInRange('fiR', 10, 18) }}
                </div>
                <div class="col text-center fw-bold">
                  {{ countHitsInRange('fiR', 1, 18) }}
                </div>
              </div>
            </div>
          </div>
          <div class="scorecardFooter m-3 d-flex">
            <div class="col">Puttar: {{ sumStats('putts', 1, 18) }}</div>
            <div class="col">GiR: {{ countHitsInRange('giR', 1, 18) }} / {{ countAvailableInRange('giR', 1, 18) }}</div>
            <div class="col">FiR: {{ countHitsInRange('fiR', 1, 18) }} / {{ countAvailableInRange('fiR', 1, 18) }}</div>
            <div class="col">Birdies: {{ countAvailableInRange('birdie', 1, 18) }}</div>
          </div>
        </section>

        <div class="scorecardModal-footer">
          <button class="btn btn-outline-secondary" @click="close">Stäng</button>
          <button class="btn btn-outline-primary" @click="changeRound">Redigera</button>
        </div>
      </div>
    </div>
</template>

<script>
import { useCourseStore } from '../stores/courseStore'

export default {
  name: 'ScorecardModal',

  inheritAttrs: false,

  props: {
    modelValue: { type: Boolean, default: false },
    round: { type: Object, default: null },
  },

  emits: ['update:modelValue', 'open', 'close', 'change'],

  data() {
    return{
      _lastLoadedCourseId: null,
      isLoadingCourse: false,
      courseStore: useCourseStore(),
    }
  },
  watch: {
    modelValue(isOpen) {
      if (isOpen) this.$emit('open')
      this.loadCourse()
    }
  },
  methods: {
    close() {
      this.$emit('update:modelValue', false)
      this.$emit('close')
    },
    changeRound(){
      this.$emit('change', this.round)
    },
    loadCourse(){
      try {
        const courseId = this.round?.statistics?.[0].hole?.courseId
        if (!courseId){
          console.warn('Ingen courseId hittades...')
        }

        if (this.isLoadingCourse || this._lastLoadedCourseId === courseId) return

        this.isLoadingCourse = true
        const courseStore = useCourseStore();
        const selectedCourse = courseStore.getCourseById(courseId)
        courseStore.setSelectedCourse(selectedCourse)
        this._lastLoadedCourseId = courseId

      } catch (error) {
        console.error('Kunde inte hämta course', error)
      }
      finally {
        this.isLoadingCourse = false
        console.log(this.courseStore.selectedCourse)
        console.log(this.round)
      }
    },
    getStatByHole(holeNumber) {
      return this.round.statistics.find(s => s.hole?.holeNumber === holeNumber) || null
    },
    sumStats(field, start, end) {
      if (!this.round || !this.round.statistics) return 0;

      const filtered = this.round.statistics.filter(stat => {
        const num = Number(stat.hole?.holeNumber);
        const value = field === 'par' ? stat.hole?.par : stat[field];
        return num >= start && num <= end && typeof value === 'number';
      });

      const sum = filtered.reduce((total, stat) => {
        const value = field === 'par' ? stat.hole?.par : stat[field];
        return total + (typeof value === 'number' ? value : 0);
      }, 0);

      return sum;
    },
    getDirectionalIcon(value) {
      switch (value) {
        case 'hit':
          return 'fa-solid fa-check text-success'
        case 'miss-left':
          return 'fa-solid fa-arrow-left text-danger'
        case 'miss-right':
          return 'fa-solid fa-arrow-right text-danger'
        case 'miss-long':
          return 'fa-solid fa-arrow-up text-danger'
        case 'miss-short':
          return 'fa-solid fa-arrow-down text-danger'
        case null:
        case undefined:
          return 'fa-solid fa-minus text-muted'
        default:
          return 'fa-solid fa-question'
      }
    },
    countHitsInRange(type, start, end) {
      return this.round.statistics
        .filter(stat => {
          const value = stat[type];
          const holeNum = stat.hole?.holeNumber;
          return value === 'hit' && holeNum >= start && holeNum <= end;
        }).length;
    },
    countAvailableInRange(field, from, to) {
      return this.round.statistics.filter(stat => {
        const holeNumber = stat.hole?.holeNumber;

        if (holeNumber < from || holeNumber > to) return false;
        const value = stat[field];

        if (typeof value === 'boolean') return value;

        return value !== null && value !== undefined;
      }).length;
    },
    getScoreClass(holeNumber) {
      const stat = this.getStatByHole(holeNumber)
      if (!stat || stat.strokes == null || stat.hole?.par == null) return ''

      const diff = stat.strokes - stat.hole.par
      if (diff <= -1) return 'score-birdie'
      if (diff >= 1) return 'score-bogey'
    },
    getScoreClass2(holeNumber) {
      const stat = this.getStatByHole(holeNumber)
      if (!stat || stat.strokes == null || stat.hole?.par == null) return ''

      const diff = stat.strokes - stat.hole.par
      if (diff <= -2) return 'score-eagle'
      if (diff >= 2) return 'score-double'
    }
  }
}
</script>


<style scoped>
.scorecardModal-backdrop {
  position: fixed;
  inset: 0;
  background: rgba(0,0,0,.6);
  display: grid;
  place-items: center;
  z-index: 1000;
}

.scorecardModal-dialog {
  background: #121212;
  color: #fff;
  width: 65%;
  margin: 1rem;
  border-radius: 12px;
  box-shadow: 0 10px 30px rgba(0,0,0,.35);
  outline: none;
}

@media (max-width: 1280px) {
  .scorecardModal-dialog {
    width: 90%;
  }
}

@media (max-width: 920px) {
  .scorecardModal-dialog {
    width: 100%;
  }
}

.scorecardModal-header {
  display: flex;
  align-items: center;
  gap: .75rem;
  padding: 1rem 1.25rem;
  border-bottom: 1px solid rgba(255,255,255,.1);
}

.scorecardModal-body { 
  padding: 1rem 1.25rem; 
  border: 1px solid rgba(255,255,255,.1);
  margin: 1rem;
}

.scorecardModal-footer {
  display: flex;
  justify-content: flex-end;
  gap: .5rem;
  padding: .75rem 1.25rem 1.25rem;
  border-top: 1px solid rgba(255,255,255,.1);
}

.scorecardBoarder {
  border-bottom: 1px solid rgba(255,255,255,.1);
}
.scorecardRowHeight {
  height: 42px;
  align-items: center;
}

.score-circle {
  width: 32px;
  aspect-ratio: 1;
  display: flex;
  justify-content: center;
  align-items: center;
  margin: -3px auto;
  box-sizing: border-box;
  line-height: 1;
}

.score-circle2 {
  width: 24px;
  aspect-ratio: 1;
  display: flex;
  justify-content: center;
  align-items: center;
  box-sizing: border-box;
}

.score-birdie {
  border: 2px solid red;
  border-radius: 50%;
}

.score-bogey {
  border: 2px solid rgb(0, 55, 255);
}

.score-eagle {
  border: 1px solid red;
  border-radius: 50%;
  margin: 3px;
}

.score-double {
  border: 1px solid rgb(0, 55, 255);
  margin: 3px;
}
</style>
