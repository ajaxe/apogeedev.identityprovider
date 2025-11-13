<script setup>
import { RouterView } from 'vue-router'
import AppBrand from '@/components/AppBrand.vue'
</script>

<template>
  <header class="p-3 text-bg-dark shadow-lg">
    <div class="container">
      <div
        class="d-flex flex-wrap align-items-center justify-content-center justify-content-lg-start"
      >
        <a href="/">
          <AppBrand />
        </a>
      </div>
    </div>
  </header>
  <div class="container mt-5">
    <RouterView />
  </div>
</template>
