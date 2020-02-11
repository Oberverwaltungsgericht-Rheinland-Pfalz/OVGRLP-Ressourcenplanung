import { UserData, ContactUser } from '.'

export interface UserState extends UserData {
  lastUpdated: Date | boolean,
  ContactUsers: ContactUser[]
}
