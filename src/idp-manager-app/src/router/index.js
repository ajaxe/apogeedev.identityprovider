import { createRouter, createWebHistory } from 'vue-router'
import ClientListView from '@/views/ClientListView.vue'
import { useAuth } from '@/composables/useAuth'

const { user } = useAuth()

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
  ],
})

router.beforeEach((to, from) => {
  if (to.name == 'AuthCallback' || to.name == 'Login') return
  else if (to.name === 'Login' && user) {
    return { name: 'clients' }
  } else if (!user.value) {
    return { name: 'Login' }
  }
})

export default router
