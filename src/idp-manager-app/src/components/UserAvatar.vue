<template>
  <div class="py-2">
    <span :title="profile?.name" class="me-2 fw-bold lead">{{ profile?.username }}</span>
    <button v-if="isLoggedIn" class="btn btn-primary btn-large" @click="logout">Log Out</button>
    <button v-else class="btn btn-primary btn-large" @click="login">Log In</button>
  </div>
</template>

<script setup>
import { useAuth } from '../composables/useAuth'

// Get the reactive user object and methods
const { isLoggedIn, login, logout, profile } = useAuth()
</script>
