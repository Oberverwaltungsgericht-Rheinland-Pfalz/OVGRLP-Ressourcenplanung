import { Allocation } from '../models/Allocation'

export async function editAllocationStatus (Id: number, status: number, From: string, To: string) : Promise<boolean> {
  const editedRequest = { Id, status, From, To }
  try {
    const response = await fetch(`/api/allocations/editRequest`, {
      method: 'PUT', // *GET, POST, PUT, DELETE, etc.
      mode: 'cors', // no-cors, cors, *same-origin
      cache: 'no-cache', // *default, no-cache, reload, force-cache, only-if-cached
      credentials: 'same-origin', // include, *same-origin, omit
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify(editedRequest)
    })
    return response.ok
  } catch (ex) {
    return false
  }
}

export async function submitAllocations (newAllocations: object): Promise<boolean> {
  try {
    const response = await fetch(`/api/Allocations/PostAllocations`, {
      method: 'POST', // *GET, POST, PUT, DELETE, etc.
      mode: 'cors', // no-cors, cors, *same-origin
      cache: 'no-cache', // *default, no-cache, reload, force-cache, only-if-cached
      credentials: 'same-origin', // include, *same-origin, omit
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify(newAllocations)
    })
    return response.ok
  } catch (ex) {
    return false
  }
}

export async function submitAllocation (newAllocation: object): Promise<boolean> {
  try {
    let response = await Allocation.api().post('allocations', newAllocation)
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

export async function deleteAllocation (id: number) : Promise<boolean> {
  try {
    const response = await Allocation.api().delete(
      `allocations/${id}`,
      { delete: id }
    )

    if (response.response.status > 300 && response.response.status < 200) return false
    else return true
  } catch (ex) {
    return false
  }
}

export async function editAllocation (allocation: any) : Promise<boolean> {
  try {
    const response = await Allocation.api().put(`allocations/edit/${allocation.Id}`, allocation)

    if (response.response.status > 300 && response.response.status < 200) return false
    else return true
  } catch (ex) {
    return false
  }
}
