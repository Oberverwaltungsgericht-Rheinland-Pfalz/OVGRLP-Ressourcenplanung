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
            Neue Gruppe hinzufügen
            <v-icon>add</v-icon>
          </v-btn>
        </v-toolbar>
      </template>
      <template v-slot:item.action="{ item }">
        <v-icon @click="editItem(item)">edit</v-icon>
        <v-icon @click="deleteItem(item)">delete</v-icon>
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
                <v-text-field
                  label="Email*"
                  required
                  v-model="editEmail"
                ></v-text-field>
              </v-col>
            </v-row>
          </v-container>
        </v-card-text>
        <v-card-actions>
          <div class="flex-grow-1"></div>
          <v-btn
            color="green darken-1"
            text
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
import { Supplier, Ressource } from '../../models'

@Component
export default class SupplierManagement extends Vue {
  private dialog: number = 0
  private editId: number = 0
  private editTitle: string = ''
  private editEmail: string = ''
  private nameRules = [(v: string) => !!v || 'Name is required']
  private emailRules = [
    (v: string) => !!v || 'E-mail ist notwendig',
    (v: string) => /.+@.+/.test(v) || 'E-mail ungültig'
  ]
  private valid: boolean = false
  private headers: object[] = [
    { text: 'Bezeichnung', value: 'Title' },
    { text: 'Email', value: 'GroupEmail' },
    { text: 'Bearbeiten', value: 'action', sortable: false }
  ]
  private get ModalTitle () {
    if (this.dialog === 1) return 'Neue Gruppe'
    if (this.dialog === 2) return 'Bearbeite Gruppe'
  }
  private get RessourceNames () {
    return Ressource.all()
      .filter((v: any) => !!v.Title)
      .map((v: any) => v.Title)
  }
  private get items () {
    return Supplier.all()
  }
  private get invalidForm (): boolean {
    return !this.editTitle || !this.editEmail
  }
  private closeModal () {
    this.dialog = 0
    this.editId = 0
    this.editTitle = ''
    this.editEmail = ''
  }
  private editItem (item: WebApi.SupplierGroup) {
    this.editId = item.Id
    this.editTitle = item.Title
    this.editEmail = item.GroupEmail
    this.dialog = 2
  }
  private async updateItem () {
    const gadget = {
      Id: this.editId,
      Title: this.editTitle,
      GroupEmail: this.editEmail
    }
    if (this.dialog === 2) {
      const response = await Supplier.api().put(
        `suppliergroups/${this.editId}`,
        gadget
      )
      await Supplier.update(gadget)
    } else {
      await Supplier.api().post('suppliergroups', gadget)
    }
    this.closeModal()
  }
  private async deleteItem (item: WebApi.SupplierGroup) {
    const confirmation = await this.$dialog.confirm({
      text: `Möchten sie die Unterstützergruppe ${item.Title} wirklich löschen?`,
      title: 'Löschen bestätigen',
      persistent: true,
      actions: [
        {
          text: 'Nein',
          color: 'blue',
          key: false
        },
        {
          text: 'Löschen',
          color: 'red',
          key: true
        }
      ]
    })

    if (confirmation === true) {
      try {
        let response = await Supplier.api().delete(`suppliergroups/${item.Id}`, { delete: item.Id })
      } catch (e) {
        await this.$dialog.error({
          text: 'Es können nur Gruppen gelöscht werden welche nicht mit einem Termin verbunden sind. Vergange Termine sind ebenfalls zu berücksichtigen. Bitte wenden sie sich an ihren IT-Support falls sie Hilfe benötigen.',
          title: 'Löschen fehlgeschlagen'
        })
      }
    }
  }
}
</script>
