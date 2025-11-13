<template>
  <div class="d-flex justify-content-between mb-3">
    <h1 class="text-3xl font-bold text-gray-800">OIDC Client Configuration</h1>
  </div>

  <div class="card">
    <div class="card-body">
      <div class="mb-3">
        <label for="display-name" class="form-label">Display Name</label>
        <input class="form-control" id="display-name" v-model="model.displayName" />
      </div>
      <div class="mb-3">
        <label for="client-id" class="form-label">Client ID</label>
        <input
          asp-for="ClientId"
          class="form-control"
          id="client-id"
          v-model="model.clientId"
          :readonly="true"
        />
      </div>
      <div class="mb-3">
        <label for="client-secret" class="form-label">Client Secret</label>
        <input class="form-control" id="client-secret" />
      </div>
      <div class="row">
        <div class="mb-3 col">
          <label for="app-type" class="form-label">Application Type</label>
          <AppTypeSelect id="app-type" v-model="model.applicationType" />
        </div>
        <div class="mb-3 col">
          <label for="client-type" class="form-label">Client Type</label>
          <ClientTypeSelect id="client-type" v-model="model.clientType" />
        </div>
      </div>
      <div class="mb-3">
        <label for="@Model.RedirectUrisViewModel.Id" class="form-label">Redirect URIs</label>
        <MultipleUriAdd v-model="model.redirectUris" />
      </div>
      <div class="mb-3">
        <label for="@Model.PostLogoutRedirectUrisViewModel.Id" class="form-label"
          >Post-Logout Redirect URIs</label
        >
        <MultipleUriAdd v-model="model.postLogoutRedirectUris" />
      </div>
      <button type="submit" class="btn btn-primary">Submit</button>
    </div>
  </div>
</template>
<script setup>
import AppTypeSelect from '../common/AppTypeSelect.vue'
import ClientTypeSelect from '../common/ClientTypeSelect.vue'
import MultipleUriAdd from '../common/MultipleUriAdd.vue'

/**
 * type {import('@/types').ClientListItem}
 */
const model = defineModel()
</script>
