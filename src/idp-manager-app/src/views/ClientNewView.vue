<template>
  <ClientEdit v-model="model" @cancel="gotoList" @submit="save" :errors="errors" />
</template>
<script setup>
import { ref } from 'vue'
import ClientEdit from '@/components/clients/ClientEdit.vue'
import { useRouter } from 'vue-router'
import { useClientStore } from '@/stores/clients'
import { useNotifications } from '@/stores/notifications'

const router = useRouter()
const store = useClientStore()
const notifStore = useNotifications()

const model = ref({
  displayName: '',
  clientId: '',
  clientSecret: '',
  applicationType: '',
  clientType: '',
  redirectUris: [],
  postLogoutRedirectUris: [],
})

const errors = ref({})

const gotoList = () => router.push({ name: 'clients' })

const save = async () => {
  var result = await store.add(model.value)
  if (result.errors) {
    errors.value = result.errors
    notifStore.pushError('Fix validation errors.')
  } else {
    notifStore.pushSuccess('Application client successfuly added.')
  }
}
</script>
