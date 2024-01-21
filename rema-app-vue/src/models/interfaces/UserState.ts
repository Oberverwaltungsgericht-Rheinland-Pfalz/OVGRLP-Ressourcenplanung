import { UserData } from './UserData'

export interface UserState extends UserData {
  lastUpdated: Date | boolean,
  ContactUsers: WebApi.ContactUser[],
  isRequestable: boolean,
  calendarFrom: number,
  hideCalendarFrom: boolean
}
