# Phase 2: Frontend Implementation for IdP Manager App

## Goal
Update the `idp-manager-app` Vue frontend to allow the registration and configuration of OAuth clients using the Client Credentials flow.

## Scope
1. **Client Registration UI**
   - File: `src/idp-manager-app/src/components/forms/FlowTypeSelect.vue` (New)
   - Action: Create a new Vue component for the flow type select based on the pattern in `ClientTypeSelect.vue`.
   - **Snippet:**
     ```vue
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
       { value: 'authorization_code', label: 'Authorization Code with PKCE' },
       { value: 'client_credentials', label: 'Client Credentials' },
     ]
     </script>
     ```
   - File: `src/idp-manager-app/src/components/clients/ClientEdit.vue`
   - Action: Import and use the new `FlowTypeSelect` component defaulting to Authorization Code.
   - **Snippet:**
     ```vue
     <div class="mb-4">
       <label for="flow-type" class="form-label text-light">OAuth Flow</label>
       <FlowTypeSelect id="flow-type" v-model="model.flowType" class="bg-dark text-light border-secondary" />
     </div>
     ```

2. **API Payload Mapping**
   - File: `src/idp-manager-app/src/stores/clients.js`
   - Action: Update the `emptyClient` getter to include the new `flowType` property so it gets sent in the payload.
   - **Snippet:**
     ```javascript
     getters: {
       emptyClient: () => ({
         // ... existing properties
         flowType: 'authorization_code', // Default flow
       }),
     }
     ```

3. **Validation and Contextual UI**
   - File: `src/idp-manager-app/src/components/clients/ClientEdit.vue`
   - Action: Hide non-applicable settings (e.g., Redirect URIs) when Client Credentials is the selected flow using `v-if`.
   - **Snippet:**
     ```vue
     <!-- Hide Redirect URIs for Client Credentials -->
     <div class="mb-4" v-if="model.flowType !== 'client_credentials'">
       <label for="redirect-uri" class="form-label text-light">Redirect URIs</label>
       <MultipleUriAdd v-model="model.redirectUris" id="redirect-uri" />
     </div>
     ```
