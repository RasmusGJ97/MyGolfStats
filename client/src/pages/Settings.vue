<template>
  <div class="background-image">
    <div class="invisible-div golf-dark text-light pb-4 rounded">
      <div class="container py-4">
        <div class="mb-5 d-flex justify-content-between align-items-center">
          <h2 class="text-light m-0">Inställningar</h2>
          <button class="btn btn-outline-light" @click="toggleEditMode">
            {{ editMode ? 'Avbryt' : 'Ändra' }}
          </button>
        </div>

        <div v-if="!loading && user">
          <!-- Profil -->
          <section>
            <hr class="border-light mb-4 mt-4" />
            <h5 class="text-uppercase">Profil</h5>
            <div class="ps-3">
              <div v-if="!editMode">
                <p><strong>Namn:</strong> {{ user.firstName }} {{ user.lastName }}</p>
                <p><strong>Email:</strong> {{ user.email }}</p>
                <p><strong>Golf-ID:</strong> {{ user.golfId }}</p>
                <p><strong>HCP:</strong> {{ user.hcp }}</p>
              </div>
              <div v-else>
                <p><strong>Namn:</strong> {{ user.firstName }} {{ user.lastName }}</p>
                <p><strong>Golf-ID:</strong> {{ user.golfId }}</p>
                <p><strong>HCP:</strong> <input type="number" v-model="editableUser.hcp" class="form-control" /></p>
              </div>
              <div>
                <button class="btn btn-outline-success w-20" @click="passChangeOn" v-if="!passChange">Byt lösenord</button>
              </div>
              <div class="alert alert-success mt-3" v-if="success">Lösenordet har uppdaterats!</div>
              <div class="alert alert-danger mt-3" v-if="error">{{ error }}</div>
              <div v-if="passChange">
                <div style="max-width: 400px;">
                  <form @submit.prevent="changePassword">
                    <div class="mb-3">
                      <label for="currentPassword" class="form-label">Nuvarande lösenord</label>
                      <input type="password" v-model="currentPassword" class="form-control" required />
                    </div>
                    <div class="mb-3">
                      <label for="newPassword" class="form-label">Nytt lösenord</label>
                      <input type="password" v-model="newPassword" class="form-control" required />
                    </div>
                    <div class="mb-3">
                      <label for="confirmPassword" class="form-label">Bekräfta nytt lösenord</label>
                      <input type="password" v-model="confirmPassword" class="form-control" required />
                    </div>
                    <div class="d-flex justify-content-end mt-4">
                      <button type="button" class="btn btn-outline-secondary w-50 me-2" @click="passChangeOn">Avbryt</button>
                      <button type="submit" class="btn btn-outline-success w-50">Byt lösenord</button>
                    </div>
                  </form>
                </div>
              </div>
            </div>
          </section>

          <!-- Statistik -->
          <section>
            <hr class="border-light mb-4 mt-4" />
            <h5 class="text-uppercase">Statistik</h5>
            <div class="ps-3">
              <ul class="list-unstyled">
                <li v-for="(value, key) in editableUser.settings" :key="key">
                  <strong v-if="!editMode">{{ formatStatLabel(key) }}: </strong>
                  <template v-if="!editMode">{{ value ? 'Ja' : 'Nej' }}</template>
                  <template v-else>
                    <input type="checkbox" v-model="editableUser.settings[key]" class="form-check-input ms-2 mx-2" />
                    <strong>{{ formatStatLabel(key) }}: </strong>
                  </template>
                </li>
              </ul>
            </div>
          </section>

          <!-- Bag -->
          <section>
            <hr class="border-light mb-4 mt-4" />
            <h5 class="text-uppercase">Bag</h5>
            <div class="ps-3">
              <ul class="list-inline">
                <li
                  v-for="(hasClub, club) in editableUser.bag"
                  :key="club"
                  class="list-inline-item m-1"
                  v-show="hasClub || editMode"
                >
                  <span v-if="!editMode" class="badge bg-light text-dark fs-8 px-3 py-2">
                    {{ formatClubName(club) }}
                  </span>
                  <label v-else class="form-check-label text-light">
                    <input
                      type="checkbox"
                      v-model="editableUser.bag[club]"
                      class="form-check-input me-1"
                    />
                    {{ formatClubName(club) }}
                  </label>
                </li>
              </ul>
            </div>
          </section>


          <!-- Spara-knapp -->
          <div v-if="editMode" class="d-flex justify-content-end mt-4">
            <button class="btn btn-outline-secondary me-2" @click="cancelChanges">Avbryt</button>
            <button class="btn btn-outline-primary" @click="saveChanges">Spara</button>
          </div>
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
import { updatePassword } from '../api/MyGolfStatsApi'

export default {
  data() {
    return {
      loading: true,
      editMode: false,
      editableUser: null,
      passChange: false,
      currentPassword: '',
      newPassword: '',
      confirmPassword: '',
      error: '',
      success: false
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
        'pWedge', 'aWedge', 'gWedge', 'sWedge', 'lWedge'
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
      this.initEditableUser()
    } catch (err) {
      console.error("Kunde inte hämta användare:", err)
    } finally {
      this.loading = false
    }
  }, 
  methods: {
    passChangeOn(){
      if(this.passChange){
        this.passChange = false
      }
      else{
        this.passChange = true
      }
    },
    async changePassword() {
      this.error = '';
      this.success = false;

      if (!this.currentPassword || !this.newPassword || !this.confirmPassword) {
        this.error = 'Alla fält måste fyllas i.';
        return;
      }

      const passwordRegex = /^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d@$!%*?&]{8,}$/;
      if (!passwordRegex.test(this.newPassword)) {
        this.error = 'Lösenordet måste vara minst 8 tecken långt och innehålla både bokstäver och siffror.';
        return;
      }

      if (this.newPassword !== this.confirmPassword) {
        this.error = 'Nya lösenorden matchar inte.';
        return;
      }

      try {
        await updatePassword(this.currentPassword, this.newPassword, this.user.id);
        this.success = true;
        this.passChange = false;
        this.currentPassword = '';
        this.newPassword = '';
        this.confirmPassword = '';
      } catch (error) {
        this.error = error.message || 'Ett fel uppstod.';
      }
    },
    toggleEditMode() {
      this.editMode = !this.editMode
      if (this.editMode) {
        this.initEditableUser()
      }
    },
    initEditableUser() {
      const clone = JSON.parse(JSON.stringify(this.user))
      delete clone.bag.id
      delete clone.bag.userId
      delete clone.settings.id
      delete clone.settings.userId
      this.editableUser = clone
    },
    saveChanges() {
      const userStore = useUserStore()
      userStore.updateUser({ id: this.user.id, ...this.editableUser })
      this.editMode = false
    },
    cancelChanges() {
      this.editMode = false
      this.initEditableUser()
    },
    formatClubName(clubKey) {
      const map = {
        driver: "Driver",
        twoW: "2 Wood", threeW: "3 Wood", fourW: "4 Wood", fiveW: "5 Wood", sixW: "6 Wood",  sevenW: "7 Wood",
        twoHy: "2 Hybrid", threeHy: "3 Hybrid", fourHy: "4 Hybrid",  fiveHy: "5 Hybrid", 
        oneI: "1 Iron", twoI: "2 Iron", threeI: "3 Iron", fourI: "4 Iron",  fiveI: "5 Iron", sixI: "6 Iron", sevenI: "7 Iron", eightI: "8 Iron", nineI: "9 Iron",
        pWedge: "Pitching Wedge", aWedge: "Approach Wedge", gWedge: "Gap Wedge", sWedge: "Sand Wedge", lWedge: "Lob Wedge"
      };
      return map[clubKey] || clubKey;
    },
    formatStatLabel(key) {
      const map = {
        statBirdie: "Birdie",
        statEagle: "Eagle",
        statFiR: "Fairway träff (FiR)",
        statGiR: "Green in regulation (GiR)",
        statPutts: "Puttar",
        statSandSave: "Bunkerräddning (Sand Save)",
        statUpAndDown: "Up & Down",
        statPenaltyStrokes: "Pliktslag",
        statPenaltyCause: "Orsak till pliktslag",
        statLostBalls: "Bortslagna bollar"
      }
      return map[key] || key
    }
  }
}
</script>