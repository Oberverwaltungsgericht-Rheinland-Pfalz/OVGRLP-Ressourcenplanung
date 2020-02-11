import { Model } from '@vuex-orm/core'
import { Ressource, Gadget } from '.'

export class Allocation extends Model {
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
      Title: this.attr(null),
      IsAllDay: this.attr(true),
      Status: this.attr(0),
      CreatedBy: this.attr(null),
      CreatedAt: this.attr(null),
      LastModified: this.attr(null),
      LastModifiedBy: this.attr(''),
      ApprovedBy: this.attr(null),
      ApprovedAt: this.attr(null),
      ReferencePerson: this.attr(''),
      RessourceId: this.attr(null),
      Ressource: this.belongsTo(Ressource, 'RessourceId', 'Id'),
      GadgetsIds: this.attr(null),
      Gadgets: this.hasManyBy(Gadget, 'GadgetsIds', 'Id')
    }
  }
}