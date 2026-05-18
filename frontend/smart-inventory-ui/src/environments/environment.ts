import { resolveRuntimeEnv } from './runtime-config';

const defaults = {
  production: true,
  apiUrl: 'http://localhost:8080/api',
  appName: 'Smart Inventory',
};

export const environment = resolveRuntimeEnv(defaults);
