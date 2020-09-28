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
    :disabled="readonly"
    clearable
  ></v-autocomplete>
</template>

<script lang="ts">
import { Component, Prop, Vue, Watch } from 'vue-property-decorator'
import { Gadget, Ressource, Supplier, Allocation } from '../../models'
import { ShowToast } from '../../models/interfaces'
import DropDownTimePicker from '@/components/DropdownTimePicker.vue'
import { getUser } from '../../services/UserApiService'

@Component({
  components: { DropDownTimePicker }
})
export default class InputReferencePerson extends Vue {
  @Prop(Number) private userid!: number
  @Prop(Boolean) private readonly!: boolean
  public entries: WebApi.AdUserViewModel[] = []
  public isLoading: number = 0
  public model: WebApi.AdUserViewModel = { Name: '', Email: '', ActiveDirectoryID: '', Phone: '' }
  public search: string = ''
  public lastLoad: string = '0'
  public requestCounter: number = 0

  public async mounted () {
    if (!this.userid) return

    const responseUser = await getUser(this.userid)
    if (responseUser != null) {
      this.search = responseUser.Name
      this.model.Name = responseUser.Name
      this.model.Email = responseUser.Email
      this.model.ActiveDirectoryID = responseUser.ActiveDirectoryID
      this.model.Phone = responseUser.Phone
    } else {
      this.$root.$emit('notify-user', { text: 'Benutzer konnte nicht geladen werden', color: 'error' } as ShowToast)
    }
  }

  public get isLoadingBool () {
    return this.isLoading ? 'blue' : false
  }

  @Watch('model')
  public userSet (val: WebApi.AdUserViewModel) {
    if (!val) {
      this.$emit('selected', { ActiveDirectoryID: '', Name: '', Email: '', Phone: '' } as WebApi.AdUserViewModel)
    } else {
      this.$emit('selected', val)
    }
  }

  @Watch('search')
  public async searchValueChanged (val: string, oldvalue: string) {
    // Lazily load input items
    if (!val || val.length < 4) return
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
      responseBody.forEach((e: WebApi.AdUserViewModel) => {
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
