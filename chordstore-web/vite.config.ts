import { defineConfig, loadEnv } from 'vite';
import react from '@vitejs/plugin-react';

// Em desenvolvimento, /api/* é encaminhado para o ASP.NET Core (sem problemas de CORS).
export default defineConfig(({ mode }) => {
  const env = loadEnv(mode, process.cwd(), '');
  return {
    plugins: [react()],
    server: {
      port: 5173,
      proxy: {
        '/api': {
          target: env.VITE_API_PROXY || 'http://localhost:5079',
          changeOrigin: true,
          secure: false, // certificado de desenvolvimento do .NET
        },
      },
    },
  };
});
