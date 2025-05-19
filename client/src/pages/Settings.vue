<template>
  <div class="background-image">
    <div class="invisible-div golf-dark text-light pb-4 rounded">
      <div class="container py-4">
        <div class="mb-5">
          <h2 class="text-light m-0">Inställningar</h2>
        </div>

        <div v-if="!loading && user">
          <!-- Profil -->
          <section>
            <hr class="border-light mb-4 mt-4" />
            <h5 class="text-uppercase">Profil</h5>
            <div class="ps-3">
              <p><strong>Namn:</strong> {{ user.firstName }} {{ user.lastName }}</p>
              <p><strong>Golf-ID:</strong> {{ user.golfId }}</p>
              <p><strong>HCP:</strong> {{ user.hcp }}</p>
            </div>
          </section>

          <!-- Statistik -->
          <section>
            <hr class="border-light mb-4 mt-4" />
            <h5 class="text-uppercase">Statistik</h5>
            <div class="ps-3">
              <ul class="list-unstyled">
                <li><strong>Birdie:</strong> {{ user.settings?.statBirdie ? 'Ja' : 'Nej' }}</li>
                <li><strong>Eagle:</strong> {{ user.settings?.statEagle ? 'Ja' : 'Nej' }}</li>
                <li><strong>Fairway träff (FiR):</strong> {{ user.settings?.statFiR ? 'Ja' : 'Nej' }}</li>
                <li><strong>Green i regulation (GiR):</strong> {{ user.settings?.statGiR ? 'Ja' : 'Nej' }}</li>
                <li><strong>Putts:</strong> {{ user.settings?.statPutts ? 'Ja' : 'Nej' }}</li>
                <li><strong>Bunkerräddning (Sand Save):</strong> {{ user.settings?.statSandSave ? 'Ja' : 'Nej' }}</li>
                <li><strong>Up & Down:</strong> {{ user.settings?.statUpAndDown ? 'Ja' : 'Nej' }}</li>
                <li><strong>Straffslag:</strong> {{ user.settings?.statPenaltyStrokes ? 'Ja' : 'Nej' }}</li>
                <li><strong>Orsak till straffslag:</strong> {{ user.settings?.statPenaltyCause ? 'Ja' : 'Nej' }}</li>
                <li><strong>Bollar bortslagna:</strong> {{ user.settings?.statLostBalls ? 'Ja' : 'Nej' }}</li>
              </ul>
            </div>
          </section>

          <!-- Bag -->
          <section>
            <hr class="border-light mb-4 mt-4" />
            <h5 class="text-uppercase">Bag</h5>
            <div class="ps-3">
              <ul class="list-inline">
                <li v-for="(hasClub, club) in bagClubs" :key="club" class="list-inline-item badge bg-light text-dark m-1">
                  {{ formatClubName(club) }}
                </li>
              </ul>
            </div>
          </section>
        </div>

        <div v-if="loading" class="text-center my-5">
          <div class="spinner-border text-light" role="status">
            <span class="visually-hidden">Laddar...</span>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>



<script>
import { useUserStore } from '../stores/userStore'

export default {
  data() {
    return {
      loading: true
    }
  },
  computed: {
    user() {
      return useUserStore().user
    },
    bagClubs() {
      if (!this.user || !this.user.bag) return {}

      const clubKeys = [
        'driver',
        'twoW', 'threeW', 'fourW', 'fiveW', 'sixW', 'sevenW',
        'twoHy', 'threeHy', 'fourHy', 'fiveHy',
        'oneI', 'twoI', 'threeI', 'fourI', 'fiveI', 'sixI', 'sevenI', 'eightI', 'nineI',
        'aWedge', 'gWedge', 'sWedge', 'lWedge'
      ]

      const result = {}
      for (const key of clubKeys) {
        if (this.user.bag[key]) {
          result[key] = true
        }
      }
      return result
    }

  },
  async mounted() {
    const userStore = useUserStore()
    try {
      await userStore.fetchUser()
    } catch (err) {
      console.error("Kunde inte hämta användare:", err)
    } finally {
      this.loading = false
    }
  }, methods: {
    formatClubName(clubKey) {
      const map = {
        driver: "Driver",
        twoW: "2 Wood", threeW: "3 Wood", fourW: "4 Wood", fiveW: "5 Wood", sixW: "6 Wood",  sevenW: "7 Wood",
        twoHy: "2 Hybrid", threeHy: "3 Hybrid", fourHy: "4 Hybrid",  fiveHy: "5 Hybrid", 
        oneI: "1 Iron", twoI: "2 Iron", threeI: "3 Iron", fourI: "4 Iron",  fiveI: "5 Iron", sixI: "6 Iron", sevenI: "7 Iron", eightI: "8 Iron", nineI: "9 Iron",
        aWedge: "Approach Wedge", gWedge: "Gap Wedge", sWedge: "Sand Wedge", lWedge: "Lob Wedge"
      };
      return map[clubKey] || clubKey;
    }
  }
}
</script>