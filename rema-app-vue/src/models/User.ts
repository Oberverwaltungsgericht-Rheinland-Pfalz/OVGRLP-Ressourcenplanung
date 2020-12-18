import { Model } from '@vuex-orm/core'

export class User extends Model {
  // This is the name used as module name of the Vuex Store.
  public static entity = 'users'
  public static primaryKey = 'Id'

  public static fields () {
    return {
      Id: this.attr(null),
      Name: this.attr(''),
      ActiveDirectoryID: this.attr(''),
      Organisation: this.attr(''),
      Email: this.attr('')
    }
  }

  Id!: number
  Name!: string
  ActiveDirectoryID!: number
  Organisation!: string
  Email!: string
}
