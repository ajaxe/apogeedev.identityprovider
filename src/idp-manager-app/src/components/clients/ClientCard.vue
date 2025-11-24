<script setup>
import { useDeleteClient } from '@/composables/useDeleteClient'
import { computed } from 'vue'
import { useRouter } from 'vue-router'

const router = useRouter()

const { /** @type {import('@/types').ClientListItem} */ client } = defineProps({
  client: {
    type: Object,
    required: true,
  },
})

// Optional: Dynamic badge colors based on type
const appTypeClass = computed(() => {
  return client.applicationType === 'web' ? 'bg-primary' : 'bg-success' // Blue for Web, Green for Native
})

const clientTypeClass = computed(() => {
  return client.clientType === 'confidential' ? 'bg-dark' : 'bg-info text-dark'
})

const navigateEdit = () => {
  router.push({ name: 'edit-client', params: { clientId: client.clientId } })
}

const { deleteClient } = useDeleteClient(client)
</script>

<template>
  <div class="card border-0 shadow-sm mb-3 rounded-3">
    <div class="card-body p-3">
      <div class="d-flex justify-content-between align-items-start mb-1">
        <div>
          <h5 class="card-title fw-bold mb-0 text-truncate" style="max-width: 200px">
            {{ client.displayName }}
          </h5>
          <small class="text-muted font-monospace">{{ client.clientId }}</small>
        </div>

        <div class="dropdown">
          <button
            class="btn btn-link text-secondary p-0 text-decoration-none"
            type="button"
            data-bs-toggle="dropdown"
            aria-expanded="false"
          >
            <i class="bi bi-three-dots-vertical fs-4"></i>
          </button>
          <ul class="dropdown-menu dropdown-menu-end">
            <li>
              <a class="dropdown-item" href="javascript:void(0)" @click="navigateEdit">Edit</a>
            </li>
            <li><a class="dropdown-item" href="#">Regenerate Secret</a></li>
            <li><hr class="dropdown-divider" /></li>
            <li>
              <a class="dropdown-item text-danger" href="javascript:void(0)" @click="deleteClient"
                >Delete</a
              >
            </li>
          </ul>
        </div>
      </div>

      <div class="mt-3">
        <span class="badge rounded-pill me-2" :class="appTypeClass">
          {{ client.appType }}
        </span>
        <span class="badge rounded-pill" :class="clientTypeClass">
          {{ client.clientType }}
        </span>
      </div>
    </div>
  </div>
</template>

<style lang="scss" scoped>
/* If page background is #121212 (very dark gray) */
.card {
  background-color: #1e1e1e; /* Slightly lighter */
  border: 1px solid #333; /* Subtle border */
}
.card-title {
  display: -webkit-box;
  -webkit-line-clamp: 2; /* Allow 2 lines */
  -webkit-box-orient: vertical;
  white-space: normal;
}
</style>
