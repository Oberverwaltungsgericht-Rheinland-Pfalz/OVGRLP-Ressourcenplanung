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
                  :label="'Unterstützergruppe*'"
                  item-text="Title"
                  item-value="Id"
                  persistent-hint
                />
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
export default class SupplierManagement extends Vue {
  private dialog: number = 0
  private editId: number = 0
  private editTitle: string = ''
  private editSupplier: number = 0

  private nameRules = [(v: string) => !!v || 'Name is required']
  private valid: boolean = false
  private headers: object[] = [
    { text: 'Bezeichnung', value: 'Title' },
    { text: 'Unterstützergruppe', value: 'SuppliedBy' },
    { text: 'Bearbeiten', value: 'action', sortable: false }
  ]

  private get ModalTitle () {
    if (this.dialog === 1) return 'Neues Hilfsmittel'
    if (this.dialog === 2) return 'Bearbeite Hilfsmittel'
  }

  private get supplierItems () {
    return Supplier.all()
  }
  private get items (): Array<GadgetItem> {
    return Gadget.query().withAll().get().map((g: Gadget) => ({
      Id: g.Id,
      Title: g.Title,
      SuppliedBy: g.SuppliedBy.Id,
      SupplierName: g.SuppliedBy.Title
    }))
  }
  private get invalidForm (): boolean {
    return !this.editTitle || !this.editSupplier
  }
  private closeModal () {
    this.dialog = 0
    this.editId = 0
    this.editTitle = ''
    this.editSupplier = 0
  }
  private editItem (item: GadgetItem) {
    this.editId = item.Id
    this.editTitle = item.Title
    this.editSupplier = item.SuppliedBy
    this.dialog = 2
  }
  private async updateItem () {
    const data = {
      Id: this.editId,
      Title: this.editTitle,
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
  private confirmDelete (item: WebApi.GadgetViewModel) {
    let data: ConfirmData = { title: 'Löschen bestätigen',
      content: `Möchten sie dieses Hilfsmittel ${item.Title} wirklich löschen?`,
      callback: this.deleteItem,
      id: item.Id
    }
    this.$root.$emit('user-confirm', data)
  }
  private async deleteItem (id: number) {
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
