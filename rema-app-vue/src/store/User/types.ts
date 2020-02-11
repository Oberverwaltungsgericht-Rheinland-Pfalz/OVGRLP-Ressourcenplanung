import UserData from '../../models/interfaces/UserData'
import ContactUser from '../../models/interfaces/ContactUser'

export interface UserState extends UserData {
  lastUpdated: Date | boolean,
  ContactUsers: ContactUser[]
}

// action & mutation names:
export const Names = {
  a: {
    loadUser: 'loadUser',
    reloadUser: 'reloadUser'
  },
  m: {
    setUser: 'setUser',
    clearUser: 'clearUser',
    addContactUser: 'addContactUser',
    reserveContactUser: 'reserveContactUser'
  }
}
