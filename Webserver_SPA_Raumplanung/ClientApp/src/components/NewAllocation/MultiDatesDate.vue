<template>
    <v-layout flex justify-left>
    <v-flex xs12 sm8>
      <v-card>
        <v-card-title class="cyan darken-1">
          <span class="headline white--text">{{purpose.title}}</span>

          <v-spacer></v-spacer>

          <v-btn dark icon title="cancel">
            <v-icon>close</v-icon>
          </v-btn>

          <v-btn dark icon @click="purposeEditable = !purposeEditable">
            <v-icon>edit</v-icon>
          </v-btn>

          <v-btn dark icon :disabled="!saveable" @click="save">
            <v-icon>save</v-icon>
          </v-btn>
        </v-card-title>

        <v-list>
          <v-list-item>
            <v-list-item-action>
              <v-icon>storage</v-icon>
            </v-list-item-action>

            <v-list-item-content>
              <v-list-item-title v-if="!purposeEditable">{{purpose.description}}</v-list-item-title>
              <v-list-item-title v-else><input class="optionated-input" type="text" v-model="purpose.description"/></v-list-item-title>
            </v-list-item-content>
          </v-list-item>

          <v-divider inset></v-divider>

          <v-list-item>
            <v-list-item-action>
              <v-icon>edit</v-icon>
            </v-list-item-action>

            <v-list-item-content>
              <v-list-item-title v-if="!purposeEditable">{{purpose.notes}}</v-list-item-title>
              <v-list-item-title v-else><input class="optionated-input" type="text" v-model="purpose.notes"/></v-list-item-title>
            </v-list-item-content>

            <v-list-item-action>
              <v-icon>edit</v-icon>
            </v-list-item-action>
          </v-list-item>

          <v-divider inset></v-divider>

          <v-list-item>
            <v-list-item-action>
              <v-icon>location_on</v-icon>
            </v-list-item-action>

            <v-list-item-content>
              <v-list-item-title v-if="!purposeEditable">{{purpose.ressource}}</v-list-item-title>
              <v-list-item-title v-else><input class="optionated-input" type="text" v-model="purpose.ressource"/></v-list-item-title>
            </v-list-item-content>

            <v-list-item-action>
              <v-icon>edit</v-icon>
            </v-list-item-action>
          </v-list-item>
        </v-list>
      </v-card>
    </v-flex>
    <v-flex  xs12 sm4>
      <v-card>
        <v-list subheader>
          <v-subheader>Gewünschte Termine</v-subheader>

          <v-list-item
            v-for="(item, idx) in appointments" :key="`${item}+${idx}`">
            <v-list-item-avatar>
              <v-list-item-action-text>{{idx}}</v-list-item-action-text>
            </v-list-item-avatar>

            <v-list-item-content>
              <v-list-item-title>{{item | toIso}}</v-list-item-title>
            </v-list-item-content>

            <v-list-item-action>
              <v-icon :color="'grey'">delete</v-icon>
            </v-list-item-action>
          </v-list-item>
        </v-list>
      </v-card>
      </v-flex>
  </v-layout>
</template>

<script lang="ts">
import dayjs from 'dayjs'
import { Component, Prop, Vue, Watch } from 'vue-property-decorator'

export default class MultiDatesDate extends Vue {
/*  initialPurpose: {
    default: () => ({ title: 'Initialer Grund', notes: 'Notizen',
    description: 'Beschreibung', ressource: 'Ressource' })
*/
  @Prop(Array) private readonly initialAppointments = []
  @Prop({
    default: () => ({
      title: 'Initialer Grund',
      notes: 'Notizen',
      description: 'Beschreibung',
      ressource: 'Ressource'
    })
  })
  private readonly initialPurpose!: ShortPurpose
  private purpose: ShortPurpose = { title: 'Titel', description: 'Beschreibung', notes: 'Notizen', ressource: 'Raum1' }
  private appointments: any[] = []
  private purposeEditable: boolean = false
  private saveable: boolean = false

  private save () {
    // send save request
    this.purposeEditable = this.saveable = false
  }
  private mounted () {
    this.appointments = this.initialAppointments
    this.purpose.title = this.initialPurpose.title
    this.purpose.notes = this.initialPurpose.notes
    this.purpose.description = this.initialPurpose.description
    this.purpose.ressource = this.initialPurpose.ressource
  }
}
interface ShortPurpose {
  title: string
  description: string
  notes: string
  ressource: string
}
</script>
<style lang="stylus" scoped>

.optionated-input
  box-shadow: inset 1px 0px 2px 0px black
  padding 2px 
  width 100%
</style>
