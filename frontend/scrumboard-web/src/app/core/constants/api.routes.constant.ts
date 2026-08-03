export const ApiRoutes = {
  Auth: {
    Login: '/auth/login',
    Register: '/auth/register',
    RefreshToken: '/auth/refresh'
  },
  Projects: {
    GetAll: '/projects',
    GetById: (id: string) => `/projects/${id}`,
    Create: '/projects',
  },
  Users: {
    Profile: '/users/profile'
  }
} as const;
