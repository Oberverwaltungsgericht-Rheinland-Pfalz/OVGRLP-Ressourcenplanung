<template>
<v-layout column>
  <ressource-form-dialog v-if="showEditDialog" hideButton :showFormDialog="showEditDialog" :formTitle="'Ressource bearbeiten'" :editedItem="editedItem" @save="save($event)" @close="showEditDialog = false"/>
    <v-data-table
      :headers="headers"
      :items="items"
      sort-by="calories"
      class="elevation-1"
    >
      <template v-slot:top>
        <v-toolbar flat color="white">
          <v-toolbar-title>Enthaltene Ressourcen</v-toolbar-title>
          <v-divider class="mx-4" inset vertical/><v-spacer/>
          <ressource-form-dialog :showFormDialog="showFormDialog" :formTitle="'Ressource erstellen'" :editedItem="editedItem" @save="save($event)" @close="showFormDialog = false"/>
        </v-toolbar>
      </template>
      <template v-slot:item.action="{ item }">
        <v-icon class="mr-2" @click="editItem(item)">edit</v-icon>
        <v-icon @click="deleteItem(item)">delete</v-icon>
      </template>
      <template v-slot:no-data>
        <v-btn color="primary" @click="initialize">Reset</v-btn>
      </template>
    </v-data-table>
  </v-layout>
</template>

<script lang="ts">
import RessourceFormDialog from './RessourceFormDialog.vue'
import { Component, Prop, Vue, Watch } from 'vue-property-decorator'
import Ressource from '../../models/RessourceModel'

@Component({
  components: { RessourceFormDialog }
})
export default class RessourceManagement extends Vue {
  private showFormDialog: boolean = false
  private showEditDialog: boolean = false
  private items: ViewRessource[] = []
  private ressourceTypes: string[] = ['Gemeinschaftsraum', 'Gerichtssaal']
  private headers: object[] = [
    { text: 'Bezeichnung', value: 'Title' },
    { text: 'Ressourcen-Typ', value: 'Type' },
    { text: 'Funktionsbeschreibung', value: 'FunctionDescription' },
    { text: 'Details', value: 'SpecialDescription' },
    { text: 'Actions', value: 'action', sortable: false }
  ]
  private editedIndex: number = -1
  private editedItem: ViewRessource = {
    Title: '',
    Type: '',
    FunctionDescription: '',
    SpecialDescription: ''
  }
  private defaultItem: object = {
    Title: '',
    Type: '',
    FunctionDescription: '',
    SpecialDescription: ''
  }

  private get editDialog (): boolean {
    return this.editedIndex !== -1
  }

  @Watch('showFormDialog')
  private onShowFormDialog (val: boolean) {
    if (!val) {
      this.$nextTick(() => {
        this.editedItem = { Title: '', Type: '', FunctionDescription: '', SpecialDescription: '' }
        this.editedIndex = -1
      })
    }
  }

  private created () {
    this.initialize()
  }


  private initialize () {
        // load the ressources from server here
    this.items.push(...[
      {
        Title: 'Multifunktionsraum',
        Type: 'Gemeinschaftsraum',
        FunctionDescription: 'Allgemeiner Raum des NJZ',
        SpecialDescription: '20 Sitzplätze und Beamer'
      },
      {
        Title: 'Sitzungssaal A008',
        Type: 'Gerichtssaal',
        FunctionDescription: 'Saal des ArbG',
        SpecialDescription: 'Kleiner Gerichtssaal'
      },
      {
        Title: 'Sitzungssaal E022',
        Type: 'Gerichtssaal',
        FunctionDescription: 'Saal des OVG',
        SpecialDescription: 'Großer Gerichtssaal'
      }
    ])
  }

  private editItem (item: ViewRessource) {
    this.editedIndex = this.items.indexOf(item)
    this.editedItem = Object.assign({}, item)
    this.showEditDialog = true
  }

  private async deleteItem (item: ViewRessource) {
    const index = this.items.indexOf(item)
    const confirmation = await this.$dialog.confirm({
      text: 'Möchten sie diese Ressource wirklich löschen?',
      title: 'Löschen bestätigen',
      persistent: true
    })
    if (confirmation) this.items.splice(index, 1)

      // delete from server
  }

  private save () {
    if (this.editDialog) {
          // Update to server here
      Object.assign(this.items[this.editedIndex], this.editedItem)
    } else {
          // Save to server here
      this.items.push(this.editedItem)
    }
    this.showFormDialog = false
    this.showEditDialog = false
  }
}

interface ViewRessource {
  Title: string
  Type: string
  FunctionDescription: string
  SpecialDescription: string
}
</script>