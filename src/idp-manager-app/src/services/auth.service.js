import { UserManager } from 'oidc-client-ts'

// Get the base URL of your app
const appUrl = `${window.location.protocol}//${window.location.host}`

/**
 * @type {import('oidc-client-ts').UserManagerSettings}
 */
const settings = {
  authority: import.meta.env.VITE_OIDC_AUTHORITY,
  client_id: import.meta.env.VITE_OIDC_CLIENT_ID,

  // The route in your Vue app that will handle the callback
  redirect_uri: `${appUrl}/auth-callback`,

  // The route to return to after logging out
  post_logout_redirect_uri: `${appUrl}/clients`,

  // **This enables the Auth Code + PKCE flow**
  response_type: 'code',
  scope: 'openid profile email',

  automaticSilentRenew: true,
  loadUserInfo: true,
}

// Export the single instance
export const userManager = new UserManager(settings)
