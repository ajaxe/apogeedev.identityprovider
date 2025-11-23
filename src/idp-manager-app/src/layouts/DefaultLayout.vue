<script setup lang="ts">
import AppBrand from '@/components/AppBrand.vue'
import AppNotificationContainer from '@/components/notifications/AppNotificationContainer.vue'
import UserAvatar from '@/components/UserAvatar.vue'
</script>

<template>
  <header class="p-3 text-bg-dark shadow-lg">
    <div class="container">
      <div class="d-flex flex-wrap justify-content-between">
        <a href="/">
          <AppBrand />
        </a>
        <UserAvatar />
      </div>
    </div>
  </header>
  <div class="container mt-5">
    <slot />
  </div>
  <AppNotificationContainer />
</template>
