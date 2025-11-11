import { createRouter, createWebHistory } from 'vue-router'
import ClientListView from '@/views/ClientListView.vue'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      name: 'home',
      component: ClientListView,
    },
  ],
})

export default router
