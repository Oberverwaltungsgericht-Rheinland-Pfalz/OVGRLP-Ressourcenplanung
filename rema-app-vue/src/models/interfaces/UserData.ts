export interface UserData {
  id: number,
  name: string,
  email: string,
  role: string | number,
  roleNames: string,
  organisation: string,
  supplierGroups: number[] | boolean
}
