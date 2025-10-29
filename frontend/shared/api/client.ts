import createFetchClient from 'openapi-fetch';
import type { paths } from './schema';
import { createClient } from 'openapi-ff';

export const client = createFetchClient<paths>({});

export const { createApiEffect, createApiEffectWithContract } = createClient(client, {
  createContract(method, path) {
    // TODO
    return {
      isData(d: unknown): d is unknown {
        return true;
      },
      getErrorMessages: () => [],
    };
  },
});
