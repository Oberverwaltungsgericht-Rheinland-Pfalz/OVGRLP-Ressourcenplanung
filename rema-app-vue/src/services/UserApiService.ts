export async function getUser (id: number): Promise<WebApi.AdUserViewModel|null> {
  try {
    const response = await fetch(`/api/Users/${id}`)
    let responseValues = await response.json() as WebApi.AdUserViewModel
    return responseValues
  } catch (ex) {
    return null
  }
}
