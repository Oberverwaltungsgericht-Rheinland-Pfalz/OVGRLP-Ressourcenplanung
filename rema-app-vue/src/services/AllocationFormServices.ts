
import { Component, Vue } from 'vue-property-decorator'
import { RessourceModel, AllocationModel, AdUsers, HintsForSuppliers } from '../models/interfaces'
import { Gadget, Ressource, Supplier, Allocation } from '../models'

@Component
export default class AllocationFormService extends Vue {
  public referencePerson: AdUsers = { ActiveDirectoryID: '', Name: '', Email: '', Phone: '' }
  public telNumber: string = ''
  public groupTextsInternal: string[] = []
  public refreshInputReferencePerson: number = 0

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
