<template>
  <select :id="id" class="form-select" v-model="model">
    <option disabled value="">Please select one</option>
    <option v-for="o in opts" :value="o.value" :key="o.value">{{ o.label }}</option>
  </select>
</template>
<script setup>
const model = defineModel()
const { id } = defineProps({
  id: String,
})
const opts = [
  { value: 'public', label: 'Public' },
  { value: 'confidential', label: 'Confidential' },
]
</script>
