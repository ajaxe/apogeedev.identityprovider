<template>
  <div :class="css">
    <span v-if="isEmpty" class="text-muted small fst-italic">No URIs added yet</span>
    <span v-else class="me-2 text-light">{{ u }}</span>
    <button
      v-if="!isEmpty"
      @click="() => $emit('click')"
      type="button"
      class="btn-close btn-close-white small"
      aria-label="Remove"
      style="width: 0.5rem; height: 0.5rem"
    ></button>
  </div>
</template>
<script setup>
import { computed } from 'vue'
const { uri, isEmpty } = defineProps({
  uri: String,
  isEmpty: Boolean,
})
const u = computed(() => (isEmpty ? 'No URI Added' : uri))
const css = isEmpty
  ? ''
  : 'badge bg-light rounded-pill bg-dark border border-secondary p-2 d-flex align-items-center'
</script>
