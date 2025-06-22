<template>
  <nav class="navbar navbar-expand-lg navbar-dark golf-dark-nav container-navbar">
    <div class="container-fluid container-navbar">
      <!-- Logotyp -->
      <a class="navbar-brand d-flex align-items-center nav-items-center" href="/">
        <img
          src="../assets/img/mygolfstats2rows-logo-bigger.png"
          alt="MyGolfStats logo"
          style="height: 40px; filter: invert(1) brightness(200%) contrast(200%);"
        />
      </a>

      <!-- Användare (desktop) -->
      <div class="d-none d-sm-flex flex-column justify-content-center text-light fw-semibold me-auto ms-3">
        <div v-if="user">{{ user.firstName }} {{ user.lastName }}</div>
        <div v-if="user" class="small text-secondary">HCP: {{ Number.isInteger(user.hcp) ? user.hcp.toFixed(1) : user.hcp }}</div>
      </div>

      <!-- Toggler (mobil) -->
      <button
        class="navbar-toggler nav-items-center"
        type="button"
        @click="toggleMenu"
        aria-controls="navbarNav"
        :aria-expanded="isMenuOpen.toString()"
        aria-label="Toggle navigation"
      >
        <span class="navbar-toggler-icon"></span>
      </button>

      <!-- Navigation -->
      <div class="collapse navbar-collapse" id="navbarNav" :class="{ show: isMenuOpen }">
        <ul class="navbar-nav ms-auto align-items-lg-center w-100 justify-content-end">
          <li class="nav-item">
            <router-link class="nav-link px-3" to="/" exact>Hem</router-link>
          </li>
          <li class="nav-item">
            <router-link class="nav-link px-3" to="/statistics">Statistik</router-link>
          </li>
          <li class="nav-item">
            <router-link class="nav-link px-3" to="/rounds">Rundor</router-link>
          </li>
          <li class="nav-item">
            <router-link class="nav-link px-3" to="/courses">Banor</router-link>
          </li>
          <li class="nav-item">
            <router-link class="nav-link px-3" to="/settings">Min Profil</router-link>
          </li>

          <li class="nav-item d-none d-lg-block mx-2">
            <div style="width: 1px; height: 24px; background-color: rgba(255,255,255,0.3);"></div>
          </li>

          <li class="nav-item">
            <a class="nav-link text-danger px-3" @click.prevent="handleLogout" style="cursor: pointer;">Logga ut</a>
          </li>

          <!-- Användare (mobil) -->
          <li v-if="user" class="nav-item d-sm-none text-center mt-2 mb-2 text-light fw-semibold">
            {{ user.firstName }} {{ user.lastName }}<br />
            <span class="small text-secondary">HCP: {{ user.hcp }}</span>
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
  name: 'Navbar',
  data() {
    return {
      isMenuOpen: false
    }
  },
  computed: {
    user() {
      return useUserStore().user
    }
  },
  methods: {
    toggleMenu() {
      this.isMenuOpen = !this.isMenuOpen
    },
    handleLogout() {
      logout()
      const userStore = useUserStore()
      userStore.user = null
      this.$router.push('/login')
    },
    handleClickOutside(event) {
      const menu = document.getElementById('navbarNav')
      const toggle = document.querySelector('.navbar-toggler')
      if (
        this.isMenuOpen &&
        menu &&
        !menu.contains(event.target) &&
        !toggle.contains(event.target)
      ) {
        this.isMenuOpen = false
      }
    }
  },
  mounted() {
    document.addEventListener('click', this.handleClickOutside)

    this.$router.afterEach(() => {
      this.isMenuOpen = false
    })
  },
  beforeUnmount() {
    document.removeEventListener('click', this.handleClickOutside)
  }
}
</script>
