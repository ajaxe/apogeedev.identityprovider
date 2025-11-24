<template>
  <ClientEdit v-model="model" @cancel="gotoList" @submit="save" is-edit />
</template>
<script setup>
import ClientEdit from '@/components/clients/ClientEdit.vue'
import { useNotification } from '@/composables/useNotification'
import { useClientStore } from '@/stores/clients'
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'

const props = defineProps({
  clientId: String,
})
const { notifySuccess, notifyError } = useNotification()

const router = useRouter()
const store = useClientStore()
const model = ref(store.emptyClient)

onMounted(() => void load())

const load = async () => (model.value = await store.fetchClientById(props.clientId))

const gotoList = () => router.push({ name: 'clients' })

const save = async () => {
  try {
    const result = await store.update(model.value)
    if (result?.errors) {
      notifyError('Failed to update client.')
      return
    }
    notifySuccess('Client updated successfully.')
    gotoList()
  } catch (error) {
    notifyError('An unexpected error occurred while updating the client.')
    console.error(error)
  }
}
</script>
