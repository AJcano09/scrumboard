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
        path:'projects/:id/columns',
        loadComponent:()=> import('./features/Columns/pages/columns.component')
          .then(m=> m.ColumnsComponent)
      },
      {
        path: 'projects',
        loadComponent: () => import('./features/projects/pages/projects.list.component')
          .then(m => m.ProjectsListComponent)
      },
      {
        path:'board',
        loadComponent:()=> import('./features/board/pages/board.component')
          .then(m=> m.BoardComponent)
      },
      {
        path:'',
        redirectTo: 'board',
        pathMatch: 'full'

      }
    ]
  }
];
