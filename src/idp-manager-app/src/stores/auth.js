import { defineStore } from 'pinia'
import { userManager } from '@/services/auth.service'
import router from '@/router'
import { apiClient } from '@/services/apiClient'

export const useAuthStore = defineStore('auth', {
  state: () => ({
    user: null,
    isAuthorized: false,
  }),
  getters: {
    isLoggedIn: (state) => !!state.user,
    token: (state) => state.user?.access_token,
  },
  actions: {
    async loadUser() {
      const userFromStorage = await userManager.getUser()
      if (userFromStorage && !userFromStorage.expired) {
        this.user = userFromStorage
      }
    },
    login() {
      // Redirects the user to the OIDC provider's login page
      userManager.signinRedirect()
    },
    logout() {
      // Redirects the user to the OIDC provider's logout page
      userManager.signoutRedirect()
    },
    async handleLoginCallback() {
      try {
        // Complete the login, exchange code for tokens
        const loggedInUser = await userManager.signinRedirectCallback()
        this.user = loggedInUser
        // Redirect to the home page
        router.push('/')
      } catch (e) {
        console.error('Error handling login callback:', e)
        router.push('/login-failed') // Or some error page
      }
    },
    unauthorized() {
      this.user = null
      this.isAuthorized = false
      router.push({ name: 'Unauthorized' })
    },
    async checkAuthorization() {
      if(!this.token) {
        return false
      }
      const r = await apiClient.get('/api/manager/check-authorization', true)
      this.isAuthorized = r.status !== 403 || r.status !== 401
      return this.isAuthorized
    },
  },
})
