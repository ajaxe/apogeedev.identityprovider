<template>
  <div
    class="toast align-items-center text-bg-success border-0"
    :class="[colorClass]"
    role="alert"
    aria-live="assertive"
    aria-atomic="true"
    data-bs-animation="true"
    data-bs-delay="8000"
    v-on="{ 'hidden.bs.toast': () => $emit('hidden', notification.id) }"
    ref="toastRef"
  >
    <div class="d-flex">
      <div class="toast-body">
        <i class="bi me-2" :class="iconClass"></i>
        <span>{{ message }}</span>
      </div>
      <button
        type="button"
        class="btn-close btn-close-white me-2 m-auto"
        data-bs-dismiss="toast"
        aria-label="Close"
      ></button>
    </div>
  </div>
</template>
<script setup>
import { Toast } from 'bootstrap'
import { computed, ref, onMounted } from 'vue'

const { notification } = defineProps({
  notification: {
    type: Object,
    required: true,
  },
})

const toastRef = ref(null)

const colorClass = computed(() =>
  notification.type === 'error' ? 'text-bg-danger' : 'text-bg-success',
)
const iconClass = computed(() =>
  notification.type === 'error' ? 'bi-exclamation-octagon' : 'bi-check-circle',
)
const message = ref(notification.message)

onMounted(() => {
  const toast = new Toast(toastRef.value)
  toast.show()
})

defineEmits(['hidden'])
</script>
