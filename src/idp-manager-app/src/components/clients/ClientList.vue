<script setup>
import { onMounted, useTemplateRef, watch } from 'vue'
import { useClientStore } from '@/stores/clients'
import ClientListItem from './ClientListItem.vue'
import { storeToRefs } from 'pinia'
import { useDeleteModalStore } from '@/stores/deleteModal'
import DeleteConfirmationModal from '../modals/DeleteConfirmationModal.vue'
import { useViewport } from '@/composables/useViewport'
import ClientCard from './ClientCard.vue'

const store = useClientStore()
const deleteModalStore = useDeleteModalStore()
const { list } = storeToRefs(store)

onMounted(() => void store.fetchClients())

const deleteModalRef = useTemplateRef('delete-modal')

const { breakpoints } = useViewport()

watch(
  () => deleteModalStore.isOpen,
  (isOpen) => {
    if (isOpen) {
      deleteModalRef.value.show()
    } else {
      deleteModalRef.value.hide()
    }
  },
)
</script>

<template>
  <table class="table table-hover" v-if="breakpoints.lg">
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
  <template v-else>
    <ClientCard v-for="v in list" :key="v.clientId" :client="v" />
  </template>
  <DeleteConfirmationModal
    ref="delete-modal"
    :item-name="deleteModalStore.displayName"
    :title="deleteModalStore.title"
    @confirm="deleteModalStore.handleConfirm"
    @hidden="deleteModalStore.handleHidden"
    @cancel="deleteModalStore.handleCancel"
  />
</template>
