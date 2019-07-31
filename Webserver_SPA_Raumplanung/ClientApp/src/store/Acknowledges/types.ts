import AllocationRequest from '../../models/AllocationRequest'
// export {default as AllocationRequest}  from '../../models/AllocationRequest'
export { AllocationRequest}

export interface AcknowledgeState {
  tasks: AllocationRequest[],
  lastUpdated: Date | boolean
}

export const Names = {
  a: {
    loadTasks: 'loadTasks',
    updateTask: 'updateTask'
  },
  m: {
    updateTask: 'updateTask',
    loadTasks: 'loadTasks',
    addTask: 'addTask'
  }
}
