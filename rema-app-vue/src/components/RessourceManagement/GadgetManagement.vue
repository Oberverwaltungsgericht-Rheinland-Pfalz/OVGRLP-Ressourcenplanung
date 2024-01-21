<template>
  <v-layout column>
    <v-data-table
      :headers="headers"
      :items="items"
      :disable-pagination="true"
      hide-default-footer
      height="75vh"
      fixed-header
    >
      <template v-slot:top>
        <v-toolbar flat color="white">
          <v-btn @click="dialog = 1" color="primary">
            Neues Hilfsmittel hinzufügen
            <v-icon>add</v-icon>
          </v-btn>
        </v-toolbar>
      </template>
      <template v-slot:[`item.action`]="{ item }">
        <v-icon @click="editItem(item)" title="Unterstützergruppe bearbeiten" class="mr-2">edit</v-icon>
        <v-icon @click="confirmDelete(item)" title="Unterstützergruppe löschen">delete</v-icon>
      </template>
      <template v-slot:[`item.SuppliedBy`]="{ item }">{{item.SupplierName}}</template>
      <template v-slot:[`item.Title`]="{ item }"><span :class="{'deactivated-title': item.IsDeactivated}">{{item.Title}}</span>
        <v-icon v-if="item.IsDeactivated" color="warning">power_off</v-icon>
      </template>
    </v-data-table>
    <v-dialog :value="dialog" persistent max-width="600px" scrollable>
      <v-card>
        <v-card-title>
          <span class="headline">{{ ModalTitle }}: {{ editTitle }}</span>
        </v-card-title>
        <v-card-text>
          <v-container>
            <v-row>
              <v-col cols="12">
                <v-text-field
                  label="Bezeichnung*"
                  v-model="editTitle"
                  required
                ></v-text-field>
              </v-col>
              <v-col cols="12">
                <v-select
                  v-model="editSupplier"
                  :items="supplierItems"
                  item-disabled="IsDeactivated"
                  :label="'Unterstützergruppe*'"
                  item-text="Title"
                  item-value="Id"
                  persistent-hint
                />
              </v-col>
            </v-row>
            <v-row>
              <v-col class="no-vertical-padding" cols="12">
                <v-switch
                  v-model="IsDeactivated"
                  :label="IsDeactivated ? 'Ressource ist deaktiviert' : 'Ressource ist aktiviert'"
                  color="warning"
                  true-value="true"
                  hide-details
                ></v-switch>
              </v-col>
            </v-row>
          </v-container>
        </v-card-text>
        <v-card-actions>
          <div class="flex-grow-1"></div>
          <v-btn
            color="green darken-1"
            :disabled="invalidForm"
            @click="updateItem"
          >
            <v-icon>save</v-icon>Speichern
          </v-btn>
          <v-btn color="orange darken-1" text @click="closeModal">
            <v-icon>close</v-icon>Abbrechen
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>
  </v-layout>
</template>

<script lang="ts">
import Vue from 'vue'
import Component from 'vue-class-component'
import { ShowToast, ConfirmData, GadgetItem } from '../../models/interfaces'
import { Supplier, Ressource, Gadget } from '../../models'
import { submitGadget, editGadget, deleteGadget } from '../../services/GadgetApiService'

@Component
export default class GadgetManagement extends Vue {
  public dialog: number = 0
  public editId: number = 0
  public editTitle: string = ''
  public editSupplier: number = 0
  public IsDeactivated: boolean = false

  public nameRules = [(v: string) => !!v || 'Name is required']
  public valid: boolean = false
  public headers: object[] = [
    { text: 'Bezeichnung', value: 'Title' },
    { text: 'Unterstützergruppe', value: 'SuppliedBy' },
    { text: 'Bearbeiten', value: 'action', sortable: false }
  ]

  public get ModalTitle () {
    if (this.dialog === 1) return 'Neues Hilfsmittel'
    if (this.dialog === 2) return 'Bearbeite Hilfsmittel'
  }

  public get supplierItems () {
    return Supplier.all()
  }
  public get items (): Array<GadgetItem> {
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
  public get invalidForm (): boolean {
    return !this.editTitle || !this.editSupplier
  }
  public closeModal () {
    this.dialog = 0
    this.editId = 0
    this.editTitle = ''
    this.IsDeactivated = false
    this.editSupplier = 0
  }
  public editItem (item: GadgetItem) {
    this.editId = item.Id
    this.editTitle = item.Title
    this.IsDeactivated = item.IsDeactivated
    this.editSupplier = item.SuppliedBy
    this.dialog = 2
  }
  public async updateItem () {
    const data: WebApi.GadgetViewModel = {
      Id: this.editId,
      Title: this.editTitle,
      IsDeactivated: this.IsDeactivated,
      SuppliedBy: this.editSupplier
    }
    if (this.dialog === 2) {
      let success = await editGadget(data)
      if (success) this.$root.$emit('notify-user', { text: 'Hilfsmittel gespeichert', color: 'success' } as ShowToast)
      else this.$root.$emit('notify-user', { text: 'Hilfsmittel speichern fehlgeschlagen', color: 'error' } as ShowToast)

      await Gadget.update(data)
    } else {
      const group = Supplier.find(this.editSupplier)

      let success = await submitGadget(data)
      if (success) this.$root.$emit('notify-user', { text: 'Hilfsmittel gespeichert', color: 'success' } as ShowToast)
      else this.$root.$emit('notify-user', { text: 'Hilfsmittel speichern fehlgeschlagen', color: 'error' } as ShowToast)
    }
    this.closeModal()
  }
  public confirmDelete (item: WebApi.GadgetViewModel) {
    let data: ConfirmData = { title: 'Löschen bestätigen',
      content: `Möchten sie dieses Hilfsmittel ${item.Title} wirklich löschen?`,
      callback: this.deleteItem,
      id: item.Id
    }
    this.$root.$emit('user-confirm', data)
  }
  public async deleteItem (id: number) {
    let success = await deleteGadget(id)
    if (success) {
      this.$root.$emit('notify-user', { text: 'Hilfsmittel gelöscht', color: 'success' } as ShowToast)
    } else {
      this.$root.$emit('notify-user', {
        text: 'Es können nur Hilfsmittel gelöscht werden welche nicht mit einem Termin verbunden sind. Vergange Termine sind ebenfalls zu berücksichtigen. Bitte wenden sie sich an ihren IT-Support falls sie Hilfe benötigen.',
        color: 'error',
        timeout: 1e4
      } as ShowToast)
    }
  }
}
</script>

<style lang="stylus">
.no-vertical-padding
  padding-top 0
  padding-bottom 0

.deactivated-title
  color darkorange
  font-weight bold
</style>
