<template>
  <v-dialog :value="value" persistent scrollable>
    <v-card>
      <v-card-title>
        <span class="headline">Anfrage bearbeiten</span>
      </v-card-title>
      <v-card-text>
        <v-container>
          <v-row>
            <v-col cols="3"><strong>Bezeichnung:</strong> </v-col>
            <v-col cols="3"
              ><v-btn text><v-icon v-html="'create'"/></v-btn
              >{{ viewAllocation.PurposeTitle }} {{ viewAllocation.Id }}</v-col
            >
            <v-col cols="3"><strong>Ganztägiges Ereignis:</strong> </v-col>
            <v-col cols="3"
              ><v-icon v-html="'create'" />{{
                viewAllocation.IsAllDay | boolean2word
              }}</v-col
            >
            <v-col cols="3"><strong>Von:</strong> </v-col>
            <v-col cols="3">{{ viewAllocation.From | simpleDateTime }}</v-col>
            <v-col cols="3"><strong>Bis:</strong> </v-col>
            <v-col cols="3">{{ viewAllocation.To | simpleDateTime }}</v-col>

            <v-col cols="3"><strong>Raum:</strong> </v-col>
            <v-col cols="3">{{ viewAllocation.RessourceTitle }}</v-col>

            <v-col cols="3"><strong>Ansprechpartner:</strong></v-col>
            <v-col cols="3">
              {{ contactUserName(viewAllocation.ReferencePerson) }}</v-col
            >
            <v-col cols="3"><strong>Telefonnummer:</strong></v-col>
            <v-col cols="3"> {{ viewAllocation.ContactTel }}</v-col>
            <v-col cols="3"><strong>Beschreibung:</strong></v-col>
            <v-col cols="3"> {{ viewAllocation.Description }}</v-col>
            <v-col cols="3"><strong>Notizen:</strong></v-col>
            <v-col cols="3"> {{ viewAllocation.Notices }}</v-col>

            <v-col cols="3"><strong>Status:</strong> </v-col>
            <v-col cols="3">{{ viewAllocation.Status | status2string }}</v-col>

            <v-col cols="3"><strong>Anfragedatum:</strong> </v-col>
            <v-col cols="3">{{
              viewAllocation.CreatedAt | simpleDateTime
            }}</v-col>
            <v-col cols="3"><strong>Letzte Veränderung:</strong> </v-col>
            <v-col cols="3">{{
              viewAllocation.LastModified | simpleDateTime
            }}</v-col>
          </v-row>
          <v-row v-show="moveEdit">
            <v-col cols="12"
              ><h2 :class="{ 'valid-range': timesInvalid }">
                Neuer Zeitraum
              </h2></v-col
            >
            <v-col cols="3"><strong>Von:</strong></v-col>
            <v-col cols="3"
              ><input
                type="datetime-local"
                v-model="editFrom"
                :class="{ 'invalid-date': !editFrom }"
            /></v-col>
            <v-col cols="3"><strong>Bis:</strong></v-col>
            <v-col cols="3"
              ><input
                type="datetime-local"
                v-model="editTo"
                :class="{ 'invalid-date': !editTo }"
            /></v-col>
          </v-row>
        </v-container>
        <v-row v-if="hasCollisions">
          <v-col cols="12"><h2>Mögliche Kollisionen</h2></v-col>
        </v-row>
        <v-row v-for="(i, idx) in possibleCollisions" v-bind:key="idx + 'cols'">
          <v-col cols="4"
            >{{ i.From | simpleDateTime }} - {{ i.To | simpleDateTime }}</v-col
          >
          <v-col cols="4"> {{ i.Purpose.Title }}</v-col>
          <v-col cols="4"> {{ i.Status | status2string }}</v-col>
        </v-row>
      </v-card-text>
      <v-card-actions>
        <div class="flex-grow-1"></div>
        <v-btn
          color="green darken-1"
          :disabled="moveEdit"
          text
          @click="acknowledge"
          ><v-icon v-html="'done'"></v-icon> Bestätigen</v-btn
        >
        <v-btn color="red darken-1" :disabled="moveEdit" text @click="reject"
          ><v-icon v-html="'close'"></v-icon> Ablehnen</v-btn
        >
        <v-btn
          color="gray darken-1"
          :disabled="timesInvalid && moveEdit"
          text
          @click="move"
          ><v-icon v-html="moveEdit ? 'done' : 'create'"></v-icon>
          Verschieben</v-btn
        >
        <v-btn color="orange darken-1" text @click="cancel"
          ><v-icon>close</v-icon> Abbrechen</v-btn
        >
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>

<script lang="ts">
import Gadgets from '../../models/GadgetModel'
import { State, Action, Getter, Mutation } from 'vuex-class'
import { Component, Prop, Vue } from 'vue-property-decorator'
import { Names as Fnn } from '../../store/User/types'
import Ressources, { RessourceModel } from '../../models/RessourceModel'
import Suppliers from '../../models/SupplierModel'
import AllocationRequest, {
  AllocationRequestView
} from '../../models/AllocationRequest'
import AllocationPurposes, {
  AllocationPurposeModel
} from '../../models/AllocationpurposeModel'
import Allocations, { AllocationModel } from '../../models/AllocationModel'
import UserData, { ContactUser } from '../../models/UserData'
const namespace = 'user'

@Component
export default class AllocationEdit extends Vue {
  @Prop(Boolean) private readonly value!: boolean
  @Prop(Object) private viewAllocation!: AllocationRequestView // = {} as AllocationRequestView
  @State('ContactUsers', { namespace })
  private ContactUsers!: ContactUser[]

  public contactUserName (id: number): string {
    // acts as filter, because it causes errors as filter
    return (
      this.ContactUsers.find((w: ContactUser) => w.Id === id) || { Title: '' }
    ).Title
  }
}
</script>
