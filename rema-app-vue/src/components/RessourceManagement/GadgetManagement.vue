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
      <template v-slot:item.action="{ item }">
        <v-icon @click="editItem(item)">edit</v-icon>
        <v-icon @click="deleteItem(item)">delete</v-icon>
      </template>
      <template v-slot:item.SuppliedBy="{ item }">{{
        item.SuppliedBy | supplierName
      }}</template>
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
import {
  Supplier,
  Ressource,
  Gadget
} from '../../models'
import { SupplierGroupModel, GadgetModel } from '../../models/interfaces'
import { submitGadget, editGadget, deleteGadget } from '../../services/GadgetApiService'

@Component({
  filters: {
    supplierName (id: number) {
      return ((Supplier.find(id) as any) || { Title: '' }).Title
    }
  }
})
export default class SupplierManagement extends Vue {
  private dialog: number = 0
  private editId: number = 0
  private editTitle: string = ''
  private editSupplier: string = ''

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
  private get RessourceNames () {
    return Ressource.all()
      .filter((v: any) => !!v.Title)
      .map((v: any) => v.Title)
  }
  private get supplierItems () {
    return Supplier.all()
  }
  private get items () {
    const suppNames: any = {}
    Supplier.all().forEach((e: any) => (suppNames[e.Id] = e.Title))
    return Gadget.all().map((v: any) => ({
      ...v,
      supplierTitle: suppNames[v.SuppliedBy]
    }))
  }
  private get invalidForm (): boolean {
    return !this.editTitle || !this.editSupplier
  }
  private closeModal () {
    this.dialog = 0
    this.editId = 0
    this.editTitle = ''
    this.editSupplier = ''
  }
  private editItem (item: GadgetModel) {
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
      if (success) this.$dialog.message.success('Hilfsmittel gespeichert', { position: 'center-left' })
      else this.$dialog.error({ text: 'Hilfsmittel speichern fehlgeschlagen', title: 'Fehler' })
      await Gadget.update(data)
    } else {
      const group = Supplier.find(this.editSupplier)

      let success = await submitGadget(data)
      if (success) this.$dialog.message.success('Hilfsmittel gespeichert', { position: 'center-left' })
      else this.$dialog.error({ text: 'Hilfsmittel speichern fehlgeschlagen', title: 'Fehler' })
    }
    this.closeModal()
  }
  private async deleteItem (item: GadgetModel) {
    const confirmation = await this.$dialog.confirm({
      text: `Möchten sie dieses Hilfsmittel ${item.Title} wirklich löschen?`,
      title: 'Löschen bestätigen',
      persistent: true,
      actions: [
        { text: 'Nein', color: 'blue', key: false },
        { text: 'Löschen', color: 'red', key: true }
      ]
    })

    if (confirmation === true) {
      let success = await deleteGadget(item.Id)
      if (success) this.$dialog.message.success('Hilfsmittel gelöscht', { position: 'center-left' })
      else this.$dialog.error({ text: 'Es können nur Hilfsmittel gelöscht werden welche nicht mit einem Termin verbunden sind. Vergange Termine sind ebenfalls zu berücksichtigen. Bitte wenden sie sich an ihren IT-Support falls sie Hilfe benötigen.', title: 'Löschen fehlgeschlagen' })
    }
  }
}
</script>
