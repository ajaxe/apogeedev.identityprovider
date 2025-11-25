<template>
  <div class="card bg-dark border-secondary border-opacity-50 mb-4">
    <div class="card-body p-2 d-flex flex-wrap align-items-center gap-3">
      <span class="text-white-50 small text-uppercase fw-bold ms-2">Filter By:</span>

      <div class="btn-group" role="group">
        <input
          type="radio"
          class="btn-check"
          name="filter"
          id="f-all"
          autocomplete="off"
          value="All"
          v-model="activeFilter"
        />
        <label class="btn btn-outline-secondary btn-sm" for="f-all">All</label>

        <input
          type="radio"
          class="btn-check"
          name="filter"
          id="f-local"
          autocomplete="off"
          value="Local"
          v-model="activeFilter"
        />
        <label class="btn btn-outline-secondary btn-sm" for="f-local">Local Only</label>

        <input
          type="radio"
          class="btn-check"
          name="filter"
          id="f-conf"
          autocomplete="off"
          value="Confidential"
          v-model="activeFilter"
        />
        <label class="btn btn-outline-secondary btn-sm" for="f-conf">Confidential</label>

        <input
          type="radio"
          class="btn-check"
          name="filter"
          id="f-pub"
          autocomplete="off"
          value="Public"
          v-model="activeFilter"
        />
        <label class="btn btn-outline-secondary btn-sm" for="f-pub">Public</label>
      </div>

      <div class="ms-auto flex-grow-1" :style="customStyle">
        <div class="input-group input-group-sm bg-secondary bg-opacity-10">
          <span class="input-group-text bg-secondary border-0 text-light">
            <i class="bi bi-search"></i>
          </span>
          <input
            name="q"
            type="search"
            class="form-control bg-secondary bg-opacity-10 border-0 text-light placeholder-light"
            placeholder="Search client name or ID..."
            v-model="searchQuery"
          />
        </div>
      </div>
    </div>
  </div>
</template>
<script setup>
import { computed } from 'vue'
import { useClientStore } from '@/stores/clients'
import { storeToRefs } from 'pinia'
import { useViewport } from '@/composables/useViewport'

const { activeFilter, searchQuery } = storeToRefs(useClientStore())
const { breakpoints } = useViewport()
console.log({ breakpoints })
const customStyle = computed(() => ({
  maxWidth: breakpoints.value.sm ? '300px' : '',
}))
</script>
