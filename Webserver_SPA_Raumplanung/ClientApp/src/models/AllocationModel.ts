import { Model } from '@vuex-orm/core'

export default class Allocation extends Model {
  // This is the name used as module name of the Vuex Store.
  public static entity = 'allocation'

  // List of all fields (schema) of the post model. `this.attr` is used
  // for the generic field type. The argument is the default value.
  public static fields () {
    return {
      id: this.attr(null),
      Title: this.attr(''),
      Start: this.attr(''),
      End: this.attr(''),
      Notes: this.attr(''),
      Ressource_id: this.attr(null),
      Purpose_id: this.attr(null)
    }
  }
}
