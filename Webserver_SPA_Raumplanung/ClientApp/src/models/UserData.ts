
export default interface UserData {
  name: string,
  email: string,
  role: string | boolean,
  phone: string,
  domain: string,
  supplierGroups: number[] | boolean
}
