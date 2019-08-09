<template>
    <v-card>
        <v-card-title class="align-end fill-height">Neuen Termin anlegen</v-card-title>
        <v-card-text>
            <allocation-form/>
        </v-card-text>
    <v-card-actions>
      <v-btn text color="green">
        Speichern
      </v-btn>
      <v-btn text color="orange">
        Abbrechen
      </v-btn>
    </v-card-actions>
    </v-card>
</template>


<script lang="ts">
import Vue from 'vue'
import Component from 'vue-class-component'
import AllocationForm from './AllocationForm.vue'

@Component({
  components: { AllocationForm }
})
export default class FormDialog extends Vue {

}
</script>
