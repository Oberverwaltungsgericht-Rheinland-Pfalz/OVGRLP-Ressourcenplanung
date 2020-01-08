<template>
  <v-container fluid>
    <v-slide-y-transition mode="out-in">
      <v-layout column>
        <input type="number" v-model="year" />
        <v-spacer />
        <YearCalendar
          lang="de"
          v-model="year"
          :activeDates.sync="activeDates"
          @toggleDate="toggleDate"
          :showYearSelector="false"
        ></YearCalendar>
      </v-layout>
    </v-slide-y-transition>
  </v-container>
</template>

<script lang="ts">
import { Component, Prop, Vue } from 'vue-property-decorator'
// @ts-ignore
import YearCalendar from 'vue-material-year-calendar'
const namespace: string = 'MultiDates'

@Component({
  components: { YearCalendar }
})
export default class RessourceMaintenance extends Vue {
  private year: number = new Date().getFullYear();
  private activeDates: any = [{ date: '2019-02-13' }, { date: '2019-02-16' }];
  private async toggleDate (d: any) {
    const res = await this.$dialog.prompt({
      text: 'Your name',
      title: 'Please input your name',
      persistent: true
    })
  }
}
</script>
