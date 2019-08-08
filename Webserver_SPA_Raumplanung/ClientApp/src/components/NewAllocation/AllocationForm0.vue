<template>
  <v-container fluid grid-list-xl>

  <v-form v-model="valid">
    <v-container grid-list-xl>
      <v-layout wrap>
        <v-flex xs12 md6>
          <v-text-field
            v-model="Title"
            :rules="nameRules"
            :counter="15"
            label="Title"
            required
          ></v-text-field>
        </v-flex>

        <v-flex xs12 md6>
          <v-text-field
            v-model="lastname"
            :rules="nameRules"
            :counter="10"
            label="Ansprechpartner/In"
            required
          ></v-text-field>
        </v-flex>

        <v-flex xs12 md6>
          <v-text-field
            v-model="email"
            :rules="emailRules"
            type="tel"
            label="Telefonnummer"
            required
          ></v-text-field>
        </v-flex>
              <v-flex xs12 sm6 d-flex>
        <v-textarea
            v-model="Description"
            :label="'Notizen'"
            auto-grow
            clearable
            outlined
            rounded
        ></v-textarea>
      </v-flex>
{{Description}}
<textarea v-model="Description"></textarea>

<input type="datetime-local" id="meeting-time"
       name="meeting-time" value="2018-06-12T19:30"
       min="2018-06-07T00:00" max="2018-06-14T00:00">
      </v-layout>
    </v-container>
  </v-form>

 
    <v-layout wrap justify-space-around align-center>
        <v-text-field
            v-model="Title"
            :rules="[rules.required]"
            label="Title"
            type="text"
        />

        <v-select
          v-model="selectedRessource"
          :items="Rooms"
          item-text="Title"
          item-value="id"
          label="Raum"
        />{{selectedRessource}}

     

    <v-switch
      v-model="isWholeDay"
      :label="`Ganztägiges Ereignis`"
    ></v-switch>{{isWholeDay}}

      <v-flex xs12 sm6 d-flex>
        <v-text-field
            v-model="telNumber"
            :rules="[rules.required]"
            label="Telefon"
            type="tel"
        ></v-text-field>
      </v-flex>

      <v-flex xs12 sm6 d-flex>
        <v-text-field
            v-model="contactPerson"
            :rules="[rules.required]"
            label="Kontaktperson"
            type="text"
        ></v-text-field>
      </v-flex>

      <v-flex xs12 sm6 d-flex>
        <v-text-field
            v-model="Email"
            :rules="[rules.email]"
            label="Email addresse"
            type="email"
        ></v-text-field>
      </v-flex>

      <v-flex xs12 sm6 d-flex>
        <v-textarea
            v-model="Description"
            placeholder="'Notizen'"
            auto-grow
            clearable
            outlined
            rounded
        ></v-textarea>
      </v-flex>
{{Description}}
<textarea v-model="Description"></textarea>

    </v-layout>
  </v-container>
</template>

<script lang="ts">
import { Component, Prop, Vue, Watch } from 'vue-property-decorator'
import Gadgets from '../../models/GadgetModel'
import Ressources from '../../models/RessourceModel'
import Suppliers from '../../models/SupplierModel'

export default class AllocationForm extends Vue {

  public isWholeDay: boolean = true
  public valid: boolean = false
  public firstname: string = ''
  public lastname: string = ''
  public nameRules = [
    (v: string) => !!v || 'Name is required',
    (v: string) => v.length <= 10 || 'Name must be less than 10 characters'
  ]
  public email: string = ''
  public emailRules = [
    (v: string) => !!v || 'E-mail is required',
    (v: string) => /.+@.+/.test(v) || 'E-mail must be valid'
  ]

  private Title: string = ''
  private Description: string = ''
  private Email: string = ''
  private rules: object = {
    email: (v: string) => (v || '').match(/@/) || 'Please enter a valid email',
    password: (v: string) => (v || '').match(/^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*(_|[^\w])).+$/) ||
          'Password must contain an upper case letter, a numeric character, and a special character',
    required: (v: string) => !!v || 'This field is required'
  }
  private selectedRessource: number = 0
  private telNumber: string = ''
  private contactPerson: string = ''
  private pickerFrom: boolean = false
  private pickerTo: boolean = false

  private dateFrom: string = new Date().toISOString().substr(0, 10)
  private dateTo: string = new Date().toISOString().substr(0, 10)

  private get Rooms () {
    return Ressources.query().where('Title', (v: string) => v.length).get()
  }
}
</script>

