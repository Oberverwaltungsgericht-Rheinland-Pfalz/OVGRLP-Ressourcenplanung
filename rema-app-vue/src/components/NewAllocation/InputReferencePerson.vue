<template>
  <v-autocomplete
    v-model="model"
    :items="entries"
    :loading="isLoading"
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
import DateTimePicker from '@/components/DateTimePicker.vue'
import { warn } from 'vue-class-component/lib/util'

@Component({
  components: {
    DateTimePicker, DropDownTimePicker
  }
})
export default class InputReferencePerson extends Vue {
  @Prop(Number) private userid!: number
  public entries: AdUsers[] = []
  public isLoading: boolean = false
  public model: AdUsers = { Name: '', Email: '', ActiveDirectoryID: '' }
  public search: string = ''
  public lastLoad: string = '0'

  public async mounted () {
    const response = await fetch(`/api/Users/${this.userid}`)
    let responseValues = await response.json() as AdUsers
    this.search = responseValues.Name
    this.model.Name = responseValues.Name
    this.model.Email = responseValues.Email
    this.model.ActiveDirectoryID = responseValues.ActiveDirectoryID
  }

  @Watch('model')
  public userSet (val: AdUsers) {
    this.$emit('selected', val)
  }

  @Watch('search')
  public searchValueChanged (val: string, oldvalue: string) {
    let loadStart = Date.now()
    // Lazily load input items
    if (!val) return
    if (val.length < 4) return
    if (val[val.length - 1] === ')') return
    let timestamp = Date.now() + ''
    fetch(`/api/Users/adUser/${this.search}`, { headers: { timestamp } })
      .then(response => {
        timestamp = response.headers.get('timestamp') || ''
        return response.json()
      })
      .then(res => {
        if (!res) return
        if (this.lastLoad > timestamp) return
        while (this.entries.length) this.entries.pop()
        res.forEach((e: AdUsers) => {
          this.entries.push(e)
        })
      })
      .catch(err => {
        console.log(err)
      })
      .finally(() => (this.isLoading = false))
  }
}
</script>
