<template>
  <div class="d-flex flex-wrap gap-2 mb-3">
    <template v-for="u in model" :key="u">
      <UriListItem :uri="u" @click="() => remove(u)" />
    </template>
    <UriListItem uri="a" v-if="model?.length <= 0" is-empty />
  </div>

  <div class="input-group mb-3">
    <input
      class="form-control bg-dark text-light border-secondary"
      placeholder="https://example.com/callback"
      type="text"
      v-model="uri"
      ref="input"
      :id="id"
    />
    <button class="btn btn-outline-primary" @click="add">Add</button>
  </div>
</template>
<script setup>
import { ref, useTemplateRef } from 'vue'
import UriListItem from './UriListItem.vue'

const { id } = defineProps({
  id: String,
})

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

const remove = (uri) => {
  model.value = model.value.filter((u) => u !== uri)
}
</script>
