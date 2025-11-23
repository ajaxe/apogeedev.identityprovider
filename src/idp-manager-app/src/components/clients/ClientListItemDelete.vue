<template>
  <a href="javascript:void(0)" class="text-blue-600 hover:text-blue-900 ms-3" @click="deleteClient"
    >Delete</a
  >
</template>
<script setup>
import { useClientStore } from '@/stores/clients'

const { clientId } = defineProps({
  clientId: {
    type: String,
    required: true,
  },
})

const store = useClientStore()
const deleteClient = async () => {
  await store.deleteClient(clientId)
}
</script>
