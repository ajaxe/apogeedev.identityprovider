import { ref } from 'vue'
import { userManager } from '@/services/auth.service'
import { useRouter } from 'vue-router'

// A reactive 'ref' to hold the user.
// This is the global auth state for your app.
const user = ref(null)

export function useAuth() {
  const router = useRouter()

  // Check user status on app load
  const loadUser = async () => {
    const userFromStorage = await userManager.getUser()
    if (userFromStorage && !userFromStorage.expired) {
      user.value = userFromStorage
    }
  }

  // --- Auth Methods ---

  const login = () => {
    // Redirects the user to the OIDC provider's login page
    userManager.signinRedirect()
  }

  const logout = () => {
    // Redirects the user to the OIDC provider's logout page
    userManager.signoutRedirect()
  }

  // --- Callback Handlers (run on your callback page) ---

  const handleLoginCallback = async () => {
    try {
      // Complete the login, exchange code for tokens
      const loggedInUser = await userManager.signinRedirectCallback()
      user.value = loggedInUser
      // Redirect to the home page
      router.push('/')
    } catch (e) {
      console.error('Error handling login callback:', e)
      router.push('/login-failed') // Or some error page
    }
  }

  // Expose the state and methods
  return {
    user, // The reactive user object
    loadUser,
    login,
    logout,
    handleLoginCallback,
  }
}
