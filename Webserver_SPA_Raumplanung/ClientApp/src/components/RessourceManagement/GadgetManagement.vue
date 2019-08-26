<template>
<v-layout column>
    <h2>Hilfsmittelverwaltung</h2>
    <v-data-table
    :headers="headers"
    :items="items">
  <template v-slot:top>
    <v-toolbar v-if="!createNew" flat color="white">
      <v-btn @click="createNew=true" color="primary">Neues Hilfsmittel hinzufügen<v-icon>add</v-icon></v-btn>
    </v-toolbar>
 <v-card v-if="createNew">
  <v-card-title class="blue white--text">
    <span class="headline">Neues Hilfsmittel</span>
    <v-spacer/>
    <v-btn @click="createNew=false" rounded><v-icon>close</v-icon></v-btn>
  </v-card-title>

  <v-form v-model="valid" ref="form">
    <v-container grid-list-xl>
      <v-layout wrap>
        <v-flex xs12 md4>
          <v-text-field
            v-model="Title"
            :rules="nameRules"
            label="Bezeichnung*"
            required
          ></v-text-field>
        </v-flex>

        <v-flex xs12 md4>
        <v-combobox
          v-model="InRoom"
          :items="RessourceNames" outlined
          label="Einem Raum zugeordnet?"
        ></v-combobox>
<!-- <v-menu offset-y>
                <template v-slot:activator="{ on }">
                    <v-btn color="primary" dark v-on="on"><v-icon>arrow_drop_down</v-icon> Im Raum</v-btn>
                </template>
                <v-list>
                    <v-list-item
                    v-for="(item, index) in RessourceNames"
                    :key="index"
                    @click="InRoom = item">
                    <v-list-item-title>{{ item }}</v-list-item-title>
                    </v-list-item></v-list></v-menu>-->
        </v-flex>
        <v-flex xs12 md3>
          <v-combobox
            v-model="InGroup"
            :items="SupplierNames"
            label="Unterstützergruppe"
          ></v-combobox>
        </v-flex>

        <v-flex xs12 md1>
            <v-btn @click="add"><v-icon>save</v-icon></v-btn>
        </v-flex>
      </v-layout>
    </v-container>
  </v-form>
</v-card>

</template>
</v-data-table>
</v-layout>
</template>

<script lang="ts">
import Vue from 'vue'
import Component from 'vue-class-component'
import Gadgets from '../../models/GadgetModel'
import Ressources from '../../models/RessourceModel'
import Suppliers from '../../models/SupplierModel'

@Component
export default class GadgetManagement extends Vue {
  private createNew: boolean = false
  private Title: string = ''
  private InRoom: string = ''
  private InGroup: string = ''
  private nameRules = [
    (v: string) => !!v || 'Name is required'
  ]

  private valid: boolean = false
  private headers: object[] = [
      { text: 'Bezeichnung', value: 'title' },
      { text: 'Bearbeiten', value: 'action', sortable: false }
  ]

  private get RessourceNames () {
    return Ressources.all().filter((v: any) => !!v.Title).map((v: any) => v.Title)
  }
  private get SupplierNames () {
    return Suppliers.query().where('Title', (v: string) => v.length).get().map((v: any) => v.Title)
  }
  private get items () {
    return Gadgets.all()
  }
  private async add () {
        // @ts-ignore
    if (!this.$refs.form.validate()) return
    const data = [
            { Title: this.Title, Gadget: this.InRoom }
    ]

    await Gadgets.insert({ data })
  }
}
</script>
