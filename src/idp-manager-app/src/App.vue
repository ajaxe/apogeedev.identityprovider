<script setup>
import { onMounted } from 'vue'
import { RouterView } from 'vue-router'
import AppBrand from '@/components/AppBrand.vue'
import { useAuth } from '@/composables/useAuth'
import UserAvatar from './components/UserAvatar.vue'

const { loadUser } = useAuth()
onMounted(() => {
  loadUser()
})
</script>

<template>
  <header class="p-3 text-bg-dark shadow-lg">
    <div class="container">
      <div class="d-flex flex-wrap justify-content-between">
        <a href="/">
          <AppBrand />
        </a>
        <UserAvatar />
      </div>
    </div>
  </header>
  <div class="container mt-5">
    <RouterView />
  </div>
</template>
