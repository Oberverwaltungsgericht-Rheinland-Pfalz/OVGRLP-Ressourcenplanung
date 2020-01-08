<template>
  <v-container fluid>
    <v-slide-y-transition mode="out-in">
      <v-layout column>
        <h1>Counter</h1>
        <p>With Vuex</p>
        <multi-dates />
      </v-layout>
    </v-slide-y-transition>
  </v-container>
</template>

<script lang="ts">
import { Component, Vue } from 'vue-property-decorator'
import MultiDates from '@/components/Counter.vue'

@Component({
  components: { MultiDates }
})
export default class CounterView extends Vue {}
</script>
