<script setup>
import { onMounted } from 'vue'
import { useClientStore } from '@/stores/clients'
import ClientListItem from './ClientListItem.vue'
import { storeToRefs } from 'pinia'

const store = useClientStore()
const { list } = storeToRefs(store)

onMounted(() => void store.fetchClients())
</script>

<template>
  <table class="table table-hover">
    <thead class="bg-gray-50">
      <tr>
        <th
          scope="col"
          class="px-6 py-3 text-start font-medium text-secondary-emphasis text-uppercase tracking-wider"
        >
          Display Name
        </th>
        <th
          scope="col"
          class="px-6 py-3 text-start text-secondary-emphasis text-uppercase tracking-wider"
        >
          Client ID
        </th>
        <th
          scope="col"
          class="px-6 py-3 text-start text-secondary-emphasis text-uppercase tracking-wider"
        >
          Application Type
        </th>
        <th
          scope="col"
          class="px-6 py-3 text-start text-secondary-emphasis text-uppercase tracking-wider"
        >
          Client Type
        </th>
        <th scope="col" class="relative px-6 py-3">
          <span class="visually-hidden">Actions</span>
        </th>
      </tr>
    </thead>
    <tbody>
      <ClientListItem v-for="v in list" :item="v" :key="v.clientId" />
    </tbody>
  </table>
</template>
