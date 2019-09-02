import { Model } from '@vuex-orm/core'
import Ressource from './RessourceModel'
import Allocationpurpose from './AllocationpurposeModel'

export default class Allocation extends Model {
  // This is the name used as module name of the Vuex Store.
  public static entity = 'allocations'
  public static primaryKey = 'Id'
  // List of all fields (schema) of the post model. `this.attr` is used
  // for the generic field type. The argument is the default value.
  public static fields (): any {
    return {
      Id: this.attr(null),
      From: this.attr(null),
      To: this.attr(null),
      IsAllDay: this.attr(true),
      Status: this.attr(0),
      CreatedBy: this.attr(null),
      CreatedAt: this.attr(null),
      LastModified: this.attr(null),
      LastModifiedBy: this.attr(''),
      ApprovedBy: this.attr(null),
      ApprovedAt: this.attr(null),
      ReferencePerson: this.attr(''),
      Ressource_id: this.attr(null),
      Ressource: this.belongsTo(Ressource, 'Ressource_id', 'Id'),
      Purpose: this.belongsTo(Allocationpurpose, 'Purpose_id', 'Id'),
      Purpose_id: this.attr(null)
    }
  }
}

export interface AllocationModel {
  Id: number
  From: string
  To: string
  IsAllDay: string
  Status: number
  CreatedBy: string
  CreatedAt: string
  LastModified: string
  LastModifiedBy: string
  ApprovedBy: string
  ApprovedAt: string
  ReferencePerson: string
}
