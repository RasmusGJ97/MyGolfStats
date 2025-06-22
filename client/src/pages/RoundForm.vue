<template>
  <div class="background-image">
    <div class="invisible-div golf-dark text-light pb-2 rounded">
      <div class="container mt-4 text-light">
        <h2>{{ isEdit ? 'Redigera golfrunda' : 'Lägg till golfrunda' }}</h2>

        <form @submit.prevent="onSubmit">
          <div class="row mb-3 align-items-end">
            <div class="col-6">
              <label class="form-label">Välj golfbana:</label>
              <select v-model="selectedCourseId" class="form-select" @change="onCourseSelected">
                <option disabled value="">-- Välj bana --</option>
                <option v-for="course in courses" :value="course.id" :key="course.id">
                  {{ course.clubName }}
                </option>
              </select>
            </div>

            <div class="col-md-6">
              <label class="form-label">Datum:</label>
              <Datepicker v-model="round.date" :format="'yyyy-MM-dd'" :input-class="'form-control form-control-sm'" required :teleport="'body'" />
            </div>
          </div>

          <div v-if="selectedCourseId" class="row mb-3">
            <div class="col-md-4">
              <label class="form-label">Tee:</label>
              <select v-model="round.tee" class="form-select form-select-sm" required>
                <option disabled value="">-- Välj tee --</option>
                <option v-for="tee in availableTees" :key="tee" :value="tee">{{ tee }}</option>
              </select>
            </div>

            <div class="col-md-4">
              <label class="form-label">Hål spelade:</label>
              <input v-model.number="round.holesPlayed" type="number" min="1" :max="selectedCourse?.holes?.length"  class="form-control form-control-sm" required />
            </div>

            <div class="col-md-4">
              <label class="form-label">Erhållna slag:</label>
              <input v-model.number="round.gainedStrokes" type="number" min="0" class="form-control form-control-sm" />
            </div>
          </div>

          <div class="row justify-content-center" v-if="selectedCourseId && userSettings">
            <div
              v-for="(stat, index) in round.statistics"
              :key="index"
              class="col-12 col-md-5-5 mx-auto m-2 p-3 border rounded bg-dark text-light">
            <h5>Hål {{ stat.hole?.holeNumber || index + 1 }} - (Par: {{ stat.hole?.par }})</h5>
  
              <div class="row g-2">
                <div class="col-6 col-md-4">
                  <label class="form-label">Slag:</label>
                  <input v-model.number="stat.strokes" type="number" min="0" class="form-control form-control-sm" />
                </div>
  
                <div class="col-6 col-md-4" v-if="userSettings.statPutts">
                  <label class="form-label">Puttar:</label>
                  <input
                    :value="stat.putts"
                    @input="val => stat.putts = val.target.value === '' ? null : Number(val.target.value)"
                    type="number"
                    min="0"
                    class="form-control form-control-sm"
                  />                
                </div>
  
                <div class="col-6 col-md-4" v-if="userSettings.statLostBalls">
                  <label class="form-label">Förlorade bollar:</label>
                  <input v-model.number="stat.lostBalls" type="number" min="0" class="form-control form-control-sm" />
                </div>
  
                <div class="col-12">
                  <label class="form-label d-block mb-2">Statistik:</label>
                  <div class="row g-2 align-items-center">
                    <div v-if="stat?.hole?.par !== 3 && userSettings.statFiR" class="col-3">
                      <label class="form-label mb-1">FIR</label>
                      <select class="form-select form-select-sm" v-model="stat.fiR">
                        <option :value="null">--</option>
                        <option value="hit">Träff</option>
                        <option value="miss-left">Miss vänster</option>
                        <option value="miss-right">Miss höger</option>
                        <option value="miss-short">Miss kort</option>
                        <option value="miss-long">Miss lång</option>
                      </select>
                    </div>

                    <div v-if="userSettings.statGiR" class="col-3">
                      <label class="form-label mb-1">GIR</label>
                      <select class="form-select form-select-sm" v-model="stat.giR">
                        <option :value="null">--</option>
                        <option value="hit">Träff</option>
                        <option value="miss-left">Miss vänster</option>
                        <option value="miss-right">Miss höger</option>
                        <option value="miss-short">Miss kort</option>
                        <option value="miss-long">Miss lång</option>
                      </select>
                    </div>

                    <div v-if="userSettings.statUpAndDown" class="col-3">
                      <label class="form-label mb-1">Up & Down</label>
                      <select class="form-select form-select-sm" v-model="stat.upAndDown">
                        <option :value="null">--</option>
                        <option :value="true">Ja</option>
                        <option :value="false">Nej</option>
                      </select>
                    </div>

                    <div v-if="userSettings.statSandSave" class="col-3">
                      <label class="form-label mb-1">Sand Save</label>
                      <select class="form-select form-select-sm" v-model="stat.sandSave">
                        <option :value="null">--</option>
                        <option :value="true">Ja</option>
                        <option :value="false">Nej</option>
                      </select>
                    </div>
                  </div>
                </div>
              </div>
  
              <hr class="bg-light my-3" />
              <h6 v-if="userSettings.statPenaltyStrokes || userSettings.statPenaltyCause">Plikt:</h6>
  
              <div v-for="(penalty, pIndex) in stat.penaltyCause" :key="pIndex" class="row g-2 align-items-end mb-2">
                <div class="col-6 col-md-3" v-if="userSettings.statPenaltyCause">
                  <label class="form-label">Orsak:</label>
                  <select v-model="penalty.cause" class="form-select form-select-sm">
                    <option disabled value="">-- Välj orsak --</option>
                    <option>Vattenhinder</option>
                    <option>Out of bounds</option>
                    <option>Ospelbar boll</option>
                    <option>Annat</option>
                  </select>
                </div>
                <div class="col-6 col-md-3" v-if="userSettings.statPenaltyCause">
                  <label class="form-label">Klubba:</label>
                    <select v-model="penalty.club" class="form-select form-select-sm">
                      <option disabled value="">-- Välj klubba --</option>
                      <option v-for="club in activeClubs" :key="club.key" :value="club.key">
                        {{ club.label }}
                      </option>
                    </select>
                </div>
                <div class="col-6 col-md-3" v-if="userSettings.statPenaltyStrokes">
                  <label class="form-label">Pliktslag:</label>
                  <input v-model.number="penalty.penaltyStrokes" type="number" min="0" class="form-control form-control-sm" />
                </div>
                <div class="col-1">
                  <button
                    class="btn btn-outline-danger btn-sm mt-2"
                    @click.prevent="stat.penaltyCause.splice(pIndex, 1)"
                    title="Ta bort orsak">
                    <i class="fas fa-trash"></i>
                  </button>
                </div>
              </div>
  
              <button
                class="btn btn-sm btn-outline-light mt-2" v-if="userSettings.statPenaltyStrokes || userSettings.statPenaltyCause"
                @click.prevent="stat.penaltyCause.push({ cause: '', club: '', penaltyStrokes: 0 })">
                Lägg till orsak
              </button>
            </div>
          </div>


          <div class="d-flex justify-content-end mt-3 mb-3">
            <router-link to="/rounds" class="btn btn-outline-secondary">Avbryt</router-link>
            <button type="submit" class="btn btn-outline-primary ms-3">Spara</button>
          </div>
        </form>
      </div>
    </div>
  </div>
</template>


<script>
import { useUserStore } from '../stores/userStore';
import { addRound, updateRound, getAllCourses } from '../api/MyGolfStatsApi.js'
import Datepicker from '@vuepic/vue-datepicker';
import '@vuepic/vue-datepicker/dist/main.css';

export default {
  name: "RoundForm",
  components: {
    Datepicker
  },
  data() {
    const userStore = useUserStore();
    return {
      userStore,
      round: {
        date: "",
        tee: "",
        holesPlayed: 0,
        gainedStrokes: 0,
        userId: "",
        statistics: [
          {
            id:0,
            fiR: null,
            giR: null,
            birdie: false,
            eagle: false,
            penaltyStrokes: 0,
            penaltyCause: [
              {
                id: 0,
                cause: "",
                club: "",
                penaltyStrokes: 0
              }
            ],
            upAndDown: null,
            sandSave: null,
            putts: 0,
            strokes: 0,
            holeId: 0,
            hole: {
              id: 0,
              holeNumber: 1,
              par: 3,
              courseId: 0
            },
            lostBalls: 0,
            roundId: 0
          }
        ]
      },
      isEdit: false,
      gainedStrokes: 0,
      courses: [],
      selectedCourseId: "",
      availableTees: [],
    };
  },
  async created() {
    const userStore = useUserStore();
    this.round.userId = userStore.user?.id || "";
    this.courses = await getAllCourses();
    const id = this.$route.params.id;

    if (id && id !== "new") {
      this.isEdit = true;

      const existing = userStore.getRoundById(id) || userStore.selectedRound;

      if (existing) {
        this.round = { ...existing };

        const firstStat = this.round.statistics?.[0];
        if (firstStat?.hole?.courseId) {
          this.selectedCourseId = firstStat.hole.courseId;

          const selectedCourse = this.courses.find(c => c.id === this.selectedCourseId);
          if (selectedCourse) {
            this.availableTees = selectedCourse.tees || [];
          }
        }

        if (this.round.bruttoScore != null && this.round.nettoScore != null) {
          this.round.gainedStrokes = this.round.bruttoScore - this.round.nettoScore;
        }
      } else {
        await userStore.fetchUser();
        const found = userStore.getRoundById(id);
        if (found) this.round = { ...found };
      }
    }
  },
  watch: {
    'round.holesPlayed'(newCount) {
      this.adjustHoleStatistics(newCount);
    }
  },
  computed: {
    selectedCourse(){
      return this.courses.find(c => c.id === this.selectedCourseId);
    },
    activeClubs() {
      const userStore = useUserStore();
      const bag = userStore.user?.bag || {};
      const nameMap = {
        oneI: "1I",
        twoI: "2I",
        threeI: "3I",
        fourI: "4I",
        fiveI: "5I",
        sixI: "6I",
        sevenI: "7I",
        eightI: "8I",
        nineI: "9I",
        driver: "Driver",
        twoW: "2W",
        threeW: "3W",
        fiveW: "5W",
        sevenW: "7W",
        sixW: "6W",
        fourW: "4W",
        aWedge: "A Wedge",
        sWedge: "S Wedge",
        lWedge: "L Wedge",
        gWedge: "G Wedge",
        threeHy: "3 Hy",
        fourHy: "4 Hy",
        fiveHy: "5 Hy",
        twoHy: "2 Hy"
      };

      return Object.entries(bag)
        .filter(([key, isActive]) =>
          isActive && key !== 'id' && key !== 'userId'
        )
        .map(([clubKey]) => ({
          key: clubKey,
          label: nameMap[clubKey] || clubKey
      }));
    },
    userSettings() {
      return this.userStore.user.settings;
    }
  },
  methods: {
    cleanupStats(stat) {
      const settings = this.userSettings;

      if (!settings || !stat) {
        console.warn("cleanupStats avbruten: settings eller stat saknas.");
        return;
      }

      const arrayFields = ['penaltyCause']; // Lägg till andra array-fält vid behov

      for (const key in settings) {
        if (Object.prototype.hasOwnProperty.call(settings, key) && settings[key] === false) {
          const statKey = key.replace(/^stat/, "");
          const statField = statKey.charAt(0).toLowerCase() + statKey.slice(1);

          if (Object.prototype.hasOwnProperty.call(stat, statField)) {
            stat[statField] = arrayFields.includes(statField) ? [] : null;
          }
        }
      }
    },
    calculateScore() {
      this.round.bruttoScore = this.round.statistics.reduce((total, stat) => {
        return total + (stat.strokes || 0);
      }, 0);
      this.round.nettoScore = this.round.bruttoScore - this.round.gainedStrokes
    },
    calculateBirdiesAndEagles() {
      this.round.statistics.forEach(stat => {
        const par = stat.hole?.par;
        const strokes = stat.strokes;

        if (typeof par === 'number' && typeof strokes === 'number') {
          stat.birdie = strokes === par - 1;
          stat.eagle = strokes <= par - 2;
        } else {
          stat.birdie = false;
          stat.eagle = false;
        }
      });
    },
    calculateTotalPenaltyStrokes() {
      this.round.statistics.forEach(stat => {
        if (Array.isArray(stat.penaltyCause)) {
          const total = stat.penaltyCause.reduce((sum, cause) => {
            return sum + (Number(cause.penaltyStrokes) || 0);
          }, 0);
          stat.penaltyStrokes = total;
        } else {
          stat.penaltyStrokes = 0;
        }
      });
    },
    updateFiRBasedOnPar() {
      this.round.statistics.forEach(stat => {
        if (stat.hole?.par === 3) {
          stat.fiR = null;
        }
      });
    },
    async onCourseSelected() {
      const selected = this.courses.find(c => c.id === this.selectedCourseId);
      if (selected) {
        this.round.statistics = selected.holes.map(hole => ({
          strokes: 0,
          putts: 0,
          penaltyStrokes: 0,
          fiR: null,
          giR: null,
          birdie: false,
          eagle: false,
          lostBalls: 0,
          upAndDown: null,
          sandSave: null,
          holeId: hole.id,
          roundId: 0,
          penaltyCause: [],
          hole: hole
        }));
        this.availableTees = selected.tees || [];
        this.round.tee = "";
        this.round.holesPlayed = selected.holes.length;
        this.adjustHoleStatistics(this.round.holesPlayed);
      }
    },
    adjustHoleStatistics(newCount) {
      if (!this.selectedCourseId) return;
      const selected = this.courses.find(c => c.id === this.selectedCourseId);
      if (!selected) return;

      const holes = selected.holes.slice(0, newCount);

      const existingStats = this.round.statistics || [];

      this.round.statistics = holes.map(hole => {
        const existing = existingStats.find(stat => stat.holeId === hole.id);

        if (existing) {
          return {
            ...existing,
            hole: hole
          };
        }

        return {
          strokes: 0,
          putts: 0,
          penaltyStrokes: 0,
          fiR: null,
          giR: null,
          birdie: false,
          eagle: false,
          lostBalls: 0,
          upAndDown: null,
          sandSave: null,
          holeId: hole.id,
          roundId: 0,
          penaltyCause: [],
          hole: hole
        };
      });
    },
    async onSubmit() {
      try {
        this.calculateTotalPenaltyStrokes();
        this.calculateBirdiesAndEagles();
        this.updateFiRBasedOnPar();
        this.calculateScore();
        this.round.statistics.forEach(stat => this.cleanupStats(stat));

        let formattedDate;

        if (this.round.date instanceof Date) {
          formattedDate = this.round.date.toISOString().split('T')[0];
        } else {
          const parsedDate = new Date(this.round.date);
          if (!isNaN(parsedDate)) {
            formattedDate = parsedDate.toISOString().split('T')[0];
          } else {
            formattedDate = null;
          }
        }

        this.round.date = formattedDate;

        const cleanRound = {
          id: this.round.id,
          date: new Date(this.round.date).toISOString().split('T')[0],
          tee: this.round.tee,
          userId: this.round.userId,
          nettoScore: this.round.nettoScore,
          statistics: this.round.statistics.map(stat => ({
            ...stat,
            hole: {
              id: stat.hole.id,
              holeNumber: stat.hole.holeNumber,
              par: stat.hole.par,
              courseId: stat.hole.courseId,
              course: { id: stat.hole.courseId }
            },
            round: { id: this.round.id }, 
            penaltyCause: Array.isArray(stat.penaltyCause)
              ? stat.penaltyCause.map(cause => ({ ...cause }))
              : []
          }))
        };

        delete cleanRound.bruttoScore;
        delete cleanRound.gainedStrokes;

        if (this.isEdit) {
          await updateRound(cleanRound);
          alert("Runda uppdaterad!");
        } else {
          await addRound(cleanRound);
          alert("Ny runda tillagd!");
        }
        this.$router.push("/rounds");
      } catch (err) {
        console.log(err)
        alert("Kunde inte spara runda. " + err.message);
      }
    }
  }
};
</script>