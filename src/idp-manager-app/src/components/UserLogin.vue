<template>
  <div v-if="!isLoggedIn">
    <button class="btn btn-primary btn-large" @click="login">Log In</button>
  </div>
</template>

<script setup>
import { useAuth } from '../composables/useAuth'

// Get the reactive user object and methods
const { isLoggedIn, login } = useAuth()
</script>
