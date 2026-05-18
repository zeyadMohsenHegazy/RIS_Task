import { resolveRuntimeEnv } from './runtime-config';

const defaults = {
  production: true,
  apiUrl: 'http://localhost:8080/api',
  appName: 'Raya International Services',
};

export const environment = resolveRuntimeEnv(defaults);
