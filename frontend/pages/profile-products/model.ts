import { createMutation, createQuery } from '@farfetched/core';
import { createApiEffectWithContract } from '~/shared/api/client.ts';
import { createEvent, createStore } from 'effector';
import { sample } from 'effector';
import { $viewer, applyAuthBarrier, chainAuthorized } from '~/shared/auth.ts';
import { routes } from '~/shared/routing.ts';
import { components } from '~/shared/api/schema';
import { changeAccountMutation } from '../profile-account/model';

export const profileProductsRoute = chainAuthorized(routes.profile.products);

export const productsQuery = createQuery({
  ...createApiEffectWithContract('get', '/api/v1/account/products'),
  mapData: (data: { result: components['schemas']['GetProductOut'][] }) => data.result,
});
applyAuthBarrier(productsQuery);
export const deleteProductMutation = createMutation({
  ...createApiEffectWithContract('delete', '/api/v1/product/{id}', {
    mapParams: (args: { id: string }) => {
      return {
        params: { path: { id: args.id } },
      };
    },
  }),
});
applyAuthBarrier(deleteProductMutation);
export const setProductName = createEvent<string>();
export const setSelectedType = createEvent<components['schemas']['ProductTypeOut']>();

export const $product = createStore<{
  name: string;
  type: components['schemas']['ProductTypeOut'];
}>({
  name: '',
  type: {
    id: '',
    fullName: '',
    shortName: '',
    combinedName: '',
  },
})
  .on(setProductName, (state, newName) => ({ ...state, name: newName }))
  .on(setSelectedType, (state, newType) => ({
    ...state,
    type: { ...state.type, ...newType },
  }));
export const $productsList = productsQuery.$data.map((products) => products ?? []);
sample({
  clock: [profileProductsRoute.opened, changeAccountMutation.finished.success],
  source: $viewer,
  filter: (viewer) => Boolean(viewer?.activeAccountId),
  fn: () => {},
  target: productsQuery.start,
});
export const setIsVisibleModal = createEvent<boolean>();

export const $isVisibleModal = createStore(false).on(setIsVisibleModal, (_, payload) => payload);

sample({
  clock: deleteProductMutation.finished.success,
  fn: () => {},
  target: productsQuery.start,
});
