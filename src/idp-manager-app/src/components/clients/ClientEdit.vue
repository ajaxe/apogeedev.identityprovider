<template>
  <!-- Header -->
  <div class="d-flex justify-content-between align-items-center mb-4">
    <h1 class="h3 mb-0 text-light fw-bold">OIDC Client Configuration</h1>
  </div>

  <!-- Global Error Alert -->
  <div
    class="alert alert-danger border-danger d-flex align-items-center"
    role="alert"
    v-if="errors?.Other"
  >
    <i class="bi bi-exclamation-triangle-fill me-2"></i>
    <div>{{ errors?.Other[0] }}</div>
  </div>

  <!-- Main Card -->
  <div class="card bg-dark text-light border-secondary shadow-sm">
    <div class="card-body p-4">
      <!-- Row 1: Identity Info -->
      <div class="row g-3 mb-3">
        <div class="col-md-6">
          <label for="display-name" class="form-label text-light">Display Name</label>
          <input
            class="form-control bg-dark text-light border-secondary"
            id="display-name"
            v-model="model.displayName"
            placeholder="e.g. My Customer Portal"
          />
          <div class="text-danger-emphasis small mt-1" v-if="errors?.DisplayName">
            {{ errors?.DisplayName[0] }}
          </div>
        </div>

        <div class="col-md-6">
          <label for="client-id" class="form-label text-light">Client ID</label>
          <input
            class="form-control bg-dark text-light border-secondary"
            id="client-id"
            v-model="model.clientId"
            asp-for="ClientId"
            :readonly="isEdit"
          />
          <div class="text-danger-emphasis small mt-1" v-if="errors?.ClientId">
            {{ errors?.ClientId[0] }}
          </div>
        </div>
      </div>

      <!-- Row 2: Types Configuration -->
      <div class="row g-3 mb-4">
        <div class="col-md-6">
          <label for="app-type" class="form-label text-light">Application Type</label>
          <!-- Passing dark classes to custom component, assuming fallthrough attributes -->
          <AppTypeSelect
            id="app-type"
            v-model="model.applicationType"
            class="form-select bg-dark text-light border-secondary"
          />
          <div class="text-danger-emphasis small mt-1" v-if="errors?.ApplicationType">
            {{ errors?.ApplicationType[0] }}
          </div>
        </div>

        <div class="col-md-6">
          <label for="client-type" class="form-label text-light">Client Type</label>
          <ClientTypeSelect
            id="client-type"
            v-model="model.clientType"
            class="form-select bg-dark text-light border-secondary"
          />
          <div class="text-danger-emphasis small mt-1" v-if="errors?.ClientType">
            {{ errors?.ClientType[0] }}
          </div>
        </div>
      </div>

      <hr class="border-secondary opacity-50 my-4" />

      <!-- Row 3: Redirect URIs -->
      <div class="mb-4">
        <label for="redirect-uri" class="form-label text-light">Redirect URIs</label>
        <!-- Preserving your existing component -->
        <MultipleUriAdd v-model="model.redirectUris" id="redirect-uri" />
        <div class="text-danger-emphasis small mt-1" v-if="errors?.RedirectUris">
          {{ errors?.RedirectUris[0] }}
        </div>
      </div>

      <!-- Row 4: Post-Logout URIs -->
      <div class="mb-4">
        <label for="post-logout-uri" class="form-label text-light">Post-Logout Redirect URIs</label>
        <MultipleUriAdd v-model="model.postLogoutRedirectUris" id="post-logout-uri" />
        <div class="text-danger-emphasis small mt-1" v-if="errors?.PostLogoutRedirectUris">
          {{ errors?.PostLogoutRedirectUris[0] }}
        </div>
      </div>

      <!-- Footer Actions -->
      <div class="d-flex justify-content-end gap-2 mt-5">
        <button
          type="button"
          class="btn btn-outline-light border-secondary text-muted"
          @click="$emit('cancel')"
        >
          Cancel
        </button>
        <button type="submit" class="btn btn-primary px-4" @click="$emit('submit')">Submit</button>
      </div>
    </div>
  </div>
</template>

<style scoped>
/* Ensure inputs stay dark on focus/active states */
.form-control:focus,
.form-select:focus {
  background-color: #212529; /* bg-dark */
  color: #f8f9fa; /* text-light */
  border-color: #0d6efd;
  box-shadow: 0 0 0 0.25rem rgba(13, 110, 253, 0.25);
}

/* Optional: If MultipleUriAdd uses internal inputs, you might need deep selectors
   to force them to dark mode if the component doesn't support it natively */
:deep(.form-control) {
  background-color: #212529;
  color: #f8f9fa;
  border-color: #6c757d;
}
</style>
<script setup>
import AppTypeSelect from '@/components/forms/AppTypeSelect.vue'
import ClientTypeSelect from '@/components/forms/ClientTypeSelect.vue'
import MultipleUriAdd from '@/components/forms/MultipleUriAdd.vue'

const { errors, isEdit } = defineProps({
  errors: {
    type: Object,
    default: () => ({}),
  },
  isEdit: {
    type: Boolean,
    default: false,
  },
})
/**
 * type {import('@/types').ClientListItem}
 */
const model = defineModel()
defineEmits(['submit', 'cancel'])
</script>
