<template>
  <div>
    <v-row v-if="hasCollisions">
      <v-col cols="12">
      <h2>Mögliche Kollisionen <v-icon color="orange">warning</v-icon></h2>
      </v-col>
    </v-row>
    <v-row v-for="(i, idx) in possibleCollisions" v-bind:key="idx + 'cols'">
      <v-col cols="5">{{ i.From | toLocal }} - {{ i.To | toLocal }}</v-col>
      <v-col cols="3">{{ i.Title }}</v-col>
      <v-col cols="2">{{ getRessourceNames(i.Ressources) }}</v-col>
      <v-col cols="2">{{ i.Status | status2string }}</v-col>
    </v-row>
  </div>
</template>

<script lang="ts">
import { Allocation, Ressource } from '../models'
import { Component, Prop, Vue, Mixins, Watch } from 'vue-property-decorator'
import { ShortAllocationView } from '../models/interfaces'

@Component
export default class CollisionDetection extends Vue {
  @Prop(Object) private viewAllocation!: ShortAllocationView
  private possibleCollisions: Allocation[] = [] as Array<Allocation>

  @Watch('viewAllocation', { deep: true })
  onviewAllocationChanged (viewAllocation: ShortAllocationView) {
    let start = Date.parse(viewAllocation.From)
    let end = Date.parse(viewAllocation.To)
    const id = viewAllocation.Id
    const rValues: Array<Allocation> = Allocation.query()
      .withAll()
      .where('Status', (s: number) => s !== 2)
      .where('RessourceIds', (a : Array<number>) =>
        viewAllocation.RessourceIds.some((rId: number) => a.includes(rId))
      )
      .where((a: Allocation) => {
        if (a.Id === id) return false

        // console.log((this.viewAllocation.dates == null))
        let rVal = false
        const aTo = Date.parse(a.To)
        const aFrom = Date.parse(a.From)
        if (viewAllocation.dates == null) {
          let hitBegin = (aTo >= start && aTo <= end)
          let hitEnd = (aFrom >= start && aFrom <= end)
          let hitBetween = (aFrom <= start && aTo >= end)
          rVal = hitBegin || hitEnd || hitBetween
        } else {
          // uhrzeiten von to u from, datums von dates
          rVal = viewAllocation.dates.filter(b => {
            start = Date.parse(b + viewAllocation.From)
            end = Date.parse(b + viewAllocation.To)
            let hitBegin = (aTo >= start && aTo <= end)
            let hitEnd = (aFrom >= start && aFrom <= end)
            let hitBetween = (aFrom <= start && aTo >= end)
            return hitBegin || hitEnd || hitBetween
          }).length > 0
        }
        return rVal
      }).get()

    this.possibleCollisions.splice(0, 9e9, ...rValues)
  }
  public getRessourceNames (r: Ressource[]) : string {
    return r.map((r: Ressource) => r.Name).join(', ')
  }
  public get hasCollisions (): boolean {
    return this.possibleCollisions.length > 0
  }
  private mounted () {
    this.onviewAllocationChanged(this.viewAllocation)
  }
  @Watch('hasCollisions')
  private watchHasCollisions (newValue: boolean) {
    this.$emit('has-collisions', newValue)
  }
}
</script>
