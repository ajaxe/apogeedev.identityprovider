<template>
  <a href="javascript:void(0)" class="text-blue-600 hover:text-blue-900 ms-3" @click="deleteClient"
    >Delete</a
  >
</template>
<script setup>
import { useClientStore } from '@/stores/clients'
import { useNotification } from '@/composables/useNotification'
import { useDeleteModalStore } from '@/stores/deleteModal'

const { /* type {import('@/types').ClientListItem} */ client } = defineProps({
  client: {
    type: Object,
    required: true,
  },
})
const { notifyError, notifySuccess } = useNotification()

const store = useClientStore()
const confirmationStore = useDeleteModalStore()
const deleteClient = async () => {
  const isConfirmed = await confirmationStore.confirm({
    displayName: client.displayName,
  })

  if (!isConfirmed) return

  try {
    await store.deleteClient(client.clientId)

    notifySuccess(`Client ${client.displayName} deleted successfully.`)
  } catch (err) {
    console.error(err)
    notifyError('Failed to delete client.')
  }
}
</script>
