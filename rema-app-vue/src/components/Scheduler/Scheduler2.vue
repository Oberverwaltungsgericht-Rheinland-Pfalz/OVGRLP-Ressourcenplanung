<template>
  <v-layout column>
    <v-row class="text-center">
      <v-col cols="2">
        <v-text-field type="text" placeholder="Namensfilter" v-model="nameFilter"/>
        <span v-if="hideEmptyRessources" @click="hideEmptyRessources = false"><v-icon>blur_circular</v-icon></span>
        <span v-else @click="hideEmptyRessources = true"><v-icon>blur_linear</v-icon></span>
      </v-col>
      <v-col cols="6">
        <v-btn @click="yesterday" fab text small><v-icon>arrow_back_ios</v-icon></v-btn>
        <v-menu ref="todayMenu" :close-on-content-click="true" transition="scale-transition" offset-y max-width="290px" min-width="290px"
          v-model="daypickerOpen">
          <template v-slot:activator="{ on }">
            <v-btn outlined v-on="on">{{today | toLocalDate}}</v-btn>
          </template>
          <v-date-picker v-model="pickedDate" locale="de" :first-day-of-week="1" no-title @input="daypickerOpen = false">
            <v-btn text color="primary" @click="daypickerOpen = false" block>Abbrechen</v-btn>
          </v-date-picker>
        </v-menu>

        <v-btn @click="tomorrow" fab text small><v-icon>arrow_forward_ios</v-icon></v-btn></v-col>
      <v-col cols="2">
        <span v-if="hideLateEarly" @click="hideLateEarly = false"><v-icon>visibility_off</v-icon></span>
        <span v-else @click="hideLateEarly = true"><v-icon>visibility</v-icon></span>
      </v-col>
    </v-row>
    <v-row>&ensp;</v-row>

    <table class="fixed" CELLSPACING=0>
      <col v-for="(v, idx0) in 25" :key="'cnt224'+idx0" width="40px" />
      <tr>
        <th>Name</th>
        <th v-for="(v, idx0) in 24" :key="'thhours'+idx0" v-show="!(hideLateEarly && (idx0 < startHour || idx0 > endHour))">{{(v-1) | 2digits}}</th>
      </tr>
      <tr v-for="(r, idx1) in ressources" :key="'res'+idx1">
          <td class="right-space first-cell">{{r.Name}}</td>
          <td v-show="!(hideLateEarly && (idx2 < startHour || idx2 > endHour))" v-for="(h2, idx2) in r.hours" :key="idx1+'row'+idx2" :class="{'s': h2}"></td>
      </tr>
    </table>
    {{ressources}}
  </v-layout>
</template>

<script lang="ts">
import { Component, Prop, Vue } from 'vue-property-decorator'
import { Allocation, Gadget, Supplier, Ressource } from '../../models'
import { SelectableGroup } from '../../models/interfaces'
import { deleteAllocation } from '../../services/AllocationApiService'
import moment from 'moment'

@Component
export default class Scheduler extends Vue {
  private displayView: boolean = false
  private displayId: number=0
  private selectedGroup: SelectableGroup | null = null
  private search: string = ''
  private hideOld: boolean = true
  private hideLateEarly: boolean = false
  private selectedOpen: number = -1
  private hours: number = 24
  private startHour: number = 8
  private endHour: number = 17
  private nameFilter: string = ''
  private hideEmptyRessources: boolean = false

  private daypickerOpen: boolean = false
  private today: string = moment().format('YYYY-MM-DD')
  private get pickedDate (): string {
    return this.today
  }
  private set pickedDate (v: string) {
    this.today = moment(v).format('YYYY-MM-DD')
  }
  private tomorrow () {
    this.today = moment(this.today).add(1, 'd').format('YYYY-MM-DD')
  }
  private yesterday () {
    this.today = moment(this.today).add(-1, 'd').format('YYYY-MM-DD')
  }
  private get ressources ():Array<object> {
    let rArray = []
    let start = moment(this.today).valueOf()
    let end = moment(this.today).add(1, 'day').valueOf()
    // [{id, name, done24:[]}]
    const allocations = Allocation.query().withAll()
      // .where((al: any) => (!this.hideNotMine && (al.Status === 1 || al.Status === 3)) || al.CreatedById === myId || al.ReferencePersonId === myId)
      .where((al: any) => {
        let to = moment(al.To).valueOf()
        let from = moment(al.From).valueOf()
        return ((to < end) && to > start) || ((from > start) && from < end) || ((from < start) && to > end)
      })
      // .where('To', (value: any) => (Date.parse(value) < this.todayNum + 36e5 * 24 * 1))
      // .where('From', (value: any) => (Date.parse(value) > this.todayNum - 36e5 * 24 * 1))
      .orderBy('From')
      .get()

    // aufteilen in ressourcen gruppen
    let ressourcesAll = allocations.map((e:any) => e.Ressource)
    let ressources = [...new Set(ressourcesAll)]
    ressources.sort()

    for (let res of ressources) {
      let newObj = { id: res.Id, Name: res.Name, hours: new Array(24) }

      let allocs = allocations.filter((e: any) => e.Ressource.Id === res.Id)
      for (let allo of allocs) {
        // @ts-ignore
        if (allo.IsAllDay) {
          newObj.hours.fill(1)
          break
        }

        let idx = 0
        for (let h of newObj.hours) {
          let hourString = idx < 10 ? '0' + idx : idx
          // @ts-ignore
          let isSet = moment(`${this.today}T${hourString}:00:00`).isBetween(allo.From, allo.To, undefined, '[)')
          if (isSet) newObj.hours[idx] = true
          idx++
        }
      }
      rArray.push(newObj)
    }
    return rArray
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
</style>
