import { Model } from '@vuex-orm/core'
import Allocation from './Allocation'

export default class Ressource extends Model {
  // This is the name used as module name of the Vuex Store.
  public static entity = 'ressources'
  public static primaryKey = 'Id'

  // List of all fields (schema) of the post model. `this.attr` is used
  // for the generic field type. The argument is the default value.
  public static fields () {
    return {
      Id: this.attr(null),
      Name: this.attr(''),
      Type: this.attr('Raum'),
      FunctionDescription: this.attr(''),
      SpecialsDescription: this.attr(''),
      allocations: this.hasMany(Allocation, 'Ressource_id')
    }
  }
}
export interface RessourceModel {
  Id: number
  Name: string
  Type: string
  FunctionDescription: string
  SpecialsDescription: string
}
