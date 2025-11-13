<template>
  <div>{{ uri }}</div>
</template>
<script setup>
const { uri } = defineProps({
  uri: String,
})
</script>
