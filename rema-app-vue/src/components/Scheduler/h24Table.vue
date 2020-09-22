<template>
  <table class="fixed" CELLSPACING=0>
    <col v-for="(v, idx0) in (hours+1)" :key="'cnt224'+idx0" width="40px" />
    <tr>
      <th v-if="Day">{{Day | onlyDay}}</th>
      <th v-else>Name</th>

      <th v-for="(v, idx0) in hours" :key="'thhours'+idx0" v-show="!(hideLateEarly && (idx0 < startHour || idx0 > endHour))">{{(v-1) | 2digits}}</th>
    </tr>
    <tr v-for="(r, idx1) in ressources" :key="'res'+idx1">
      <td class="right-space first-cell">{{r.Name}}</td>
      <td
        v-for="(h2, idx2) in r.Hours"
        :key="idx1+'row'+idx2"
        v-show="!(hideLateEarly && (idx2 < startHour || idx2 > endHour))"
        @click="createAllocation(h2,r.Id, idx2)"
        :class="{'s': h2, 'f': !h2}">
      </td>
    </tr>
  </table>
</template>

<script lang="ts">
import { Component, Prop, Vue } from 'vue-property-decorator'
import { Allocation, Ressource } from '../../models'
import { ScheduledRessource, InitAllocation } from '../../models/interfaces'
import { deleteAllocation } from '../../services/AllocationApiService'
import moment from 'moment'

@Component
export default class h24Table extends Vue {
  @Prop(Array) private ressources!: ScheduledRessource[]
  @Prop(Boolean) private hideLateEarly!: boolean
  @Prop(String) private Day!: string
  private hours: number = 24
  private startHour: number = 8
  private endHour: number = 17

  private createAllocation (blocked: boolean, id: number, hourNumber: number) {
    if (blocked) return

    let isPast = !moment().isBefore(this.Day + ' ' + hourNumber)
    if (isPast) return

    let request: InitAllocation = {
      // @ts-ignore
      From: this.$options.filters['2digits'](hourNumber) + ':00',
      RessourceId: id,
      Day: this.Day
    }
    this.$root.$emit('open-allocation-form', request)
  }
}
</script>

<style lang="stylus" scoped>
.blocked
  background-color lightgrey
td.s
  background-color lightgrey
td.f
  background-color white
.new-allocations-in-table td.f:hover
  background-color lightgreen
  background-image url(/plus.svg)
  background-repeat no-repeat
  background-position center
td
  border-top 1px solid black
td.right-space
  padding-right 1em
td
  border-right 1px solid #b2b2b2
.first-cell
  padding-top .25em
  padding-bottom .25em
  padding-left: .25em
  padding-right 0.1em !important
table.fixed
  border-bottom 1px solid black
</style>
