import { Routes } from '@angular/router';
import {AppLayoutComponent} from "./layout/app.layout.component";
import {authGuard} from "./core/auth/auth.guard";

export const routes: Routes = [
  {
    path:'login',
    loadComponent:()=> import('./features/auth/login.component')
      .then(m=> m.LoginComponent)
  },
  {
    path: '',
    component: AppLayoutComponent,
    canActivate:[authGuard],
    children: [
      {
        path:'',
        redirectTo: 'projects',
        pathMatch: 'full'

      },
      {
        path: 'projects',
        loadComponent: () => import('./features/Projects/pages/projects.list.component')
          .then(m => m.ProjectsListComponent)
      },
      {
        path: 'projects/:id/board',
        loadComponent: () => import('./features/Projects/pages/project-board.component')
          .then(m => m.ProjectBoardComponent)
      },
      {
        path:'projects/:id/columns',
        loadComponent:()=> import('./features/Columns/pages/columns.component')
          .then(m=> m.ColumnsComponent)
      },
      {
        path: '**',
        redirectTo: 'projects'
      }
    ]
  }
];
