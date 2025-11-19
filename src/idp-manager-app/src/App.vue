<script setup>
import { onMounted, ref } from 'vue'
import { RouterView } from 'vue-router'
import { useAuth } from '@/composables/useAuth'
import { useRouter } from 'vue-router'
import DefaultLayout from './layouts/DefaultLayout.vue'

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
  <component :is="$route.meta.layout || DefaultLayout" v-if="isRouterReady">
    <RouterView />
  </component>
</template>
