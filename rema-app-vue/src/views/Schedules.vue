<template>
  <div>
    <scheduler-overview />
  </div>
</template>

<script lang="ts">
import { Component, Vue } from 'vue-property-decorator'
import SchedulerOverview from '../components/Scheduler/SchedulerOverview.vue'

@Component({
  components: { SchedulerOverview }
})
export default class Schedules extends Vue {}
</script>
