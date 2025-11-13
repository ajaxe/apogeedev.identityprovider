import { createRouter, createWebHistory } from 'vue-router'
import ClientListView from '@/views/ClientListView.vue'

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
  ],
})

export default router
