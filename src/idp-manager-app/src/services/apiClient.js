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

const post = async (url, body) => {
  const token = getApiToken()
  if (!token) {
    return
  }
  const r = await fetch(url, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
      Authorization: `Bearer ${token}`,
    },
    body: JSON.stringify(body),
  })
  return r
}

const put = async (url, body) => {
  const token = getApiToken()
  if (!token) {
    return
  }
  const r = await fetch(url, {
    method: 'PUT',
    headers: {
      'Content-Type': 'application/json',
      Authorization: `Bearer ${token}`,
    },
    body: JSON.stringify(body),
  })
  return r
}

const deleteOp = async (url) => {
  const token = getApiToken()
  if (!token) {
    return
  }
  const r = await fetch(url, {
    method: 'DELETE',
    headers: {
      Authorization: `Bearer ${token}`,
    },
  })
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
  post,
  put,
  delete: deleteOp,
  isSuccessful,
}

const getApiToken = () => {
  const { token } = useAuthStore()
  if (!token) {
    return
  }
  return token
}
