
export default interface UserData {
  name: string,
  email: string,
  role: string | number,
  phone: string,
  domain: string,
  supplierGroups: number[] | boolean
}
