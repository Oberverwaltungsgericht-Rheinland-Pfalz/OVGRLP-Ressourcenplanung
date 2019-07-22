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

          <v-btn dark icon @click="purposeEditable =!purposeEditable">
            <v-icon>edit</v-icon>
          </v-btn>

          <v-btn dark icon :disabled="!saveable" @click="save">
            <v-icon>save</v-icon>
          </v-btn>
        </v-card-title>

        <v-list>
          <v-list-tile>
            <v-list-tile-action>
              <v-icon>storage</v-icon>
            </v-list-tile-action>

            <v-list-tile-content>
              <v-list-tile-title v-if="!purposeEditable">{{purpose.description}}</v-list-tile-title>
              <v-list-tile-title v-else><input class="optionated-input" type="text" v-model="purpose.description"/></v-list-tile-title>
            </v-list-tile-content>
          </v-list-tile>

          <v-divider inset></v-divider>

          <v-list-tile>
            <v-list-tile-action>
              <v-icon>edit</v-icon>
            </v-list-tile-action>

            <v-list-tile-content>
              <v-list-tile-title v-if="!purposeEditable">{{purpose.notes}}</v-list-tile-title>
              <v-list-tile-title v-else><input class="optionated-input" type="text" v-model="purpose.notes"/></v-list-tile-title>
            </v-list-tile-content>

            <v-list-tile-action>
              <v-icon>edit</v-icon>
            </v-list-tile-action>
          </v-list-tile>

          <v-divider inset></v-divider>

          <v-list-tile>
            <v-list-tile-action>
              <v-icon>location_on</v-icon>
            </v-list-tile-action>

            <v-list-tile-content>
              <v-list-tile-title v-if="!purposeEditable">{{purpose.ressource}}</v-list-tile-title>
              <v-list-tile-title v-else><input class="optionated-input" type="text" v-model="purpose.ressource"/></v-list-tile-title>
            </v-list-tile-content>

            <v-list-tile-action>
              <v-icon>edit</v-icon>
            </v-list-tile-action>
          </v-list-tile>
        </v-list>
      </v-card>
    </v-flex>
    <v-flex  xs12 sm4>
      <v-card>
        <v-list subheader>
          <v-subheader>Gewünschte Termine</v-subheader>

          <v-list-tile
            v-for="(item, idx) in appointments" :key="`${item}+${idx}`">
            <v-list-tile-avatar>
              <v-list-tile-action-text>{{idx}}</v-list-tile-action-text>
            </v-list-tile-avatar>

            <v-list-tile-content>
              <v-list-tile-title>{{item | toIso}}</v-list-tile-title>
            </v-list-tile-content>

            <v-list-tile-action>
              <v-icon :color="'grey'">delete</v-icon>
            </v-list-tile-action>
          </v-list-tile>
        </v-list>
      </v-card>
      </v-flex>
  </v-layout>
</template>

<script lang="ts">
import { Component, Prop, Vue } from 'vue-property-decorator'
import dayjs from 'dayjs'

@Component({
    filters: {
        toIso(date: string) {
            return dayjs(date).format('DD.MM.YYYY')
        }
    }
})
export default class MultiDatesDate extends Vue {
    @Prop({type: Array, default: () => ['2019-07-30']}) public readonly initialAppointments: any[]
    @Prop({type: Object}) public readonly initialPurpose: any
    private purpose: any = {title: 'Titel', description: 'Beschreibung', notes: 'Notizen', ressource: 'Raum1'}
    private appointments: any = []
    private purposeEditable = false
    private saveable = false

    public save() {
        // send save request
        this.purposeEditable = this.saveable = false
    }

    public mounted() {
        this.appointments = this.initialAppointments;
        ({this.purpose.title, this.purpose.notes} = this.initialPurpose)
    }
}
public interface ShortPurpose {
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
