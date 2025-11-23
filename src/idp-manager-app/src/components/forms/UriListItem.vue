<template>
  <div class="d-flex text-bg-light rounded-5 px-3 my-1 uri-display">
    <div class="w-100 pt-1">
      <span class="uri-value">{{ uri }}</span>
    </div>
    <div class="flex-shrink-1">
      <button type="button" class="btn btn-light btn-sm">
        <i class="bi bi-x"></i>
      </button>
    </div>
  </div>
</template>
<script setup>
const { uri } = defineProps({
  uri: String,
})
</script>
