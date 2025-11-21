<template>
  <ClientEdit v-model="model" @cancel="gotoList" @submit="save" />
</template>
<script setup>
import ClientEdit from '@/components/clients/ClientEdit.vue'
import { useClientStore } from '@/stores/clients'
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'

const props = defineProps({
  clientId: String,
})

const router = useRouter()
const store = useClientStore()
const model = ref(store.emptyClient)

onMounted(() => void load())

const load = async () => (model.value = await store.fetchClientById(props.clientId))

const gotoList = () => router.push({ name: 'clients' })

const save = async () => {
  await store.add(model.value)
}
</script>
