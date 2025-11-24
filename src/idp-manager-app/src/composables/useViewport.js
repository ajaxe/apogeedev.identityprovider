// useViewport.js
import { computed, ref, onMounted, onUnmounted } from 'vue'

export function useViewport() {
  const width = ref(window.innerWidth)
  const height = ref(window.innerHeight)

  let timeoutId

  const handleResize = () => {
    clearTimeout(timeoutId)
    // Wait 100ms after the user stops resizing to update
    timeoutId = setTimeout(() => {
      width.value = window.innerWidth
    }, 100)
  }

  const breakpoints = computed(() => {
    const w = width.value
    return {
      // 'xs' is always true because the screen is always >= 0
      xs: true,
      sm: w >= 576,
      md: w >= 768,
      lg: w >= 992,
      xl: w >= 1200,
      xxl: w >= 1400,

      // Helper: Identify the strict current range name
      current:
        w >= 1400
          ? 'xxl'
          : w >= 1200
            ? 'xl'
            : w >= 992
              ? 'lg'
              : w >= 768
                ? 'md'
                : w >= 576
                  ? 'sm'
                  : 'xs',
    }
  })

  onMounted(() => window.addEventListener('resize', handleResize))
  onUnmounted(() => window.removeEventListener('resize', handleResize))

  return { width, height, breakpoints }
}
