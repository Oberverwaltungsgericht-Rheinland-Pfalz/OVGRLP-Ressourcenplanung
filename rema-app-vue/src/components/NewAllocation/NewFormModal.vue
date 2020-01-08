<template>
  <v-dialog v-model="dialog" scrollable>
    <template v-slot:activator="{ on }">
      <v-btn color="success" dark v-on="on">
        <v-icon>add</v-icon>Termin anlegen
      </v-btn>
    </template>

    <AllocationForm v-on:close="dialog=false"></AllocationForm>
  </v-dialog>
</template>

<script lang="ts">
import { Component, Prop, Vue } from 'vue-property-decorator'
import AllocationForm from './AllocationForm.vue'

@Component({
  components: { AllocationForm }
})
export default class NewFormModal extends Vue {
  public dialog: boolean = false;
  public get title () {
    return 'Neue Terminanfrage stellen / Termin eintragen'
  }
}
</script>
