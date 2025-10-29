import { startChain } from '@farfetched/atomic-router';
import { createQuery } from '@farfetched/core';
import { chainRoute } from 'atomic-router';
import { sample } from 'effector';
import { spread } from 'patronum';
import { ToastMessage } from 'primereact/toast';
import { createApiEffectWithContract } from '~/shared/api/client';
import { showToast } from '~/shared/notifications';
import { routes } from '~/shared/routing';
import { showError } from '../error/model';

export const productQuery = createQuery({
  ...createApiEffectWithContract('get', '/api/v1/product/{id}', {
    mapParams: ({ id }: { id: string }) => {
      return { params: { path: { id } } };
    },
  }),
  mapData: ({ result }) => result,
});

export const productRoute = chainRoute({
  route: routes.product.item,
  ...startChain(productQuery),
});

// sample({
//   clock: softwareQuery.$error,
//   filter: softwareQuery.$failed,
//   fn: (e) => {
//     const error = e!;
//     return {
//       notification: {
//         severity: 'error',
//         summary: error.explanation ?? 'Unknown Error',
//         detail: error.errorType,
//       } satisfies ToastMessage,
//       error: { title: error.errorType, description: error.explanation },
//     };
//   },
//   target: spread({
//     notification: showToast,
//     error: showError,
//   }),
// });
