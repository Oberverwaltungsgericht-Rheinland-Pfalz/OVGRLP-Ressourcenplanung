import { Model } from '@vuex-orm/core'
import { Ressource, Gadget } from '.'
import { User } from './User'

export class Allocation extends Model {
  // This is the name used as module name of the Vuex Store.
  public static entity = 'allocations'
  public static primaryKey = 'Id'
  // List of all fields (schema) of the post model. `this.attr` is used
  // for the generic field type. The argument is the default value.
  public static fields () {
    return {
      Id: this.attr(null),
      ScheduleSeries: this.attr(null),
      From: this.attr(null),
      To: this.attr(null),
      Title: this.attr(null),
      Notes: this.attr(null),
      ContactPhone: this.attr(null),
      IsAllDay: this.attr(true),
      Status: this.attr(0),
      CreatedById: this.number(0),
      CreatedAt: this.attr(null),
      LastModified: this.attr(null),
      ApprovedById: this.attr(null),
      ApprovedAt: this.attr(null),
      // ReferencePerson: this.hasOne(User, 'Id', 'ReferencePersonId'),  // die user werden nicht am Anfang geladen!
      ReferencePersonId: this.number(0),
      RessourceIds: this.attr(null),
      Ressources: this.hasManyBy(Ressource, 'RessourceIds', 'Id'),
      GadgetsIds: this.attr(null),
      Gadgets: this.hasManyBy(Gadget, 'GadgetsIds', 'Id'),
      HintsForSuppliers: this.attr(null)
    }
  }

  Id!: number
  ScheduleSeries!: string
  From!: string
  To!: string
  Title!: string
  Notes!: string
  ContactPhone!: string
  IsAllDay!: boolean
  Status!: number
  CreatedById!: number
  CreatedAt!: string
  LastModified!: string
  ApprovedById!: number
  ApprovedAt!: string
  ReferencePersonId!: number
  RessourceIds!: Array<number>
  Ressources!: Array<Ressource>
  GadgetsIds!: Array<number>
  Gadgets!: Array<Gadget>
  HintsForSuppliers!: Array<WebApi.SimpleSupplierHint>
}
