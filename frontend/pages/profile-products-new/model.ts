import { chainAuthorized } from '~/shared/auth.ts';
import { routes } from '~/shared/routing.ts';
import { createEvent, createStore, sample } from 'effector';
import { productsOptionsQuery } from '~/widgets/product-form/model';

export const profileProductsNewRoute = chainAuthorized(routes.profile['products-new']);

export const setOpenedDirtyPage = createEvent<boolean>();
export const openedDirtyPage = createStore(true).on(setOpenedDirtyPage, (_, payload) => payload);

sample({
  clock: profileProductsNewRoute.opened,
  filter: () => openedDirtyPage.getState(),
  fn: () => {},
  target: routes.profile.products.open,
});
