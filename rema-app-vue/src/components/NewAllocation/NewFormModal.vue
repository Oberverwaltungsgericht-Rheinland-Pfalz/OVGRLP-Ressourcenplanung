<template>
  <v-dialog v-model="dialog" scrollable persistent max-width="1200px">
    <template v-slot:activator="{ on }">
      <v-btn v-show="permissionToEdit || requestsAllowed" color="success" dark v-on="on">
        <span v-if="permissionToEdit"><v-icon>add</v-icon>Termin anlegen</span>
        <span v-else-if="requestsAllowed"><v-icon>add</v-icon>Termin anfragen</span>
      </v-btn>
    </template>

    <AllocationForm v-if="dialog" v-on:close="dialog=false"></AllocationForm>
  </v-dialog>
</template>

<script lang="ts">
import { Component, Prop, Vue } from 'vue-property-decorator'
import AllocationForm from './AllocationForm.vue'
import { State, Action, Getter, Mutation } from 'vuex-class'

@Component({
  components: { AllocationForm }
})
export default class NewFormModal extends Vue {
  @State('isRequestable', { namespace: 'user' })
  private requestsAllowed!: boolean

  public dialog: boolean = false;
  public get title () {
    return 'Neue Terminanfrage stellen / Termin eintragen'
  }
}
</script>
