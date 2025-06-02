<template>
  <div class="login-container d-flex justify-content-center align-items-center min-vh-100">
    <div class="card shadow-lg p-4 bg-dark text-light" style="width: 100%; max-width: 400px;">
      <h2 class="text-center mb-3">Återställ lösenord</h2>
      <form @submit.prevent="resetPassword">
        <div class="mb-3">
          <label for="newPassword" class="form-label">Nytt lösenord:</label>
          <input
            id="newPassword"
            type="password"
            v-model="newPassword"
            class="form-control bg-secondary text-light"
            required
          />
        </div>
        <div class="mb-3">
          <label for="confirmPassword" class="form-label">Bekräfta lösenord:</label>
          <input
            id="confirmPassword"
            type="password"
            v-model="confirmPassword"
            class="form-control bg-secondary text-light"
            required
          />
        </div>
        <div v-if="errorMessage" class="text-danger mb-3">{{ errorMessage }}</div>
        <div v-if="successMessage" class="text-success mb-3">{{ successMessage }}</div>
        <button class="btn btn-primary w-100">Återställ lösenord</button>
      </form>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useRoute } from 'vue-router'
import { resetPassword as resetPasswordApi } from '../api/MyGolfStatsApi'

const route = useRoute()

const newPassword = ref('')
const confirmPassword = ref('')
const errorMessage = ref('')
const successMessage = ref('')
const token = ref('')

onMounted(() => {
  token.value = route.query.token
})

const resetPassword = async () => {
  if (newPassword.value !== confirmPassword.value) {
    errorMessage.value = 'Lösenorden matchar inte.'
    successMessage.value = ''
    return
  }

  try {
    await resetPasswordApi(token.value, newPassword.value)
    successMessage.value = 'Lösenordet är nu återställt.'
    errorMessage.value = ''
  } catch (err) {
    errorMessage.value = err.response?.data?.message || 'Något gick fel.'
    successMessage.value = ''
  }
}
</script>

