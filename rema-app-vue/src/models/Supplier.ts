import { Model } from '@vuex-orm/core'
import { Gadget } from '.'

export class Supplier extends Model {
  // This is the name used as module name of the Vuex Store.
  public static entity = 'SupplierGroups'
  public static primaryKey = 'Id'

  // List of all fields (schema) of the post model. `this.attr` is used
  // for the generic field type. The argument is the default value.
  public static fields () {
    return {
      Id: this.attr(null),
      Title: this.attr(''),
      GroupEmail: this.attr(''),
      Gadgets: this.hasMany(Gadget, 'SuppliedBy', 'Id')
    }
  }
}
