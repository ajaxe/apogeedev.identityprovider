import { createRouter, createWebHistory } from 'vue-router'
import ClientListView from '@/views/ClientListView.vue'
import { useAuthStore } from '@/stores/auth'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    { path: '/', redirect: '/clients' },
    {
      path: '/clients',
      name: 'clients',
      component: ClientListView,
    },
    {
      path: '/clients/new',
      name: 'new-client',
      component: () => import('@/views/ClientNewView.vue'),
    },
    {
      path: '/clients/edit/:clientId',
      name: 'edit-client',
      props: true,
      component: () => import('@/views/ClientEditView.vue'),
    },
    {
      path: '/auth-callback', // <-- This must match your redirect_uri
      name: 'AuthCallback',
      component: () => import('@/views/AuthCallbackView.vue'),
    },
    {
      path: '/login',
      name: 'Login',
      component: () => import('@/views/LoginView.vue'),
    },
    {
      path: '/unauthorized',
      name: 'Unauthorized',
      component: () => import('@/views/UnauthorizedView.vue'),
    },
  ],
})

router.beforeEach(async (to) => {
  const store = useAuthStore()

  if (!store.user) await store.loadUser()
  if (!store.isAuthorized) await store.checkAuthorization()

  const canProceed = store.isLoggedIn && store.isAuthorized
  const publicRoutes = ['AuthCallback', 'Login', 'Unauthorized']
  const isPublicRoute = publicRoutes.includes(to.name)

  if (canProceed && to.name === 'Login') {
    return { name: 'clients' }
  }

  if (!canProceed && !isPublicRoute) {
    return store.isLoggedIn ? { name: 'Unauthorized' } : { name: 'Login' }
  }

  return true
})

export default router
