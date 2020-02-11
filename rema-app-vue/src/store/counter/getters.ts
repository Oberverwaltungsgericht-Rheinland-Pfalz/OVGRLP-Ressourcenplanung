import { GetterTree } from 'vuex'
import { RootState, CounterState } from '@/models/interfaces'

export const getters: GetterTree<CounterState, RootState> = {
  currentCount (state): number {
    return state.counter
  }
}
