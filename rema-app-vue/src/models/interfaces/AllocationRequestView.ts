export interface AllocationRequestView {
  Id: number
  From: string
  To: string
  IsAllDay: boolean
  Status: number
  CreatedAt: string
  CreatedById: number
  LastModified: string
  ApprovedAt: string
  ReferencePerson: string
  ReferencePersonId: number
  RessourceIds: Array<number>
  Title: string
  RessourceTitles: string
  ContactTel: string
  Description: string
  Notices: string
}

export interface ShortAllocationView {
  Id: number
  From: string
  To: string
  RessourceIds: Array<number>
  dates: string[] | null
}
export interface InitAllocation {
  From: string
  RessourceIds: Array<number>
  Day: string
}
