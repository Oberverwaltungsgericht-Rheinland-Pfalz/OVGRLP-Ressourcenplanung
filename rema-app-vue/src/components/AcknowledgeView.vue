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
              >{{ viewAllocation.PurposeTitle }} {{ viewAllocation.Id }}</v-col
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
              viewAllocation.CreatedAt | simpleDateTime
            }}</v-col>
            <v-col cols="3">
              <strong>Letzte Veränderung:</strong>
            </v-col>
            <v-col cols="3">{{
              viewAllocation.LastModified | simpleDateTime
            }}</v-col>

            <v-col cols="3">
              <strong>Ganztägiges Ereignis:</strong>
            </v-col>
            <v-col cols="3">{{ viewAllocation.IsAllDay | boolean2word }}</v-col>
            <v-col cols="3">
              <strong>Von:</strong>
            </v-col>
            <v-col cols="3">{{ viewAllocation.From | simpleDateTime }}</v-col>
            <v-col cols="3">
              <strong>Bis:</strong>
            </v-col>
            <v-col cols="3">{{ viewAllocation.To | simpleDateTime }}</v-col>

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
              <strong>Beschreibung:</strong>
            </v-col>
            <v-col cols="3">{{ viewAllocation.Description }}</v-col>
            <v-col cols="3">
              <strong>Notizen:</strong>
            </v-col>
            <v-col cols="3">{{ viewAllocation.Notices }}</v-col>
          </v-row>
          <v-row v-show="moveEdit">
            <v-col cols="12">
              <h2 :class="{ 'valid-range': timesInvalid }">Neuer Zeitraum</h2>
            </v-col>
            <v-col cols="3">
              <strong>Von:</strong>
            </v-col>
            <v-col cols="3">
              <input
                type="datetime-local"
                v-model="editFrom"
                :class="{ 'invalid-date': !editFrom }"
              />
            </v-col>
            <v-col cols="3">
              <strong>Bis:</strong>
            </v-col>
            <v-col cols="3">
              <input
                type="datetime-local"
                v-model="editTo"
                :class="{ 'invalid-date': !editTo }"
              />
            </v-col>
          </v-row>
        </v-container>
        <v-row v-if="hasCollisions">
          <v-col cols="12">
            <h2>Mögliche Kollisionen</h2>
          </v-col>
        </v-row>
        <v-row v-for="(i, idx) in possibleCollisions" v-bind:key="idx + 'cols'">
          <v-col cols="4"
            >{{ i.From | simpleDateTime }} - {{ i.To | simpleDateTime }}</v-col
          >
          <v-col cols="4">{{ i.Purpose.Title }}</v-col>
          <v-col cols="4">{{ i.Status | status2string }}</v-col>
        </v-row>
      </v-card-text>
      <v-card-actions>
        <div class="flex-grow-1"></div>
        <v-btn
          color="green darken-1"
          :disabled="moveEdit"
          text
          @click="acknowledge"
        >
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
import { Component, Prop, Vue } from 'vue-property-decorator'
import { Names as Fnn } from '../store/User/types'
import AllocationRequest from '../models/interfaces/AllocationRequest'
import AllocationRequestView from '../models/interfaces/AllocationRequestView'
import UserData from '../models/interfaces/UserData'
import ContactUser from '../models/interfaces/ContactUser'
import Allocation from '../models/Allocation'
import AllocationModel from '../models/interfaces/AllocationModel'
const namespace = 'user'

@Component
export default class AcknowledgeView extends Vue {
  @State('ContactUsers', { namespace })
  private ContactUsers!: ContactUser[]
  @Mutation(Fnn.m.addContactUser, { namespace })
  private addContactUser: any
  @Mutation(Fnn.m.reserveContactUser, { namespace })
  private reserveContactUser: any
  @Prop(Boolean) private readonly value!: boolean
  @Prop(Object) private viewAllocation!: AllocationRequestView // = {} as AllocationRequestView
  private moveEdit: boolean = false
  private editFrom: string = ''
  private editTo: string = ''

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
    // this.viewAllocation = {} as AllocationRequestView
  }
  public get UnAcknowledgedAllocations (): Allocation[] {
    return Allocation.query()
      .withAll()
      .get()
  }

  public get possibleCollisions (): Allocation[] {
    const start = Date.parse(this.viewAllocation.From)
    const end = Date.parse(this.viewAllocation.To)
    const id = this.viewAllocation.Id
    const rValues = Allocation.query()
      .withAll()
      .where('Status', (s: number) => s !== 2)
      // @ts-ignore
      .where('Ressource_id', this.viewAllocation.Ressource_id)
      .where((a: any) => {
        const aTo = Date.parse(a.To)
        const aFrom = Date.parse(a.From)
        const rVal =
          (a.Id !== id && aTo >= start && aTo <= end) ||
          (aFrom >= start && aFrom <= end) ||
          (aFrom <= start && aTo >= end)

        return rVal
      })
      .get()
    return rValues
  }
  public get hasCollisions () {
    return this.possibleCollisions.length > 0
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
      this.editFrom = this.viewAllocation.From
      this.editTo = this.viewAllocation.To
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
    console.dir(task)
    const editedRequest = { Id: task.Id, status, From: task.From, To: task.To }
    const response = await fetch(`/api/Allocations/EditRequest`, {
      method: 'PUT', // *GET, POST, PUT, DELETE, etc.
      mode: 'cors', // no-cors, cors, *same-origin
      cache: 'no-cache', // *default, no-cache, reload, force-cache, only-if-cached
      credentials: 'same-origin', // include, *same-origin, omit
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify(editedRequest)
    })
    Allocation.update({
      where: task.Id,
      data: { From: task.From, To: task.To, Status: status }
    })
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
