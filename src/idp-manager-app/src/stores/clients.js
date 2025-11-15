import { defineStore } from 'pinia'
import { apiClient } from '@/services/apiClient'

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
      const r = await apiClient.get('/api/app-client')
      const d = await r.json()
      this.list = d
    },
    async fetchClientById(clientId) {
      const r = await apiClient.get(`/api/app-client/${clientId}`)
      const d = await r.json()
      return d
    },
  },
})
