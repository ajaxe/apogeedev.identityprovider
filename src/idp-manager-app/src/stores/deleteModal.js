import { defineStore } from 'pinia'

export const useDeleteModalStore = defineStore('deleteModal', {
  state: () => ({
    displayName: '',
    isOpen: false,
    resolver: null,
  }),
  getters: {
    title: (state) => `Delete '${state.displayName}' Client?`,
  },
  actions: {
    handleCancel() {
      this.isOpen = false
      this.resolver(false)
    },
    async handleConfirm() {
      this.isOpen = false
      this.resolver(true)
    },
    handleHidden() {
      this.isOpen = false
    },
    async confirm({ displayName }) {
      this.displayName = displayName
      this.isOpen = true

      return new Promise((resolve) => {
        this.resolver = resolve
      })
    },
  },
})
