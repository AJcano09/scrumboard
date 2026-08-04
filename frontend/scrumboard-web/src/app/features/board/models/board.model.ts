export type TaskPriority = 'Baja' | 'Media' | 'Alta';

export interface BoardTask {
  id: string;
  title: string;
  description: string;
  priority: TaskPriority;
  responsibleId: string;
  responsibleName: string;
  columnId: string;
  order: number;
  createdAt: string;
}

export interface BoardColumn {
  id: string;
  name: string;
  order: number;
  tasks: BoardTask[];
}

export interface Board {
  projectId: string;
  columns: BoardColumn[];
}

export interface User {
  id: string;
  name: string;
  email: string;
}

