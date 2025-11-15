import { defineStore } from 'pinia'
import { useAuth } from '@/composables/useAuth'

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
      const token = getApiToken()
      if (!token) {
        return
      }
      const r = await fetch('/api/app-client', {
        method: 'GET',
        headers: {
          Authorization: `Bearer ${token}`,
        },
      })
      this.list = await r.json()
    },
    async fetchClientById(clientId) {
      const token = getApiToken()
      if (!token) {
        return
      }
      const r = await fetch(`/api/app-client/${clientId}`, {
        method: 'GET',
        headers: {
          Authorization: `Bearer ${token}`,
        },
      })
      const d = await r.json()
      return d
    },
  },
})

const getApiToken = () => {
  const { user } = useAuth()
  if (!user.value) {
    return
  }
  return user.value.access_token
}
