import { querySync } from 'atomic-router';
import { createEvent, createStore, sample } from 'effector';
import { reset } from 'patronum';
import { viewerQuery, withViewer } from '~/shared/auth';
import { routes, controls } from '~/shared/routing.ts';

export const docsRoute = withViewer(routes.docs);
export const docNameChanged = createEvent<
  'user-agreement' | 'personal-data' | 'privacy' | 'terms' | 'terms-use' | ''
>();
export const $docName = createStore<'user-agreement' | 'personal-data' | 'privacy' | 'terms' | 'terms-use' | ''>('').on(
  docNameChanged,
  (_, payload) => payload,
);

querySync({
  source: { name: $docName },
  controls,
});
sample({
  clock: docsRoute.navigate,
  fn: () => '' as const,
  target: $docName,
});
sample({
  clock: docsRoute.opened,
  target: viewerQuery.start,
});
