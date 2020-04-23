import { AdUsers } from '../models/interfaces'

export async function getUser (id: number): Promise<AdUsers|any> {
  try {
    const response = await fetch(`/api/Users/${id}`)
    let responseValues = await response.json() as AdUsers
    return responseValues
  } catch (ex) {
    return null
  }
}
