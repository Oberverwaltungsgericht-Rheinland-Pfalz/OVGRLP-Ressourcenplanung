<template>
  <v-layout id="all-list" column>
    <v-data-table
      v-if="hasItems"
      :headers="headers"
      :items="Requests"
      :search="search"
      sort-by="From"
    >
      <template v-slot:top>
        <v-toolbar flat color="white">
          <label class="blue-icon">
            <input hidden type="checkbox" v-model="hideOld" />
            Vergangene Termine &ensp;<v-icon v-if="!hideOld">visibility</v-icon>
            <v-icon v-else>visibility_off</v-icon>
          </label>
          <v-divider class="mx-4" inset vertical></v-divider>
          <label class="blue-icon">
            <input hidden type="checkbox" v-model="hideNotMine" />
            Zeige nur meine Termine &ensp;<v-icon v-if="!hideNotMine">check_box_outline_blank</v-icon>
            <v-icon v-else>check_box</v-icon>
          </label>
          <v-spacer/>
          <v-text-field
            v-model="search"
            append-icon="search"
            label="Filter"
            single-line
            hide-details
          />
          <v-spacer/>
          <v-select
              v-model="selectedGroup"
              :items="GroupItems"
              item-text="Title"
              return-object
              clearable
              placeholder=""
              label="Hilfsmittel-Abteilung"
              :menu-props="{ offsetY: true }"
              class="select-group"
            />
        </v-toolbar>
      </template>
      <template v-slot:item="props">
        <tr @click.stop="rowClicked(props.item.Id)" :class="{'mark-today': props.item.From.startsWith(today)}">
          <td class="text-start">
            <v-icon v-if="permissionToEdit" @click.stop="confirmDelete(props.item)">delete</v-icon>&emsp;
            <v-icon @click.stop="printItem(props.item)">print</v-icon>&emsp;
            <v-icon v-if="permissionToEdit" @click.stop="editItem(props.item.Id)">edit</v-icon>
          </td>
          <td class="text-start">{{props.item.Title}}
            <v-chip v-show="selectedGroup" v-for="gadget in showGadgets(props.item)" :key="'gadget'+props.item.Id+gadget" class="ma-2" color="yellow">
              {{gadget}}</v-chip>
          </td>
          <td class="text-start">{{props.item.Status}}</td>
          <td class="text-start">{{props.item.Ressource}}</td>
          <td class="text-start">{{props.item.From | toLocal}}</td>
          <td class="text-start">{{props.item.To | toLocal}}</td>
          <td class="text-start">{{props.item.LastModified | toLocal}}</td>
        </tr>
      </template>
    </v-data-table>
    <h3 v-else>Es liegen keine Terminanfragen von Ihnen vor</h3>

    <edit-form-modal v-if="displayView" @close="displayView=false" show readonly :eventId="displayId" :key="'readonlyAlloc'">
    <span/></edit-form-modal>
    <edit-form-modal v-if="selectedOpen !== -1" show :eventId="selectedOpen" :key="'editAlloc'"
      @updateview="selectedOpen = -1"><span/>
    </edit-form-modal>
  </v-layout>
</template>

<script lang="ts">
import { Component, Prop, Vue } from 'vue-property-decorator'
import { Allocation, Gadget, Supplier } from '../models'
import { SelectableGroup, ShowToast, ConfirmData } from '../models/interfaces'
import { deleteAllocation } from '../services/AllocationApiService'
import EditFormModal from './EditFormModal.vue'
import moment from 'moment'
import print from 'print-js'

@Component({
  components: { EditFormModal }
})
export default class AllList extends Vue {
  private displayView: boolean = false
  private displayId: number=0
  private selectedGroup: SelectableGroup | null = null
  private search: string = ''
  private hideOld: boolean = true
  private hideNotMine: boolean = false
  private selectedOpen: number = -1
  private headers: object[] = [
    { text: 'Bearbeiten', value: 'action', sortable: false },
    { text: 'Bezeichnung', value: 'Title' },
    { text: 'Status', value: 'Status' },
    { text: 'Raum', value: 'Ressource' },
    { text: 'Von', value: 'From' },
    { text: 'Bis', value: 'To' },
    { text: 'Zuletzt geändert', value: 'LastModified' }
  ];

  private today: string = moment().format('YYYY-MM-DD')
  public showGadgets (item: any): string[] {
    let selectedGroupId = this.selectedGroup?.Id || 0
    let rVal: string[] = []
    item.Gadgets.filter((e: any) => e.SuppliedBy === selectedGroupId).forEach((e: any) => rVal.push(e.Title))
    item.HintsForSuppliers.filter((e:any) => e.GroupId === selectedGroupId).forEach((e: any) => rVal.push(e.Message))
    return rVal
  }
  public get hasItems () {
    const allocations = Allocation.query()
      .get()
    return allocations.length
  }
  public get Requests (): VisibleAllocation[] {
    let myId = this.$store.state.user.id

    const allocations = Allocation.query().withAll()
      .where((al: any) => (!this.hideNotMine && (al.Status === 1 || al.Status === 3)) || al.CreatedById === myId || al.ReferencePersonId === myId)
      .where('To', (value: any) => (!this.hideOld || Date.parse(value) > Date.now()))
      .where('GadgetsIds', (values: number[]) => (!this.selectedGroup || values.length > 0))
      .where('GadgetsIds', (values: number[]) => !this.selectedGroup || (this.selectedGroup && this.selectedGroup.gadgetIds.some((e: number) => values.includes(e))))
      .orderBy('From')
      .get()

    if (!allocations.length) return []

    return allocations.map((v: any) => ({
      ...v,
      // @ts-ignore
      Status: this.$options.filters.status2string(v.Status),
      Ressource: (v.Ressource || { Name: '' }).Name
    }))
  }
  public get GroupItems (): object[] {
    var groups = new Map()

    Allocation.query().withAll().get().forEach((e:any) => e.GadgetsIds.forEach((f: number) => {
      let groupId = (Gadget.find(f) as any).SuppliedBy
      let group = Supplier.find(groupId) as any
      if (!groups.has(groupId)) {
        let filterObj: SelectableGroup = { Id: group.Id, Title: (group.Title || ''), gadgetIds: [f] }
        groups.set(groupId, filterObj)
      } else {
        let filterObj = groups.get(groupId)
        filterObj.gadgetIds.push(f)
      }
    }))
    let rawValues = [...groups.values()]
    let returnValues = rawValues.map((e: SelectableGroup) => ({ Id: e.Id, Title: e.Title, gadgetIds: [...new Set(e.gadgetIds)] }))
    return returnValues
  }

  private confirmDelete (item: VisibleAllocation) {
    let data: ConfirmData = { title: 'Löschen bestätigen',
      content: `Möchten sie diese Buchung ${item.Title} wirklich löschen?`,
      callback: this.deleteItem,
      id: item.Id
    }
    this.$root.$emit('user-confirm', data)
  }
  private async deleteItem (id: number) {
    let success = await deleteAllocation(id)

    if (success) this.$root.$emit('notify-user', { text: 'Löschung erfolgreich', color: 'success' } as ShowToast)
    else this.$root.$emit('notify-user', { text: 'Löschen fehlgeschlagen', color: 'error' } as ShowToast)
  }
  private async printItem (item: VisibleAllocation) {
    print('api/Allocations/print/' + item.Id)
  }

  public editItem (id: number) {
    this.selectedOpen = -1
    this.$nextTick(() => {
      this.selectedOpen = id
    })
  }
  private rowClicked (id: number) {
    this.displayView = false
    this.displayId = id
    this.$nextTick(() => {
      this.displayView = true
    })
  }
}

interface VisibleAllocation {
  Id: number;
  Title: string;
  Status: string;
  Ressource: string;
  From: string;
  PurposeId: number;
  LastModified: string;
}
</script>

<style scoped lang="stylus">
.appointment-open-list:nth-of-type(2n) {
  background-color: lightgrey;
}

.blue-icon i
  color #82b1ff !important
</style>

<style lang="stylus">
#all-list
  .mark-today
    background-color lightblue
  .select-group .v-input__slot
    margin-bottom 0
  .select-group .v-text-field__details
    display none
</style>
