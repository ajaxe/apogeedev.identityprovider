import { useAuthStore } from '@/stores/auth'

const get = async (url, skipRedirect) => {
  skipRedirect = !!skipRedirect
  const token = getApiToken()
  if (!token) {
    return
  }
  const r = await fetch(url, {
    method: 'GET',
    headers: {
      Authorization: `Bearer ${token}`,
    },
  })

  if (!skipRedirect && !isSuccessful(r) && (r.status == 401 || r.status == 403)) {
    const store = useAuthStore()
    store.unauthorized()
    return
  }

  return r
}

/**
 *
 * @param {Response} response
 * @returns
 */
const isSuccessful = (response) => {
  return response.status >= 200 && response.status < 300
}
export const apiClient = {
  get,
  isSuccessful,
}

const getApiToken = () => {
  const { token } = useAuthStore()
  if (!token) {
    return
  }
  return token
}
