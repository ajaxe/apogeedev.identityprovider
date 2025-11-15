<template>
  <div v-if="!user">
    <p>You are not logged in.</p>
    <button class="btn btn-primary btn-large" @click="login">Log In</button>
  </div>
</template>

<script setup>
import { useAuth } from '../composables/useAuth'

// Get the reactive user object and methods
const { user, login } = useAuth()
</script>
