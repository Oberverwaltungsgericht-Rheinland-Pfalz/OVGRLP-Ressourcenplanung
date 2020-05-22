<template>
  <v-dialog :value="value" max-width="900px" persistent scrollable>
    <v-card>
      <v-card-title>
        <span class="headline">Anfrage bearbeiten</span>
      </v-card-title>
      <v-card-text>
        <v-container>
          <v-row>
            <v-col cols="3">
              <strong>Bezeichnung:</strong>
            </v-col>
            <v-col cols="3"
              >{{ viewAllocation.Title }} {{ viewAllocation.Id }}</v-col
            >
            <v-col cols="3">
              <strong>Status:</strong>
            </v-col>
            <v-col cols="3">{{ viewAllocation.Status | status2string }}</v-col>
            <v-col cols="3">
              <strong>Raum:</strong>
            </v-col>
            <v-col cols="3">{{ viewAllocation.RessourceTitle }}</v-col>
            <v-col cols="3">
              <strong>Anfragedatum:</strong>
            </v-col>
            <v-col cols="3">{{
              viewAllocation.CreatedAt | toLocal
            }}</v-col>
            <v-col cols="3">
              <strong>Letzte Veränderung:</strong>
            </v-col>
            <v-col cols="3">{{
              viewAllocation.LastModified | toLocal
            }}</v-col>

            <v-col cols="3">
              <strong>Ganztägiges Ereignis:</strong>
            </v-col>
            <v-col cols="3">{{ viewAllocation.IsAllDay | boolean2word }}</v-col>
            <v-col cols="3">
              <strong>Von:</strong>
            </v-col>
            <v-col cols="3">{{ viewAllocation.From | toLocal }}</v-col>
            <v-col cols="3">
              <strong>Bis:</strong>
            </v-col>
            <v-col cols="3">{{ viewAllocation.To | toLocal }}</v-col>

            <v-col cols="3">
              <strong>Ansprechpartner:</strong>
            </v-col>
            <v-col cols="3">{{
              contactUserName(viewAllocation.ReferencePerson)
            }}</v-col>
            <v-col cols="3">
              <strong>Telefonnummer:</strong>
            </v-col>
            <v-col cols="3">{{ viewAllocation.ContactTel }}</v-col>
            <v-col cols="3">
              <strong>Notizen:</strong>
            </v-col>
            <v-col cols="3">{{ viewAllocation.Notices }}</v-col>
          </v-row>
          <v-row v-if="moveEdit">
            <v-col cols="12">
              <h2 :class="{ 'valid-range': timesInvalid }">Neuer Zeitraum</h2>
            </v-col>
            <v-col cols="3">
              <v-menu ref="fromMenu" :close-on-content-click="true" transition="scale-transition" offset-y max-width="290px" min-width="290px"
                v-model="fromMenu">
                <template v-slot:activator="{ on }">
                  <v-text-field persistent-hint prepend-icon="event" v-on="on" readonly
                  label="Von"
                  :value="dateFormatted(editFrom)"
                  ></v-text-field>
                </template>
                <v-date-picker v-model="editFrom" :min="dateMin" locale="de" no-title @input="fromMenu = false">
                  <v-btn text color="primary" @click="fromMenu = false" block>Abbrechen</v-btn>
                </v-date-picker>
              </v-menu>
            </v-col>
            <v-col cols="3">
              <drop-down-time-picker v-model="editFromTime"/>
            </v-col>
            <v-col cols="3">
              <v-menu ref="toMenu" :close-on-content-click="true" transition="scale-transition" offset-y max-width="290px" min-width="290px"
                v-model="toMenu">
                <template v-slot:activator="{ on }">
                  <v-text-field persistent-hint prepend-icon="event" v-on="on" readonly
                  label="Bis"
                  :value="dateFormatted(editTo)"
                  ></v-text-field>
                </template>
                <v-date-picker v-model="editTo" :min="dateMin" locale="de" no-title @input="toMenu = false">
                  <v-btn text color="primary" @click="toMenu = false" block>Abbrechen</v-btn>
                </v-date-picker>
              </v-menu>
            </v-col>
            <v-col cols="3">
              <drop-down-time-picker v-model="editToTime"/>
            </v-col>
          </v-row>
        </v-container>
        <collision-detection :viewAllocation="viewAllocation"/>
      </v-card-text>
      <v-card-actions>
        <div class="flex-grow-1"></div>
        <v-btn color="green darken-1" :disabled="moveEdit" text @click="acknowledge">
          <v-icon v-html="'done'"></v-icon>Bestätigen
        </v-btn>
        <v-btn color="red darken-1" :disabled="moveEdit" text @click="reject">
          <v-icon v-html="'close'"></v-icon>Ablehnen
        </v-btn>
        <v-btn
          color="gray darken-1"
          :disabled="timesInvalid && moveEdit"
          text
          @click="move"
        >
          <v-icon v-html="moveEdit ? 'done' : 'create'"></v-icon>Verschieben
        </v-btn>
        <v-btn color="orange darken-1" text @click="cancel">
          <v-icon>close</v-icon>Abbrechen
        </v-btn>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>

<script lang="ts">
import { State, Action, Getter, Mutation } from 'vuex-class'
import { Component, Prop, Vue, Mixins } from 'vue-property-decorator'
import { Names as Fnn } from '../store/User/types'
import DropDownTimePicker from '@/components/DropdownTimePicker.vue'
import AllocationFormService from '@/services/AllocationFormServices'
import { AllocationRequest, AllocationRequestView, UserData, ContactUser, AllocationModel } from '../models/interfaces'
import { Allocation } from '../models'
import { editAllocationStatus, refreshAllocations } from '../services/AllocationApiService'
import CollisionDetection from './CollisionDetection.vue'
const namespace = 'user'

@Component({ components: { DropDownTimePicker, CollisionDetection } })
export default class AcknowledgeView extends Vue {
  @State('ContactUsers', { namespace })
  private ContactUsers!: ContactUser[]
  @Mutation(Fnn.m.addContactUser, { namespace })
  private addContactUser: any
  @Mutation(Fnn.m.reserveContactUser, { namespace })
  private reserveContactUser: any
  @Prop(Boolean) private readonly value!: boolean
  @Prop(Object) private viewAllocation!: AllocationRequestView
  private moveEdit: boolean = false
  private editFrom: string = ''
  private editFromTime: string = ''
  private editTo: string = ''
  private editToTime: string = ''

  public get timesInvalid (): boolean {
    if (!this.editFrom || !this.editTo) return true
    return Date.parse(this.editFrom) > Date.parse(this.editTo)
  }

  public cancel () {
    if (this.moveEdit) {
      this.moveEdit = false
      return
    }
    this.$emit('input', false)
  }
  public get UnAcknowledgedAllocations (): Allocation[] {
    return Allocation.query()
      .withAll()
      .get()
  }

  public contactUserName (id: number): string {
    // acts as filter, because it causes errors as filter
    return (
      this.ContactUsers.find((w: ContactUser) => w.Id === id) || { Title: '' }
    ).Title
  }
  public async acknowledge () {
    this.saveStatus(this.viewAllocation, 1)
  }
  public reject () {
    // change appointment status to rejected
    this.saveStatus(this.viewAllocation, 2)
  }
  public async move () {
    this.moveEdit = !this.moveEdit

    if (this.moveEdit) {
      this.editFrom = this.viewAllocation.From.substr(0, 10)
      this.editFromTime = this.viewAllocation.From.substr(11, 5)
      this.editTo = this.viewAllocation.To.substr(0, 10)
      this.editToTime = this.viewAllocation.To.substr(11, 5)
      return
    }
    const changedAllocation: AllocationModel = {
      ...this.viewAllocation,
      From: this.editFrom,
      To: this.editTo
    }
    this.saveStatus(changedAllocation, 3)
  }

  public async saveStatus (task: AllocationModel, status: number) {
    let success = editAllocationStatus(task.Id, status, task.From, task.To)
    if (success) this.$dialog.message.success('Bearbeitung erfolgreich', { position: 'center-left' })
    else this.$dialog.error({ text: 'Bearbeitung konnte nicht gespeichert werden', title: 'Fehler' })

    /* Allocation.update({
      where: task.Id,
      data: { From: task.From, To: task.To, Status: status }
    }) */
    await refreshAllocations()
    this.$emit('input', false)
  }
}
</script>

<style scoped lang="stylus">
.invalid-date {
  border: 3px solid red;
}

input[type='datetime-local'] {
  border-bottom: 1px solid black;
}

.valid-range {
  color: red;
}
</style>
