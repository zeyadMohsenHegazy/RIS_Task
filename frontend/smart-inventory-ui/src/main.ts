import { bootstrapApplication } from '@angular/platform-browser';
import { appConfig } from './app/app.config';
import { App } from './app/app';

bootstrapApplication(App, appConfig).catch((err) => {
  console.error(err);
  document.body.innerHTML =
    '<div style="font-family:Roboto,sans-serif;padding:2rem;max-width:40rem">' +
    '<h1>Application failed to start</h1>' +
    '<p>Check the browser console for details, then refresh the page.</p>' +
    '</div>';
});
