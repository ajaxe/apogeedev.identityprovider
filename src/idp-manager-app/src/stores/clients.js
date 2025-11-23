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
    /**
     *
     * @param {import('@/types').ClientListItem[]} client
     * @returns {Promise<import('@/types').AppClientDataWithSecretRespose>}
     */
    async add(client) {
      const r = await apiClient.post('/api/app-client', client)
      const d = await r.json()
      if (!apiClient.isSuccessful(r)) {
        return {
          errors: d.errors,
        }
      }
      const { appClientData } = d
      const { clientSecret, ...c } = appClientData
      this.list.push(c)
      return appClientData
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
    async deleteClient(clientId) {
      const r = await apiClient.delete(`/api/app-client/${clientId}`)
      if (apiClient.isSuccessful(r)) {
        this.list = this.list.filter((c) => c.clientId !== clientId)
        return true
      } else {
        return false
      }
    },
  },
})
