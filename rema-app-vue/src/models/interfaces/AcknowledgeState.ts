import { AllocationRequest } from './AllocationRequest'

export interface AcknowledgeState {
  tasks: AllocationRequest[],
  lastUpdated: Date | boolean
}
