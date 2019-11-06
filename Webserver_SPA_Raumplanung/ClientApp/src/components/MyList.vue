<template>
<v-layout column>
 <v-data-table v-if="hasItems"
    :headers="headers"
    :items="Requests"
    :search="search"
    sort-by="calories"
    class="elevation-1"
  >
    <template v-slot:top>
      <v-toolbar flat color="white">
        <v-toolbar-title>Wartende Anfragen</v-toolbar-title>
        <v-divider class="mx-4" inset vertical></v-divider>
        <v-spacer></v-spacer>
        <v-text-field v-model="search" append-icon="search" label="Filter" single-line hide-details/>
      </v-toolbar>
    </template>
    <template v-slot:item.DateTime="{ item }">{{item.DateTime | toLocal}}</template>
    <template v-slot:item.From="{ item }">{{item.From | toLocal}}</template>
    <template v-slot:item.action="{ item }">
      <v-icon @click="deleteItem(item)">delete</v-icon>
    </template>
  </v-data-table>
  <h3 v-else>Es liegen keine Terminanfragen von Ihnen vor</h3>
  </v-layout>
</template>

<script lang="ts">
import dayjs from 'dayjs'
import { Component, Prop, Vue } from 'vue-property-decorator'
import { Names as Fnn } from '../store/Acknowledges/types'
import AllocationRequest from '../models/AllocationRequest'
import Allocations, { AllocationModel } from '../models/AllocationModel'
import AllocationPurposes, { AllocationPurposeModel } from '../models/AllocationpurposeModel'
const namespace = 'acknowledges'

@Component
export default class AcknowledgeList extends Vue {

  private search: string = ''
  private headers: object[] = [
      { text: 'Bearbeiten', value: 'action', sortable: false },
      { text: 'Bezeichnung', value: 'Title' },
      { text: 'Status' , value: 'Status' },
      { text: 'Raum', value: 'Ressource' },
      { text: 'Ab', value: 'From' },
      { text: 'Zuletzt geändert', value: 'DateTime' }
  ]
  public get hasItems () {
    const allocations = Allocations.query().withAll().get()
    return allocations.length
  }
  public get Requests (): VisibleAllocation[] {
    const allocations = Allocations.query().withAll().get()
    if (!allocations.length) return []
    console.dir(allocations)
    return allocations.map((v: any) => ({
      Id: v.Id,
      Title: (v.Purpose || {}).Title,
      PurposeId: v.Purpose_id,
      Status: this.$options.filters.status2string(v.Status),
      Ressource: (v.Ressource || {}).Name,
      From: v.From,
      DateTime: v.LastModified}))
  }

  private async deleteItem (item: VisibleAllocation) {
    const confirmation = await this.$dialog.confirm({
      text: `Möchten sie diese Buchung ${item.Title} wirklich löschen?`,
      title: 'Löschen bestätigen',
      persistent: true,
      actions: [{
        text: 'Nein', color: 'blue', key: false
      }, {
        text: 'Löschen', color: 'red', key: true
      }]
    })

    if (confirmation !== true) return

    const isLastAllocation = this.isLastAllocation(item.PurposeId)
    // @ts-ignore
    const responseDeleteAllocation = await Allocations.$delete({ params: { id: item.Id } })
    if (isLastAllocation) {
      // @ts-ignore
      const responseDeletePurpose = await AllocationPurposes.$delete({ params: { id: item.PurposeId } })
    }
  }
  private isLastAllocation (purposeID: number): boolean {
    const purposes = this.Requests.filter((v: VisibleAllocation) => v.PurposeId === purposeID)
    if (purposes.length <= 1) return true
    return false
  }
}

interface VisibleAllocation {
  Id: number,
  Title: string,
  Status: string,
  Ressource: string,
  From: string,
  PurposeId: number,
  DateTime: string
}
</script>

<style scoped lang="stylus">
.appointment-open-list:nth-of-type(2n)
  background-color lightgrey

</style>

