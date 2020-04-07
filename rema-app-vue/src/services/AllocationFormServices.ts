
import { Component, Vue, Prop, Watch } from 'vue-property-decorator'
import { RessourceModel, AllocationModel, AdUsers, HintsForSuppliers } from '../models/interfaces'
import { Gadget, Ressource, Supplier, Allocation } from '../models'
import moment from 'moment'

@Component
export default class AllocationFormService extends Vue {
  public referencePerson: AdUsers = { ActiveDirectoryID: '', Name: '', Email: '', Phone: '' }
  public telNumber: string = ''
  public groupTextsInternal: string[] = []
  public refreshInputReferencePerson: number = 0
  public dateFrom: string = ''
  public dateTo: string = ''
  public fromMenu: boolean = false
  public toMenu: boolean = false
  public timeFrom: string = '08:00'
  public timeTo: string = '17:00'

  @Watch('dateFrom')
  @Watch('dateTo')
  public dateFromChange (val: string) {
    if (!this.dateTo || this.dateTo.length === 0) {
      this.dateTo = val
    }
    if (this.dateFrom > this.dateTo) {
      this.dateTo = this.dateFrom
    }
    if (this.dateFrom === this.dateTo && (this.timeFrom > this.timeTo)) {
      if (this.timeTo.endsWith('59')) this.timeFrom = this.timeFrom.split(':')[0] + ':45'
      else this.timeTo = this.timeTo.split(':')[0] + parseInt(this.timeTo.split(':')[1]) + 1
    }
  }

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
  public setReferencePerson (e:AdUsers) {
    this.referencePerson = e
    this.telNumber = this.referencePerson.Phone
  }

  public get Rooms () {
    function compareNumbers (a: any, b: any) {
      return (a.Name > b.Name) ? 1 : -1
    }
    return Ressource.all().sort(compareNumbers)
  }
  public dateFormatted (dateValue: string): string {
    if (!dateValue) return ''
    if (dateValue) return moment(dateValue).format('DD.MM.YYYY')
    else return moment().format('DD.MM.YYYY')
  }

  public get GadgetGroups () {
    return Supplier.all()
  }
  public get groupTexts () : string[] {
    if (!this.groupTextsInternal.length) {
      this.GadgetGroups.forEach((g:any) => {
        this.groupTextsInternal[g.Id] = ''
      })
    }
    return this.groupTextsInternal
  }
  public set groupTexts (input: string[]) {
    this.groupTextsInternal.splice(0, Infinity, ...input)
  }
  public get GetHintsForSuppliers (): HintsForSuppliers[] {
    let rVal: HintsForSuppliers[] = []
    for (let key in this.groupTexts) {
      let value = this.groupTexts[key]
      if (!value) continue
      let newHint: HintsForSuppliers = { GroupId: parseInt(key), Message: this.groupTexts[key] }
      rVal.push(newHint)
    }
    return rVal
  }
}
