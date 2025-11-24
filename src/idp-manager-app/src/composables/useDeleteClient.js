import { useDeleteModalStore } from '@/stores/deleteModal'
import { useNotification } from './useNotification'
import { useClientStore } from '@/stores/clients'

export function useDeleteClient(clientInput) {
  const confirmationStore = useDeleteModalStore()
  const { notifyError, notifySuccess } = useNotification()
  const client = clientInput
  const store = useClientStore()
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

  return { deleteClient }
}
