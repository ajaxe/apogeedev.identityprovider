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
    async add(client) {
      const r = await apiClient.post('/api/app-client', client)
      const d = await r.json()
      if (!apiClient.isSuccessful(r)) {
        return {
          success: false,
          errors: d.errors,
        }
      }
      const { clientSecret, ...c } = d
      this.list.push(c)
      return c
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
