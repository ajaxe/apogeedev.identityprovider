import { UserManager } from 'oidc-client-ts'

// Get the base URL of your app
let base = window.APP_CONFIG?.API_URL_BASE
if (base && base.endsWith('/')) base = base.substring(0, base.length - 1)
const appUrl = `${window.location.protocol}//${window.location.host}${base}`

/**
 * @type {import('oidc-client-ts').UserManagerSettings}
 */
const settings = {
  authority: window.APP_CONFIG.OIDC_AUTHORITY,
  client_id: window.APP_CONFIG.OIDC_CLIENT_ID,

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
