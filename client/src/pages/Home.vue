<template>
  <div class="background-image">
    <p>This is homepage</p>
    <h1 v-if="user">Välkommen, {{ user.firstName }}</h1>
    <p v-else>Ingen användardata tillgänglig.</p>
  </div>
</template>


<script>
import { getUserId, getUser } from '../api/MyGolfStatsApi'

export default {
  data() {
    return {
      user: null
    }
  },
  methods: {
    async fetchUser() {
      const userId = await getUserId()
      if (userId) {
        this.user = await getUser(userId)
      }
    }
  },
  mounted() {
    this.fetchUser()
  }
}
</script>