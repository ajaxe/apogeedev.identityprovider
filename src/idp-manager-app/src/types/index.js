/**
 * @typedef {object} ClientListItem
 * @property {string} displayName
 * @property {string} clientId
 * @property {string} applicationType
 * @property {string} clientType
 * @property {Array<string>} redirectUris
 * @property {Array<string>} postLogoutRedirectUris
 */

/**
 * @typedef {ClientListItem & { clientSecret: string } AppClientDataWithSecret
 */

/**
 * @typedef {Object<string, string[]>} ErrorResponseProps
 *
 */
/**
 * @typedef {object} ErrorsRespose
 * @property {ErrorResponseProps} errors
 */
/**
 * @typedef {AppClientDataWithSecret & ErrorsRespose} AppClientDataWithSecretRespose
 */
export {}
