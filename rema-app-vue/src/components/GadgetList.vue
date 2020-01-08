<template>
  <v-layout column>
    <v-data-table
      :headers="headers"
      :items="list"
      :search="search"
      sort-by="calories"
      class="elevation-1"
    >
      <template v-slot:top>
        <v-toolbar flat color="white">
          <v-toolbar-title>Wartende Aufgaben</v-toolbar-title>
          <v-divider class="mx-4" inset vertical></v-divider>
          <v-spacer></v-spacer>
          <v-text-field
            v-model="search"
            append-icon="search"
            label="Filter"
            single-line
            hide-details
          />
        </v-toolbar>
      </template>
      <template v-slot:item.DateTime="{ item }">{{item.DateTime | toLocal}}</template>
      <template v-slot:item.action="{ item }">
        <v-menu bottom offset-y eager>
          <template v-slot:activator="{ on }">
            <v-btn v-on="on">
              <v-icon>edit</v-icon>
              {{item.status}}
            </v-btn>
          </template>
          <v-list>
            <v-list-item @click="acknowledge(item)">
              <v-list-item-title>
                <v-icon v-html="'done'"></v-icon>Erledigt
              </v-list-item-title>
            </v-list-item>
            <v-list-item @click="inProgress(item)">
              <v-list-item-title>
                <v-icon v-html="'close'"></v-icon>In Arbeit
              </v-list-item-title>
            </v-list-item>
            <v-list-item @click="canceled(item)">
              <v-list-item-title>
                <v-icon v-html="'create'"></v-icon>Gecancelt
              </v-list-item-title>
            </v-list-item>
          </v-list>
        </v-menu>
      </template>
      <template v-slot:no-data>
        <v-btn color="primary" @click="initialize">Neu laden</v-btn>
      </template>
    </v-data-table>

    <v-divider />
    <v-spacer />
    <h3 v-if="isEmpty">Es liegen keine zu bearbeitenden Terminanfragen vor</h3>
  </v-layout>
</template>

<script lang="ts">
import dayjs from 'dayjs'
import { State, Action, Getter, Mutation } from 'vuex-class'
import { Component, Prop, Vue } from 'vue-property-decorator'
import { Names as Fnn } from '../store/Acknowledges/types'

@Component({
  filters: {
    toLocal (dateVal: Date): string {
      return dayjs(dateVal).format(' DD.MM.YYYY hh:mm')
    }
  }
})
export default class GadgetList extends Vue {
  private isEmpty: boolean = false; // todo: listen fn nutzen
  private search: string = '';
  private headers: object[] = [
    { text: 'Bearbeiten', value: 'action', sortable: false },
    { text: 'Name', value: 'Title' },
    { text: 'Status', value: 'Status' },
    { text: 'Ressource', value: 'Ressource' },
    { text: 'Hilfsmittel', value: 'Gadget' },
    { text: 'Datum', value: 'DateTime' }
  ];

  private list: object[] = [
    {
      Title: 'Meeting',
      Status: 'offen',
      Ressource: 'Raum1',
      Gadget: 'Beamer',
      DateTime: new Date()
    },
    {
      Title: 'Meeting2',
      Status: 'erledigt',
      Ressource: 'Raum2',
      Gadget: 'Monitor',
      DateTime: new Date()
    }
  ];
  private acknowledge (item: any) {
    // change appointment status to accepted
    item.Status = 'erledigt'
    return true
  }
  private inProgress (item: any) {
    // change appointment status to rejected
    item.Status = 'wird erledigt'
    return false
  }
  private canceled (item: any) {
    // change appointment status to changed and change the date
    item.Status = 'verschoben'
    return false
  }
}
</script>

<style scoped lang="stylus">
.appointment-open-list:nth-of-type(2n) {
  background-color: lightgrey;
}
</style>
