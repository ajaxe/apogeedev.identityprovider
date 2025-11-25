<template>
  <div class="row g-3 mb-4">
    <div class="col d-lg-none">
      <div class="d-inline-block chip">
        Total Clients <span class="badge text-bg-secondary">{{ stats.total }}</span>
      </div>
      <div class="d-inline-block chip">
        Confidential <span class="badge bg-warning text-dark">{{ stats.confidential }}</span>
      </div>
      <div class="d-inline-block chip">
        Public <span class="badge bg-info text-dark">{{ stats.public }}</span>
      </div>
      <div class="d-inline-block chip">
        Web <span class="badge bg-light text-dark">{{ stats.web }}</span>
      </div>
      <div class="d-inline-block chip">
        Native <span class="badge bg-light text-dark">{{ stats.native }}</span>
      </div>
    </div>
    <div class="col-md-3 col-sm-6 d-none d-lg-block">
      <div class="card bg-secondary bg-opacity-25 border-0 text-white h-100">
        <div class="card-body d-flex flex-column justify-content-center align-items-center">
          <h1 class="display-5 fw-bold mb-0">{{ stats.total }}</h1>
          <span class="text-white-50 text-uppercase small fw-bold">Total Clients</span>
        </div>
      </div>
    </div>

    <div class="col-md-3 col-sm-6 d-none d-lg-block">
      <div
        class="card bg-secondary bg-opacity-10 border-secondary border-opacity-25 text-white h-100"
      >
        <div class="card-body">
          <h6 class="card-title text-info mb-3">Security Profile</h6>
          <div class="d-flex justify-content-between mb-2">
            <span>Confidential <i class="bi bi-shield-lock-fill text-warning"></i></span>
            <span class="badge bg-warning text-dark">{{ stats.confidential }}</span>
          </div>
          <div class="d-flex justify-content-between">
            <span>Public <i class="bi bi-globe2 text-info-emphasis"></i></span>
            <span class="badge bg-light text-dark">{{ stats.public }}</span>
          </div>
        </div>
      </div>
    </div>

    <div class="col-md-3 col-sm-6 d-none d-lg-block">
      <div
        class="card bg-secondary bg-opacity-10 border-secondary border-opacity-25 text-white h-100"
      >
        <div class="card-body">
          <h6 class="card-title text-success mb-3">Platform</h6>
          <div class="d-flex justify-content-between mb-2">
            <span>Web App</span>
            <span class="fw-bold">{{ stats.web }}</span>
          </div>
          <div class="d-flex justify-content-between">
            <span>Native</span>
            <span class="fw-bold">{{ stats.native }}</span>
          </div>
        </div>
      </div>
    </div>

    <div class="col-md-3 col-sm-6 d-none d-lg-block">
      <div
        class="card bg-secondary bg-opacity-10 border-secondary border-opacity-25 text-white h-100"
      >
        <div class="card-body">
          <h6 class="card-title text-warning mb-3">Environment</h6>
          <div class="progress bg-dark mb-2" style="height: 6px">
            <div
              class="progress-bar bg-warning"
              role="progressbar"
              :style="{ width: stats.localPercent + '%' }"
            ></div>
          </div>
          <div class="small text-white-50">
            {{ stats.local }} Local Dev / {{ stats.remote }} Remote
          </div>
        </div>
      </div>
    </div>
  </div>
</template>
<style lang="scss" scoped>
.chip {
  padding: 0.25rem 0.75rem;
  font-size: 0.875rem;
  font-weight: 500;
  color: #fff;
  background-color: rgba(255, 255, 255, 0.1);
  border-radius: 16px;
  margin-top: 2px;
  .badge {
    margin-left: 0.5rem;
  }
}
</style>
<script setup>
import { watch } from 'vue'
const { stats } = defineProps({
  stats: {
    type: Object,
    required: true,
    default: () => ({
      total: 0,
      confidential: 0,
      public: 0,
      web: 0,
      native: 0,
      local: 0,
      remote: 0,
      localPercent: 0,
    }),
  },
})
watch(
  () => stats,
  (newStats) => {
    console.log('Stats updated:', newStats)
  },
)
</script>
