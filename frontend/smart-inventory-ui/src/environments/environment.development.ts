import { resolveRuntimeEnv } from './runtime-config';

const defaults = {
  production: false,
  apiUrl: 'http://localhost:5000/api',
  appName: 'Smart Inventory (Dev)',
};

export const environment = resolveRuntimeEnv(defaults);
