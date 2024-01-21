<template>
  <v-layout column>
    <v-data-table
      :headers="headers"
      :items="items"
      :disable-pagination="true"
      hide-default-footer
      height="75vh"
      fixed-header
      sort-by="SuppliedBy"
    >
      <template v-slot:[`item.SuppliedBy`]="{ item }">{{ item.SupplierName }}</template>
      <template v-slot:[`item.Title`]="{ item }">
        <span :style="{color: item.IsDeactivated ? 'darkorange' : 'black', 'font-weight': item.IsDeactivated ? 'bold' : 'normal'}">{{ item.Title }}</span>
      </template>
    </v-data-table>
  </v-layout>
  </template>

<script lang="ts">
import { Component, Vue } from 'vue-property-decorator'
import { Gadget, Supplier } from '../models'
import { GadgetItem } from '../models/interfaces'

@Component
export default class GadgetView extends Vue {
  headers: object[] = [
    { text: 'Bezeichnung', value: 'Title' },
    { text: 'Unterstützergruppe', value: 'SuppliedBy' }
  ]
  get items (): Array<GadgetItem> {
    let supplierGroups = new Map()
    Supplier.query().get().forEach((s: Supplier) => {
      supplierGroups.set(s.Id, s.Title)
    })

    let allGadgets: Array<Gadget> = Gadget.query().withAll().get()
    let rValue: Array<GadgetItem> = allGadgets.map((g: Gadget) => ({
      Id: g.Id,
      Title: g.Title,
      IsDeactivated: g.IsDeactivated,
      SuppliedBy: g.SuppliedBy,
      SupplierName: supplierGroups.get(g.SuppliedBy)
    }))

    return rValue
  }
}
</script>
