<template>
  <div class="login-container d-flex justify-content-center align-items-center min-vh-100">
    <div class="card shadow-lg p-4 bg-dark text-light" style="width: 100%; max-width: 400px;">
      <h3>Glömt lösenord</h3>
      <form @submit.prevent="submitForgotPassword">
        <div class="mb-3">
          <label for="email" class="form-label">E-postadress:</label>
          <input v-model="email" type="email" class="form-control bg-secondary text-light" required />
        </div>
        <button type="submit" class="btn btn-primary">Skicka återställningslänk</button>
        <p class="mt-2 text-success" v-if="success">{{ success }}</p>
        <p class="mt-2 text-danger" v-if="error">{{ error }}</p>
      </form>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import { forgotPassword } from '../api/MyGolfStatsApi'

const email = ref('')
const success = ref('')
const error = ref('')

const submitForgotPassword = async () => {
  try {
    await forgotPassword(email.value)
    success.value = 'Om e-postadressen är registrerad har ett återställningsmail skickats. Glöm inte kolla skräpposten.'
    error.value = ''
  } catch (err) {
    error.value = err.response?.data?.message || 'Något gick fel.'
    success.value = ''
  }
}
</script>
