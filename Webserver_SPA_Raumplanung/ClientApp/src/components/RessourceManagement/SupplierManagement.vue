<template>
<v-layout column>
  <v-data-table
    :headers="headers"
    :items="items"
    :disable-pagination="true" hide-default-footer
    >
    <template v-slot:top>
      <v-toolbar flat color="white">
        <v-btn @click="dialog = 1" color="primary">Neue Gruppe hinzufügen<v-icon>add</v-icon></v-btn>
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
        <span class="headline">{{ModalTitle}}: {{editTitle}}</span>
      </v-card-title>
      <v-card-text>
        <v-container>
          <v-row>
            <v-col cols="12">
              <v-text-field label="Bezeichnung*" v-model="editTitle" required></v-text-field>
            </v-col>
            <v-col cols="12">
              <v-text-field label="Email*" required v-model="editEmail"></v-text-field>
            </v-col>
          </v-row>
        </v-container>
      </v-card-text>
      <v-card-actions>
        <div class="flex-grow-1"></div>
        <v-btn color="green darken-1" text :disabled="invalidForm" @click="updateItem"><v-icon>save</v-icon> Speichern</v-btn>
        <v-btn color="orange darken-1" text @click="closeModal"><v-icon>close</v-icon> Abbrechen</v-btn>
      </v-card-actions>
    </v-card>
  </v-dialog>
</v-layout>
</template>

<script lang="ts">
import Vue from 'vue'
import Component from 'vue-class-component'
import Suppliers, { SupplierGroupModel } from '../../models/SupplierModel'
import Ressources from '../../models/RessourceModel'

@Component
export default class SupplierManagement extends Vue {
  private dialog: number = 0
  private editId: number = 0
  private editTitle: string = ''
  private editEmail: string = ''
  private nameRules = [
    (v: string) => !!v || 'Name is required'
  ]
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
    return Ressources.all().filter((v: any) => !!v.Title).map((v: any) => v.Title)
  }
  private get items () {
    return Suppliers.all()
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
  private editItem (item: SupplierGroupModel) {
    this.editId = item.Id
    this.editTitle = item.Title
    this.editEmail = item.GroupEmail
    this.dialog = 2
  }
  private async updateItem () {
    const gadget = { Id: this.editId, Title: this.editTitle, GroupEmail: this.editEmail }
    if (this.dialog === 2) {
      const response = await Suppliers.api().put(`SupplierGroups/${this.editId}`, gadget)
      await Suppliers.update(gadget)
    } else {
      await Suppliers.api().post('SupplierGroups', gadget)
    }
    this.closeModal()
  }
  private async deleteItem (item: SupplierGroupModel) {
    const confirmation = await this.$dialog.confirm({
      text: `Möchten sie die Unterstützergruppe ${item.Title} wirklich löschen?`,
      title: 'Löschen bestätigen',
      persistent: true,
      actions: [{
        text: 'Nein', color: 'blue', key: false
      }, {
        text: 'Löschen', color: 'red', key: true
      }]
    })

    if (confirmation === true) Suppliers.api().delete(`SupplierGroups/${item.Id}`, { delete: item.Id })
  }
}
</script>
