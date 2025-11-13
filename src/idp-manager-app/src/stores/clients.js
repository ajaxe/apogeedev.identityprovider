import { defineStore } from 'pinia'

export const useClientStore = defineStore('clients', {
  state: () => ({
    /**
     * @type {import('@/types').ClientListItem[]}
     */
    list: [],
  }),
  actions: {
    add(client) {
      this.list.push(client)
    },
    async fetchClients() {
      const r = await fetch('/api/app-client')
      this.list = await r.json()
    },
  },
})
