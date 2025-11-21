<script setup lang="ts">
import AppBrand from '@/components/AppBrand.vue'
import UserLogin from '@/components/UserLogin.vue'
</script>

<template>
  <div class="container col-xl-10 col-xxl-8 px-4 py-5">
    <div class="bg-dark text-secondary px-4 py-5 text-center">
      <div class="py-5 border rounded-3 bg-body-tertiary">
        <div class="d-flex justify-content-center mb-5">
          <AppBrand />
          <div class="clearfix"></div>
        </div>
        <h1 class="display-5 fw-bold text-white">Configuration Manager</h1>
        <div class="col-lg-6 mx-auto">
          <p class="fs-5 mb-4">Log in to manage configuration.</p>
          <div class="d-grid gap-2 d-sm-flex justify-content-sm-center">
            <UserLogin />
          </div>
        </div>
      </div>
    </div>
  </div>
</template>
