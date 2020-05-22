<template>
  <div>
    <v-row v-if="hasCollisions">
      <v-col cols="12">
      <h2>Mögliche Kollisionen</h2>
      </v-col>
    </v-row>
    <v-row v-for="(i, idx) in possibleCollisions" v-bind:key="idx + 'cols'">
      <v-col cols="4">{{ i.From | toLocal }} - {{ i.To | toLocal }}</v-col>
      <v-col cols="4">{{ i.Title }}</v-col>
      <v-col cols="4">{{ i.Status | status2string }}</v-col>
    </v-row>
  </div>
</template>

<script lang="ts">
import { Allocation } from '../models'
import { Component, Prop, Vue, Mixins } from 'vue-property-decorator'
import { ShortAllocationView } from '../models/interfaces'

@Component
export default class CollisionDetection extends Vue {
  @Prop(Object) private viewAllocation!: ShortAllocationView

  public get possibleCollisions (): Allocation[] {
    const start = Date.parse(this.viewAllocation.From)
    const end = Date.parse(this.viewAllocation.To)
    const id = this.viewAllocation.Id

    const rValues = Allocation.query()
      .withAll()
      .where('Status', (s: number) => s !== 2)
      .where('RessourceId', this.viewAllocation.RessourceId)
      .where((a: any) => {
        if (a.Id === id) return false
        let rVal = false
        if (this.viewAllocation.dates == null) {
          let aTo = Date.parse(a.To)
          let aFrom = Date.parse(a.From)
          rVal = (aTo >= start && aTo <= end) ||
            (aFrom >= start && aFrom <= end) ||
            (aFrom <= start && aTo >= end)
        } else {
          // uhrzeiten von to u from, datums von dates
          rVal = this.viewAllocation.dates.filter(b => {
            let aTo = Date.parse(a.To)
            let aFrom = Date.parse(a.From)
            rVal = (aTo >= start && aTo <= end) ||
            (aFrom >= start && aFrom <= end) ||
            (aFrom <= start && aTo >= end)
          }).length > 0
        }

        return rVal
      })
      .get()
    return rValues
  }
  public get hasCollisions () {
    return this.possibleCollisions.length > 0
  }
}
</script>
