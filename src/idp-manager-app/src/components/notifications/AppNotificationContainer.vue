<template>
  <div
    id="toastContainer"
    class="toast-container position-fixed bottom-0 end-0 p-3"
    style="z-index: 1100"
  >
    <template v-for="v in notifications" :key="v.id">
      <AppNotification :notification="v" @hidden="handleHidden" />
    </template>
  </div>
</template>
<script setup>
import { useNotifications } from '@/stores/notifications'
import AppNotification from './AppNotification.vue'
import { storeToRefs } from 'pinia'

const store = useNotifications()
const { notifications } = storeToRefs(store)
const handleHidden = (id) => store.remove(id)
</script>
