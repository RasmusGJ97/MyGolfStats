<template>
  <div class="login-container d-flex justify-content-center align-items-center min-vh-100">
    <div class="card shadow-lg p-4 bg-dark text-light" style="width: 100%; max-width: 400px;">
      <h2 class="text-center mb-3">Logga in</h2>
      <form @submit.prevent="handleLogin">
        <div class="form-group mb-3">
          <label for="golfId" class="form-label">Golf-ID:</label>
          <input id="golfId" v-model="golfId" type="text" class="form-control bg-secondary text-light" required />
        </div>

        <div class="form-group mb-4">
          <label for="password" class="form-label">Lösenord:</label>
          <input id="password" v-model="password" type="password" class="form-control bg-secondary text-light" required />
        </div>

        <button type="submit" class="btn btn-primary w-100" :disabled="isLoading">
          <i v-if="isLoading" class="fas fa-circle-notch fa-spin me-2"></i>
          <span>{{ isLoading ? '' : 'Logga in' }}</span>
        </button>

        <p v-if="error" class="text-danger mt-3 text-center">{{ error }}</p>
      </form>

      <div class="text-center mt-3">
        <router-link to="/forgot-password" class="text-decoration-underline text-light">
          Glömt lösenord?
        </router-link>
        <div class="d-block mt-2">
          <span 
            @click="demoLogin"
            class="text-light text-decoration-underline"
            style="cursor: pointer;">
            Gå till demo
          </span>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { login } from '../api/MyGolfStatsApi.js'
import { useUserStore } from '../stores/userStore'
import { useCourseStore } from '../stores/courseStore'

const golfId = ref('')
const password = ref('')
const error = ref('')
const router = useRouter()
const userStore = useUserStore()
const courseStore = useCourseStore()
const isLoading = ref(false)

const handleLogin = async () => {
  isLoading.value = true
  error.value = ''
  try {
    const user = await login({ golfId: golfId.value, password: password.value })
    await userStore.fetchUser()
    await courseStore.fetchCourses()
    router.push('/')
  } catch (err) {
    error.value = 'Fel användarnamn eller lösenord.'
    console.error(err)
  } finally {
    isLoading.value = false
  }
}

const demoLogin = async () => {
  isLoading.value = true
  error.value = ''
  try {
    const user = await login({ golfId: 'Demo-001', password: 'demo1234' })
    await userStore.fetchUser()
    await courseStore.fetchCourses()
    router.push('/')
  } catch (err) {
    error.value = 'Fel användarnamn eller lösenord.'
    console.error(err)
  } finally {
    isLoading.value = false
  }
}
</script>
