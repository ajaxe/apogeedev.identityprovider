<script setup>
import { ref, computed, onMounted, nextTick, useTemplateRef } from 'vue'
import { Modal } from 'bootstrap'

// Props: The name of the item being deleted (e.g., 'keytag-dev')
const props = defineProps({
  itemName: {
    type: String,
    required: true,
  },
  title: {
    type: String,
    default: 'Delete Client?',
  },
})

// Emits: Let the parent know when to proceed
const emit = defineEmits(['confirm', 'cancel'])

const modalRef = useTemplateRef('modal-ref')
const inputRef = ref(null)
const modalInstance = ref(null)
const confirmationInput = ref('')
const isDeleting = ref(false)

// Computed: Check if input matches exactly
const isMatch = computed(() => {
  return confirmationInput.value === props.itemName
})

// Method exposed to parent to open the modal
const show = () => {
  confirmationInput.value = '' // Reset input
  isDeleting.value = false // Reset loading state
  modalInstance.value.show()
}

// Method exposed to parent to hide (if needed manually)
const hide = () => {
  modalInstance.value.hide()
}

// Handle the actual delete click
const handleConfirm = () => {
  if (!isMatch.value) return

  isDeleting.value = true // Show spinner
  emit('confirm') // Tell parent to fire API call
}

// Initialize Bootstrap Modal & Focus Logic
onMounted(() => {
  if (modalRef.value) {
    modalInstance.value = new Modal(modalRef.value, {
      backdrop: 'static', // Prevent closing by clicking outside (high friction)
      keyboard: false, // Prevent closing via ESC (optional, usually good for danger)
    })

    // UX GOLD: Auto-focus the input field when modal opens
    modalRef.value.addEventListener('shown.bs.modal', () => {
      nextTick(() => inputRef.value?.focus())
    })
  }
})

// Expose methods to the parent component
defineExpose({ show, hide })
</script>

<template>
  <div class="modal fade" ref="modal-ref" tabindex="-1" aria-hidden="true">
    <div class="modal-dialog modal-dialog-centered">
      <div class="modal-content border-0 shadow-lg">
        <div class="modal-header border-bottom-0 pb-0">
          <h5 class="modal-title text-danger d-flex align-items-center gap-2">
            <i class="bi bi-exclamation-triangle-fill"></i>
            {{ title }}
          </h5>
          <button
            type="button"
            class="btn-close"
            data-bs-dismiss="modal"
            aria-label="Close"
            :disabled="isDeleting"
          ></button>
        </div>

        <div class="modal-body">
          <p class="lead mb-3">
            This action cannot be undone. This will permanently delete the
            <strong>{{ itemName }}</strong> client and remove all associated active sessions.
          </p>

          <label class="form-label small text-muted">
            Please type <strong>{{ itemName }}</strong> to confirm.
          </label>

          <input
            ref="inputRef"
            type="text"
            class="form-control"
            v-model="confirmationInput"
            :class="{ 'is-valid': isMatch }"
            placeholder="Type client name here"
            :disabled="isDeleting"
            @keyup.enter="handleConfirm"
          />
        </div>

        <div class="modal-footer border-top-0 pt-0">
          <button
            type="button"
            class="btn btn-light"
            data-bs-dismiss="modal"
            :disabled="isDeleting"
            @click="() => $emit('cancel')"
          >
            Cancel
          </button>

          <button
            type="button"
            class="btn btn-danger px-4"
            :disabled="!isMatch || isDeleting"
            @click="handleConfirm"
          >
            <span
              v-if="isDeleting"
              class="spinner-border spinner-border-sm me-2"
              role="status"
              aria-hidden="true"
            ></span>
            <span v-if="isDeleting">Deleting...</span>
            <span v-else>Delete Client</span>
          </button>
        </div>
      </div>
    </div>
  </div>
</template>
