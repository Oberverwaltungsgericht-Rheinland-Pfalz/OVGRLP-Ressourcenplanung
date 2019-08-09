import UserData from '../../models/UserData'
export { UserData }

export interface UserState extends UserData {
  lastUpdated: Date | boolean
}

// action & mutation names:
export const Names = {
  a: {
    loadUser: 'loadUser',
    reloadUser: 'reloadUser'
  },
  m: {
    setUser: 'setUser',
    clearUser: 'clearUser'
  }
}
