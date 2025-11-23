import { useNotificationStore } from '@/stores/notifications'
export function useNotification() {
  const store = useNotificationStore()

  const notifySuccess = store.pushSuccess
  const notifyError = store.pushError

  return {
    notifySuccess,
    notifyError,
  }
}
