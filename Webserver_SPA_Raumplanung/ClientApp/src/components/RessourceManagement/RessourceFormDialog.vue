<template>
    <v-dialog v-model="showDialog" max-width="75vw">
        <template v-if="!hideButton" v-slot:activator="{ on }">
            <v-btn color="primary" dark class="mb-2" v-on="on">Neue Ressource erstellen</v-btn>
        </template>
        <v-card>
        <v-card-title>
            <span class="headline">{{ formTitle }}</span>
        </v-card-title>

        <v-card-text>
            <v-container grid-list-xl>
            <v-layout wrap>
                <v-flex xs12 sm6 md6>
                <v-text-field v-model="name" label="Bezeichnung"></v-text-field>
                </v-flex>
                <v-flex xs12 sm6 md6>
                <v-text-field v-model="type" label="Ressourcen-Typ"></v-text-field>
                </v-flex>
                <v-flex xs12 sm6 md6>
                <v-text-field v-model="FunctionDescription" label="Funktionsbeschreibung"></v-text-field>
                </v-flex>
                <v-flex xs12 sm6 md6>
                <v-text-field v-model="SpecialDescription" label="Details"></v-text-field>
                </v-flex>
            </v-layout>
            </v-container>
        </v-card-text>

        <v-card-actions>
            <v-spacer></v-spacer>
            <v-btn color="red darken-1"  @click="close">Abbrechen</v-btn>
            <v-btn :disabled="formInvalid" color="green darken-1" @click="save">Speichern</v-btn>
        </v-card-actions>
        </v-card>
    </v-dialog>
</template>

<script lang="ts">

export default {
  props: {
    editedItem: Object,
    formTitle: String,
    showFormDialog: Boolean,
    hideButton: {
      type: Boolean,
      default: false
    }
  },
  data () {
    return{
      name: this.editedItem.name,
      type: this.editedItem.type,
      FunctionDescription: this.editedItem.FunctionDescription,
      SpecialDescription: this.editedItem.SpecialDescription,
      showDialog: this.showFormDialog
    }
  },
  computed: {
    formInvalid (): boolean {
      return !this.name
    }
  },
  watch: {
    showDialog (val) {
      if (!val) {
        this.$emit('close')
      }
    }
  },
  methods: {
    close () {
      this.showDialog = false
      this.$emit('close')
    },
    save () {
      const updateObject = {
        name: this.name,
        type: this.type,
        FunctionDescription: this.FunctionDescription,
        SpecialDescription: this.SpecialDescription
      }
      this.$emit('save', updateObject)
    }
  }
}
</script>
