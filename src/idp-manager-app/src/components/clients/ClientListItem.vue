<template>
  <tr>
    <td class="px-6 py-4 whitespace-nowrap">
      <div class="text-sm text-gray-900">{{ item.displayName }}</div>
    </td>
    <td class="px-6 py-4 whitespace-nowrap">
      <div class="text-sm text-gray-700 font-monospace">{{ item.clientId }}</div>
    </td>
    <td class="px-6 py-4 whitespace-nowrap text-capitalize">
      <span class="badge rounded-pill" :class="getCss(item.applicationType)"
        >{{ item.applicationType }}
      </span>
    </td>
    <td class="px-6 py-4 whitespace-nowrap text-capitalize">
      <span class="badge rounded-pill" :class="getCss(item.clientType)">
        {{ item.clientType }}
      </span>
    </td>
    <td class="px-6 py-2 whitespace-nowrap text-right text-sm font-medium">
      <a href="javascript:void(0)" @click="navigateEdit" class="text-blue-600 hover:text-blue-900"
        >Edit</a
      >
      <ClientListItemDelete :clientId="item.clientId" /><br />
      <a href="javascript:void(0)" class="text-blue-600 hover:text-blue-900">Regenerate Secret</a>
    </td>
  </tr>
</template>
<script setup>
import { useRouter } from 'vue-router'
import ClientListItemDelete from './ClientListItemDelete.vue'

const { /* type {import('@/types').ClientListItem} */ item } = defineProps({
  item: Object,
})
console.log(item)
const router = useRouter()
const navigateEdit = () => router.push({ name: 'edit-client', params: { clientId: item.clientId } })
const getCss = (type) => {
  let css = ''
  switch (type.toUpperCase()) {
    case 'WEB':
      css = 'text-bg-info'
      break
    case 'NATIVE':
      css = 'text-bg-success'
      break
    case 'SPA':
      css = 'text-bg-purple-400'
      break
    case 'CONFIDENTIAL':
      css = 'text-bg-warning'
      break
    case 'PUBLIC':
      css = 'text-bg-light'
      break
  }
  return css
}
</script>
