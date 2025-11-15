import { computed } from 'vue'
import { useAuthStore } from '@/stores/auth'
import { storeToRefs } from 'pinia'

export function useAuth() {
  const store = useAuthStore()
  const { user } = storeToRefs(store)

  // Check user status on app load
  const loadUser = async () => {
    await store.loadUser()
  }

  const login = () => store.login()

  const logout = () => store.logout()

  const handleLoginCallback = async () => {
    return await store.handleLoginCallback()
  }

  // Expose the state and methods
  return {
    isLoggedIn: computed(() => store.isLoggedIn),
    profile: computed(() => store.user?.profile),
    user, // The reactive user object
    loadUser,
    login,
    logout,
    handleLoginCallback,
  }
}
