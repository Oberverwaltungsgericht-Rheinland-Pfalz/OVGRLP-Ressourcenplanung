<template>
  <v-data-table
    :headers="headers"
    :items="desserts"
    sort-by="calories"
    class="elevation-1"
  >
    <template v-slot:top>
      <v-toolbar flat color="white">
        <v-toolbar-title>Enthaltene Ressourcen</v-toolbar-title>
        <v-divider class="mx-4" inset vertical></v-divider>
        <v-spacer></v-spacer>
        <ressource-form-dialog :showFormDialog="showFormDialog" :formTitle="formTitle" :editedItem="editedItem" @save="save($event)" @close="showFormDialog = false">
        </ressource-form-dialog>

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
</template>

<script>
  import RessourceFormDialog from './RessourceFormDialog'

  export default {
    components: {RessourceFormDialog},
    data: () => ({
      showFormDialog: false,
      ressourceTypes: ['Gemeinschaftsraum', 'Gerichtssaal'],
      headers: [
        {
          text: 'Bezeichnung',
          align: 'left',
          sortable: false,
          value: 'name',
        },
        { text: 'Ressourcen-Typ', value: 'type' },
        { text: 'Funktionsbeschreibung', value: 'FunctionDescription' },
        { text: 'Details', value: 'SpecialDescription' },
        { text: 'Actions', value: 'action', sortable: false },
      ],
      desserts: [],
      editedIndex: -1,
      editedItem: {
        name: '',
        type: 0,
        FunctionDescription: 0,
        SpecialDescription: 0
      },
      defaultItem: {
        name: '',
        type: 0,
        FunctionDescription: 0,
        SpecialDescription: 0
      },
    }),

    computed: {
      formTitle () {
          if(this.editDialog) return 'Ressource bearbeiten'
          return 'Ressource erstellen'
      },
      editDialog () {
          return this.editedIndex !== -1
      }
    },

    watch: {
      showFormDialog (val) {
          if(!val) {
            this.$nextTick(() => {
                this.editedItem = Object.assign({}, this.defaultItem)
                this.editedIndex = -1
            })
        }
      },
    },

    created() {
        this.initialize()
    },

    methods: {
      initialize () {
          // load the ressources from server here
        this.desserts = [
          {
            name: 'Multifunktionsraum',
            type: 'Gemeinschaftsraum',
            FunctionDescription: 'Allgemeiner Raum des NJZ',
            SpecialDescription: '20 Sitzplätze und Beamer'
          },
          {
            name: 'Sitzungssaal A008',
            type: 'Gerichtssaal',
            FunctionDescription: 'Saal des ArbG',
            SpecialDescription: 'Kleiner Gerichtssaal'
          },
          {
            name: 'Sitzungssaal E022',
            type: 'Gerichtssaal',
            FunctionDescription: 'Saal des OVG',
            SpecialDescription: 'Großer Gerichtssaal'
          }
        ]
      },

      editItem (item) {
        this.editedIndex = this.desserts.indexOf(item)
        this.editedItem = Object.assign({}, item)
        this.showFormDialog = true
      },

      async deleteItem (item) {
        const index = this.desserts.indexOf(item)
        const confirmation = await this.$dialog.confirm({
            text: 'Möchten sie diese Ressource wirklich löschen?',
            title: 'Löschen bestätigen',
            persistent: true
        })
        if(confirmation) this.desserts.splice(index, 1)

        // delete from server
      },

      save () {
        if (this.editDialog) {
          // Update to server here
          Object.assign(this.desserts[this.editedIndex], this.editedItem)
        } else {
          // Save to server here
          this.desserts.push(this.editedItem)
        }
        this.showFormDialog = false
      },
    },
  }
</script>