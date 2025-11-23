import { defineStore } from 'pinia'

export const useNotifications = defineStore('notifications', {
  state: () => ({
    notifications: [],
  }),
  actions: {
    pushSuccess(message) {
      this.add({
        id: new Date().getTime(),
        message,
        type: 'success',
        show: true,
      })
    },
    pushError(message) {
      this.add({
        id: new Date().getTime(),
        message,
        type: 'error',
        show: true,
      })
    },
    remove(id) {
      this.notifications = this.notifications.filter((n) => n.id !== id)
    },
    removeAll() {
      this.notifications = []
    },
    add(messageObj) {
      const n = [...this.notifications, messageObj].slice(-3)
      this.notifications = n
      console.log(n)
    },
  },
})
