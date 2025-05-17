<template>
  <nav class="navbar navbar-expand-lg navbar-dark golf-dark-nav" style="max-height: 56px;">
    <div class="container-fluid container-navbar">
      <a class="navbar-brand" href="/">MyGolfStats</a>
      
      <!-- Toggler (mobil) -->
      <button
        class="navbar-toggler"
        type="button"
        data-bs-toggle="collapse"
        data-bs-target="#navbarNav"
        aria-controls="navbarNav"
        aria-expanded="false"
        aria-label="Toggle navigation"
      >
        <span class="navbar-toggler-icon"></span>
      </button>

      <div v-if="user" class="d-flex align-items-center text-light fw-semibold me-auto m-3 ms-4">
        {{ user.firstName }} {{ user.lastName }}
      </div>
      <div v-if="user" class="d-flex align-items-center fw-semibold ms-3 small text-secondary">
        HCP: {{ user.hcp }}
      </div>

      <div class="collapse navbar-collapse" id="navbarNav">
        <ul class="navbar-nav ms-auto align-items-center">
          <li class="nav-item">
            <router-link class="nav-link p-3" to="/" exact>Hem</router-link>
          </li>
          <li class="nav-item">
            <router-link class="nav-link p-3" to="/">Statistik</router-link>
          </li>
          <li class="nav-item">
            <router-link class="nav-link p-3" to="/rounds">Rundor</router-link>
          </li>
          <li class="nav-item">
            <router-link class="nav-link p-3" to="/courses">Banor</router-link>
          </li>
          <li class="nav-item">
            <router-link class="nav-link p-3" to="/">Min Profil</router-link>
          </li>
          <li class="nav-item mx-2">
            <div style="width: 1px; height: 24px; background-color: rgba(255,255,255,0.3);"></div>
          </li>
          <li class="nav-item">
            <a class="nav-link text-danger p-3" @click.prevent="handleLogout" style="cursor: pointer;">Logga ut</a>
          </li>
        </ul>
      </div>
    </div>
  </nav>
</template>

<script>
import { logout } from '../api/MyGolfStatsApi'
import { useUserStore } from '../stores/userStore'

export default {
  methods: {
    handleLogout() {
      logout()
      const userStore = useUserStore()
      userStore.user = null
      this.$router.push('/login')
    }
  },
  computed: {
    user() {
      const user = useUserStore().user
      console.log(user)
      return user
    }
  }
}

</script>