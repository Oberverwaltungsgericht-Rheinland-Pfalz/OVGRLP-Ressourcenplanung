<template>
  <label>
    {{label}}:
    <input
      v-if="isWholeDay"
      :value="dateFrom"
      @input="dateFrom = $event.target.value"
      type="date"
      id="meeting-from"
      name="meeting-time"
      :min="today"
    />
  </label>
</template>

<script lang="ts">
import { Component, Prop, Vue, Watch } from 'vue-property-decorator'
import { Boolean } from '@vuex-orm/core'
import dayjs from 'dayjs'

@Component
export default class DayPicker extends Vue {
  @Prop(Boolean) private readonly fullDay!: boolean;
  @Prop(Date) private readonly startDate!: Date;

  private withTime: boolean = this.fullDay;
  private dateIntern: Date = new Date();
  // dateFrom
  private get dateFrom (): string {
    return dayjs(this.dateIntern).format('YYYY-MM-DDThh:mm')
  }
  private set dateFrom (val: string) {
    this.dateIntern = new Date(val)
  }
}
</script>
