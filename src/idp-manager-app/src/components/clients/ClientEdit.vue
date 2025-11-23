<template>
  <div class="d-flex justify-content-between mb-3">
    <h1 class="text-3xl font-bold text-gray-800">OIDC Client Configuration</h1>
  </div>
  <div class="text-danger-emphasis my-2" v-if="errors.Other">
    {{ errors.Other[0] }}
  </div>
  <div class="card">
    <div class="card-body">
      <div class="mb-3">
        <label for="display-name" class="form-label">Display Name</label>
        <input class="form-control" id="display-name" v-model="model.displayName" />
        <div class="text-danger-emphasis" v-if="errors.DisplayName">
          {{ errors.DisplayName[0] }}
        </div>
      </div>
      <div class="mb-3">
        <label for="client-id" class="form-label">Client ID</label>
        <input asp-for="ClientId" class="form-control" id="client-id" v-model="model.clientId" />
        <div class="text-danger-emphasis" v-if="errors.ClientId">
          {{ errors.ClientId[0] }}
        </div>
      </div>
      <div class="row">
        <div class="mb-3 col-md-6">
          <label for="app-type" class="form-label">Application Type</label>
          <AppTypeSelect id="app-type" v-model="model.applicationType" />
          <div class="text-danger-emphasis" v-if="errors.ApplicationType">
            {{ errors.ApplicationType[0] }}
          </div>
        </div>
        <div class="mb-3 col-md-6">
          <label for="client-type" class="form-label">Client Type</label>
          <ClientTypeSelect id="client-type" v-model="model.clientType" />
          <div class="text-danger-emphasis" v-if="errors.ClientType">
            {{ errors.ClientType[0] }}
          </div>
        </div>
      </div>
      <div class="mb-3">
        <label for="redirect-uri" class="form-label">Redirect URIs</label>
        <MultipleUriAdd v-model="model.redirectUris" id="redirect-uri" />
        <div class="text-danger-emphasis" v-if="errors.RedirectUris">
          {{ errors.RedirectUris[0] }}
        </div>
      </div>
      <div class="mb-3">
        <label for="post-logout-uri" class="form-label">Post-Logout Redirect URIs</label>
        <MultipleUriAdd v-model="model.postLogoutRedirectUris" id="post-logout-uri" />
        <div class="text-danger-emphasis" v-if="errors.PostLogoutRedirectUris">
          {{ errors.PostLogoutRedirectUris[0] }}
        </div>
      </div>
      <button type="submit" class="btn btn-primary" @click="$emit('submit')">Submit</button>
      <button type="submit" class="btn btn-link ms-2" @click="$emit('cancel')">Cancel</button>
    </div>
  </div>
</template>
<script setup>
import AppTypeSelect from '@/components/forms/AppTypeSelect.vue'
import ClientTypeSelect from '@/components/forms/ClientTypeSelect.vue'
import MultipleUriAdd from '@/components/forms/MultipleUriAdd.vue'

const { errors } = defineProps({
  errors: {
    type: Object,
    default: () => ({}),
  },
})
/**
 * type {import('@/types').ClientListItem}
 */
const model = defineModel()
defineEmits(['submit', 'cancel'])
</script>
