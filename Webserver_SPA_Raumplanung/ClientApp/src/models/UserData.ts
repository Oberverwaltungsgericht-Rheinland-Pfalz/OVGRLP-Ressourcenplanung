
export default interface UserData {
  name: string,
  email: string,
  role: string | number,
  organisation: string,
  supplierGroups: number[] | boolean
}
