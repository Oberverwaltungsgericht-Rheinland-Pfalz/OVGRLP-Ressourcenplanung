
export default interface UserData {
  name: string,
  email: string,
  role: string | number,
  roleNames: string,
  organisation: string,
  supplierGroups: number[] | boolean
}
