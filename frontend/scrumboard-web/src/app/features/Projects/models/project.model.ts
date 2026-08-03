export enum ProjectStatus {
  Pending = 'Pending',
  InProgres = 'InProgres',
  Completed = 'Completed',
  Cancelled = 'Cancelled'
}

export interface Project {
  id: string;
  name: string;
  description: string;
  startDate: string;
  endDate: string;
  status: ProjectStatus;
}

export interface ProjectFormValue {
  name: string;
  description: string;
  startDate: string;
  endDate: string;
  status: ProjectStatus;
}

export interface PagedResult<T> {
  items: T[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
  totalPages: number;
}
