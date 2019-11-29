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
                <v-text-field v-model="lol" type="text" label="Bezeichnung"></v-text-field>
                </v-flex>
                <v-flex xs12 sm6 md6>
                <v-text-field v-model="RessourceType" disabled="true" value="Raum" label="Ressourcen-Typ"></v-text-field>
                </v-flex>
                <v-flex xs12 sm6 md6>
                <v-text-field v-model="FunctionDescription" label="Funktionsbeschreibung"></v-text-field>
                </v-flex>
                <v-flex xs12 sm6 md6>
                <v-text-field v-model="SpecialsDescription" label="Details"></v-text-field>
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
import { Component, Prop, Vue, Watch } from 'vue-property-decorator'
import { Boolean } from '@vuex-orm/core'

@Component
export default class RessourceFormDialog extends Vue {
  @Prop(Boolean) private readonly showFormDialog!: boolean
  @Prop(Object) private readonly editedItem!: PropEditItem
  @Prop({ default: '' }) private readonly formTitle!: string
  @Prop({ default: false }) private readonly hideButton!: boolean

  private lol: string = this.editedItem.Title
  private RessourceType: string = this.editedItem.Type
  private FunctionDescription: string = this.editedItem.FunctionDescription
  private SpecialDescription: string = this.editedItem.SpecialDescription
  private showDialog: boolean = this.showFormDialog

  private get formInvalid (): boolean {
    return !this.lol
  }

  @Watch('showDialog')
  public showDialogChange (val: boolean) {
    if (!val) this.$emit('close')
  }
  private close (): void {
    this.showDialog = false
    this.$emit('close')
  }
  private save (): void {
    const updateObject: PropEditItem = {
      Title: this.lol,
      Type: this.RessourceType,
      FunctionDescription: this.FunctionDescription,
      SpecialDescription: this.SpecialDescription
    }
    this.$emit('save', updateObject)
  }
}
interface PropEditItem {
  Title: string,
  Type: string,
  FunctionDescription: string,
  SpecialDescription: string
}
</script>
