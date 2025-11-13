<script setup lang="ts">
import ClientList from '@/components/clients/ClientList.vue'
</script>

<template>
  <div class="d-flex justify-content-between mb-3">
    <h1 class="text-3xl font-bold text-gray-800">OIDC Clients</h1>
    <a
      asp-controller="Manage"
      asp-action="ClientAdd"
      class="btn btn-primary btn-lg"
      style="line-height: 38px"
    >
      + Create New Client
    </a>
  </div>
  <div>
    <ClientList />
  </div>
</template>
