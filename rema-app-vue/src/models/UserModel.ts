import { Model } from '@vuex-orm/core'

export default class Gadget extends Model {
  // This is the name used as module name of the Vuex Store.
  public static entity = 'users'
  public static primaryKey = 'Id'

  // List of all fields (schema) of the post model. `this.attr` is used
  // for the generic field type. The argument is the default value.
  public static fields () {
    return {
      Id: this.attr(null),
      Name: this.attr(null),
      Email: this.attr(null),
      Role: this.attr(0),
      RoleNames: this.attr(null),
      organisation: this.attr(null),
      SupplierGroups: this.attr(null)
    }
  }
}
