<template>
  <table class="fixed" CELLSPACING=0 :class="{'new-allocations-in-table': allocationPossible}">
    <col v-for="(v, idx0) in (hours+1)" :key="'cnt224'+idx0" width="40px" />
    <tr>
      <th v-if="Day">{{Day | onlyDay}}</th>
      <th v-else>Name</th>

      <th v-for="(v, idx0) in hours" :key="'thhours'+idx0" v-show="!(HideLateEarly && (idx0 < startHour || idx0 > endHour))">{{(v-1) | 2digits}}</th>
    </tr>
    <tr v-for="(r, idx1) in ressources" :key="'res'+idx1">
      <td :title="r.Details" class="right-space first-cell">{{r.Name}}
        <v-icon v-if="r.Details" color="blue" class="info-cell">info_icon</v-icon>
      </td>
      <td
        v-for="(h2, idx2) in r.Hours"
        :key="idx1+'row'+idx2"
        v-show="!(HideLateEarly && (idx2 < startHour || idx2 > endHour))"
        @click="createAllocation(h2,r.Id, idx2)"
        :class="{'s': h2, 'f': !h2}">
      </td>
    </tr>
  </table>
</template>

<script lang="ts">
import { Component, Prop, Vue } from 'vue-property-decorator'
import { Allocation, Ressource } from '../../models'
import { ScheduledRessource, InitAllocation, ShowToast } from '../../models/interfaces'
import { deleteAllocation } from '../../services/AllocationApiService'
import moment from 'moment'

@Component
export default class h24Table extends Vue {
  @Prop(String) private Day!: string
  @Prop(String) private NameFilter!: string
  @Prop(Boolean) private HideLateEarly!: boolean
  @Prop(Boolean) private HideEmptyRessources!: boolean

  private hours: number = 24
  private startHour: number = 8
  private endHour: number = 17

  private get allocationPossible () {
    return moment(moment().add(-1, 'days')).isBefore(this.Day) &&
      // @ts-ignore
      (this.$store.state.user.isRequestable || this.permissionToEdit)
  }

  private get ressources ():Array<object> {
    let myId = this.$store.state.user.id
    let rArray = []
    let start = moment(this.Day).valueOf()
    let end = moment(this.Day).add(1, 'day').valueOf()
    // [{id, name, done24:[]}]
    const allocations = Allocation.query().withAll()
      .where((al: any) => (al.Status === 1 || al.Status === 3) || al.CreatedById === myId || al.ReferencePersonId === myId)
      .where((al: any) => {
        let to = moment(al.To).valueOf()
        let from = moment(al.From).valueOf()
        return ((to < end) && to > start) || ((from > start) && from < end) || ((from < start) && to > end)
      })
      .orderBy('From')
      .get()

    // aufteilen in ressourcen gruppen
    let ressourcesAll = allocations.map((e:any) => e.Ressource)
    let ressources = [...new Set(ressourcesAll)]
    ressources.sort()

    for (let res of ressources) {
      let newObj: ScheduledRessource = { Id: res.Id, Name: res.Name, Hours: new Array(24), Details: res.SpecialsDescription }

      let allocs = allocations.filter((e: any) => e.Ressource.Id === res.Id)
      for (let allo of allocs) {
        // @ts-ignore
        if (allo.IsAllDay) {
          newObj.Hours.fill(true)
          break
        }

        let idx = 0
        for (let h of newObj.Hours) {
          let hourString = idx < 10 ? '0' + idx : idx
          // @ts-ignore
          let isSet = moment(`${this.Day}T${hourString}:00:00`).isBetween(allo.From, allo.To, undefined, '[)')
          if (isSet) newObj.Hours[idx] = true
          idx++
        }
      }
      rArray.push(newObj)
    }
    if (!this.HideEmptyRessources) this.addEmptyRessources(rArray)
    if (this.NameFilter && this.NameFilter.length) this.applyNameFilter(rArray)
    rArray.sort((a: ScheduledRessource, b: ScheduledRessource) => Number(a.Name > b.Name) - 1)

    return rArray
  }
  public applyNameFilter (ar: ScheduledRessource[]) {
    ar.splice(0, Infinity, ...ar.filter((x: ScheduledRessource) => x.Name.toLowerCase().includes(this.NameFilter.toLowerCase())))
  }
  public addEmptyRessources (ar: ScheduledRessource[]) {
    let allRessources = Ressource.query().get()
    let withoutExisting = allRessources.filter((a: any) => !ar.find((b: any) => a.Id === b.Id))
    for (let res of withoutExisting) {
      // @ts-ignore
      let newObj: ScheduledRessource = { Id: res.Id, Name: res.Name, Hours: new Array(24), Details: res.SpecialsDescription }
      ar.push(newObj)
    }
  }

  private createAllocation (blocked: boolean, id: number, hourNumber: number) {
    if (blocked || !this.allocationPossible) return

    // @ts-ignore
    let slotString = this.Day + ' ' + this.$options.filters['2digits'](hourNumber) + ':00:00'
    let isPast = !moment().isBefore(slotString)
    if (isPast) {
      this.$root.$emit('notify-user', { text: 'Zeitpunkt liegt in der Vergangenheit', color: 'warning', center: false } as ShowToast)
      return
    }

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
.info-cell
  cursor help
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
