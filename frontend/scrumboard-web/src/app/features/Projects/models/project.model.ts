export enum ProjectStatus {
  Planificado = 'Planificado',
  EnProgreso = 'EnProgreso',
  Completado = 'Completado',
  Cancelado = 'Cancelado'
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
