import { Model } from '@vuex-orm/core'
import AllocationModel from './AllocationModel'

export default class Ressource extends Model {
  // This is the name used as module name of the Vuex Store.
  public static entity = 'ressources'

  // List of all fields (schema) of the post model. `this.attr` is used
  // for the generic field type. The argument is the default value.
  public static fields () {
    return {
      id: this.attr(null),
      Title: this.attr(''),
      Type: this.attr(''),
      FunctionDescription: this.attr(''),
      SpecialDescription: this.attr(''),
      Allocations: this.hasMany(AllocationModel, 'ressource_id')
    }
  }
}
