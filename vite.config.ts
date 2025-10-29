import { defineConfig } from 'vite';
import react from '@vitejs/plugin-react';
import { babel } from '@rollup/plugin-babel';
import path from 'node:path';
import { buildIcons } from './tools/icons';
import { updateRazorAssets, prepareRasorIndex } from './tools/razor';

// https://vitejs.dev/config/
export default defineConfig({
  root: 'frontend',
  css: {
    modules: {
      localsConvention: 'camelCase',
    },
    preprocessorOptions: {
      scss: {
        api: 'modern',
        silenceDeprecations: ['legacy-js-api', 'mixed-decls', 'color-functions', 'global-builtin', 'import'],
      },
    },
  },
  build: {
    emptyOutDir: true,
    outDir: 'dist',
    rollupOptions: {
      input: {
        app: 'frontend/razor.html',
        main: 'frontend/index.html',
        'profile-widget': 'frontend/profile-widget.html',
      },
      output: {
        // manualChunks: (id, meta) => {
        //   const name = id.replace(`${__dirname}${path.sep}`, '');
        //   if (name.includes('primereact')) {
        //     if (
        //       name.includes('calendar') ||
        //       name.includes('datatable') ||
        //       name.includes('tree') ||
        //       name.includes('galleria')
        //     ) {
        //       return 'ui-extra';
        //     }
        //     return 'ui';
        //   }
        //   if (name.includes('react-dom@') || name.includes('react@')) {
        //     return 'react';
        //   }
        // },
      },
    },
  },
  plugins: [
    react(),
    babel({
      include: ['./frontend/**'],
      extensions: ['.ts', '.tsx'],
      babelHelpers: 'bundled',
    }),
    prepareRasorIndex,
    buildIcons(),
    updateRazorAssets(),
  ],
  clearScreen: false,
  resolve: {
    alias: [
      {
        find: '~',
        replacement: path.resolve('frontend'),
      },
    ],
  },
  server: {
    port: 4000,
    proxy: {
      '/api': {
        target: 'http://localhost:5000',
        changeOrigin: true,
      },
      '/ws': {
        target: 'http://localhost:5000',
        changeOrigin: true,
      },
      '/auth': {
        target: 'http://localhost:5000',
        changeOrigin: true,
      },
    },
  },
});
