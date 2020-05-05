<template>
  <v-layout column>
    <v-data-table
      v-if="hasItems"
      :headers="headers"
      :items="Requests"
      :search="search"
      sort-by="calories"
      class="elevation-1"
    >
      <template v-slot:top>
        <v-toolbar flat color="white">
          <v-toolbar-title>Von Ihnen gestellte Anfragen</v-toolbar-title>
          <v-divider class="mx-4" inset vertical></v-divider>
          <label>
            <input type="checkbox" v-model="hideOld" /> vergangene Termine ausblenden
          </label>
          <v-spacer></v-spacer>
          <v-text-field
            v-model="search"
            append-icon="search"
            label="Filter"
            single-line
            hide-details
          />
        </v-toolbar>
      </template>
      <template v-slot:item.LastModified="{ item }">{{item.LastModified | toLocal}}</template>
      <template v-slot:item.From="{ item }">{{item.From | toLocal}}</template>
      <template v-slot:item.To="{ item }">{{item.To | toLocal}}</template>
      <template v-slot:item.action="{ item }">
        <v-icon @click="deleteItem(item)">delete</v-icon>
      </template>
    </v-data-table>
    <h3 v-else>Es liegen keine Terminanfragen von Ihnen vor</h3>
  </v-layout>
</template>

<script lang="ts">
import { Component, Prop, Vue } from 'vue-property-decorator'
import { Allocation } from '../models'
import { AllocationRequest, AllocationModel } from '../models/interfaces'
import moment from 'moment'
import { deleteAllocation } from '../services/AllocationApiService'

@Component
export default class MyAllocationsList extends Vue {
  private search: string = ''
  private hideOld: boolean = true
  private headers: object[] = [
    { text: 'Bearbeiten', value: 'action', sortable: false },
    { text: 'Bezeichnung', value: 'Title' },
    { text: 'Status', value: 'Status' },
    { text: 'Raum', value: 'Ressource' },
    { text: 'Von', value: 'From' },
    { text: 'Bis', value: 'To' },
    { text: 'Zuletzt geändert', value: 'LastModified' }
  ];
  public get hasItems () {
    const allocations = Allocation.query()
      .withAll()
      .get()
    return allocations.length
  }
  public get Requests (): VisibleAllocation[] {
    let myId = this.$store.state.user.id
    const allocations = Allocation.query().withAll()
      .where((record : any, query: any) => {
        query.where('CreatedById', myId).orWhere('ReferencePersonId', `${myId}`)
      })
      .where((a: any) => {
        return !this.hideOld || Date.parse(a.To) > Date.now()
      })
      .get()

    if (!allocations.length) return []

    return allocations.map((v: any) => ({
      ...v,
      // @ts-ignore
      Status: this.$options.filters.status2string(v.Status),
      Ressource: v.Ressource.Name
    }))
  }

  private async deleteItem (item: VisibleAllocation) {
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

    const isLastAllocation = this.isLastAllocation(item.PurposeId)
    let success = await deleteAllocation(item.Id)
    if (success) this.$dialog.message.success('Löschung erfolgreich', { position: 'center-left' })
    else this.$dialog.error({ text: 'Löschen fehlgeschlagen', title: 'Fehler' })
  }

  private isLastAllocation (purposeID: number): boolean {
    const purposes = this.Requests.filter(
      (v: VisibleAllocation) => v.PurposeId === purposeID
    )
    if (purposes.length <= 1) return true
    return false
  }
}

interface VisibleAllocation {
  Id: number;
  Title: string;
  Status: string;
  Ressource: string;
  From: string;
  PurposeId: number;
  LastModified: string;
}
</script>

<style scoped lang="stylus">
.appointment-open-list:nth-of-type(2n) {
  background-color: lightgrey;
}
</style>
