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
<template v-slot:item.action="{ item }">
    <v-dialog v-model="isEditable" persistent max-width="600px">
      <template v-slot:activator="{ on }">
        <v-btn color="primary" outlined dark v-on="on" @click="editItem(item)"><v-icon class="mr-2">edit</v-icon></v-btn>
      </template>
      <v-card>
        <v-card-title>
          <span class="headline">Bearbeiten</span>
        </v-card-title>
        <v-card-text>
          <v-container>
            <v-row>
              <v-col cols="12">
                <v-text-field label="Bezeichnung*" v-model="editTitle" required></v-text-field>
              </v-col>
              <v-col cols="12" sm="6">
                <v-autocomplete
                  v-model="editRessource"
                  :items="RessourceNames"
                  label="In Raum"
                ></v-autocomplete>
              </v-col>
              <v-col cols="12" sm="6">
                <v-autocomplete
                  v-model="editSupplier"
                  :items="SupplierNames"
                  label="Unterstützergruppe"
                ></v-autocomplete>
              </v-col>
            </v-row>
          </v-container>
        </v-card-text>
        <v-card-actions>
          <div class="flex-grow-1"></div>
          <v-btn color="blue darken-1" text @click="isEditable = false">Abbrechen</v-btn>
          <v-btn color="blue darken-1" text @click="saveEditItem">Speichern</v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>

  <v-icon @click="deleteItem(item)">delete</v-icon>
</template>
<template v-slot:no-data>
  <v-btn color="primary">Keine Daten vorhanden</v-btn>
</template>
</v-data-table>
</v-layout>
</template>

<script lang="ts">
import Vue from 'vue'
import Component from 'vue-class-component'
import Gadgets, { GadgetModel } from '../../models/GadgetModel'
import Ressources from '../../models/RessourceModel'
import Suppliers, { SupplierGroupModel } from '../../models/SupplierModel'

@Component
export default class GadgetManagement extends Vue {

  private get RessourceNames () {
    return Ressources.all().filter((v: any) => !!v.title).map((v: any) => v.title)
  }
  private get sn () {
    return Suppliers.all()
  }
  private get SupplierNames () {
    return Suppliers.query()./*where('title', (v: string) => !!v).*/get().map((v: any) => v.title)
  }
  private get items () {
    return Gadgets.all()
  }
  private createNew: boolean = false
  private Title: string = ''
  private InRoom: string = ''
  private InGroup: string = ''
  private nameRules = [
    (v: string) => !!v || 'Name is required'
  ]
  private isEditable: boolean = false
  private editRessource: string = ''
  private editSupplier: string = ''
  private editTitle: string = ''
  private editId: number = 0

  private valid: boolean = false
  private headers: object[] = [
      { text: 'Bezeichnung', value: 'Title' },
    //  { text: 'In Raum', value: 'ressourceId' },
      { text: 'Unterstützergruppe', value: 'SuppliedBy' },
      { text: 'Bearbeiten', value: 'action', sortable: false }
  ]
  private async add () {
    // @ts-ignore
    if (!this.$refs.form.validate()) return
    const data = [
            { Title: this.Title, Gadget: this.InRoom }
    ]

    await Gadgets.insert({ data })
  }

  private editItem (item: GadgetModel) {
    this.editTitle = item.Title
    if (item.SuppliedBy) this.editSupplier = Gadgets.find(item.SuppliedBy).Title
    else this.editSupplier = ''
    this.editRessource = ''
    this.editId = item.Id
  }
  private saveEditItem () {
    this.isEditable = false
    let suppliedBy = null
    if (this.editSupplier) {
      const supplier = Suppliers.query().where('title', this.editSupplier).first()
      suppliedBy = {
        id: supplier && supplier.Id,
        title: supplier && supplier.Title,
        GroupEmail: supplier && supplier.GroupEmail
      }
    }
    // @ts-ignore
    Gadgets.$update({
      params: { id: this.editId },
      data: { id: this.editId, title: this.editTitle, suppliedBy }
    })
  }

  private async deleteItem (item: GadgetModel) {
    const confirmation = await this.$dialog.confirm({
      text: 'Möchten sie dieses Hilfsmittel wirklich löschen?',
      title: 'Löschen bestätigen',
      persistent: true
    })
    // @ts-ignore
    if (confirmation) Gadgets.$delete({ params: { id: item.id } })
  }
}
</script>

<style lang="stylus" scoped>
button > span > i.v-icon.mr-2
  margin-right: 0!important;
</style>