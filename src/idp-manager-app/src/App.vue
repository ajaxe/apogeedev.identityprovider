<script setup>
import { onMounted, ref } from 'vue'
import { RouterView } from 'vue-router'
import AppBrand from '@/components/AppBrand.vue'
import { useAuth } from '@/composables/useAuth'
import UserAvatar from './components/UserAvatar.vue'
import { useRouter } from 'vue-router'

const router = useRouter()
const { loadUser } = useAuth()
const isRouterReady = ref(false)

onMounted(() => {
  void loadUser()
  void router.isReady().then(() => {
    isRouterReady.value = true
  })
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
  <div class="container mt-5" v-if="isRouterReady">
    <RouterView />
  </div>
</template>
