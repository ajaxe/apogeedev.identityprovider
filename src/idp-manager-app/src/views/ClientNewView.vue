<template>
  <ClientEdit v-model="model" @cancel="gotoList" @submit="save" :errors="errors" />
</template>
<script setup>
import { ref } from 'vue'
import ClientEdit from '@/components/clients/ClientEdit.vue'
import { useRouter } from 'vue-router'
import { useClientStore } from '@/stores/clients'
import { useNotification } from '@/composables/useNotification'

const router = useRouter()
const store = useClientStore()
const { notifyError, notifySuccess } = useNotification()

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
    notifyError('Fix validation errors.')
  } else {
    notifySuccess('Application client successfuly added.')
  }
}
</script>
