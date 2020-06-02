export interface UserData {
  id: number,
  name: string,
  email: string,
  role: string | number,
  roleNames: string,
  supplierGroups: number[] | boolean
  organisation: string
}
