<script setup>
import { useRouter } from 'vue-router'
import ClientList from '@/components/clients/ClientList.vue'

const router = useRouter()
const newClient = () => router.push({ name: 'new-client' })
</script>

<template>
  <div class="d-flex justify-content-between mb-3">
    <h1 class="text-3xl font-bold text-gray-800">OIDC Clients</h1>
    <button class="btn btn-primary btn-lg" style="line-height: 38px" @click="newClient">
      + Create New Client
    </button>
  </div>
  <div>
    <ClientList />
  </div>
</template>
