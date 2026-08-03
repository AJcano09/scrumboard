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
  }
} as const;
