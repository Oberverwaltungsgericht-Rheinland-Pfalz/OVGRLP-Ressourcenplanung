
export default interface UserData {
  id: number,
  name: string,
  email: string,
  role: string | number,
  roleNames: string,
  organisation: string,
  supplierGroups: number[] | boolean
}

export interface ContactUser {
  Id: number
  Title: string
  Email: string
  Organisation: string
}
