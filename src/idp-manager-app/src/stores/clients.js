import { defineStore } from 'pinia'
import { apiClient } from '@/services/apiClient'

export const useClientStore = defineStore('clients', {
  state: () => ({
    /**
     * @type {import('@/types').ClientListItem[]}
     */
    list: [],
    activeFilter: 'All',
    searchQuery: '',
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

    filteredClients: (state) =>
      state.list.filter((client) => {
        let matches = true
        if (state.searchQuery) {
          matches = client.displayName.toLowerCase().includes(state.searchQuery.toLowerCase())
        }
        const filter = state.activeFilter?.toLowerCase()
        if (matches && filter) {
          if (filter === 'confidential' || filter === 'public') {
            matches = client.clientType === filter
          } else if (filter === 'web' || filter === 'native') {
            matches = client.applicationType === filter
          } else if (filter === 'local') {
            matches = client.displayName?.toUpperCase().indexOf('LOCAL') !== -1
          }
        }
        return matches
      }),

    stats: (state) => {
      const local = state.list.filter(
        (c) => c.displayName?.toUpperCase().indexOf('LOCAL') !== -1,
      ).length
      const total = state.list.length
      return {
        total: total,
        confidential: state.list.filter((c) => c.clientType === 'confidential').length,
        public: state.list.filter((c) => c.clientType === 'public').length,
        web: state.list.filter((c) => c.applicationType === 'web').length,
        native: state.list.filter((c) => c.applicationType === 'native').length,
        local: local,
        remote: total - local,
        localPercent: (local / total) * 100,
      }
    },
  },
  actions: {
    /**
     *
     * @param {import('@/types').ClientListItem} client
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
    /**
     *
     * @param {import('@/types').ClientListItem} client
     * @returns {Promise<import('@/types').ErrorsRespose|void>}
     */
    async update(client) {
      const r = await apiClient.put(`/api/app-client/${client.clientId}`, client)

      if (!apiClient.isSuccessful(r)) {
        const d = await r.json()
        return {
          errors: d.errors,
        }
      }
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

function applyFilters(clients, { q, filter }) {
  console.log('Applying filters:', { q, filter })
  return clients.filter((client) => {
    let matches = true
    if (q) {
      matches = client.displayName.toLowerCase().includes(q.toLowerCase())
    }
    if (matches && filter) {
      if (filter === 'confidential' || filter === 'public') {
        matches = client.clientType === filter
      } else if (filter === 'web' || filter === 'native') {
        matches = client.applicationType === filter
      }
    }
    return matches
  })
}
