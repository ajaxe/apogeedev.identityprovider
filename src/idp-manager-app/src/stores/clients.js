import { defineStore } from 'pinia'

export const useClientStore = defineStore('clients', {
  state: () => ({
    /**
     * @type {import('@/types').ClientListItem[]}
     */
    list: [],
  }),
  getters: {
    emptyClient: () => ({
      displayName: '',
      clientId: '',
      clientSecret: '',
      applicationType: '',
      clientType: '',
      redirectUris: [],
      postLogoutRedirectUris: [],
    }),
  },
  actions: {
    add(client) {
      this.list.push(client)
    },
    async fetchClients() {
      const r = await fetch('/api/app-client')
      this.list = await r.json()
    },
    async fetchClientById(clientId) {
      const r = await fetch(`/api/app-client/${clientId}`)
      const d = await r.json()
      return d
    },
  },
})
