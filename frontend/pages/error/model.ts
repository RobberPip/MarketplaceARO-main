import { createEvent, createStore, sample } from 'effector';
import { withViewer } from '~/shared/auth';
import { router, routes } from '~/shared/routing';

export const errorRoute = withViewer(routes.error);
export const showError = createEvent<{ code: string; title: string; description?: string }>();
export const $hasError = createStore(false);
export const $error = createStore<{ code: string; title: string; description?: string }>({
  code: '404',
  title: 'Страница не найдена',
  description: 'Страница была удалена или её адрес поменялся. Проверьте адрес или перейдите на главную страниц',
});

sample({
  clock: showError,
  target: [$error, routes.error.open],
});

sample({
  clock: router.routeNotFound,
  source: $error,
  target: routes.error.open,
});
