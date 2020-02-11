import { GetterTree } from 'vuex'
import { UserData, UserState, RootState } from '@/models/interfaces'

export const getters: GetterTree<UserState, RootState> = {
  isLoadingDone (state: UserState): boolean {
    return !!state.lastUpdated
  },
  getUserData (state: any): UserData {
    const returnObject: any = {}
    for (const key in state as UserData) {
      if (state.hasOwnProperty(key)) returnObject[key] = state[key]
    }
    return returnObject as UserData
  }
}
