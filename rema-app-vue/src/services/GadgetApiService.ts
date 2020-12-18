import { Allocation } from '../models/Allocation'
import { Gadget } from '../models/Gadget'

const config = {
  method: 'PUT', // *GET, POST, PUT, DELETE, etc.
  mode: 'cors', // no-cors, cors, *same-origin
  cache: 'no-cache', // *default, no-cache, reload, force-cache, only-if-cached
  credentials: 'same-origin', // include, *same-origin, omit
  headers: {
    'Content-Type': 'application/json'
  } }

export async function submitGadget (newGadget: WebApi.GadgetViewModel): Promise<boolean> {
  try {
    const response = await Gadget.api().post('gadgets', newGadget)
    if (response.response.status > 300 && response.response.status < 200) return false
    else return true
  } catch (ex) {
    return false
  }
}

export async function editGadget (gadget: WebApi.GadgetViewModel) : Promise<boolean> {
  try {
    const response = await Gadget.api().put(`gadgets/${gadget.Id}`, gadget)

    if (response.response.status > 300 && response.response.status < 200) return false
    else return true
  } catch (ex) {
    return false
  }
}

export async function refreshGadgets () : Promise<boolean> {
  try {
    let response = await Gadget.api().get('gadgets')

    if (response.response.status > 300 && response.response.status < 200) return false
    else return true
  } catch (ex) {
    return false
  }
}

export async function deleteGadget (id: number) : Promise<boolean> {
  try {
    const response = await Gadget.api().delete(`gadgets/${id}`, { delete: id })

    if (response.response.status > 300 && response.response.status < 200) return false
    else return true
  } catch (ex) {
    return false
  }
}
