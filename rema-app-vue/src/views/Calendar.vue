<template>
  <div>
    <calendar-component />
  </div>
</template>

<script lang="ts">
import { Component, Vue } from 'vue-property-decorator'
import CalendarComponent from '@/components/Calendar.vue'

@Component({
  components: { CalendarComponent }
})
export default class Calendar extends Vue {}
</script>
