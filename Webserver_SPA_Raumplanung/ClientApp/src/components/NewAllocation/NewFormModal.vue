<template>
    <v-dialog v-model="dialog" persistent >
        <template v-slot:activator="{ on }">
        <v-btn color="primary" dark v-on="on"><v-icon>add</v-icon> Termin anlegen</v-btn>
        </template>
        <v-card>
        <v-card-title class="headline">{{title}}</v-card-title>
        <v-card-text><AllocationForm/></v-card-text>
        <v-card-actions>
            <div class="flex-grow-1"></div>
            <v-btn color="green darken-1" text @click="dialog = false"><v-icon>save</v-icon> Anfragen</v-btn>
            <v-btn color="green darken-1" text @click="dialog = false"><v-icon>save</v-icon> Speichern</v-btn>
            <v-btn color="red darken-1" text @click="dialog = false"><v-icon>cancel</v-icon> Abbrechen</v-btn>
        </v-card-actions>
        </v-card>
    </v-dialog>
</template>

<script lang="ts">
import { Component, Prop, Vue } from 'vue-property-decorator'
import AllocationForm from './AllocationForm.vue'

@Component({
  components: { AllocationForm }
})
export default class NewFormModal extends Vue {
  public dialog: boolean = false
  public get title () {
    return 'Neue Terminanfrage stellen / Termin eintragen'
  }
}
</script>