
import { Component, Vue, Watch } from 'vue-property-decorator'
import { Gadget, Ressource, Supplier } from '../models'
import { ConfirmData, ShowToast } from '../models/interfaces'
import moment from 'moment'

@Component
export default class AllocationFormService extends Vue {
  public referencePerson: WebApi.AdUserViewModel = { ActiveDirectoryID: '', Name: '', Email: '', Phone: '' }
  public telNumber: string = ''
  public ressourceIds: Array<number> = []
  public groupTextsInternal: string[] = []
  public refreshInputReferencePerson: number = 0
  public dateFrom: string = ''
  public dateTo: string = ''
  public fromMenu: boolean = false
  public toMenu: boolean = false
  public timeFrom: string = '08:00'
  public timeTo: string = '17:00'
  public checkForm: boolean = false
  public hasCollisions: boolean = false

  @Watch('dateFrom')
  @Watch('dateTo')
  public dateFromChange (val: string) {
    if (!this.dateTo || this.dateTo.length === 0) {
      this.dateTo = val
    }
    if (this.dateFrom > this.dateTo) {
      this.dateTo = this.dateFrom
    }
    if (this.dateFrom === this.dateTo && (this.timeFrom > this.timeTo)) { // geht nicht
      if (this.timeTo.endsWith('59')) this.timeFrom = this.timeFrom.split(':')[0] + ':45'
      else this.timeTo = this.timeFrom.split(':')[0] + ':' + (parseInt(this.timeTo.split(':')[1]) + 1)
    }
  }

  // abstract
  public get formInvalid ():boolean { return true }
  public get dateMin () {
    return moment().format('YYYY-MM-DD')
  }
  public get dateToMin () : string {
    if (this.dateFrom > this.dateMin) return this.dateFrom
    return this.dateMin
  }
  public get timeToMin () : string {
    if (this.dateFrom === this.dateTo) return this.timeFrom
    return '00:00'
  }
  public setReferencePerson (e: WebApi.AdUserViewModel) {
    this.referencePerson = e
    this.telNumber = this.referencePerson.Phone
  }

  public get Rooms (): Array<Ressource> {
    return Ressource.all().sort(compareNumbers)
  }
  public get RoomsActivated (): Array<Ressource> {
    return Ressource.query().where('IsDeactivated', false).get().sort(compareNumbers)
  }

  public saveDateWithWarning (callbackFn: Function) {
    let data: ConfirmData = { title: 'Mögliche Doppelbuchung',
      content: `Möchten sie den Termin trotz möglicher Doppelbelegung wirklich speichern?`,
      callback: callbackFn,
      id: 0
    }
    this.$root.$emit('user-confirm', data)
  }

  public isFormInvalid (): boolean {
    this.checkForm = true
    if (this.formInvalid) {
      this.$root.$emit('notify-user', { text: 'Bitte füllen sie alle Pflichtfelder richtig aus', color: 'warning', center: true } as ShowToast)
      return true
    }
    return false
  }

  public dateFormatted (dateValue: string): string {
    if (!dateValue) return ''
    if (dateValue) return moment(dateValue).format('DD.MM.YYYY')
    else return moment().format('DD.MM.YYYY')
  }

  public get GadgetGroups (): Array<Supplier> {
    return Supplier.all()
  }
  private getGadgets (groupId: number) {
    return Gadget.query()
      .where('SuppliedBy', groupId)
      .get()
  }

  public get groupTexts () : string[] {
    if (!this.groupTextsInternal.length) {
      this.GadgetGroups.forEach((g: Supplier) => {
        this.groupTextsInternal[g.Id] = ''
      })
    }
    return this.groupTextsInternal
  }
  public set groupTexts (input: string[]) {
    this.groupTextsInternal.splice(0, Infinity, ...input)
  }

  public get GetHintsForSuppliers (): WebApi.SimpleSupplierHint[] {
    let rVal: WebApi.SimpleSupplierHint[] = []
    for (let key in this.groupTexts) {
      let value = this.groupTexts[key]
      if (!value) continue
      let newHint: WebApi.SimpleSupplierHint = {
        GroupId: parseInt(key),
        Message: this.groupTexts[key]
      }
      rVal.push(newHint)
    }
    return rVal
  }
}

let compareNumbers = (a: Ressource, b: Ressource) => (a.Name > b.Name) ? 1 : -1
