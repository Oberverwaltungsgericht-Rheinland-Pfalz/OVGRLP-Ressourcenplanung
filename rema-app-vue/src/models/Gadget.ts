import { Model } from '@vuex-orm/core'
import { Supplier } from './Supplier'

export class Gadget extends Model {
  // This is the name used as module name of the Vuex Store.
  public static entity = 'gadgets'
  public static primaryKey = 'Id'

  // List of all fields (schema) of the post model. `this.attr` is used
  // for the generic field type. The argument is the default value.
  public static fields () {
    return {
      Id: this.attr(null),
      Title: this.attr(''),
      SuppliedBy: this.attr(null),
      Supplier: this.hasOne(Supplier, 'Id')
    }
  }

  Id!: number
  Title!: string
  SuppliedBy!: number
  Supplier!: Supplier
}
