<template>
  <div>
    User: {{ user.name }} ({{ user['private:idp'] }}) is not authorized to manage applications
    clients.
  </div>
</template>
<script setup>
import { useAuthStore } from '@/stores/auth'

const store = useAuthStore()
const user = store.user?.profile
console.log({ profile: user })
</script>
