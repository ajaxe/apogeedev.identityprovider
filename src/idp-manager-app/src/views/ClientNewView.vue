<template>
  <ClientEdit v-model="model" />
</template>
<script setup>
import { ref } from 'vue'
import ClientEdit from '@/components/clients/ClientEdit.vue'
const model = ref({
  displayName: '',
  clientId: '',
  clientSecret: '',
  applicationType: '',
  clientType: '',
  redirectUris: [],
  postLogoutRedirectUris: [],
})
</script>
