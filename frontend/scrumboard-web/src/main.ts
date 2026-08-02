import { bootstrapApplication } from '@angular/platform-browser';
import { appConfig } from './app/app.config';
import { AppComponent } from './app/app.component';

const config = {
  ...appConfig,
  providers: [
    ...(appConfig.providers || []),
    {
      provide: 'APP_CONFIG',
      useValue: (window as any).__env || {},
    },
  ],
};

bootstrapApplication(AppComponent, config)
  .catch((err) => console.error(err));
