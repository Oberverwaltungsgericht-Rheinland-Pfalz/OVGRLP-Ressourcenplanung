import { ShowToast } from '@/models/interfaces'
import { Allocation } from '../models/Allocation'

export async function editAllocationStatus (editedRequest: WebApi.AllocationRequestEdition, errorCallback?: Function) : Promise<boolean> {
  try {
    const response = await fetch(`/api/allocations/editRequest`, {
      method: 'PUT',
      mode: 'cors',
      cache: 'no-cache',
      credentials: 'same-origin',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify(editedRequest)
    })
    checkMailErrorCallback(response, errorCallback)

    return response.ok
  } catch (ex) {
    return false
  }
}

export async function submitAllocations (newAllocations: WebApi.MultipleAllocationsViewModel, errorCallback?: Function): Promise<boolean> {
  try {
    const response = await fetch(`/api/Allocations/PostAllocations`, {
      method: 'POST',
      mode: 'cors',
      cache: 'no-cache',
      credentials: 'same-origin',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify(newAllocations)
    })
    checkMailErrorCallback(response, errorCallback)

    return response.ok
  } catch (ex) {
    return false
  }
}

export async function submitAllocation (newAllocation: WebApi.AllocationViewModel, errorCallback?: Function): Promise<boolean> {
  try {
    let response = await Allocation.api().post('allocations', newAllocation)
    checkMailErrorCallback(response, errorCallback)

    if (response.response.status > 300 && response.response.status < 200) return false
    else return true
  } catch (ex) {
    return false
  }
}

export async function refreshAllocations () : Promise<boolean> {
  try {
    let response = await Allocation.api().get('allocations')

    if (response.response.status > 300 && response.response.status < 200) return false
    else return true
  } catch (ex) {
    return false
  }
}

export async function deleteAllocation (id: number, errorCallback?: Function) : Promise<boolean> {
  try {
    const response = await Allocation.api().delete(
      `allocations/${id}`,
      { delete: id }
    )
    checkMailErrorCallback(response, errorCallback)

    if (response.response.status > 300 && response.response.status < 200) return false
    else return true
  } catch (ex) {
    console.dir(ex)
    return false
  }
}

export async function editAllocation (allocation: WebApi.AllocationViewModel, errorCallback?: Function) : Promise<boolean> {
  try {
    const response = await Allocation.api().put(`allocations/edit/${allocation.Id}`, allocation)
    checkMailErrorCallback(response, errorCallback)

    if (response.response.status > 300 && response.response.status < 200) return false
    else return true
  } catch (ex) {
    return false
  }
}

export function errorCallbackFactory (instance: any): Function {
  return (text: string) => {
    instance.$root.$emit('notify-user', { text: text, center: true, timeout: 8000, color: 'warning' } as ShowToast)
  }
}

function checkMailErrorCallback (response: any, errorCallback?: Function) {
  if (!errorCallback) return
  let mailerror: string | boolean = false

  if (response.headers) {
    mailerror = response.headers.get('mailerror') || false
  } else if (response.response.hasOwnProperty('headers')) {
    mailerror = response.response.headers.mailerror || false
  }
  if (mailerror) errorCallback(mailerror)
}
