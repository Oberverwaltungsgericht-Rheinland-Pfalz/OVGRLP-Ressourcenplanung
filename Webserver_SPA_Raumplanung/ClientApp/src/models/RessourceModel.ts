import { Model } from '@vuex-orm/core'

export default class Ressource extends Model {
  // This is the name used as module name of the Vuex Store.
  public static entity = 'ressource'

  // List of all fields (schema) of the post model. `this.attr` is used
  // for the generic field type. The argument is the default value.
  public static fields() {
    return {
      id: this.attr(null),
      Title: this.attr(''),
      Type: this.attr(''),
      FunctionDescription: this.attr(''),
      SpecialDescription: this.attr('')
    }
  }
}
