import { defineStore } from 'pinia'
import { getUserId, getUser, updateUser } from '../api/MyGolfStatsApi'

export const useUserStore = defineStore('user', {
  state: () => ({
    user: null,
    setSelectedRound: null,
  }),
  actions: {
    async fetchUser() {
      const userId = await getUserId()
      if (userId) {
        const userData = await getUser(userId)
        this.user = userData
      }
    },

    setSelectedRound(round) {
      this.selectedRound = round;
    },

    getRoundById(id) {
      const round = this.user.rounds.find(c => c.id === parseInt(id));
      return round
    },

    async updateUser(userToUpdate) {
      try {
        const updatedUser = await updateUser(userToUpdate)
        this.user = updatedUser
      } catch (error) {
        console.error("Misslyckades med att uppdatera användare:", error)
      }
    }
  }
})
