<template>
  <div>
    <scheduler-component />
  </div>
</template>

<script lang="ts">
import { Component, Vue } from 'vue-property-decorator'
import SchedulerComponent from '../components/Scheduler/Scheduler.vue'

@Component({
  components: { SchedulerComponent }
})
export default class Schedules extends Vue {}
</script>
