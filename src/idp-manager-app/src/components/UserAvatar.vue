<template>
  <div class="py-2">
    <button v-if="user" class="btn btn-primary btn-large" @click="logout">Log Out</button>
    <button v-else class="btn btn-primary btn-large" @click="login">Log In</button>
  </div>
</template>

<script setup>
import { useAuth } from '../composables/useAuth'

// Get the reactive user object and methods
const { user, login, logout } = useAuth()
</script>
