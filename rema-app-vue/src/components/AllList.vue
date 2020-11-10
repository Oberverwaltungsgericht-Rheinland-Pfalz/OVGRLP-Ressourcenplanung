<template>
  <v-layout id="all-list" column>
    <v-sheet height="64">
      <v-toolbar flat color="white">
        <v-checkbox v-model="hideOld" label="Vergangene Termine" on-icon="visibility_off" off-icon="visibility_on" class="pad-top"></v-checkbox>
        <v-divider class="mx-4" inset vertical></v-divider>
        <v-checkbox v-model="hideNotMine" label="Zeige nur meine Termine" on-icon="check_box" off-icon="check_box_outline_blank" class="pad-top"></v-checkbox>

        <v-spacer/>

        <v-tooltip bottom>
          <template v-slot:activator="{ on, attrs }">
            <v-text-field
              v-bind="attrs" v-on="on"
              v-model="search"
              append-icon="search"
              label="Bezeichnungsfilter"
              single-line
              hide-details
            />
          </template>
          <span>Schränkt die Ansicht auf Termine ein, deren Namen die hier eingetippten Buchstaben enthalten</span>
        </v-tooltip>

        <v-spacer/>

        <v-tooltip bottom>
          <template v-slot:activator="{ on, attrs }">
            <div class="select-wrap-width" v-bind="attrs" v-on="on">
              <v-select
                v-model="selectedGroup"
                :items="GroupItems"
                item-text="Title"
                return-object
                clearable
                placeholder=""
                label="Unterstützergruppe auswählen"
                :menu-props="{ offsetY: true }"
                class="select-group"
              />
            </div>
          </template>
          <span>Zeige nur Termine welche für eine Unterstützergruppe zu beachten sind</span>
        </v-tooltip>
      </v-toolbar>
    </v-sheet>
    <v-data-table
      v-if="hasItems"
      :headers="headers"
      :items="Requests"
      :search="search"
      sort-by="From"
    >
      <template v-slot:item="props">
        <tr @click.stop="rowClicked(props.item.Id)" :class="{'mark-today': props.item.From.startsWith(today)}">
          <td class="text-start fit-cell">
            <v-icon v-if="permissionToEdit" @click.stop="confirmDelete(props.item)" title="Termin löschen" class="pad-right">delete</v-icon>&emsp;
            <v-icon @click.stop="printItem(props.item)" title="Termin drucken" class="pad-right">print</v-icon>&emsp;
            <v-icon v-if="permissionToEdit" @click.stop="editItem(props.item.Id)" title="Termin bearbeiten">edit</v-icon>
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

    // Search only if a supplier-group is selected for filtering
    if (this.selectedGroup) {
      let existingIds = allocations.map((e: any) => e.Id)
      let allocations2 = Allocation.query().withAll()
        .where((al: any) => al.HintsForSuppliers.length > 0 && !existingIds.includes(al.Id) && ((!this.hideNotMine && (al.Status === 1 || al.Status === 3)) || al.CreatedById === myId || al.ReferencePersonId === myId))
        .where('To', (value: any) => (!this.hideOld || Date.parse(value) > Date.now()))
        .where('HintsForSuppliers', (values: WebApi.SimpleSupplierHint[]) => values.some((e: WebApi.SimpleSupplierHint) => e.GroupId === this.selectedGroup?.Id))
        .get()
      allocations.push(...allocations2)
      allocations.sort((a: any, b: any) => Number(a.From > b.From) - 1)
    }

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
.icon-button
  cursor pointer
</style>

<style lang="stylus">
.v-input--selection-controls__input
    display -webkit-inline-box !important

#all-list
  .mark-today
    background-color lightblue
  .select-group .v-input__slot
    margin-bottom 0
  .select-group .v-text-field__details
    display none
  .pad-right
    padding-right .5em
  .pad-top
    padding-top 2em
  .select-wrap-width
    min-width 25%
    max-width 100%
.fit-cell
  white-space nowrap
  width 1%
</style>
