<template>
  <v-autocomplete
    v-model="model"
    :items="entries"
    :loading="isLoadingBool"
    :search-input.sync="search"
    color="black"
    item-text="Name"
    item-value="ActiveDirectoryID"
    label="AnsprechpartnerIn"
    no-data-text="Sie müssen mindestens 4 Buchstaben eingeben"
    placeholder="Bitte Namen teilweise eintippen"
    prepend-icon="mdi-database-search"
    :return-object="true"
  ></v-autocomplete>
</template>

<script lang="ts">
import { Component, Prop, Vue, Watch } from 'vue-property-decorator'
import { Gadget, Ressource, Supplier, Allocation } from '../../models'
import DropDownTimePicker from '@/components/DropdownTimePicker.vue'
import { RessourceModel, AllocationModel, AdUsers } from '../../models/interfaces'

@Component({
  components: { DropDownTimePicker }
})
export default class InputReferencePerson extends Vue {
  @Prop(Number) private userid!: number
  public entries: AdUsers[] = []
  public isLoading: number = 0
  public model: AdUsers = { Name: '', Email: '', ActiveDirectoryID: '', Phone: '' }
  public search: string = ''
  public lastLoad: string = '0'
  public requestCounter: number = 0

  public async mounted () {
    if (!this.userid) return
    const response = await fetch(`/api/Users/${this.userid}`)
    let responseValues = await response.json() as AdUsers
    this.search = responseValues.Name
    this.model.Name = responseValues.Name
    this.model.Email = responseValues.Email
    this.model.ActiveDirectoryID = responseValues.ActiveDirectoryID
    this.model.Phone = responseValues.Phone
  }

  public get isLoadingBool () {
    return this.isLoading ? 'blue' : false
  }

  @Watch('model')
  public userSet (val: AdUsers) {
    this.$emit('selected', val)
  }

  @Watch('search')
  public async searchValueChanged (val: string, oldvalue: string) {
    let loadStart = Date.now()
    // Lazily load input items
    if (!val) return
    if (val.length < 4) return
    if (val[val.length - 1] === ')') return

    setTimeout(this.fetchAdUsers, 400, ++this.requestCounter)
  }

  public async fetchAdUsers (counter: number) {
    let timestamp = Date.now() + ''
    if (counter < this.requestCounter) return

    this.lastLoad = timestamp

    try {
      this.isLoading++
      const response = await fetch(`/api/Users/adUser/${this.search}`, { headers: { timestamp } })
      timestamp = response.headers.get('timestamp') || ''
      const responseBody = await response.json()
      if (!responseBody) return
      if (this.lastLoad > timestamp) return
      this.entries.splice(0, Infinity)
      responseBody.forEach((e: AdUsers) => {
        this.entries.push(e)
      })
    } catch (ex) {
      console.log(ex)
    } finally {
      this.isLoading--
    }
  }
}
</script>
