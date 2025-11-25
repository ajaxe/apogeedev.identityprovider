<script setup>
import { useRouter } from 'vue-router'
import ClientList from '@/components/clients/ClientList.vue'
import ClientListSummary from '@/components/clients/ClientListSummary.vue'
import ClientListFilter from '@/components/clients/ClientListFilter.vue'
import { useClientStore } from '@/stores/clients.js'
import { storeToRefs } from 'pinia'

const router = useRouter()
const newClient = () => router.push({ name: 'new-client' })
const store = useClientStore()
const { stats } = storeToRefs(store)
</script>

<template>
  <div class="d-flex justify-content-between mb-3">
    <h1 class="text-3xl font-bold text-gray-800">OIDC Clients</h1>
    <button class="btn btn-primary d-none d-md-block" style="line-height: 38px" @click="newClient">
      + Create New Client
    </button>
  </div>

  <ClientListSummary :stats="stats" />

  <ClientListFilter />

  <div>
    <ClientList />
  </div>
  <button
    class="btn btn-primary rounded-circle shadow-lg d-md-none fab-btn"
    @click="newClient"
    aria-label="Create New Client"
  >
    <i class="bi bi-plus-lg fs-2"></i>
  </button>
</template>
<style scoped>
/* FAB Positioning */
.fab-btn {
  position: fixed; /* Sticks to the screen even when scrolling */
  bottom: 24px; /* Distance from bottom */
  right: 24px; /* Distance from right */
  width: 64px; /* Large touch target */
  height: 64px;
  z-index: 1050; /* Ensures it sits on top of all other content */

  /* Centering the icon */
  display: flex;
  align-items: center;
  justify-content: center;
}

/* Optional: Add a subtle hover lift effect */
.fab-btn:active {
  transform: scale(0.95);
}
</style>
