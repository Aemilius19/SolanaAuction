import { defineConfig } from 'vite';
import path from 'path';

export default defineConfig({
  optimizeDeps: {
    include: [
      '@solana/web3.js',
      '@coral-xyz/anchor',
      '@solana/spl-token',
      'text-encoding',
      'buffer'
    ],
  },
  resolve: {
    alias: {
      '@solana/web3.js': path.resolve(__dirname, 'node_modules/@solana/web3.js'),
      '@coral-xyz/anchor': path.resolve(__dirname, 'node_modules/@coral-xyz/anchor'),
      '@solana/spl-token': path.resolve(__dirname, 'node_modules/@solana/spl-token'),
      'text-encoding': path.resolve(__dirname, 'node_modules/text-encoding'),
      'buffer': path.resolve(__dirname, 'node_modules/buffer')
    }
  }
});
