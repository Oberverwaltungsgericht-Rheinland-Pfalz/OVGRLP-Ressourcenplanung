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
      <template v-slot:[`item.Remind`]="{ item }"><v-icon v-if="item.Remind">check</v-icon></template>
      <template v-slot:[`item.action`]="{ item }">
        <v-icon @click="editItem(item)" title="Hilfsmittel bearbeiten" class="mr-2">edit</v-icon>
        <v-icon @click="openDialog(item)" title="Hilfsmittel löschen">delete</v-icon>
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
            <v-row>
              <v-col cols="12">
                <v-checkbox v-model="editRemind" label="Gruppe erinnern"></v-checkbox>
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
import { ShowToast, ConfirmData } from '../../models/interfaces'

@Component
export default class SupplierManagement extends Vue {
  private dialog: number = 0
  private editId: number = 0
  private editTitle: string = ''
  private editEmail: string = ''
  private editRemind: boolean = false
  private nameRules = [(v: string) => !!v || 'Name is required']
  private emailRules = [
    (v: string) => !!v || 'E-mail ist notwendig',
    (v: string) => /.+@.+/.test(v) || 'E-mail ungültig'
  ]
  private valid: boolean = false
  private headers: object[] = [
    { text: 'Bezeichnung', value: 'Title' },
    { text: 'Email', value: 'GroupEmail' },
    { text: 'Erinnern', value: 'Remind' },
    { text: 'Bearbeiten', value: 'action', sortable: false }
  ]
  private get ModalTitle () {
    if (this.dialog === 1) return 'Neue Gruppe'
    if (this.dialog === 2) return 'Bearbeite Gruppe'
  }
  private get items (): Array<WebApi.SupplierGroup> {
    return Supplier.all()
  }
  private get invalidForm (): boolean {
    return !this.editTitle || !this.editEmail
  }
  private closeModal (): void {
    this.dialog = 0
    this.editId = 0
    this.editTitle = ''
    this.editEmail = ''
    this.editRemind = false
  }
  private editItem (item: WebApi.SupplierGroup): void {
    this.editId = item.Id
    this.editTitle = item.Title
    this.editEmail = item.GroupEmail
    this.editRemind = item.Remind
    this.dialog = 2
  }
  private async updateItem () {
    const gadget: WebApi.SupplierGroup = {
      Id: this.editId,
      Title: this.editTitle,
      GroupEmail: this.editEmail,
      Remind: this.editRemind
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
  private openDialog (item: WebApi.SupplierGroup) {
    let data: ConfirmData = { title: 'Löschen bestätigen',
      content: `Möchten sie die Unterstützergruppe ${item.Title} wirklich löschen?`,
      callback: this.deleteItem,
      id: item.Id
    }
    this.$root.$emit('user-confirm', data)
  }
  private async deleteItem (id: number) {
    try {
      let response = await Supplier.api().delete(`suppliergroups/${id}`, { delete: id })
      this.$root.$emit('notify-user', { text: 'Löschung erfolgreich', color: 'success' } as ShowToast)
    } catch (e) {
      await this.$root.$emit('notify-user', {
        text: `Löschen fehlgeschlagen
        Es können nur Ressourcen gelöscht werden welche nicht mit einem Termin verbunden sind. Vergange Termine sind ebenfalls zu berücksichtigen. Bitte wenden sie sich an ihren IT-Support falls sie Hilfe benötigen.`,
        color: 'error',
        timeout: 1e4
      } as ShowToast)
    }
  }
}
</script>
