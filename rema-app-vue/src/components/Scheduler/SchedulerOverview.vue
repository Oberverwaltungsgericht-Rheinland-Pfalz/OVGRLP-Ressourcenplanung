<template>
  <v-layout column :class="{'new-allocations-in-table': allocationPossible}">
    <v-row class="text-center">
      <v-col cols="2">
        <v-text-field type="text" placeholder="Namensfilter" v-model="nameFilter"/>
      </v-col>
      <v-col cols="1">
        <v-btn v-if="hideEmptyRessources" @click="hideEmptyRessources = false" title="Freie Ressourcen anzeigen"><v-icon>blur_circular</v-icon></v-btn>
        <v-btn v-else @click="hideEmptyRessources = true" title="Freie Ressourcen ausblenden"><v-icon>blur_linear</v-icon></v-btn>
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
      <v-col cols="3">
        <v-btn v-if="!hideLateEarly" @click="hideLateEarly = true" title="Nur Arbeitszeit"><v-icon>visibility_off</v-icon></v-btn>
        <v-btn v-else @click="hideLateEarly = false" title="Alle Stunden anzeigen"><v-icon>visibility</v-icon></v-btn>
      </v-col>
    </v-row>

    <h24-table :ressources="ressources" :hideLateEarly="hideLateEarly" :Day="today"/>
  </v-layout>
</template>

<script lang="ts">
import { Component, Vue } from 'vue-property-decorator'
import { Allocation, Ressource } from '../../models'
import { ScheduledRessource } from '../../models/interfaces'
import { State } from 'vuex-class'
import h24Table from './h24Table.vue'
import moment from 'moment'

@Component({
  components: { h24Table }
})
export default class SchedulerOverview extends Vue {
  private hideLateEarly: boolean = true
  private selectedOpen: number = -1
  private hours: number = 24
  private nameFilter: string = ''
  private hideEmptyRessources: boolean = false
  @State('isRequestable', { namespace: 'user' })
  private requestsAllowed!: boolean

  private daypickerOpen: boolean = false
  private get allocationPossible () {
    // @ts-ignore
    return moment(moment().add(-1, 'days')).isBefore(this.today) && (this.requestsAllowed || this.permissionToEdit)
  }
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
      let newObj: ScheduledRessource = { Id: res.Id, Name: res.Name, Hours: new Array(24) }

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
          let isSet = moment(`${this.today}T${hourString}:00:00`).isBetween(allo.From, allo.To, undefined, '[)')
          if (isSet) newObj.Hours[idx] = true
          idx++
        }
      }
      rArray.push(newObj)
    }
    if (!this.hideEmptyRessources) this.addEmptyRessources(rArray)
    if (this.nameFilter.length > 0) this.applyNameFilter(rArray)

    return rArray
  }
  public applyNameFilter (ar: ScheduledRessource[]) {
    ar.splice(0, Infinity, ...ar.filter((x: ScheduledRessource) => x.Name.toLowerCase().includes(this.nameFilter.toLowerCase())))
  }
  public addEmptyRessources (ar: ScheduledRessource[]) {
    let allRessources = Ressource.query().get()
    let withoutExisting = allRessources.filter((a: any) => !ar.find((b: any) => a.Id === b.id))
    for (let res of withoutExisting) {
      // @ts-ignore
      let newObj: ScheduledRessource = { Id: res.Id, Name: res.Name, Hours: new Array(24) }
      ar.push(newObj)
    }
    ar.sort((a: ScheduledRessource, b: ScheduledRessource) => Number(a.Name > b.Name) - 1)
  }
}
</script>
