import { UserData } from '.'

export interface UserState extends UserData {
  lastUpdated: Date | boolean,
  ContactUsers: WebApi.ContactUser[],
  isRequestable: boolean,
  calendarFrom: number
}
