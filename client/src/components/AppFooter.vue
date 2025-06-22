<template>
  <footer class="footer golf-dark text-white text-center py-2 small fixed-bottom">
    <div class="container">
      <p class="mb-1">© {{ currentYear }} MyGolfStats | Version 0.1.3 | Utvecklad av: Rasmus GJ</p>
    </div>
  </footer>
</template>

<script setup>
const currentYear = new Date().getFullYear();
</script>

<style scoped>
.footer {
  z-index: 1000;
}
</style>