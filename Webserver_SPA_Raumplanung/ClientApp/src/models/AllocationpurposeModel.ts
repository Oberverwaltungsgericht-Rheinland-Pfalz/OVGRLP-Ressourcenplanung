import { Model } from '@vuex-orm/core'
import Allocation from './AllocationModel'
import Gadget from './GadgetModel'

export default class Allocationpurpose extends Model {
  // This is the name used as module name of the Vuex Store.
  public static entity = 'AllocationPurposes'

  // List of all fields (schema) of the post model. `this.attr` is used
  // for the generic field type. The argument is the default value.
  public static fields () {
    return {
      id: this.attr(null),
      title: this.attr(''),
      description: this.attr(''),
      notes: this.attr(''),
      contactPhone: this.attr(''),
      Allocations: this.hasMany(Allocation, 'Purpose_id'),
      Gadget_ids: this.attr(null),
      Gadgets: this.hasManyBy(Gadget, 'Gadget_ids')
    }
  }
}
