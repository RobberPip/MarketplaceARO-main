import { createHistoryRouter, createRoute, createRouterControls } from 'atomic-router';
import { sample } from 'effector';
import { createBrowserHistory } from 'history';
import { showError } from '~/pages/error';
import { appStarted } from '~/shared/config';

export const routes = {
  home: createRoute(),
  profile: {
    self: createRoute(),
    account: createRoute(),
    subscriptions: createRoute(),
    products: createRoute(),
    'products-new': createRoute(),
    finances: createRoute(),
  },
  forum: createRoute(),
  signin: createRoute(),
  signup: createRoute(),
  resetpass: createRoute(),
  docs: createRoute(),
  todos: createRoute(),
  product: {
    item: createRoute<{ id: string }>(),
  },

  error: createRoute(),
};

export const routesMap = [
  { path: '/', route: routes.home },
  { path: '/profile', route: routes.profile.self },
  { path: '/profile/account', route: routes.profile.account },
  { path: '/profile/subscriptions', route: routes.profile.subscriptions },
  { path: '/profile/products', route: routes.profile.products },
  { path: '/profile/products/new', route: routes.profile['products-new'] },
  { path: '/profile/finances', route: routes.profile.finances },
  { path: '/forum', route: routes.forum },
  { path: '/login', route: routes.signin },
  { path: '/resetpass', route: routes.resetpass },
  { path: '/signup', route: routes.signup },
  { path: '/todos', route: routes.todos },
  { path: '/product/:id', route: routes.product.item },
  { path: '/error', route: routes.error },
  { path: '/docs', route: routes.docs },
];

export const controls = createRouterControls();

export const router = createHistoryRouter({
  base: import.meta.env.PROD ? import.meta.env.BASE_URL : undefined,
  controls,
  routes: routesMap,
});

sample({
  clock: appStarted,
  fn: () => createBrowserHistory(),
  target: router.setHistory,
});
