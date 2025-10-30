// vite.config.ts
import { defineConfig } from 'vite';
import react from '@vitejs/plugin-react';

export default defineConfig({
  plugins: [react()],
  server: {
    // fallback to index.html for SPA routing
    // this is the correct way in Vite 7+
    open: true, // optional, automatically opens browser
  },
  build: {
    rollupOptions: {
      input: '/index.html'
    }
  }
});
