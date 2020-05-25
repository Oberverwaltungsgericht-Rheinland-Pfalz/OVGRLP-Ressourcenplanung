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
import { RessourceModel } from '../models/interfaces'

@Component({})
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

  private clicked (a1: any, a2: any) {
    this.dialog = 1
    this.editId = a1.Id
    this.editTitle = a1.Name
    this.editType = a1.Type
    this.editDescription = a1.FunctionDescription
    this.editDetails = a1.SpecialsDescription
  }
  private get items () {
    return Ressource.all()
  }
}
</script>

<style lang="stylus" scoped>
.alignRight
  margin-left auto
</style>
