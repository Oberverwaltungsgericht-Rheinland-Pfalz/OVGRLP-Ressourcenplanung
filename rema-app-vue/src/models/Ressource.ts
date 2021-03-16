import { Attr, Attribute, Fields, HasMany, Model } from '@vuex-orm/core'
import { Allocation } from '.'

export class Ressource extends Model {
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
      IsDeactivated: this.attr(false),
      FunctionDescription: this.attr(''),
      SpecialsDescription: this.attr('')
      // allocations: this.hasMany(Allocation, 'Ressource_id') // überflüssig to delete
    }
  }

  Id!: number
  Name!: string
  Type!: string
  IsDeactivated!: boolean
  FunctionDescription!: string
  SpecialsDescription!: string
  allocations!: []
}
