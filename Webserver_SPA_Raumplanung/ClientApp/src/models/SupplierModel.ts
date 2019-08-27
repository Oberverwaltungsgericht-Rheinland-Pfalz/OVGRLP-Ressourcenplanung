import { Model } from '@vuex-orm/core'
import Gadget from './GadgetModel'

export default class Supplier extends Model {
  // This is the name used as module name of the Vuex Store.
  public static entity = 'SupplierGroups'

  // List of all fields (schema) of the post model. `this.attr` is used
  // for the generic field type. The argument is the default value.
  public static fields () {
    return {
      id: this.attr(null),
      title: this.attr(''),
      groupEmail: this.attr(''),
      Gadgets: this.hasMany(Gadget, 'suppliedBy')
    }
  }
}

export interface SupplierGroupModel {
  id: string
  title: string
  groupEmail: string
}
