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
    </v-data-table>
  </v-layout>
  </template>

<script lang="ts">
import { submitGadget, editGadget, deleteGadget } from '../services/GadgetApiService'
import { Component, Prop, Vue, Watch } from 'vue-property-decorator'
import { Ressource, Gadget, Supplier } from '../models'
import { GadgetItem } from '@/models/interfaces'

@Component
export default class GadgetView extends Vue {
  private headers: object[] = [
    { text: 'Bezeichnung', value: 'Title' },
    { text: 'Unterstützergruppe', value: 'SuppliedBy' }
  ]
  private get items (): Array<GadgetItem> {
    let supplierGroups = new Map()
    Supplier.query().get().forEach((s: Supplier) => {
      supplierGroups.set(s.Id, s.Title)
    })

    let allGadgets: Array<Gadget> = Gadget.query().withAll().get()
    let rValue: Array<GadgetItem> = allGadgets.map((g: Gadget) => ({
      Id: g.Id,
      Title: g.Title,
      SuppliedBy: g.SuppliedBy,
      SupplierName: supplierGroups.get(g.SuppliedBy)
    }))

    return rValue
  }
}
</script>
