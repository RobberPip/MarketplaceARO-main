import { applyBarrier, createBarrier, createMutation, createQuery } from '@farfetched/core';
import { RouteInstance, RouteParams, RouteParamsAndQuery, chainRoute } from 'atomic-router';
import { combine, createEvent, createStore, sample } from 'effector';
import { not } from 'patronum';
import { createApiEffect } from './api/client';
import { router, routes } from './routing';
import { components } from './api/schema';

export const viewerQuery = createQuery({
  effect: createApiEffect('get', '/auth/current-user-info'),
});

export const checkViewerQuery = createQuery({
  effect: createApiEffect('get', '/auth/current-user-info'),
});

export const signInMutation = createMutation({
  effect: createApiEffect('post', '/auth/login', {
    mapParams: (args: components['schemas']['LoginDto']) => {
      return { body: args };
    },
  }),
});

export const logout = createEvent();

export const authorized = createEvent();
export const unauthorized = createEvent();

// TODO: emit on queries failures
export const forbidden = createEvent();

export const $viewer = combine([viewerQuery.$data, checkViewerQuery.$data], ([v1, v2]) => v1 ?? v2);

export const $authorized = createStore(false);

export const $next = createStore('/softwares');
export const $nextRoute = createStore<null | RouteInstance<{}>>(null);

$authorized.on(authorized, () => true).on(unauthorized, () => false);

// login
{
  sample({
    clock: [checkViewerQuery.finished.failure, signInMutation.finished.failure],
    filter: ({ error }) => {
      if ('status' in error) {
        return error.status === 401;
      }
      return false;
    },
    target: unauthorized,
  });
  {
    sample({
      clock: unauthorized,
      source: router.$path,
      filter: not(routes.signin.$isOpened),
      target: $next,
    });
    sample({
      clock: unauthorized,
      target: routes.signin.open,
    });
  }
  sample({
    clock: [viewerQuery.finished.success, checkViewerQuery.finished.success, signInMutation.finished.success],
    target: authorized,
  });
}

// Logout
sample({
  clock: logout,
  // TODO
  target: createApiEffect('post', '/auth/logout'),
});
$next.reset(logout);
$authorized.reset(logout);

export function chainAuthorized<Params extends RouteParams>(route: RouteInstance<Params>) {
  const sessionCheckStarted = createEvent<RouteParamsAndQuery<Params>>();

  const alreadyAuthorized = sample({
    clock: sessionCheckStarted,
    filter: $authorized,
  });

  sample({
    clock: sessionCheckStarted,
    filter: not($authorized),
    fn: () => {},
    target: checkViewerQuery.start,
  });

  return chainRoute({
    route,
    beforeOpen: sessionCheckStarted,
    openOn: [alreadyAuthorized, authorized],
  });
}

const authBarrier = createBarrier({
  active: not($authorized),
});

export function withViewer<Params extends RouteParams>(route: RouteInstance<Params>) {
  sample({
    clock: route.opened,
    filter: not($authorized),
    fn: () => ({}),
    target: viewerQuery.start,
  });
  return route;
}

type RemoteOperation = Parameters<typeof applyBarrier>[0][0];

export function applyAuthBarrier(operation: RemoteOperation | RemoteOperation[]) {
  const operations = Array.isArray(operation) ? operation : [operation];
  for (const operation of operations) {
    sample({
      clock: operation.finished.failure,
      filter: ({ error }) => {
        return (error as any)?.status === 401;
      },
      target: unauthorized,
    });
    sample({
      clock: operation.finished.failure,
      filter: ({ error }) => {
        return (error as any)?.status === 403;
      },
      target: forbidden,
    });
  }

  applyBarrier(operations, { barrier: authBarrier });
}
