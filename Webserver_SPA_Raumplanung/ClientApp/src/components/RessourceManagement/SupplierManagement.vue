<template>
<v-layout column>
    <h2>Unterstützergruppenverwaltung</h2>
    
    <v-data-table
    :headers="headers"
    :items="items">
  <template v-slot:top>
    <v-toolbar v-if="!createNew" flat color="white">
      <v-btn @click="createNew=true" color="primary">Neue Gruppe hinzufügen<v-icon>add</v-icon></v-btn>
    </v-toolbar>
 <v-card v-if="createNew">
  <v-card-title class="blue white--text">
    <span class="headline">Neues Gruppe</span>
    <v-spacer/>
    <v-btn @click="createNew=false" rounded><v-icon>close</v-icon></v-btn>
  </v-card-title>

  <v-form v-model="valid" ref="form">
    <v-container grid-list-xl>
      <v-layout wrap>
        <v-flex xs12 md5>
          <v-text-field
            v-model="Title"
            :rules="nameRules"
            label="Bezeichnung*"
            required
          ></v-text-field>
        </v-flex>

        <v-flex xs12 md5>
          <v-text-field
            v-model="Email"
            :rules="emailRules"
            label="Email"
            required
          ></v-text-field>
        </v-flex>

        <v-flex xs12 md2>
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
import Suppliers from '../../models/SupplierModel'
import Ressources from '../../models/RessourceModel'

@Component
export default class GadgetManagement extends Vue {
  private createNew: boolean = false
  private Title: string = ''
  private Email: string = ''
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

  private get RessourceNames () {
    return Ressources.all().filter((v: any) => !!v.Title).map((v: any) => v.Title)
  }
  private get items () {
    return Suppliers.all()
  }
  private async add () {
        // @ts-ignore
    if (!this.$refs.form.validate()) return
    const data = [
            { Title: this.Title, GroupEmail: this.Email }
    ]

    await Suppliers.insert({ data })
  }

  private async mounted () {
    const data = [
            { id: 1, Title: 'Wachtmeister', GroupEmail: 'NJZ.Wachtmeister@ovg.jm.rlp.de' },
            { id: 2, Title: 'EDV', GroupEmail: 'edv.support@ovg.jm.rlp.de' }
    ]
    await Suppliers.insert({ data })

  }
}
</script>
