<template>
  <app-navbar v-if="isLoggedIn" class="sticky-top" />
  <router-view />
</template>

<script>
import AppNavbar from "./components/AppNavbar.vue"
import { onMounted } from 'vue'
import { useUserStore } from './stores/userStore'
import { useCourseStore } from './stores/courseStore'

export default {
  name: 'App',
  components: {
    AppNavbar
  },
  setup() {
    const userStore = useUserStore()
    const courseStore = useCourseStore();
    onMounted(() => {
      userStore.fetchUser()
      try {
        courseStore.fetchCourses();
      } catch (err) {
        console.error("Kunde inte hämta golfbanor:", err);
      }
    })
  },
  data() {
    return {
      isLoggedIn: false
    }
  },
  mounted() {
    this.checkLoginStatus();

    window.addEventListener('token-changed', this.checkLoginStatus);
  },
  methods: {
    checkLoginStatus() {
      this.isLoggedIn = !!localStorage.getItem('token');
    }
  },
  beforeUnmount() {
    window.removeEventListener('token-changed', this.checkLoginStatus);
  }
}
</script>
