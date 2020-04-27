import { Allocation } from '../models/Allocation'

export async function editAllocationStatus (Id: number, status: number, From: string, To: string) : Promise<boolean> {
  const editedRequest = { Id, status, From, To }
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
    return response.ok
  } catch (ex) {
    return false
  }
}

export async function submitAllocations (newAllocations: object): Promise<boolean> {
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
