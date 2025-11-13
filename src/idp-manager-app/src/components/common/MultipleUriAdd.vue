<template>
  <template v-for="u in model" :key="u">
    <UriListItem :uri="u" />
  </template>

  <div class="d-flex multiple-uri-input">
    <div class="w-100 pe-2">
      <input class="form-control" v-model="uri" ref="input" />
    </div>
    <div class="flex-shrink-1">
      <button class="btn btn-secondary btn-small" @click="add">Add</button>
    </div>
  </div>
</template>
<script setup>
import { ref, useTemplateRef } from 'vue'
import UriListItem from './UriListItem.vue'
const model = defineModel()
const uri = ref('')

const input = useTemplateRef('input')

const add = () => {
  const v = uri.value

  if (
    !checkValidity({
      input: input.value,
      message: 'Must be a valid URI',
      check: () => URL.canParse(v),
    })
  ) {
    return
  }

  const url = new URL(v)

  if (
    !checkValidity({
      input: input.value,
      message: 'Must be a valid URI',
      check: () => url.protocol === 'https:',
    })
  ) {
    return
  }

  if (model.value.includes(v)) return

  model.value.push(v)
  uri.value = ''
}

const checkValidity = function ({ input, message, check }) {
  const valid = check()
  if (valid) {
    input.setCustomValidity('')
  } else {
    input.setCustomValidity(message)
    input.reportValidity()
  }
  return valid
}
</script>
