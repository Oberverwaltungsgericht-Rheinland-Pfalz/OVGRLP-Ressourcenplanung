export async function getUser (id: number): Promise<WebApi.AdUserViewModel|any> {
  try {
    const response = await fetch(`/api/Users/${id}`)
    let responseValues = await response.json() as WebApi.AdUserViewModel
    return responseValues
  } catch (ex) {
    return null
  }
}
