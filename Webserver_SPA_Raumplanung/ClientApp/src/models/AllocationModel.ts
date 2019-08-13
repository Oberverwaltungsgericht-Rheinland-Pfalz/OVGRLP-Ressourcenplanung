import { Model } from '@vuex-orm/core'
import Ressource from './RessourceModel'
import Allocationpurpose from './AllocationpurposeModel'

export default class Allocation extends Model {
  // This is the name used as module name of the Vuex Store.
  public static entity = 'allocation'

  // List of all fields (schema) of the post model. `this.attr` is used
  // for the generic field type. The argument is the default value.
  public static fields () {
    return {
      id: this.attr(null),
      Start: this.attr(null),
      End: this.attr(null),
      IsAllDay: this.attr(true),
      Status: this.attr(''),
      CreatedBy: this.attr(null),
      CreatedAt: this.attr(null),
      LastModified: this.attr(null),
      LastModifiedBy: this.attr(''),
      ApprovedBy: this.attr(null),
      ApprovedAt: this.attr(null),
      ReferencePerson: this.attr(''),
      Ressource_id: this.attr(null),
      Ressource: this.belongsTo(Ressource, 'Ressource_id'),
      Purpose: this.belongsTo(Allocationpurpose, 'Purpose_id'),
      Purpose_id: this.attr(null)
    }
  }
}
