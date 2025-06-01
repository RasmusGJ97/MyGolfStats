<template>
  <div class="background-image">
    <div class="invisible-div golf-dark text-light pb-4 rounded">
      <div class="container mt-4">
        <div class="d-flex justify-content-between align-items-center mb-3">
          <h2 class="text-light m-0">Rundor</h2>
          <button class="btn btn-outline-success" @click="onAddRound">
            <i class="fas fa-plus me-2"></i>Lägg till
          </button>
        </div>

        <div v-if="loading" class="text-center my-5">
          <div class="spinner-border text-light" role="status">
            <span class="visually-hidden">Laddar...</span>
          </div>
        </div>

        <DataGrid
          v-else-if="rounds.length"
          :items="rounds"
          :columns="columns"
          :pageSize="10"
          @row-clicked="onRowClicked"
          @delete-item="onDeleteClicked"
        />

        <div v-else class="text-light">Inga rundor kunde visas.</div>
      </div>
    </div>
  </div>
</template>

<script>
import DataGrid from '../components/DataGrid.vue'
import { useUserStore } from '../stores/userStore'
import { useCourseStore } from '../stores/courseStore'
import { deleteRound } from '../api/MyGolfStatsApi'

export default {
  components: { DataGrid },
  data() {
    return {
      rounds: [],
      columns: [
        { label: 'Golfklubb - Bana', key: 'clubName' },
        { label: 'Datum', key: 'date' },
        { label: 'Tee', key: 'tee' },
        { label: 'Par', key: 'par' },
        { label: 'Netto Score', key: 'nettoScore' },
        { label: 'Brutto Score', key: 'bruttoScore' },
      ],
      loading: true
    };
  },
  async mounted() {
    await this.loadRounds()
  },
  methods: {
    async loadRounds() {
      const userStore = useUserStore();
      const courseStore = useCourseStore();

      try {
        await userStore.fetchUser();
        courseStore.getCourseById();
      } catch (err) {
        console.error("Kunde inte hämta golfrundor:", err);
      } finally {
        this.loading = false;
      }

      this.user = userStore.user;
      this.rounds = (this.user.rounds || []).map(round => {
        const courseId = round.statistics?.[0]?.hole?.courseId;
        const calculatedPar = round.statistics?.reduce((sum, stat) => {
          return sum + (stat.hole?.par || 0);
        }, 0);
        const course = courseStore.courses.find(c => c.id === courseId);
        const clubName = course?.clubName || 'Okänd klubb';

        return {
          id: round.id,
          clubName: clubName,
          date: new Date(round.date).toLocaleDateString('sv-SE'),
          tee: round.tee || 'Ej angiven',
          par: calculatedPar,
          nettoScore: round.nettoScore ?? '-',
          bruttoScore: round.bruttoScore ?? '-',
          rawDate: new Date(round.date)
        };
      })
      .sort((a, b) => b.rawDate - a.rawDate)
      .map(({ rawDate, ...rest }) => rest);
    },
    onAddRound() {
      this.$router.push(`/round/new`);
    },
    onRowClicked(round) {
      const userStore = useUserStore();
      const fullRoundInfo = userStore.getRoundById(round.id)
      userStore.setSelectedRound(fullRoundInfo);
      this.$router.push(`/round/${fullRoundInfo.id}`);
    },
    async onDeleteClicked(round) {
      const confirmed = window.confirm(`Är du säker på att du vill ta bort denna rundan?`);
      
      if (!confirmed) return;
      await deleteRound(round.id)
      await this.loadRounds()
    }
  }
};
</script>