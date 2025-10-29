import { sample } from 'effector';
import { viewerQuery } from '~/shared/auth';
import { routes } from '~/shared/routing';

export const rootRoute = routes.home;

sample({
  clock: rootRoute.opened,
  target: viewerQuery.start,
});
