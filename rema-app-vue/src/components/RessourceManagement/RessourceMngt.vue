<template>
  <v-layout column>
    <v-data-table
      :headers="headers"
      :items="items"
      sort-by="calories"
      :disable-pagination="true"
      hide-default-footer
      height="75vh"
      fixed-header
    >
      <template v-slot:top>
        <v-toolbar flat color="white">
          <v-btn @click="dialog = 1" color="primary">
            Neue Ressource hinzufügen
            <v-icon>add</v-icon>
          </v-btn>
        </v-toolbar>
      </template>

      <template v-slot:item.action="{ item }">
        <v-icon class="mr-2" @click="editItem(item)">edit</v-icon>
        <v-icon @click="confirmItem(item)">delete</v-icon>
      </template>
      <template v-slot:no-data>
        <v-btn color="primary">Neu laden</v-btn>
      </template>
    </v-data-table>

    <v-dialog :value="dialog" persistent max-width="800px" scrollable>
      <v-card>
        <v-card-title>
          <span class="headline">{{ModalTitle}}: {{editTitle}}</span>
        </v-card-title>
        <v-card-text>
          <v-container>
            <v-row>
              <v-col cols="12">
                <v-text-field :label="headers[0].text + '*'" v-model="editTitle" required></v-text-field>
              </v-col>
              <v-col cols="12">
                <!--<v-text-field :label="headers[1].text + '*'" v-model="editType" required></v-text-field>-->
                <v-combobox
                  v-model="editType"
                  :items="typeItems"
                  :label="headers[1].text+'*'"
                  required
                ></v-combobox>
              </v-col>
              <v-col cols="12">
                <v-text-field :label="headers[2].text" v-model="editDescription" required></v-text-field>
              </v-col>
              <v-col cols="12">
                <v-text-field :label="headers[3].text" v-model="editDetails" required></v-text-field>
              </v-col>
            </v-row>
          </v-container>
        </v-card-text>
        <v-card-actions>
          <div class="flex-grow-1"></div>
          <v-btn color="green darken-1" :disabled="invalidForm" text @click="updateItem">
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
import { Component, Prop, Vue, Watch } from 'vue-property-decorator'
import { Ressource } from '../../models'
import { ShowToast, ConfirmData } from '../../models/interfaces'

@Component({})
export default class RessourceManagement extends Vue {
  private showNewForm: boolean = false;
  private dialog: number = 0;
  private editId: number = 0;
  private editTitle: string = '';
  private editType: string = '';
  private editDescription: string = '';
  private editDetails: string = '';

  private nameRules = [(v: string) => !!v || 'Name is required'];
  private valid: boolean = false;
  private ressourceTypes: string[] = ['Gemeinschaftsraum', 'Gerichtssaal'];
  private headers: object[] = [
    { text: 'Bezeichnung', value: 'Name' },
    { text: 'Ressourcen-Typ', value: 'Type' },
    { text: 'Funktionsbeschreibung', value: 'FunctionDescription' },
    { text: 'Details', value: 'SpecialsDescription' },
    { text: 'Actions', value: 'action', sortable: false }
  ];
  private editedIndex: number = -1;

  private get typeItems () {
    return Ressource.query()
      .all()
      .map((e: any) => e.Type)
  }
  private get ModalTitle () {
    if (this.dialog === 1) return 'Neue Ressource'
    if (this.dialog === 2) return 'Bearbeite Ressource'
  }
  private async updateItem () {
    const data = {
      Id: this.editId,
      Name: this.editTitle,
      Type: this.editType,
      FunctionDescription: this.editDescription,
      SpecialsDescription: this.editDetails
    }
    if (this.dialog === 2) {
      const response = await Ressource.api().put(
        `ressources/${this.editId}`,
        data
      )
      await Ressource.update(data)
    } else {
      await Ressource.api().post('ressources', data)
    }
    this.closeModal()
  }

  private closeModal () {
    this.dialog = 0
    this.editId = 0
    this.editTitle = ''
    this.editType = ''
    this.editDescription = ''
    this.editDetails = ''
  }
  private get invalidForm (): boolean {
    return !this.editTitle || !this.editType
  }
  private get editDialog (): boolean {
    return this.editedIndex !== -1
  }

  private get items () {
    return Ressource.all()
  }

  private editItem (item: WebApi.RessourceViewModel) {
    this.editId = item.Id
    this.editTitle = item.Name
    this.editType = item.Type
    this.editDescription = item.FunctionDescription
    this.editDetails = item.SpecialsDescription
    this.dialog = 2
  }
  private confirmItem (item: WebApi.RessourceViewModel) {
    let data: ConfirmData = { title: 'Löschen bestätigen',
      content: `Möchten sie die Ressource ${item.Name} wirklich löschen?`,
      callback: this.deleteItem,
      id: item.Id
    }
    this.$root.$emit('user-confirm', data)
  }
  private async deleteItem (id: number) {
    try {
      let response = await Ressource.api().delete(`ressources/${id}`, { delete: id })
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

  private saveNew (event: WebApi.RessourceViewModel) {
    // Save to server here
    //  this.items.push({ ...this.newItem })
    this.showNewForm = false
  }

  private save (event: any) {
    if (this.editDialog) {
      // Update to server here
    } else {
      // Save to server here
      //    this.items.push(this.editedItem)
    }
    this.dialog = 0
  }
}

</script>
