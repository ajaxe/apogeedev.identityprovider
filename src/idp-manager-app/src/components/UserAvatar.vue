<template>
  <div class="py-2">
    <span :title="profile?.name" class="me-2 fw-bold">{{ profile?.username }}</span>
    <button class="btn btn-primary btn-large" @click="logout">
      <span class="d-none d-md-inline me-2">Log Out</span>
      <i class="bi bi-box-arrow-right"></i>
    </button>
  </div>
</template>

<script setup>
import { useAuth } from '../composables/useAuth'

// Get the reactive user object and methods
const { logout, profile } = useAuth()
</script>
