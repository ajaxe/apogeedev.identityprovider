<template>
  <a href="javascript:void(0)" class="text-blue-600 hover:text-blue-900 ms-3" @click="deleteClient"
    >Delete</a
  >
</template>
<script setup>
import { useDeleteClient } from '@/composables/useDeleteClient'

const { /* type {import('@/types').ClientListItem} */ client } = defineProps({
  client: {
    type: Object,
    required: true,
  },
})
const { deleteClient } = useDeleteClient(client)
</script>
