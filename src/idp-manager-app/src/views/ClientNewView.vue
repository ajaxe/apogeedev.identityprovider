<template>
  <ClientEdit v-model="model" @cancel="gotoList"/>
</template>
<script setup>
import { ref } from 'vue'
import ClientEdit from '@/components/clients/ClientEdit.vue'
import { useRouter } from 'vue-router';

const router = useRouter();

const model = ref({
  displayName: '',
  clientId: '',
  clientSecret: '',
  applicationType: '',
  clientType: '',
  redirectUris: [],
  postLogoutRedirectUris: [],
})

const gotoList = () => router.push({ name: 'clients' })
</script>
