<template>
  <v-layout column>
    <v-data-table
      :headers="headers"
      :items="items"
      sort-by="Name"
      :disable-pagination="true"
      hide-default-footer
      height="75vh"
      fixed-header
      @click:row="clicked"
    >
    </v-data-table>

    <v-dialog v-model="dialog" max-width="800px" scrollable>
      <v-card>
        <v-card-title>
          <span class="headline">{{ModalTitle}}: {{editTitle}}</span>
          <v-btn @click="dialog=false" color="red darken-1" rounded outlined class="alignRight"><v-icon>close</v-icon></v-btn>
        </v-card-title>
        <v-card-text>
          <v-container>
            <v-row>
              <v-col cols="12">
                <strong>{{headers[0].text}}:</strong>
                <p><span>{{editTitle}}</span></p>
              </v-col>
              <v-col cols="12">
                <strong>{{headers[1].text}}:</strong>
                <p><span>{{editDescription}}</span></p>
              </v-col>
              <v-col cols="12">
                <strong>{{headers[2].text}}:</strong>
               <p><span>{{editDetails}}</span></p>
              </v-col>
            </v-row>
          </v-container>
        </v-card-text>
      </v-card>
    </v-dialog>
  </v-layout>
  </template>

<script lang="ts">
import { Component, Prop, Vue, Watch } from 'vue-property-decorator'
import { Ressource } from '../models'

@Component
export default class Roomview extends Vue {
  private dialog: number = 0;
  private editId: number = 0;
  private editTitle: string = '';
  private editType: string = '';
  private editDescription: string = '';
  private editDetails: string = '';
  private ModalTitle: string = 'Raum'

  private headers: object[] = [
    { text: 'Bezeichnung', value: 'Name' },
    { text: 'Funktionsbeschreibung', value: 'FunctionDescription' },
    { text: 'Details', value: 'SpecialsDescription' }
  ];

  private clicked (element: Ressource): void {
    this.dialog = 1
    this.editId = element.Id
    this.editTitle = element.Name
    this.editType = element.Type
    this.editDescription = element.FunctionDescription
    this.editDetails = element.SpecialsDescription
  }
  private get items (): Array<Ressource> {
    return Ressource.query().where('IsDeactivated', false).get()
  }
}
</script>
