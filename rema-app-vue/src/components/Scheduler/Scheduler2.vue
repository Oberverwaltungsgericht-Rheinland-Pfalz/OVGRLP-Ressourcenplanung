<template>
  <v-layout column>
    <h3><strong @click="yesterday"> &lt; </strong> &emsp; {{today | toLocalDate}} &emsp; <strong @click="tomorrow">></strong></h3>

    <table class="fixed" CELLSPACING=0>
      <col width="40px" /><col width="40px" />
      <col width="40px" /><col width="40px" />
      <col width="40px" /><col width="40px" />
      <col width="40px" /><col width="40px" />
      <col width="40px" /><col width="40px" />
      <col width="40px" /><col width="40px" />
      <col width="40px" /><col width="40px" />
      <col width="40px" /><col width="40px" />
      <col width="40px" /><col width="40px" />
      <col width="40px" /><col width="40px" />
      <col width="40px" /><col width="40px" />
      <col width="40px" /><col width="40px" />
      <col width="40px" />
      <tr>
        <th></th>
        <th>00</th><th>01</th><th>02</th><th>03</th>
        <th>04</th><th>05</th><th>06</th><th>07</th>
        <th>08</th><th>09</th><th>10</th><th>11</th>
        <th>12</th><th>13</th><th>14</th><th>15</th>
        <th>16</th><th>17</th><th>18</th><th>19</th>
        <th>20</th><th>21</th><th>22</th><th>23</th>
      </tr>
      <tr v-for="(r, idx1) in ressources" :key="'res'+idx1">
          <td class="right-space">{{r.Name}}</td>
          <td v-for="(h2, idx2) in r.hours" :key="idx1+'row'+idx2" :class="{'s': h2}"></td>
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
  private hideNotMine: boolean = false
  private selectedOpen: number = -1
  private hours: number = 24

  private today: string = moment().format('YYYY-MM-DD')
  private todayNum: number = Date.now()
  private tomorrow () {
    this.today = moment(this.today).add(1, 'd').format('YYYY-MM-DD')
    this.todayNum = moment(this.today).valueOf()
  }
  private yesterday () {
    this.today = moment(this.today).add(-1, 'd').format('YYYY-MM-DD')
    this.todayNum = moment(this.today).valueOf()
  }
  private get ressources ():Array<object> {
    let rArray = []
    let start = this.todayNum
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
</style>
