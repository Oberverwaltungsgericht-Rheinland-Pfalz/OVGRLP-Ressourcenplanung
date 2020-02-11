import { ActionTree } from 'vuex'
import { RootState, CounterState } from '@/models/interfaces'

export const actions: ActionTree<CounterState, RootState> = {
  increment ({ commit }): any {
    commit('incrementCounter')
  },
  reset ({ commit }): any {
    commit('resetCounter')
  }
}
