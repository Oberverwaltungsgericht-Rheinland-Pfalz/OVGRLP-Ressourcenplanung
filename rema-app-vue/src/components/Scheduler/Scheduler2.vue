<template>
  <v-layout column>
    <table class="fixed">
      <col width="40px" />
      <col width="40px" />
      <col width="40px" />
      <col width="40px" />
      <col width="40px" />
      <col width="40px" />
      <col width="40px" />
      <col width="40px" />
      <col width="40px" />
      <col width="40px" />
      <col width="40px" />
      <col width="40px" />
      <col width="40px" />
      <col width="40px" />
      <col width="40px" />
      <col width="40px" />
      <col width="40px" />
      <col width="40px" />
      <col width="40px" />
      <col width="40px" />
      <col width="40px" />
      <col width="40px" />
      <col width="40px" />
      <col width="40px" />
      <col width="40px" />
      <tr v-for="r in ressources" :key="'res'+r">
          <td>{{r}}:</td>
          <td v-for="h2 in hours" :key="'lol'+h2">{{h2}}</td>
      </tr>
</table>
  </v-layout>
</template>

<script lang="ts">
import { Component, Prop, Vue } from 'vue-property-decorator'
import { Allocation, Gadget, Supplier, Ressource } from '../../models'
import { SelectableGroup } from '../../models/interfaces'
import { deleteAllocation } from '../../services/AllocationApiService'

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
  private ressources: Array<string> = []

  public mounted () {
    this.buildTasksStream()
  }
  private today: string = moment().format('YYYY-MM-DD')
  public buildTasksStream () {
    let stringStream = ''
    const allocations = Allocation.query().withAll()
      // .where((al: any) => (!this.hideNotMine && (al.Status === 1 || al.Status === 3)) || al.CreatedById === myId || al.ReferencePersonId === myId)
      .where('To', (value: any) => (Date.parse(value) > Date.now()))
      .where('From', (value: any) => (Date.parse(value) > Date.now() + 36e5 * 24 * 7))
      .orderBy('From')
      .get()
    debugger
    // aufteilen in ressourcen gruppen
    let ressourcesAll = allocations.map((e:any) => e.Ressource)
    let ressources = [...new Set(ressourcesAll)]
    ressources.sort()

    for (let res of ressources) {
      this.ressources.push(res.Name)
      stringStream += `${res.Name}\r\n`
      // var r: any = Ressource.find(res.Id as number) || { Name: '' }
      // stringStream += r.Name + '\n'

      var allocs = allocations.filter((e: any) => e.RessourceId === res.Id)
      for (let alloc of allocs) {
        stringStream += `${alloc.Title.replace(':', '>')}   :${alloc.From}, ${alloc.To}\r\n`
      }
    }

    return stringStream
  }

  public get hasItems () {
    const allocations = Allocation.query()
      .get()
    return allocations.length
  }
  public get Requests (): any[] {
    let myId = this.$store.state.user.id

    const allocations = Allocation.query().withAll()
      .where((al: any) => (!this.hideNotMine && (al.Status === 1 || al.Status === 3)) || al.CreatedById === myId || al.ReferencePersonId === myId)
      .where('To', (value: any) => (!this.hideOld || Date.parse(value) > Date.now()))
      .where('GadgetsIds', (values: number[]) => (!this.selectedGroup || values.length > 0))
      .where('GadgetsIds', (values: number[]) => !this.selectedGroup || (this.selectedGroup && this.selectedGroup.gadgetIds.some((e: number) => values.includes(e))))
      .orderBy('From')
      .get()

    if (!allocations.length) return []

    return allocations.map((v: any) => ({
      ...v,
      // @ts-ignore
      Status: this.$options.filters.status2string(v.Status),
      Ressource: (v.Ressource || { Name: '' }).Name
    }))
  }
  public get GroupItems (): object[] {
    var groups = new Map()

    Allocation.query().withAll().get().forEach((e:any) => e.GadgetsIds.forEach((f: number) => {
      let groupId = (Gadget.find(f) as any).SuppliedBy
      let group = Supplier.find(groupId) as any
      if (!groups.has(groupId)) {
        let filterObj: SelectableGroup = { Id: group.Id, Title: (group.Title || ''), gadgetIds: [f] }
        groups.set(groupId, filterObj)
      } else {
        let filterObj = groups.get(groupId)
        filterObj.gadgetIds.push(f)
      }
    }))
    let rawValues = [...groups.values()]
    let returnValues = rawValues.map((e: SelectableGroup) => ({ Id: e.Id, Title: e.Title, gadgetIds: [...new Set(e.gadgetIds)] }))
    return returnValues
  }

  private async deleteItem (item: any) {
    const confirmation = await this.$dialog.confirm({
      text: `Möchten sie diese Buchung ${item.Title} wirklich löschen?`,
      title: 'Löschen bestätigen',
      persistent: true,
      actions: [
        { text: 'Nein', color: 'blue', key: false },
        { text: 'Löschen', color: 'red', key: true }
      ]
    })

    if (confirmation !== true) return

    let success = await deleteAllocation(item.Id)
    if (success) this.$dialog.message.success('Löschung erfolgreich', { position: 'center-left' })
    else this.$dialog.error({ text: 'Löschen fehlgeschlagen', title: 'Fehler' })
  }
}
</script>

<style lang="stylus" scoped>
.blocked
  background-color lightgrey
</style>
