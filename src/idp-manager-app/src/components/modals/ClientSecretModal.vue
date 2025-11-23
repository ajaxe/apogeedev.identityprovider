<template>
  <div
    v-if="isOpen"
    class="modal fade show d-block"
    tabindex="-1"
    role="dialog"
    aria-modal="true"
    style="background-color: rgba(0, 0, 0, 0.5)"
  >
    <div class="modal-dialog modal-dialog-centered" role="document">
      <div class="modal-content shadow-lg">
        <div class="modal-header border-0 d-flex flex-column align-items-center pt-4">
          <div class="text-success mb-2">
            <svg
              xmlns="http://www.w3.org/2000/svg"
              width="64"
              height="64"
              fill="currentColor"
              class="bi bi-check-circle-fill"
              viewBox="0 0 16 16"
            >
              <path
                d="M16 8A8 8 0 1 1 0 8a8 8 0 0 1 16 0zm-3.97-3.03a.75.75 0 0 0-1.08.022L7.477 9.417 5.384 7.323a.75.75 0 0 0-1.06 1.06L6.97 11.03a.75.75 0 0 0 1.079-.02l3.992-4.99a.75.75 0 0 0-.01-1.05z"
              />
            </svg>
          </div>
          <h4 class="modal-title fw-bold">Client Successfully Created</h4>
          <p class="text-muted mb-0">{{ clientName }}</p>
        </div>

        <div class="modal-body px-4">
          <div class="alert alert-warning d-flex align-items-start mb-4" role="alert">
            <span class="me-2 fs-4">⚠️</span>
            <div>
              <strong>Save your Client Secret now.</strong>
              <div class="small">
                This secret is only displayed once. We do not store it in plain text. If you lose
                it, you will have to generate a new one.
              </div>
            </div>
          </div>

          <div class="mb-3">
            <label class="form-label text-uppercase small fw-bold text-muted">Client ID</label>
            <div class="input-group">
              <input
                type="text"
                class="form-control font-monospace text-bg-light"
                :value="clientId"
                readonly
              />
              <button
                class="btn btn-outline-secondary"
                type="button"
                @click="copyToClipboard(clientId, 'id')"
              >
                <span v-if="copiedField === 'id'">Saved!</span>
                <span v-else>Copy</span>
              </button>
            </div>
          </div>

          <div class="mb-3">
            <label class="form-label text-uppercase small fw-bold text-muted">Client Secret</label>
            <div class="input-group">
              <input
                :type="isSecretVisible ? 'text' : 'password'"
                class="form-control font-monospace bg-light text-dark"
                :value="clientSecret"
                readonly
              />

              <button
                class="btn btn-outline-secondary"
                type="button"
                @click="isSecretVisible = !isSecretVisible"
                title="Show/Hide"
              >
                <span v-if="isSecretVisible">Hide</span>
                <span v-else>Show</span>
              </button>

              <button
                class="btn btn-primary"
                type="button"
                @click="copyToClipboard(clientSecret, 'secret')"
              >
                <span v-if="copiedField === 'secret'">Saved!</span>
                <span v-else>Copy Secret</span>
              </button>
            </div>
          </div>

          <div class="text-center mt-4">
            <button class="btn btn-link btn-sm text-decoration-none" @click="downloadConfig">
              ⬇️ Download client-config.json
            </button>
          </div>
        </div>

        <div class="modal-footer justify-content-center border-0 pb-4">
          <button type="button" class="btn btn-success w-100 py-2 mx-3" @click="handleClose">
            I have saved my Client Secret
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue'

const props = defineProps({
  isOpen: {
    type: Boolean,
    required: true,
  },
  clientName: {
    type: String,
    default: 'My New Application',
  },
  clientId: {
    type: String,
    required: true,
  },
  clientSecret: {
    type: String,
    required: true,
  },
})

const emit = defineEmits(['close'])

// State
const isSecretVisible = ref(false)
const copiedField = ref(null) // 'id' or 'secret' to track which button shows "Saved!"

// Logic: Copy to Clipboard
const copyToClipboard = async (text, field) => {
  try {
    await navigator.clipboard.writeText(text)
    copiedField.value = field

    // Reset the "Saved!" text after 2 seconds
    setTimeout(() => {
      copiedField.value = null
    }, 2000)
  } catch (err) {
    console.error('Failed to copy: ', err)
  }
}

// Logic: Download JSON
const downloadConfig = () => {
  const data = {
    client_name: props.clientName,
    client_id: props.clientId,
    client_secret: props.clientSecret,
  }

  const blob = new Blob([JSON.stringify(data, null, 2)], { type: 'application/json' })
  const url = window.URL.createObjectURL(blob)
  const a = document.createElement('a')
  a.href = url
  a.download = `${props.clientName.replace(/\s+/g, '_').toLowerCase()}_config.json`
  a.click()
  window.URL.revokeObjectURL(url)
}

// Logic: Close Modal
const handleClose = () => {
  emit('close')
}
</script>

<style scoped>
/* Ensure the monospace font looks distinct for credentials */
.font-monospace {
  font-family: 'Courier New', Courier, monospace;
  letter-spacing: 0.5px;
}
</style>
