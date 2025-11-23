<template>
  <ClientEdit v-model="model" @cancel="gotoList" @submit="save" :errors="errors" />
  <ClientSecretModal
    :is-open="showSuccessModal"
    :client-name="newClientName"
    :client-id="newClientId"
    :client-secret="generatedClientSecret"
    @close="onModalClose"
  />
</template>
<script setup>
import { ref } from 'vue'
import ClientEdit from '@/components/clients/ClientEdit.vue'
import { useRouter } from 'vue-router'
import { useClientStore } from '@/stores/clients'
import { useNotification } from '@/composables/useNotification'
import ClientSecretModal from '@/components/modals/ClientSecretModal.vue'

const router = useRouter()
const store = useClientStore()
const { notifyError, notifySuccess } = useNotification()
const showSuccessModal = ref(false)

const model = ref({
  displayName: '',
  clientId: '',
  clientSecret: '',
  applicationType: '',
  clientType: '',
  redirectUris: [],
  postLogoutRedirectUris: [],
})

const newClientId = ref('')
const newClientName = ref('')
const generatedClientSecret = ref('')

const onModalClose = () => {
  showSuccessModal.value = false
  newClientId.value = ''
  newClientName.value = ''
  generatedClientSecret.value = ''

  gotoList()
}

const errors = ref({})

const gotoList = () => router.push({ name: 'clients' })

const save = async () => {
  var result = await store.add(model.value)
  if (result.errors) {
    errors.value = result.errors
    notifyError('Fix validation errors.')
  } else {
    if (result.clientType !== 'public') {
      newClientId.value = result.clientId
      newClientName.value = model.value.displayName
      generatedClientSecret.value = result.clientSecret
      showSuccessModal.value = true
    }
    notifySuccess('Application client successfuly added.')
  }
}
</script>
