import { ActionTree } from 'vuex'
import axios from 'axios'
import { AcknowledgeState, AllocationRequest, Names } from './types'
import { RootState } from '../types'

const exampleTasks: AllocationRequest[] = [
  {id: 2, Title: 'Meeting', Status: 'offen', Description: 'Raum1', DateTime: new Date()},
  {id: 3, Title: 'Meeting2', Status: 'offen', Description: 'Raum2', DateTime: new Date()}
]

export const actions: ActionTree<AcknowledgeState, RootState> = {
  [Names.a.loadTasks]({commit}): any {

    // todo: load from server
    const tasks: AllocationRequest[] = exampleTasks

    commit(Names.m.loadTasks, tasks)
  },
  [Names.a.updateTask]({commit}, task): any {
    // todo: send updated task to server, update state

    commit(Names.m.updateTask, task)
  }
}
