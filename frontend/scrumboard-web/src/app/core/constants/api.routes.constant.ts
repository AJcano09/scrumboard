export const ApiRoutes = {
  Auth: {
    Login: '/auth/login',
    Register: '/auth/register',
    RefreshToken: '/auth/refresh'
  },
  Projects: {
    GetPaged: '/projects',
    GetById: (id: string) => `/projects/${id}`,
    Create: '/projects',
    Update:(id:string) => `/projects/${id}`,
    Delete:(id: string) => `/projects/${id}`,
  },
  Users: {
    Profile: '/users/profile'
  },
  Columns: {
    GetByProject: (projectId: string) => `/projects/${projectId}/columns`,
    Create: (projectId: string) => `/projects/${projectId}/columns`,
    Update: (projectId: string, id: string) => `/projects/${projectId}/columns/${id}`,
    Delete: (projectId: string, id: string) => `/projects/${projectId}/columns/${id}`,
    Reorder: (projectId: string) => `/projects/${projectId}/columns/reorder`,
  }
} as const;
