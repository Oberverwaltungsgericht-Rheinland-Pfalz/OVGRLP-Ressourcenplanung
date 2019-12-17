import { MutationTree } from 'vuex'
import { AcknowledgeState, AllocationRequest, Names } from './types'
import { stat } from 'fs'

export const mutations: MutationTree<AcknowledgeState> = {
  [Names.m.updateTask] (state, payload: AllocationRequest) {
    const task = state.tasks.find((el, idx) => el.id === payload.id)
    if (!task) { throw new Error('task id not found') }
    task.Status = payload.Status
    task.DateTime = payload.DateTime
  },
  [Names.m.loadTasks] (state, tasks: AllocationRequest[]) {
    while (state.tasks.length) state.tasks.pop()
    tasks.forEach((el) => state.tasks.push(el))
    state.lastUpdated = new Date()
  },
  [Names.m.addTask] (state, task: AllocationRequest) {
    state.tasks.push(task)
  }
}
